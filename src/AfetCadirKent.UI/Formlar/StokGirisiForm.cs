using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Depo;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 9 – Stok girişi (REQ-208): ürün, parti numarası, miktar, son kullanma tarihi ve bağışçı ile yeni parti açılır.
/// Bağışçı kaydı da bu ekrandan yapılır (şartname Ekran 9). Kurallar (pasif ürün, geçmiş SKT, pozitif miktar) Business'tadır.
/// </summary>
public partial class StokGirisiForm : XtraForm
{
    private sealed record BagisciSatiri(int Id, string Ad, string? Telefon, string? Aciklama, string KayitDurumu, bool Aktif);

    private sealed record GirisSatiri(DateTime Zaman, string Urun, string PartiNo, string Miktar, string? Bagisci, DateTime? Skt,
        string? Aciklama, string? Kullanici);

    private const int SonGunSayisi = 30;

    private readonly IUrunServisi _urunServisi;
    private readonly IStokServisi _stokServisi;
    private List<Urun> _urunler = [];
    private bool _yeniBagisci = true;

    public StokGirisiForm(IUrunServisi urunServisi, IStokServisi stokServisi)
    {
        InitializeComponent();
        _urunServisi = urunServisi;
        _stokServisi = stokServisi;

        cmbUrun.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbBagisci.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbBagisci.Properties.NullText = "(bağışçı yok / belirtilmedi)";
        dteSkt.Properties.NullText = "(SKT yok)";
        dteSkt.Properties.MinValue = DateTime.Today;

        GridYardimcisi.BosMetinGoster(viewBagiscilar, "Henüz bağışçı kaydı yok.");
        GridYardimcisi.BosMetinGoster(viewGirisler, $"Son {SonGunSayisi} günde stok girişi yapılmamış.");
        GirisFormunuTemizle();
        YeniBagisciDurumu();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await YenileAsync();
    }

    private async Task YenileAsync(int? secilecekBagisciId = null)
    {
        IReadOnlyList<Urun> urunler = [];
        IReadOnlyList<Bagisci> bagiscilar = [];
        IReadOnlyList<StokHareketSatiri> girisler = [];
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                urunler = await _urunServisi.UrunleriListeleAsync();
                bagiscilar = await _urunServisi.BagiscilariListeleAsync(pasifleriDeGoster: true);
                girisler = await _stokServisi.HareketleriListeleAsync(new StokHareketFiltresi
                {
                    Tur = StokHareketTuru.Giris, Baslangic = DateOnly.FromDateTime(DateTime.Today.AddDays(-(SonGunSayisi - 1))),
                });
            }))
            return;

        _urunler = urunler.ToList();
        var seciliUrun = cmbUrun.EditValue;
        cmbUrun.Properties.Items.Clear();
        foreach (var u in _urunler)
            cmbUrun.Properties.Items.Add(new ImageComboBoxItem($"{u.Ad} ({u.Birim.Metin()})", (object)u.Id));
        cmbUrun.EditValue = seciliUrun is int id && _urunler.Any(u => u.Id == id) ? id : null;

        var seciliBagisci = secilecekBagisciId is { } yeniId ? yeniId : cmbBagisci.EditValue;
        cmbBagisci.Properties.Items.Clear();
        cmbBagisci.Properties.Items.Add(new ImageComboBoxItem("(bağışçı yok / belirtilmedi)", (object?)null));
        foreach (var b in bagiscilar.Where(b => b.Aktif))
            cmbBagisci.Properties.Items.Add(new ImageComboBoxItem(b.Ad, (object)b.Id));
        cmbBagisci.EditValue = seciliBagisci is int bId && bagiscilar.Any(b => b.Id == bId && b.Aktif) ? bId : null;

        var odakBagisci = secilecekBagisciId ?? SeciliBagisci?.Id;
        gridBagiscilar.DataSource = bagiscilar
            .Select(b => new BagisciSatiri(b.Id, b.Ad, b.Telefon, b.Aciklama, b.Aktif ? "Aktif" : "Pasif", b.Aktif)).ToList();
        var satir = odakBagisci is { } oId ? viewBagiscilar.LocateByValue(nameof(BagisciSatiri.Id), oId) : -1;
        if (satir >= 0)
            viewBagiscilar.FocusedRowHandle = satir;
        BagisciSeciminiGoster();

        gridGirisler.DataSource = girisler.Select(g => new GirisSatiri(g.Zaman, g.UrunAdi, g.PartiNo, $"{g.Miktar:#,##0.##}", g.BagisciAdi,
            g.SonKullanmaTarihi?.ToDateTime(TimeOnly.MinValue), g.Aciklama, g.KullaniciAdi)).ToList();
        viewGirisler.BestFitColumns();
        grpGirisler.Text = $"Son {SonGunSayisi} günün stok girişleri – {girisler.Count}";
    }

    private void cmbUrun_EditValueChanged(object? sender, EventArgs e)
    {
        var urun = cmbUrun.EditValue is int id ? _urunler.FirstOrDefault(u => u.Id == id) : null;
        lblBirim.Text = urun is null ? "" : urun.Birim.Metin();
    }

    private void GirisFormunuTemizle()
    {
        txtPartiNo.Text = "";
        spnMiktar.Value = 0;
        dteSkt.EditValue = null;
        txtAciklama.Text = "";
    }

    private async void btnYenile_Click(object? sender, EventArgs e) => await YenileAsync();

    private async void btnKaydet_Click(object? sender, EventArgs e)
    {
        if (cmbUrun.EditValue is not int urunId)
        {
            HataYonetimi.Uyari(this, "Ürünü seçin.", "Eksik bilgi");
            cmbUrun.Focus();
            return;
        }

        var bilgi = new StokGirisiBilgisi
        {
            UrunId = urunId, PartiNo = txtPartiNo.Text, Miktar = spnMiktar.Value,
            SonKullanmaTarihi = dteSkt.EditValue is DateTime skt ? DateOnly.FromDateTime(skt) : null,
            BagisciId = cmbBagisci.EditValue as int?, Aciklama = string.IsNullOrWhiteSpace(txtAciklama.Text) ? null : txtAciklama.Text,
        };
        Parti? parti = null;
        if (!await Bekleme.CalistirAsync(this, async () => parti = await _stokServisi.StokGirisiAsync(bilgi)))
            return;

        var urun = _urunler.First(u => u.Id == urunId);
        HataYonetimi.Bilgi(this, $"Stok girişi kaydedildi: {urun.Ad}, parti {parti!.PartiNo}, {bilgi.Miktar:#,##0.##} {urun.Birim.Metin()}.");
        GirisFormunuTemizle();
        await YenileAsync();
        txtPartiNo.Focus();
    }

    // ---------------------------------------------------------------- bağışçılar

    private BagisciSatiri? SeciliBagisci => viewBagiscilar.GetFocusedRow() as BagisciSatiri;

    private void viewBagiscilar_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => BagisciSeciminiGoster();

    private void BagisciSeciminiGoster()
    {
        if (SeciliBagisci is not { } b)
        {
            YeniBagisciDurumu();
            return;
        }

        _yeniBagisci = false;
        txtBagisciAd.Text = b.Ad;
        txtBagisciTelefon.Text = b.Telefon;
        txtBagisciAciklama.Text = b.Aciklama;
        grpBagiscilar.Text = $"Bağışçılar – {b.Ad} düzenleniyor";
        btnBagisciAktiflik.Enabled = true;
        btnBagisciAktiflik.Text = b.Aktif ? "Pasif yap" : "Aktif yap";
    }

    private void YeniBagisciDurumu()
    {
        _yeniBagisci = true;
        viewBagiscilar.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        txtBagisciAd.Text = txtBagisciTelefon.Text = txtBagisciAciklama.Text = "";
        grpBagiscilar.Text = "Bağışçılar – yeni bağışçı";
        btnBagisciAktiflik.Enabled = false;
        btnBagisciAktiflik.Text = "Pasif yap";
    }

    private void btnBagisciYeni_Click(object? sender, EventArgs e)
    {
        YeniBagisciDurumu();
        txtBagisciAd.Focus();
    }

    private async void btnBagisciKaydet_Click(object? sender, EventArgs e)
    {
        var id = _yeniBagisci ? null : SeciliBagisci?.Id;
        Bagisci? kayit = null;
        if (!await Bekleme.CalistirAsync(this, async () => kayit = await _urunServisi.BagisciKaydetAsync(id, txtBagisciAd.Text,
                string.IsNullOrWhiteSpace(txtBagisciTelefon.Text) ? null : txtBagisciTelefon.Text, txtBagisciAciklama.Text)))
            return;

        // Yeni bağışçı stok girişinde hemen seçili gelir.
        HataYonetimi.Bilgi(this, id is null ? $"'{kayit!.Ad}' eklendi ve stok girişinde seçildi." : $"'{kayit!.Ad}' güncellendi.");
        await YenileAsync(id is null ? kayit.Id : null);
    }

    private async void btnBagisciAktiflik_Click(object? sender, EventArgs e)
    {
        if (SeciliBagisci is not { } b)
            return;
        if (await Bekleme.CalistirAsync(this, () => _urunServisi.BagisciAktiflikDegistirAsync(b.Id, !b.Aktif)))
            await YenileAsync();
    }
}
