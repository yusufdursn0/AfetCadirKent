using DevExpress.XtraSplashScreen;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// Uzun işlemlerde bekleme göstergesi ve hata yönetimi (CLAUDE.md 4.5 "Loading/Error state"). İşlem sürerken kontrolün
/// üzerine yarı saydam bekleme katmanı çizilir; kullanıcı aynı butona ikinci kez basamaz, arayüz donmaz.
/// </summary>
public static class Bekleme
{
    /// <returns>İşlem hatasız bittiyse true; hata kullanıcıya gösterildiyse false.</returns>
    public static async Task<bool> CalistirAsync(Control kaplanacak, Func<Task> islem)
    {
        IOverlaySplashScreenHandle? katman = null;
        try
        {
            if (kaplanacak.IsHandleCreated && kaplanacak.Visible)
                katman = SplashScreenManager.ShowOverlayForm(kaplanacak);
            await islem();
            return true;
        }
        catch (Exception ex)
        {
            if (katman is not null)
            {
                SplashScreenManager.CloseOverlayForm(katman);
                katman = null;
            }

            HataYonetimi.Goster(kaplanacak.FindForm(), ex);
            return false;
        }
        finally
        {
            if (katman is not null)
                SplashScreenManager.CloseOverlayForm(katman);
        }
    }
}
