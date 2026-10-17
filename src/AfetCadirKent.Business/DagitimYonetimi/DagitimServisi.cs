using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Gerekceler;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.DagitimYonetimi;

public interface IDagitimServisi
{
    /// <summary>Dağıtım listeleri (en yeni önce); tarih verilirse yalnızca o gün.</summary>
    Task<IReadOnlyList<DagitimOzeti>> ListeleriGetirAsync(DateOnly? tarih = null, CancellationToken iptal = default);

    Task<DagitimListesi> ListeGetirAsync(int dagitimId, CancellationToken iptal = default);

    /// <summary>
    /// Bugün için taslak liste oluşturur: hakkı doğan aileler ve önceki kısmi teslimattan bekleyen hakkı olanlar.
    /// Malzemeye özel ihtiyacı olan aileler önde sıralanır.
    /// </summary>
    Task<DagitimListesi> ListeOlusturAsync(int urunId, string? aciklama = null, CancellationToken iptal = default);

    /// <summary>Taslak listeye eklenebilecek aileler (tekrar alma süresi dolmamışlar istisna gerektirir).</summary>
    Task<IReadOnlyList<EklenebilirAile>> EklenebilirAilelerAsync(int dagitimId, CancellationToken iptal = default);

    /// <summary>Taslak listeye aile ekler. Tekrar alma süresi dolmamışsa gerekçe zorunludur (istisna).</summary>
    Task AileEkleAsync(int dagitimId, int aileId, string? gerekce, CancellationToken iptal = default);

    /// <summary>Taslak listeden satırı çıkarır (satır silinmez, pasif olur).</summary>
    Task SatirCikarAsync(int detayId, string aciklama, CancellationToken iptal = default);

    /// <summary>Listeyi onaylar: her satır için FEFO ile partilerden ayırma yapılır, miktar kullanılabilir stoktan düşer.</summary>
    Task OnaylaAsync(int dagitimId, CancellationToken iptal = default);

    /// <summary>Teslim: ayrılan miktar dağıtım hareketine döner. Kısmi teslimde kalan miktar bekleyen hak olarak kalır.</summary>
    /// <returns>Satırda kalan bekleyen hak.</returns>
    Task<decimal> TeslimEtAsync(int detayId, decimal miktar, int teslimAlanKisiId, string? aciklama = null, CancellationToken iptal = default);

    /// <summary>Satırın kalan teslimini iptal eder: ayırma kaldırılır, miktar kullanılabilir stoğa döner.</summary>
    Task TeslimIptalAsync(int detayId, string aciklama, CancellationToken iptal = default);

    /// <summary>Teslim yapılmamış listeyi iptal eder (liste silinmez, pasif olur).</summary>
    Task ListeIptalAsync(int dagitimId, string aciklama, CancellationToken iptal = default);

    /// <summary>Gün sonu: bugün dahil açık ayırmalar kaldırılır, onaylı listeler kapanır, onaylanmamış taslaklar iptal olur.</summary>
    Task<GunSonuSonucu> GunSonuAsync(CancellationToken iptal = default);

    /// <summary>Önceki günlerden kalan listeleri kapatır (uygulama açılışı ve dağıtım ekranı yüklenirken, K-047).</summary>
    Task<GunSonuSonucu> OncekiGunleriKapatAsync(CancellationToken iptal = default);
}

public class DagitimServisi(
    IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman, GerekceDegerlendirici degerlendirici) : IDagitimServisi
{
    /// <summary>Refakatsiz çocuk kaydını yalnızca Koordinatör görür (şartname M2, K-068).</summary>
    private bool RefakatsizGorebilir => oturum.RefakatsizCocukGorebilir();

    // ---------------------------------------------------------------- Listeleme

    public async Task<IReadOnlyList<DagitimOzeti>> ListeleriGetirAsync(DateOnly? tarih = null, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Dagitim>().Sorgu()
            .Where(d => tarih == null || d.Tarih == tarih)
            .OrderByDescending(d => d.Tarih).ThenByDescending(d => d.Id)
            .Select(d => new DagitimOzeti
            {
                Id = d.Id, UrunId = d.UrunId, UrunAdi = d.Urun!.Ad, Birim = d.Urun.Birim, Tarih = d.Tarih, Durum = d.Durum,
                Aciklama = d.Aciklama,
                SatirSayisi = d.Detaylar.Count(x => x.Aktif),
                ToplamHak = d.Detaylar.Where(x => x.Aktif).Sum(x => x.HakMiktari),
                ToplamTeslim = d.Detaylar.Where(x => x.Aktif).Sum(x => x.TeslimEdilenMiktar),
                AcikAyrilan = d.Detaylar.SelectMany(x => x.Ayirmalar).Where(a => a.Durum == AyirmaDurumu.Acik)
                    .Sum(a => a.Miktar - a.TeslimEdilenMiktar - a.SerbestBirakilanMiktar),
            })
            .ToListAsync(iptal);
    }

    public async Task<DagitimListesi> ListeGetirAsync(int dagitimId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await ListeOkuAsync(veri, dagitimId, iptal);
    }

    private async Task<DagitimListesi> ListeOkuAsync(IVeriOturumu veri, int dagitimId, CancellationToken iptal)
    {
        var d = await veri.Repo<Dagitim>().Sorgu().AsNoTracking().AsSplitQuery()
            .Include(x => x.Urun)
            .Include(x => x.Detaylar).ThenInclude(x => x.Aile!.Uyeler)
            .Include(x => x.Detaylar).ThenInclude(x => x.Ayirmalar)
            .Include(x => x.Detaylar).ThenInclude(x => x.Teslimler).ThenInclude(t => t.Parti)
            .Include(x => x.Detaylar).ThenInclude(x => x.Teslimler).ThenInclude(t => t.Kullanici)
            .Include(x => x.Detaylar).ThenInclude(x => x.Teslimler).ThenInclude(t => t.TeslimAlanKisi)
            .FirstOrDefaultAsync(x => x.Id == dagitimId, iptal) ?? throw new KayitBulunamadiException("Dağıtım listesi");

        var simdi = zaman.Simdi();
        var oncelikler = await GerekceServisi.DegerlendirAsync(veri, d.Detaylar.Select(x => x.AileId).ToList(), simdi, degerlendirici, iptal);

        var satirlar = d.Detaylar.Select(x =>
        {
            var aile = x.Aile!;
            var gizli = oturum.GizliMi(aile);
            return new DagitimSatiri
            {
                DetayId = x.Id, AileId = aile.Id,
                AileNo = gizli ? "—" : aile.AileNo,
                AileAdi = gizli ? Gosterim.GizliKayit : aile.AileAdi,
                KoordinatorTakibinde = aile.YalnizRefakatsizMi(),
                HakSahibiKisiSayisi = aile.Uyeler.Count(u => u.HakSahibiMi),
                HakMiktari = x.HakMiktari, TeslimEdilenMiktar = x.TeslimEdilenMiktar,
                AyrilanMiktar = x.Ayirmalar.Where(a => a.Durum == AyirmaDurumu.Acik).Sum(a => a.AcikMiktar),
                OncelikliIhtiyac = x.OncelikliIhtiyac,
                DestekOnceligi = oncelikler.TryGetValue(aile.Id, out var o) ? o.DestekOnceligi : DestekOnceligi.Normal,
                IstisnaGerekcesi = x.IstisnaGerekcesi, Aktif = x.Aktif,
                TeslimAlabilecekler = gizli ? [] : aile.Uyeler
                    .Where(u => u.Aktif && u.Durum == KisiDurumu.Burada && (RefakatsizGorebilir || !u.RefakatsizCocuk))
                    .OrderBy(u => u.Ad).Select(u => new KisiSecenegi(u.Id, u.KisiNo, u.AdSoyad)).ToList(),
                Teslimler = x.Teslimler.OrderBy(t => t.Zaman).Select(t => new TeslimKaydi(
                    t.Id, t.Zaman, t.Miktar, t.Parti!.PartiNo, t.Kullanici?.KullaniciAdi,
                    t.TeslimAlanKisi is { } k && (RefakatsizGorebilir || !k.RefakatsizCocuk) ? k.AdSoyad : t.TeslimAlanKisi is null ? null : Gosterim.GizliKayit))
                    .ToList(),
            };
        })
        .OrderByDescending(s => s.Aktif)
        .ThenByDescending(s => s.OncelikliIhtiyac)
        .ThenByDescending(s => s.DestekOnceligi)
        .ThenBy(s => s.AileNo)
        .ToList();

        var aktifler = satirlar.Where(s => s.Aktif).ToList();
        return new DagitimListesi
        {
            Ozet = new DagitimOzeti
            {
                Id = d.Id, UrunId = d.UrunId, UrunAdi = d.Urun!.Ad, Birim = d.Urun.Birim, Tarih = d.Tarih, Durum = d.Durum,
                Aciklama = d.Aciklama, SatirSayisi = aktifler.Count, ToplamHak = aktifler.Sum(s => s.HakMiktari),
                ToplamTeslim = aktifler.Sum(s => s.TeslimEdilenMiktar), AcikAyrilan = satirlar.Sum(s => s.AyrilanMiktar),
            },
            Satirlar = satirlar,
        };
    }

    // ---------------------------------------------------------------- Liste hazırlama

    public async Task<DagitimListesi> ListeOlusturAsync(int urunId, string? aciklama = null, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var simdi = zaman.Simdi();
        var bugun = DateOnly.FromDateTime(simdi);
        await KapatAsync(veri, simdi, bugun.AddDays(-1), iptal);

        var urun = await veri.Repo<Urun>().BulAsync(urunId, iptal) ?? throw new KayitBulunamadiException("Ürün");
        if (!urun.Aktif)
            throw new IsKuraliException($"'{urun.Ad}' pasif bir ürün; dağıtım listesi hazırlanamaz.");
        if (await veri.Repo<Dagitim>().VarMiAsync(d => d.UrunId == urunId && d.Aktif
                && (d.Durum == DagitimDurumu.Taslak || d.Durum == DagitimDurumu.Onaylandi), iptal))
            throw new IsKuraliException($"'{urun.Ad}' için açık bir dağıtım listesi var. Önce onu tamamlayın, gün sonu yapın veya iptal edin.");

        var adaylar = (await HakDurumlari.HesaplaAsync(veri, urun, bugun, null, iptal)).Values
            .Where(h => h.ListeHakki > 0).ToList();
        if (adaylar.Count == 0)
            throw new IsKuraliException($"'{urun.Ad}' için şu an hakkı doğmuş aile yok (tekrar alma süresi dolmamış veya hak sahibi üye yok).");

        var oncelikler = await GerekceServisi.DegerlendirAsync(veri, adaylar.Select(a => a.Aile.Id).ToList(), simdi, degerlendirici, iptal);
        var dagitim = new Dagitim
        {
            UrunId = urun.Id, Tarih = bugun, Durum = DagitimDurumu.Taslak, OlusturmaZamani = simdi, OlusturanKullaniciId = oturum.KullaniciId,
            Aciklama = Dogrula.IstegeBagliMetin(aciklama, "Açıklama", 500),
        };
        foreach (var aday in adaylar)
        {
            var detay = new DagitimDetay { AileId = aday.Aile.Id, OncelikliIhtiyac = oncelikler[aday.Aile.Id].UrunIcinOncelikliMi(urun.Id) };
            detay.HakAta(aday.YeniHak, aday.Devreden);
            dagitim.Detaylar.Add(detay);
        }

        veri.Repo<Dagitim>().Ekle(dagitim);
        try
        {
            await veri.KaydetVeGecmiseYazAsync(oturum, simdi, IslemTuru.Olusturma, nameof(Dagitim), () => dagitim.Id,
                () => $"{urun.Ad} dağıtım listesi hazırlandı: {adaylar.Count} aile, toplam {adaylar.Sum(a => a.ListeHakki):0.##} {urun.Birim.Metin()}.",
                iptal);
        }
        catch (IsKuraliException ex) when (ex.InnerException is DbUpdateException)
        {
            // Başka bir görevli aynı anda bu ürün için liste hazırladı (tek açık liste indeksi).
            throw new IsKuraliException($"'{urun.Ad}' için az önce başka bir görevli dağıtım listesi hazırladı. Listeyi yenileyip onu kullanın.", ex);
        }

        return await ListeOkuAsync(veri, dagitim.Id, iptal);
    }

    public async Task<IReadOnlyList<EklenebilirAile>> EklenebilirAilelerAsync(int dagitimId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var dagitim = await TaslakListeAsync(veri, dagitimId, iptal);
        var listedekiler = dagitim.Detaylar.Where(x => x.Aktif).Select(x => x.AileId).ToHashSet();

        return (await HakDurumlari.HesaplaAsync(veri, dagitim.Urun!, zaman.Bugun(), null, iptal)).Values
            .Where(h => !listedekiler.Contains(h.Aile.Id) && h.IstisnaHakki > 0 && !oturum.GizliMi(h.Aile))
            .Select(h => new EklenebilirAile(h.Aile.Id, h.Aile.AileNo, h.Aile.AileAdi,
                h.ListeHakki > 0 ? h.ListeHakki : h.IstisnaHakki, h.ListeHakki == 0, h.SonrakiHakTarihi))
            .OrderBy(a => a.IstisnaGerekir).ThenBy(a => a.AileNo)
            .ToList();
    }

    public async Task AileEkleAsync(int dagitimId, int aileId, string? gerekce, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        var temizGerekce = Dogrula.IstegeBagliMetin(gerekce, "İstisna gerekçesi", 500);
        await using var veri = veriFabrikasi.Ac();
        var dagitim = await TaslakListeAsync(veri, dagitimId, iptal);
        var urun = dagitim.Urun!;

        var mevcut = dagitim.Detaylar.FirstOrDefault(x => x.AileId == aileId);
        if (mevcut is { Aktif: true })
            throw new IsKuraliException("Aile bu listede zaten var.");

        var durum = (await HakDurumlari.HesaplaAsync(veri, urun, zaman.Bugun(), [aileId], iptal)).GetValueOrDefault(aileId)
            ?? throw new KayitBulunamadiException("Aile");
        if (oturum.GizliMi(durum.Aile))
            throw new KayitBulunamadiException("Aile");
        if (durum.IstisnaHakki <= 0)
            throw new IsKuraliException($"Ailenin '{urun.Ad}' için hakkı yok ({urun.HesaplamaTuru.Metin().ToLowerInvariant()} hesaplamada hak sahibi üye yok).");

        var istisna = durum.ListeHakki <= 0;
        if (istisna && temizGerekce is null)
            throw new DogrulamaException(
                $"Ailenin tekrar alma süresi dolmadı (yeni hak {durum.SonrakiHakTarihi:dd.MM.yyyy} tarihinde). İstisna için gerekçe yazmalısınız.");

        var simdi = zaman.Simdi();
        var oncelikli = (await GerekceServisi.DegerlendirAsync(veri, [aileId], simdi, degerlendirici, iptal))[aileId].UrunIcinOncelikliMi(urun.Id);
        var detay = mevcut ?? new DagitimDetay { AileId = aileId };
        detay.Aktif = true;
        detay.HakAta(istisna ? durum.TamHak : durum.YeniHak, durum.Devreden);
        detay.IstisnaGerekcesi = istisna ? temizGerekce : null;
        detay.OncelikliIhtiyac = oncelikli;
        if (mevcut is null)
            dagitim.Detaylar.Add(detay);

        veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(Dagitim), dagitimId,
            $"{urun.Ad} listesine {durum.Aile.AileNo} eklendi ({detay.HakMiktari:0.##} {urun.Birim.Metin()})"
            + (istisna ? $". İstisna gerekçesi: {temizGerekce}" : "."));
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task SatirCikarAsync(int detayId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);
        await using var veri = veriFabrikasi.Ac();
        var detay = await veri.Repo<DagitimDetay>().Sorgu().Include(x => x.Dagitim!.Urun).Include(x => x.Aile!.Uyeler)
            .FirstOrDefaultAsync(x => x.Id == detayId && x.Aktif, iptal) ?? throw new KayitBulunamadiException("Liste satırı");
        if (oturum.GizliMi(detay.Aile!))
            throw new KayitBulunamadiException("Liste satırı");
        if (detay.Dagitim!.Durum != DagitimDurumu.Taslak)
            throw new IsKuraliException("Onaylanmış listeden satır çıkarılamaz. Aile gelmeyecekse teslimi iptal edin.");

        detay.Aktif = false;
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.PasifYapma, nameof(Dagitim), detay.DagitimId,
            $"{detay.Dagitim.Urun!.Ad} listesinden {detay.Aile!.AileNo} çıkarıldı. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task OnaylaAsync(int dagitimId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var dagitim = await TaslakListeAsync(veri, dagitimId, iptal);
        var urun = dagitim.Urun!;
        var simdi = zaman.Simdi();
        var bugun = DateOnly.FromDateTime(simdi);

        var satirlar = dagitim.Detaylar.Where(x => x.Aktif)
            .OrderByDescending(x => x.OncelikliIhtiyac).ThenBy(x => x.Id).ToList();
        if (satirlar.Count == 0)
            throw new IsKuraliException("Listede aile yok; onaylanamaz.");

        var partiler = await veri.Repo<Parti>().ListeleAsync(p => p.UrunId == urun.Id && p.Aktif, iptal);
        FefoSecici.Sec(partiler, satirlar.Sum(x => x.BekleyenHak), bugun); // toplam yeterli değilse anlaşılır mesajla durur

        foreach (var satir in satirlar)
            foreach (var pay in FefoSecici.Sec(partiler, satir.BekleyenHak, bugun))
            {
                pay.Parti.Ayir(pay.Miktar, bugun);
                satir.Ayirmalar.Add(new StokAyirma { Parti = pay.Parti, Miktar = pay.Miktar, OlusturmaZamani = simdi });
            }

        dagitim.Durum = DagitimDurumu.Onaylandi;
        dagitim.OnayZamani = simdi;
        dagitim.OnaylayanKullaniciId = oturum.KullaniciId;
        var kullanilan = satirlar.SelectMany(x => x.Ayirmalar).GroupBy(a => a.Parti!.PartiNo)
            .Select(g => $"{g.Key}: {g.Sum(a => a.Miktar):0.##}");
        veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(Dagitim), dagitimId,
            $"{urun.Ad} listesi onaylandı; {satirlar.Sum(x => x.HakMiktari):0.##} {urun.Birim.Metin()} ayrıldı ({string.Join(", ", kullanilan)}).");
        await veri.GuvenliKaydetAsync(iptal);
    }

    private async Task<Dagitim> TaslakListeAsync(IVeriOturumu veri, int dagitimId, CancellationToken iptal)
    {
        var dagitim = await veri.Repo<Dagitim>().Sorgu().Include(d => d.Urun).Include(d => d.Detaylar)
            .FirstOrDefaultAsync(d => d.Id == dagitimId && d.Aktif, iptal) ?? throw new KayitBulunamadiException("Dağıtım listesi");
        if (dagitim.Durum != DagitimDurumu.Taslak)
            throw new IsKuraliException($"Bu liste {dagitim.Durum.Metin().ToLowerInvariant()} durumunda; yalnızca taslak liste değiştirilebilir.");
        if (dagitim.Tarih != zaman.Bugun())
            throw new IsKuraliException("Bu liste önceki bir güne ait; bugün için yeni liste hazırlayın.");
        return dagitim;
    }

    // ---------------------------------------------------------------- Teslim

    public async Task<decimal> TeslimEtAsync(int detayId, decimal miktar, int teslimAlanKisiId, string? aciklama = null,
        CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        Dogrula.Pozitif(miktar, "Teslim miktarı");
        var temizAciklama = Dogrula.IstegeBagliMetin(aciklama, "Açıklama", 500);

        await using var veri = veriFabrikasi.Ac();
        var detay = await OnayliSatirAsync(veri, detayId, iptal);
        var aile = detay.Aile!;
        var urun = detay.Dagitim!.Urun!;
        if (oturum.GizliMi(aile))
            throw new YetkisizIslemException("Bu ailenin teslimini Koordinatör yapar.");
        if (miktar > detay.BekleyenHak)
            throw new IsKuraliException($"Teslim miktarı bekleyen haktan ({detay.BekleyenHak:0.##}) fazla olamaz.");

        var acikAyirmalar = detay.Ayirmalar.Where(a => a.Durum == AyirmaDurumu.Acik)
            .OrderBy(a => a.Parti!.SonKullanmaTarihi is null).ThenBy(a => a.Parti!.SonKullanmaTarihi).ThenBy(a => a.Id)
            .ToList();
        var ayrilan = acikAyirmalar.Sum(a => a.AcikMiktar);
        if (ayrilan < miktar)
            throw new IsKuraliException(ayrilan == 0
                ? "Bu satırın ayırması kaldırılmış (teslim iptal edildi); teslim yapılamaz. Kalan hak sonraki listeye devreder."
                : $"Bu satır için ayrılmış miktar {ayrilan:0.##}; daha fazlası teslim edilemez.");

        var alan = aile.Uyeler.FirstOrDefault(u => u.Id == teslimAlanKisiId)
            ?? throw new IsKuraliException("Teslim alan kişi bu ailenin üyesi olmalıdır.");
        if (!alan.Aktif || alan.Durum != KisiDurumu.Burada)
            throw new IsKuraliException($"{alan.AdSoyad} şu an çadır kentte değil ({alan.Durum.Metin()}); teslimi başka bir üye almalı.");
        if (!RefakatsizGorebilir && alan.RefakatsizCocuk)
            throw new KayitBulunamadiException("Kişi");

        var simdi = zaman.Simdi();
        var kalan = miktar;
        foreach (var ayirma in acikAyirmalar)
        {
            var pay = Math.Min(kalan, ayirma.AcikMiktar);
            ayirma.TeslimEt(pay, simdi);
            var hareket = new DagitimHareketi
            {
                Parti = ayirma.Parti, DagitimDetay = detay, Miktar = pay, Zaman = simdi,
                KullaniciId = oturum.KullaniciId, TeslimAlanKisiId = alan.Id, Aciklama = temizAciklama,
            };
            hareket.PartiyeUygula(ayirma.Parti!);
            veri.Repo<StokHareketi>().Ekle(hareket);
            kalan -= pay;
            if (kalan == 0)
                break;
        }

        detay.TeslimKaydet(miktar);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Olusturma, nameof(DagitimDetay), detay.Id,
            $"Teslim: {aile.AileNo}, {urun.Ad} {miktar:0.##} {urun.Birim.Metin()}, teslim alan {alan.KisiNo}"
            + (detay.TamamlandiMi ? "." : $"; bekleyen hak {detay.BekleyenHak:0.##}."));
        await veri.GuvenliKaydetAsync(iptal);
        return detay.BekleyenHak;
    }

    public async Task TeslimIptalAsync(int detayId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);
        await using var veri = veriFabrikasi.Ac();
        var detay = await OnayliSatirAsync(veri, detayId, iptal);
        if (oturum.GizliMi(detay.Aile!))
            throw new KayitBulunamadiException("Liste satırı");

        var simdi = zaman.Simdi();
        var serbest = AyirmalariKaldir(detay.Ayirmalar, simdi);
        if (serbest == 0)
            throw new IsKuraliException("Bu satırda teslim bekleyen ayrılmış miktar yok.");

        var urun = detay.Dagitim!.Urun!;
        veri.IslemKaydet(oturum, simdi, IslemTuru.Iptal, nameof(Dagitim), detay.DagitimId,
            $"{urun.Ad} teslimi iptal edildi: {detay.Aile!.AileNo}, {serbest:0.##} {urun.Birim.Metin()} stoğa döndü; bekleyen hak {detay.BekleyenHak:0.##}. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    private async Task<DagitimDetay> OnayliSatirAsync(IVeriOturumu veri, int detayId, CancellationToken iptal)
    {
        var detay = await veri.Repo<DagitimDetay>().Sorgu()
            .Include(x => x.Dagitim!.Urun).Include(x => x.Aile!.Uyeler).Include(x => x.Ayirmalar).ThenInclude(a => a.Parti)
            .FirstOrDefaultAsync(x => x.Id == detayId && x.Aktif && x.Dagitim!.Aktif, iptal)
            ?? throw new KayitBulunamadiException("Liste satırı");
        var dagitim = detay.Dagitim!;
        if (dagitim.Durum == DagitimDurumu.Taslak)
            throw new IsKuraliException("Liste henüz onaylanmadı; teslimden önce listeyi onaylayın.");
        if (dagitim.Durum != DagitimDurumu.Onaylandi || dagitim.Tarih != zaman.Bugun())
            throw new IsKuraliException("Bu liste kapandı. Kalan hak bugünkü listeye bekleyen hak olarak gelir.");
        return detay;
    }

    // ---------------------------------------------------------------- İptal ve gün sonu

    public async Task ListeIptalAsync(int dagitimId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);
        await using var veri = veriFabrikasi.Ac();
        var dagitim = await veri.Repo<Dagitim>().Sorgu().Include(d => d.Urun)
            .Include(d => d.Detaylar).ThenInclude(x => x.Ayirmalar).ThenInclude(a => a.Parti)
            .FirstOrDefaultAsync(d => d.Id == dagitimId && d.Aktif, iptal) ?? throw new KayitBulunamadiException("Dağıtım listesi");
        if (dagitim.Durum is not (DagitimDurumu.Taslak or DagitimDurumu.Onaylandi))
            throw new IsKuraliException($"Bu liste {dagitim.Durum.Metin().ToLowerInvariant()} durumunda; iptal edilemez.");
        if (dagitim.Detaylar.Any(x => x.TeslimEdilenMiktar > 0))
            throw new IsKuraliException("Bu listede teslim yapılmış; liste iptal edilemez. Kalan teslimleri iptal edip gün sonu yapın.");

        var simdi = zaman.Simdi();
        var serbest = dagitim.Detaylar.Sum(x => AyirmalariKaldir(x.Ayirmalar, simdi));
        dagitim.Durum = DagitimDurumu.Iptal;
        dagitim.Aktif = false;
        veri.IslemKaydet(oturum, simdi, IslemTuru.Iptal, nameof(Dagitim), dagitimId,
            $"{dagitim.Urun!.Ad} listesi ({dagitim.Tarih:dd.MM.yyyy}) iptal edildi"
            + (serbest > 0 ? $"; {serbest:0.##} {dagitim.Urun.Birim.Metin()} stoğa döndü" : "") + $". Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<GunSonuSonucu> GunSonuAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        return await KapatAsync(veri, zaman.Simdi(), zaman.Bugun(), iptal);
    }

    public async Task<GunSonuSonucu> OncekiGunleriKapatAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Dagitim, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        return await KapatAsync(veri, zaman.Simdi(), zaman.Bugun().AddDays(-1), iptal);
    }

    /// <summary>Tarihi <paramref name="sonTarih"/> veya öncesi olan açık listeleri kapatır.</summary>
    private async Task<GunSonuSonucu> KapatAsync(IVeriOturumu veri, DateTime simdi, DateOnly sonTarih, CancellationToken iptal)
    {
        var listeler = await veri.Repo<Dagitim>().Sorgu().Include(d => d.Urun)
            .Include(d => d.Detaylar).ThenInclude(x => x.Ayirmalar).ThenInclude(a => a.Parti)
            .Where(d => d.Aktif && d.Tarih <= sonTarih && (d.Durum == DagitimDurumu.Taslak || d.Durum == DagitimDurumu.Onaylandi))
            .ToListAsync(iptal);
        if (listeler.Count == 0)
            return new GunSonuSonucu(0, 0, 0);

        int kapanan = 0, iptalEdilen = 0;
        decimal toplamSerbest = 0;
        foreach (var d in listeler)
        {
            if (d.Durum == DagitimDurumu.Taslak)
            {
                d.Durum = DagitimDurumu.Iptal;
                d.Aktif = false;
                iptalEdilen++;
                veri.IslemKaydet(oturum, simdi, IslemTuru.Iptal, nameof(Dagitim), d.Id,
                    $"{d.Urun!.Ad} taslak listesi ({d.Tarih:dd.MM.yyyy}) gün sonunda onaylanmadığı için iptal edildi.");
                continue;
            }

            var serbest = d.Detaylar.Sum(x => AyirmalariKaldir(x.Ayirmalar, simdi));
            toplamSerbest += serbest;
            d.Durum = DagitimDurumu.Kapandi;
            kapanan++;
            veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(Dagitim), d.Id,
                $"Gün sonu: {d.Urun!.Ad} listesi ({d.Tarih:dd.MM.yyyy}) kapandı; teslim edilmeyen {serbest:0.##} {d.Urun.Birim.Metin()} stoğa döndü.");
        }

        await veri.GuvenliKaydetAsync(iptal);
        return new GunSonuSonucu(kapanan, iptalEdilen, toplamSerbest);
    }

    /// <summary>Açık ayırmaları kapatır ve partilerdeki ayrılmış miktarı kullanılabilir stoğa döndürür.</summary>
    private static decimal AyirmalariKaldir(IEnumerable<StokAyirma> ayirmalar, DateTime simdi)
    {
        decimal toplam = 0;
        foreach (var ayirma in ayirmalar.Where(a => a.Durum == AyirmaDurumu.Acik))
        {
            var serbest = ayirma.SerbestBirak(simdi);
            if (serbest > 0)
                ayirma.Parti!.AyirmaKaldir(serbest);
            toplam += serbest;
        }

        return toplam;
    }
}
