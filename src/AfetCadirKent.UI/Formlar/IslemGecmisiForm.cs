using AfetCadirKent.Business.Denetim;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 18 – İşlem geçmişi (REQ-217, şartname M14): kim, ne zaman, neyi oluşturdu/değiştirdi/iptal etti. Yalnızca okunur;
/// tarih, kayıt türü, işlem türü, kullanıcı ve açıklama metniyle süzülür. Veri <see cref="IIslemGecmisiServisi"/>'nden gelir.
/// </summary>
public partial class IslemGecmisiForm : XtraForm
{
    private const int EnFazlaKayit = 1000;
    private const int VarsayilanGun = 7;

    private sealed record KayitSatiri(DateTime Zaman, string KullaniciAdi, string Islem, string KayitTuru, int? VarlikId, string Aciklama);

    private readonly IIslemGecmisiServisi _servis;

    public IslemGecmisiForm(IIslemGecmisiServisi servis)
    {
        InitializeComponent();
        _servis = servis;

        cmbVarlik.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbVarlik.Properties.Items.Add(new ImageComboBoxItem("(tüm kayıtlar)", (object?)null));
        foreach (var (varlik, ad) in KayitTurleri.Adlar.OrderBy(x => x.Value))
            cmbVarlik.Properties.Items.Add(new ImageComboBoxItem(ad, varlik));
        cmbTur.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbTur.Properties.Items.Add(new ImageComboBoxItem("(tüm işlemler)", (object?)null));
        foreach (var tur in Enum.GetValues<IslemTuru>())
            cmbTur.Properties.Items.Add(new ImageComboBoxItem(tur.Metin(), tur));
        dteBitis.Properties.NullText = "(bugün)";
        dteBaslangic.Properties.NullText = "(en baştan)";
        FiltreyiSifirla();

        GridYardimcisi.BosMetinGoster(viewKayitlar, "Bu filtreye uyan işlem kaydı yok.");
        // Enter tuşu filtre alanlarında listeyi yeniler.
        AcceptButton = btnListele;
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await ListeleAsync();
    }

    private void FiltreyiSifirla()
    {
        dteBaslangic.EditValue = DateTime.Today.AddDays(-(VarsayilanGun - 1));
        dteBitis.EditValue = null;
        cmbVarlik.EditValue = null;
        cmbTur.EditValue = null;
        txtKullanici.Text = "";
        txtMetin.Text = "";
    }

    private async Task ListeleAsync()
    {
        var filtre = new IslemGecmisiFiltresi
        {
            Baslangic = dteBaslangic.EditValue is DateTime b ? DateOnly.FromDateTime(b) : null,
            Bitis = dteBitis.EditValue is DateTime s ? DateOnly.FromDateTime(s) : null,
            VarlikAdi = cmbVarlik.EditValue as string,
            IslemTuru = cmbTur.EditValue as IslemTuru?,
            KullaniciAdi = string.IsNullOrWhiteSpace(txtKullanici.Text) ? null : txtKullanici.Text,
            Metin = string.IsNullOrWhiteSpace(txtMetin.Text) ? null : txtMetin.Text,
            EnFazlaKayit = EnFazlaKayit,
        };
        IReadOnlyList<Entities.Sistem.IslemKaydi> kayitlar = [];
        if (!await Bekleme.CalistirAsync(this, async () => kayitlar = await _servis.ListeleAsync(filtre)))
            return;

        gridKayitlar.DataSource = kayitlar.Select(k => new KayitSatiri(k.Zaman, k.KullaniciAdi, k.IslemTuru.Metin(),
            KayitTurleri.Metin(k.VarlikAdi), k.VarlikId, k.Aciklama)).ToList();
        grpKayitlar.Text = kayitlar.Count >= EnFazlaKayit
            ? $"İşlem geçmişi – ilk {EnFazlaKayit} kayıt gösteriliyor; daha eskisi için filtreyi daraltın"
            : $"İşlem geçmişi – {kayitlar.Count} kayıt";
    }

    private async void btnListele_Click(object? sender, EventArgs e) => await ListeleAsync();

    private async void btnTemizle_Click(object? sender, EventArgs e)
    {
        FiltreyiSifirla();
        await ListeleAsync();
    }
}
