using Microsoft.Extensions.Configuration;
using Serilog;

namespace AfetCadirKent.UI;

internal static class Program
{
    private const string AyarDosyasiAdi = "appsettings.json";

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "afetcadirkent-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            var ayarDosyasi = Path.Combine(AppContext.BaseDirectory, AyarDosyasiAdi);
            if (!File.Exists(ayarDosyasi))
            {
                MessageBox.Show(
                    "Ayar dosyası (appsettings.json) bulunamadı.\n\n" +
                    "Kurulum: src/AfetCadirKent.UI klasöründeki appsettings.example.json dosyasını " +
                    "appsettings.json adıyla kopyalayın, bağlantı ve Ollama ayarlarını düzenleyin, " +
                    "ardından projeyi yeniden derleyin. Ayrıntılar: docs/SETUP.md",
                    "Afet Çadır Kent – Kurulum gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var yapilandirma = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(AyarDosyasiAdi, optional: false)
                .Build();

            if (string.IsNullOrWhiteSpace(yapilandirma.GetConnectionString("AfetDb")))
            {
                MessageBox.Show("appsettings.json içinde 'ConnectionStrings:AfetDb' tanımlı değil.",
                    "Afet Çadır Kent – Ayar hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Uygulama başlatılamadı");
            MessageBox.Show("Uygulama başlatılamadı. Ayrıntılar log dosyasına yazıldı.",
                "Afet Çadır Kent", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
