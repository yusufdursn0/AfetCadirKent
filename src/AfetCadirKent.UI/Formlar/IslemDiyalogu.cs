using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>Ortak işlem diyaloğunun ayarları: isteğe bağlı seçim ve metin alanı, açıklama (gerekçe) alanı.</summary>
public sealed record DiyalogAyarlari
{
    public required string Baslik { get; init; }
    public required string Mesaj { get; init; }

    /// <summary>Seçim listesi verilirse seçim zorunludur.</summary>
    public string? SecimEtiketi { get; init; }

    public IReadOnlyList<(object Deger, string Metin)>? Secenekler { get; init; }
    public object? VarsayilanSecim { get; init; }

    /// <summary>Metin alanı etiketi verilirse alan gösterilir ve zorunludur.</summary>
    public string? MetinEtiketi { get; init; }

    public string? VarsayilanMetin { get; init; }
    public string AciklamaEtiketi { get; init; } = "Açıklama";
    public bool AciklamaZorunlu { get; init; } = true;
    public string TamamMetni { get; init; } = "Tamam";
}

public sealed record DiyalogSonucu(object? Secim, string? Metin, string? Aciklama);

/// <summary>
/// Durum değiştirme, pasif yapma, gerekçe ekleme/kaldırma ve aileden ayırma gibi açıklama isteyen işlemlerin ortak diyaloğu.
/// Zorunlu alanlar boşsa kapanmaz; iş kuralları yine Business katmanında doğrulanır.
/// </summary>
public partial class IslemDiyalogu : XtraForm
{
    private readonly DiyalogAyarlari _ayarlar;

    public IslemDiyalogu() : this(new DiyalogAyarlari { Baslik = "İşlem", Mesaj = "" })
    {
    }

    public IslemDiyalogu(DiyalogAyarlari ayarlar)
    {
        InitializeComponent();
        _ayarlar = ayarlar;
        Text = ayarlar.Baslik;
        lblMesaj.Text = ayarlar.Mesaj;
        btnTamam.Text = ayarlar.TamamMetni;

        if (ayarlar.Secenekler is { } secenekler)
        {
            lciSecim.Text = ayarlar.SecimEtiketi ?? "Seçim";
            cmbSecim.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            foreach (var (deger, metin) in secenekler)
                cmbSecim.Properties.Items.Add(new ImageComboBoxItem(metin, deger));
            cmbSecim.EditValue = ayarlar.VarsayilanSecim;
        }
        else
        {
            lciSecim.Visibility = LayoutVisibility.Never;
        }

        if (ayarlar.MetinEtiketi is { } etiket)
        {
            lciMetin.Text = etiket;
            txtMetin.Text = ayarlar.VarsayilanMetin;
        }
        else
        {
            lciMetin.Visibility = LayoutVisibility.Never;
        }

        lciAciklama.Text = ayarlar.AciklamaEtiketi + (ayarlar.AciklamaZorunlu ? " (zorunlu)" : " (isteğe bağlı)");
    }

    public DiyalogSonucu? Sonuc { get; private set; }

    /// <summary>Diyaloğu açar; kullanıcı vazgeçerse null döner.</summary>
    public static DiyalogSonucu? Goster(IWin32Window sahip, DiyalogAyarlari ayarlar)
    {
        using var diyalog = new IslemDiyalogu(ayarlar);
        return diyalog.ShowDialog(sahip) == DialogResult.OK ? diyalog.Sonuc : null;
    }

    private void btnTamam_Click(object? sender, EventArgs e)
    {
        if (_ayarlar.Secenekler is not null && cmbSecim.EditValue is null)
        {
            HataYonetimi.Uyari(this, $"{lciSecim.Text} seçin.", "Eksik bilgi");
            cmbSecim.Focus();
            return;
        }

        if (_ayarlar.MetinEtiketi is not null && string.IsNullOrWhiteSpace(txtMetin.Text))
        {
            HataYonetimi.Uyari(this, $"{lciMetin.Text} boş bırakılamaz.", "Eksik bilgi");
            txtMetin.Focus();
            return;
        }

        if (_ayarlar.AciklamaZorunlu && string.IsNullOrWhiteSpace(memoAciklama.Text))
        {
            HataYonetimi.Uyari(this, $"{_ayarlar.AciklamaEtiketi} yazılmadan işlem yapılamaz.", "Eksik bilgi");
            memoAciklama.Focus();
            return;
        }

        Sonuc = new DiyalogSonucu(cmbSecim.EditValue, txtMetin.Text?.Trim(),
            string.IsNullOrWhiteSpace(memoAciklama.Text) ? null : memoAciklama.Text.Trim());
        DialogResult = DialogResult.OK;
        Close();
    }
}
