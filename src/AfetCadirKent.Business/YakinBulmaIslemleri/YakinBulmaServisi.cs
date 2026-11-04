using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.YakinBulmaIslemleri;

/// <summary>Yeni yakın bulma başvurusu: başvuran ve aranan kişinin bilgisi.</summary>
public sealed record YakinBulmaBilgisi
{
    public required string BasvuranAdSoyad { get; init; }
    public string? BasvuranTelefon { get; init; }

    /// <summary>Başvuran çadır kentte kayıtlıysa ailesi.</summary>
    public int? BasvuranAileId { get; init; }

    public required string ArananAd { get; init; }
    public required string ArananSoyad { get; init; }
    public int? ArananYaklasikYas { get; init; }
    public string? ArananMemleket { get; init; }
}

/// <summary>Başvuru listesi satırı. Bulunan kişinin çadır ve iletişim bilgisi bilerek yoktur (şartname M8).</summary>
public sealed record YakinBulmaOzeti
{
    public required int Id { get; init; }
    public required string BasvuranAdSoyad { get; init; }
    public string? BasvuranTelefon { get; init; }
    public string? BasvuranAile { get; init; }
    public required string ArananAdSoyad { get; init; }
    public int? ArananYaklasikYas { get; init; }
    public string? ArananMemleket { get; init; }
    public required YakinBulmaDurumu Durum { get; init; }
    public int? EslesenKisiId { get; init; }

    /// <summary>"K-000012 Ali Yılmaz"; refakatsiz çocuk ve Koordinatör olmayan kullanıcıda gizli gösterim.</summary>
    public string? EslesenKisi { get; init; }

    public string? DogrulamaAciklamasi { get; init; }
    public string? Sonuc { get; init; }
    public required DateTime AcilisZamani { get; init; }
    public DateTime? KapanisZamani { get; init; }
    public string? AcanKullanici { get; init; }

    public bool AcikMi => Durum != YakinBulmaDurumu.Kapandi;
    public string DurumMetni => YakinBulmaBasvurusu.DurumAdi(Durum);
}

/// <summary>
/// Aramada bulunan aday kişi. Yalnızca tanıma için gereken bilgiler vardır; çadır, telefon ve aile bağlantısı paylaşılmaz
/// (şartname M8: "çadır ve iletişim bilgisi otomatik paylaşılmaz").
/// </summary>
public sealed record YakinAdayi(int KisiId, string KisiNo, string AdSoyad, int Yas, Cinsiyet Cinsiyet, string? Memleket, KisiDurumu Durum,
    bool FotografVar, string MemleketUyumu);

public interface IYakinBulmaServisi
{
    Task<YakinBulmaOzeti> BasvuruAcAsync(YakinBulmaBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Başvurular (en yeni önce); <paramref name="sadeceAcik"/> ise kapananlar hariç.</summary>
    Task<IReadOnlyList<YakinBulmaOzeti>> ListeleAsync(bool sadeceAcik, CancellationToken iptal = default);

    /// <summary>
    /// Aranan kişiye uyan kayıtlı kişiler: ad ve soyad Türkçe karakter farkı yok sayılarak eşleşir; yaklaşık yaş verildiyse
    /// ±<see cref="YasToleransi"/> yaş, memleket verildiyse aynı memleket (memleketi bilinmeyen kişi elenmez) ile daraltılır.
    /// </summary>
    Task<IReadOnlyList<YakinAdayi>> AraAsync(int basvuruId, CancellationToken iptal = default);

    /// <summary>Görevli adaylardan birini olası eşleşme olarak işaretler (Açık → Olası eşleşme).</summary>
    Task EslesmeIsaretleAsync(int basvuruId, int kisiId, CancellationToken iptal = default);

    /// <summary>Yanlış adayı kaldırır (Olası eşleşme → Açık). Açıklama zorunlu.</summary>
    Task EslesmeyiKaldirAsync(int basvuruId, string aciklama, CancellationToken iptal = default);

    /// <summary>Olası eşleşme → Doğrulandı. Açıklama zorunlu; kişiyle görüşülmeden (yalnızca fotoğrafla) doğrulanamaz.</summary>
    Task DogrulaAsync(int basvuruId, string aciklama, bool kisiyleGorusuldu, CancellationToken iptal = default);

    /// <summary>Başvuruyu sonuç yazarak kapatır.</summary>
    Task KapatAsync(int basvuruId, string sonuc, CancellationToken iptal = default);

    /// <summary>Adayın fotoğrafı (görevlinin tanımasına yardım için; tek başına doğrulama sayılmaz).</summary>
    Task<byte[]?> FotografAsync(int kisiId, CancellationToken iptal = default);
}

/// <summary>Yakın bulma (şartname M8, K-054): aramayı ve başvuruları yalnızca yetkili görevli (Yakın bulma izni) yürütür.</summary>
public class YakinBulmaServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IYakinBulmaServisi
{
    /// <summary>Yaklaşık yaştan en fazla bu kadar sapan kişiler aday sayılır (K-054).</summary>
    public const int YasToleransi = 5;

    public async Task<YakinBulmaOzeti> BasvuruAcAsync(YakinBulmaBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(bilgi);
        await using var veri = veriFabrikasi.Ac();

        if (bilgi.BasvuranAileId is { } aileId)
        {
            var aile = await veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == aileId && a.Aktif, iptal) ?? throw new KayitBulunamadiException("Aile");
            oturum.AileGorunurOlmali(aile);
        }

        var basvuru = new YakinBulmaBasvurusu
        {
            BasvuranAdSoyad = bilgi.BasvuranAdSoyad, BasvuranTelefon = bilgi.BasvuranTelefon, BasvuranAileId = bilgi.BasvuranAileId,
            ArananAd = bilgi.ArananAd, ArananSoyad = bilgi.ArananSoyad, ArananYaklasikYas = bilgi.ArananYaklasikYas,
            ArananMemleket = bilgi.ArananMemleket, AcilisZamani = zaman.Simdi(), KullaniciId = oturum.KullaniciId,
        };
        veri.Repo<YakinBulmaBasvurusu>().Ekle(basvuru);
        // İşlem geçmişine kişi adı yazılmaz; başvuru numarası yeterlidir.
        await veri.KaydetVeGecmiseYazAsync(oturum, basvuru.AcilisZamani, IslemTuru.Olusturma, nameof(YakinBulmaBasvurusu), () => basvuru.Id,
            () => $"Yakın bulma başvurusu #{basvuru.Id} açıldı.", iptal);
        return (await OzetlerAsync(veri, b => b.Id == basvuru.Id, iptal)).Single();
    }

    public async Task<IReadOnlyList<YakinBulmaOzeti>> ListeleAsync(bool sadeceAcik, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await OzetlerAsync(veri, b => !sadeceAcik || b.Durum != YakinBulmaDurumu.Kapandi, iptal);
    }

    public async Task<IReadOnlyList<YakinAdayi>> AraAsync(int basvuruId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var basvuru = await veri.Repo<YakinBulmaBasvurusu>().Sorgu().AsNoTracking().FirstOrDefaultAsync(b => b.Id == basvuruId, iptal)
            ?? throw new KayitBulunamadiException("Yakın bulma başvurusu");

        var ad = TurkceMetin.Normallestir(basvuru.ArananAd);
        var soyad = TurkceMetin.Normallestir(basvuru.ArananSoyad);
        var adaylar = await veri.Repo<Afetzede>().Sorgu().AsNoTracking()
            .Where(k => k.Aktif && k.AramaAdi.Contains(ad) && k.AramaAdi.Contains(soyad))
            .GorunurKisiler(oturum)
            .Select(k => new { k.Id, k.KisiNo, k.Ad, k.Soyad, k.AramaAdi, k.DogumTarihi, k.Cinsiyet, k.Memleket, k.Durum, FotografVar = k.Fotograf != null })
            .ToListAsync(iptal);

        var bugun = zaman.Bugun();
        var arananMemleket = TurkceMetin.Normallestir(basvuru.ArananMemleket);
        return adaylar
            .Where(k => AdSoyadUyar(k.AramaAdi, ad, soyad))
            .Select(k => new { k, Yas = Afetzede.YasHesapla(k.DogumTarihi, bugun), Memleket = TurkceMetin.Normallestir(k.Memleket) })
            .Where(x => basvuru.ArananYaklasikYas is not { } yas || Math.Abs(x.Yas - yas) <= YasToleransi)
            .Where(x => arananMemleket.Length == 0 || x.Memleket.Length == 0 || x.Memleket == arananMemleket)
            .OrderBy(x => basvuru.ArananYaklasikYas is { } yas ? Math.Abs(x.Yas - yas) : 0)
            .ThenBy(x => x.k.KisiNo)
            .Select(x => new YakinAdayi(x.k.Id, x.k.KisiNo, $"{x.k.Ad} {x.k.Soyad}", x.Yas, x.k.Cinsiyet, x.k.Memleket, x.k.Durum, x.k.FotografVar,
                arananMemleket.Length == 0 ? "Aranmadı" : x.Memleket.Length == 0 ? "Bilinmiyor" : "Aynı"))
            .ToList();
    }

    /// <summary>Aranan ad ve soyad, kişinin normalize edilmiş adında ayrı kelimeler olarak geçmeli (ör. "Ali" "Alican"ı bulmaz).</summary>
    internal static bool AdSoyadUyar(string aramaAdi, string ad, string soyad)
    {
        var kelimeler = aramaAdi.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bool Icerir(string parca) => parca.Split(' ', StringSplitOptions.RemoveEmptyEntries).All(kelimeler.Contains);
        return Icerir(ad) && Icerir(soyad);
    }

    public async Task EslesmeIsaretleAsync(int basvuruId, int kisiId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var basvuru = await BasvuruAsync(veri, basvuruId, iptal);
        var kisi = await veri.Repo<Afetzede>().Sorgu().AsNoTracking().FirstOrDefaultAsync(k => k.Id == kisiId && k.Aktif, iptal)
            ?? throw new KayitBulunamadiException("Kişi");
        oturum.KisilerGorunurOlmali([kisi]);

        basvuru.EslesmeIsaretle(kisiId);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(YakinBulmaBasvurusu), basvuruId,
            $"Yakın bulma başvurusu #{basvuruId}: {kisi.KisiNo} olası eşleşme olarak işaretlendi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task EslesmeyiKaldirAsync(int basvuruId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Degistirme);
        var temiz = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);
        await using var veri = veriFabrikasi.Ac();
        var basvuru = await BasvuruAsync(veri, basvuruId, iptal);

        basvuru.EslesmeyiKaldir();
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(YakinBulmaBasvurusu), basvuruId,
            $"Yakın bulma başvurusu #{basvuruId}: olası eşleşme kaldırıldı. Açıklama: {temiz}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task DogrulaAsync(int basvuruId, string aciklama, bool kisiyleGorusuldu, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var basvuru = await BasvuruAsync(veri, basvuruId, iptal);

        basvuru.Dogrulandi(aciklama, kisiyleGorusuldu);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(YakinBulmaBasvurusu), basvuruId,
            $"Yakın bulma başvurusu #{basvuruId} doğrulandı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task KapatAsync(int basvuruId, string sonuc, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var basvuru = await BasvuruAsync(veri, basvuruId, iptal);

        var simdi = zaman.Simdi();
        basvuru.Kapat(sonuc, simdi);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(YakinBulmaBasvurusu), basvuruId,
            $"Yakın bulma başvurusu #{basvuruId} kapandı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<byte[]?> FotografAsync(int kisiId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.YakinBulma, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var kisi = await veri.Repo<Afetzede>().Sorgu().AsNoTracking().Where(k => k.Id == kisiId && k.Aktif)
            .Select(k => new { k.RefakatsizCocuk, k.Fotograf }).FirstOrDefaultAsync(iptal) ?? throw new KayitBulunamadiException("Kişi");
        if (kisi.RefakatsizCocuk && !oturum.RefakatsizCocukGorebilir())
            throw new KayitBulunamadiException("Kişi");
        return kisi.Fotograf;
    }

    private static async Task<YakinBulmaBasvurusu> BasvuruAsync(IVeriOturumu veri, int basvuruId, CancellationToken iptal) =>
        await veri.Repo<YakinBulmaBasvurusu>().Sorgu().FirstOrDefaultAsync(b => b.Id == basvuruId, iptal)
        ?? throw new KayitBulunamadiException("Yakın bulma başvurusu");

    private async Task<IReadOnlyList<YakinBulmaOzeti>> OzetlerAsync(IVeriOturumu veri,
        System.Linq.Expressions.Expression<Func<YakinBulmaBasvurusu, bool>> kosul, CancellationToken iptal)
    {
        var refakatsizGorebilir = oturum.RefakatsizCocukGorebilir();
        var satirlar = await veri.Repo<YakinBulmaBasvurusu>().Sorgu().AsNoTracking().Where(kosul)
            .OrderByDescending(b => b.AcilisZamani).ThenByDescending(b => b.Id)
            .Select(b => new
            {
                b.Id, b.BasvuranAdSoyad, b.BasvuranTelefon, AileNo = (string?)b.BasvuranAile!.AileNo, AileAdi = (string?)b.BasvuranAile.AileAdi,
                b.ArananAd, b.ArananSoyad, b.ArananYaklasikYas, b.ArananMemleket, b.Durum, b.EslesenKisiId,
                EslesenNo = (string?)b.EslesenKisi!.KisiNo, EslesenAd = (string?)b.EslesenKisi.Ad, EslesenSoyad = (string?)b.EslesenKisi.Soyad,
                EslesenRefakatsiz = (bool?)b.EslesenKisi.RefakatsizCocuk,
                b.DogrulamaAciklamasi, b.Sonuc, b.AcilisZamani, b.KapanisZamani, Acan = (string?)b.Kullanici!.KullaniciAdi,
            })
            .ToListAsync(iptal);

        return satirlar.Select(s => new YakinBulmaOzeti
        {
            Id = s.Id, BasvuranAdSoyad = s.BasvuranAdSoyad, BasvuranTelefon = s.BasvuranTelefon,
            BasvuranAile = s.AileNo is null ? null : $"{s.AileNo} {s.AileAdi}",
            ArananAdSoyad = $"{s.ArananAd} {s.ArananSoyad}", ArananYaklasikYas = s.ArananYaklasikYas, ArananMemleket = s.ArananMemleket,
            Durum = s.Durum, EslesenKisiId = s.EslesenKisiId,
            EslesenKisi = s.EslesenNo is null ? null
                : s.EslesenRefakatsiz == true && !refakatsizGorebilir ? Gosterim.GizliKayit : $"{s.EslesenNo} {s.EslesenAd} {s.EslesenSoyad}",
            DogrulamaAciklamasi = s.DogrulamaAciklamasi, Sonuc = s.Sonuc, AcilisZamani = s.AcilisZamani, KapanisZamani = s.KapanisZamani,
            AcanKullanici = s.Acan,
        }).ToList();
    }
}
