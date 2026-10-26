using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.UI.Formlar;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// Ekranlar arası akış: hiç kullanıcı yoksa ilk kurulum (M1) → giriş → ana ekran. "Oturumu kapat" giriş ekranına döner;
/// ana ekran pencereden kapatılırsa uygulama kapanır. Her iki durumda çıkış işlem geçmişine yazılır.
/// </summary>
public sealed class UygulamaAkisi(IServiceProvider servisler)
{
    private readonly FormAcici _formAcici = servisler.GetRequiredService<FormAcici>();

    public void Calistir()
    {
        var kurulum = servisler.GetRequiredService<IKurulumServisi>();
        if (Bekle(() => kurulum.IlkKurulumGerekliMiAsync()) && _formAcici.DiyalogAc<IlkKurulumForm>(null) != DialogResult.OK)
            return;

        while (true)
        {
            if (_formAcici.DiyalogAc<GirisForm>(null) != DialogResult.OK)
                return;

            bool oturumKapatildi;
            using (var anaForm = _formAcici.Olustur<AnaForm>())
            {
                Application.Run(anaForm);
                oturumKapatildi = anaForm.OturumKapatildi;
            }

            OturumuKapat();
            if (!oturumKapatildi)
                return;
        }
    }

    private void OturumuKapat()
    {
        try
        {
            Bekle(() => servisler.GetRequiredService<IOturumServisi>().CikisYapAsync());
        }
        catch (Exception ex)
        {
            // Çıkış kaydı yazılamadı (ör. veritabanı bağlantısı kesildi); oturum yine de kapanmalı.
            Log.Warning(ex, "Çıkış işlem geçmişine yazılamadı");
            servisler.GetRequiredService<Business.Ortak.Oturum>().Kapat();
        }
    }

    /// <summary>Mesaj döngüsü dışındaki (formlar arası) kısa veritabanı işlemleri için: arka planda çalıştırıp bekler.</summary>
    private static T Bekle<T>(Func<Task<T>> islem) => Task.Run(islem).GetAwaiter().GetResult();

    private static void Bekle(Func<Task> islem) => Task.Run(islem).GetAwaiter().GetResult();
}
