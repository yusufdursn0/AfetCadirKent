using System.IO;
using AfetCadirKent.Business;
using AfetCadirKent.DataAccess;
using AfetCadirKent.UI.Altyapi;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace AfetCadirKent.UI;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // EF Core SQL komutları loga yazılmaz
            .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "logs", "afetcadirkent-.log"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        HataYonetimi.GenelYakalayicilariBagla();
        TurkceYerellestirme.Uygula();
        WindowsFormsSettings.AllowAutoScale = DevExpress.Utils.DefaultBoolean.True;
        UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXI);

        try
        {
            var (ayarlar, hata) = UygulamaAyarlari.Yukle();
            if (ayarlar is null)
            {
                XtraMessageBox.Show(hata, "Afet Çadır Kent – Kurulum gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var servisler = ServisleriOlustur(ayarlar);
            if (!VeritabaniniHazirla(servisler))
                return;

            // Arka plan talep sınıflandırıcısı uygulama boyunca çalışır (K-080).
            using var kapanis = new CancellationTokenSource();
            var baslatici = servisler.GetRequiredService<IUygulamaBaslatici>();
            var arkaPlan = Task.Run(() => baslatici.ArkaPlanIsleriniCalistirAsync(kapanis.Token));

            new UygulamaAkisi(servisler).Calistir();

            kapanis.Cancel();
            arkaPlan.Wait(TimeSpan.FromSeconds(3));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Uygulama başlatılamadı veya beklenmedik biçimde durdu");
            XtraMessageBox.Show("Uygulama beklenmedik bir hatayla durdu. Ayrıntılar log dosyasına yazıldı.",
                HataYonetimi.UygulamaAdi, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static ServiceProvider ServisleriOlustur(UygulamaAyarlari ayarlar)
    {
        var servisler = new ServiceCollection();
        servisler.AddLogging(l => l.AddSerilog(dispose: false));
        servisler.VeriErisiminiEkle(ayarlar.BaglantiDizesi);
        servisler.IsKatmaniniEkle(ayarlar.Ollama, ayarlar.Depo);
        servisler.AddSingleton<FormAcici>();
        return servisler.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
    }

    /// <summary>Migration'ları uygular, boşsa demo verisini yükler (K-060). Başarısızsa kullanıcıya nedenini söyler.</summary>
    private static bool VeritabaniniHazirla(IServiceProvider servisler)
    {
        SplashScreenManager.ShowDefaultWaitForm("Lütfen bekleyin", "Veritabanı hazırlanıyor…");
        try
        {
            var baslatici = servisler.GetRequiredService<IUygulamaBaslatici>();
            Task.Run(() => baslatici.VeritabaniniHazirlaAsync()).GetAwaiter().GetResult();
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Veritabanı hazırlanamadı");
            SplashScreenManager.CloseDefaultWaitForm();
            XtraMessageBox.Show(
                "Veritabanına bağlanılamadı veya veritabanı hazırlanamadı.\n\n" +
                "SQL Server LocalDB'nin kurulu olduğunu ve appsettings.json içindeki 'ConnectionStrings:AfetDb' değerinin " +
                "doğru olduğunu kontrol edin (docs/SETUP.md). Ayrıntılar log dosyasına yazıldı.",
                "Afet Çadır Kent – Veritabanı", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
        finally
        {
            if (SplashScreenManager.Default is not null)
                SplashScreenManager.CloseDefaultWaitForm();
        }
    }
}
