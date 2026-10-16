using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.Gerekceler;

public interface IGerekceServisi
{
    /// <summary>Ailenin destek önceliği, gerekçeleri ve malzeme ihtiyaçları.</summary>
    Task<AileOncelikSonucu> DegerlendirAsync(int aileId, CancellationToken iptal = default);

    /// <summary>Birden fazla aile için toplu değerlendirme (listeler, dağıtım, bekleme listesi).</summary>
    Task<IReadOnlyDictionary<int, AileOncelikSonucu>> DegerlendirAsync(IReadOnlyCollection<int> aileIdler, CancellationToken iptal = default);

    /// <summary>Görevlinin gerekçe eklemesi veya otomatik gerekçeyi kaldırması. Açıklama zorunludur.</summary>
    Task ElleDegistirAsync(int aileId, GerekceKodu kod, GerekceDegisiklikTuru tur, string aciklama, CancellationToken iptal = default);

    /// <summary>Geçerli elle değişikliği geri alır (otomatik hesaplamaya döner). Açıklama zorunludur.</summary>
    Task ElleDegisikligiGeriAlAsync(int aileId, GerekceKodu kod, string aciklama, CancellationToken iptal = default);

    /// <summary>Ailenin elle gerekçe değişikliği geçmişi (en yeni önce).</summary>
    Task<IReadOnlyList<AileGerekcesi>> DegisiklikGecmisiAsync(int aileId, CancellationToken iptal = default);
}

public class GerekceServisi(
    IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman, GerekceDegerlendirici degerlendirici) : IGerekceServisi
{
    public async Task<AileOncelikSonucu> DegerlendirAsync(int aileId, CancellationToken iptal = default)
    {
        var sonuclar = await DegerlendirAsync([aileId], iptal);
        return sonuclar.TryGetValue(aileId, out var sonuc) ? sonuc : throw new KayitBulunamadiException("Aile");
    }

    public async Task<IReadOnlyDictionary<int, AileOncelikSonucu>> DegerlendirAsync(
        IReadOnlyCollection<int> aileIdler, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await DegerlendirAsync(veri, aileIdler, zaman.Simdi(), degerlendirici, iptal);
    }

    /// <summary>
    /// Açık bir veri oturumu içinde toplu değerlendirme. Diğer servisler (çadır, dağıtım) aynı oturumda kullanır.
    /// Sorgular aile sayısından bağımsızdır (aileler, son teslimler, elle değişiklikler, ürün eşleşmeleri).
    /// </summary>
    internal static async Task<IReadOnlyDictionary<int, AileOncelikSonucu>> DegerlendirAsync(
        IVeriOturumu veri, IReadOnlyCollection<int> aileIdler, DateTime simdi, GerekceDegerlendirici degerlendirici,
        CancellationToken iptal)
    {
        var idler = aileIdler.Distinct().ToList();

        var aileler = await veri.Repo<Aile>().Sorgu()
            .Include(a => a.Uyeler)
            .Where(a => idler.Contains(a.Id))
            .AsNoTracking()
            .ToListAsync(iptal);

        var sonTeslimler = await veri.Repo<StokHareketi>().Sorgu()
            .OfType<DagitimHareketi>()
            .Where(h => h.Aktif && idler.Contains(h.DagitimDetay!.AileId))
            .GroupBy(h => h.DagitimDetay!.AileId)
            .Select(g => new { AileId = g.Key, Son = g.Max(h => h.Zaman) })
            .ToDictionaryAsync(x => x.AileId, x => (DateTime?)x.Son, iptal);

        var elleDegisiklikler = (await veri.Repo<AileGerekcesi>().Sorgu()
                .Where(g => g.Gecerli && idler.Contains(g.AileId))
                .Select(g => new { g.AileId, g.GerekceId, g.Tur, g.Zaman })
                .ToListAsync(iptal))
            .GroupBy(g => g.AileId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyDictionary<GerekceKodu, GerekceDegisiklikTuru>)g
                    .GroupBy(x => (GerekceKodu)x.GerekceId)
                    .ToDictionary(x => x.Key, x => x.OrderByDescending(d => d.Zaman).First().Tur));

        var urunler = await veri.Repo<Urun>().Sorgu().Where(u => u.Aktif).OrderBy(u => u.Ad)
            .Select(u => new { u.Id, u.Ad }).ToListAsync(iptal);
        var tumUrunlerGerekceleri = await veri.Repo<Gerekce>().Sorgu().Where(g => g.TumUrunler)
            .Select(g => g.Kod).ToListAsync(iptal);
        var eslesmeler = (await veri.Repo<GerekceUrun>().Sorgu().ToListAsync(iptal))
            .GroupBy(e => (GerekceKodu)e.GerekceId)
            .ToDictionary(g => g.Key, g => g.Select(e => e.UrunId).ToHashSet());

        var sonuclar = new Dictionary<int, AileOncelikSonucu>();
        foreach (var aile in aileler)
        {
            var baglam = new AileBaglami(aile, aile.CadirKenttekiUyeler.ToList(), simdi, sonTeslimler.GetValueOrDefault(aile.Id));
            var sonuc = degerlendirici.Degerlendir(baglam, elleDegisiklikler.GetValueOrDefault(aile.Id));

            var kodlar = sonuc.Gerekceler.Select(g => g.Kod).ToList();
            var tumUrunler = kodlar.Any(tumUrunlerGerekceleri.Contains);
            var oncelikliUrunIdleri = kodlar.SelectMany(k => eslesmeler.GetValueOrDefault(k) ?? []).ToHashSet();

            sonuclar[aile.Id] = new AileOncelikSonucu
            {
                AileId = sonuc.AileId,
                DestekOnceligi = sonuc.DestekOnceligi,
                Gerekceler = sonuc.Gerekceler,
                MalzemeIhtiyaclari = urunler
                    .Select(u => new MalzemeIhtiyaci(u.Id, u.Ad, tumUrunler || oncelikliUrunIdleri.Contains(u.Id)))
                    .ToList(),
            };
        }

        return sonuclar;
    }

    public async Task ElleDegistirAsync(int aileId, GerekceKodu kod, GerekceDegisiklikTuru tur, string aciklama,
        CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        if (kod == GerekceKodu.OzelDurumYok)
            throw new IsKuraliException("'Özel durumu yok' gerekçesi elle eklenip çıkarılamaz; başka gerekçe yoksa kendiliğinden görünür.");

        await using var veri = veriFabrikasi.Ac();
        var aile = await AktifAileGetirAsync(veri, aileId, iptal);
        var simdi = zaman.Simdi();

        await OncekileriGecersizYapAsync(veri, aileId, kod, iptal);
        var degisiklik = new AileGerekcesi
        {
            AileId = aileId,
            GerekceId = (int)kod,
            Tur = tur,
            Aciklama = aciklama, // entity: boş olamaz
            Zaman = simdi,
            KullaniciId = oturum.Kullanici is { Id: > 0 } k ? k.Id : null,
        };
        veri.Repo<AileGerekcesi>().Ekle(degisiklik);

        var islem = tur == GerekceDegisiklikTuru.ElleEklendi ? "eklendi" : "kaldırıldı";
        veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(Aile), aileId,
            $"{aile.AileNo}: '{GerekceAdi(kod)}' gerekçesi elle {islem}. Açıklama: {degisiklik.Aciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task ElleDegisikligiGeriAlAsync(int aileId, GerekceKodu kod, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);

        await using var veri = veriFabrikasi.Ac();
        var aile = await AktifAileGetirAsync(veri, aileId, iptal);
        if (await OncekileriGecersizYapAsync(veri, aileId, kod, iptal) == 0)
            throw new IsKuraliException("Bu gerekçe için geri alınacak elle değişiklik yok.");

        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Iptal, nameof(Aile), aileId,
            $"{aile.AileNo}: '{GerekceAdi(kod)}' gerekçesindeki elle değişiklik geri alındı. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<IReadOnlyList<AileGerekcesi>> DegisiklikGecmisiAsync(int aileId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<AileGerekcesi>().Sorgu()
            .Include(g => g.Gerekce).Include(g => g.Kullanici)
            .Where(g => g.AileId == aileId)
            .OrderByDescending(g => g.Zaman)
            .AsNoTracking()
            .ToListAsync(iptal);
    }

    private string GerekceAdi(GerekceKodu kod) => degerlendirici.Kurallar.First(k => k.Kod == kod).Ad;

    private static async Task<Aile> AktifAileGetirAsync(IVeriOturumu veri, int aileId, CancellationToken iptal) =>
        await veri.Repo<Aile>().Sorgu().FirstOrDefaultAsync(a => a.Id == aileId && a.Aktif, iptal)
        ?? throw new KayitBulunamadiException("Aile");

    private static async Task<int> OncekileriGecersizYapAsync(IVeriOturumu veri, int aileId, GerekceKodu kod, CancellationToken iptal)
    {
        var oncekiler = await veri.Repo<AileGerekcesi>().ListeleAsync(
            g => g.AileId == aileId && g.GerekceId == (int)kod && g.Gecerli, iptal);
        foreach (var onceki in oncekiler)
            onceki.Gecerli = false;
        return oncekiler.Count;
    }
}
