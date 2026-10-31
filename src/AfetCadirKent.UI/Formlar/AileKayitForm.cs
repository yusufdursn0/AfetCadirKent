using System.Text;
using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Camera;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 4 – Aile ve kişi kayıt (REQ-203): yeni aile ve üyeleri, sonradan gelen yakın, kişi bilgileri ve fotoğraf,
/// mükerrer kontrol uyarıları, ailenin destek önceliği ile malzeme ihtiyaçları (iki ayrı alan), açıklamalı gerekçe
/// değişiklikleri, aileden ayırma ve aileyi pasif yapma. Tüm kurallar Business'tadır; ekran yalnızca girdi alır ve gösterir.
/// </summary>
public partial class AileKayitForm : XtraForm
{
    private sealed record AileSecenegi(int Id, string AileNo, string AileAdi, int UyeSayisi)
    {
        public string Metin => $"{AileNo} – {AileAdi}";
    }

    private sealed record UyeSatiri(int? KisiId, int? BekleyenSira, string KisiNo, string AdSoyad, DateTime DogumTarihi, int Yas,
        string? Yakinlik, string Durum, string OzelDurum);

    private sealed record GerekceSatiri(GerekceKodu Kod, string Ad, string Aciklama, string Oncelik, string Yerlesim, string Kaynak);

    private static readonly string[] Yakinliklar =
        ["Anne", "Baba", "Eş", "Çocuk", "Kardeş", "Büyükanne", "Büyükbaba", "Torun", "Akraba", "Diğer"];

    private readonly IKayitServisi _kayitServisi;
    private readonly IGerekceServisi _gerekceServisi;
    private readonly IOturum _oturum;
    private readonly FormAcici _formAcici;
    private readonly bool _degistirebilir;
    private readonly bool _yerlestirebilir;
    private readonly bool _saglikGorebilir;
    private readonly bool _saglikDegistirebilir;

    private Aile? _aile;
    private AileOncelikSonucu? _oncelik;
    private readonly List<KisiBilgisi> _bekleyenler = [];
    private int? _duzenlenenKisiId;
    private int? _duzenlenenSira;
    private byte[]? _yeniFotograf;
    private bool _programatik;

    public AileKayitForm(IKayitServisi kayitServisi, IGerekceServisi gerekceServisi, IOturum oturum, FormAcici formAcici)
    {
        InitializeComponent();
        _kayitServisi = kayitServisi;
        _gerekceServisi = gerekceServisi;
        _oturum = oturum;
        _formAcici = formAcici;
        _degistirebilir = oturum.IzinVarMi(Modul.Kayit, Izin.Degistirme);
        _yerlestirebilir = oturum.IzinVarMi(Modul.Cadir, Izin.Degistirme);
        _saglikGorebilir = oturum.IzinVarMi(Modul.SaglikBilgisi, Izin.Gorme);
        _saglikDegistirebilir = oturum.IzinVarMi(Modul.SaglikBilgisi, Izin.Degistirme);

        lueAile.Properties.DisplayMember = nameof(AileSecenegi.Metin);
        lueAile.Properties.ValueMember = nameof(AileSecenegi.Id);
        lueAile.Properties.Columns.Add(new LookUpColumnInfo(nameof(AileSecenegi.AileNo), "Aile no", 70));
        lueAile.Properties.Columns.Add(new LookUpColumnInfo(nameof(AileSecenegi.AileAdi), "Aile adı", 200));
        lueAile.Properties.Columns.Add(new LookUpColumnInfo(nameof(AileSecenegi.UyeSayisi), "Kişi", 40));
        lueAile.Properties.ShowHeader = true;

        cmbCinsiyet.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        foreach (var cinsiyet in Enum.GetValues<Cinsiyet>())
            cmbCinsiyet.Properties.Items.Add(new ImageComboBoxItem(cinsiyet.Metin(), cinsiyet));
        cmbYakinlik.Properties.Items.AddRange(Yakinliklar);
        dteDogum.Properties.MinValue = new DateTime(1900, 1, 1);
        dteDogum.Properties.MaxValue = DateTime.Today;

        lblFotoBilgi.Text = "<color=gray>Fotoğraf isteğe bağlıdır. Kamerayla çekilen fotoğraf ortadan kare kırpılıp 300×300 " +
                            "boyutuna küçültülerek saklanır. Fotoğraf tek başına kimlik doğrulaması sayılmaz.</color>";
        GridYardimcisi.BosMetinGoster(viewUyeler, "Henüz üye yok. Sağdaki kişi bilgilerini doldurup ekleyin.");
        GridYardimcisi.BosMetinGoster(viewGerekceler, "Aile kaydedilince gerekçeler hesaplanır.");

        YetkileriUygula();
        YeniAileDurumu();
    }

    /// <summary>Boş ekranla yeni aile kaydına başlar (kişi listesinden "Yeni aile kaydı").</summary>
    public void YeniAile()
    {
        if (!_degistirebilir)
            return;
        _programatik = true;
        lueAile.EditValue = null;
        _programatik = false;
        YeniAileDurumu();
    }

    /// <summary>Kayıtlı aileyi açar (kişi listesinden "Aileyi aç").</summary>
    public async void AileyiAc(int aileId) => await AileyiYukleAsync(aileId);

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await AileListesiniYukleAsync();
    }

    // ---------------------------------------------------------------- Yükleme ve gösterim

    private void YetkileriUygula()
    {
        var ogeler = new[]
        {
            lciYeniAile, lciAilePasif, lciAileKaydet, lciKisiYeni, lciUyeCikar, lciAiledenAyir, lciKamera,
            lciKisiKaydet, lciKisiTemizle, lciGerekceEkle, lciGerekceKaldir, lciGerekceGeriAl,
        };
        foreach (var oge in ogeler.Where(_ => !_degistirebilir))
            oge.Visibility = LayoutVisibility.Never;

        foreach (var editor in new BaseEdit[]
                 {
                     txtAileAdi, chkTekEbeveyn, txtNotlar, txtAd, txtSoyad, dteDogum, chkTahmini, txtTc, cmbCinsiyet,
                     cmbYakinlik, txtMemleket, txtTelefon, chkHamile, chkTekerlekli, chkKronik, chkRefakatsiz,
                 })
            editor.Properties.ReadOnly = !_degistirebilir;

        lciCadiraYerlestir.Visibility = _yerlestirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        lciSaglik.Visibility = _saglikGorebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        memoSaglik.Properties.ReadOnly = !_saglikDegistirebilir;
        if (!_saglikDegistirebilir)
            memoSaglik.Properties.NullValuePrompt = "Sağlık notunu sağlık bilgisi yetkisi olan görevli girer.";
    }

    private async Task AileListesiniYukleAsync()
    {
        IReadOnlyList<Aile> aileler = [];
        if (!await Bekleme.CalistirAsync(lueAile, async () => aileler = await _kayitServisi.AileleriListeleAsync()))
            return;

        _programatik = true;
        lueAile.Properties.DataSource = aileler
            .Select(a => new AileSecenegi(a.Id, a.AileNo, a.AileAdi, a.Uyeler.Count(u => u.CadirKenttemi))).ToList();
        lueAile.EditValue = _aile?.Id;
        _programatik = false;
    }

    private void YeniAileDurumu()
    {
        _aile = null;
        _oncelik = null;
        _bekleyenler.Clear();
        lblAileNo.Text = "<b>Yeni aile</b> – kaydedilince sistem benzersiz aile ve kişi numarası verir.";
        txtAileAdi.EditValue = null;
        chkTekEbeveyn.Checked = false;
        txtNotlar.EditValue = null;
        btnAileKaydet.Text = "Aileyi ve üyeleri kaydet";
        btnAilePasif.Enabled = false;
        btnCadiraYerlestir.Enabled = false;
        lciUyeCikar.Visibility = _degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        lciAiledenAyir.Visibility = LayoutVisibility.Never;
        UyeleriGoster();
        GerekceleriGoster();
        KisiEditorunuTemizle();
    }

    private async Task AileyiYukleAsync(int aileId)
    {
        Aile? aile = null;
        AileOncelikSonucu? oncelik = null;
        var basarili = await Bekleme.CalistirAsync(this, async () =>
        {
            aile = await _kayitServisi.AileGetirAsync(aileId);
            oncelik = await _gerekceServisi.DegerlendirAsync(aileId);
        });
        if (!basarili || aile is null)
            return;

        _aile = aile;
        _oncelik = oncelik;
        _bekleyenler.Clear();
        _programatik = true;
        lueAile.EditValue = aile.Id;
        _programatik = false;

        var buradakiler = aile.CadirKenttekiUyeler.Count();
        lblAileNo.Text = $"<b>{aile.AileNo}</b> · kayıt {aile.KayitZamani:dd.MM.yyyy} · çadır kentte {buradakiler} kişi" +
                         (aile.Aktif ? "" : " · <color=red>pasif aile</color>");
        txtAileAdi.EditValue = aile.AileAdi;
        chkTekEbeveyn.Checked = aile.TekEbeveyn;
        txtNotlar.EditValue = aile.Notlar;
        btnAileKaydet.Text = "Aile bilgilerini kaydet";
        btnAilePasif.Enabled = aile.Aktif;
        btnCadiraYerlestir.Enabled = aile.Aktif && buradakiler > 0;
        lciUyeCikar.Visibility = LayoutVisibility.Never;
        lciAiledenAyir.Visibility = _degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        UyeleriGoster();
        GerekceleriGoster();
        KisiEditorunuTemizle();
    }

    private void UyeleriGoster()
    {
        var bugun = DateOnly.FromDateTime(DateTime.Today);
        List<UyeSatiri> satirlar;
        if (_aile is { } aile)
        {
            satirlar = aile.Uyeler.OrderBy(u => u.DogumTarihi).Select(u => new UyeSatiri(u.Id, null, u.KisiNo, u.AdSoyad,
                u.DogumTarihi.ToDateTime(TimeOnly.MinValue), u.Yas(bugun), u.Yakinlik,
                u.Aktif ? u.Durum.Metin() : "Pasif", KisiListesiForm.OzelDurumlar(u))).ToList();
        }
        else
        {
            satirlar = _bekleyenler.Select((b, i) => new UyeSatiri(null, i, "(yeni)", $"{b.Ad} {b.Soyad}",
                b.DogumTarihi.ToDateTime(TimeOnly.MinValue), Afetzede.YasHesapla(b.DogumTarihi, bugun),
                b.Yakinlik, "Kaydedilmedi", OzelDurumlar(b))).ToList();
        }

        _programatik = true;
        gridUyeler.DataSource = satirlar;
        viewUyeler.BestFitColumns();
        viewUyeler.ClearSelection();
        viewUyeler.FocusedRowHandle = GridControl.InvalidRowHandle;
        _programatik = false;
    }

    private static string OzelDurumlar(KisiBilgisi b)
    {
        var liste = new List<string>();
        if (b.Hamile) liste.Add("Hamile");
        if (b.TekerlekliSandalye) liste.Add("Tekerlekli sandalye");
        if (b.KronikHasta) liste.Add("Kronik hasta");
        if (b.RefakatsizCocuk) liste.Add("Refakatsiz çocuk");
        return string.Join(", ", liste);
    }

    /// <summary>Şartname M3: iki ayrı alan – destek önceliği (gerekçeleriyle) ve malzemeye özel ihtiyaçlar.</summary>
    private void GerekceleriGoster()
    {
        var kayitli = _oncelik is not null && _aile is { Aktif: true };
        foreach (var dugme in new[] { btnGerekceEkle, btnGerekceKaldir, btnGerekceGeriAl })
            dugme.Enabled = kayitli;

        if (_oncelik is not { } oncelik)
        {
            lblDestek.Text = "<b>Destek önceliği:</b> <color=gray>aile kaydedilince hesaplanır</color>";
            lblMalzeme.Text = "<b>Malzeme ihtiyaçları:</b> <color=gray>aile kaydedilince hesaplanır</color>";
            gridGerekceler.DataSource = null;
            return;
        }

        var renk = oncelik.DestekOnceligi switch
        {
            DestekOnceligi.Yuksek => "red",
            DestekOnceligi.Orta => "darkorange",
            _ => "green",
        };
        var gerekceler = string.Join(", ", oncelik.Gerekceler.Select(g => g.Aciklama));
        lblDestek.Text = $"<b>Destek önceliği:</b> <b><color={renk}>{oncelik.DestekOnceligi.Metin()}</color></b> ({gerekceler})";

        var oncelikliler = oncelik.MalzemeIhtiyaclari.Where(m => m.Oncelikli).Select(m => $"{m.UrunAdi} ✔").ToList();
        lblMalzeme.Text = "<b>Malzeme ihtiyaçları:</b> " + (oncelikliler.Count == 0 ? "— (özel malzeme önceliği yok)" : string.Join(" · ", oncelikliler));

        gridGerekceler.DataSource = oncelik.Gerekceler.Select(g => new GerekceSatiri(g.Kod, g.Ad, g.Aciklama, g.DestekOnceligi.Metin(),
            g.YerlesimEtkisi.Metin(), g.ElleEklendi ? "Elle eklendi" : "Otomatik")).ToList();
        viewGerekceler.BestFitColumns();
    }

    // ---------------------------------------------------------------- Kişi düzenleyici

    private void KisiEditorunuTemizle()
    {
        _duzenlenenKisiId = null;
        _duzenlenenSira = null;
        _yeniFotograf = null;
        foreach (var editor in new BaseEdit[] { txtAd, txtSoyad, dteDogum, txtTc, cmbYakinlik, txtMemleket, txtTelefon, memoSaglik })
            editor.EditValue = null;
        foreach (var kutu in new[] { chkTahmini, chkHamile, chkTekerlekli, chkKronik, chkRefakatsiz })
            kutu.Checked = false;
        cmbCinsiyet.EditValue = Cinsiyet.Belirtilmemis;
        FotografiGoster(null);
        btnKisiKaydet.Text = _aile is null ? "Listeye ekle" : "Aileye ekle";
        btnKisiKaydet.Enabled = _aile is null or { Aktif: true };
        grpKisi.Text = _aile is null ? "Kişi bilgileri – yeni üye" : "Kişi bilgileri – aileye yeni kişi (sonradan gelen yakın)";
    }

    private void FotografiGoster(byte[]? baytlar)
    {
        var eski = picFoto.Image;
        picFoto.Image = FotografIslemleri.Goruntu(baytlar);
        eski?.Dispose();
    }

    private void KisiyiEditoreAl(UyeSatiri satir)
    {
        KisiEditorunuTemizle();
        if (satir.KisiId is { } kisiId && _aile?.Uyeler.FirstOrDefault(u => u.Id == kisiId) is { } k)
        {
            _duzenlenenKisiId = kisiId;
            Doldur(k.Ad, k.Soyad, k.DogumTarihi, k.DogumTarihiTahmini, k.TcKimlikNo, k.Cinsiyet, k.Yakinlik, k.Memleket, k.Telefon,
                k.Hamile, k.TekerlekliSandalye, k.KronikHasta, k.RefakatsizCocuk, k.SaglikNotu);
            FotografiGoster(k.Fotograf);
            grpKisi.Text = $"Kişi bilgileri – {k.KisiNo} {k.AdSoyad}";
            btnKisiKaydet.Text = "Değişiklikleri kaydet";
            btnKisiKaydet.Enabled = k.Aktif;
        }
        else if (satir.BekleyenSira is { } sira && sira < _bekleyenler.Count)
        {
            var b = _bekleyenler[sira];
            _duzenlenenSira = sira;
            Doldur(b.Ad, b.Soyad, b.DogumTarihi, b.DogumTarihiTahmini, b.TcKimlikNo, b.Cinsiyet, b.Yakinlik, b.Memleket, b.Telefon,
                b.Hamile, b.TekerlekliSandalye, b.KronikHasta, b.RefakatsizCocuk, b.SaglikNotu);
            _yeniFotograf = b.Fotograf;
            FotografiGoster(b.Fotograf);
            grpKisi.Text = $"Kişi bilgileri – {b.Ad} {b.Soyad} (kaydedilmedi)";
            btnKisiKaydet.Text = "Listede güncelle";
        }
    }

    private void Doldur(string ad, string soyad, DateOnly dogum, bool tahmini, string? tc, Cinsiyet cinsiyet, string? yakinlik,
        string? memleket, string? telefon, bool hamile, bool tekerlekli, bool kronik, bool refakatsiz, string? saglikNotu)
    {
        txtAd.EditValue = ad;
        txtSoyad.EditValue = soyad;
        dteDogum.DateTime = dogum.ToDateTime(TimeOnly.MinValue);
        chkTahmini.Checked = tahmini;
        txtTc.EditValue = tc;
        cmbCinsiyet.EditValue = cinsiyet;
        cmbYakinlik.EditValue = yakinlik;
        txtMemleket.EditValue = memleket;
        txtTelefon.EditValue = telefon;
        chkHamile.Checked = hamile;
        chkTekerlekli.Checked = tekerlekli;
        chkKronik.Checked = kronik;
        chkRefakatsiz.Checked = refakatsiz;
        memoSaglik.EditValue = saglikNotu;
    }

    /// <summary>Ekrandaki zorunlu alanları denetler ve kişi bilgisini üretir; eksik varsa kullanıcıyı uyarır.</summary>
    private KisiBilgisi? KisiBilgisiOku()
    {
        (BaseEdit Editor, string Ad)? eksik =
            string.IsNullOrWhiteSpace(txtAd.Text) ? (txtAd, "Ad")
            : string.IsNullOrWhiteSpace(txtSoyad.Text) ? (txtSoyad, "Soyad")
            : dteDogum.EditValue is null ? (dteDogum, "Doğum tarihi (bilinmiyorsa yaklaşık tarih girip 'tahmini' işaretleyin)")
            : null;
        if (eksik is { } e)
        {
            HataYonetimi.Uyari(this, $"{e.Ad} boş bırakılamaz.", "Eksik bilgi");
            e.Editor.Focus();
            return null;
        }

        return new KisiBilgisi
        {
            Ad = txtAd.Text.Trim(),
            Soyad = txtSoyad.Text.Trim(),
            DogumTarihi = DateOnly.FromDateTime(dteDogum.DateTime),
            DogumTarihiTahmini = chkTahmini.Checked,
            TcKimlikNo = BosIseNull(txtTc.Text),
            Cinsiyet = cmbCinsiyet.EditValue as Cinsiyet? ?? Cinsiyet.Belirtilmemis,
            Yakinlik = BosIseNull(cmbYakinlik.Text),
            Memleket = BosIseNull(txtMemleket.Text),
            Telefon = BosIseNull(txtTelefon.Text),
            Hamile = chkHamile.Checked,
            TekerlekliSandalye = chkTekerlekli.Checked,
            KronikHasta = chkKronik.Checked,
            RefakatsizCocuk = chkRefakatsiz.Checked,
            SaglikNotu = _saglikDegistirebilir ? BosIseNull(memoSaglik.Text) : null,
            Fotograf = _yeniFotograf,
        };
    }

    private static string? BosIseNull(string? metin) => string.IsNullOrWhiteSpace(metin) ? null : metin.Trim();

    /// <summary>
    /// Mükerrer kontrol (şartname M2): aynı TC varsa kayıt engellenir; TC yoksa ad-soyadı aynı ve yaşı yakın kişiler gösterilir,
    /// görevli karar verir. Devam edilebilirse true.
    /// </summary>
    private async Task<bool> MukerrerKontroluAsync(KisiBilgisi bilgi)
    {
        if (bilgi.TcKimlikNo is { } tc && _bekleyenler.Where((_, i) => i != _duzenlenenSira).Any(b => b.TcKimlikNo == tc))
        {
            HataYonetimi.Uyari(this, $"{tc} TC kimlik numarası bu ailede başka bir kişiye girilmiş.", "Mükerrer kayıt");
            return false;
        }

        MukerrerSonucu? sonuc = null;
        if (!await Bekleme.CalistirAsync(this, async () => sonuc = await _kayitServisi.MukerrerKontrolAsync(bilgi, _duzenlenenKisiId)) ||
            sonuc is null)
            return false;

        if (sonuc.TcSahibi is { } sahip)
        {
            HataYonetimi.Uyari(this, $"{sahip.TcKimlikNo} TC kimlik numarasıyla kayıtlı bir kişi var: {sahip.KisiNo} {sahip.AdSoyad}." +
                                     "\n\nAynı kişi ikinci kez kaydedilemez. Kişi listesinden mevcut kaydı açın.", "Mükerrer kayıt");
            return false;
        }

        if (!sonuc.UyariVar)
            return true;

        var liste = new StringBuilder();
        foreach (var k in sonuc.OlasiEslesmeler.Take(10))
            liste.AppendLine($"• {k.KisiNo} {k.AdSoyad} – doğum {k.DogumTarihi:dd.MM.yyyy}{(k.Aktif ? "" : " (pasif)")}");
        return HataYonetimi.Onayla(this,
            "Bu kişiye benzeyen kayıtlar var (ad-soyad aynı, yaş farkı 2'den az; Türkçe karakter farkları yok sayıldı):\n\n" +
            liste + "\nAynı kişi değilse kayda devam edebilirsiniz. Devam edilsin mi?", "Olası mükerrer kayıt");
    }

    // ---------------------------------------------------------------- Olaylar

    private async void lueAile_EditValueChanged(object? sender, EventArgs e)
    {
        if (_programatik)
            return;
        if (lueAile.EditValue is int aileId)
            await AileyiYukleAsync(aileId);
    }

    private void btnYeniAile_Click(object? sender, EventArgs e) => YeniAile();

    private void viewUyeler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (_programatik || viewUyeler.GetRow(e.FocusedRowHandle) is not UyeSatiri satir)
            return;
        KisiyiEditoreAl(satir);
    }

    private void btnKisiYeni_Click(object? sender, EventArgs e)
    {
        _programatik = true;
        viewUyeler.FocusedRowHandle = GridControl.InvalidRowHandle;
        _programatik = false;
        KisiEditorunuTemizle();
        txtAd.Focus();
    }

    private void btnKisiTemizle_Click(object? sender, EventArgs e) => btnKisiYeni_Click(sender, e);

    private void btnKamera_Click(object? sender, EventArgs e)
    {
        try
        {
            var diyalog = new TakePictureDialog { Caption = "Kamerayla fotoğraf çek" };
            if (diyalog.ShowDialog() != DialogResult.OK || diyalog.Image is not { } goruntu)
                return;
            using (goruntu)
                _yeniFotograf = FotografIslemleri.KareyeKucult(goruntu);
            FotografiGoster(_yeniFotograf);
        }
        catch (Exception ex)
        {
            HataYonetimi.Goster(this, ex);
        }
    }

    private async void btnKisiKaydet_Click(object? sender, EventArgs e)
    {
        if (KisiBilgisiOku() is not { } bilgi || !await MukerrerKontroluAsync(bilgi))
            return;

        if (_aile is null)
        {
            if (_duzenlenenSira is { } sira)
                _bekleyenler[sira] = bilgi;
            else
                _bekleyenler.Add(bilgi);
            if (string.IsNullOrWhiteSpace(txtAileAdi.Text))
                txtAileAdi.EditValue = $"{bilgi.Soyad} Ailesi";
            UyeleriGoster();
            KisiEditorunuTemizle();
            txtAd.Focus();
            return;
        }

        var aileId = _aile.Id;
        bool basarili;
        if (_duzenlenenKisiId is { } kisiId)
        {
            var eskiNot = _aile.Uyeler.First(u => u.Id == kisiId).SaglikNotu;
            basarili = await Bekleme.CalistirAsync(this, async () =>
            {
                await _kayitServisi.KisiGuncelleAsync(kisiId, bilgi);
                if (_saglikDegistirebilir && bilgi.SaglikNotu != eskiNot)
                    await _kayitServisi.SaglikBilgisiGuncelleAsync(kisiId, bilgi.KronikHasta, bilgi.Hamile, bilgi.SaglikNotu);
            });
        }
        else
        {
            basarili = await Bekleme.CalistirAsync(this, () => _kayitServisi.KisiEkleAsync(aileId, bilgi));
        }

        if (basarili)
            await KaydedilenAileyiAcAsync(aileId);
    }

    private void btnUyeCikar_Click(object? sender, EventArgs e)
    {
        var siralar = viewUyeler.GetSelectedRows().Select(viewUyeler.GetRow).OfType<UyeSatiri>()
            .Select(s => s.BekleyenSira).OfType<int>().OrderDescending().ToList();
        if (siralar.Count == 0 && _duzenlenenSira is { } duzenlenen)
            siralar.Add(duzenlenen);
        if (siralar.Count == 0)
        {
            HataYonetimi.Uyari(this, "Listeden çıkarılacak kişiyi seçin (satırdaki kutuyu işaretleyin).");
            return;
        }

        foreach (var sira in siralar)
            _bekleyenler.RemoveAt(sira);
        UyeleriGoster();
        KisiEditorunuTemizle();
    }

    private async void btnAileKaydet_Click(object? sender, EventArgs e)
    {
        var aileAdi = txtAileAdi.Text;
        var notlar = BosIseNull(txtNotlar.Text);
        var tekEbeveyn = chkTekEbeveyn.Checked;

        if (_aile is { } mevcut)
        {
            if (await Bekleme.CalistirAsync(this, () => _kayitServisi.AileGuncelleAsync(mevcut.Id, aileAdi, tekEbeveyn, notlar)))
                await KaydedilenAileyiAcAsync(mevcut.Id);
            return;
        }

        if (_bekleyenler.Count == 0)
        {
            HataYonetimi.Uyari(this, "Aileye en az bir kişi ekleyin: sağdaki kişi bilgilerini doldurup 'Listeye ekle'ye basın.", "Eksik bilgi");
            txtAd.Focus();
            return;
        }

        Aile? yeni = null;
        if (!await Bekleme.CalistirAsync(this, async () =>
                yeni = await _kayitServisi.AileOlusturAsync(aileAdi, tekEbeveyn, _bekleyenler.ToList(), notlar)) || yeni is null)
            return;

        HataYonetimi.Bilgi(this, $"Aile kaydedildi: {yeni.AileNo} – {yeni.AileAdi}\n\n" +
                                 $"Kişi numaraları: {string.Join(", ", yeni.Uyeler.Select(u => $"{u.KisiNo} {u.AdSoyad}"))}",
            "Kayıt tamamlandı");
        await KaydedilenAileyiAcAsync(yeni.Id);
    }

    /// <summary>Kayıttan sonra aile listesini ve aileyi yeniden yükler. Aile kullanıcıya gizliyse (refakatsiz çocuk) boş ekrana döner.</summary>
    private async Task KaydedilenAileyiAcAsync(int aileId)
    {
        await AileListesiniYukleAsync();
        try
        {
            _aile = await _kayitServisi.AileGetirAsync(aileId);
        }
        catch (KayitBulunamadiException)
        {
            HataYonetimi.Bilgi(this, "Kayıt tamamlandı. Refakatsiz çocuk kayıtlarını yalnızca Koordinatör görür ve takip eder.");
            YeniAileDurumu();
            return;
        }
        catch (Exception ex)
        {
            HataYonetimi.Goster(this, ex);
            return;
        }

        await AileyiYukleAsync(aileId);
    }

    private async void btnAiledenAyir_Click(object? sender, EventArgs e)
    {
        if (_aile is not { } aile)
            return;
        var secilenler = viewUyeler.GetSelectedRows().Select(viewUyeler.GetRow).OfType<UyeSatiri>()
            .Where(s => s.KisiId is not null).ToList();
        if (secilenler.Count == 0)
        {
            HataYonetimi.Uyari(this, "Yeni aileye geçecek kişileri satırlardaki kutuları işaretleyerek seçin.", "Aileden ayırma");
            return;
        }

        var ilkSoyad = aile.Uyeler.First(u => u.Id == secilenler[0].KisiId).Soyad;
        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Aileden ayır",
            Mesaj = $"<b>{string.Join(", ", secilenler.Select(s => s.AdSoyad))}</b> {aile.AileNo} ailesinden ayrılıp yeni aile olacak. " +
                    "Yeni aile kendi numarasını alır, eski aileyle bağlantısı geçmişte kalır; gerekçeler ve dağıtım hakları iki aile için yeniden hesaplanır.",
            MetinEtiketi = "Yeni aile adı",
            VarsayilanMetin = $"{ilkSoyad} Ailesi",
            AciklamaEtiketi = "Ayırma gerekçesi",
            TamamMetni = "Ayır",
        });
        if (sonuc is not { Metin: { } yeniAd, Aciklama: { } gerekce })
            return;

        Aile? yeni = null;
        var idler = secilenler.Select(s => s.KisiId!.Value).ToList();
        if (!await Bekleme.CalistirAsync(this, async () => yeni = await _kayitServisi.AiledenAyirAsync(aile.Id, idler, yeniAd, gerekce)) ||
            yeni is null)
            return;

        var yeniAc = HataYonetimi.Onayla(this, $"Yeni aile oluşturuldu: {yeni.AileNo} – {yeni.AileAdi}.\n\nYeni aileyi açmak ister misiniz?",
            "Aileden ayırma tamamlandı");
        await KaydedilenAileyiAcAsync(yeniAc ? yeni.Id : aile.Id);
    }

    /// <summary>Ana akış: aile kaydı → uygun çadır. Çadır planı ekranı bu aileyle açılır ve öneri getirilir.</summary>
    private void btnCadiraYerlestir_Click(object? sender, EventArgs e)
    {
        if (_aile is { } aile)
            _formAcici.EkranAc<CadirPlaniForm>(Owner ?? this).AileyiYerlestir(aile.Id);
    }

    private async void btnAilePasif_Click(object? sender, EventArgs e)
    {
        if (_aile is not { } aile)
            return;
        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Aileyi pasif yap",
            Mesaj = $"<b>{aile.AileNo} – {aile.AileAdi}</b> pasif yapılacak. Aile ancak çadır kentte üyesi kalmadıysa pasif yapılabilir; " +
                    "kayıt silinmez, geçmişiyle korunur ve <b>tekrar aktif yapılamaz</b>. Bekleme listesindeyse listeden çıkar.",
            TamamMetni = "Pasif yap",
        });
        if (sonuc?.Aciklama is not { } aciklama)
            return;

        if (!await Bekleme.CalistirAsync(this, () => _kayitServisi.AilePasifYapAsync(aile.Id, aciklama)))
            return;
        HataYonetimi.Bilgi(this, $"{aile.AileNo} pasif yapıldı.");
        YeniAile();
        await AileListesiniYukleAsync();
    }

    private async void btnGerekceEkle_Click(object? sender, EventArgs e)
    {
        if (_aile is not { } aile || _oncelik is not { } oncelik)
            return;
        var mevcut = oncelik.Gerekceler.Select(g => g.Kod).ToHashSet();
        var secenekler = GerekceDegerlendirici.TumKurallar()
            .Where(k => k.Kod != GerekceKodu.OzelDurumYok && !mevcut.Contains(k.Kod))
            .Select(k => ((object)k.Kod, $"{k.Ad} (destek önceliği: {k.DestekOnceligi.Metin()})")).ToList();

        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Gerekçe ekle",
            Mesaj = $"<b>{aile.AileNo}</b> için sistemin bulmadığı bir gerekçeyi elle ekleyin. Değişiklik açıklamasıyla kaydedilir.",
            SecimEtiketi = "Gerekçe",
            Secenekler = secenekler,
            TamamMetni = "Ekle",
        });
        if (sonuc is not { Secim: GerekceKodu kod, Aciklama: { } aciklama })
            return;

        if (await Bekleme.CalistirAsync(this, () => _gerekceServisi.ElleDegistirAsync(aile.Id, kod, GerekceDegisiklikTuru.ElleEklendi, aciklama)))
            await AileyiYukleAsync(aile.Id);
    }

    private async void btnGerekceKaldir_Click(object? sender, EventArgs e)
    {
        if (_aile is not { } aile)
            return;
        if (viewGerekceler.GetFocusedRow() is not GerekceSatiri satir)
        {
            HataYonetimi.Uyari(this, "Kaldırılacak gerekçeyi listeden seçin.");
            return;
        }

        if (satir.Kod == GerekceKodu.OzelDurumYok)
        {
            HataYonetimi.Uyari(this, "'Özel durumu yok' gerekçesi kaldırılamaz; başka gerekçe yoksa kendiliğinden görünür.");
            return;
        }

        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Gerekçeyi kaldır",
            Mesaj = $"<b>{satir.Ad}</b> gerekçesi {aile.AileNo} için kaldırılacak. Değişiklik açıklamasıyla kaydedilir ve geri alınabilir.",
            TamamMetni = "Kaldır",
        });
        if (sonuc?.Aciklama is not { } aciklama)
            return;

        if (await Bekleme.CalistirAsync(this, () => _gerekceServisi.ElleDegistirAsync(aile.Id, satir.Kod, GerekceDegisiklikTuru.ElleKaldirildi, aciklama)))
            await AileyiYukleAsync(aile.Id);
    }

    private async void btnGerekceGeriAl_Click(object? sender, EventArgs e)
    {
        if (_aile is not { } aile)
            return;
        IReadOnlyList<AileGerekcesi> gecmis = [];
        if (!await Bekleme.CalistirAsync(this, async () => gecmis = await _gerekceServisi.DegisiklikGecmisiAsync(aile.Id)))
            return;

        var gecerliler = gecmis.Where(g => g.Gecerli).GroupBy(g => g.GerekceId).Select(g => g.First()).ToList();
        if (gecerliler.Count == 0)
        {
            HataYonetimi.Bilgi(this, "Bu ailede geri alınacak elle gerekçe değişikliği yok.");
            return;
        }

        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Elle değişikliği geri al",
            Mesaj = "Seçilen gerekçe yeniden sistemin otomatik hesabına bırakılır.",
            SecimEtiketi = "Değişiklik",
            Secenekler = gecerliler.Select(g => ((object)(GerekceKodu)g.GerekceId,
                $"{g.Gerekce?.Ad} – {(g.Tur == GerekceDegisiklikTuru.ElleEklendi ? "elle eklendi" : "elle kaldırıldı")} ({g.Zaman:dd.MM.yyyy})")).ToList(),
            TamamMetni = "Geri al",
        });
        if (sonuc is not { Secim: GerekceKodu kod, Aciklama: { } aciklama })
            return;

        if (await Bekleme.CalistirAsync(this, () => _gerekceServisi.ElleDegisikligiGeriAlAsync(aile.Id, kod, aciklama)))
            await AileyiYukleAsync(aile.Id);
    }
}
