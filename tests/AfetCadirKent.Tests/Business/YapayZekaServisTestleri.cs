using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

/// <summary>Talep servisinin yapay zeka önerisi akışı ve koordinatörün günlük özeti (test veritabanı + sahte yapay zeka).</summary>
[Collection(VeritabaniKoleksiyonu.Ad)]
public class YapayZekaServisTestleri(VeritabaniFixture fixture)
{
    private async Task<TalepOzeti> KayitMasasiTalebiAsync(string aciklama)
    {
        var kayit = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.KayitGorevlisi));
        return await kayit.OlusturAsync(new TalepBilgisi { Aciklama = aciklama, Kaynak = TalepKaynagi.KayitMasasi });
    }

    private async Task<TalepServisi> KoordinatorAsync(IYapayZekaServisi yapayZeka) =>
        fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.Koordinator), yapayZeka: yapayZeka);

    [Fact]
    public async Task Senaryo4_YapayZekaOnerir_GorevliOnaylar_TalepYonlendirilir()
    {
        var talep = await KayitMasasiTalebiAsync("Çadır su alıyor");
        var yz = new SahteYapayZeka(new TalepSiniflandirmaOnerisi(TalepTuru.CadirAriza, DestekOnceligi.Yuksek, "Barınma bozuluyor."));
        var koordinator = await KoordinatorAsync(yz);

        var sonuc = await koordinator.YapayZekaOnerisiAlAsync(talep.Id);

        Assert.True(sonuc.Basarili);
        Assert.Equal(["Çadır su alıyor"], yz.GelenTalepler);
        Assert.Equal(SiniflandirmaDurumu.YapayZekaOnerdi, sonuc.Talep.SiniflandirmaDurumu);
        Assert.Equal((TalepTuru?)TalepTuru.CadirAriza, sonuc.Talep.YzOnerilenTur);
        Assert.Equal(DestekOnceligi.Yuksek, sonuc.Talep.YzOnerilenOncelik);
        Assert.Equal("Barınma bozuluyor.", sonuc.Talep.YzGerekce);
        Assert.Null(sonuc.Talep.Tur); // son karar görevlide
        Assert.Contains(await koordinator.ListeleAsync(new TalepFiltresi { SadeceSiniflandirilmamis = true }), t => t.Id == talep.Id);

        await koordinator.YapayZekaOnerisiniOnaylaAsync(talep.Id);

        var onaylanan = await koordinator.GetirAsync(talep.Id);
        Assert.Equal((TalepTuru.CadirAriza, RolTuru.SahaGorevlisi, SiniflandirmaDurumu.GorevliBelirledi, "Yardım önceliği: Yüksek"),
            (onaylanan.Tur!.Value, onaylanan.HedefRol!.Value, onaylanan.SiniflandirmaDurumu, onaylanan.OncelikMetni));

        await using var db = fixture.YeniContext();
        var kayitlar = await db.IslemGecmisi.Where(i => i.VarlikAdi == "Talep" && i.VarlikId == talep.Id)
            .OrderBy(i => i.Id).Select(i => i.Aciklama).ToListAsync();
        Assert.Contains(kayitlar, k => k.Contains("yapay zeka önerisi: Çadır/arıza"));
        Assert.Contains(kayitlar, k => k.Contains("yapay zeka önerisi onaylandı"));
    }

    [Fact]
    public async Task Senaryo5_CocukAtesleniyor_OncelikAlmadanSaglikPersonelineGider()
    {
        var talep = await KayitMasasiTalebiAsync("Çocuk ateşleniyor");
        var koordinator = await KoordinatorAsync(
            new SahteYapayZeka(new TalepSiniflandirmaOnerisi(TalepTuru.Saglik, DestekOnceligi.Yuksek, "Ateş")));

        var sonuc = await koordinator.YapayZekaOnerisiAlAsync(talep.Id);

        Assert.Equal((TalepTuru.Saglik, RolTuru.SaglikPersoneli, true, (DestekOnceligi?)null, (DestekOnceligi?)null),
            (sonuc.Talep.Tur!.Value, sonuc.Talep.HedefRol!.Value, sonuc.Talep.SaglikPersoneliDegerlendirecek,
                sonuc.Talep.YardimOnceligi, sonuc.Talep.YzOnerilenOncelik));
        Assert.Equal("Sağlık personeli değerlendirecek", sonuc.Talep.OncelikMetni);

        // Görevli onayı beklenmeden sağlık personelinin listesine düşer.
        var saglik = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.SaglikPersoneli));
        Assert.Contains(await saglik.ListeleAsync(new TalepFiltresi()), t => t.Id == talep.Id);
    }

    [Fact]
    public async Task OllamaKapali_TalepSiniflandirilmamisKalir_GorevliElleDevamEder()
    {
        var talep = await KayitMasasiTalebiAsync("Battaniye lazım");
        var koordinator = await KoordinatorAsync(new SahteYapayZeka()); // ulaşılamadı döner

        var sonuc = await koordinator.YapayZekaOnerisiAlAsync(talep.Id);

        Assert.False(sonuc.Basarili);
        Assert.Contains("ulaşılamadı", sonuc.HataMesaji);
        Assert.Equal((SiniflandirmaDurumu.Siniflandirilmamis, (TalepTuru?)null, (TalepTuru?)null),
            (sonuc.Talep.SiniflandirmaDurumu, sonuc.Talep.Tur, sonuc.Talep.YzOnerilenTur));
        Assert.Equal("Sınıflandırılmamış", sonuc.Talep.TurMetni);
        await Assert.ThrowsAsync<IsKuraliException>(() => koordinator.YapayZekaOnerisiniOnaylaAsync(talep.Id));

        await koordinator.SiniflandirAsync(talep.Id, TalepTuru.Malzeme, DestekOnceligi.Normal);
        Assert.Equal(SiniflandirmaDurumu.GorevliBelirledi, (await koordinator.GetirAsync(talep.Id)).SiniflandirmaDurumu);
    }

    [Fact]
    public async Task GorevliFarkliKararVerirse_KendiKarariGecerli()
    {
        var talep = await KayitMasasiTalebiAsync("Isıtıcı çalışmıyor");
        var koordinator = await KoordinatorAsync(new SahteYapayZeka(new TalepSiniflandirmaOnerisi(TalepTuru.Diger, DestekOnceligi.Normal, null)));
        await koordinator.YapayZekaOnerisiAlAsync(talep.Id);

        await koordinator.SiniflandirAsync(talep.Id, TalepTuru.CadirAriza, DestekOnceligi.Orta);

        var sonuc = await koordinator.GetirAsync(talep.Id);
        Assert.Equal((TalepTuru.CadirAriza, DestekOnceligi.Orta, (TalepTuru?)TalepTuru.Diger),
            (sonuc.Tur!.Value, sonuc.YardimOnceligi!.Value, sonuc.YzOnerilenTur));
    }

    [Fact]
    public async Task SiniflandirilmisTalep_YapayZekayaGonderilmez()
    {
        var talep = await KayitMasasiTalebiAsync("Su bitti");
        var yz = new SahteYapayZeka(new TalepSiniflandirmaOnerisi(TalepTuru.Malzeme, null, null));
        var koordinator = await KoordinatorAsync(yz);
        await koordinator.SiniflandirAsync(talep.Id, TalepTuru.Malzeme, DestekOnceligi.Normal);

        await Assert.ThrowsAsync<IsKuraliException>(() => koordinator.YapayZekaOnerisiAlAsync(talep.Id));
        Assert.Empty(yz.GelenTalepler);
    }

    [Fact]
    public async Task YetkisizRol_YapayZekaOnerisiAlamaz()
    {
        var talep = await KayitMasasiTalebiAsync("Çadır yırtıldı");
        var kiosk = fixture.TalepServisi(OturumAc(RolTuru.Kiosk), yapayZeka: new SahteYapayZeka());
        await Assert.ThrowsAsync<YetkisizIslemException>(() => kiosk.YapayZekaOnerisiAlAsync(talep.Id));

        // Saha görevlisi yalnızca kendine atanan talepleri görür.
        var saha = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.SahaGorevlisi), yapayZeka: new SahteYapayZeka());
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => saha.YapayZekaOnerisiAlAsync(talep.Id));
    }

    // ---------------------------------------------------------------- Günlük özet

    private GunlukOzetServisi OzetServisi(RolTuru rol, IYapayZekaServisi yapayZeka)
    {
        var oturum = OturumAc(rol);
        return new GunlukOzetServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System,
            new StokServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, new DepoAyarlari()), yapayZeka);
    }

    [Fact]
    public async Task GunlukDurum_VeritabanindakiSayilarlaUyusur()
    {
        await KayitMasasiTalebiAsync("Özet testi talebi");
        var durum = await OzetServisi(RolTuru.Koordinator, new SahteYapayZeka()).DurumuGetirAsync();

        await using var db = fixture.YeniContext();
        var bugun = DateTime.Today;
        Assert.Equal(await db.Kisiler.CountAsync(k => k.Aktif && k.Durum == KisiDurumu.Burada), durum.BuradakiKisi);
        Assert.Equal(await db.Kisiler.CountAsync(k => k.Aktif && k.Durum == KisiDurumu.Hastanede), durum.HastanedekiKisi);
        var cadirlar = await db.Cadirlar.Where(c => c.Aktif && c.Durum == CadirDurumu.Kullanilabilir).ToListAsync();
        Assert.Equal(cadirlar.Sum(c => c.Kapasite - c.Doluluk), durum.BosYatak);
        Assert.Equal(cadirlar.Sum(c => c.Kapasite), durum.ToplamKapasite);
        Assert.Equal(await db.BeklemeListesi.CountAsync(b => b.CikisZamani == null), durum.BekleyenAile);
        Assert.Equal(await db.Talepler.CountAsync(t => t.Durum != TalepDurumu.Tamamlandi && t.Durum != TalepDurumu.Iptal),
            durum.AcikTalepToplami);
        Assert.True(durum.AcikTalepler["Sınıflandırılmamış"] >= 1);
        Assert.Equal(await db.Talepler.CountAsync(t => t.AcilisZamani >= bugun), durum.BugunAcilanTalep);

        // Demo verisinde SKT'si yaklaşmış bir parti vardır (şartname 3.9).
        Assert.NotEmpty(durum.SktYaklasanlar);
    }

    [Fact]
    public async Task GunlukOzet_YapayZekaMetniniDurumlaBirlikteDoner()
    {
        var yz = new SahteYapayZeka(ozet: "Çadır kentte durum sakin.");

        var ozet = await OzetServisi(RolTuru.Koordinator, yz).OlusturAsync();

        Assert.True(ozet.YapayZekaYazdi);
        Assert.Equal("Çadır kentte durum sakin.", ozet.Metin);
        Assert.Same(yz.GelenDurum, ozet.Durum);
    }

    [Fact]
    public async Task GunlukOzet_OllamaKapaliysaDurumYineGosterilir()
    {
        var ozet = await OzetServisi(RolTuru.Koordinator, new SahteYapayZeka()).OlusturAsync();

        Assert.False(ozet.YapayZekaYazdi);
        Assert.Contains("ulaşılamadı", ozet.HataMesaji);
        Assert.True(ozet.Durum.ToplamKapasite > 0);
    }

    [Theory]
    [InlineData(RolTuru.KayitGorevlisi)]
    [InlineData(RolTuru.DepoSorumlusu)]
    [InlineData(RolTuru.SaglikPersoneli)]
    [InlineData(RolTuru.SahaGorevlisi)]
    [InlineData(RolTuru.Kiosk)]
    public async Task GunlukOzet_YalnizcaKoordinatorIcindir(RolTuru rol)
    {
        var yz = new SahteYapayZeka(ozet: "x");
        await Assert.ThrowsAsync<YetkisizIslemException>(() => OzetServisi(rol, yz).OlusturAsync());
        Assert.Null(yz.GelenDurum);
    }
}
