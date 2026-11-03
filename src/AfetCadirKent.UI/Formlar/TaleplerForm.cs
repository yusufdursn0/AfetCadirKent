using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 12 – Talepler (REQ-211): talep girişi, yapay zeka sınıflandırma önerisi ve görevli onayı, türe göre yönlendirme
/// (sağlık talebi öncelik almadan sağlık personeline gider), atama, durum takibi ve zorunlu sonuçla kapatma.
/// Rol bazlı görünürlük, atama yetkisi ve durum geçişleri <see cref="ITalepServisi"/>'ndedir.
/// </summary>
public partial class TaleplerForm : XtraForm
{
    private const string AcikFiltre = "acik";
    private const string SiniflandirilmamisFiltre = "sinifsiz";

    private sealed record TalepSatiri(int Id, DateTime AcilisZamani, string Kaynak, string Aile, string? Cadir, string Tur, string Oncelik,
        string Durum, string? Sorumlu, string Aciklama, DateTime? KapanisZamani, bool Acik, bool Siniflandirilmamis);

    private readonly ITalepServisi _talepServisi;
    private readonly IKayitServisi _kayitServisi;
    private readonly ICadirServisi _cadirServisi;
    private readonly IOturum _oturum;
    private readonly bool _degistirebilir;
    private readonly bool _koordinator;
    private List<TalepOzeti> _talepler = [];
    private bool _yukleniyor;

    public TaleplerForm(ITalepServisi talepServisi, IKayitServisi kayitServisi, ICadirServisi cadirServisi, IOturum oturum)
    {
        InitializeComponent();
        _talepServisi = talepServisi;
        _kayitServisi = kayitServisi;
        _cadirServisi = cadirServisi;
        _oturum = oturum;
        _degistirebilir = oturum.IzinVarMi(Modul.Talep, Izin.Degistirme);
        _koordinator = oturum.Rol == RolTuru.Koordinator;

        foreach (var cmb in new[] { cmbFDurum, cmbFTur, cmbAile, cmbCadir, cmbKaynak, cmbYeniTur, cmbYeniOncelik, cmbTur, cmbOncelik, cmbGorevli })
            cmb.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

        _yukleniyor = true;
        cmbFDurum.Properties.Items.Add(new ImageComboBoxItem("Açık talepler", AcikFiltre));
        cmbFDurum.Properties.Items.Add(new ImageComboBoxItem("(tümü)", (object?)null));
        foreach (var durum in Enum.GetValues<TalepDurumu>())
            cmbFDurum.Properties.Items.Add(new ImageComboBoxItem(durum.Metin(), durum));
        cmbFDurum.EditValue = AcikFiltre;
        cmbFTur.Properties.Items.Add(new ImageComboBoxItem("(tüm türler)", (object?)null));
        cmbFTur.Properties.Items.Add(new ImageComboBoxItem("Sınıflandırılmamış", SiniflandirilmamisFiltre));
        foreach (var tur in Enum.GetValues<TalepTuru>())
            cmbFTur.Properties.Items.Add(new ImageComboBoxItem(tur.Metin(), tur));
        cmbFTur.EditValue = null;
        // Saha görevlisi yalnızca kendine atanan talepleri izler (şartname rolleri); diğer roller isterse süzer.
        chkBanaAtanan.Checked = oturum.Rol == RolTuru.SahaGorevlisi;
        _yukleniyor = false;

        cmbKaynak.Properties.Items.Add(new ImageComboBoxItem(TalepKaynagi.KayitMasasi.Metin(), TalepKaynagi.KayitMasasi));
        cmbKaynak.Properties.Items.Add(new ImageComboBoxItem(TalepKaynagi.Gorevli.Metin(), TalepKaynagi.Gorevli));
        cmbKaynak.EditValue = TalepKaynagi.KayitMasasi;
        TurSecenekleri(cmbYeniTur, "(yapay zeka sınıflandırsın)");
        TurSecenekleri(cmbTur, null);
        OncelikSecenekleri(cmbYeniOncelik);
        OncelikSecenekleri(cmbOncelik);
        cmbAile.Properties.NullText = "(aile belirtilmedi)";
        cmbCadir.Properties.NullText = "(ailenin çadırı)";
        memoSonuc.Properties.NullValuePrompt = "Tamamlarken veya iptal ederken sonuç zorunludur.";
        memoAciklama.Properties.NullValuePrompt = "Örn. “Çadırımız su alıyor.”";

        GridYardimcisi.BosMetinGoster(viewTalepler, "Bu filtreye uyan talep yok.");
        viewTalepler.RowStyle += (_, e) =>
        {
            if (viewTalepler.GetRow(e.RowHandle) is not TalepSatiri t)
                return;
            if (!t.Acik)
                e.Appearance.ForeColor = Color.Gray;
            else if (t.Siniflandirilmamis)
                e.Appearance.ForeColor = Color.DarkOrange;
        };

        lciAile.Visibility = oturum.IzinVarMi(Modul.Kayit, Izin.Gorme) ? LayoutVisibility.Always : LayoutVisibility.Never;
        lciCadir.Visibility = oturum.IzinVarMi(Modul.Cadir, Izin.Gorme) ? LayoutVisibility.Always : LayoutVisibility.Never;
        if (!_degistirebilir)
        {
            grpYeni.Visibility = LayoutVisibility.Never;
            foreach (var oge in new[] { lciYzOneri, lciYzOnayla, lciTur, lciOncelik, lciSiniflandir, bosSiniflandir, lciGorevli, lciAta,
                         lciIslemeAl, lciSonuc, lciTamamla, lciIptal })
                oge.Visibility = LayoutVisibility.Never;
        }

        DetayiGoster();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await SecenekleriYukleAsync();
        await YenileAsync();
    }

    private TalepOzeti? SeciliTalep => viewTalepler.GetFocusedRow() is TalepSatiri s ? _talepler.FirstOrDefault(t => t.Id == s.Id) : null;

    private static void TurSecenekleri(ImageComboBoxEdit cmb, string? bosMetin)
    {
        if (bosMetin is not null)
            cmb.Properties.Items.Add(new ImageComboBoxItem(bosMetin, (object?)null));
        foreach (var tur in Enum.GetValues<TalepTuru>())
            cmb.Properties.Items.Add(new ImageComboBoxItem(tur.Metin(), tur));
        cmb.EditValue = null;
    }

    private static void OncelikSecenekleri(ImageComboBoxEdit cmb)
    {
        cmb.Properties.Items.Add(new ImageComboBoxItem("(belirtilmedi)", (object?)null));
        foreach (var oncelik in Enum.GetValues<DestekOnceligi>().OrderDescending())
            cmb.Properties.Items.Add(new ImageComboBoxItem(oncelik.Metin(), oncelik));
        cmb.EditValue = null;
    }

    /// <summary>Yeni talep için aile ve çadır seçenekleri (yalnızca rolün görebildiği kayıtlar).</summary>
    private async Task SecenekleriYukleAsync()
    {
        if (!_degistirebilir)
            return;
        var aileGorur = lciAile.Visibility == LayoutVisibility.Always;
        var cadirGorur = lciCadir.Visibility == LayoutVisibility.Always;
        IReadOnlyList<Entities.Kayit.Aile> aileler = [];
        IReadOnlyList<CadirPlanOgesi> cadirlar = [];
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                if (aileGorur)
                    aileler = await _kayitServisi.AileleriListeleAsync();
                if (cadirGorur)
                    cadirlar = await _cadirServisi.CadirPlaniAsync();
            }))
            return;

        cmbAile.Properties.Items.Clear();
        cmbAile.Properties.Items.Add(new ImageComboBoxItem("(aile belirtilmedi)", (object?)null));
        foreach (var a in aileler.OrderBy(a => a.AileNo))
            cmbAile.Properties.Items.Add(new ImageComboBoxItem($"{a.AileNo} – {a.AileAdi}", (object)a.Id));
        cmbAile.EditValue = null;

        cmbCadir.Properties.Items.Clear();
        cmbCadir.Properties.Items.Add(new ImageComboBoxItem("(ailenin çadırı)", (object?)null));
        foreach (var c in cadirlar.Where(c => c.Aktif).OrderBy(c => c.Kod))
            cmbCadir.Properties.Items.Add(new ImageComboBoxItem(c.Kod, (object)c.Id));
        cmbCadir.EditValue = null;
    }

    private async Task YenileAsync(int? talepId = null)
    {
        var filtre = new TalepFiltresi
        {
            SadeceAcik = AcikFiltre.Equals(cmbFDurum.EditValue),
            Durum = cmbFDurum.EditValue as TalepDurumu?,
            SadeceSiniflandirilmamis = SiniflandirilmamisFiltre.Equals(cmbFTur.EditValue),
            Tur = cmbFTur.EditValue as TalepTuru?,
            SadeceBanaAtanan = chkBanaAtanan.Checked,
        };
        IReadOnlyList<TalepOzeti> talepler = [];
        if (!await Bekleme.CalistirAsync(this, async () => talepler = await _talepServisi.ListeleAsync(filtre)))
            return;

        talepId ??= SeciliTalep?.Id;
        _talepler = talepler.ToList();
        _yukleniyor = true;
        gridTalepler.DataSource = _talepler.Select(t => new TalepSatiri(t.Id, t.AcilisZamani, t.Kaynak.Metin(),
            t.AileNo is null ? "—" : $"{t.AileNo} {t.AileAdi}", t.CadirKodu, t.TurMetni, t.OncelikMetni, t.Durum.Metin(), t.SorumluAdi,
            t.Aciklama, t.KapanisZamani, t.AcikMi, t.Tur is null)).ToList();
        var satir = talepId is { } id ? viewTalepler.LocateByValue(nameof(TalepSatiri.Id), id) : -1;
        viewTalepler.FocusedRowHandle = satir >= 0 ? satir : _talepler.Count > 0 ? 0 : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        _yukleniyor = false;
        var acik = _talepler.Count(t => t.AcikMi);
        var sinifsiz = _talepler.Count(t => t.AcikMi && t.Tur is null);
        grpListe.Text = $"Talepler – {_talepler.Count} (açık {acik}, sınıflandırılmamış {sinifsiz})";
        await DetayiGosterAsync();
    }

    /// <summary>Seçili talebin ayrıntısı; atanabilir görevliler talebin hedef rolüne göre okunur.</summary>
    private async Task DetayiGosterAsync()
    {
        DetayiGoster();
        cmbGorevli.Properties.Items.Clear();
        if (!_degistirebilir || SeciliTalep is not { AcikMi: true, Tur: not null } talep ||
            talep.Durum is not (TalepDurumu.Yeni or TalepDurumu.Atandi))
            return;

        IReadOnlyList<AtanabilirGorevli> gorevliler = [];
        if (!await Bekleme.CalistirAsync(this, async () => gorevliler = await _talepServisi.AtanabilirGorevlilerAsync(talep.Id)))
            return;
        // Başkasına atamayı yalnızca Koordinatör yapar; diğer roller kendi rolündeki talebi üstlenir.
        foreach (var g in gorevliler.Where(g => _koordinator || g.Id == _oturum.KullaniciId))
            cmbGorevli.Properties.Items.Add(new ImageComboBoxItem($"{g.AdSoyad} ({g.KullaniciAdi}) – {g.Rol.Metin()}", (object)g.Id));
        cmbGorevli.EditValue = gorevliler.Any(g => g.Id == talep.SorumluKullaniciId) && (_koordinator || talep.SorumluKullaniciId == _oturum.KullaniciId)
            ? talep.SorumluKullaniciId
            : cmbGorevli.Properties.Items.Count == 1 ? cmbGorevli.Properties.Items[0].Value : null;
        btnAta.Enabled = cmbGorevli.Properties.Items.Count > 0;
    }

    private void DetayiGoster()
    {
        var t = SeciliTalep;
        if (t is null)
        {
            lblDetay.Text = "Soldan bir talep seçin.";
            grpDetay.Text = "Seçili talep";
            foreach (var b in new[] { btnYzOneri, btnYzOnayla, btnSiniflandir, btnAta, btnIslemeAl, btnTamamla, btnIptal })
                b.Enabled = false;
            cmbTur.Enabled = cmbOncelik.Enabled = cmbGorevli.Enabled = memoSonuc.Enabled = false;
            cmbTur.EditValue = cmbOncelik.EditValue = null;
            memoSonuc.Text = "";
            return;
        }

        grpDetay.Text = $"Seçili talep – #{t.Id}";
        var metin = $"<b>Talep #{t.Id}</b> · {t.Durum.Metin()} · {t.Kaynak.Metin()} · {t.AcilisZamani:dd.MM.yyyy HH:mm}<br>" +
                    $"Aile: {Html(t.AileNo is null ? "—" : $"{t.AileNo} {t.AileAdi}")} · Çadır: {Html(t.CadirKodu ?? "—")}<br>" +
                    $"Tür: <b>{t.TurMetni}</b> · {t.OncelikMetni}" +
                    (t.HedefRol is { } rol ? $" · Yönlendirilen: {rol.Metin()}" : "") + "<br>" +
                    $"Sorumlu: {Html(t.SorumluAdi ?? "—")} · Açan: {Html(t.OlusturanAdi ?? "kiosk")}<br><br>{Html(t.Aciklama)}";
        if (t.YzOnerilenTur is { } yzTur)
            metin += $"<br><br><b>Yapay zeka önerisi:</b> {yzTur.Metin()}" +
                     (yzTur == TalepTuru.Saglik ? " (Sağlık personeli değerlendirecek)" :
                         t.YzOnerilenOncelik is { } yzO ? $", yardım önceliği {yzO.Metin()}" : "") +
                     (string.IsNullOrWhiteSpace(t.YzGerekce) ? "" : $" – {Html(t.YzGerekce)}");
        if (t.Sonuc is not null)
            metin += $"<br><br><b>Sonuç</b> ({t.KapanisZamani:dd.MM.yyyy HH:mm}): {Html(t.Sonuc)}";
        lblDetay.Text = metin;

        var sorumluBen = t.SorumluKullaniciId is { } s && s == _oturum.KullaniciId;
        var ilerletebilir = _koordinator || sorumluBen;
        btnYzOneri.Enabled = t.AcikMi && t.Tur is null;
        btnYzOnayla.Enabled = t.AcikMi && t.SiniflandirmaDurumu == SiniflandirmaDurumu.YapayZekaOnerdi && t.Tur is null && t.YzOnerilenTur is not null;
        cmbTur.Enabled = cmbOncelik.Enabled = btnSiniflandir.Enabled = t.AcikMi && t.Durum != TalepDurumu.Islemde;
        _yukleniyor = true;
        cmbTur.EditValue = t.Tur ?? t.YzOnerilenTur;
        cmbOncelik.EditValue = t.Tur is null ? t.YzOnerilenOncelik : t.YardimOnceligi;
        _yukleniyor = false;
        cmbTur_EditValueChanged(null, EventArgs.Empty);
        cmbGorevli.Enabled = t.AcikMi && t.Tur is not null && t.Durum is TalepDurumu.Yeni or TalepDurumu.Atandi;
        btnAta.Enabled = false;
        btnAta.Text = _koordinator ? "Ata" : "Üstlen";
        btnIslemeAl.Enabled = t.Durum == TalepDurumu.Atandi && ilerletebilir;
        btnTamamla.Enabled = t.Durum == TalepDurumu.Islemde && ilerletebilir;
        btnIptal.Enabled = t.AcikMi;
        memoSonuc.Enabled = t.AcikMi;
        memoSonuc.Text = "";
    }

    /// <summary>
    /// Kullanıcı metnindeki etiket karakterlerini etkisizleştirir. Türkçe harfler olduğu gibi kalır
    /// (DevExpress HTML'i sayısal karakter kodlarını çözmez).
    /// </summary>
    private static string Html(string metin) => metin.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    // ---------------------------------------------------------------- olaylar

    private async void btnYenile_Click(object? sender, EventArgs e)
    {
        await SecenekleriYukleAsync();
        await YenileAsync();
    }

    private async void Filtre_EditValueChanged(object? sender, EventArgs e)
    {
        if (!_yukleniyor)
            await YenileAsync();
    }

    private async void viewTalepler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            await DetayiGosterAsync();
    }

    /// <summary>Sağlık talebi yardım önceliği almaz (şartname M10); seçim kapatılır.</summary>
    private void cmbYeniTur_EditValueChanged(object? sender, EventArgs e) => OncelikKilidi(cmbYeniTur, cmbYeniOncelik, true);

    private void cmbTur_EditValueChanged(object? sender, EventArgs e) =>
        OncelikKilidi(cmbTur, cmbOncelik, SeciliTalep is { AcikMi: true, Durum: not TalepDurumu.Islemde });

    private static void OncelikKilidi(ImageComboBoxEdit tur, ImageComboBoxEdit oncelik, bool duzenlenebilir)
    {
        var saglik = tur.EditValue is TalepTuru.Saglik;
        var turYok = tur.EditValue is null;
        if (saglik || turYok)
            oncelik.EditValue = null;
        oncelik.Enabled = duzenlenebilir && !saglik && !turYok;
    }

    private async void btnOlustur_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(memoAciklama.Text))
        {
            HataYonetimi.Uyari(this, "Talebin açıklamasını yazın.", "Eksik bilgi");
            memoAciklama.Focus();
            return;
        }

        var bilgi = new TalepBilgisi
        {
            Aciklama = memoAciklama.Text, Kaynak = cmbKaynak.EditValue is TalepKaynagi k ? k : TalepKaynagi.KayitMasasi,
            AileId = cmbAile.EditValue as int?, CadirId = cmbCadir.EditValue as int?,
            Tur = cmbYeniTur.EditValue as TalepTuru?, YardimOnceligi = cmbYeniOncelik.EditValue as DestekOnceligi?,
        };
        TalepOzeti? talep = null;
        if (!await Bekleme.CalistirAsync(this, async () => talep = await _talepServisi.OlusturAsync(bilgi)))
            return;

        HataYonetimi.Bilgi(this, talep!.Tur is null
            ? $"Talep #{talep.Id} kaydedildi. Türü seçilmediği için yapay zeka arka planda öneri hazırlayacak; " +
              "beklemeden “Yapay zekadan öneri al” ile isteyebilir veya türü elle seçebilirsiniz."
            : $"Talep #{talep.Id} kaydedildi: {talep.TurMetni}, {talep.OncelikMetni}. Yönlendirilen: {talep.HedefRol?.Metin()}.");
        memoAciklama.Text = "";
        cmbAile.EditValue = cmbCadir.EditValue = cmbYeniTur.EditValue = cmbYeniOncelik.EditValue = null;
        _yukleniyor = true;
        cmbFDurum.EditValue = AcikFiltre;
        cmbFTur.EditValue = null;
        _yukleniyor = false;
        await YenileAsync(talep.Id);
    }

    private async void btnYzOneri_Click(object? sender, EventArgs e)
    {
        if (SeciliTalep is not { } talep)
            return;
        YzOneriSonucu? sonuc = null;
        if (!await Bekleme.CalistirAsync(this, async () => sonuc = await _talepServisi.YapayZekaOnerisiAlAsync(talep.Id)))
            return;

        if (!sonuc!.Basarili)
            HataYonetimi.Uyari(this, $"{sonuc.HataMesaji}\n\nTalep “Sınıflandırılmamış” kaldı; türü ve önceliği elle seçip kaydedebilirsiniz.",
                "Yapay zeka önerisi alınamadı");
        else if (sonuc.Talep.YzOnerilenTur == TalepTuru.Saglik)
            HataYonetimi.Bilgi(this, "Yapay zeka bunu sağlık talebi olarak değerlendirdi. Sağlık talepleri öncelik verilmeden doğrudan " +
                                     "sağlık personeline yönlendirilir; tıbbi değerlendirmeyi sağlık personeli yapar.");
        await YenileAsync(talep.Id);
    }

    private async void btnYzOnayla_Click(object? sender, EventArgs e)
    {
        if (SeciliTalep is { } talep && await Bekleme.CalistirAsync(this, () => _talepServisi.YapayZekaOnerisiniOnaylaAsync(talep.Id)))
            await YenileAsync(talep.Id);
    }

    private async void btnSiniflandir_Click(object? sender, EventArgs e)
    {
        if (SeciliTalep is not { } talep)
            return;
        if (cmbTur.EditValue is not TalepTuru tur)
        {
            HataYonetimi.Uyari(this, "Talep türünü seçin.", "Eksik bilgi");
            cmbTur.Focus();
            return;
        }

        var oncelik = cmbOncelik.EditValue as DestekOnceligi?;
        if (await Bekleme.CalistirAsync(this, () => _talepServisi.SiniflandirAsync(talep.Id, tur, oncelik)))
            await YenileAsync(talep.Id);
    }

    private async void btnAta_Click(object? sender, EventArgs e)
    {
        if (SeciliTalep is not { } talep)
            return;
        if (cmbGorevli.EditValue is not int gorevliId)
        {
            HataYonetimi.Uyari(this, "Sorumlu görevliyi seçin.", "Eksik bilgi");
            cmbGorevli.Focus();
            return;
        }

        if (await Bekleme.CalistirAsync(this, () => _talepServisi.AtaAsync(talep.Id, gorevliId)))
            await YenileAsync(talep.Id);
    }

    private async void btnIslemeAl_Click(object? sender, EventArgs e)
    {
        if (SeciliTalep is { } talep && await Bekleme.CalistirAsync(this, () => _talepServisi.IslemeAlAsync(talep.Id)))
            await YenileAsync(talep.Id);
    }

    private async void btnTamamla_Click(object? sender, EventArgs e) => await KapatAsync(tamamla: true);

    private async void btnIptal_Click(object? sender, EventArgs e) => await KapatAsync(tamamla: false);

    /// <summary>Sonuç yazılmadan talep kapatılamaz (şartname M9); kural Business'ta da denetlenir.</summary>
    private async Task KapatAsync(bool tamamla)
    {
        if (SeciliTalep is not { } talep)
            return;
        if (string.IsNullOrWhiteSpace(memoSonuc.Text))
        {
            HataYonetimi.Uyari(this, tamamla ? "Sonuç yazılmadan talep tamamlanamaz." : "İptal nedeni (sonuç) yazılmadan talep iptal edilemez.",
                "Sonuç zorunlu");
            memoSonuc.Focus();
            return;
        }

        if (!tamamla && !HataYonetimi.Onayla(this, $"Talep #{talep.Id} iptal edilsin mi?", "Talebi iptal et"))
            return;

        var sonuc = memoSonuc.Text;
        if (await Bekleme.CalistirAsync(this, () => tamamla
                ? _talepServisi.TamamlaAsync(talep.Id, sonuc)
                : _talepServisi.IptalEtAsync(talep.Id, sonuc)))
            await YenileAsync(talep.Id);
    }
}
