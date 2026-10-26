using AfetCadirKent.Business.AnaEkran;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Tests.Altyapi;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

/// <summary>Ana ekran göstergeleri: her bölüm yalnızca o modülü görme izni olan role gösterilir (REQ-160, K-081).</summary>
[Collection(VeritabaniKoleksiyonu.Ad)]
public class AnaEkranServisiTestleri(VeritabaniFixture fixture)
{
    private AnaEkranServisi Servis(IOturum oturum) => new(fixture.OturumFabrikasi(), oturum,
        new StokServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, new DepoAyarlari()),
        fixture.TalepServisi(oturum));

    private Task<TalepOzeti> TalepAcAsync(IOturum oturum, string aciklama, TalepTuru? tur = null) =>
        fixture.TalepServisi(oturum).OlusturAsync(new TalepBilgisi { Aciklama = aciklama, Kaynak = TalepKaynagi.Gorevli, Tur = tur });

    [Fact]
    public async Task Koordinator_TumGostergeleriGorur_GunlukDurumlaAyniSayilar()
    {
        var oturum = await fixture.KullaniciOturumuAsync(RolTuru.Koordinator);
        await TalepAcAsync(oturum, "Ana ekran testi: sınıflandırılmamış talep");

        var ozet = await Servis(oturum).GetirAsync();
        var durum = await new GunlukOzetServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System,
            new StokServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, new DepoAyarlari()), new SahteYapayZeka())
            .DurumuGetirAsync();

        Assert.Equal(durum.BuradakiKisi + durum.HastanedekiKisi, ozet.ToplamKisi);
        Assert.Equal(durum.HastanedekiKisi, ozet.HastanedekiKisi);
        Assert.Equal(durum.AileSayisi, ozet.AileSayisi);
        Assert.Equal(durum.BosYatak, ozet.BosYatak);
        Assert.Equal(durum.ToplamKapasite, ozet.ToplamKapasite);
        Assert.Equal(durum.KullanilamayanCadir, ozet.KullanilamayanCadir);
        Assert.Equal(durum.BekleyenAile, ozet.BekleyenAile);
        Assert.Equal(durum.AcikTalepToplami, ozet.AcikTalepler!.Values.Sum());
        Assert.True(ozet.AcikTalepler["Sınıflandırılmamış"] >= 1);
        Assert.Equal(durum.KritikStoklar.Count + durum.SktYaklasanlar.Count + durum.SktGecenler.Count, ozet.StokUyarilari!.Count);
        Assert.True(ozet.GunlukOzetGorebilir);
        Assert.False(ozet.BosMu);
        Assert.InRange(ozet.DolulukYuzdesi!.Value, 0, 100);
    }

    [Fact]
    public async Task DepoSorumlusu_CadirVeYapayZekaOzetiniGormez()
    {
        var ozet = await Servis(OturumAc(RolTuru.DepoSorumlusu)).GetirAsync();

        Assert.NotNull(ozet.ToplamKisi); // kayıt görme izni var
        Assert.NotNull(ozet.StokUyarilari);
        Assert.NotNull(ozet.AcikTalepler);
        Assert.Null(ozet.BosYatak);
        Assert.Null(ozet.BekleyenAile);
        Assert.Null(ozet.DolulukYuzdesi);
        Assert.False(ozet.GunlukOzetGorebilir);
    }

    [Fact]
    public async Task KayitGorevlisi_StokUyarilariniGormez()
    {
        var ozet = await Servis(OturumAc(RolTuru.KayitGorevlisi)).GetirAsync();

        Assert.NotNull(ozet.ToplamKisi);
        Assert.NotNull(ozet.BosYatak);
        Assert.NotNull(ozet.BekleyenAile);
        Assert.Null(ozet.StokUyarilari);
        Assert.False(ozet.GunlukOzetGorebilir);
    }

    [Fact]
    public async Task SahaGorevlisi_YalnizcaKendineAtananTalepleriSayar()
    {
        var koordinator = await fixture.KullaniciOturumuAsync(RolTuru.Koordinator);
        await TalepAcAsync(koordinator, "Ana ekran testi: başkasının talebi", TalepTuru.CadirAriza);
        var saha = await fixture.KullaniciOturumuAsync(RolTuru.SahaGorevlisi);

        var ozet = await Servis(saha).GetirAsync();

        Assert.NotNull(ozet.AcikTalepler);
        Assert.Empty(ozet.AcikTalepler); // yeni görevliye atanmış talep yok
        Assert.NotNull(ozet.BosYatak);   // çadır görme izni var
        Assert.Null(ozet.ToplamKisi);
        Assert.Null(ozet.StokUyarilari);
    }

    [Fact]
    public async Task SaglikPersoneli_YalnizcaSaglikTaleplerini_Sayar()
    {
        var koordinator = await fixture.KullaniciOturumuAsync(RolTuru.Koordinator);
        await TalepAcAsync(koordinator, "Ana ekran testi: malzeme talebi", TalepTuru.Malzeme);
        await TalepAcAsync(koordinator, "Ana ekran testi: sağlık talebi", TalepTuru.Saglik);

        var ozet = await Servis(await fixture.KullaniciOturumuAsync(RolTuru.SaglikPersoneli)).GetirAsync();

        Assert.NotNull(ozet.AcikTalepler);
        Assert.Equal(["Sağlık"], ozet.AcikTalepler.Keys);
        Assert.Null(ozet.StokUyarilari);
        Assert.Null(ozet.BosYatak);
    }

    [Fact]
    public async Task Kiosk_HicGostergeGormez()
    {
        var ozet = await Servis(OturumAc(RolTuru.Kiosk)).GetirAsync();

        Assert.True(ozet.BosMu);
    }

    [Fact]
    public async Task OturumYoksa_Yetkisiz()
    {
        await Assert.ThrowsAsync<YetkisizIslemException>(() => Servis(new Oturum()).GetirAsync());
    }
}
