using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AfetCadirKent.Business.YapayZeka;

/// <summary>Ollama bağlantı ayarları (appsettings.json → Ollama bölümü). Adres yerel makinedir; internet gerekmez.</summary>
public sealed record OllamaAyarlari
{
    public string Adres { get; init; } = "http://localhost:11434";
    public string Model { get; init; } = "llama3.1:8b";
    public int ZamanAsimiSaniye { get; init; } = 180;

    /// <exception cref="ArgumentException">Ayar dosyasındaki değer kullanılamazsa.</exception>
    public Uri ChatAdresi()
    {
        if (!Uri.TryCreate(Adres, UriKind.Absolute, out var taban) || (taban.Scheme != Uri.UriSchemeHttp && taban.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException($"Ollama adresi geçersiz: '{Adres}'. Örnek: http://localhost:11434");
        if (string.IsNullOrWhiteSpace(Model))
            throw new ArgumentException("Ollama model adı ayar dosyasında boş.");
        if (ZamanAsimiSaniye <= 0)
            throw new ArgumentException("Ollama zaman aşımı (ZamanAsimiSaniye) sıfırdan büyük olmalıdır.");
        return new Uri(taban, "/api/chat");
    }
}

/// <summary>
/// Yerel Ollama sunucusuyla çalışan yapay zeka servisi (REQ-131). <c>/api/chat</c> uç noktasına <see cref="HttpClient"/> ile
/// istek atar; sınıflandırmada JSON çıktı modu (<c>"format": "json"</c>) kullanılır. Bağlantı hatası, zaman aşımı,
/// bulunamayan model veya okunamayan cevap istisna değil başarısız sonuç olarak döner (REQ-134); teknik ayrıntı loga yazılır.
/// Verilen <see cref="HttpClient"/>'ın kendi zaman aşımı (varsayılan 100 sn) ayardaki süreden kısaysa kaldırılır; süreyi
/// yalnızca <see cref="OllamaAyarlari.ZamanAsimiSaniye"/> belirler (K-078).
/// </summary>
public class OllamaServisi : IYapayZekaServisi
{
    private readonly HttpClient _http;
    private readonly OllamaAyarlari _ayarlar;
    private readonly Uri _chatAdresi;
    private readonly ILogger _log;

    /// <exception cref="ArgumentException">Ayar geçersizse veya <paramref name="http"/> daha önce istek göndermiş ve zaman aşımı ayardan kısaysa.</exception>
    public OllamaServisi(HttpClient http, OllamaAyarlari ayarlar, ILogger<OllamaServisi>? log = null)
    {
        ArgumentNullException.ThrowIfNull(http);
        ArgumentNullException.ThrowIfNull(ayarlar);
        _http = http;
        _ayarlar = ayarlar;
        _chatAdresi = ayarlar.ChatAdresi();
        _log = log ?? NullLogger<OllamaServisi>.Instance;

        if (http.Timeout != Timeout.InfiniteTimeSpan && http.Timeout < TimeSpan.FromSeconds(ayarlar.ZamanAsimiSaniye))
        {
            try
            {
                http.Timeout = Timeout.InfiniteTimeSpan;
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException("Ollama için kullanılmamış bir HttpClient verilmelidir (zaman aşımı ayarlanamadı).", nameof(http), ex);
            }
        }
    }

    public Task<YzSonucu<TalepSiniflandirmaOnerisi>> TalepSiniflandirAsync(string talepMetni, CancellationToken iptal = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(talepMetni);
        return SorAsync(YzIstemleri.SiniflandirmaSistemi, YzIstemleri.SiniflandirmaKullanici(talepMetni), jsonModu: true, sicaklik: 0,
            YzCevapAyristirici.Siniflandirma, iptal);
    }

    public Task<YzSonucu<string>> GunlukOzetYazAsync(GunlukDurum durum, CancellationToken iptal = default)
    {
        ArgumentNullException.ThrowIfNull(durum);
        return SorAsync(YzIstemleri.OzetSistemi, YzIstemleri.OzetKullanici(durum), jsonModu: false, sicaklik: 0.3,
            YzCevapAyristirici.Ozet, iptal);
    }

    private async Task<YzSonucu<T>> SorAsync<T>(string sistem, string kullanici, bool jsonModu, double sicaklik,
        Func<string?, T?> ayristir, CancellationToken iptal) where T : class
    {
        var istek = new OllamaIstegi(_ayarlar.Model, [new("system", sistem), new("user", kullanici)], Stream: false,
            Format: jsonModu ? "json" : null, new OllamaSecenekleri(sicaklik));

        using var sure = CancellationTokenSource.CreateLinkedTokenSource(iptal);
        sure.CancelAfter(TimeSpan.FromSeconds(_ayarlar.ZamanAsimiSaniye));
        try
        {
            using var cevap = await _http.PostAsJsonAsync(_chatAdresi, istek, sure.Token);
            if (cevap.StatusCode == HttpStatusCode.NotFound)
            {
                _log.LogWarning("Ollama modeli bulunamadı: {Model} ({Adres})", _ayarlar.Model, _chatAdresi);
                return YzSonucu<T>.Basarisiz(YzHataTuru.ModelBulunamadi,
                    $"Yapay zeka modeli '{_ayarlar.Model}' bulunamadı. Modeli indirmek için: ollama pull {_ayarlar.Model}");
            }

            if (!cevap.IsSuccessStatusCode)
            {
                _log.LogWarning("Ollama hata döndürdü: {Durum} {Govde}", (int)cevap.StatusCode,
                    (await cevap.Content.ReadAsStringAsync(sure.Token)).Kisalt(500));
                return YzSonucu<T>.Basarisiz(YzHataTuru.SunucuHatasi,
                    $"Yapay zeka servisi hata döndürdü ({(int)cevap.StatusCode}). {ElleDevam}");
            }

            var govde = await cevap.Content.ReadFromJsonAsync<OllamaCevabi>(sure.Token);
            var icerik = govde?.Message?.Content;
            if (ayristir(icerik) is { } deger)
                return YzSonucu<T>.Basari(deger);

            _log.LogWarning("Ollama cevabı beklenen biçimde değil ({Model}): {Icerik}", _ayarlar.Model, (icerik ?? "(boş)").Kisalt(500));
            return Okunamadi<T>();
        }
        catch (OperationCanceledException ex) when (!iptal.IsCancellationRequested)
        {
            _log.LogWarning(ex, "Ollama {Sure} sn içinde cevap vermedi ({Adres})", _ayarlar.ZamanAsimiSaniye, _chatAdresi);
            return YzSonucu<T>.Basarisiz(YzHataTuru.ZamanAsimi,
                $"Yapay zeka {_ayarlar.ZamanAsimiSaniye} saniye içinde cevap vermedi. {ElleDevam}");
        }
        catch (HttpRequestException ex)
        {
            _log.LogWarning(ex, "Ollama'ya bağlanılamadı ({Adres})", _chatAdresi);
            return YzSonucu<T>.Basarisiz(YzHataTuru.Ulasilamadi,
                $"Yapay zeka servisine (Ollama) ulaşılamadı; Ollama'nın çalıştığını kontrol edin. {ElleDevam}");
        }
        catch (Exception ex) when (ex is JsonException or NotSupportedException)
        {
            _log.LogWarning(ex, "Ollama cevap gövdesi okunamadı ({Adres})", _chatAdresi);
            return Okunamadi<T>();
        }
    }

    private const string ElleDevam = "İşleme elle devam edebilirsiniz.";

    private static YzSonucu<T> Okunamadi<T>() where T : class =>
        YzSonucu<T>.Basarisiz(YzHataTuru.CevapOkunamadi, $"Yapay zeka cevabı okunamadı. {ElleDevam}");

    // Ollama /api/chat sözleşmesi (https://github.com/ollama/ollama/blob/main/docs/api.md)
    private sealed record OllamaIstegi(
        string Model,
        IReadOnlyList<OllamaMesaji> Messages,
        bool Stream,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Format,
        OllamaSecenekleri Options);

    private sealed record OllamaMesaji(string Role, string Content);

    private sealed record OllamaSecenekleri(double Temperature);

    private sealed record OllamaCevabi(OllamaMesaji? Message);
}
