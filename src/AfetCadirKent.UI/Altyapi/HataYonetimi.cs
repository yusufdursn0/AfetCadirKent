using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Ortak;
using DevExpress.XtraEditors;
using Serilog;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// Hataları kullanıcıya sade mesajla gösterir. İş kuralı/doğrulama hataları servisin mesajıyla uyarı olarak gösterilir;
/// beklenmeyen hataların teknik ayrıntısı yalnızca log dosyasına yazılır (CLAUDE.md 4.5 "Error state").
/// </summary>
public static class HataYonetimi
{
    public const string UygulamaAdi = "Afet Çadır Kent";

    public static void Goster(IWin32Window? sahip, Exception hata)
    {
        switch (hata)
        {
            case YetkisizIslemException:
                Uyari(sahip, hata.Message, "Yetki yok");
                break;
            case DogrulamaException:
                Uyari(sahip, hata.Message, "Eksik veya hatalı bilgi");
                break;
            case IsKuraliException:
                Uyari(sahip, hata.Message, UygulamaAdi);
                break;
            default:
                Log.Error(hata, "Beklenmeyen hata");
                XtraMessageBox.Show(sahip,
                    "Beklenmeyen bir hata oluştu ve işlem tamamlanamadı.\n\n" +
                    "Lütfen işlemin sonucunu ilgili listeden kontrol edip tekrar deneyin. Ayrıntılar log dosyasına yazıldı.",
                    UygulamaAdi, MessageBoxButtons.OK, MessageBoxIcon.Error);
                break;
        }
    }

    public static void Uyari(IWin32Window? sahip, string mesaj, string baslik = UygulamaAdi) =>
        XtraMessageBox.Show(sahip, mesaj, baslik, MessageBoxButtons.OK, MessageBoxIcon.Warning);

    public static void Bilgi(IWin32Window? sahip, string mesaj, string baslik = UygulamaAdi) =>
        XtraMessageBox.Show(sahip, mesaj, baslik, MessageBoxButtons.OK, MessageBoxIcon.Information);

    public static bool Onayla(IWin32Window? sahip, string soru, string baslik = UygulamaAdi) =>
        XtraMessageBox.Show(sahip, soru, baslik, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

    /// <summary>Hiçbir yerde yakalanmamış hatalar için son savunma hattı (Program açılışında bağlanır).</summary>
    public static void GenelYakalayicilariBagla()
    {
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (_, e) => Goster(null, e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            Log.Fatal(e.ExceptionObject as Exception, "Yakalanmamış hata (uygulama kapanıyor)");
        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            Log.Error(e.Exception, "Gözlenmemiş görev hatası");
            e.SetObserved();
        };
    }
}
