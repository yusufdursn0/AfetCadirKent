using AfetCadirKent.Business.KioskIslemleri;
using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 15 – Kiosk (REQ-214, şartname M11): "Kiosk" rolündeki hesapla tam ekran açılır. Afetzede aile numarası ve doğum yılıyla
/// girer, talep bırakır, kendi taleplerinin durumunu ve yayındaki duyuruları görür. Çadır ve iletişim bilgisi gösterilmez (K-052).
/// Kullanılmayan ekranda aile oturumu kendiliğinden kapanır; kiosktan çıkışı yalnızca kiosk hesabının parolasını bilen görevli
/// yapar (Ctrl+Shift+Q) (K-088).
/// </summary>
public partial class KioskForm : XtraForm, IMessageFilter
{
    private const int HareketsizlikSaniyesi = 120;
    private const int DuyuruYenilemeDakikasi = 5;

    private readonly IKioskServisi _kiosk;
    private readonly IOturumServisi _oturumServisi;
    private readonly System.Windows.Forms.Timer _zamanlayici = new() { Interval = 1000 };
    private readonly Font _buyukYazi = new("Segoe UI", 15F);
    private readonly Font _baslikYazisi = new("Segoe UI", 24F);
    private DateTime _sonHareket = DateTime.Now;
    private DateTime _sonDuyuruYenileme = DateTime.MinValue;
    private bool _cikisOnaylandi;

    public KioskForm(IKioskServisi kiosk, IOturumServisi oturumServisi)
    {
        InitializeComponent();
        _kiosk = kiosk;
        _oturumServisi = oturumServisi;

        YazilariBuyut();
        MenuKutucuklariniOlustur();
        txtDogumYili.Properties.MaskSettings.Set("mask", "0000");
        txtDogumYili.Properties.MaskSettings.MaskExpression = "0000";
        GridYardimcisi.BosMetinGoster(viewTaleplerim, "Henüz talebiniz yok.");
        AcceptButton = btnGiris;

        _zamanlayici.Tick += zamanlayici_Tick;
        Application.AddMessageFilter(this);
        Disposed += (_, _) =>
        {
            Application.RemoveMessageFilter(this);
            _zamanlayici.Dispose();
            _buyukYazi.Dispose();
            _baslikYazisi.Dispose();
        };
        GirisEkrani();
    }

    /// <summary>Görevli kiosktan parolayla çıktıysa true (uygulama giriş ekranına döner).</summary>
    public bool OturumKapatildi => _cikisOnaylandi;

    private void YazilariBuyut()
    {
        lblBaslik.Appearance.Font = _baslikYazisi;
        foreach (var etiket in new[] { lblGirisBilgi, lblGirisMesaj, lblHosgeldin })
            etiket.Appearance.Font = _buyukYazi;
        foreach (var editor in new BaseEdit[] { txtAileNo, txtDogumYili, memoTalep })
            editor.Properties.Appearance.Font = _buyukYazi;
        foreach (var dugme in new[] { btnGiris, btnTalepGonder })
            dugme.Appearance.Font = _buyukYazi;
        layoutControl.Appearance.ControlFocused.Font = _buyukYazi;
        foreach (var oge in new[] { lciAileNo, lciDogumYili, lciTalep, lciTaleplerim })
            oge.AppearanceItemCaption.Font = _buyukYazi;
        foreach (var grup in new[] { grpGiris, grpAile, grpDuyurular })
            grup.AppearanceGroup.Font = _buyukYazi;
        viewTaleplerim.Appearance.Row.Font = _buyukYazi;
        viewTaleplerim.Appearance.HeaderPanel.Font = _buyukYazi;
        lblGirisMesaj.Appearance.ForeColor = Color.Firebrick;
    }

    private enum MenuKomutu
    {
        YeniTalep,
        Yenile,
        AileCikisi,
    }

    private void MenuKutucuklariniOlustur()
    {
        var grup = new TileGroup();
        foreach (var (komut, metin, renk) in new[]
                 {
                     (MenuKomutu.YeniTalep, "Yeni talep", Color.FromArgb(0, 120, 212)),
                     (MenuKomutu.Yenile, "Yenile", Color.FromArgb(16, 124, 16)),
                     (MenuKomutu.AileCikisi, "Çıkış", Color.FromArgb(164, 38, 44)),
                 })
        {
            var oge = new TileItem { Tag = komut, ItemSize = TileItemSize.Wide };
            oge.AppearanceItem.Normal.BackColor = renk;
            oge.AppearanceItem.Normal.BorderColor = renk;
            oge.AppearanceItem.Normal.ForeColor = Color.White;
            var yazi = new TileItemElement { Text = metin, TextAlignment = TileItemContentAlignment.MiddleCenter };
            yazi.Appearance.Normal.FontSizeDelta = 3;
            yazi.Appearance.Normal.FontStyleDelta = FontStyle.Bold;
            oge.Elements.Add(yazi);
            grup.Items.Add(oge);
        }

        tileMenu.Groups.Add(grup);
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _zamanlayici.Start();
        await DuyurulariYukleAsync();
        txtAileNo.Focus();
    }

    // ---------------------------------------------------------------- giriş ve aile oturumu

    private void GirisEkrani()
    {
        grpGiris.Visibility = LayoutVisibility.Always;
        grpAile.Visibility = LayoutVisibility.Never;
        txtAileNo.Text = txtDogumYili.Text = memoTalep.Text = "";
        gridTaleplerim.DataSource = null;
        AcceptButton = btnGiris;
    }

    private async void btnGiris_Click(object? sender, EventArgs e)
    {
        lblGirisMesaj.Text = "";
        if (!int.TryParse(txtDogumYili.Text, out var yil))
        {
            lblGirisMesaj.Text = "Doğum yılını dört haneli olarak girin (ör. 1985).";
            txtDogumYili.Focus();
            return;
        }

        KioskAilesi? aile = null;
        try
        {
            aile = await _kiosk.AileGirisiAsync(txtAileNo.Text, yil);
        }
        catch (IsKuraliException ex)
        {
            // Hatalı aile no / yıl ve bekletme mesajı ekranda kalır (kioskta açılır pencere yerine).
            lblGirisMesaj.Text = ex.Message;
            txtDogumYili.Text = "";
            txtAileNo.Focus();
            return;
        }
        catch (Exception ex)
        {
            HataYonetimi.Goster(this, ex);
            return;
        }

        grpGiris.Visibility = LayoutVisibility.Never;
        grpAile.Visibility = LayoutVisibility.Always;
        lblHosgeldin.Text = $"Hoş geldiniz, <b>{aile.AileNo} {aile.AileAdi}</b>";
        AcceptButton = null;
        await TaleplerimiYukleAsync();
        memoTalep.Focus();
    }

    private void AileCikisiYap()
    {
        _kiosk.AileCikisi();
        GirisEkrani();
        txtAileNo.Focus();
    }

    private async Task TaleplerimiYukleAsync()
    {
        IReadOnlyList<KioskTalebi> talepler = [];
        if (!await Bekleme.CalistirAsync(this, async () => talepler = await _kiosk.TaleplerimAsync()))
            return;
        gridTaleplerim.DataSource = talepler.ToList();
        lciTaleplerim.Text = $"Taleplerim ({talepler.Count(t => t.Durum is not (Entities.Enumlar.TalepDurumu.Tamamlandi or Entities.Enumlar.TalepDurumu.Iptal))} açık)";
    }

    private async void tileMenu_ItemClick(object? sender, TileItemEventArgs e)
    {
        switch (e.Item.Tag)
        {
            case MenuKomutu.YeniTalep:
                memoTalep.Focus();
                break;
            case MenuKomutu.Yenile:
                await TaleplerimiYukleAsync();
                break;
            case MenuKomutu.AileCikisi:
                AileCikisiYap();
                break;
        }
    }

    private async void btnTalepGonder_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(memoTalep.Text))
        {
            HataYonetimi.Uyari(this, "Talebinizi kısaca yazın.", "Talep");
            memoTalep.Focus();
            return;
        }

        KioskTalebi? talep = null;
        var metin = memoTalep.Text;
        if (!await Bekleme.CalistirAsync(this, async () => talep = await _kiosk.TalepBirakAsync(metin)))
            return;

        memoTalep.Text = "";
        HataYonetimi.Bilgi(this, $"Talebiniz alındı (No: {talep!.Id}). Görevliler inceleyecek; durumunu bu ekrandan takip edebilirsiniz.", "Talep alındı");
        await TaleplerimiYukleAsync();
    }

    // ---------------------------------------------------------------- duyurular

    private async Task DuyurulariYukleAsync()
    {
        IReadOnlyList<KioskDuyurusu> duyurular = [];
        try
        {
            duyurular = await _kiosk.DuyurulariGetirAsync();
        }
        catch (Exception ex)
        {
            // Duyurular okunamazsa kiosk çalışmaya devam eder; ayrıntı logda.
            Serilog.Log.Warning(ex, "Kiosk duyuruları okunamadı");
            return;
        }

        _sonDuyuruYenileme = DateTime.Now;
        tileDuyurular.BeginUpdate();
        try
        {
            tileDuyurular.Groups.Clear();
            var grup = new TileGroup();
            if (duyurular.Count == 0)
                grup.Items.Add(DuyuruKutucugu("Şu an yayında duyuru yok.", "", null));
            foreach (var d in duyurular)
                grup.Items.Add(DuyuruKutucugu(d.Baslik, d.Metin, d.BitisTarihi));
            tileDuyurular.Groups.Add(grup);
        }
        finally
        {
            tileDuyurular.EndUpdate();
        }

        grpDuyurular.Text = $"Duyurular ({duyurular.Count})";
    }

    private TileItem DuyuruKutucugu(string baslik, string metin, DateTime? bitis)
    {
        var oge = new TileItem { ItemSize = TileItemSize.Large };
        oge.AppearanceItem.Normal.BackColor = Color.FromArgb(242, 246, 252);
        oge.AppearanceItem.Normal.BorderColor = Color.FromArgb(0, 120, 212);
        oge.AppearanceItem.Normal.ForeColor = Color.FromArgb(32, 32, 32);
        var ust = new TileItemElement { Text = baslik, TextAlignment = TileItemContentAlignment.TopLeft };
        ust.Appearance.Normal.FontSizeDelta = 4;
        ust.Appearance.Normal.FontStyleDelta = FontStyle.Bold;
        oge.Elements.Add(ust);
        var govde = new TileItemElement
        {
            Text = metin.Kisalt(260), TextAlignment = TileItemContentAlignment.Manual, TextLocation = new Point(0, 34),
            Width = tileDuyurular.ItemSize * 2 + tileDuyurular.IndentBetweenItems - 24,
            Height = tileDuyurular.ItemSize * 2 - 70,
        };
        govde.Appearance.Normal.FontSizeDelta = 2;
        govde.Appearance.Normal.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        oge.Elements.Add(govde);
        if (bitis is { } b)
            oge.Elements.Add(new TileItemElement { Text = $"{b:dd.MM.yyyy} tarihine kadar", TextAlignment = TileItemContentAlignment.BottomRight });
        return oge;
    }

    // ---------------------------------------------------------------- hareketsizlik ve görevli çıkışı

    /// <summary>Uygulamadaki her tuş ve fare hareketi son hareket zamanını günceller.</summary>
    public bool PreFilterMessage(ref Message m)
    {
        const int WmKeyDown = 0x0100, WmMouseMove = 0x0200, WmLButtonDown = 0x0201;
        if (m.Msg is WmKeyDown or WmMouseMove or WmLButtonDown)
            _sonHareket = DateTime.Now;
        return false;
    }

    private async void zamanlayici_Tick(object? sender, EventArgs e)
    {
        if (_kiosk.Aile is not null && DateTime.Now - _sonHareket > TimeSpan.FromSeconds(HareketsizlikSaniyesi))
            AileCikisiYap();
        if (DateTime.Now - _sonDuyuruYenileme > TimeSpan.FromMinutes(DuyuruYenilemeDakikasi))
            await DuyurulariYukleAsync();
    }

    protected override async void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Control && e.Shift && e.KeyCode == Keys.Q)
        {
            e.Handled = true;
            await GorevliCikisiAsync();
        }
    }

    private async Task GorevliCikisiAsync()
    {
        var parola = XtraInputBox.Show(new XtraInputBoxArgs
        {
            Caption = "Kiosktan çıkış", Prompt = "Kiosk hesabının parolası (yalnızca görevli):", DefaultButtonIndex = 0,
            Editor = new TextEdit { Properties = { UseSystemPasswordChar = true } },
        }) as string;
        if (string.IsNullOrEmpty(parola))
            return;

        var dogru = false;
        if (!await Bekleme.CalistirAsync(this, async () => dogru = await _oturumServisi.ParolaDogrulaAsync(parola)))
            return;
        if (!dogru)
        {
            HataYonetimi.Uyari(this, "Parola hatalı.", "Kiosktan çıkış");
            return;
        }

        _kiosk.AileCikisi();
        _cikisOnaylandi = true;
        Close();
    }

    /// <summary>Kiosk, görevli parolası olmadan kapatılamaz (Alt+F4 dahil); Windows kapanırken kapanır.</summary>
    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (!_cikisOnaylandi && e.CloseReason == CloseReason.UserClosing)
            e.Cancel = true;
        base.OnFormClosing(e);
    }
}
