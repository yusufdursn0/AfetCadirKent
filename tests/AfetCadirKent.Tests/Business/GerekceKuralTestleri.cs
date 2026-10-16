using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.DataAccess.Konfigurasyonlar;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Kisiler;

namespace AfetCadirKent.Tests.Business;

public class GerekceKuralTestleri
{
    private static readonly DateTime Simdi = new(2026, 10, 16, 12, 0, 0);
    private static readonly DateOnly Bugun = DateOnly.FromDateTime(Simdi);
    private readonly GerekceDegerlendirici _degerlendirici = GerekceDegerlendirici.Varsayilan();

    private static Afetzede Uye(int yas, bool hamile = false, bool tekerlekli = false, bool kronik = false,
        bool refakatsiz = false, KisiDurumu durum = KisiDurumu.Burada) => new()
    {
        Ad = "Ad", Soyad = "Soyad", DogumTarihi = Bugun.AddYears(-yas).AddDays(-1),
        Hamile = hamile, TekerlekliSandalye = tekerlekli, KronikHasta = kronik, RefakatsizCocuk = refakatsiz, Durum = durum,
    };

    private static AileBaglami Baglam(IEnumerable<Afetzede> uyeler, bool tekEbeveyn = false,
        int kayitGunOnce = 1, DateTime? sonTeslim = null)
    {
        var aile = new Aile { Id = 7, AileAdi = "Test Ailesi", TekEbeveyn = tekEbeveyn, KayitZamani = Simdi.AddDays(-kayitGunOnce) };
        aile.Uyeler.AddRange(uyeler);
        return new AileBaglami(aile, aile.CadirKenttekiUyeler.ToList(), Simdi, sonTeslim);
    }

    private IReadOnlyList<GerekceKodu> Kodlar(AileBaglami b, Dictionary<GerekceKodu, GerekceDegisiklikTuru>? elle = null) =>
        _degerlendirici.Degerlendir(b, elle).Gerekceler.Select(g => g.Kod).ToList();

    [Fact]
    public void KurallarSartnameTablosuVeVeritabaniTanimlariylaAynidir()
    {
        var kurallar = GerekceDegerlendirici.TumKurallar();
        Assert.Equal(10, kurallar.Count);
        foreach (var kural in kurallar)
        {
            var tanim = ReferansVerisi.Gerekceler.Single(g => g.Kod == kural.Kod);
            Assert.Equal(tanim.Ad, kural.Ad);
            Assert.Equal(tanim.DestekOnceligi, kural.DestekOnceligi);
            Assert.Equal(tanim.YerlesimEtkisi, kural.YerlesimEtkisi);
            Assert.Equal(tanim.TumUrunler, kural.TumUrunler);
            var beklenen = ReferansVerisi.OncelikliKategoriler.GetValueOrDefault(kural.Kod) ?? [];
            Assert.Equal(beklenen.Order(), kural.OncelikliKategoriler.Order());
        }
    }

    [Fact]
    public void OzelDurumuOlmayanAileNormalOncelikAlir()
    {
        var sonuc = _degerlendirici.Degerlendir(Baglam([Uye(35), Uye(33)]));
        Assert.Equal(DestekOnceligi.Normal, sonuc.DestekOnceligi);
        Assert.Equal([GerekceKodu.OzelDurumYok], sonuc.Gerekceler.Select(g => g.Kod));
    }

    [Fact]
    public void BebekVeHamileAileYuksekOncelikVeIkiGerekceAlir()
    {
        var sonuc = _degerlendirici.Degerlendir(Baglam([Uye(34), Uye(30, hamile: true), Uye(1)]));
        Assert.Equal(DestekOnceligi.Yuksek, sonuc.DestekOnceligi);
        Assert.Equal([GerekceKodu.Bebek, GerekceKodu.Hamile], sonuc.Gerekceler.Select(g => g.Kod));
        Assert.Equal("Destek önceliği: Yüksek (0–2 yaş bebek var, hamile birey var)", sonuc.DestekOnceligiMetni);
        Assert.Equal(
            new HashSet<YerlesimEtkisi> { YerlesimEtkisi.SicakCadir, YerlesimEtkisi.SaglikNoktasinaYakin },
            sonuc.YerlesimEtkileri);
        Assert.DoesNotContain("Acil", sonuc.DestekOnceligiMetni, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DestekOnceligiEnYuksekGerekceyeGoreBelirlenir()
    {
        // Yaşlı (Orta) + tekerlekli sandalye (Yüksek) → Yüksek
        var sonuc = _degerlendirici.Degerlendir(Baglam([Uye(70), Uye(68, tekerlekli: true)]));
        Assert.Equal(DestekOnceligi.Yuksek, sonuc.DestekOnceligi);
        // Yalnız orta gerekçeler → Orta
        Assert.Equal(DestekOnceligi.Orta, _degerlendirici.Degerlendir(Baglam([Uye(70)])).DestekOnceligi);
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(3, false)]
    public void BebekSiniriIkiYastir(int yas, bool bebek) =>
        Assert.Equal(bebek, Kodlar(Baglam([Uye(30), Uye(yas)])).Contains(GerekceKodu.Bebek));

    [Theory]
    [InlineData(5, false)]
    [InlineData(6, true)]
    public void BestenKalabalikAile(int kisi, bool kalabalik) =>
        Assert.Equal(kalabalik, Kodlar(Baglam(Enumerable.Range(0, kisi).Select(_ => Uye(20)))).Contains(GerekceKodu.KalabalikAile));

    [Fact]
    public void TumOzelDurumKurallariTespitEdilir()
    {
        var b = Baglam([Uye(40, kronik: true), Uye(15, refakatsiz: true), Uye(66, tekerlekli: true)], tekEbeveyn: true);
        Assert.Equal(
            [GerekceKodu.TekerlekliSandalye, GerekceKodu.KronikHasta, GerekceKodu.RefakatsizCocuk, GerekceKodu.Yasli, GerekceKodu.TekEbeveyn],
            Kodlar(b));
    }

    [Fact]
    public void AyrilanUyeGerekceUretmez_HastanedekiUyeUretir()
    {
        Assert.DoesNotContain(GerekceKodu.Bebek, Kodlar(Baglam([Uye(30), Uye(1, durum: KisiDurumu.Ayrildi)])));
        Assert.Contains(GerekceKodu.Hamile, Kodlar(Baglam([Uye(30, hamile: true, durum: KisiDurumu.Hastanede)])));
    }

    [Theory]
    [InlineData(2, null, false)]   // 3 günden kısa süredir kayıtlı
    [InlineData(5, null, true)]    // 5 gündür kayıtlı, hiç teslim almamış
    [InlineData(5, 2, false)]      // 2 gün önce teslim almış
    [InlineData(5, 4, true)]       // son teslim 4 gün önce
    public void UcGundurYardimAlmadi(int kayitGunOnce, int? sonTeslimGunOnce, bool beklenen)
    {
        var b = Baglam([Uye(30)], kayitGunOnce: kayitGunOnce,
            sonTeslim: sonTeslimGunOnce is { } g ? Simdi.AddDays(-g) : null);
        Assert.Equal(beklenen, Kodlar(b).Contains(GerekceKodu.UcGundurYardimAlmadi));
    }

    [Fact]
    public void ElleKaldirilanOtomatikGerekceSonuctaYerAlmaz()
    {
        var b = Baglam([Uye(30), Uye(1)]);
        var elle = new Dictionary<GerekceKodu, GerekceDegisiklikTuru> { [GerekceKodu.Bebek] = GerekceDegisiklikTuru.ElleKaldirildi };
        var sonuc = _degerlendirici.Degerlendir(b, elle);
        Assert.Equal([GerekceKodu.OzelDurumYok], sonuc.Gerekceler.Select(g => g.Kod));
        Assert.Equal(DestekOnceligi.Normal, sonuc.DestekOnceligi);
    }

    [Fact]
    public void ElleEklenenGerekceOnceligiEtkilerVeIsaretlenir()
    {
        var b = Baglam([Uye(30)]);
        var elle = new Dictionary<GerekceKodu, GerekceDegisiklikTuru> { [GerekceKodu.KronikHasta] = GerekceDegisiklikTuru.ElleEklendi };
        var sonuc = _degerlendirici.Degerlendir(b, elle);
        var gerekce = Assert.Single(sonuc.Gerekceler);
        Assert.Equal(GerekceKodu.KronikHasta, gerekce.Kod);
        Assert.True(gerekce.ElleEklendi);
        Assert.Equal(DestekOnceligi.Yuksek, sonuc.DestekOnceligi);
    }

    [Fact]
    public void MalzemeIhtiyaciMetniYalnizcaOncelikliUrunleriGosterir()
    {
        var sonuc = new AileOncelikSonucu
        {
            AileId = 1,
            DestekOnceligi = DestekOnceligi.Yuksek,
            Gerekceler = [],
            MalzemeIhtiyaclari = [new(1, "Bebek maması", true), new(2, "Su", false), new(3, "Bebek bezi", true)],
        };
        Assert.Equal("Malzeme ihtiyaçları: Bebek maması ✔ · Bebek bezi ✔", sonuc.MalzemeIhtiyaclariMetni);
        Assert.True(sonuc.UrunIcinOncelikliMi(3));
        Assert.False(sonuc.UrunIcinOncelikliMi(2));
    }

    [Fact]
    public void OzelDurumYokKuraliOlmadanDegerlendiriciKurulamaz() =>
        Assert.Throws<ArgumentException>(() => new GerekceDegerlendirici([new BebekKurali()]));

    [Fact]
    public void GosterimMetinlerindeAcilKelimesiYoktur()
    {
        var metinler = Enum.GetValues<DestekOnceligi>().Select(o => o.Metin())
            .Concat(GerekceDegerlendirici.TumKurallar().Select(k => k.Ad));
        Assert.All(metinler, m => Assert.DoesNotContain("acil", m, StringComparison.OrdinalIgnoreCase));
    }
}
