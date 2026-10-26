using Microsoft.Extensions.DependencyInjection;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// Formları bağımlılıklarıyla birlikte oluşturur. Formlar konteynerde kayıtlı değildir; her açılışta yeni nesne üretilir ve
/// çağıran kapatınca kendisi dispose eder (konteyner form nesnelerini biriktirmez).
/// </summary>
public sealed class FormAcici(IServiceProvider servisler)
{
    public T Olustur<T>() where T : Form => ActivatorUtilities.CreateInstance<T>(servisler);

    /// <summary>Formu modal açar ve kapanınca serbest bırakır.</summary>
    public DialogResult DiyalogAc<T>(IWin32Window? sahip) where T : Form
    {
        using var form = Olustur<T>();
        return form.ShowDialog(sahip);
    }
}
