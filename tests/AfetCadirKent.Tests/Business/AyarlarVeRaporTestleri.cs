using AfetCadirKent.Business.Ayarlar;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class AyarlarVeRaporTestleri(VeritabaniFixture fixture)
{
    // ---------------------------------------------------------------- duyurular

    [Fact]
    public async Task Duyuru_EklenirGuncellenir_YayinDurumuHesaplanir_PasifYapilir()
    {
        var servis = fixture.DuyuruServisi();
        var simdi = DateTime.Now;
        var yayinda = await servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Test su saati", Metin = "Su dağıtımı 09:00'da.", BaslangicTarihi = simdi.AddHours(-1), BitisTarihi = simdi.AddDays(1),
        });
        var ileri = await servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Test ileri", Metin = "Yarın başlar.", BaslangicTarihi = simdi.AddDays(1), BitisTarihi = simdi.AddDays(2),
        });
        var bitmis = await servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Test bitmiş", Metin = "Dün bitti.", BaslangicTarihi = simdi.AddDays(-3), BitisTarihi = simdi.AddDays(-1),
        });
        Assert.Equal(("Yayında", "Henüz başlamadı", "Süresi doldu"), (yayinda.YayinDurumu, ileri.YayinDurumu, bitmis.YayinDurumu));

        var guncel = await servis.KaydetAsync(yayinda.Id, new DuyuruBilgisi
        {
            Baslik = "Test su saati (güncel)", Metin = "Su dağıtımı 10:00'da.", BaslangicTarihi = yayinda.BaslangicTarihi,
            BitisTarihi = yayinda.BitisTarihi,
        });
        Assert.Equal((yayinda.Id, "Test su saati (güncel)"), (guncel.Id, guncel.Baslik));

        await servis.AktiflikDegistirAsync(yayinda.Id, false);
        Assert.DoesNotContain(await servis.ListeleAsync(), d => d.Id == yayinda.Id);
        Assert.Equal("Pasif", (await servis.ListeleAsync(pasifleriDeGoster: true)).Single(d => d.Id == yayinda.Id).YayinDurumu);

        await Assert.ThrowsAsync<DogrulamaException>(() => servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Ters", Metin = "x", BaslangicTarihi = simdi, BitisTarihi = simdi.AddMinutes(-1),
        }));
        await Assert.ThrowsAsync<DogrulamaException>(() => servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = " ", Metin = "x", BaslangicTarihi = simdi, BitisTarihi = simdi.AddDays(1),
        }));
    }

    [Theory]
    [InlineData(RolTuru.KayitGorevlisi)]
    [InlineData(RolTuru.DepoSorumlusu)]
    [InlineData(RolTuru.Kiosk)]
    public async Task DuyuruVeGerekceAyarlariYalnizcaKoordinatorde(RolTuru rol)
    {
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.DuyuruServisi(rol).ListeleAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.GerekceAyarServisi(rol).GerekceleriGetirAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            fixture.GerekceAyarServisi(rol).EslesmeleriKaydetAsync(GerekceKodu.Yasli, [], "deneme"));
    }

    // ---------------------------------------------------------------- gerekçe ayarları

    [Fact]
    public async Task GerekceEslesmesiDegisince_AileninMalzemeIhtiyaciDegisir()
    {
        var ayar = fixture.GerekceAyarServisi();
        var urun = await fixture.TestUrunuAsync();
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Hasan", soyad, 72)]);
        var gerekceServisi = fixture.GerekceServisi(RolTuru.KayitGorevlisi);
        bool Oncelikli(global::AfetCadirKent.Business.Gerekceler.AileOncelikSonucu s) => s.MalzemeIhtiyaclari.Single(m => m.UrunId == urun.Id).Oncelikli;

        Assert.False(Oncelikli(await gerekceServisi.DegerlendirAsync(aile.Id)));
        var orijinal = (await ayar.GerekceleriGetirAsync()).Single(g => g.Kod == GerekceKodu.Yasli).UrunIdler;

        await Assert.ThrowsAsync<DogrulamaException>(() => ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, [.. orijinal, urun.Id], " "));
        try
        {
            await ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, [.. orijinal, urun.Id], "Yaşlılara bu ürün de öncelikli verilsin");
            Assert.Contains(urun.Id, (await ayar.GerekceleriGetirAsync()).Single(g => g.Kod == GerekceKodu.Yasli).UrunIdler);
            Assert.True(Oncelikli(await gerekceServisi.DegerlendirAsync(aile.Id)));
        }
        finally
        {
            await ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, orijinal, "Test sonrası eski haline döndü");
        }

        Assert.Equal(orijinal, (await ayar.GerekceleriGetirAsync()).Single(g => g.Kod == GerekceKodu.Yasli).UrunIdler);
        await Assert.ThrowsAsync<IsKuraliException>(() => ayar.EslesmeleriKaydetAsync(GerekceKodu.UcGundurYardimAlmadi, [urun.Id], "x"));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, [int.MaxValue], "x"));
    }
}
