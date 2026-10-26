using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;

namespace AfetCadirKent.UI.Formlar;

/// <summary>Oturumdaki kullanıcının kendi parolasını değiştirmesi (mevcut parola doğrulanır).</summary>
public partial class ParolaDegistirForm : XtraForm
{
    private readonly IOturumServisi _oturumServisi;

    public ParolaDegistirForm(IOturumServisi oturumServisi)
    {
        InitializeComponent();
        _oturumServisi = oturumServisi;
        lblParolaKurali.Text = ParolaGuvenligi.KuralAciklamasi;
    }

    private async void btnKaydet_Click(object? sender, EventArgs e)
    {
        var mevcut = txtMevcutParola.Text;
        var yeni = txtYeniParola.Text;
        var tekrar = txtYeniParolaTekrar.Text;

        var basarili = await Bekleme.CalistirAsync(this, () =>
            Task.Run(() => _oturumServisi.ParolaDegistirAsync(mevcut, yeni, tekrar)));
        if (!basarili)
            return;

        HataYonetimi.Bilgi(this, "Parolanız değiştirildi. Bir sonraki girişte yeni parolanızı kullanın.");
        DialogResult = DialogResult.OK;
        Close();
    }
}
