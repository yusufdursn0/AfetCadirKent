using AfetCadirKent.Business;
using AfetCadirKent.Business.AnaEkran;
using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.Denetim;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.KioskIslemleri;
using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.DataAccess;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.Extensions.DependencyInjection;

namespace AfetCadirKent.Tests.Business;

/// <summary>Uygulamanın açılışta kurduğu bağımlılık kayıtları: her servis çözülebilmeli, oturum tek olmalı.</summary>
[Collection(VeritabaniKoleksiyonu.Ad)]
public class ServisKayitlariTestleri(VeritabaniFixture fixture)
{
    private ServiceProvider Olustur() => new ServiceCollection()
        .VeriErisiminiEkle(fixture.BaglantiDizesi)
        .IsKatmaniniEkle(new OllamaAyarlari(), new DepoAyarlari())
        .BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });

    [Theory]
    [InlineData(typeof(IUygulamaBaslatici))]
    [InlineData(typeof(IKurulumServisi))]
    [InlineData(typeof(IOturumServisi))]
    [InlineData(typeof(IKullaniciServisi))]
    [InlineData(typeof(IKayitServisi))]
    [InlineData(typeof(IGerekceServisi))]
    [InlineData(typeof(ICadirServisi))]
    [InlineData(typeof(IBeklemeListesiServisi))]
    [InlineData(typeof(IUrunServisi))]
    [InlineData(typeof(IStokServisi))]
    [InlineData(typeof(IDagitimServisi))]
    [InlineData(typeof(ITalepServisi))]
    [InlineData(typeof(IKioskServisi))]
    [InlineData(typeof(IGunlukOzetServisi))]
    [InlineData(typeof(IAnaEkranServisi))]
    [InlineData(typeof(IIslemGecmisiServisi))]
    [InlineData(typeof(IYapayZekaServisi))]
    public void HerServisCozulur(Type servisTuru)
    {
        using var servisler = Olustur();

        Assert.NotNull(servisler.GetRequiredService(servisTuru));
    }

    [Fact]
    public void OturumVeSiniflandirmaKuyrugu_UygulamaBoyuncaTekNesne()
    {
        using var servisler = Olustur();

        Assert.Same(servisler.GetRequiredService<Oturum>(), servisler.GetRequiredService<IOturum>());
        Assert.Same(servisler.GetRequiredService<OtomatikSiniflandirici>(), servisler.GetRequiredService<ITalepSiniflandirmaKuyrugu>());
        Assert.IsType<OllamaServisi>(servisler.GetRequiredService<IYapayZekaServisi>());
    }

    [Fact]
    public async Task GirisYapilinca_ServislerAyniOturumuGorur()
    {
        using var servisler = Olustur();
        var kullanici = TestYardimcilari.YeniKullanici();
        await fixture.KullaniciServisi(TestYardimcilari.OturumAc(RolTuru.Koordinator))
            .EkleAsync(kullanici, RolTuru.DepoSorumlusu);

        await servisler.GetRequiredService<IOturumServisi>().GirisYapAsync(kullanici.KullaniciAdi, kullanici.Parola);

        var oturum = servisler.GetRequiredService<IOturum>();
        Assert.True(oturum.AcikMi);
        Assert.Equal(RolTuru.DepoSorumlusu, oturum.Rol);
        var ozet = await servisler.GetRequiredService<IAnaEkranServisi>().GetirAsync();
        Assert.NotNull(ozet.StokUyarilari);

        await servisler.GetRequiredService<IOturumServisi>().CikisYapAsync();
        Assert.False(oturum.AcikMi);
    }
}
