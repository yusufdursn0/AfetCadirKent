using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;

namespace AfetCadirKent.UI.Formlar;

/// <summary>Ekran 2 – Giriş: kullanıcı adı ve parolayla oturum açar (REQ-201, REQ-033).</summary>
public partial class GirisForm : XtraForm
{
    private readonly IOturumServisi _oturumServisi;

    public GirisForm(IOturumServisi oturumServisi)
    {
        InitializeComponent();
        _oturumServisi = oturumServisi;
    }

    /// <summary>Başarılı girişte oturum açan kullanıcı.</summary>
    public KullaniciOzeti? Kullanici { get; private set; }

    private async void btnGiris_Click(object? sender, EventArgs e)
    {
        var kullaniciAdi = txtKullaniciAdi.Text;
        var parola = txtParola.Text;

        // Parola doğrulaması (BCrypt) işlemciyi meşgul eder; arayüz donmasın diye arka planda çalışır.
        var basarili = await Bekleme.CalistirAsync(this, async () =>
            Kullanici = await Task.Run(() => _oturumServisi.GirisYapAsync(kullaniciAdi, parola)));

        if (basarili)
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        txtParola.EditValue = null;
        (string.IsNullOrWhiteSpace(txtKullaniciAdi.Text) ? txtKullaniciAdi : txtParola).Focus();
    }
}
