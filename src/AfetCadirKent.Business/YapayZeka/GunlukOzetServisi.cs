using AfetCadirKent.Business.DepoYonetimi;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.YapayZeka;

public interface IGunlukOzetServisi
{
    /// <summary>Günlük özetin dayandığı toplu durum (ana ekran göstergeleri için de kullanılır). Yalnızca Koordinatör.</summary>
    Task<GunlukDurum> DurumuGetirAsync(CancellationToken iptal = default);

    /// <summary>
    /// Koordinatör için yapay zekanın yazdığı günlük durum raporu (REQ-135). Yapay zeka çalışmıyorsa durum bilgisi yine döner,
    /// metin boş kalır ve hata mesajı verilir.
    /// </summary>
    Task<GunlukOzet> OlusturAsync(CancellationToken iptal = default);
}

public class GunlukOzetServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman,
    IStokServisi stokServisi, IYapayZekaServisi yapayZeka) : IGunlukOzetServisi
{
    public async Task<GunlukDurum> DurumuGetirAsync(CancellationToken iptal = default)
    {
        KoordinatorOlmali();
        var bugun = zaman.Bugun();
        var gunBasi = bugun.ToDateTime(TimeOnly.MinValue);

        await using var veri = veriFabrikasi.Ac();
        var nufus = await DurumSayaclari.NufusAsync(veri, iptal);
        var yatak = await DurumSayaclari.YatakAsync(veri, iptal);
        var bekleyen = await DurumSayaclari.BekleyenAileAsync(veri, iptal);

        var acikTalepler = await veri.Repo<Talep>().Sorgu()
            .Where(t => t.Durum != TalepDurumu.Tamamlandi && t.Durum != TalepDurumu.Iptal)
            .GroupBy(t => t.Tur).Select(g => new { Tur = g.Key, Sayi = g.Count() })
            .ToListAsync(iptal);
        var bugunAcilan = await veri.Repo<Talep>().Sorgu().CountAsync(t => t.AcilisZamani >= gunBasi, iptal);
        var bugunKapanan = await veri.Repo<Talep>().Sorgu().CountAsync(t => t.KapanisZamani >= gunBasi, iptal);
        var bugunTeslim = await veri.Repo<StokHareketi>().Sorgu().OfType<DagitimHareketi>()
            .CountAsync(h => h.Aktif && h.Zaman >= gunBasi, iptal);

        var uyarilar = await stokServisi.UyarilariGetirAsync(iptal);
        List<string> Uyarilar(StokUyariTuru tur) => uyarilar.Where(u => u.Tur == tur).Select(u => u.Mesaj).ToList();

        return new GunlukDurum
        {
            Tarih = bugun,
            BuradakiKisi = nufus.Burada,
            HastanedekiKisi = nufus.Hastanede,
            AileSayisi = nufus.AileSayisi,
            ToplamKapasite = yatak.ToplamKapasite,
            BosYatak = yatak.BosYatak,
            KullanilamayanCadir = yatak.KullanilamayanCadir,
            BekleyenAile = bekleyen,
            KritikStoklar = Uyarilar(StokUyariTuru.KritikSeviye),
            SktYaklasanlar = Uyarilar(StokUyariTuru.SktYaklasiyor),
            SktGecenler = Uyarilar(StokUyariTuru.SktGecti),
            AcikTalepler = acikTalepler.OrderBy(t => t.Tur is null ? -1 : (int)t.Tur)
                .ToDictionary(t => t.Tur.Metin(), t => t.Sayi),
            BugunAcilanTalep = bugunAcilan,
            BugunKapananTalep = bugunKapanan,
            BugunkuTeslimSayisi = bugunTeslim,
        };
    }

    public async Task<GunlukOzet> OlusturAsync(CancellationToken iptal = default)
    {
        var durum = await DurumuGetirAsync(iptal);
        var sonuc = await yapayZeka.GunlukOzetYazAsync(durum, iptal);
        return new GunlukOzet(durum, sonuc.Deger, sonuc.HataMesaji);
    }

    private void KoordinatorOlmali()
    {
        if (!oturum.AcikMi)
            throw new YetkisizIslemException("Bu işlem için giriş yapmalısınız.");
        if (oturum.Rol != RolTuru.Koordinator)
            throw new YetkisizIslemException("Günlük özet yalnızca Koordinatör içindir.");
    }
}
