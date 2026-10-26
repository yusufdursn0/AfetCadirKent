using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.TalepYonetimi;

namespace AfetCadirKent.Business.AnaEkran;

/// <summary>
/// Ana ekran göstergeleri (şartname M13). Her bölüm, oturumdaki rolün o modülü görme izni varsa doludur; yoksa null
/// ve ekranda gösterilmez (K-081).
/// </summary>
public sealed record AnaEkranOzeti
{
    /// <summary>Kayıt görme izni: çadır kentte bulunan kişi (Burada + Hastanede) ve aile sayısı.</summary>
    public int? ToplamKisi { get; init; }

    public int? HastanedekiKisi { get; init; }
    public int? AileSayisi { get; init; }

    /// <summary>Çadır görme izni: kullanılabilir çadırlardaki boş yatak ve kapasite.</summary>
    public int? BosYatak { get; init; }

    public int? ToplamKapasite { get; init; }
    public int? KullanilamayanCadir { get; init; }
    public int? BekleyenAile { get; init; }

    /// <summary>Depo görme izni: kritik seviye, SKT'si yaklaşan ve geçmiş parti uyarıları.</summary>
    public IReadOnlyList<StokUyarisi>? StokUyarilari { get; init; }

    /// <summary>Talep görme izni: rolün görebildiği açık talepler, türe göre (sınıflandırılmamışlar dahil).</summary>
    public IReadOnlyDictionary<string, int>? AcikTalepler { get; init; }

    /// <summary>Yapay zeka günlük özeti yalnızca Koordinatör içindir.</summary>
    public required bool GunlukOzetGorebilir { get; init; }

    public bool BosMu => ToplamKisi is null && BosYatak is null && StokUyarilari is null && AcikTalepler is null && !GunlukOzetGorebilir;

    /// <summary>Kullanılabilir yatakların doluluk yüzdesi; kapasite yoksa null.</summary>
    public double? DolulukYuzdesi => ToplamKapasite is > 0 && BosYatak is { } bos
        ? Math.Round(100.0 * (ToplamKapasite.Value - bos) / ToplamKapasite.Value, 1)
        : null;
}

public interface IAnaEkranServisi
{
    Task<AnaEkranOzeti> GetirAsync(CancellationToken iptal = default);
}

public class AnaEkranServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, IStokServisi stokServisi,
    ITalepServisi talepServisi) : IAnaEkranServisi
{
    public async Task<AnaEkranOzeti> GetirAsync(CancellationToken iptal = default)
    {
        if (!oturum.AcikMi)
            throw new YetkisizIslemException("Bu işlem için giriş yapmalısınız.");

        DurumSayaclari.Nufus? nufus = null;
        DurumSayaclari.Yatak? yatak = null;
        int? bekleyen = null;
        await using (var veri = veriFabrikasi.Ac())
        {
            if (oturum.IzinVarMi(Modul.Kayit, Izin.Gorme))
                nufus = await DurumSayaclari.NufusAsync(veri, iptal);
            if (oturum.IzinVarMi(Modul.Cadir, Izin.Gorme))
            {
                yatak = await DurumSayaclari.YatakAsync(veri, iptal);
                bekleyen = await DurumSayaclari.BekleyenAileAsync(veri, iptal);
            }
        }

        var stokUyarilari = oturum.IzinVarMi(Modul.Depo, Izin.Gorme) ? await stokServisi.UyarilariGetirAsync(iptal) : null;

        IReadOnlyDictionary<string, int>? acikTalepler = null;
        if (oturum.IzinVarMi(Modul.Talep, Izin.Gorme))
        {
            // Talep servisi rol filtresini uygular (K-072): saha görevlisi kendine atananları, sağlık personeli sağlık taleplerini görür.
            var talepler = await talepServisi.ListeleAsync(new TalepFiltresi { SadeceAcik = true }, iptal);
            acikTalepler = talepler.GroupBy(t => t.Tur).OrderBy(g => g.Key is null ? -1 : (int)g.Key)
                .ToDictionary(g => g.Key.Metin(), g => g.Count());
        }

        return new AnaEkranOzeti
        {
            ToplamKisi = nufus is null ? null : nufus.Burada + nufus.Hastanede,
            HastanedekiKisi = nufus?.Hastanede,
            AileSayisi = nufus?.AileSayisi,
            BosYatak = yatak?.BosYatak,
            ToplamKapasite = yatak?.ToplamKapasite,
            KullanilamayanCadir = yatak?.KullanilamayanCadir,
            BekleyenAile = bekleyen,
            StokUyarilari = stokUyarilari,
            AcikTalepler = acikTalepler,
            GunlukOzetGorebilir = oturum.Rol == RolTuru.Koordinator,
        };
    }
}
