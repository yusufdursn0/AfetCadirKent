using AfetCadirKent.Business.Denetim;
using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class KullaniciVeOturumTestleri(VeritabaniFixture fixture)
{
    private async Task<KullaniciServisi> KoordinatorAsync() =>
        fixture.KullaniciServisi(await fixture.KullaniciOturumuAsync(RolTuru.Koordinator));

    [Fact]
    public async Task EklenenGorevliGirisYapar_RolununYetkileriVeritabanindanYuklenir_ParolaOzetlenir()
    {
        var bilgi = YeniKullanici();
        var eklenen = await (await KoordinatorAsync()).EkleAsync(bilgi, RolTuru.DepoSorumlusu);

        var oturum = new Oturum();
        var giris = await fixture.OturumServisi(oturum).GirisYapAsync($"  {bilgi.KullaniciAdi.ToUpperInvariant()} ", bilgi.Parola);

        Assert.Equal(eklenen.Id, giris.Id);
        Assert.True(oturum.AcikMi);
        Assert.Equal(RolTuru.DepoSorumlusu, oturum.Rol);
        Assert.True(oturum.IzinVarMi(Modul.Dagitim, Izin.Rapor));
        Assert.False(oturum.IzinVarMi(Modul.KullaniciYonetimi, Izin.Gorme));
        Assert.NotNull(giris.SonGirisZamani);

        await using var db = fixture.YeniContext();
        var kayit = await db.Kullanicilar.SingleAsync(k => k.Id == eklenen.Id);
        Assert.StartsWith("$2", kayit.ParolaHash);
        Assert.DoesNotContain(bilgi.Parola, kayit.ParolaHash);
        Assert.True(await db.IslemGecmisi.AnyAsync(i => i.IslemTuru == IslemTuru.Giris && i.KullaniciId == eklenen.Id));

        await fixture.OturumServisi(oturum).CikisYapAsync();
        Assert.False(oturum.AcikMi);
    }

    [Fact]
    public async Task HataliGiris_AyniMesaj_DenemeKaydedilir_PasifKullaniciGiremez()
    {
        var bilgi = YeniKullanici();
        var koordinator = await KoordinatorAsync();
        var eklenen = await koordinator.EkleAsync(bilgi, RolTuru.SahaGorevlisi);
        var oturum = new Oturum();
        var servis = fixture.OturumServisi(oturum);

        var yanlisParola = await Assert.ThrowsAsync<IsKuraliException>(() => servis.GirisYapAsync(bilgi.KullaniciAdi, "Yanlis123"));
        var yanlisAd = await Assert.ThrowsAsync<IsKuraliException>(() => servis.GirisYapAsync(bilgi.KullaniciAdi + "x", bilgi.Parola));
        Assert.Equal("Kullanıcı adı veya parola hatalı.", yanlisParola.Message);
        Assert.Equal(yanlisParola.Message, yanlisAd.Message); // kullanıcı adının var olup olmadığı belli edilmez
        await Assert.ThrowsAsync<DogrulamaException>(() => servis.GirisYapAsync("", ""));
        Assert.False(oturum.AcikMi);

        await koordinator.AktiflikDegistirAsync(eklenen.Id, false);
        var pasif = await Assert.ThrowsAsync<IsKuraliException>(() => servis.GirisYapAsync(bilgi.KullaniciAdi, bilgi.Parola));
        Assert.Contains("pasif", pasif.Message);
        Assert.False(oturum.AcikMi);

        await using var db = fixture.YeniContext();
        Assert.True(await db.IslemGecmisi.AnyAsync(i => i.Aciklama.Contains($"Başarısız giriş denemesi: '{bilgi.KullaniciAdi}'")));
    }

    [Fact]
    public async Task KullaniciEkleme_ParolaKurallariVeTekilKullaniciAdi()
    {
        var koordinator = await KoordinatorAsync();
        await Assert.ThrowsAsync<DogrulamaException>(() => koordinator.EkleAsync(YeniKullanici("zayif"), RolTuru.KayitGorevlisi));
        await Assert.ThrowsAsync<DogrulamaException>(() =>
            koordinator.EkleAsync(YeniKullanici() with { ParolaTekrar = "Baska123" }, RolTuru.KayitGorevlisi));

        var ilk = await koordinator.EkleAsync(YeniKullanici(), RolTuru.KayitGorevlisi);
        var hata = await Assert.ThrowsAsync<IsKuraliException>(() =>
            koordinator.EkleAsync(YeniKullanici(kullaniciAdi: ilk.KullaniciAdi.ToUpperInvariant()), RolTuru.KayitGorevlisi));
        Assert.Contains("kullanılıyor", hata.Message);
    }

    [Fact]
    public async Task ParolaDegistirmeVeSifirlama()
    {
        var bilgi = YeniKullanici();
        var koordinator = await KoordinatorAsync();
        var eklenen = await koordinator.EkleAsync(bilgi, RolTuru.KayitGorevlisi);
        var oturum = new Oturum();
        var servis = fixture.OturumServisi(oturum);
        await servis.GirisYapAsync(bilgi.KullaniciAdi, bilgi.Parola);

        await Assert.ThrowsAsync<IsKuraliException>(() => servis.ParolaDegistirAsync("Yanlis123", "Yeni12345", "Yeni12345"));
        await servis.ParolaDegistirAsync(bilgi.Parola, "Yeni12345", "Yeni12345");
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.GirisYapAsync(bilgi.KullaniciAdi, bilgi.Parola));
        await servis.GirisYapAsync(bilgi.KullaniciAdi, "Yeni12345");

        await koordinator.ParolaSifirlaAsync(eklenen.Id, "Sifir12345", "Sifir12345");
        await servis.GirisYapAsync(bilgi.KullaniciAdi, "Sifir12345");
        Assert.True(oturum.AcikMi);
    }

    [Fact]
    public async Task Silme_KullanilmamisSilinir_GirisYapmisSilinemez_KendiniSilemez()
    {
        var koordinatorOturumu = await fixture.KullaniciOturumuAsync(RolTuru.Koordinator);
        var koordinator = fixture.KullaniciServisi(koordinatorOturumu);

        var hic = await koordinator.EkleAsync(YeniKullanici(), RolTuru.KayitGorevlisi);
        await koordinator.SilAsync(hic.Id);
        Assert.DoesNotContain(await koordinator.ListeleAsync(pasifleriDeGoster: true), k => k.Id == hic.Id);

        var bilgi = YeniKullanici();
        var giren = await koordinator.EkleAsync(bilgi, RolTuru.KayitGorevlisi);
        await fixture.OturumServisi(new Oturum()).GirisYapAsync(bilgi.KullaniciAdi, bilgi.Parola);
        var hata = await Assert.ThrowsAsync<SilinemezException>(() => koordinator.SilAsync(giren.Id));
        Assert.Equal("Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz.", hata.Message);

        await Assert.ThrowsAsync<IsKuraliException>(() => koordinator.SilAsync(koordinatorOturumu.KullaniciId!.Value));
        await Assert.ThrowsAsync<IsKuraliException>(() => koordinator.AktiflikDegistirAsync(koordinatorOturumu.KullaniciId!.Value, false));
    }

    [Fact]
    public async Task AcikTalebiOlanGorevliPasifYapilamaz_RoluTalebiAlamayacakSekildeDegistirilemez()
    {
        var koordinatorOturumu = await fixture.KullaniciOturumuAsync(RolTuru.Koordinator);
        var sahaOturumu = await fixture.KullaniciOturumuAsync(RolTuru.SahaGorevlisi);
        var talepler = fixture.TalepServisi(koordinatorOturumu);
        var talep = await talepler.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Çadır direği kırıldı", Kaynak = TalepKaynagi.Gorevli, Tur = TalepTuru.CadirAriza,
        });
        await talepler.AtaAsync(talep.Id, sahaOturumu.KullaniciId!.Value);

        var kullanicilar = fixture.KullaniciServisi(koordinatorOturumu);
        var sahaId = sahaOturumu.KullaniciId!.Value;
        await Assert.ThrowsAsync<IsKuraliException>(() => kullanicilar.AktiflikDegistirAsync(sahaId, false));
        await Assert.ThrowsAsync<IsKuraliException>(() => kullanicilar.GuncelleAsync(sahaId, "Test", "Saha", null, RolTuru.DepoSorumlusu));
        await kullanicilar.GuncelleAsync(sahaId, "Test", "Saha", "0555 000 00 00", RolTuru.Koordinator); // Koordinatör her talebi alabilir

        await talepler.IptalEtAsync(talep.Id, "Test sonu");
        await kullanicilar.AktiflikDegistirAsync(sahaId, false);
    }

    [Fact]
    public async Task IslemGecmisi_YalnizKoordinatorGorur_Filtrelenir()
    {
        var koordinatorOturumu = await fixture.KullaniciOturumuAsync(RolTuru.Koordinator);
        var eklenen = await fixture.KullaniciServisi(koordinatorOturumu).EkleAsync(YeniKullanici(), RolTuru.KayitGorevlisi);

        var gecmis = fixture.IslemGecmisiServisi(koordinatorOturumu);
        var kayitlar = await gecmis.ListeleAsync(new IslemGecmisiFiltresi
        {
            VarlikAdi = "Gorevli", VarlikId = eklenen.Id, Baslangic = DateOnly.FromDateTime(DateTime.Today),
        });
        var kayit = Assert.Single(kayitlar);
        Assert.Equal((IslemTuru.Olusturma, koordinatorOturumu.Kullanici!.KullaniciAdi), (kayit.IslemTuru, kayit.KullaniciAdi));
        Assert.Contains(eklenen.KullaniciAdi, kayit.Aciklama);
        Assert.Single(await gecmis.ListeleAsync(new IslemGecmisiFiltresi { Metin = $"Kullanıcı eklendi: {eklenen.KullaniciAdi}" }));
        await Assert.ThrowsAsync<DogrulamaException>(() => gecmis.ListeleAsync(new IslemGecmisiFiltresi { EnFazlaKayit = 0 }));

        var kayitGorevlisi = await fixture.KullaniciOturumuAsync(RolTuru.KayitGorevlisi);
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.IslemGecmisiServisi(kayitGorevlisi).ListeleAsync(new IslemGecmisiFiltresi()));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.KullaniciServisi(kayitGorevlisi).ListeleAsync());
    }
}

/// <summary>İlk kurulum: demo verisi kullanıcı içermez; ilk Koordinatör yalnızca bir kez oluşturulur (ayrı veritabanı).</summary>
[Collection(KurulumKoleksiyonu.Ad)]
public class KurulumTestleri(KurulumVeritabaniFixture fixture)
{
    [Fact]
    public async Task IlkYonetici_YalnizcaHicKullaniciYokkenOlusturulur_GirisYapar_SonKoordinatorKorunur()
    {
        var kurulum = fixture.KurulumServisi();
        Assert.True(await kurulum.IlkKurulumGerekliMiAsync()); // migration + demo verisi sonrası bile kullanıcı yok

        await Assert.ThrowsAsync<DogrulamaException>(() => kurulum.IlkYoneticiOlusturAsync(YeniKullanici("123")));
        Assert.True(await kurulum.IlkKurulumGerekliMiAsync());

        var bilgi = YeniKullanici(kullaniciAdi: "yonetici");
        var yonetici = await kurulum.IlkYoneticiOlusturAsync(bilgi);
        Assert.Equal(RolTuru.Koordinator, yonetici.Rol);
        Assert.False(await kurulum.IlkKurulumGerekliMiAsync());
        await Assert.ThrowsAsync<IsKuraliException>(() => kurulum.IlkYoneticiOlusturAsync(YeniKullanici(kullaniciAdi: "ikinci")));

        var oturum = new Oturum();
        await fixture.OturumServisi(oturum).GirisYapAsync("yonetici", bilgi.Parola);
        Assert.All(Enum.GetValues<Modul>(), m => Assert.True(oturum.IzinVarMi(m, Izin.Rapor)));

        // Tek Koordinatörün rolü değiştirilemez; ikinci Koordinatör eklenince değişebilir.
        var kullanicilar = fixture.KullaniciServisi(oturum);
        var hata = await Assert.ThrowsAsync<IsKuraliException>(() =>
            kullanicilar.GuncelleAsync(yonetici.Id, yonetici.Ad, yonetici.Soyad, null, RolTuru.KayitGorevlisi));
        Assert.Contains("en az bir aktif Koordinatör", hata.Message);

        var ikinci = await kullanicilar.EkleAsync(YeniKullanici(kullaniciAdi: "koordinator2"), RolTuru.Koordinator);
        await kullanicilar.GuncelleAsync(ikinci.Id, ikinci.Ad, ikinci.Soyad, null, RolTuru.KayitGorevlisi);
        Assert.Equal(RolTuru.KayitGorevlisi, (await kullanicilar.ListeleAsync()).Single(k => k.Id == ikinci.Id).Rol);

        await using var db = fixture.YeniContext();
        Assert.Equal("sistem", (await db.IslemGecmisi.SingleAsync(i => i.Aciklama.StartsWith("İlk kurulum"))).KullaniciAdi);
    }
}
