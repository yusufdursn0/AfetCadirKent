using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.TalepYonetimi;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.Raporlama;

/// <summary>Rapor sütunu; genişlik diğer sütunlara göre oranlıdır.</summary>
public sealed record RaporSutunu(string Baslik, float Genislik, bool Sayisal = false);

/// <summary>
/// Tablo biçimli rapor içeriği. İçeriği iş katmanı hazırlar; arayüz yalnızca XtraReports ile gösterir ve PDF/Excel'e aktarır.
/// </summary>
public sealed record TabloRaporu
{
    public required string Baslik { get; init; }
    public IReadOnlyList<string> AltBasliklar { get; init; } = [];
    public required IReadOnlyList<RaporSutunu> Sutunlar { get; init; }
    public required IReadOnlyList<IReadOnlyList<string>> Satirlar { get; init; }
    public IReadOnlyList<string> Ozet { get; init; } = [];

    /// <summary>Tutanakta imza alanları gösterilir (ör. "Teslim eden", "Onaylayan").</summary>
    public IReadOnlyList<string> ImzaAlanlari { get; init; } = [];

    public required string Hazirlayan { get; init; }
    public required DateTime HazirlanmaZamani { get; init; }
}

/// <summary>Kayıt sonunda yazdırılan aile fişi (şartname M2): aile numarası, çadır ve kiosk giriş bilgisi.</summary>
public sealed record AileFisi(string AileNo, string AileAdi, DateTime KayitZamani, IReadOnlyList<string> Uyeler, IReadOnlyList<string> Cadirlar,
    string KioskBilgisi, DateTime YazdirmaZamani);

public interface IRaporServisi
{
    /// <summary>Dağıtım tutanağı: listedeki aileler, hak, teslim edilen, bekleyen, teslim alan/eden ve imza alanları.</summary>
    Task<TabloRaporu> DagitimTutanagiAsync(int dagitimId, CancellationToken iptal = default);

    /// <summary>Stok raporu: ürün bazında depodaki, ayrılan, kullanılabilir, SKT'si geçmiş miktar ve kritik seviye.</summary>
    Task<TabloRaporu> StokRaporuAsync(CancellationToken iptal = default);

    /// <summary>Nüfus raporu: durum, yaş grubu, cinsiyet, destek önceliği ve bölge dağılımı.</summary>
    Task<TabloRaporu> NufusRaporuAsync(CancellationToken iptal = default);

    /// <summary>Talep raporu: tarih aralığında açılan talepler, tür/durum dağılımı ve ortalama kapanma süresi.</summary>
    Task<TabloRaporu> TalepRaporuAsync(DateOnly baslangic, DateOnly bitis, CancellationToken iptal = default);

    /// <summary>Aile fişi. Rapor izni gerektirmez; kayıt yapan görevli yazdırır ("Kayıt" görme izni).</summary>
    Task<AileFisi> AileFisiAsync(int aileId, CancellationToken iptal = default);
}

/// <summary>
/// Raporlar (şartname M13, Ekran 16). Her rapor ilgili modülün "Rapor" iznini ister (K-059: Koordinatör tümü, Depo sorumlusu
/// depo ve dağıtım). Veriler mevcut servislerden okunur; görünürlük kuralları (ör. refakatsiz çocuk) o servislerde uygulanır.
/// </summary>
public class RaporServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman, GerekceDegerlendirici degerlendirici,
    IDagitimServisi dagitimServisi, IStokServisi stokServisi, ITalepServisi talepServisi, IKayitServisi kayitServisi) : IRaporServisi
{
    private string Hazirlayan => oturum.Kullanici is { } k ? $"{k.AdSoyad} ({k.KullaniciAdi})" : "—";

    private static string Sayi(decimal deger) => deger.ToString("#,##0.##");

    public async Task<TabloRaporu> DagitimTutanagiAsync(int dagitimId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Rapor);
        var liste = await dagitimServisi.ListeGetirAsync(dagitimId, iptal);
        var o = liste.Ozet;
        var birim = o.Birim.Metin();
        var satirlar = liste.Satirlar.Where(s => s.Aktif).Select(s => (IReadOnlyList<string>)
        [
            s.AileNo, s.AileAdi, s.HakSahibiKisiSayisi.ToString(), Sayi(s.HakMiktari), Sayi(s.TeslimEdilenMiktar), Sayi(s.BekleyenHak),
            string.Join(", ", s.Teslimler.Select(t => t.TeslimAlan).Distinct()),
            string.Join(", ", s.Teslimler.Select(t => t.TeslimEden).Distinct()),
            s.Teslimler.Count == 0 ? "—" : s.Teslimler.Max(t => t.Zaman).ToString("dd.MM HH:mm"),
            "",
        ]).ToList();

        var cikarilan = liste.Satirlar.Count(s => !s.Aktif);
        return new TabloRaporu
        {
            Baslik = "Dağıtım Tutanağı",
            AltBasliklar =
            [
                $"Ürün: {o.UrunAdi} ({birim}) · Tarih: {o.Tarih:dd.MM.yyyy} · Liste no: {o.Id} · Durum: {o.Durum.Metin()}",
                .. o.Aciklama is { Length: > 0 } a ? [$"Açıklama: {a}"] : Array.Empty<string>(),
            ],
            Sutunlar =
            [
                new("Aile no", 9), new("Aile", 14), new("Hak sahibi", 7, true), new("Hak", 6, true), new("Teslim", 7, true),
                new("Bekleyen", 8, true), new("Teslim alan", 14), new("Teslim eden", 11), new("Son teslim", 9), new("İmza", 10),
            ],
            Satirlar = satirlar,
            Ozet =
            [
                $"Aile sayısı: {satirlar.Count}" + (cikarilan > 0 ? $" (listeden çıkarılan {cikarilan} satır tutanakta yok)" : ""),
                $"Toplam hak: {Sayi(o.ToplamHak)} {birim} · Teslim edilen: {Sayi(o.ToplamTeslim)} {birim} · Bekleyen: {Sayi(o.ToplamBekleyen)} {birim}",
                "Kısmi teslimatta kalan miktar ailenin bekleyen hakkıdır ve sonraki listeye devreder.",
            ],
            ImzaAlanlari = ["Teslim eden (depo sorumlusu)", "Onaylayan (koordinatör)"],
            Hazirlayan = Hazirlayan, HazirlanmaZamani = zaman.Simdi(),
        };
    }

    public async Task<TabloRaporu> StokRaporuAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Rapor);
        var durum = await stokServisi.StokDurumuAsync(iptal);
        var satirlar = durum.Select(u =>
        {
            var partiler = u.Partiler.Where(p => p.Miktar > 0).ToList();
            var enYakin = partiler.Where(p => p.SonKullanmaTarihi is not null && !p.SktGecti).Min(p => p.SonKullanmaTarihi);
            return (IReadOnlyList<string>)
            [
                u.UrunAdi, u.Birim.Metin(), Sayi(u.FizikselMiktar), Sayi(u.AyrilanMiktar), Sayi(u.KullanilabilirMiktar), Sayi(u.SktGecmisMiktar),
                Sayi(u.KritikSeviye), partiler.Count.ToString(), enYakin?.ToString("dd.MM.yyyy") ?? "—",
                u.KritikAltinda ? "Kritik seviyenin altında" : "Yeterli",
            ];
        }).ToList();

        var yaklasan = durum.Sum(u => u.Partiler.Count(p => p.SktYaklasiyor && p.Miktar > 0));
        var gecmis = durum.Sum(u => u.Partiler.Count(p => p.SktGecti && p.Miktar > 0));
        return new TabloRaporu
        {
            Baslik = "Stok Raporu",
            AltBasliklar = [$"Aktif ürünler · {zaman.Simdi():dd.MM.yyyy HH:mm} itibarıyla"],
            Sutunlar =
            [
                new("Ürün", 16), new("Birim", 6), new("Depodaki", 9, true), new("Ayrılan", 8, true), new("Kullanılabilir", 10, true),
                new("SKT geçmiş", 8, true), new("Kritik seviye", 9, true), new("Parti", 5, true), new("En yakın SKT", 9), new("Durum", 14),
            ],
            Satirlar = satirlar,
            Ozet =
            [
                $"Ürün sayısı: {durum.Count} · Kritik seviyenin altında: {durum.Count(u => u.KritikAltinda)}",
                $"SKT'si yaklaşan parti: {yaklasan} · SKT'si geçmiş (fireye düşülmemiş) parti: {gecmis}",
                "Ayrılan: dağıtım listesi onaylanmış, henüz teslim edilmemiş miktar. Kullanılabilir = depodaki − ayrılan − SKT'si geçmiş.",
            ],
            Hazirlayan = Hazirlayan, HazirlanmaZamani = zaman.Simdi(),
        };
    }

    public async Task<TabloRaporu> NufusRaporuAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Rapor);
        await using var veri = veriFabrikasi.Ac();
        var bugun = zaman.Bugun();
        var simdi = zaman.Simdi();

        var kisiler = await veri.Repo<Afetzede>().Sorgu().AsNoTracking().Where(k => k.Aktif)
            .Select(k => new { k.AileId, k.Durum, k.DogumTarihi, k.Cinsiyet }).ToListAsync(iptal);
        var burada = kisiler.Where(k => k.Durum is KisiDurumu.Burada or KisiDurumu.Hastanede).ToList();
        var aileIdler = burada.Select(k => k.AileId).Distinct().ToList();
        var oncelikler = await GerekceServisi.DegerlendirAsync(veri, aileIdler, simdi, degerlendirici, iptal);
        var bolgeler = await veri.Repo<Yerlesim>().Sorgu().AsNoTracking().Where(y => y.Bitis == null)
            .GroupBy(y => new { y.Cadir!.Bolge!.Kod, y.Cadir.Bolge.Ad })
            .Select(g => new { g.Key.Kod, g.Key.Ad, Kisi = g.Count() }).OrderBy(g => g.Kod).ToListAsync(iptal);
        // Ana ekranla aynı hesap: yalnızca kullanılabilir çadırların kapasitesi ve boş yatağı.
        var yatak = await DurumSayaclari.YatakAsync(veri, iptal);
        var bekleyen = await DurumSayaclari.BekleyenAileAsync(veri, iptal);

        var satirlar = new List<IReadOnlyList<string>>();
        void Ekle(string grup, string deger, int sayi, int toplam) =>
            satirlar.Add([grup, deger, sayi.ToString(), toplam == 0 ? "—" : $"%{100.0 * sayi / toplam:0.#}"]);

        foreach (var durum in Enum.GetValues<KisiDurumu>())
            Ekle("Kişi durumu", durum.Metin(), kisiler.Count(k => k.Durum == durum), kisiler.Count);
        foreach (var (ad, alt, ust) in new[] { ("0–2 yaş", 0, 2), ("3–12 yaş", 3, 12), ("13–17 yaş", 13, 17), ("18–64 yaş", 18, 64), ("65 yaş ve üstü", 65, 200) })
            Ekle("Yaş grubu (çadır kentte)", ad, burada.Count(k => Afetzede.YasHesapla(k.DogumTarihi, bugun) is var y && y >= alt && y <= ust), burada.Count);
        foreach (var cinsiyet in Enum.GetValues<Cinsiyet>())
            Ekle("Cinsiyet (çadır kentte)", cinsiyet.Metin(), burada.Count(k => k.Cinsiyet == cinsiyet), burada.Count);
        foreach (var oncelik in Enum.GetValues<DestekOnceligi>().OrderDescending())
            Ekle("Destek önceliği (aile)", oncelik.Metin(), oncelikler.Values.Count(o => o.DestekOnceligi == oncelik), aileIdler.Count);
        var cadirdaki = bolgeler.Sum(b => b.Kisi);
        foreach (var b in bolgeler)
            Ekle("Bölge (çadırdaki kişi)", $"{b.Kod} – {b.Ad}", b.Kisi, cadirdaki);

        return new TabloRaporu
        {
            Baslik = "Nüfus Raporu",
            AltBasliklar = [$"{simdi:dd.MM.yyyy HH:mm} itibarıyla · aktif kayıtlar"],
            Sutunlar = [new("Grup", 30), new("Değer", 30), new("Sayı", 15, true), new("Oran", 15, true)],
            Satirlar = satirlar,
            Ozet =
            [
                $"Çadır kentteki kişi: {burada.Count} ({aileIdler.Count} aile) · Kayıtlı toplam kişi: {kisiler.Count}",
                $"Kullanılabilir çadır kapasitesi: {yatak.ToplamKapasite} · Boş yatak: {yatak.BosYatak} · Bakımda/kapalı çadır: {yatak.KullanilamayanCadir} · Bekleme listesindeki aile: {bekleyen}",
                "Destek önceliği tıbbi değerlendirme değildir; ailenin gerekçelerine göre yerleşim ve takip önceliğidir.",
            ],
            Hazirlayan = Hazirlayan, HazirlanmaZamani = simdi,
        };
    }

    public async Task<TabloRaporu> TalepRaporuAsync(DateOnly baslangic, DateOnly bitis, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Talep, Izin.Rapor);
        if (bitis < baslangic)
            throw new DogrulamaException("Bitiş tarihi başlangıç tarihinden önce olamaz.");

        var bas = baslangic.ToDateTime(TimeOnly.MinValue);
        var bit = bitis.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var talepler = (await talepServisi.ListeleAsync(new TalepFiltresi(), iptal))
            .Where(t => t.AcilisZamani >= bas && t.AcilisZamani < bit).OrderBy(t => t.AcilisZamani).ToList();

        var satirlar = talepler.Select(t => (IReadOnlyList<string>)
        [
            t.Id.ToString(), t.AcilisZamani.ToString("dd.MM HH:mm"), t.Kaynak.Metin(), t.AileNo ?? "—", t.TurMetni,
            t.SaglikPersoneliDegerlendirecek ? "Sağlık personeli" : t.YardimOnceligi?.Metin() ?? "—", t.Durum.Metin(), t.SorumluAdi ?? "—",
            t.Aciklama.Kisalt(80), t.Sonuc?.Kisalt(80) ?? "",
        ]).ToList();

        var kapanan = talepler.Where(t => t.Durum == TalepDurumu.Tamamlandi && t.KapanisZamani is not null).ToList();
        var ortalama = kapanan.Count == 0 ? "hesaplanamıyor (tamamlanan talep yok)"
            : $"{kapanan.Average(t => (t.KapanisZamani!.Value - t.AcilisZamani).TotalHours):0.#} saat";
        var turler = string.Join(" · ", talepler.GroupBy(t => t.TurMetni).OrderBy(g => g.Key).Select(g => $"{g.Key}: {g.Count()}"));
        var durumlar = string.Join(" · ", talepler.GroupBy(t => t.Durum).OrderBy(g => g.Key).Select(g => $"{g.Key.Metin()}: {g.Count()}"));
        return new TabloRaporu
        {
            Baslik = "Talep Raporu",
            AltBasliklar = [$"Açılış tarihi: {baslangic:dd.MM.yyyy} – {bitis:dd.MM.yyyy}"],
            Sutunlar =
            [
                new("No", 4, true), new("Açılış", 8), new("Geldiği yer", 8), new("Aile", 8), new("Tür", 9), new("Yardım önceliği", 9),
                new("Durum", 8), new("Sorumlu", 10), new("Açıklama", 18), new("Sonuç", 18),
            ],
            Satirlar = satirlar,
            Ozet =
            [
                $"Toplam talep: {talepler.Count} · Açık: {talepler.Count(t => t.AcikMi)}",
                $"Türe göre: {(turler.Length == 0 ? "—" : turler)}",
                $"Duruma göre: {(durumlar.Length == 0 ? "—" : durumlar)}",
                $"Ortalama tamamlanma süresi: {ortalama}",
            ],
            Hazirlayan = Hazirlayan, HazirlanmaZamani = zaman.Simdi(),
        };
    }

    public async Task<AileFisi> AileFisiAsync(int aileId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        var aile = await kayitServisi.AileGetirAsync(aileId, iptal);
        if (!aile.Aktif)
            throw new IsKuraliException("Pasif ailenin fişi yazdırılamaz.");

        await using var veri = veriFabrikasi.Ac();
        var cadirlar = await veri.Repo<Yerlesim>().Sorgu().AsNoTracking()
            .Where(y => y.Bitis == null && y.Kisi!.AileId == aileId)
            .Select(y => new { y.Cadir!.Bolge!.Kod, y.Cadir.Sira, y.Cadir.Numara, BolgeAd = y.Cadir.Bolge.Ad })
            .Distinct().ToListAsync(iptal);

        var uyeler = aile.CadirKenttekiUyeler.OrderBy(u => u.Id).Select(u => $"{u.AdSoyad} ({u.KisiNo})").ToList();
        return new AileFisi(aile.AileNo, aile.AileAdi, aile.KayitZamani, uyeler,
            cadirlar.Count == 0 ? ["Henüz çadır verilmedi (kayıt masasına başvurun)"]
                : cadirlar.Select(c => $"{c.Kod}{c.Sira}-{c.Numara:00} ({c.BolgeAd})").ToList(),
            $"Kioskta aile numaranız ({aile.AileNo}) ve ailenizden çadır kentte bulunan birinin doğum yılıyla giriş yapabilirsiniz.",
            zaman.Simdi());
    }
}
