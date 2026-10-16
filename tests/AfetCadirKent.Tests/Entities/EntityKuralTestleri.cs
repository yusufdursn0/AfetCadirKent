using AfetCadirKent.Entities.Cadirlar;
using AfetCadirKent.Entities.Depo;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.Entities.Ortak;

namespace AfetCadirKent.Tests.Entities;

public class TurkceMetinTestleri
{
    [Theory]
    [InlineData("Şükrü ÇAĞLI", "sukru cagli")]
    [InlineData("  IŞIK   Ödül ", "isik odul")]
    [InlineData("İbrahim", "ibrahim")]
    [InlineData("ibrahım", "ibrahim")]
    [InlineData("", "")]
    [InlineData(null, "")]
    public void Normallestir_TurkceKarakterVeBuyukHarfFarkiniYokSayar(string? girdi, string beklenen) =>
        Assert.Equal(beklenen, TurkceMetin.Normallestir(girdi));

    [Fact]
    public void Esit_TurkceKarakterFarkliIsimleriEsitSayar() =>
        Assert.True(TurkceMetin.Esit("Gül Öztürk", "GUL OZTURK"));

    [Fact]
    public void Icerir_BosAramadaFalseDoner() =>
        Assert.False(TurkceMetin.Icerir("Kahramanmaraş", " "));
}

public class TcKimlikTestleri
{
    [Fact]
    public void UretilenNumaraGecerlidir()
    {
        var tc = TcKimlik.KontrolHaneleriyleTamamla("123456789");
        Assert.Equal("12345678950", tc);
        Assert.True(TcKimlik.GecerliMi(tc));
    }

    [Theory]
    [InlineData("12345678951")] // son hane hatalı
    [InlineData("02345678950")] // 0 ile başlıyor
    [InlineData("1234567895")]  // 10 hane
    [InlineData("1234567895a")]
    [InlineData(null)]
    public void GecersizNumaralarReddedilir(string? tc) => Assert.False(TcKimlik.GecerliMi(tc));

    [Fact]
    public void Afetzede_GecersizTcAtanamaz()
    {
        var kisi = new Afetzede();
        var hata = Assert.Throws<DogrulamaException>(() => kisi.TcKimlikNo = "11111111111");
        Assert.Contains("TC", hata.Message);
    }

    [Fact]
    public void Afetzede_BosTcKimliksizKayitIcinNullOlur()
    {
        var kisi = new Afetzede { TcKimlikNo = "   " };
        Assert.Null(kisi.TcKimlikNo);
    }
}

public class KisiTestleri
{
    [Fact]
    public void BosAdDogrulamaHatasiVerir() =>
        Assert.Throws<DogrulamaException>(() => new Afetzede { Ad = "  " });

    [Fact]
    public void AramaAdiNormallestirilmisTutulur()
    {
        var kisi = new Afetzede { Ad = "Şükrü", Soyad = "Çağlı" };
        Assert.Equal("sukru cagli", kisi.AramaAdi);
    }

    [Fact]
    public void KisiTuruAltSinifaGoreDegisir()
    {
        Kisi[] kisiler = [new Afetzede(), new Gorevli(), new Gonullu()];
        Assert.Equal(["Afetzede", "Görevli", "Gönüllü"], kisiler.Select(k => k.KisiTuru));
    }

    [Fact]
    public void YasVeBebekYasliHesabi()
    {
        var bugun = new DateOnly(2026, 10, 16);
        var bebek = new Afetzede { DogumTarihi = new DateOnly(2024, 10, 17) };  // 1 yaş
        var ikiYas = new Afetzede { DogumTarihi = new DateOnly(2024, 10, 16) }; // tam 2 yaş
        var ucYas = new Afetzede { DogumTarihi = new DateOnly(2023, 10, 16) };
        var yasli = new Afetzede { DogumTarihi = new DateOnly(1961, 10, 16) };  // tam 65

        Assert.Equal(1, bebek.Yas(bugun));
        Assert.True(ikiYas.BebekMi(bugun));
        Assert.False(ucYas.BebekMi(bugun));
        Assert.True(yasli.YasliMi(bugun));
    }

    [Fact]
    public void GelecekDogumTarihiReddedilir() =>
        Assert.Throws<DogrulamaException>(() => new Afetzede { DogumTarihi = DateOnly.FromDateTime(DateTime.Today.AddDays(1)) });

    [Theory]
    [InlineData(KisiDurumu.Burada, true)]
    [InlineData(KisiDurumu.Hastanede, true)] // hastanedeki kişinin hakkı ailede kalır
    [InlineData(KisiDurumu.Ayrildi, false)]
    [InlineData(KisiDurumu.Nakil, false)]
    public void HakSahipligiDurumaGore(KisiDurumu durum, bool beklenen) =>
        Assert.Equal(beklenen, new Afetzede { Durum = durum }.HakSahibiMi);
}

public class CadirTestleri
{
    private static Cadir YeniCadir(int kapasite = 4) =>
        new() { Bolge = new Bolge { Kod = "A", Ad = "A" }, Sira = 1, Numara = 2, Kapasite = kapasite };

    [Fact]
    public void KapasiteAsilamaz()
    {
        var cadir = YeniCadir(4);
        cadir.Yerlestir(3);
        var hata = Assert.Throws<IsKuraliException>(() => cadir.Yerlestir(2));
        Assert.Contains("yeterli yer yok", hata.Message);
        Assert.Equal(3, cadir.Doluluk);
        Assert.Equal(1, cadir.BosYatak);
    }

    [Fact]
    public void BakimdakiCadiraYerlesilemez()
    {
        var cadir = YeniCadir();
        cadir.Durum = CadirDurumu.Bakimda;
        Assert.Throws<IsKuraliException>(() => cadir.Yerlestir(1));
    }

    [Fact]
    public void KapasiteDolulugunAltinaDusurulemez()
    {
        var cadir = YeniCadir(6);
        cadir.Yerlestir(5);
        Assert.Throws<IsKuraliException>(() => cadir.Kapasite = 4);
    }

    [Fact]
    public void KodBolgeSiraNumaradanOlusur() => Assert.Equal("A1-02", YeniCadir().Kod);
}

public class PartiVeStokHareketiTestleri
{
    private static readonly DateOnly Bugun = new(2026, 10, 16);

    private static Parti YeniParti(decimal miktar, int? sktGun = 30)
    {
        var parti = new Parti { PartiNo = "P-1", SonKullanmaTarihi = sktGun is { } g ? Bugun.AddDays(g) : null };
        new GirisHareketi { Miktar = miktar }.PartiyeUygula(parti);
        return parti;
    }

    [Fact]
    public void AyirmaKullanilabilirdenDuser_TeslimFizikselStoktanDuser()
    {
        var parti = YeniParti(10);
        parti.Ayir(4, Bugun);
        Assert.Equal(6, parti.KullanilabilirMiktar(Bugun));
        Assert.Equal(4, parti.AyrilanMiktar);

        new DagitimHareketi { Miktar = 3 }.PartiyeUygula(parti);
        Assert.Equal(7, parti.Miktar);
        Assert.Equal(1, parti.AyrilanMiktar);

        parti.AyirmaKaldir(1); // gün sonu / iptal
        Assert.Equal(7, parti.KullanilabilirMiktar(Bugun));
    }

    [Fact]
    public void YetersizStokAyrilamaz()
    {
        var parti = YeniParti(5);
        Assert.Throws<IsKuraliException>(() => parti.Ayir(6, Bugun));
    }

    [Fact]
    public void SktGecmisPartiAyrilamazVeKullanilabilirSifirdir()
    {
        var parti = YeniParti(10, sktGun: -1);
        Assert.True(parti.SktGectiMi(Bugun));
        Assert.Equal(0, parti.KullanilabilirMiktar(Bugun));
        Assert.Throws<IsKuraliException>(() => parti.Ayir(1, Bugun));
    }

    [Fact]
    public void SktYaklasiyorUyarisi()
    {
        Assert.True(YeniParti(1, sktGun: 5).SktYaklasiyorMu(Bugun, 7));
        Assert.False(YeniParti(1, sktGun: 20).SktYaklasiyorMu(Bugun, 7));
        Assert.False(YeniParti(1, sktGun: null).SktYaklasiyorMu(Bugun, 7));
    }

    [Fact]
    public void HasarAyrilmisMiktariDusemez()
    {
        var parti = YeniParti(10);
        parti.Ayir(8, Bugun);
        Assert.Throws<IsKuraliException>(() => new HasarHareketi { Miktar = 3 }.PartiyeUygula(parti));
    }

    [Fact]
    public void StokEtkisiHareketTurunePolimorfikOlarakBaglidir()
    {
        StokHareketi[] hareketler =
        [
            new GirisHareketi { Miktar = 10 },
            new DagitimHareketi { Miktar = 3 },
            new IadeHareketi { Miktar = 1 },
            new HasarHareketi { Miktar = 2 },
            new SayimFarkiHareketi { Miktar = 1, FazlaMi = false },
        ];
        Assert.Equal(5, hareketler.Sum(h => h.StokEtkisi));
    }

    [Fact]
    public void HareketMiktariPozitifOlmali() =>
        Assert.Throws<DogrulamaException>(() => new GirisHareketi { Miktar = 0 });

    [Fact]
    public void KismiTeslimattaKalanBekleyenHakOlur()
    {
        var detay = new DagitimDetay { HakMiktari = 3 };
        detay.TeslimKaydet(1);
        Assert.Equal(2, detay.BekleyenHak);
        Assert.False(detay.TamamlandiMi);
        Assert.Throws<IsKuraliException>(() => detay.TeslimKaydet(3));
    }
}
