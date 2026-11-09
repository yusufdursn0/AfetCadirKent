using System.IO;
using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.Raporlama;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using AfetCadirKent.UI.Raporlar;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 16 – Raporlar (REQ-215, şartname M13): dağıtım tutanağı, stok, nüfus ve talep raporları; önizleme, PDF/Excel'e aktarma ve
/// yazdırma (XtraReports). Rapor içeriği ve "Rapor" izni denetimi <see cref="IRaporServisi"/>'ndedir; ekran yalnızca izinli raporları sunar.
/// </summary>
public partial class RaporlarForm : XtraForm
{
    private enum RaporTuru
    {
        DagitimTutanagi,
        Stok,
        Nufus,
        Talep,
    }

    private readonly IRaporServisi _raporServisi;
    private readonly IDagitimServisi _dagitimServisi;
    private XtraReport? _belge;

    public RaporlarForm(IRaporServisi raporServisi, IDagitimServisi dagitimServisi, IOturum oturum)
    {
        InitializeComponent();
        _raporServisi = raporServisi;
        _dagitimServisi = dagitimServisi;

        foreach (var (tur, ad, modul) in new[]
                 {
                     (RaporTuru.DagitimTutanagi, "Dağıtım tutanağı", Modul.Dagitim),
                     (RaporTuru.Stok, "Stok raporu", Modul.Depo),
                     (RaporTuru.Nufus, "Nüfus raporu", Modul.Kayit),
                     (RaporTuru.Talep, "Talep raporu", Modul.Talep),
                 })
        {
            if (oturum.IzinVarMi(modul, Izin.Rapor))
                rdgRapor.Properties.Items.Add(new RadioGroupItem(tur, ad));
        }

        cmbDagitim.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        dteBaslangic.EditValue = DateTime.Today.AddDays(-6);
        dteBitis.EditValue = DateTime.Today;
        Disposed += (_, _) => _belge?.Dispose();
        rdgRapor.SelectedIndex = rdgRapor.Properties.Items.Count > 0 ? 0 : -1;
        SecenekleriGoster();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (rdgRapor.Properties.Items.Cast<RadioGroupItem>().Any(i => Equals(i.Value, RaporTuru.DagitimTutanagi)))
            await DagitimListeleriniYukleAsync();
    }

    private RaporTuru? SeciliRapor => rdgRapor.EditValue as RaporTuru?;

    private async Task DagitimListeleriniYukleAsync()
    {
        IReadOnlyList<DagitimOzeti> listeler = [];
        if (!await Bekleme.CalistirAsync(this, async () => listeler = await _dagitimServisi.ListeleriGetirAsync()))
            return;
        cmbDagitim.Properties.Items.Clear();
        foreach (var l in listeler)
            cmbDagitim.Properties.Items.Add(new ImageComboBoxItem($"{l.Tarih:dd.MM.yyyy} · {l.UrunAdi} · {l.Durum.Metin()} (no {l.Id})", (object)l.Id));
        cmbDagitim.EditValue = listeler.Count > 0 ? listeler[0].Id : null;
    }

    private void rdgRapor_SelectedIndexChanged(object? sender, EventArgs e) => SecenekleriGoster();

    private void SecenekleriGoster()
    {
        var tur = SeciliRapor;
        lciDagitim.Visibility = tur == RaporTuru.DagitimTutanagi ? LayoutVisibility.Always : LayoutVisibility.Never;
        lciBaslangic.Visibility = lciBitis.Visibility = tur == RaporTuru.Talep ? LayoutVisibility.Always : LayoutVisibility.Never;
        lblRaporNot.Text = tur switch
        {
            RaporTuru.DagitimTutanagi => "Seçilen listedeki aileler, hak, teslim edilen ve bekleyen miktar, teslim alan ve teslim eden; " +
                                         "altta teslim eden ve onaylayan için imza alanları.",
            RaporTuru.Stok => "Ürün bazında depodaki, dağıtıma ayrılan, kullanılabilir ve SKT'si geçmiş miktar; kritik seviye ve en yakın SKT.",
            RaporTuru.Nufus => "Kişi durumu, yaş grubu, cinsiyet, ailelerin destek önceliği ve bölgelere göre dağılım; yatak doluluğu.",
            RaporTuru.Talep => "Seçilen tarihlerde açılan talepler; türe ve duruma göre sayılar, ortalama tamamlanma süresi.",
            _ => "Bu rolde rapor alma izni yok.",
        };
        btnOlustur.Enabled = tur is not null;
        // Rapor türü değişince eski önizleme kaldırılır; yanlış raporun dışa aktarılması önlenir.
        OnizlemeyiTemizle();
    }

    private void OnizlemeyiTemizle()
    {
        viewer.DocumentSource = null;
        _belge?.Dispose();
        _belge = null;
        grpOnizleme.Text = "Önizleme";
        DisaAktarmaDugmeleri();
    }

    private void DisaAktarmaDugmeleri() => btnPdf.Enabled = btnExcel.Enabled = btnYazdir.Enabled = _belge is not null;

    private async void btnOlustur_Click(object? sender, EventArgs e)
    {
        if (SeciliRapor is not { } tur)
            return;
        if (tur == RaporTuru.DagitimTutanagi && cmbDagitim.EditValue is not int)
        {
            HataYonetimi.Uyari(this, "Tutanağı alınacak dağıtım listesini seçin.", "Eksik bilgi");
            return;
        }

        var dagitimId = cmbDagitim.EditValue as int?;
        var baslangic = dteBaslangic.EditValue is DateTime b ? DateOnly.FromDateTime(b) : DateOnly.FromDateTime(DateTime.Today);
        var bitis = dteBitis.EditValue is DateTime s ? DateOnly.FromDateTime(s) : DateOnly.FromDateTime(DateTime.Today);
        TabloRaporu? rapor = null;
        if (!await Bekleme.CalistirAsync(this, async () => rapor = tur switch
            {
                RaporTuru.DagitimTutanagi => await _raporServisi.DagitimTutanagiAsync(dagitimId!.Value),
                RaporTuru.Stok => await _raporServisi.StokRaporuAsync(),
                RaporTuru.Nufus => await _raporServisi.NufusRaporuAsync(),
                _ => await _raporServisi.TalepRaporuAsync(baslangic, bitis),
            }))
            return;

        OnizlemeyiTemizle();
        _belge = new TabloRaporuBelgesi(rapor!);
        _belge.CreateDocument();
        viewer.DocumentSource = _belge;
        viewer.ExecCommand(PrintingSystemCommand.ZoomToPageWidth);
        grpOnizleme.Text = $"Önizleme – {rapor!.Baslik}";
        DisaAktarmaDugmeleri();
    }

    private void btnPdf_Click(object? sender, EventArgs e) =>
        DisaAktar("PDF dosyası (*.pdf)|*.pdf", "pdf", (belge, yol) => belge.ExportToPdf(yol));

    private void btnExcel_Click(object? sender, EventArgs e) =>
        DisaAktar("Excel dosyası (*.xlsx)|*.xlsx", "xlsx",
            (belge, yol) => belge.ExportToXlsx(yol, new XlsxExportOptions { ExportMode = XlsxExportMode.SingleFile, SheetName = "Rapor" }));

    private void DisaAktar(string filtre, string uzanti, Action<XtraReport, string> aktar)
    {
        if (_belge is null)
            return;
        using var dialog = new XtraSaveFileDialog
        {
            Filter = filtre, DefaultExt = uzanti, AddExtension = true,
            FileName = $"{DosyaAdi(_belge.DisplayName)}_{DateTime.Now:yyyyMMdd_HHmm}.{uzanti}",
        };
        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            aktar(_belge, dialog.FileName);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // Dosya başka bir programda açıksa veya klasöre yazılamıyorsa anlaşılır mesaj (ayrıntı logda).
            Serilog.Log.Warning(ex, "Rapor dışa aktarılamadı: {Yol}", dialog.FileName);
            HataYonetimi.Uyari(this, "Dosya kaydedilemedi. Dosya başka bir programda açıksa kapatıp tekrar deneyin veya başka bir klasör seçin.",
                "Dışa aktarma");
            return;
        }

        HataYonetimi.Bilgi(this, $"Rapor kaydedildi:\n{dialog.FileName}");
    }

    private static string DosyaAdi(string ad) =>
        string.Concat(ad.Select(c => Path.GetInvalidFileNameChars().Contains(c) || c == ' ' ? '_' : c));

    private void btnYazdir_Click(object? sender, EventArgs e)
    {
        if (_belge is not null)
            new ReportPrintTool(_belge).PrintDialog();
    }
}
