using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.Ayarlar;

/// <summary>Duyuru ekle/güncelle bilgisi.</summary>
public sealed record DuyuruBilgisi
{
    public required string Baslik { get; init; }
    public required string Metin { get; init; }
    public required DateTime BaslangicTarihi { get; init; }
    public required DateTime BitisTarihi { get; init; }
}

/// <summary>Ayarlar ekranındaki duyuru satırı.</summary>
public sealed record DuyuruOzeti(int Id, string Baslik, string Metin, DateTime BaslangicTarihi, DateTime BitisTarihi, bool Aktif,
    string YayinDurumu, string? Olusturan);

public interface IDuyuruServisi
{
    /// <summary>Tüm duyurular (yayındakiler, ileri tarihliler, süresi dolanlar; en yeni başlangıç önce).</summary>
    Task<IReadOnlyList<DuyuruOzeti>> ListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default);

    Task<DuyuruOzeti> KaydetAsync(int? id, DuyuruBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Duyuru silinmez; pasif yapılınca kioskta görünmez.</summary>
    Task AktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default);
}

/// <summary>Kiosk duyuruları (şartname M11): yalnızca Koordinatör ("Duyuru" izni) yönetir; süresi dolan kioskta görünmez.</summary>
public class DuyuruServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IDuyuruServisi
{
    public async Task<IReadOnlyList<DuyuruOzeti>> ListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Duyuru, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var simdi = zaman.Simdi();
        var duyurular = await veri.Repo<Duyuru>().Sorgu().AsNoTracking().Include(d => d.OlusturanKullanici)
            .Where(d => pasifleriDeGoster || d.Aktif)
            .OrderByDescending(d => d.BaslangicTarihi).ThenByDescending(d => d.Id)
            .ToListAsync(iptal);
        return duyurular.Select(d => Ozet(d, simdi)).ToList();
    }

    private static DuyuruOzeti Ozet(Duyuru d, DateTime simdi) => new(d.Id, d.Baslik, d.Metin, d.BaslangicTarihi, d.BitisTarihi, d.Aktif,
        !d.Aktif ? "Pasif" : d.YayindaMi(simdi) ? "Yayında" : d.BaslangicTarihi > simdi ? "Henüz başlamadı" : "Süresi doldu",
        d.OlusturanKullanici?.KullaniciAdi);

    public async Task<DuyuruOzeti> KaydetAsync(int? id, DuyuruBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Duyuru, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(bilgi);
        if (bilgi.BitisTarihi <= bilgi.BaslangicTarihi)
            throw new DogrulamaException("Duyurunun bitiş tarihi başlangıcından sonra olmalıdır.");

        await using var veri = veriFabrikasi.Ac();
        var repo = veri.Repo<Duyuru>();
        var duyuru = id is { } mevcutId
            ? await repo.BulAsync(mevcutId, iptal) ?? throw new KayitBulunamadiException("Duyuru")
            : new Duyuru { OlusturanKullaniciId = oturum.KullaniciId };
        duyuru.Baslik = bilgi.Baslik;
        duyuru.Metin = bilgi.Metin;
        duyuru.BaslangicTarihi = bilgi.BaslangicTarihi;
        duyuru.BitisTarihi = bilgi.BitisTarihi;
        if (id is null)
            repo.Ekle(duyuru);

        var simdi = zaman.Simdi();
        await veri.KaydetVeGecmiseYazAsync(oturum, simdi, id is null ? IslemTuru.Olusturma : IslemTuru.Guncelleme, nameof(Duyuru),
            () => duyuru.Id,
            () => $"Duyuru '{duyuru.Baslik}' {(id is null ? "eklendi" : "güncellendi")} ({duyuru.BaslangicTarihi:dd.MM.yyyy HH:mm} – {duyuru.BitisTarihi:dd.MM.yyyy HH:mm}).",
            iptal);
        return Ozet(duyuru, simdi);
    }

    public async Task AktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Duyuru, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var duyuru = await veri.Repo<Duyuru>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Duyuru");
        duyuru.Aktif = aktif;
        veri.IslemKaydet(oturum, zaman.Simdi(), aktif ? IslemTuru.Guncelleme : IslemTuru.PasifYapma, nameof(Duyuru), id,
            $"Duyuru '{duyuru.Baslik}' {(aktif ? "yeniden yayına alındı" : "pasif yapıldı")}.");
        await veri.GuvenliKaydetAsync(iptal);
    }
}

/// <summary>Gerekçe ayarları ekranındaki satır: gerekçenin sabit etkileri ve öncelikli ürünleri.</summary>
public sealed record GerekceAyari(GerekceKodu Kod, string Ad, DestekOnceligi DestekOnceligi, YerlesimEtkisi YerlesimEtkisi, bool TumUrunler,
    IReadOnlyList<int> UrunIdler);

/// <summary>Eşleştirilebilecek ürün.</summary>
public sealed record EslesmeUrunu(int Id, string Ad, UrunKategorisi Kategori, bool Aktif);

public interface IGerekceAyarServisi
{
    Task<IReadOnlyList<GerekceAyari>> GerekceleriGetirAsync(CancellationToken iptal = default);

    Task<IReadOnlyList<EslesmeUrunu>> UrunleriGetirAsync(CancellationToken iptal = default);

    /// <summary>
    /// Gerekçenin öncelikli ürünlerini değiştirir (ör. "65 yaş üstü" için battaniyeye ek olarak su). Açıklama zorunlu.
    /// "Tüm ürünler" önceliği olan gerekçe ve "Özel durumu yok" düzenlenemez.
    /// </summary>
    Task EslesmeleriKaydetAsync(GerekceKodu kod, IReadOnlyCollection<int> urunIdler, string aciklama, CancellationToken iptal = default);
}

/// <summary>
/// Gerekçe ayarları (şartname Ekran 17, K-041): destek önceliği ve yerleşim etkisi şartname tablosuna sabittir; düzenlenebilen
/// gerekçe ↔ öncelikli ürün eşleşmeleridir. Dağıtım listesinin sıralaması veritabanındaki eşleşmeyi kullanır.
/// </summary>
public class GerekceAyarServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IGerekceAyarServisi
{
    public async Task<IReadOnlyList<GerekceAyari>> GerekceleriGetirAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Ayarlar, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var gerekceler = await veri.Repo<Gerekce>().Sorgu().AsNoTracking().Include(g => g.OncelikliUrunler).OrderBy(g => g.Id)
            .ToListAsync(iptal);
        return gerekceler.Select(g => new GerekceAyari(g.Kod, g.Ad, g.DestekOnceligi, g.YerlesimEtkisi, g.TumUrunler,
            g.OncelikliUrunler.Select(u => u.UrunId).Order().ToList())).ToList();
    }

    public async Task<IReadOnlyList<EslesmeUrunu>> UrunleriGetirAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Ayarlar, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Urun>().Sorgu().AsNoTracking().OrderBy(u => u.Ad)
            .Select(u => new EslesmeUrunu(u.Id, u.Ad, u.Kategori, u.Aktif)).ToListAsync(iptal);
    }

    public async Task EslesmeleriKaydetAsync(GerekceKodu kod, IReadOnlyCollection<int> urunIdler, string aciklama,
        CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Ayarlar, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(urunIdler);
        var temiz = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);

        await using var veri = veriFabrikasi.Ac();
        var gerekce = await veri.Repo<Gerekce>().Sorgu().Include(g => g.OncelikliUrunler).ThenInclude(u => u.Urun)
            .FirstOrDefaultAsync(g => g.Kod == kod, iptal) ?? throw new KayitBulunamadiException("Gerekçe");
        if (gerekce.TumUrunler)
            throw new IsKuraliException($"'{gerekce.Ad}' gerekçesi tüm ürünlerde öncelik verir; ürün seçimi yapılmaz.");
        if (kod == GerekceKodu.OzelDurumYok)
            throw new IsKuraliException("'Özel durumu yok' için öncelikli ürün seçilmez.");

        var yeni = urunIdler.Distinct().ToList();
        var urunler = await veri.Repo<Urun>().Sorgu().Where(u => yeni.Contains(u.Id)).ToListAsync(iptal);
        if (urunler.Count != yeni.Count)
            throw new KayitBulunamadiException("Ürün");

        var eski = gerekce.OncelikliUrunler.Select(e => e.Urun!.Ad).Order().ToList();
        // Bileşik anahtarlı eşleşmeler farka göre güncellenir (aynı satır silinip yeniden eklenmez).
        var repo = veri.Repo<GerekceUrun>();
        foreach (var e in gerekce.OncelikliUrunler.Where(e => !yeni.Contains(e.UrunId)).ToList())
            repo.Sil(e);
        var mevcut = gerekce.OncelikliUrunler.Select(e => e.UrunId).ToHashSet();
        foreach (var u in urunler.Where(u => !mevcut.Contains(u.Id)))
            repo.Ekle(new GerekceUrun { GerekceId = gerekce.Id, UrunId = u.Id });

        var yeniAdlar = urunler.Select(u => u.Ad).Order().ToList();
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Gerekce), gerekce.Id,
            $"Gerekçe '{gerekce.Ad}' öncelikli ürünleri: {Liste(eski)} → {Liste(yeniAdlar)}. Açıklama: {temiz}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    private static string Liste(IReadOnlyList<string> adlar) => adlar.Count == 0 ? "yok" : string.Join(", ", adlar);
}
