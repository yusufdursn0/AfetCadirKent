using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Tests.Altyapi;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

/// <summary>Refakatsiz çocuk kaydı Koordinatör dışındaki rollere hiçbir serviste görünmez ve işlenemez (K-055, K-068).</summary>
[Collection(VeritabaniKoleksiyonu.Ad)]
public class RefakatsizGizlilikTestleri(VeritabaniFixture fixture)
{
    [Fact]
    public async Task YalnizRefakatsizCocuklardanOlusanAile_KoordinatorDisindaBulunamaz()
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.Koordinator).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Çocuk", soyad, 12, refakatsiz: true)]);

        var kayit = fixture.KayitServisi(RolTuru.KayitGorevlisi);
        var gerekce = fixture.GerekceServisi(RolTuru.KayitGorevlisi);
        var bekleme = fixture.BeklemeListesiServisi(RolTuru.KayitGorevlisi);

        await Assert.ThrowsAsync<KayitBulunamadiException>(() => kayit.AileGetirAsync(aile.Id));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => kayit.AileGuncelleAsync(aile.Id, "Yeni ad", false, null));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => kayit.KisiEkleAsync(aile.Id, Kisi("Yeni", soyad, 30)));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => kayit.AilePasifYapAsync(aile.Id, "Deneme"));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => gerekce.DegerlendirAsync(aile.Id));
        Assert.Empty(await gerekce.DegerlendirAsync([aile.Id]));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() =>
            gerekce.ElleDegistirAsync(aile.Id, GerekceKodu.TekEbeveyn, GerekceDegisiklikTuru.ElleEklendi, "Deneme"));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => gerekce.DegisiklikGecmisiAsync(aile.Id));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => bekleme.EkleAsync(aile.Id, null));

        // Koordinatör listeye alır; diğer rol kaydı göremez ve çıkaramaz.
        var kayitBekleme = await fixture.BeklemeListesiServisi(RolTuru.Koordinator).EkleAsync(aile.Id, "Koordinatör takibi");
        Assert.DoesNotContain(await bekleme.ListeleAsync(), b => b.AileId == aile.Id);
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => bekleme.ListedenCikarAsync(kayitBekleme.Id, "Deneme"));

        // Koordinatör için her şey çalışır.
        Assert.Equal(DestekOnceligi.Yuksek, (await fixture.GerekceServisi(RolTuru.Koordinator).DegerlendirAsync(aile.Id)).DestekOnceligi);
        await fixture.BeklemeListesiServisi(RolTuru.Koordinator).ListedenCikarAsync(kayitBekleme.Id, "Test sonu");
    }

    [Fact]
    public async Task KarmaAiledeRefakatsizCocukKoordinatorDisindaAiledenAyrilamaz()
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.Koordinator).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Abla", soyad, 25), Kisi("Kardeş", soyad, 10, refakatsiz: true), Kisi("Abi", soyad, 27)]);
        var cocukId = aile.Uyeler.Single(u => u.RefakatsizCocuk).Id;

        await Assert.ThrowsAsync<KayitBulunamadiException>(() => fixture.KayitServisi(RolTuru.KayitGorevlisi)
            .AiledenAyirAsync(aile.Id, [cocukId], $"{soyad} Yeni", "Deneme"));

        var yeni = await fixture.KayitServisi(RolTuru.Koordinator).AiledenAyirAsync(aile.Id, [cocukId], $"{soyad} Yeni", "Koordinatör kararı");
        Assert.NotEqual(aile.Id, yeni.Id);
    }
}
