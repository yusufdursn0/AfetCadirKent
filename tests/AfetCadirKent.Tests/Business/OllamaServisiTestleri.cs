using System.Net;
using System.Text.Json;
using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.Extensions.Logging;

namespace AfetCadirKent.Tests.Business;

/// <summary>Ollama servisi sahte HTTP işleyicisiyle sınanır; gerçek Ollama gerekmez (K-070).</summary>
public class OllamaServisiTestleri
{
    private static readonly OllamaAyarlari Ayarlar = new() { Adres = "http://localhost:11434", Model = "qwen2.5:7b", ZamanAsimiSaniye = 5 };

    private static OllamaServisi Servis(SahteHttpIsleyici isleyici, OllamaAyarlari? ayarlar = null) =>
        new(new HttpClient(isleyici), ayarlar ?? Ayarlar);

    private static GunlukDurum OrnekDurum() => new()
    {
        Tarih = new DateOnly(2026, 10, 20), BuradakiKisi = 80, HastanedekiKisi = 2, AileSayisi = 20,
        ToplamKapasite = 150, BosYatak = 70, KullanilamayanCadir = 1, BekleyenAile = 3,
        KritikStoklar = ["Su: kullanılabilir stok 120 lt, kritik seviye 500 lt."], SktYaklasanlar = [], SktGecenler = [],
        AcikTalepler = new Dictionary<string, int> { ["Sınıflandırılmamış"] = 2, ["Malzeme"] = 4 },
        BugunAcilanTalep = 5, BugunKapananTalep = 1, BugunkuTeslimSayisi = 12,
    };

    [Fact]
    public async Task Siniflandirma_ChatUcNoktasinaJsonModuylaIstekAtarVeOneriDoner()
    {
        var isleyici = SahteHttpIsleyici.OllamaCevabi("""{"tur":"CadirAriza","oncelik":"Yuksek","gerekce":"Çadır su alıyor."}""");

        var sonuc = await Servis(isleyici).TalepSiniflandirAsync("Çadır su alıyor");

        Assert.True(sonuc.Basarili);
        Assert.Equal(new TalepSiniflandirmaOnerisi(TalepTuru.CadirAriza, DestekOnceligi.Yuksek, "Çadır su alıyor."), sonuc.Deger);

        var (adres, govde) = Assert.Single(isleyici.Istekler);
        Assert.Equal("http://localhost:11434/api/chat", adres.ToString());
        using var istek = JsonDocument.Parse(govde);
        var kok = istek.RootElement;
        Assert.Equal("qwen2.5:7b", kok.GetProperty("model").GetString());
        Assert.Equal("json", kok.GetProperty("format").GetString());
        Assert.False(kok.GetProperty("stream").GetBoolean());
        var mesajlar = kok.GetProperty("messages").EnumerateArray().ToList();
        Assert.Equal(["system", "user"], mesajlar.Select(m => m.GetProperty("role").GetString()));
        Assert.Contains("Çadır su alıyor", mesajlar[1].GetProperty("content").GetString());
    }

    [Fact]
    public async Task SaglikTalebi_ModelOncelikDondurseBileOncelikBosGelir()
    {
        var isleyici = SahteHttpIsleyici.OllamaCevabi("""{"tur":"Saglik","oncelik":"Yuksek","gerekce":"Ateş"}""");

        var sonuc = await Servis(isleyici).TalepSiniflandirAsync("Çocuk ateşleniyor");

        Assert.Equal((TalepTuru.Saglik, (DestekOnceligi?)null), (sonuc.Deger!.Tur, sonuc.Deger.YardimOnceligi));
    }

    [Fact]
    public async Task OllamaCalismiyor_UlasilamadiDoner()
    {
        var sonuc = await Servis(SahteHttpIsleyici.Hata(new HttpRequestException("Bağlantı reddedildi")))
            .TalepSiniflandirAsync("Çadır su alıyor");

        Assert.False(sonuc.Basarili);
        Assert.Equal(YzHataTuru.Ulasilamadi, sonuc.Hata);
        Assert.Contains("ulaşılamadı", sonuc.HataMesaji);
        Assert.Contains("elle", sonuc.HataMesaji);
    }

    [Fact]
    public async Task CevapGecikirse_ZamanAsimiDoner()
    {
        var isleyici = new SahteHttpIsleyici(async (_, iptal) =>
        {
            await Task.Delay(Timeout.Infinite, iptal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var sonuc = await Servis(isleyici, Ayarlar with { ZamanAsimiSaniye = 1 }).TalepSiniflandirAsync("Çadır su alıyor");

        Assert.Equal(YzHataTuru.ZamanAsimi, sonuc.Hata);
        Assert.Contains("1 saniye", sonuc.HataMesaji);
    }

    [Fact]
    public async Task KullaniciIptalEderse_IstisnaYukselir()
    {
        var isleyici = new SahteHttpIsleyici(async (_, iptal) =>
        {
            await Task.Delay(Timeout.Infinite, iptal);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        using var iptal = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Servis(isleyici).TalepSiniflandirAsync("x", iptal.Token));
    }

    [Fact]
    public async Task ModelIndirilmemis_ModelBulunamadiVeIndirmeKomutuDoner()
    {
        var sonuc = await Servis(SahteHttpIsleyici.Durum(HttpStatusCode.NotFound, """{"error":"model 'qwen2.5:7b' not found"}"""))
            .TalepSiniflandirAsync("x");

        Assert.Equal(YzHataTuru.ModelBulunamadi, sonuc.Hata);
        Assert.Contains("ollama pull qwen2.5:7b", sonuc.HataMesaji);
    }

    [Fact]
    public async Task SunucuHatasi_SunucuHatasiDoner()
    {
        var sonuc = await Servis(SahteHttpIsleyici.Durum(HttpStatusCode.InternalServerError)).TalepSiniflandirAsync("x");
        Assert.Equal(YzHataTuru.SunucuHatasi, sonuc.Hata);
    }

    [Theory]
    [InlineData("Bu talep çadırla ilgili.")]
    [InlineData("""{"tur":"Uzay"}""")]
    [InlineData("")]
    public async Task ModelCevabiOkunamazsa_CevapOkunamadiDoner(string icerik)
    {
        var sonuc = await Servis(SahteHttpIsleyici.OllamaCevabi(icerik)).TalepSiniflandirAsync("x");
        Assert.Equal(YzHataTuru.CevapOkunamadi, sonuc.Hata);
    }

    [Fact]
    public async Task OllamaGovdesiJsonDegilse_CevapOkunamadiDoner()
    {
        var isleyici = new SahteHttpIsleyici((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("<html>proxy hatası</html>", System.Text.Encoding.UTF8, "application/json"),
        }));

        var sonuc = await Servis(isleyici).TalepSiniflandirAsync("x");
        Assert.Equal(YzHataTuru.CevapOkunamadi, sonuc.Hata);
    }

    [Fact]
    public async Task GunlukOzet_DuzMetinModundaSayilariGonderirVeMetniDoner()
    {
        var isleyici = SahteHttpIsleyici.OllamaCevabi("Çadır kentte 80 kişi bulunuyor. Su stoğu kritik seviyenin altında; acil takviye gerekli.");

        var sonuc = await Servis(isleyici).GunlukOzetYazAsync(OrnekDurum());

        Assert.Equal("Çadır kentte 80 kişi bulunuyor. Su stoğu kritik seviyenin altında; öncelikli takviye gerekli.", sonuc.Deger);
        using var istek = JsonDocument.Parse(Assert.Single(isleyici.Istekler).Govde);
        Assert.False(istek.RootElement.TryGetProperty("format", out _));
        var kullanici = istek.RootElement.GetProperty("messages")[1].GetProperty("content").GetString()!;
        Assert.Contains("Çadır kentte bulunan kişi: 80 (ayrıca hastanede: 2)", kullanici);
        Assert.Contains("- Su: kullanılabilir stok 120 lt, kritik seviye 500 lt.", kullanici);
        Assert.Contains("Açık talepler: 6 (Sınıflandırılmamış: 2, Malzeme: 4)", kullanici);
        Assert.Contains("Son kullanma tarihi yaklaşan partiler: yok", kullanici);
    }

    [Fact]
    public async Task GunlukOzet_OllamaKapaliysaBasarisizDoner()
    {
        var sonuc = await Servis(SahteHttpIsleyici.Hata(new HttpRequestException())).GunlukOzetYazAsync(OrnekDurum());
        Assert.Equal(YzHataTuru.Ulasilamadi, sonuc.Hata);
        Assert.Null(sonuc.Deger);
    }

    [Theory]
    [InlineData("localhost:11434", "qwen2.5:7b", 60)]
    [InlineData("ftp://localhost", "qwen2.5:7b", 60)]
    [InlineData("http://localhost:11434", " ", 60)]
    [InlineData("http://localhost:11434", "qwen2.5:7b", 0)]
    public void GecersizAyar_AnlasilirHataVerir(string adres, string model, int sure)
    {
        var hata = Assert.Throws<ArgumentException>(() =>
            new OllamaServisi(new HttpClient(), new OllamaAyarlari { Adres = adres, Model = model, ZamanAsimiSaniye = sure }));
        Assert.Contains("Ollama", hata.Message);
    }

    [Fact]
    public async Task OllamaServisi_InterfaceUzerindenKullanilir()
    {
        IYapayZekaServisi servis = Servis(SahteHttpIsleyici.OllamaCevabi("""{"tur":"Diger","oncelik":"Normal"}"""));
        Assert.Equal(TalepTuru.Diger, (await servis.TalepSiniflandirAsync("Kayıp belge")).Deger!.Tur);
    }
    // ---------------------------------------------------------------- Zaman aşımı ve loglama (K-078)

    [Fact]
    public void HttpClientinVarsayilanSuresiAyardanKisaysa_Kaldirilir()
    {
        var http = new HttpClient(); // varsayılan 100 sn
        _ = new OllamaServisi(http, Ayarlar with { ZamanAsimiSaniye = 180 });
        Assert.Equal(Timeout.InfiniteTimeSpan, http.Timeout);

        var yeterli = new HttpClient();
        _ = new OllamaServisi(yeterli, Ayarlar with { ZamanAsimiSaniye = 60 });
        Assert.Equal(TimeSpan.FromSeconds(100), yeterli.Timeout);
    }

    [Fact]
    public async Task KullanilmisHttpClientSuresiAyarlanamazsa_AnlasilirHataVerir()
    {
        var http = new HttpClient(SahteHttpIsleyici.Durum(HttpStatusCode.OK)) { Timeout = TimeSpan.FromSeconds(10) };
        await http.GetAsync("http://localhost:11434/");

        var hata = Assert.Throws<ArgumentException>(() => new OllamaServisi(http, Ayarlar with { ZamanAsimiSaniye = 180 }));
        Assert.Contains("HttpClient", hata.Message);
    }

    [Fact]
    public async Task HataninTeknikAyrintisiLogaYazilir_KullaniciMesajindaYerAlmaz()
    {
        var log = new ListeLogger<OllamaServisi>();
        var servis = new OllamaServisi(new HttpClient(SahteHttpIsleyici.Hata(new HttpRequestException("Connection refused (localhost:11434)"))),
            Ayarlar, log);

        var sonuc = await servis.TalepSiniflandirAsync("Çadır su alıyor");

        var kayit = Assert.Single(log.Kayitlar);
        Assert.Equal(LogLevel.Warning, kayit.Seviye);
        Assert.IsType<HttpRequestException>(kayit.Hata);
        Assert.DoesNotContain("Connection refused", sonuc.HataMesaji);
    }

    [Fact]
    public async Task OkunamayanCevap_ModelinYazdigiMetinLogaYazilir()
    {
        var log = new ListeLogger<OllamaServisi>();
        var servis = new OllamaServisi(new HttpClient(SahteHttpIsleyici.OllamaCevabi("Bu talep çadırla ilgili.")), Ayarlar, log);

        await servis.TalepSiniflandirAsync("x");

        Assert.Contains("Bu talep çadırla ilgili.", Assert.Single(log.Kayitlar).Mesaj);
    }
}
