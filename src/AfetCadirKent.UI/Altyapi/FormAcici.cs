using Microsoft.Extensions.DependencyInjection;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// Formları bağımlılıklarıyla birlikte oluşturur. Formlar konteynerde kayıtlı değildir; her açılışta yeni nesne üretilir ve
/// çağıran kapatınca kendisi dispose eder (konteyner form nesnelerini biriktirmez).
/// </summary>
public sealed class FormAcici(IServiceProvider servisler)
{
    public T Olustur<T>() where T : Form => ActivatorUtilities.CreateInstance<T>(servisler);

    /// <summary>
    /// Ekran formunu modsuz açar; aynı türden açık bir pencere varsa yenisini açmaz, onu öne getirir.
    /// Pencere kapanınca (WinForms tarafından) serbest bırakılır.
    /// </summary>
    public T EkranAc<T>(Form sahip) where T : Form
    {
        var acik = Application.OpenForms.OfType<T>().FirstOrDefault();
        if (acik is not null)
        {
            if (acik.WindowState == FormWindowState.Minimized)
                acik.WindowState = FormWindowState.Normal;
            acik.Activate();
            return acik;
        }

        var form = Olustur<T>();
        form.Show(sahip);
        return form;
    }

    /// <summary>Formu modal açar ve kapanınca serbest bırakır.</summary>
    public DialogResult DiyalogAc<T>(IWin32Window? sahip) where T : Form
    {
        using var form = Olustur<T>();
        return form.ShowDialog(sahip);
    }
}
