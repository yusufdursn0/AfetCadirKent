using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Entities.Depo;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.Entities.Ortak;

namespace AfetCadirKent.Tests.Business;

/// <summary>Veritabanısız depo kuralları: FEFO, hak hesabı, tekrar alma süresi, stok tahmini, ayırma ve hareket geri alma.</summary>
public class DepoKuralTestleri
{
    private static readonly DateOnly Bugun = new(2026, 10, 17);

    private static Parti Parti(int id, decimal miktar, int? sktGun, int girisGunOnce = 5)
    {
        var p = new Parti
        {
            Id = id, PartiNo = $"P{id}", SonKullanmaTarihi = sktGun is { } g ? Bugun.AddDays(g) : null,
            GirisZamani = Bugun.AddDays(-girisGunOnce).ToDateTime(new TimeOnly(9, 0)),
        };
        p.MiktarEkle(miktar);
        return p;
    }

    private static Urun Urun(HesaplamaTuru tur, decimal hak, int tekrar = 1) =>
        new() { Id = 1, Ad = "Test", HesaplamaTuru = tur, HakMiktari = hak, TekrarAlmaGunu = tekrar };

    private static Afetzede Kisi(int yas, KisiDurumu durum = KisiDurumu.Burada, bool aktif = true) => new()
    {
        Ad = "Ad", Soyad = "Soyad", DogumTarihi = Bugun.AddYears(-yas).AddDays(-10), Durum = durum, Aktif = aktif,
    };

    // ---------------------------------------------------------------- FEFO

    [Fact]
    public void Fefo_SktEnYakinPartiOnceGelir_SktsizEnSonda_SktGecmisDislanir()
    {
        var partiler = new[] { Parti(1, 10, null), Parti(2, 10, 20), Parti(3, 10, 3), Parti(4, 10, -1), Parti(5, 10, 3, girisGunOnce: 9) };

        var sira = FefoSecici.UygunPartiler(partiler, Bugun).Select(p => p.Id).ToList();

        Assert.Equal([5, 3, 2, 1], sira); // eşit SKT'de önce giren; SKT'siz en sonda; 4 (SKT geçmiş) yok
    }

    [Fact]
    public void Fefo_MiktarPartilereBolunur_AyrilanMiktarDusulur()
    {
        var yakin = Parti(1, 10, 2);
        yakin.Ayir(6, Bugun); // kullanılabilir 4
        var uzak = Parti(2, 50, 30);

        var paylar = FefoSecici.Sec([uzak, yakin], 10, Bugun);

        Assert.Equal(new[] { (1, 4m), (2, 6m) }, paylar.Select(p => (p.Parti.Id, p.Miktar)));
    }

    [Fact]
    public void Fefo_YetersizStoktaAnlasilirHata_SktGecmisStokSayilmaz()
    {
        var hata = Assert.Throws<IsKuraliException>(() => FefoSecici.Sec([Parti(1, 5, 1), Parti(2, 100, -2)], 6, Bugun));
        Assert.Contains("Yetersiz stok", hata.Message);
        Assert.Contains("Kullanılabilir: 5", hata.Message);
    }

    [Fact]
    public void SktGecmisPartidenAyirmaYapilamaz()
    {
        var hata = Assert.Throws<IsKuraliException>(() => Parti(1, 10, -1).Ayir(1, Bugun));
        Assert.Contains("son kullanma tarihi geçmiş", hata.Message);
        Assert.Equal(0, Parti(2, 10, -1).KullanilabilirMiktar(Bugun));
    }

    // ---------------------------------------------------------------- Hak

    [Fact]
    public void Hak_KisiBasi_AileBasi_BebekBasi()
    {
        var uyeler = new[] { Kisi(35), Kisi(33), Kisi(1), Kisi(0), Kisi(8) };

        Assert.Equal(15, HakHesaplayici.Hesapla(Urun(HesaplamaTuru.KisiBasi, 3), uyeler, Bugun)); // su: 5 × 3 lt
        Assert.Equal(1, HakHesaplayici.Hesapla(Urun(HesaplamaTuru.AileBasi, 1), uyeler, Bugun));  // hijyen paketi
        Assert.Equal(2, HakHesaplayici.Hesapla(Urun(HesaplamaTuru.BebekBasi, 1), uyeler, Bugun)); // mama: 2 bebek
        Assert.Equal(0, HakHesaplayici.Hesapla(Urun(HesaplamaTuru.BebekBasi, 1), [Kisi(30)], Bugun));
    }

    [Fact]
    public void Hak_HastanedekiUyeSayilir_AyrilanVePasifUyeSayilmaz()
    {
        var uyeler = new[]
        {
            Kisi(40), Kisi(70, KisiDurumu.Hastanede), Kisi(20, KisiDurumu.Ayrildi), Kisi(18, KisiDurumu.Nakil), Kisi(10, aktif: false),
        };

        Assert.Equal(2, HakHesaplayici.Hesapla(Urun(HesaplamaTuru.KisiBasi, 1), uyeler, Bugun));
        Assert.Equal(0, HakHesaplayici.Hesapla(Urun(HesaplamaTuru.AileBasi, 1), [Kisi(20, KisiDurumu.Ayrildi)], Bugun));
    }

    [Fact]
    public void TekrarAlmaSuresi_DolmadanYeniHakDogmaz()
    {
        var mama = Urun(HesaplamaTuru.BebekBasi, 1, tekrar: 3);

        Assert.True(HakHesaplayici.TekrarAlabilirMi(mama, null, Bugun));                // hiç almamış
        Assert.False(HakHesaplayici.TekrarAlabilirMi(mama, Bugun.AddDays(-2), Bugun));  // 2 gün önce aldı
        Assert.True(HakHesaplayici.TekrarAlabilirMi(mama, Bugun.AddDays(-3), Bugun));   // süre doldu
        Assert.Equal(Bugun.AddDays(1), HakHesaplayici.SonrakiHakTarihi(mama, Bugun.AddDays(-2), Bugun));
        Assert.Null(HakHesaplayici.SonrakiHakTarihi(mama, Bugun.AddDays(-3), Bugun));
    }

    // ---------------------------------------------------------------- Stok tahmini

    [Fact]
    public void Tahmin_VeriYoksaHesaplanamiyor()
    {
        var t = StokTahminMotoru.Hesapla(Urun(HesaplamaTuru.KisiBasi, 3), 900, [], 0, Bugun);

        Assert.False(t.TuketimeGore.HesaplanabildiMi);
        Assert.Equal("hesaplanamıyor", t.TuketimeGore.Metin);
        Assert.False(t.HakaGore.HesaplanabildiMi);
        Assert.Null(t.DonemBaslangici);
        Assert.Equal("Dönem: dağıtım verisi yok", t.DonemMetni);
    }

    [Fact]
    public void Tahmin_TuketimeVeHakaGore_DonemGosterilir_EskiVeriYokSayilir()
    {
        var dagitimlar = new[]
        {
            (Bugun.AddDays(-3), 100m), (Bugun.AddDays(-2), 120m), (Bugun.AddDays(-1), 80m), (Bugun.AddDays(-30), 5000m),
        };

        var t = StokTahminMotoru.Hesapla(Urun(HesaplamaTuru.KisiBasi, 3, tekrar: 1), 600, dagitimlar, 150, Bugun);

        // Dönem: 3 gün önce → bugün = 4 gün; günlük ortalama 300/4 = 75 → 600/75 = 8 gün
        Assert.Equal(Bugun.AddDays(-3), t.DonemBaslangici);
        Assert.Equal(Bugun, t.DonemBitisi);
        Assert.Equal(75, t.TuketimeGore.GunlukMiktar);
        Assert.Equal(8, t.TuketimeGore.YeterlilikGunu);
        Assert.Contains("(4 gün)", t.DonemMetni);
        // Hakka göre: günlük ihtiyaç 150 / 1 gün → 600/150 = 4 gün
        Assert.Equal(4, t.HakaGore.YeterlilikGunu);
        Assert.Equal("4 gün", t.HakaGore.Metin);
    }

    [Fact]
    public void Tahmin_HakaGore_TekrarAlmaSuresiylebolunur()
    {
        var t = StokTahminMotoru.Hesapla(Urun(HesaplamaTuru.KisiBasi, 1, tekrar: 30), 100, [], 300, Bugun);

        Assert.Equal(10, t.HakaGore.GunlukMiktar);
        Assert.Equal(10, t.HakaGore.YeterlilikGunu);
        Assert.False(t.TuketimeGore.HesaplanabildiMi);
    }

    // ---------------------------------------------------------------- Ayırma ve hareketler

    [Fact]
    public void Ayirma_KismiTeslimSonraSerbestBirakma_StokHesabiTutar()
    {
        var parti = Parti(1, 100, 10);
        parti.Ayir(10, Bugun);
        var ayirma = new StokAyirma { Parti = parti, Miktar = 10 };
        Assert.Equal(90, parti.KullanilabilirMiktar(Bugun));

        ayirma.TeslimEt(4, DateTime.Now);
        new DagitimHareketi { Miktar = 4 }.PartiyeUygula(parti);
        Assert.Equal(96, parti.Miktar);
        Assert.Equal(6, parti.AyrilanMiktar);
        Assert.Equal(AyirmaDurumu.Acik, ayirma.Durum);

        var serbest = ayirma.SerbestBirak(DateTime.Now);
        parti.AyirmaKaldir(serbest);
        Assert.Equal(6, serbest);
        Assert.Equal(AyirmaDurumu.Kapandi, ayirma.Durum);
        Assert.Equal(0, ayirma.AcikMiktar);
        Assert.Equal(96, parti.KullanilabilirMiktar(Bugun));
        Assert.Equal(0, ayirma.SerbestBirak(DateTime.Now)); // ikinci kez serbest bırakılacak bir şey yok
    }

    [Fact]
    public void Ayirma_AcikMiktardanFazlaTeslimEdilemez()
    {
        var ayirma = new StokAyirma { Miktar = 5 };
        Assert.Throws<IsKuraliException>(() => ayirma.TeslimEt(6, DateTime.Now));
    }

    [Fact]
    public void KismiTeslim_KalanBekleyenHakOlarakKalir()
    {
        var detay = new DagitimDetay { HakMiktari = 6 };
        detay.TeslimKaydet(4);

        Assert.Equal(2, detay.BekleyenHak);
        Assert.False(detay.TamamlandiMi);
        Assert.Throws<IsKuraliException>(() => detay.TeslimKaydet(3));
    }

    [Theory]
    [InlineData(2, 0, 1, true, 1)]  // yeni hak 2, 1 teslim → kalan 1 devreder, dönem başladı
    [InlineData(2, 0, 0, false, 0)] // hiç gelmedi → devretmez (yeni dönemde tam hak alır)
    [InlineData(1, 1, 0, false, 1)] // yalnız devreden vardı, gelmedi → devreden kaybolmaz
    [InlineData(1, 1, 1, false, 0)] // yalnız devreden teslim alındı → süre yeniden başlamaz
    [InlineData(3, 1, 1, false, 0)] // teslim önce devredene sayılır; yeni 2 alınmadı → yeni dönemde tam hak
    [InlineData(3, 1, 2, true, 1)]  // devreden + yeni haktan 1 → kalan 1 devreder
    public void Devretme_TeslimOnceDevredeneSayilir(decimal hak, decimal devreden, decimal teslim, bool yeniKullanildi, decimal devredecek)
    {
        Assert.Equal(yeniKullanildi, DagitimDetay.YeniHakKullanildi(devreden, teslim));
        Assert.Equal(devredecek, DagitimDetay.Devredecek(hak, devreden, teslim));
    }

    [Fact]
    public void HakAta_YeniVeDevredenAyriTutulur_TutarsizAtamaReddedilir()
    {
        var detay = new DagitimDetay();
        detay.HakAta(2, 1);
        Assert.Equal((3m, 1m, 2m), (detay.HakMiktari, detay.DevredenMiktar, detay.YeniHakMiktari));

        detay.TeslimKaydet(1);
        Assert.False(detay.YeniHakKullanildiMi);
        Assert.Equal(0, detay.DevredecekMiktar);
        detay.TeslimKaydet(1);
        Assert.True(detay.YeniHakKullanildiMi);
        Assert.Equal(1, detay.DevredecekMiktar);

        Assert.Throws<IsKuraliException>(() => detay.HakAta(0, 1)); // teslim edilenden (2) az
        Assert.Throws<DogrulamaException>(() => new DagitimDetay().HakAta(0, 0));
        Assert.Throws<IsKuraliException>(() => detay.HakMiktari = 0.5m);
    }

    [Fact]
    public void HareketGeriAlma_HerTurKendiEtkisiniTersineCevirir()
    {
        var parti = Parti(1, 100, 10);

        new HasarHareketi { Miktar = 10 }.PartidenGeriAl(parti);        // hasar geri alınınca stok artar
        Assert.Equal(110, parti.Miktar);
        new IadeHareketi { Miktar = 5 }.PartidenGeriAl(parti);          // iade geri alınınca stok azalır
        Assert.Equal(105, parti.Miktar);
        new SayimFarkiHareketi { Miktar = 3, FazlaMi = true }.PartidenGeriAl(parti);
        Assert.Equal(102, parti.Miktar);
        new SayimFarkiHareketi { Miktar = 2, FazlaMi = false }.PartidenGeriAl(parti);
        Assert.Equal(104, parti.Miktar);

        var hata = Assert.Throws<IsKuraliException>(() => new DagitimHareketi { Miktar = 1 }.PartidenGeriAl(parti));
        Assert.Contains("iade", hata.Message);
    }
}
