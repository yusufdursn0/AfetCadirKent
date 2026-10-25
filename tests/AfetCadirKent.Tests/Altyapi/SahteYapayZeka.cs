using System.Net;
using System.Text;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Business.YapayZeka;

namespace AfetCadirKent.Tests.Altyapi;

/// <summary>
/// Servis testleri için sahte yapay zeka. Öneri verilmezse Ollama kapalıymış gibi "ulaşılamadı" döner (K-070).
/// </summary>
public sealed class SahteYapayZeka(TalepSiniflandirmaOnerisi? oneri = null, string? ozet = null) : IYapayZekaServisi
{
    public List<string> GelenTalepler { get; } = [];
    public GunlukDurum? GelenDurum { get; private set; }

    public Task<YzSonucu<TalepSiniflandirmaOnerisi>> TalepSiniflandirAsync(string talepMetni, CancellationToken iptal = default)
    {
        GelenTalepler.Add(talepMetni);
        return Task.FromResult(oneri is null
            ? YzSonucu<TalepSiniflandirmaOnerisi>.Basarisiz(YzHataTuru.Ulasilamadi, "Yapay zeka servisine (Ollama) ulaşılamadı.")
            : YzSonucu<TalepSiniflandirmaOnerisi>.Basari(oneri));
    }

    public Task<YzSonucu<string>> GunlukOzetYazAsync(GunlukDurum durum, CancellationToken iptal = default)
    {
        GelenDurum = durum;
        return Task.FromResult(ozet is null
            ? YzSonucu<string>.Basarisiz(YzHataTuru.Ulasilamadi, "Yapay zeka servisine (Ollama) ulaşılamadı.")
            : YzSonucu<string>.Basari(ozet));
    }
}

/// <summary>Gerçek Ollama olmadan <see cref="OllamaServisi"/>'ni sınamak için sahte HTTP işleyicisi; gelen istekleri saklar.</summary>
public sealed class SahteHttpIsleyici(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> cevapla) : HttpMessageHandler
{
    public List<(Uri Adres, string Govde)> Istekler { get; } = [];

    /// <summary>Ollama /api/chat cevabı: <paramref name="icerik"/> modelin yazdığı metindir.</summary>
    public static SahteHttpIsleyici OllamaCevabi(string icerik) => new((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(new { model = "test", message = new { role = "assistant", content = icerik }, done = true }),
            Encoding.UTF8, "application/json"),
    }));

    public static SahteHttpIsleyici Durum(HttpStatusCode durum, string govde = "{}") =>
        new((_, _) => Task.FromResult(new HttpResponseMessage(durum) { Content = new StringContent(govde, Encoding.UTF8, "application/json") }));

    public static SahteHttpIsleyici Hata(Exception hata) => new((_, _) => Task.FromException<HttpResponseMessage>(hata));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage istek, CancellationToken iptal)
    {
        Istekler.Add((istek.RequestUri!, istek.Content is null ? "" : await istek.Content.ReadAsStringAsync(iptal)));
        return await cevapla(istek, iptal);
    }
}

/// <summary>Sıraya alınan talepleri yalnızca kaydeden sahte sınıflandırma kuyruğu.</summary>
public sealed class SahteKuyruk : ITalepSiniflandirmaKuyrugu
{
    public List<int> Eklenenler { get; } = [];

    public void Ekle(int talepId) => Eklenenler.Add(talepId);
}
