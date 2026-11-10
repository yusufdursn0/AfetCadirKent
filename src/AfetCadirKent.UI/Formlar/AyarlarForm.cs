using AfetCadirKent.Business.Ayarlar;
using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 17 – Ayarlar ve kullanıcılar (REQ-216): kullanıcılar (ekle, güncelle, parola sıfırla, pasif/aktif, sil), roller ve yetkileri,
/// gerekçe ayarları (gerekçe ↔ öncelikli ürün) ve kiosk duyuruları. Kurallar ilgili Business servislerindedir.
/// </summary>
public partial class AyarlarForm : XtraForm
{
    private sealed record YetkiSatiri(string Rol, string Modul, string Gorme, string Degistirme, string Rapor);

    private sealed record GerekceSatiri(GerekceKodu Kod, string Ad, string Oncelik, string Etki, string Urunler, bool Duzenlenebilir,
        IReadOnlyList<int> UrunIdler, bool TumUrunler);

    private sealed record KullaniciSatiri(int Id, string AdSoyad, string Ad, string Soyad, string KullaniciAdi, RolTuru Rol, string RolAdi,
        string? Telefon, string Durum, bool Aktif, DateTime? SonGirisZamani);

    private readonly IKullaniciServisi _kullaniciServisi;
    private readonly IGerekceAyarServisi _gerekceServisi;
    private readonly IDuyuruServisi _duyuruServisi;
    private readonly IOturum _oturum;
    private bool _yeniKullanici = true;
    private bool _yeniDuyuru = true;
    private bool _yukleniyor;

    public AyarlarForm(IKullaniciServisi kullaniciServisi, IGerekceAyarServisi gerekceServisi, IDuyuruServisi duyuruServisi,
        IOturum oturum)
    {
        InitializeComponent();
        _kullaniciServisi = kullaniciServisi;
        _gerekceServisi = gerekceServisi;
        _duyuruServisi = duyuruServisi;
        _oturum = oturum;

        tabKullanicilar.PageVisible = tabRoller.PageVisible = oturum.IzinVarMi(Modul.KullaniciYonetimi, Izin.Gorme);
        tabGerekceler.PageVisible = oturum.IzinVarMi(Modul.Ayarlar, Izin.Gorme);
        tabDuyurular.PageVisible = oturum.IzinVarMi(Modul.Duyuru, Izin.Gorme);

        cmbKRol.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        txtKParola.Properties.UseSystemPasswordChar = txtKParolaTekrar.Properties.UseSystemPasswordChar = true;
        foreach (var dte in new[] { dteDBaslangic, dteDBitis })
        {
            dte.Properties.CalendarTimeEditing = DevExpress.Utils.DefaultBoolean.True;
            dte.Properties.DisplayFormat.FormatString = dte.Properties.EditFormat.FormatString = "dd.MM.yyyy HH:mm";
            dte.Properties.DisplayFormat.FormatType = dte.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            dte.Properties.MaskSettings.Set("mask", "dd.MM.yyyy HH:mm");
        }

        GridYardimcisi.BosMetinGoster(viewKullanicilar, "Kullanıcı yok.");
        GridYardimcisi.BosMetinGoster(viewDuyurular, "Henüz duyuru yok. Sağdan yeni duyuru ekleyin.");
        viewKullanicilar.RowStyle += (_, e) =>
        {
            if (viewKullanicilar.GetRow(e.RowHandle) is KullaniciSatiri { Aktif: false })
                e.Appearance.ForeColor = Color.Gray;
        };
        viewDuyurular.RowStyle += (_, e) =>
        {
            if (viewDuyurular.GetRow(e.RowHandle) is DuyuruOzeti d && d.YayinDurumu != "Yayında")
                e.Appearance.ForeColor = Color.Gray;
        };

        YeniKullaniciDurumu();
        YeniDuyuruDurumu();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await YenileAsync();
    }

    private async Task YenileAsync()
    {
        if (tabKullanicilar.PageVisible)
        {
            await RolleriYukleAsync();
            await KullanicilariYukleAsync();
        }

        if (tabGerekceler.PageVisible)
            await GerekceleriYukleAsync();
        if (tabDuyurular.PageVisible)
            await DuyurulariYukleAsync();
    }

    private static string? BosIseNull(string? metin) => string.IsNullOrWhiteSpace(metin) ? null : metin;

    // ---------------------------------------------------------------- kullanıcılar ve roller

    private KullaniciSatiri? SeciliKullanici => viewKullanicilar.GetFocusedRow() as KullaniciSatiri;

    private async Task RolleriYukleAsync()
    {
        IReadOnlyList<Entities.Sistem.Rol> roller = [];
        if (!await Bekleme.CalistirAsync(this, async () => roller = await _kullaniciServisi.RolleriListeleAsync()))
            return;

        cmbKRol.Properties.Items.Clear();
        foreach (var rol in roller)
            cmbKRol.Properties.Items.Add(new ImageComboBoxItem($"{rol.Ad} – {rol.Aciklama}", rol.Tur));

        string Isaret(Entities.Sistem.Rol rol, Modul modul, Izin izin) => rol.Yetkiler.Any(y => y.Modul == modul && y.Izin == izin) ? "✔" : "";
        gridYetkiler.DataSource = roller
            .SelectMany(rol => rol.Yetkiler.Select(y => y.Modul).Distinct().OrderBy(m => m)
                .Select(m => new YetkiSatiri(rol.Ad, m.Metin(), Isaret(rol, m, Izin.Gorme), Isaret(rol, m, Izin.Degistirme), Isaret(rol, m, Izin.Rapor))))
            .ToList();
    }

    private async Task KullanicilariYukleAsync(int? secilecekId = null)
    {
        IReadOnlyList<KullaniciOzeti> liste = [];
        var pasifler = chkKPasifler.Checked;
        if (!await Bekleme.CalistirAsync(this, async () => liste = await _kullaniciServisi.ListeleAsync(pasifler)))
            return;

        secilecekId ??= SeciliKullanici?.Id;
        _yukleniyor = true;
        gridKullanicilar.DataSource = liste.Select(k => new KullaniciSatiri(k.Id, k.AdSoyad, k.Ad, k.Soyad, k.KullaniciAdi, k.Rol, k.RolAdi,
            k.Telefon, k.Aktif ? "Aktif" : "Pasif", k.Aktif, k.SonGirisZamani)).ToList();
        var satir = secilecekId is { } id ? viewKullanicilar.LocateByValue(nameof(KullaniciSatiri.Id), id) : -1;
        viewKullanicilar.FocusedRowHandle = satir >= 0 ? satir : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        _yukleniyor = false;
        grpKullanicilar.Text = $"Kullanıcılar – {liste.Count(k => k.Aktif)} aktif";
        KullaniciSeciminiGoster();
    }

    private void KullaniciSeciminiGoster()
    {
        if (SeciliKullanici is not { } k)
        {
            YeniKullaniciDurumu();
            return;
        }

        _yeniKullanici = false;
        txtKAd.Text = k.Ad;
        txtKSoyad.Text = k.Soyad;
        txtKTelefon.Text = k.Telefon;
        txtKKullaniciAdi.Text = k.KullaniciAdi;
        txtKKullaniciAdi.Properties.ReadOnly = true;
        cmbKRol.EditValue = k.Rol;
        txtKParola.Text = txtKParolaTekrar.Text = "";
        lciKParola.Text = "Yeni parola";
        lciKParolaTekrar.Text = "Yeni parola (tekrar)";
        var kendisi = k.Id == _oturum.KullaniciId;
        btnKParola.Enabled = true;
        btnKAktiflik.Enabled = btnKSil.Enabled = !kendisi;
        btnKAktiflik.Text = k.Aktif ? "Pasif yap" : "Aktif yap";
        grpKullanici.Text = $"Kullanıcı – {k.KullaniciAdi} düzenleniyor" + (kendisi ? " (siz)" : "");
    }

    private void YeniKullaniciDurumu()
    {
        _yeniKullanici = true;
        txtKAd.Text = txtKSoyad.Text = txtKTelefon.Text = txtKKullaniciAdi.Text = txtKParola.Text = txtKParolaTekrar.Text = "";
        txtKKullaniciAdi.Properties.ReadOnly = false;
        cmbKRol.EditValue = null;
        lciKParola.Text = "Parola";
        lciKParolaTekrar.Text = "Parola (tekrar)";
        btnKParola.Enabled = btnKAktiflik.Enabled = btnKSil.Enabled = false;
        btnKAktiflik.Text = "Pasif yap";
        grpKullanici.Text = "Yeni kullanıcı";
    }

    private void viewKullanicilar_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            KullaniciSeciminiGoster();
    }

    private async void chkKPasifler_CheckedChanged(object? sender, EventArgs e) => await KullanicilariYukleAsync();

    private void btnKYeni_Click(object? sender, EventArgs e)
    {
        viewKullanicilar.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        YeniKullaniciDurumu();
        txtKAd.Focus();
    }

    private async void btnKKaydet_Click(object? sender, EventArgs e)
    {
        if (cmbKRol.EditValue is not RolTuru rol)
        {
            HataYonetimi.Uyari(this, "Kullanıcının rolünü seçin.", "Eksik bilgi");
            cmbKRol.Focus();
            return;
        }

        int? id = null;
        if (_yeniKullanici)
        {
            var bilgi = new YeniKullaniciBilgisi
            {
                Ad = txtKAd.Text, Soyad = txtKSoyad.Text, KullaniciAdi = txtKKullaniciAdi.Text, Parola = txtKParola.Text,
                ParolaTekrar = txtKParolaTekrar.Text, Telefon = BosIseNull(txtKTelefon.Text),
            };
            KullaniciOzeti? eklenen = null;
            if (!await Bekleme.CalistirAsync(this, async () => eklenen = await _kullaniciServisi.EkleAsync(bilgi, rol)))
                return;
            id = eklenen!.Id;
            HataYonetimi.Bilgi(this, $"{eklenen.KullaniciAdi} kullanıcısı {eklenen.RolAdi} rolüyle eklendi.");
        }
        else if (SeciliKullanici is { } k)
        {
            id = k.Id;
            var (ad, soyad, telefon) = (txtKAd.Text, txtKSoyad.Text, BosIseNull(txtKTelefon.Text));
            if (!await Bekleme.CalistirAsync(this, () => _kullaniciServisi.GuncelleAsync(k.Id, ad, soyad, telefon, rol)))
                return;
            HataYonetimi.Bilgi(this, $"{k.KullaniciAdi} güncellendi.");
        }

        await KullanicilariYukleAsync(id);
    }

    private async void btnKParola_Click(object? sender, EventArgs e)
    {
        if (SeciliKullanici is not { } k)
            return;
        if (string.IsNullOrEmpty(txtKParola.Text))
        {
            HataYonetimi.Uyari(this, "Yeni parolayı ve tekrarını yazın, sonra “Parolayı sıfırla”ya basın.", "Parola sıfırlama");
            txtKParola.Focus();
            return;
        }

        var (parola, tekrar) = (txtKParola.Text, txtKParolaTekrar.Text);
        if (!await Bekleme.CalistirAsync(this, () => _kullaniciServisi.ParolaSifirlaAsync(k.Id, parola, tekrar)))
            return;
        txtKParola.Text = txtKParolaTekrar.Text = "";
        HataYonetimi.Bilgi(this, $"{k.KullaniciAdi} kullanıcısının parolası değiştirildi. Yeni parolayı kendisine güvenli biçimde iletin.");
    }

    private async void btnKAktiflik_Click(object? sender, EventArgs e)
    {
        if (SeciliKullanici is not { } k)
            return;
        if (k.Aktif && !HataYonetimi.Onayla(this, $"{k.KullaniciAdi} pasif yapılsın mı? Pasif kullanıcı giriş yapamaz.", "Pasif yap"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _kullaniciServisi.AktiflikDegistirAsync(k.Id, !k.Aktif)))
            await KullanicilariYukleAsync(k.Id);
    }

    private async void btnKSil_Click(object? sender, EventArgs e)
    {
        if (SeciliKullanici is not { } k || !HataYonetimi.Onayla(this, $"{k.KullaniciAdi} silinsin mi?", "Kullanıcıyı sil"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _kullaniciServisi.SilAsync(k.Id)))
            await KullanicilariYukleAsync();
    }

    // ---------------------------------------------------------------- gerekçe ayarları

    private GerekceSatiri? SeciliGerekce => viewGerekceler.GetFocusedRow() as GerekceSatiri;
    private IReadOnlyList<EslesmeUrunu> _urunler = [];

    private async Task GerekceleriYukleAsync(GerekceKodu? secilecek = null)
    {
        IReadOnlyList<GerekceAyari> gerekceler = [];
        IReadOnlyList<EslesmeUrunu> urunler = [];
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                gerekceler = await _gerekceServisi.GerekceleriGetirAsync();
                urunler = await _gerekceServisi.UrunleriGetirAsync();
            }))
            return;

        secilecek ??= SeciliGerekce?.Kod;
        _urunler = urunler;
        var adlar = urunler.ToDictionary(u => u.Id, u => u.Ad);
        _yukleniyor = true;
        gridGerekceler.DataSource = gerekceler.Select(g => new GerekceSatiri(g.Kod, g.Ad, g.DestekOnceligi.Metin(), g.YerlesimEtkisi.Metin(),
            g.TumUrunler ? "Tüm ürünler" : g.UrunIdler.Count == 0 ? "—" : string.Join(", ", g.UrunIdler.Select(id => adlar.GetValueOrDefault(id, "?")).Order()),
            !g.TumUrunler && g.Kod != GerekceKodu.OzelDurumYok, g.UrunIdler, g.TumUrunler)).ToList();
        var satir = secilecek is { } kod ? viewGerekceler.LocateByValue(nameof(GerekceSatiri.Kod), kod) : 0;
        viewGerekceler.FocusedRowHandle = Math.Max(0, satir);
        viewGerekceler.BestFitColumns();
        _yukleniyor = false;
        GerekceSeciminiGoster();
    }

    private void GerekceSeciminiGoster()
    {
        chklUrunler.Items.Clear();
        txtGAciklama.Text = "";
        if (SeciliGerekce is not { } g)
            return;

        foreach (var u in _urunler.Where(u => u.Aktif || g.UrunIdler.Contains(u.Id)))
            chklUrunler.Items.Add(new CheckedListBoxItem(u.Id, $"{u.Ad} ({u.Kategori.Metin()}){(u.Aktif ? "" : " – pasif")}",
                g.TumUrunler || g.UrunIdler.Contains(u.Id) ? CheckState.Checked : CheckState.Unchecked));
        chklUrunler.Enabled = txtGAciklama.Enabled = btnGKaydet.Enabled = g.Duzenlenebilir;
        grpEslesme.Text = g.Duzenlenebilir ? $"Seçili gerekçe – {g.Ad}"
            : g.TumUrunler ? $"Seçili gerekçe – {g.Ad} (tüm ürünlerde öncelik; düzenlenmez)" : $"Seçili gerekçe – {g.Ad} (öncelik vermez)";
    }

    private void viewGerekceler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            GerekceSeciminiGoster();
    }

    private async void btnGKaydet_Click(object? sender, EventArgs e)
    {
        if (SeciliGerekce is not { } g)
            return;
        if (string.IsNullOrWhiteSpace(txtGAciklama.Text))
        {
            HataYonetimi.Uyari(this, "Değişikliğin açıklamasını yazın; işlem geçmişine kaydedilir.", "Açıklama zorunlu");
            txtGAciklama.Focus();
            return;
        }

        var secilenler = chklUrunler.CheckedItems.Cast<CheckedListBoxItem>().Select(i => (int)i.Value).ToList();
        var aciklama = txtGAciklama.Text;
        if (await Bekleme.CalistirAsync(this, () => _gerekceServisi.EslesmeleriKaydetAsync(g.Kod, secilenler, aciklama)))
        {
            HataYonetimi.Bilgi(this, $"'{g.Ad}' için öncelikli ürünler kaydedildi. Yeni dağıtım listelerinde bu sıralama kullanılır.");
            await GerekceleriYukleAsync(g.Kod);
        }
    }

    // ---------------------------------------------------------------- duyurular

    private DuyuruOzeti? SeciliDuyuru => viewDuyurular.GetFocusedRow() as DuyuruOzeti;

    private async Task DuyurulariYukleAsync(int? secilecekId = null)
    {
        IReadOnlyList<DuyuruOzeti> liste = [];
        var pasifler = chkDPasifler.Checked;
        if (!await Bekleme.CalistirAsync(this, async () => liste = await _duyuruServisi.ListeleAsync(pasifler)))
            return;

        secilecekId ??= SeciliDuyuru?.Id;
        _yukleniyor = true;
        gridDuyurular.DataSource = liste;
        var satir = secilecekId is { } id ? viewDuyurular.LocateByValue(nameof(DuyuruOzeti.Id), id) : -1;
        viewDuyurular.FocusedRowHandle = satir >= 0 ? satir : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        _yukleniyor = false;
        grpDuyurular.Text = $"Duyurular – {liste.Count(d => d.YayinDurumu == "Yayında")} yayında (kioskta görünür)";
        DuyuruSeciminiGoster();
    }

    private void DuyuruSeciminiGoster()
    {
        if (SeciliDuyuru is not { } d)
        {
            YeniDuyuruDurumu();
            return;
        }

        _yeniDuyuru = false;
        txtDBaslik.Text = d.Baslik;
        memoDMetin.Text = d.Metin;
        dteDBaslangic.EditValue = d.BaslangicTarihi;
        dteDBitis.EditValue = d.BitisTarihi;
        btnDAktiflik.Enabled = true;
        btnDAktiflik.Text = d.Aktif ? "Pasif yap" : "Yeniden yayına al";
        grpDuyuru.Text = $"Duyuru – {d.YayinDurumu}";
    }

    private void YeniDuyuruDurumu()
    {
        _yeniDuyuru = true;
        txtDBaslik.Text = memoDMetin.Text = "";
        var simdi = DateTime.Now;
        dteDBaslangic.EditValue = new DateTime(simdi.Year, simdi.Month, simdi.Day, simdi.Hour, 0, 0);
        dteDBitis.EditValue = DateTime.Today.AddDays(8).AddMinutes(-1);
        btnDAktiflik.Enabled = false;
        btnDAktiflik.Text = "Pasif yap";
        grpDuyuru.Text = "Yeni duyuru";
    }

    private void viewDuyurular_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            DuyuruSeciminiGoster();
    }

    private async void chkDPasifler_CheckedChanged(object? sender, EventArgs e) => await DuyurulariYukleAsync();

    private void btnDYeni_Click(object? sender, EventArgs e)
    {
        viewDuyurular.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        YeniDuyuruDurumu();
        txtDBaslik.Focus();
    }

    private async void btnDKaydet_Click(object? sender, EventArgs e)
    {
        if (dteDBaslangic.EditValue is not DateTime bas || dteDBitis.EditValue is not DateTime bit)
        {
            HataYonetimi.Uyari(this, "Başlangıç ve bitiş zamanını girin.", "Eksik bilgi");
            return;
        }

        var bilgi = new DuyuruBilgisi { Baslik = txtDBaslik.Text, Metin = memoDMetin.Text, BaslangicTarihi = bas, BitisTarihi = bit };
        var id = _yeniDuyuru ? null : SeciliDuyuru?.Id;
        DuyuruOzeti? kayit = null;
        if (!await Bekleme.CalistirAsync(this, async () => kayit = await _duyuruServisi.KaydetAsync(id, bilgi)))
            return;
        HataYonetimi.Bilgi(this, $"Duyuru kaydedildi: {kayit!.Baslik} ({kayit.YayinDurumu}).");
        await DuyurulariYukleAsync(kayit.Id);
    }

    private async void btnDAktiflik_Click(object? sender, EventArgs e)
    {
        if (SeciliDuyuru is not { } d)
            return;
        if (await Bekleme.CalistirAsync(this, () => _duyuruServisi.AktiflikDegistirAsync(d.Id, !d.Aktif)))
            await DuyurulariYukleAsync(d.Id);
    }
}
