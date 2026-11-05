using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.GonulluYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraScheduler;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 14 – Gönüllüler (REQ-213, şartname M12): gönüllü kaydı ve uzmanlık alanı, SchedulerControl ile haftalık vardiya planı,
/// görev ataması (görev tanımı ve/veya açık talep). Takvimde doğrudan düzenleme kapalıdır; vardiyalar alttaki alanlardan
/// kaydedilir ki çakışma ve süre kuralları <see cref="IGonulluServisi"/>'nde denetlensin.
/// </summary>
public partial class GonullulerForm : XtraForm
{
    private sealed record GonulluSatiri(int Id, string AdSoyad, string Ad, string Soyad, string? Telefon, string UzmanlikAlani, string? Notlar,
        int VardiyaSayisi, DateTime? SonrakiVardiya, string KayitDurumu, bool Aktif);

    private readonly IGonulluServisi _servis;
    private readonly ICadirServisi _cadirServisi;
    private readonly SchedulerDataStorage _takvimVerisi = new();
    private readonly Dictionary<Appointment, VardiyaOzeti> _vardiyalar = [];
    private List<GonulluSatiri> _gonulluler = [];
    private VardiyaOzeti? _seciliVardiya;
    private bool _yeniGonullu = true;
    private bool _yukleniyor;

    public GonullulerForm(IGonulluServisi servis, ICadirServisi cadirServisi)
    {
        InitializeComponent();
        _servis = servis;
        _cadirServisi = cadirServisi;
        Disposed += (_, _) => _takvimVerisi.Dispose();

        TakvimiHazirla();
        foreach (var cmb in new[] { cmbVGonullu, cmbVBolge, cmbVTalep })
            cmb.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbVBolge.Properties.NullText = "(bölge belirtilmedi)";
        cmbVTalep.Properties.NullText = "(talebe bağlı değil)";
        foreach (var dte in new[] { dteVBaslangic, dteVBitis })
        {
            dte.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            dte.Properties.DisplayFormat.FormatString = "dd.MM.yyyy HH:mm";
            dte.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dte.Properties.EditFormat.FormatString = "dd.MM.yyyy HH:mm";
            dte.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dte.Properties.MaskSettings.Set("mask", "dd.MM.yyyy HH:mm");
        }

        txtVGorev.Properties.NullValuePrompt = "Örn. “B bölgesinde çadır kurulumu”, “Dağıtım noktasında sıra düzeni”";
        GridYardimcisi.BosMetinGoster(viewGonulluler, "Henüz gönüllü kaydı yok. Sağdaki alanları doldurup “Kaydet”e basın.");
        viewGonulluler.RowStyle += (_, e) =>
        {
            if (viewGonulluler.GetRow(e.RowHandle) is GonulluSatiri { Aktif: false })
                e.Appearance.ForeColor = Color.Gray;
        };

        YeniGonulluDurumu();
        YeniVardiyaDurumu(null, null);
    }

    /// <summary>Takvim yalnızca gösterir; ekleme/düzenleme alttaki alanlardan yapılır (kurallar Business'ta).</summary>
    private void TakvimiHazirla()
    {
        scheduler.DataStorage = _takvimVerisi;
        scheduler.ActiveViewType = SchedulerViewType.FullWeek;
        scheduler.Start = HaftaBasi(DateTime.Today);
        scheduler.FullWeekView.TopRowTime = TimeSpan.FromHours(7);
        scheduler.FullWeekView.TimeRulers.Add(new TimeRuler { Caption = "Saat" });
        var izin = scheduler.OptionsCustomization;
        izin.AllowAppointmentCreate = UsedAppointmentType.None;
        izin.AllowAppointmentEdit = UsedAppointmentType.None;
        izin.AllowAppointmentDelete = UsedAppointmentType.None;
        izin.AllowAppointmentDrag = UsedAppointmentType.None;
        izin.AllowAppointmentResize = UsedAppointmentType.None;
        izin.AllowAppointmentCopy = UsedAppointmentType.None;
        izin.AllowInplaceEditor = UsedAppointmentType.None;
        scheduler.EditAppointmentFormShowing += (_, e) => e.Handled = true;
        scheduler.PopupMenuShowing += (_, e) => e.Menu.Items.Clear();
        scheduler.SelectionChanged += scheduler_SelectionChanged;
        scheduler.DoubleClick += scheduler_DoubleClick;
        // Takvimin kendi ileri/geri okları da haftayı değiştirir; vardiyalar yeniden okunur.
        scheduler.VisibleIntervalChanged += async (_, _) =>
        {
            if (!_yukleniyor && IsHandleCreated)
                await TakvimiYukleAsync();
        };
    }

    private static DateTime HaftaBasi(DateTime gun) => gun.Date.AddDays(-(((int)gun.DayOfWeek + 6) % 7));

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await SecenekleriYukleAsync();
        await GonulluleriYukleAsync();
        await TakvimiYukleAsync();
    }

    private GonulluSatiri? SeciliGonullu => viewGonulluler.GetFocusedRow() as GonulluSatiri;

    // ---------------------------------------------------------------- gönüllüler

    private async Task GonulluleriYukleAsync(int? secilecekId = null)
    {
        IReadOnlyList<GonulluOzeti> liste = [];
        var pasifler = chkPasifler.Checked;
        if (!await Bekleme.CalistirAsync(this, async () => liste = await _servis.ListeleAsync(pasifler)))
            return;

        secilecekId ??= SeciliGonullu?.Id;
        _gonulluler = liste.Select(g => new GonulluSatiri(g.Id, g.AdSoyad, g.Ad, g.Soyad, g.Telefon, g.UzmanlikAlani, g.Notlar,
            g.VardiyaSayisi, g.SonrakiVardiya, g.Aktif ? "Aktif" : "Pasif", g.Aktif)).ToList();
        _yukleniyor = true;
        gridGonulluler.DataSource = _gonulluler;
        var satir = secilecekId is { } id ? viewGonulluler.LocateByValue(nameof(GonulluSatiri.Id), id) : -1;
        viewGonulluler.FocusedRowHandle = satir >= 0 ? satir : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        _yukleniyor = false;
        grpGonulluler.Text = $"Gönüllüler – {_gonulluler.Count(g => g.Aktif)} aktif";
        GonulluSeciminiGoster();

        var seciliVardiyaGonullusu = cmbVGonullu.EditValue;
        cmbVGonullu.Properties.Items.Clear();
        foreach (var g in _gonulluler.Where(g => g.Aktif))
            cmbVGonullu.Properties.Items.Add(new ImageComboBoxItem($"{g.AdSoyad} – {g.UzmanlikAlani}", (object)g.Id));
        cmbVGonullu.EditValue = seciliVardiyaGonullusu is int vId && _gonulluler.Any(g => g.Id == vId && g.Aktif) ? vId : null;
    }

    private void GonulluSeciminiGoster()
    {
        if (SeciliGonullu is not { } g)
        {
            YeniGonulluDurumu();
            return;
        }

        _yeniGonullu = false;
        txtAd.Text = g.Ad;
        txtSoyad.Text = g.Soyad;
        txtTelefon.Text = g.Telefon;
        txtUzmanlik.Text = g.UzmanlikAlani;
        memoNotlar.Text = g.Notlar;
        btnSil.Enabled = btnAktiflik.Enabled = true;
        btnAktiflik.Text = g.Aktif ? "Pasif yap" : "Aktif yap";
        grpGonulluler.Text = $"Gönüllüler – {g.AdSoyad} düzenleniyor";
    }

    private void YeniGonulluDurumu()
    {
        _yeniGonullu = true;
        txtAd.Text = txtSoyad.Text = txtTelefon.Text = txtUzmanlik.Text = memoNotlar.Text = "";
        btnSil.Enabled = btnAktiflik.Enabled = false;
        btnAktiflik.Text = "Pasif yap";
    }

    private void viewGonulluler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            GonulluSeciminiGoster();
    }

    private async void chkPasifler_CheckedChanged(object? sender, EventArgs e) => await GonulluleriYukleAsync();

    private void btnYeni_Click(object? sender, EventArgs e)
    {
        viewGonulluler.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        YeniGonulluDurumu();
        grpGonulluler.Text = "Gönüllüler – yeni gönüllü";
        txtAd.Focus();
    }

    private async void btnKaydet_Click(object? sender, EventArgs e)
    {
        var id = _yeniGonullu ? null : SeciliGonullu?.Id;
        var bilgi = new GonulluBilgisi
        {
            Ad = txtAd.Text, Soyad = txtSoyad.Text, Telefon = BosIseNull(txtTelefon.Text), UzmanlikAlani = txtUzmanlik.Text,
            Notlar = BosIseNull(memoNotlar.Text),
        };
        Entities.Kisiler.Gonullu? kayit = null;
        if (!await Bekleme.CalistirAsync(this, async () => kayit = await _servis.KaydetAsync(id, bilgi)))
            return;
        HataYonetimi.Bilgi(this, id is null ? $"{kayit!.AdSoyad} gönüllü olarak eklendi." : $"{kayit!.AdSoyad} güncellendi.");
        await GonulluleriYukleAsync(kayit.Id);
        await TakvimiYukleAsync();
    }

    private async void btnSil_Click(object? sender, EventArgs e)
    {
        if (SeciliGonullu is not { } g || !HataYonetimi.Onayla(this, $"{g.AdSoyad} silinsin mi?", "Gönüllüyü sil"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _servis.SilAsync(g.Id)))
            await GonulluleriYukleAsync();
    }

    private async void btnAktiflik_Click(object? sender, EventArgs e)
    {
        if (SeciliGonullu is not { } g)
            return;
        if (g.Aktif && !HataYonetimi.Onayla(this, $"{g.AdSoyad} pasif yapılsın mı? Pasif gönüllüye vardiya verilemez.", "Pasif yap"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _servis.AktiflikDegistirAsync(g.Id, !g.Aktif)))
            await GonulluleriYukleAsync(g.Id);
    }

    // ---------------------------------------------------------------- takvim

    private async Task SecenekleriYukleAsync()
    {
        IReadOnlyList<Entities.Cadirlar.Bolge> bolgeler = [];
        IReadOnlyList<AtanabilirTalep> talepler = [];
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                bolgeler = await _cadirServisi.BolgeleriListeleAsync();
                talepler = await _servis.AtanabilirTaleplerAsync();
            }))
            return;

        var bolge = cmbVBolge.EditValue;
        cmbVBolge.Properties.Items.Clear();
        cmbVBolge.Properties.Items.Add(new ImageComboBoxItem("(bölge belirtilmedi)", (object?)null));
        foreach (var b in bolgeler)
            cmbVBolge.Properties.Items.Add(new ImageComboBoxItem($"{b.Kod} – {b.Ad}", (object)b.Id));
        cmbVBolge.EditValue = bolge is int bId && bolgeler.Any(b => b.Id == bId) ? bId : null;

        TalepSecenekleri(talepler, cmbVTalep.EditValue as int?);
    }

    private void TalepSecenekleri(IReadOnlyList<AtanabilirTalep> talepler, int? secili, string? seciliMetin = null)
    {
        cmbVTalep.Properties.Items.Clear();
        cmbVTalep.Properties.Items.Add(new ImageComboBoxItem("(talebe bağlı değil)", (object?)null));
        foreach (var t in talepler)
            cmbVTalep.Properties.Items.Add(new ImageComboBoxItem(t.Ozet, (object)t.Id));
        // Bağlı talep sonradan kapandıysa da seçimde görünsün.
        if (secili is { } id && talepler.All(t => t.Id != id))
            cmbVTalep.Properties.Items.Add(new ImageComboBoxItem($"#{id} (kapandı) {seciliMetin}", (object)id));
        cmbVTalep.EditValue = secili;
        _talepler = talepler;
    }

    private IReadOnlyList<AtanabilirTalep> _talepler = [];

    private async Task TakvimiYukleAsync(int? secilecekVardiyaId = null)
    {
        var bas = scheduler.Start.Date;
        var bit = bas.AddDays(7);
        IReadOnlyList<VardiyaOzeti> vardiyalar = [];
        if (!await Bekleme.CalistirAsync(this, async () => vardiyalar = await _servis.VardiyalariGetirAsync(bas, bit)))
            return;

        _yukleniyor = true;
        scheduler.BeginUpdate();
        try
        {
            _takvimVerisi.Appointments.Clear();
            _vardiyalar.Clear();
            Appointment? secilecek = null;
            foreach (var v in vardiyalar)
            {
                var randevu = _takvimVerisi.CreateAppointment(AppointmentType.Normal);
                randevu.Start = v.Baslangic;
                randevu.End = v.Bitis;
                randevu.Subject = v.Baslik;
                randevu.Location = v.BolgeAdi ?? "";
                randevu.Description = v.UzmanlikAlani;
                _takvimVerisi.Appointments.Add(randevu);
                _vardiyalar[randevu] = v;
                if (v.Id == secilecekVardiyaId)
                    secilecek = randevu;
            }

            if (secilecek is not null)
                scheduler.SelectedAppointments.Add(secilecek);
        }
        finally
        {
            scheduler.EndUpdate();
            _yukleniyor = false;
        }

        lblHafta.Text = $"<b>{bas:dd MMMM}</b> – <b>{bit.AddDays(-1):dd MMMM yyyy}</b> · {vardiyalar.Count} vardiya, " +
                        $"{vardiyalar.Select(v => v.GonulluId).Distinct().Count()} gönüllü";
        if (secilecekVardiyaId is { } vId && vardiyalar.FirstOrDefault(v => v.Id == vId) is { } secili)
            VardiyayiGoster(secili);
    }

    private void btnOnceki_Click(object? sender, EventArgs e) => HaftayaGit(scheduler.Start.AddDays(-7));

    private void btnSonraki_Click(object? sender, EventArgs e) => HaftayaGit(scheduler.Start.AddDays(7));

    private void btnBugun_Click(object? sender, EventArgs e) => HaftayaGit(DateTime.Today);

    /// <summary>Haftayı değiştirir; vardiyalar <c>VisibleIntervalChanged</c> ile yeniden okunur.</summary>
    private void HaftayaGit(DateTime gun) => scheduler.Start = HaftaBasi(gun);

    private void scheduler_SelectionChanged(object? sender, EventArgs e)
    {
        if (_yukleniyor)
            return;
        if (scheduler.SelectedAppointments.Count > 0 && _vardiyalar.TryGetValue(scheduler.SelectedAppointments[0], out var vardiya))
            VardiyayiGoster(vardiya);
    }

    /// <summary>Boş saate çift tıklama: seçili aralık ve seçili gönüllüyle yeni vardiya hazırlanır.</summary>
    private void scheduler_DoubleClick(object? sender, EventArgs e)
    {
        if (scheduler.SelectedAppointments.Count > 0)
            return;
        var aralik = scheduler.SelectedInterval;
        YeniVardiyaDurumu(aralik.Start, aralik.End);
        cmbVGonullu.Focus();
    }

    private void VardiyayiGoster(VardiyaOzeti v)
    {
        _seciliVardiya = v;
        if (!cmbVGonullu.Properties.Items.Cast<ImageComboBoxItem>().Any(i => Equals(i.Value, v.GonulluId)))
            cmbVGonullu.Properties.Items.Add(new ImageComboBoxItem($"{v.GonulluAdi} – {v.UzmanlikAlani} (pasif)", (object)v.GonulluId));
        cmbVGonullu.EditValue = v.GonulluId;
        dteVBaslangic.EditValue = v.Baslangic;
        dteVBitis.EditValue = v.Bitis;
        cmbVBolge.EditValue = v.BolgeId;
        txtVGorev.Text = v.GorevTanimi;
        TalepSecenekleri(_talepler, v.TalepId, v.TalepOzeti);
        var bitti = v.Bitis <= DateTime.Now;
        btnVKaydet.Enabled = btnVSil.Enabled = !bitti;
        grpVardiya.Text = bitti ? $"Vardiya – {v.Baslik} (bitti; kayıt olarak kalır)" : $"Vardiya – {v.Baslik} düzenleniyor";
    }

    private void YeniVardiyaDurumu(DateTime? baslangic, DateTime? bitis)
    {
        _seciliVardiya = null;
        var bas = baslangic ?? DateTime.Today.AddDays(1).AddHours(8);
        dteVBaslangic.EditValue = bas;
        dteVBitis.EditValue = bitis is { } b && b > bas ? b : bas.AddHours(4);
        cmbVGonullu.EditValue = SeciliGonullu is { Aktif: true } g ? g.Id : cmbVGonullu.EditValue;
        txtVGorev.Text = "";
        cmbVTalep.EditValue = null;
        btnVKaydet.Enabled = true;
        btnVSil.Enabled = false;
        grpVardiya.Text = "Vardiya ve görev ataması – yeni vardiya";
    }

    private void btnVYeni_Click(object? sender, EventArgs e)
    {
        scheduler.SelectedAppointments.Clear();
        YeniVardiyaDurumu(null, null);
        cmbVGonullu.Focus();
    }

    private async void btnVKaydet_Click(object? sender, EventArgs e)
    {
        if (cmbVGonullu.EditValue is not int gonulluId)
        {
            HataYonetimi.Uyari(this, "Vardiya verilecek gönüllüyü seçin.", "Eksik bilgi");
            cmbVGonullu.Focus();
            return;
        }

        if (dteVBaslangic.EditValue is not DateTime bas || dteVBitis.EditValue is not DateTime bit)
        {
            HataYonetimi.Uyari(this, "Vardiyanın başlangıç ve bitiş zamanını girin.", "Eksik bilgi");
            return;
        }

        var bilgi = new VardiyaBilgisi
        {
            GonulluId = gonulluId, Baslangic = bas, Bitis = bit, BolgeId = cmbVBolge.EditValue as int?,
            GorevTanimi = BosIseNull(txtVGorev.Text), TalepId = cmbVTalep.EditValue as int?,
        };
        VardiyaOzeti? kayit = null;
        var id = _seciliVardiya?.Id;
        if (!await Bekleme.CalistirAsync(this, async () => kayit = await _servis.VardiyaKaydetAsync(id, bilgi)))
            return;

        // Kayıtlı vardiyanın haftası takvimde açılır.
        _yukleniyor = true;
        scheduler.Start = HaftaBasi(kayit!.Baslangic);
        _yukleniyor = false;
        await GonulluleriYukleAsync();
        await TakvimiYukleAsync(kayit.Id);
    }

    private async void btnVSil_Click(object? sender, EventArgs e)
    {
        if (_seciliVardiya is not { } v || !HataYonetimi.Onayla(this, $"{v.Baslik} ({v.Baslangic:dd.MM HH:mm}–{v.Bitis:HH:mm}) silinsin mi?", "Vardiyayı sil"))
            return;
        if (!await Bekleme.CalistirAsync(this, () => _servis.VardiyaSilAsync(v.Id)))
            return;
        YeniVardiyaDurumu(null, null);
        await GonulluleriYukleAsync();
        await TakvimiYukleAsync();
    }

    private static string? BosIseNull(string? metin) => string.IsNullOrWhiteSpace(metin) ? null : metin;
}
