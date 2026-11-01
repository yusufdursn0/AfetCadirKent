using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Depo;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 8 – Ürün tanımları (REQ-207): ürün, kategori, birim, hesaplama türü (kişi/aile/bebek başı), hak miktarı, tekrar alma süresi
/// ve kritik seviye. Kullanılmış ürün silinmez, pasif yapılır; kurallar <see cref="IUrunServisi"/>'ndedir.
/// </summary>
public partial class UrunTanimlariForm : XtraForm
{
    private sealed record UrunSatiri(int Id, string Ad, string Kategori, string Birim, string Hesaplama, decimal HakMiktari,
        int TekrarAlmaGunu, decimal KritikSeviye, string KayitDurumu, Urun Urun);

    private readonly IUrunServisi _urunServisi;
    private bool _yeni = true;

    public UrunTanimlariForm(IUrunServisi urunServisi, IOturum oturum)
    {
        InitializeComponent();
        _urunServisi = urunServisi;
        var degistirebilir = oturum.IzinVarMi(Modul.Depo, Izin.Degistirme);

        foreach (var cmb in new[] { cmbKategori, cmbBirim, cmbHesaplama })
            cmb.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        foreach (var kategori in Enum.GetValues<UrunKategorisi>())
            cmbKategori.Properties.Items.Add(new ImageComboBoxItem(kategori.Metin(), kategori));
        foreach (var birim in Enum.GetValues<Birim>())
            cmbBirim.Properties.Items.Add(new ImageComboBoxItem(birim.Metin(), birim));
        foreach (var tur in Enum.GetValues<HesaplamaTuru>())
            cmbHesaplama.Properties.Items.Add(new ImageComboBoxItem(tur.Metin(), tur));

        GridYardimcisi.BosMetinGoster(viewUrunler, "Henüz ürün tanımlanmamış. Aşağıdan yeni ürün ekleyin.");
        if (!degistirebilir)
        {
            grpDuzen.Visibility = LayoutVisibility.Never;
            lblBilgi.Text += " (Salt okunur: ürün değiştirme yetkiniz yok.)";
        }

        YeniDurumu();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await YenileAsync();
    }

    private UrunSatiri? Secili => viewUrunler.GetFocusedRow() as UrunSatiri;

    private async Task YenileAsync(int? odaklanacakId = null)
    {
        IReadOnlyList<Urun> urunler = [];
        if (!await Bekleme.CalistirAsync(this, async () => urunler = await _urunServisi.UrunleriListeleAsync(chkPasifler.Checked)))
            return;

        var secili = odaklanacakId ?? Secili?.Id;
        gridUrunler.DataSource = urunler.Select(u => new UrunSatiri(u.Id, u.Ad, u.Kategori.Metin(), u.Birim.Metin(), u.HesaplamaTuru.Metin(),
            u.HakMiktari, u.TekrarAlmaGunu, u.KritikSeviye, u.Aktif ? "Aktif" : "Pasif", u)).ToList();
        grpUrunler.Text = $"Ürünler – {urunler.Count}";

        var satir = secili is { } id ? viewUrunler.LocateByValue(nameof(UrunSatiri.Id), id) : -1;
        if (satir >= 0)
            viewUrunler.FocusedRowHandle = satir;
        SeciliGoster();
    }

    private void viewUrunler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => SeciliGoster();

    private void SeciliGoster()
    {
        if (Secili?.Urun is not { } u)
        {
            YeniDurumu();
            return;
        }

        _yeni = false;
        txtAd.Text = u.Ad;
        cmbKategori.EditValue = u.Kategori;
        cmbBirim.EditValue = u.Birim;
        cmbHesaplama.EditValue = u.HesaplamaTuru;
        spnHak.Value = u.HakMiktari;
        spnTekrar.Value = u.TekrarAlmaGunu;
        spnKritik.Value = u.KritikSeviye;
        grpDuzen.Text = $"{u.Ad} düzenleniyor";
        btnSil.Enabled = btnAktiflik.Enabled = true;
        btnAktiflik.Text = u.Aktif ? "Pasif yap" : "Aktif yap";
        AyarDegisti(null, EventArgs.Empty);
    }

    private void YeniDurumu()
    {
        _yeni = true;
        viewUrunler.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        txtAd.Text = "";
        cmbKategori.EditValue = UrunKategorisi.Diger;
        cmbBirim.EditValue = Birim.Adet;
        cmbHesaplama.EditValue = HesaplamaTuru.KisiBasi;
        spnHak.Value = 1;
        spnTekrar.Value = 1;
        spnKritik.Value = 0;
        grpDuzen.Text = "Yeni ürün";
        btnSil.Enabled = btnAktiflik.Enabled = false;
        btnAktiflik.Text = "Pasif yap";
        AyarDegisti(null, EventArgs.Empty);
    }

    /// <summary>Ayarın düz cümleyle özeti (ör. "Kişi başı 3 lt, 1 günde bir").</summary>
    private void AyarDegisti(object? sender, EventArgs e)
    {
        if (cmbHesaplama.EditValue is not HesaplamaTuru tur || cmbBirim.EditValue is not Birim birim)
        {
            lblHakAciklama.Text = "";
            return;
        }

        var tekrar = (int)spnTekrar.Value;
        lblHakAciklama.Text = $"Her aileye: <b>{tur.Metin()} {spnHak.Value:0.##} {birim.Metin()}</b>, " +
                              (tekrar == 0 ? "beklemeden tekrar alabilir" : tekrar == 1 ? "her gün" : $"{tekrar} günde bir");
    }

    private async void chkPasifler_CheckedChanged(object? sender, EventArgs e) => await YenileAsync();

    private async void btnYenile_Click(object? sender, EventArgs e) => await YenileAsync();

    private void btnYeni_Click(object? sender, EventArgs e)
    {
        YeniDurumu();
        txtAd.Focus();
    }

    private async void btnKaydet_Click(object? sender, EventArgs e)
    {
        if (cmbKategori.EditValue is not UrunKategorisi kategori || cmbBirim.EditValue is not Birim birim
            || cmbHesaplama.EditValue is not HesaplamaTuru tur)
        {
            HataYonetimi.Uyari(this, "Kategori, birim ve hesaplama türünü seçin.", "Eksik bilgi");
            return;
        }

        var bilgi = new UrunBilgisi
        {
            Ad = txtAd.Text, Kategori = kategori, Birim = birim, HesaplamaTuru = tur, HakMiktari = spnHak.Value,
            TekrarAlmaGunu = (int)spnTekrar.Value, KritikSeviye = spnKritik.Value,
        };
        var id = _yeni ? null : Secili?.Id;
        Urun? kayit = null;
        if (await Bekleme.CalistirAsync(this, async () => kayit = await _urunServisi.UrunKaydetAsync(id, bilgi)))
        {
            HataYonetimi.Bilgi(this, id is null ? $"'{kayit!.Ad}' eklendi." : $"'{kayit!.Ad}' güncellendi.");
            await YenileAsync(kayit.Id);
        }
    }

    private async void btnSil_Click(object? sender, EventArgs e)
    {
        if (Secili is not { } u || !HataYonetimi.Onayla(this, $"'{u.Ad}' silinsin mi?", "Ürün sil"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _urunServisi.UrunSilAsync(u.Id)))
            await YenileAsync();
    }

    private async void btnAktiflik_Click(object? sender, EventArgs e)
    {
        if (Secili is not { } u)
            return;
        var aktif = !u.Urun.Aktif;
        if (!aktif && !HataYonetimi.Onayla(this, $"'{u.Ad}' pasif yapılırsa stok girişi ve yeni dağıtım listesi yapılamaz; geçmişi korunur. " +
                                                  "Devam edilsin mi?", "Ürünü pasif yap"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _urunServisi.UrunAktiflikDegistirAsync(u.Id, aktif)))
        {
            if (!aktif && !chkPasifler.Checked)
                HataYonetimi.Bilgi(this, $"'{u.Ad}' pasif yapıldı. Pasif ürünleri görmek için “Pasif ürünleri de göster”i işaretleyin.");
            await YenileAsync(u.Id);
        }
    }
}
