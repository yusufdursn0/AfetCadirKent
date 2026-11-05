using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.GonulluYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class GonulluServisiTestleri(VeritabaniFixture fixture)
{
    private GonulluServisi Servis() => fixture.GonulluServisi();

    /// <summary>Yarın belirli saatte başlayan zaman (testler birbirinin vardiyasıyla karışmasın diye gönüllüler ayrıdır).</summary>
    private static DateTime Yarin(int saat) => DateTime.Today.AddDays(1).AddHours(saat);

    private Task<global::AfetCadirKent.Entities.Kisiler.Gonullu> GonulluAsync(string uzmanlik = "Arama kurtarma", string? telefon = null) =>
        Servis().KaydetAsync(null, new GonulluBilgisi { Ad = "Deniz", Soyad = BenzersizSoyad(), Telefon = telefon, UzmanlikAlani = uzmanlik });

    private static string BenzersizTelefon() => "05" + Random.Shared.NextInt64(100_000_000, 999_999_999);

    [Fact]
    public async Task GonulluEklenirGuncellenir_TelefonTekrariVeEksikAlanReddedilir()
    {
        var telefon = BenzersizTelefon();
        var gonullu = await GonulluAsync("İlk yardım", telefon);

        var guncel = await Servis().KaydetAsync(gonullu.Id, new GonulluBilgisi
        {
            Ad = gonullu.Ad, Soyad = gonullu.Soyad, Telefon = telefon, UzmanlikAlani = "Psikolojik destek", Notlar = "Hafta sonu müsait",
        });
        var ozet = (await Servis().ListeleAsync()).Single(g => g.Id == gonullu.Id);
        Assert.Equal(("Psikolojik destek", "Hafta sonu müsait", 0), (ozet.UzmanlikAlani, ozet.Notlar, ozet.VardiyaSayisi));
        Assert.Equal(gonullu.Id, guncel.Id);

        await Assert.ThrowsAsync<IsKuraliException>(() => GonulluAsync(telefon: telefon));
        await Assert.ThrowsAsync<DogrulamaException>(() => GonulluAsync(uzmanlik: " "));
        await Assert.ThrowsAsync<DogrulamaException>(() => GonulluAsync(telefon: "numara"));
    }

    [Fact]
    public async Task Vardiya_ZamanKurallari_CakismaEngellenir_BitisikVardiyaOlur()
    {
        var gonullu = await GonulluAsync();
        VardiyaBilgisi Bilgi(int bas, int bit) => new() { GonulluId = gonullu.Id, Baslangic = Yarin(bas), Bitis = Yarin(bit), GorevTanimi = "Çadır kurulumu" };

        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().VardiyaKaydetAsync(null, Bilgi(10, 10)));
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().VardiyaKaydetAsync(null, Bilgi(10, 9)));
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().VardiyaKaydetAsync(null, Bilgi(0, 25)));

        var sabah = await Servis().VardiyaKaydetAsync(null, Bilgi(8, 12));
        Assert.Equal($"{gonullu.AdSoyad} – Çadır kurulumu", sabah.Baslik);
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().VardiyaKaydetAsync(null, Bilgi(11, 14)));
        var ogle = await Servis().VardiyaKaydetAsync(null, Bilgi(12, 16)); // bitişik: çakışma sayılmaz

        // Güncellemede vardiya kendisiyle çakışmaz; diğeriyle çakışırsa reddedilir.
        await Servis().VardiyaKaydetAsync(sabah.Id, Bilgi(7, 12));
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().VardiyaKaydetAsync(sabah.Id, Bilgi(7, 13)));

        var takvim = await Servis().VardiyalariGetirAsync(DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));
        Assert.Equal([sabah.Id, ogle.Id], takvim.Where(v => v.GonulluId == gonullu.Id).Select(v => v.Id));
        Assert.Equal(2, (await Servis().ListeleAsync()).Single(g => g.Id == gonullu.Id).VardiyaSayisi);
    }

    [Fact]
    public async Task GorevAtamasi_AcikTalepBaglanir_KapaliTalepVeyaPasifGonulluReddedilir()
    {
        var gonullu = await GonulluAsync();
        var talepServisi = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.Koordinator));
        var acik = await talepServisi.OlusturAsync(new TalepBilgisi { Aciklama = "Bölgede çadır kurulumu yardımı", Kaynak = TalepKaynagi.Gorevli });
        var kapali = await talepServisi.OlusturAsync(new TalepBilgisi { Aciklama = "Eski talep", Kaynak = TalepKaynagi.Gorevli });
        await talepServisi.IptalEtAsync(kapali.Id, "Gerek kalmadı");

        Assert.Contains(await Servis().AtanabilirTaleplerAsync(), t => t.Id == acik.Id);
        Assert.DoesNotContain(await Servis().AtanabilirTaleplerAsync(), t => t.Id == kapali.Id);

        var bolge = await fixture.CadirServisi(RolTuru.Koordinator).BolgeKaydetAsync(null, BenzersizBolgeKodu(), "Gönüllü bölgesi", null);
        var vardiya = await Servis().VardiyaKaydetAsync(null, new VardiyaBilgisi
        {
            GonulluId = gonullu.Id, Baslangic = Yarin(9), Bitis = Yarin(13), BolgeId = bolge.Id, TalepId = acik.Id,
        });
        Assert.Equal((acik.Id, $"{gonullu.AdSoyad} – Talep #{acik.Id}"), (vardiya.TalepId!.Value, vardiya.Baslik));
        Assert.Contains("Gönüllü bölgesi", vardiya.BolgeAdi);

        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().VardiyaKaydetAsync(null, new VardiyaBilgisi
        {
            GonulluId = gonullu.Id, Baslangic = Yarin(14), Bitis = Yarin(16), TalepId = kapali.Id,
        }));

        // Bitmemiş vardiyası olan gönüllü pasif yapılamaz; vardiya silinince yapılır ve pasife vardiya verilmez.
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().AktiflikDegistirAsync(gonullu.Id, false));
        await Servis().VardiyaSilAsync(vardiya.Id);
        await Servis().AktiflikDegistirAsync(gonullu.Id, false);
        Assert.DoesNotContain(await Servis().ListeleAsync(), g => g.Id == gonullu.Id);
        Assert.Contains(await Servis().ListeleAsync(pasifleriDeGoster: true), g => g.Id == gonullu.Id && !g.Aktif);
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().VardiyaKaydetAsync(null, new VardiyaBilgisi
        {
            GonulluId = gonullu.Id, Baslangic = Yarin(9), Bitis = Yarin(10),
        }));
    }

    [Fact]
    public async Task Silme_VardiyasiOlanSilinmez_GecmisVardiyaKayitOlarakKalir()
    {
        var bos = await GonulluAsync();
        await Servis().SilAsync(bos.Id);
        Assert.DoesNotContain(await Servis().ListeleAsync(true), g => g.Id == bos.Id);

        var gonullu = await GonulluAsync();
        var dun = DateTime.Today.AddDays(-1);
        var gecmis = await Servis().VardiyaKaydetAsync(null, new VardiyaBilgisi
        {
            GonulluId = gonullu.Id, Baslangic = dun.AddHours(8), Bitis = dun.AddHours(16), GorevTanimi = "Dağıtım desteği",
        });

        var hata = await Assert.ThrowsAsync<SilinemezException>(() => Servis().SilAsync(gonullu.Id));
        Assert.Equal("Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz.", hata.Message);
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().VardiyaSilAsync(gecmis.Id));
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().VardiyaKaydetAsync(gecmis.Id, new VardiyaBilgisi
        {
            GonulluId = gonullu.Id, Baslangic = Yarin(8), Bitis = Yarin(9),
        }));

        // Geçmiş vardiya pasif yapmayı engellemez.
        await Servis().AktiflikDegistirAsync(gonullu.Id, false);
    }

    [Theory]
    [InlineData(RolTuru.KayitGorevlisi)]
    [InlineData(RolTuru.DepoSorumlusu)]
    [InlineData(RolTuru.SaglikPersoneli)]
    [InlineData(RolTuru.SahaGorevlisi)]
    [InlineData(RolTuru.Kiosk)]
    public async Task GonullulerYalnizcaKoordinatorTarafindanYonetilir(RolTuru rol)
    {
        var servis = fixture.GonulluServisi(rol);
        await Assert.ThrowsAsync<YetkisizIslemException>(() => servis.ListeleAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() => servis.KaydetAsync(null,
            new GonulluBilgisi { Ad = "A", Soyad = "B", UzmanlikAlani = "C" }));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => servis.VardiyalariGetirAsync(DateTime.Today, DateTime.Today.AddDays(1)));
    }
}
