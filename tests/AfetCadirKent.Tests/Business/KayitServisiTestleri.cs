using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Cadirlar;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class KayitServisiTestleri(VeritabaniFixture fixture)
{
    private KayitServisi Servis(RolTuru rol = RolTuru.KayitGorevlisi) => fixture.KayitServisi(rol);

    [Fact]
    public async Task AileKaydiNumaraVerirHareketVeIslemGecmisiYazar()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Mehmet", soyad, 34, YeniTc()), Kisi("Elif", soyad, 1)]);

        Assert.Matches(@"^A-\d{6}$", aile.AileNo);
        Assert.All(aile.Uyeler, u => Assert.Matches(@"^K-\d{6}$", u.KisiNo));

        await using var db = fixture.YeniContext();
        Assert.Equal(2, await db.KisiHareketleri.CountAsync(h => h.Kisi!.AileId == aile.Id && h.Tur == KisiHareketTuru.Gelis));
        var kayit = await db.IslemGecmisi.SingleAsync(i => i.VarlikAdi == nameof(Aile) && i.VarlikId == aile.Id);
        Assert.Equal(IslemTuru.Olusturma, kayit.IslemTuru);
        Assert.Equal("test_kayitgorevlisi", kayit.KullaniciAdi);
    }

    [Fact]
    public async Task KimligiOlmayanKisiKaydedilebilir()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Deniz", soyad, 40)]);
        Assert.Null(Assert.Single(aile.Uyeler).TcKimlikNo);
    }

    [Fact]
    public async Task AyniTcIleKayitEngellenir()
    {
        var tc = YeniTc();
        var soyad = BenzersizSoyad();
        await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Ali", soyad, 40, tc)]);

        var hata = await Assert.ThrowsAsync<IsKuraliException>(() =>
            Servis().AileOlusturAsync("Başka Aile", false, [Kisi("Veli", "Başka", 22, tc)]));
        Assert.Contains("ikinci kez kaydedilemez", hata.Message);

        var kontrol = await Servis().MukerrerKontrolAsync(Kisi("Veli", "Başka", 22, tc));
        Assert.True(kontrol.KayitEngellenir);
    }

    [Fact]
    public async Task ListedeTekrarlananVeyaHataliTcReddedilir()
    {
        var tc = YeniTc();
        await Assert.ThrowsAsync<DogrulamaException>(() =>
            Servis().AileOlusturAsync("X Ailesi", false, [Kisi("A", "X", 30, tc), Kisi("B", "X", 28, tc)]));
        await Assert.ThrowsAsync<DogrulamaException>(() =>
            Servis().AileOlusturAsync("X Ailesi", false, [Kisi("A", "X", 30, "12345678901")]));
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().AileOlusturAsync("X Ailesi", false, []));
    }

    [Fact]
    public async Task TcYoksaAdSoyadVeYasaGoreOlasiEslesmeGosterilir()
    {
        var soyad = BenzersizSoyad("Çağ");
        await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Şükrü", soyad, 30)]);
        var normalSoyad = TurkceMetin.Normallestir(soyad).ToUpperInvariant();

        var yakin = await Servis().MukerrerKontrolAsync(Kisi("SUKRU", normalSoyad, 31));
        Assert.False(yakin.KayitEngellenir);
        Assert.True(yakin.UyariVar);

        var uzak = await Servis().MukerrerKontrolAsync(Kisi("Sukru", soyad, 33));
        Assert.False(uzak.UyariVar);
    }

    [Fact]
    public async Task YetkisizRolKayitYapamaz()
    {
        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            Servis(RolTuru.SahaGorevlisi).AileOlusturAsync("Y Ailesi", false, [Kisi("A", "Y", 30)]));
        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            Servis(RolTuru.DepoSorumlusu).AileOlusturAsync("Y Ailesi", false, [Kisi("A", "Y", 30)]));
        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            Servis(RolTuru.SahaGorevlisi).KisileriListeleAsync(new KisiFiltresi()));
    }

    [Fact]
    public async Task RefakatsizCocuguYalnizcaKoordinatorGorur()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Can", soyad, 13, refakatsiz: true)]);
        var filtre = new KisiFiltresi { Metin = soyad };

        Assert.Empty(await Servis().KisileriListeleAsync(filtre));
        Assert.Single(await Servis(RolTuru.Koordinator).KisileriListeleAsync(filtre));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => Servis().AileGetirAsync(aile.Id));
        Assert.Single((await Servis(RolTuru.Koordinator).AileGetirAsync(aile.Id)).Uyeler);
    }

    [Fact]
    public async Task SaglikNotunuYalnizcaYetkiliGirerVeGorur()
    {
        var soyad = BenzersizSoyad();
        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("A", soyad, 50, saglikNotu: "Şeker hastası")]));

        var aile = await Servis(RolTuru.Koordinator).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("A", soyad, 50, saglikNotu: "Şeker hastası")]);
        var filtre = new KisiFiltresi { AileId = aile.Id };

        Assert.Null(Assert.Single(await Servis().KisileriListeleAsync(filtre)).SaglikNotu);
        Assert.Equal("Şeker hastası", Assert.Single(await Servis(RolTuru.SaglikPersoneli).KisileriListeleAsync(filtre)).SaglikNotu);
    }

    [Fact]
    public async Task TurkceKarakterFarkiYokSayilarakAranir()
    {
        var soyad = BenzersizSoyad("Öz");
        await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Gül", soyad, 40)]);
        var sonuc = await Servis().KisileriListeleAsync(new KisiFiltresi { Metin = $"gul {TurkceMetin.Normallestir(soyad)}" });
        Assert.Single(sonuc);
    }

    [Fact]
    public async Task HastanedekiKisininYeriKorunur_AyrilanKisininYeriBosalir()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("A", soyad, 40), Kisi("B", soyad, 38)]);
        var cadirId = await TestCadirinaYerlestirAsync(aile);
        var (a, b) = (aile.Uyeler[0].Id, aile.Uyeler[1].Id);

        await Servis().DurumDegistirAsync(a, KisiDurumu.Hastanede, "Hastaneye sevk");
        Assert.Equal(2, await DolulukAsync(cadirId));

        await Servis().DurumDegistirAsync(a, KisiDurumu.Burada, null);
        await Servis().DurumDegistirAsync(b, KisiDurumu.Ayrildi, "Akrabasının yanına gitti");
        Assert.Equal(1, await DolulukAsync(cadirId));

        var hareketler = await Servis().KisiHareketleriAsync(a);
        Assert.Equal(
            [KisiHareketTuru.Gelis, KisiHareketTuru.HastaneyeSevk, KisiHareketTuru.HastanedenDonus],
            hareketler.Select(h => h.Tur));
        var ayrilis = (await Servis().KisiHareketleriAsync(b)).Last();
        Assert.Equal(KisiHareketTuru.Ayrilis, ayrilis.Tur);
        Assert.Equal(cadirId, ayrilis.EskiCadirId);

        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().DurumDegistirAsync(b, KisiDurumu.Hastanede, null));
    }

    [Fact]
    public async Task AiledenAyirmaYeniAileOlusturur_GerekceVeHaklarIkiAileIcinYenidenHesaplanir()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false,
        [
            Kisi("Hasan", soyad, 55), Kisi("Fatma", soyad, 52), Kisi("Emre", soyad, 26), Kisi("Büşra", soyad, 24, hamile: true),
        ]);
        var gerekceServisi = fixture.GerekceServisi(RolTuru.KayitGorevlisi);
        Assert.Contains(GerekceKodu.Hamile, (await gerekceServisi.DegerlendirAsync(aile.Id)).Gerekceler.Select(g => g.Kod));

        var tasinacaklar = aile.Uyeler.Where(u => u.Ad is "Emre" or "Büşra").Select(u => u.Id).ToList();
        var yeni = await Servis().AiledenAyirAsync(aile.Id, tasinacaklar, $"{soyad} (Emre) Ailesi", "Evli çocuk eşiyle ayrı çadıra geçiyor");

        Assert.NotEqual(aile.AileNo, yeni.AileNo);
        Assert.Equal(2, (await Servis().AileGetirAsync(yeni.Id)).Uyeler.Count);
        Assert.Equal(2, (await Servis().AileGetirAsync(aile.Id)).Uyeler.Count);

        var eskiSonuc = await gerekceServisi.DegerlendirAsync(aile.Id);
        var yeniSonuc = await gerekceServisi.DegerlendirAsync(yeni.Id);
        Assert.Equal(DestekOnceligi.Normal, eskiSonuc.DestekOnceligi);
        Assert.Equal(DestekOnceligi.Yuksek, yeniSonuc.DestekOnceligi);

        var gecmis = Assert.Single(await Servis().AileGecmisiAsync(yeni.Id));
        Assert.Equal(AileGecmisiTuru.Ayirma, gecmis.Tur);
        Assert.Equal(aile.Id, gecmis.KaynakAileId);
        Assert.Equal(KisiHareketTuru.AileDegisikligi, (await Servis().KisiHareketleriAsync(tasinacaklar[0])).Last().Tur);
    }

    [Fact]
    public async Task AiledenAyirmaKurallari()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("A", soyad, 40), Kisi("B", soyad, 38)]);
        var baska = await Servis().AileOlusturAsync($"{soyad}2 Ailesi", false, [Kisi("C", soyad, 30)]);
        var tumu = aile.Uyeler.Select(u => u.Id).ToList();

        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().AiledenAyirAsync(aile.Id, tumu, "Yeni", "gerekçe"));
        await Assert.ThrowsAsync<IsKuraliException>(() =>
            Servis().AiledenAyirAsync(aile.Id, [baska.Uyeler[0].Id], "Yeni", "gerekçe"));
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().AiledenAyirAsync(aile.Id, [tumu[0]], "Yeni", " "));
    }

    [Fact]
    public async Task KisiPasifYapilirFizikselSilinmez_AileAktifUyeyleyPasifYapilamaz()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("A", soyad, 40)]);
        var cadirId = await TestCadirinaYerlestirAsync(aile);
        var kisiId = aile.Uyeler[0].Id;

        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().AilePasifYapAsync(aile.Id, "Kapatıldı"));

        await Servis().KisiPasifYapAsync(kisiId, "Hatalı kayıt");
        await using (var db = fixture.YeniContext())
        {
            var kisi = await db.Kisiler.SingleAsync(k => k.Id == kisiId);
            Assert.False(kisi.Aktif);
        }
        Assert.Equal(0, await DolulukAsync(cadirId));

        await Servis().AilePasifYapAsync(aile.Id, "Tüm üyeler pasif");
        Assert.DoesNotContain(await Servis().AileleriListeleAsync(soyad), a => a.Id == aile.Id);
    }

    [Fact]
    public async Task UzunMetinler_KullaniciyaAnlasilirMesaj_SistemMetniSutunaSigar()
    {
        var soyad = BenzersizSoyad();
        var uzunNot = new string('n', 1001);
        await Assert.ThrowsAsync<DogrulamaException>(() =>
            Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Uzun", soyad, 30)], uzunNot));

        // 500 karakterlik açıklama + sistemin eklediği ön ek, 500'lük hareket sütununa kısaltılarak yazılır.
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Uzun", soyad, 30), Kisi("Kalan", soyad, 32)]);
        var bolge = await fixture.CadirServisi(RolTuru.KayitGorevlisi).BolgeKaydetAsync(null, BenzersizBolgeKodu(), "Uzun metin", null);
        var cadir = await fixture.CadirServisi(RolTuru.KayitGorevlisi).CadirEkleAsync(
            new() { BolgeId = bolge.Id, Sira = 1, Numara = 1, Kapasite = 4 });
        await fixture.CadirServisi(RolTuru.KayitGorevlisi).YerlestirAsync(aile.Id, [new(cadir.Id, "?", 2)]);

        await Servis().KisiPasifYapAsync(aile.Uyeler[0].Id, new string('a', 500));
        await using var db = fixture.YeniContext();
        var hareket = await db.KisiHareketleri.Where(h => h.KisiId == aile.Uyeler[0].Id && h.Tur == KisiHareketTuru.Ayrilis).SingleAsync();
        Assert.Equal(500, hareket.Aciklama!.Length);
        Assert.EndsWith("…", hareket.Aciklama);
    }

    [Fact]
    public async Task VeritabaniUzunlukHatasiAnlasilirMesajaCevrilir()
    {
        await using var veri = fixture.OturumFabrikasi().Ac();
        veri.Repo<Bolge>().Ekle(new Bolge { Kod = BenzersizBolgeKodu(), Ad = "Test", Aciklama = new string('x', 600) });

        var hata = await Assert.ThrowsAsync<DogrulamaException>(() => veri.GuvenliKaydetAsync());
        Assert.Contains("uzunluğu aşıyor", hata.Message);
    }

    [Fact]
    public async Task GerekceElleKaldirilirGeriAlinirVeGecmisTutulur()
    {
        var soyad = BenzersizSoyad();
        var aile = await Servis().AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Anne", soyad, 30), Kisi("Bebek", soyad, 1)]);
        var servis = fixture.GerekceServisi(RolTuru.KayitGorevlisi);

        var ilk = await servis.DegerlendirAsync(aile.Id);
        Assert.Equal(DestekOnceligi.Yuksek, ilk.DestekOnceligi);
        Assert.Contains("Bebek maması ✔", ilk.MalzemeIhtiyaclariMetni);
        Assert.Contains("Bebek bezi ✔", ilk.MalzemeIhtiyaclariMetni);
        Assert.DoesNotContain("Su ✔", ilk.MalzemeIhtiyaclariMetni);

        await Assert.ThrowsAsync<DogrulamaException>(() =>
            servis.ElleDegistirAsync(aile.Id, GerekceKodu.Bebek, GerekceDegisiklikTuru.ElleKaldirildi, ""));
        await servis.ElleDegistirAsync(aile.Id, GerekceKodu.Bebek, GerekceDegisiklikTuru.ElleKaldirildi, "Bebek akrabada kalıyor");
        Assert.Equal(DestekOnceligi.Normal, (await servis.DegerlendirAsync(aile.Id)).DestekOnceligi);

        await servis.ElleDegisikligiGeriAlAsync(aile.Id, GerekceKodu.Bebek, "Bebek geri döndü");
        Assert.Equal(DestekOnceligi.Yuksek, (await servis.DegerlendirAsync(aile.Id)).DestekOnceligi);

        var gecmis = await servis.DegisiklikGecmisiAsync(aile.Id);
        var degisiklik = Assert.Single(gecmis);
        Assert.False(degisiklik.Gecerli);
        Assert.Equal("Bebek akrabada kalıyor", degisiklik.Aciklama);

        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.GerekceServisi(RolTuru.DepoSorumlusu)
            .ElleDegistirAsync(aile.Id, GerekceKodu.Hamile, GerekceDegisiklikTuru.ElleEklendi, "deneme"));
    }

    [Fact]
    public async Task DemoAilesiIcinIkiAyriBilgiGosterilir()
    {
        await using var db = fixture.YeniContext();
        var yilmaz = await db.Aileler.FirstAsync(a => a.AileAdi == "Yılmaz Ailesi");

        var sonuc = await fixture.GerekceServisi(RolTuru.KayitGorevlisi).DegerlendirAsync(yilmaz.Id);
        Assert.Equal("Destek önceliği: Yüksek (0–2 yaş bebek var, hamile birey var)", sonuc.DestekOnceligiMetni);
        Assert.Equal("Malzeme ihtiyaçları: Battaniye ✔ · Bebek bezi ✔ · Bebek maması ✔ · Ekmek ✔ · Hijyen paketi ✔ · Konserve gıda ✔ · Kuru gıda kolisi ✔",
            sonuc.MalzemeIhtiyaclariMetni);
    }

    private async Task<int> TestCadirinaYerlestirAsync(Aile aile)
    {
        await using var db = fixture.YeniContext();
        var bolge = await db.Bolgeler.FirstOrDefaultAsync(b => b.Kod == "TKS") ?? new Bolge { Kod = "TKS", Ad = "Kayıt testleri" };
        var numara = await db.Cadirlar.CountAsync(c => c.Bolge!.Kod == "TKS") + 1;
        var cadir = new Cadir { Bolge = bolge, Sira = 1, Numara = numara, Kapasite = 8 };
        cadir.Yerlestir(aile.Uyeler.Count);
        db.Cadirlar.Add(cadir);
        foreach (var uye in aile.Uyeler)
            db.Yerlesimler.Add(new Yerlesim { KisiId = uye.Id, Cadir = cadir, Baslangic = DateTime.Now });
        await db.SaveChangesAsync();
        return cadir.Id;
    }

    private async Task<int> DolulukAsync(int cadirId)
    {
        await using var db = fixture.YeniContext();
        return await db.Cadirlar.Where(c => c.Id == cadirId).Select(c => c.Doluluk).SingleAsync();
    }
}
