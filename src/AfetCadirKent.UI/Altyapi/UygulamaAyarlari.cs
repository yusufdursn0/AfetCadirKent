using System.IO;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.YapayZeka;
using Microsoft.Extensions.Configuration;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// appsettings.json'dan okunan ayarlar. Dosya repoda yoktur (gizli bilgiler, Föy 2.1); repoda appsettings.example.json bulunur.
/// </summary>
public sealed record UygulamaAyarlari(string BaglantiDizesi, OllamaAyarlari Ollama, DepoAyarlari Depo)
{
    public const string DosyaAdi = "appsettings.json";

    /// <summary>Ayarları okur; dosya yoksa veya zorunlu bir değer eksikse kullanıcıya gösterilecek mesajı döndürür.</summary>
    public static (UygulamaAyarlari? Ayarlar, string? Hata) Yukle()
    {
        var dosya = Path.Combine(AppContext.BaseDirectory, DosyaAdi);
        if (!File.Exists(dosya))
            return (null,
                "Ayar dosyası (appsettings.json) bulunamadı.\n\n" +
                "Kurulum: src/AfetCadirKent.UI klasöründeki appsettings.example.json dosyasını appsettings.json adıyla " +
                "kopyalayın, bağlantı ve Ollama ayarlarını düzenleyin, ardından projeyi yeniden derleyin. Ayrıntılar: docs/SETUP.md");

        IConfigurationRoot yapilandirma;
        try
        {
            yapilandirma = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(DosyaAdi, optional: false)
                .Build();
        }
        catch (Exception ex) when (ex is FormatException or InvalidDataException or IOException)
        {
            return (null, "appsettings.json okunamadı. Dosyanın geçerli JSON biçiminde olduğunu kontrol edin.\n\nAyrıntı: " + ex.Message);
        }

        var baglanti = yapilandirma.GetConnectionString("AfetDb");
        if (string.IsNullOrWhiteSpace(baglanti))
            return (null, "appsettings.json içinde 'ConnectionStrings:AfetDb' tanımlı değil. Örnek için appsettings.example.json dosyasına bakın.");

        try
        {
            var ollama = yapilandirma.GetSection("Ollama").Get<OllamaAyarlari>() ?? new OllamaAyarlari();
            ollama.ChatAdresi(); // adres, model ve zaman aşımı kullanılabilir mi
            var depo = new DepoAyarlari
            {
                SktUyariGunu = yapilandirma.GetValue("Uygulama:SktUyariGunu", new DepoAyarlari().SktUyariGunu),
            };
            if (depo.SktUyariGunu < 0)
                return (null, "appsettings.json içindeki 'Uygulama:SktUyariGunu' sıfır veya daha büyük olmalıdır.");
            return (new UygulamaAyarlari(baglanti, ollama, depo), null);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            return (null, "appsettings.json içindeki bir ayar geçersiz.\n\nAyrıntı: " + ex.Message);
        }
    }
}
