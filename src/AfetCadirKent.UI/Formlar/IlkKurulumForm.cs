using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 1 – İlk kurulum: hiç kullanıcı yokken ilk Koordinatör hesabını oluşturur (REQ-030, REQ-200).
/// Kodda ve demo verisinde parola yoktur; hesap burada kullanıcının girdiği parolayla oluşur.
/// </summary>
public partial class IlkKurulumForm : XtraForm
{
    private readonly IKurulumServisi _kurulumServisi;

    public IlkKurulumForm(IKurulumServisi kurulumServisi)
    {
        InitializeComponent();
        _kurulumServisi = kurulumServisi;
        lblParolaKurali.Text = ParolaGuvenligi.KuralAciklamasi;
    }

    private async void btnOlustur_Click(object? sender, EventArgs e)
    {
        var bilgi = new YeniKullaniciBilgisi
        {
            Ad = txtAd.Text,
            Soyad = txtSoyad.Text,
            Telefon = string.IsNullOrWhiteSpace(txtTelefon.Text) ? null : txtTelefon.Text,
            KullaniciAdi = txtKullaniciAdi.Text,
            Parola = txtParola.Text,
            ParolaTekrar = txtParolaTekrar.Text,
        };

        KullaniciOzeti? yonetici = null;
        var basarili = await Bekleme.CalistirAsync(this, async () =>
            yonetici = await Task.Run(() => _kurulumServisi.IlkYoneticiOlusturAsync(bilgi)));
        if (!basarili || yonetici is null)
            return;

        HataYonetimi.Bilgi(this,
            $"Koordinatör hesabı oluşturuldu: {yonetici.KullaniciAdi}\n\nŞimdi bu kullanıcı adı ve parolayla giriş yapabilirsiniz.",
            "İlk kurulum tamamlandı");
        DialogResult = DialogResult.OK;
        Close();
    }
}
