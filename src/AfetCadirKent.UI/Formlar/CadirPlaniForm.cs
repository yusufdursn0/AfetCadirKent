using System.Text;
using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Cadirlar;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 6 – Çadır planı (REQ-078, REQ-205): renkli, tıklanabilir plan (TileControl); seçili çadırdaki kişiler ve çadır
/// değişikliği; yer açılan çadıra bekleme listesinden aile önerisi; aile için çadır önerisi ve görevli onayı; yer yoksa
/// bekleme listesine alma. "Tanımlar" sekmesinde bölge ve çadır ekle/güncelle/sil/pasif yap (REQ-180).
/// </summary>
public partial class CadirPlaniForm : XtraForm
{
    private sealed record CadirdakiSatir(int Id, string KisiNo, string AdSoyad, int Yas, string Durum, string Aile, DateTime Baslangic);

    private sealed record AileSecenegi(int AileId, string Metin, bool BeklemeListesinde);

    private sealed record BolgeSatiri(int Id, string Kod, string Ad, string? Aciklama, int CadirSayisi, string KayitDurumu, Bolge Bolge);

    private sealed record CadirTanimSatiri(int Id, string Kod, string Bolge, int Kapasite, int Doluluk, string Durum, bool Erisilebilir,
        bool SaglikNoktasinaYakin, bool Isitmali, string KayitDurumu, CadirPlanOgesi Oge);

    // Plan renkleri: boş yeşil, kısmen dolu sarı, dolu kırmızı, kullanılamaz gri (lejant metniyle aynı).
    private static readonly Dictionary<PlanDurumu, Color> PlanRenkleri = new()
    {
        [PlanDurumu.Bos] = Color.FromArgb(46, 125, 50),
        [PlanDurumu.KismenDolu] = Color.FromArgb(230, 145, 20),
        [PlanDurumu.Dolu] = Color.FromArgb(198, 40, 40),
        [PlanDurumu.Kullanilamaz] = Color.FromArgb(117, 117, 117),
    };

    private readonly ICadirServisi _cadirServisi;
    private readonly IBeklemeListesiServisi _beklemeServisi;
    private readonly bool _degistirebilir;
    private List<CadirPlanOgesi> _plan = [];
    private List<Bolge> _bolgeler = [];
    private int? _seciliCadirId;
    private BekleyenAile? _cadiraOnerilenAile;
    private int _secimSayaci;
    private bool _yeniBolge = true;
    private bool _yeniCadir = true;
    private bool _yuklendi;
    private int? _istenenAileId;

    public CadirPlaniForm(ICadirServisi cadirServisi, IBeklemeListesiServisi beklemeServisi, IOturum oturum)
    {
        InitializeComponent();
        _cadirServisi = cadirServisi;
        _beklemeServisi = beklemeServisi;
        _degistirebilir = oturum.IzinVarMi(Modul.Cadir, Izin.Degistirme);
        oneriPaneli.Baglan(cadirServisi, beklemeServisi, oturum);

        cmbBolge.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbCadirBolge.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbCadirDurum.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        foreach (var durum in Enum.GetValues<CadirDurumu>())
            cmbCadirDurum.Properties.Items.Add(new ImageComboBoxItem(durum.Metin(), durum));

        lblLejant.Text = $"<color={Html(PlanDurumu.Bos)}>■ Boş</color>   <color={Html(PlanDurumu.KismenDolu)}>■ Kısmen dolu</color>   " +
                         $"<color={Html(PlanDurumu.Dolu)}>■ Dolu</color>   <color={Html(PlanDurumu.Kullanilamaz)}>■ Kullanılamaz</color>   " +
                         "· E: erişilebilir · S: sağlık noktasına yakın · I: ısıtmalı (sıcak çadır)";

        GridYardimcisi.BosMetinGoster(viewCadirdakiler, "Çadır seçildiğinde içindeki kişiler burada görünür.");
        GridYardimcisi.BosMetinGoster(viewBolgeler, "Henüz bölge tanımlanmamış.");
        GridYardimcisi.BosMetinGoster(viewCadirTanim, "Henüz çadır tanımlanmamış.");

        foreach (var oge in new[]
                 {
                     lciTasi, lciBekleyeniYerlestir, lciBolgeKod, lciBolgeAd, lciBolgeAciklama, lciBolgeYeni, lciBolgeKaydet,
                     lciBolgeSil, lciBolgeAktiflik, lciCadirBolge, lciSira, lciNumara, lciKapasite, lciCadirDurum, lciErisilebilir,
                     lciSaglik, lciIsitmali, lciCadirYeni, lciCadirKaydet, lciCadirSil, lciCadirAktiflik,
                 })
            oge.Visibility = _degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        viewCadirdakiler.OptionsSelection.MultiSelect = _degistirebilir;

        oneriPaneli.KayitDegisti += async (_, _) => await YenileAsync(paneliKoru: true);
        CadirSecimiGoster(null);
        YeniBolgeDurumu();
        YeniCadirDurumu();
    }

    private static string Html(PlanDurumu durum) => ColorTranslator.ToHtml(PlanRenkleri[durum]);

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await YenileAsync();
        _yuklendi = true;
        await IstenenAileyiAcAsync();
    }

    /// <summary>
    /// Başka bir ekrandan (ör. aile kaydı) aileyi yerleştirmek için açar: aile seçilir ve çadır önerisi getirilir.
    /// Ekran ilk kez açılıyorsa öneri, ilk yükleme bitince getirilir.
    /// </summary>
    public async void AileyiYerlestir(int aileId)
    {
        _istenenAileId = aileId;
        if (_yuklendi)
        {
            await AileleriYukleAsync(null);
            await IstenenAileyiAcAsync();
        }
    }

    private async Task IstenenAileyiAcAsync()
    {
        if (_istenenAileId is not { } aileId)
            return;
        _istenenAileId = null;
        tabControl.SelectedTabPage = tabPlan;
        tabSag.SelectedTabPage = grpYerlestir;
        if (lkAile.Properties.DataSource is not List<AileSecenegi> aileler || aileler.All(a => a.AileId != aileId))
        {
            HataYonetimi.Bilgi(this, "Bu ailenin çadıra yerleşmemiş üyesi yok.");
            return;
        }

        lkAile.EditValue = aileId;
        await OneriGetirAsync();
    }

    /// <param name="paneliKoru">Öneri panelindeki sonuç mesajı (ör. "yerleştirildi") silinmesin.</param>
    private async Task YenileAsync(bool paneliKoru = false)
    {
        IReadOnlyList<CadirPlanOgesi> plan = [];
        IReadOnlyList<Bolge> bolgeler = [];
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                plan = await _cadirServisi.CadirPlaniAsync();
                bolgeler = await _cadirServisi.BolgeleriListeleAsync(pasifleriDeGoster: true);
            }))
            return;

        _plan = plan.ToList();
        _bolgeler = bolgeler.ToList();
        BolgeSecenekleriniDoldur();
        PlaniCiz();
        TanimlariDoldur();
        await AileleriYukleAsync(lkAile.EditValue as int?, paneliKoru);
        await CadirSecAsync(_seciliCadirId);
    }

    // ---------------------------------------------------------------- Plan

    private void BolgeSecenekleriniDoldur()
    {
        var secili = cmbBolge.EditValue as int?;
        cmbBolge.Properties.Items.BeginUpdate();
        cmbBolge.Properties.Items.Clear();
        cmbBolge.Properties.Items.Add(new ImageComboBoxItem("Tüm bölgeler", null));
        foreach (var b in _bolgeler.Where(b => b.Aktif))
            cmbBolge.Properties.Items.Add(new ImageComboBoxItem($"{b.Kod} – {b.Ad}", (object)b.Id));
        cmbBolge.Properties.Items.EndUpdate();
        cmbBolge.EditValue = secili is { } id && _bolgeler.Any(b => b.Id == id && b.Aktif) ? id : null;
        if (cmbBolge.SelectedIndex < 0)
            cmbBolge.SelectedIndex = 0;

        var cadirBolgesi = cmbCadirBolge.EditValue;
        cmbCadirBolge.Properties.Items.Clear();
        foreach (var b in _bolgeler)
            cmbCadirBolge.Properties.Items.Add(new ImageComboBoxItem($"{b.Kod} – {b.Ad}{(b.Aktif ? "" : " (pasif)")}", (object)b.Id));
        cmbCadirBolge.EditValue = cadirBolgesi;
    }

    /// <summary>Çadırları bölge gruplarına, durum rengine göre çizer. Pasif çadır ve bölgeler planda gösterilmez.</summary>
    private void PlaniCiz()
    {
        var bolgeId = cmbBolge.EditValue as int?;
        tileCadirlar.BeginUpdate();
        try
        {
            tileCadirlar.Groups.Clear();
            foreach (var bolge in _plan.Where(c => c.Aktif && c.BolgeAktif && (bolgeId is null || c.BolgeId == bolgeId))
                         .GroupBy(c => (c.BolgeKodu, c.BolgeAdi)).OrderBy(g => g.Key.BolgeKodu))
            {
                var grup = new TileGroup { Text = bolge.Key.BolgeAdi }; // çadır kodları bölge kodunu zaten içerir
                foreach (var cadir in bolge)
                    grup.Items.Add(Kutucuk(cadir));
                tileCadirlar.Groups.Add(grup);
            }
        }
        finally
        {
            tileCadirlar.EndUpdate();
        }

        var toplamBos = _plan.Where(c => c.PlanDurumu is PlanDurumu.Bos or PlanDurumu.KismenDolu).Sum(c => c.BosYatak);
        lblPlanOzet.Text = $"Kullanılabilir çadırlarda toplam boş yatak: <b>{toplamBos}</b>";
    }

    private TileItem Kutucuk(CadirPlanOgesi c)
    {
        var ozellik = string.Concat(c.Erisilebilir ? "E " : "", c.SaglikNoktasinaYakin ? "S " : "", c.Isitmali ? "I" : "").Trim();
        var alt = c.PlanDurumu == PlanDurumu.Kullanilamaz ? c.Durum.Metin() : $"{c.Doluluk}/{c.Kapasite} kişi";
        var oge = new TileItem { Tag = c.Id, ItemSize = TileItemSize.Medium };
        oge.AppearanceItem.Normal.BackColor = PlanRenkleri[c.PlanDurumu];
        oge.AppearanceItem.Normal.BorderColor = PlanRenkleri[c.PlanDurumu];
        oge.AppearanceItem.Normal.ForeColor = Color.White;
        var kod = new TileItemElement { Text = c.Kod, TextAlignment = TileItemContentAlignment.TopLeft };
        kod.Appearance.Normal.FontSizeDelta = 4;
        kod.Appearance.Normal.FontStyleDelta = FontStyle.Bold;
        oge.Elements.Add(kod);
        oge.Elements.Add(new TileItemElement { Text = ozellik, TextAlignment = TileItemContentAlignment.TopRight });
        oge.Elements.Add(new TileItemElement { Text = alt, TextAlignment = TileItemContentAlignment.BottomLeft });
        oge.SuperTip = new DevExpress.Utils.SuperToolTip();
        oge.SuperTip.Items.Add(c.Aileler.Count == 0 ? $"{c.Kod}: boş" : $"{c.Kod}: {string.Join(", ", c.Aileler)}");
        return oge;
    }

    /// <summary>Seçili çadırın kutucuğunu seçim çerçevesiyle gösterir.</summary>
    private void SeciliKutucuguIsaretle() =>
        tileCadirlar.SelectedItem = tileCadirlar.Groups.Cast<TileGroup>().SelectMany(g => g.Items.Cast<TileItem>())
            .FirstOrDefault(o => Equals(o.Tag, _seciliCadirId));

    private void cmbBolge_EditValueChanged(object? sender, EventArgs e)
    {
        PlaniCiz();
        SeciliKutucuguIsaretle();
    }

    private async void btnYenile_Click(object? sender, EventArgs e) => await YenileAsync();

    private async void tileCadirlar_ItemClick(object? sender, TileItemEventArgs e)
    {
        if (e.Item.Tag is not int id)
            return;
        tabSag.SelectedTabPage = grpCadir;
        await CadirSecAsync(id);
    }

    private async Task CadirSecAsync(int? cadirId)
    {
        var sayac = ++_secimSayaci;
        var cadir = cadirId is { } id ? _plan.FirstOrDefault(c => c.Id == id) : null;
        _seciliCadirId = cadir?.Id;
        SeciliKutucuguIsaretle();

        CadirSecimiGoster(cadir);
        if (cadir is null)
            return;

        IReadOnlyList<CadirdakiKisi> kisiler = [];
        IReadOnlyList<BekleyenAile> bekleyenler = [];
        if (!await Bekleme.CalistirAsync(gridCadirdakiler, async () =>
            {
                kisiler = await _cadirServisi.CadirdakiKisilerAsync(cadir.Id);
                if (cadir.PlanDurumu is PlanDurumu.Bos or PlanDurumu.KismenDolu)
                    bekleyenler = await _beklemeServisi.CadirIcinOneriAsync(cadir.Id);
            }))
            return;
        if (sayac != _secimSayaci)
            return; // bu arada başka çadır seçildi

        gridCadirdakiler.DataSource = kisiler.Select(k => new CadirdakiSatir(k.Id, k.KisiNo, k.AdSoyad, k.Yas, k.Durum.Metin(),
            $"{k.AileNo} – {k.AileAdi}", k.Baslangic)).ToList();
        _cadiraOnerilenAile = bekleyenler.FirstOrDefault();
        CadirSecimiGoster(cadir);
    }

    private void CadirSecimiGoster(CadirPlanOgesi? cadir)
    {
        btnTasi.Enabled = cadir is { Doluluk: > 0 };
        btnBekleyeniYerlestir.Enabled = cadir is not null && _cadiraOnerilenAile is not null;
        if (cadir is null)
        {
            _cadiraOnerilenAile = null;
            gridCadirdakiler.DataSource = null;
            lblCadirOzet.Text = "<color=gray>Plandan bir çadıra tıklayın.</color>";
            return;
        }

        var metin = new StringBuilder();
        metin.Append($"<b>{cadir.Kod}</b> · {cadir.BolgeKodu} – {cadir.BolgeAdi}<br>");
        metin.Append($"Kapasite {cadir.Kapasite} · dolu {cadir.Doluluk} · boş yatak <b>{cadir.BosYatak}</b> · durum: {cadir.Durum.Metin()}" +
                     $" (<color={Html(cadir.PlanDurumu)}>{cadir.PlanDurumu.Metin()}</color>)<br>");
        var ozellikler = new List<string>();
        if (cadir.Erisilebilir) ozellikler.Add("erişilebilir");
        if (cadir.SaglikNoktasinaYakin) ozellikler.Add("sağlık noktasına yakın");
        if (cadir.Isitmali) ozellikler.Add("ısıtmalı");
        metin.Append($"Özellikler: {(ozellikler.Count == 0 ? "—" : string.Join(", ", ozellikler))}<br>");
        if (_cadiraOnerilenAile is { } aile)
            metin.Append($"Bekleme listesinden önerilen aile: <b>{aile.AileNo} – {aile.AileAdi}</b> ({aile.KisiSayisi} kişi, " +
                         $"yardım önceliği {aile.DestekOnceligi.Metin()}, {aile.EklenmeZamani:dd.MM.yyyy HH:mm} tarihinden beri bekliyor)");
        else if (cadir.PlanDurumu is PlanDurumu.Bos or PlanDurumu.KismenDolu)
            metin.Append("<color=gray>Bekleme listesinde bu çadıra sığan aile yok.</color>");
        lblCadirOzet.Text = metin.ToString();
    }

    private CadirPlanOgesi? SeciliCadir => _seciliCadirId is { } id ? _plan.FirstOrDefault(c => c.Id == id) : null;

    private async void btnTasi_Click(object? sender, EventArgs e)
    {
        if (SeciliCadir is not { } cadir)
            return;
        var secilenler = viewCadirdakiler.GetSelectedRows().Select(viewCadirdakiler.GetRow).OfType<CadirdakiSatir>().ToList();
        if (secilenler.Count == 0 && viewCadirdakiler.GetFocusedRow() is CadirdakiSatir odak)
            secilenler.Add(odak);
        if (secilenler.Count == 0)
        {
            HataYonetimi.Uyari(this, "Taşınacak kişileri listeden seçin.", "Eksik bilgi");
            return;
        }

        var hedefler = _plan
            .Where(c => c.Id != cadir.Id && (c.PlanDurumu is PlanDurumu.Bos or PlanDurumu.KismenDolu) && c.BosYatak >= secilenler.Count)
            .Select(c => ((object)c.Id, $"{c.Kod} – boş yatak {c.BosYatak}"))
            .ToList();
        if (hedefler.Count == 0)
        {
            HataYonetimi.Uyari(this, $"{secilenler.Count} kişinin sığabileceği kullanılabilir başka çadır yok.");
            return;
        }

        var cevap = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Başka çadıra taşı",
            Mesaj = $"<b>{cadir.Kod}</b> çadırından taşınacak: {string.Join(", ", secilenler.Select(s => s.AdSoyad))}. " +
                    "Çadır değişikliği kişilerin hareket geçmişine yazılır.",
            SecimEtiketi = "Yeni çadır",
            Secenekler = hedefler,
            TamamMetni = "Taşı",
        });
        if (cevap is not { Secim: int yeniCadirId, Aciklama: { } aciklama })
            return;

        if (await Bekleme.CalistirAsync(this, () => _cadirServisi.CadirDegistirAsync(secilenler.Select(s => s.Id).ToList(), yeniCadirId, aciklama)))
            await YenileAsync();
    }

    private async void btnBekleyeniYerlestir_Click(object? sender, EventArgs e)
    {
        if (SeciliCadir is not { } cadir || _cadiraOnerilenAile is not { } aile)
            return;
        if (!HataYonetimi.Onayla(this, $"{aile.AileNo} – {aile.AileAdi} ({aile.KisiSayisi} kişi) {cadir.Kod} çadırına yerleştirilecek ve " +
                                       "bekleme listesinden çıkacak. Onaylıyor musunuz?", "Bekleme listesinden yerleştir"))
            return;

        // Hata olsa da yenilenir: çadır bu arada dolmuş olabilir, güncel durum gösterilir.
        await Bekleme.CalistirAsync(this, () => _beklemeServisi.OnaylaVeYerlestirAsync(aile.KayitId, cadir.Id));
        await YenileAsync();
    }

    // ---------------------------------------------------------------- Aile yerleştirme

    private async Task AileleriYukleAsync(int? secilecekAileId, bool paneliKoru = false)
    {
        IReadOnlyList<YerlesmemisAile> aileler = [];
        if (!await Bekleme.CalistirAsync(lkAile, async () => aileler = await _cadirServisi.YerlesmemisAilelerAsync()))
            return;

        lkAile.Properties.DataSource = aileler.Select(a => new AileSecenegi(a.AileId,
            $"{a.AileNo} – {a.AileAdi} · {a.YerlesmemisKisiSayisi} kişi{(a.BeklemeListesinde ? " · bekleme listesinde" : "")}",
            a.BeklemeListesinde)).ToList();
        lkAile.EditValue = secilecekAileId is { } id && aileler.Any(a => a.AileId == id) ? id : null;
        lciAile.Text = $"Çadır bekleyen aile ({aileler.Count})";
        grpYerlestir.Text = $"Aile yerleştir ({aileler.Count} aile çadır bekliyor)";
        if (lkAile.EditValue is null && !paneliKoru)
            oneriPaneli.Temizle(aileler.Count == 0
                ? "Çadıra yerleşmemiş aile yok."
                : "Yerleştirilecek aileyi seçip “Çadır öner”e basın.");
    }

    private async void btnOner_Click(object? sender, EventArgs e) => await OneriGetirAsync();

    private async Task OneriGetirAsync()
    {
        if (lkAile.GetSelectedDataRow() is not AileSecenegi aile)
        {
            HataYonetimi.Uyari(this, "Önce yerleştirilecek aileyi seçin.", "Eksik bilgi");
            return;
        }

        await oneriPaneli.YukleAsync(aile.AileId, null, aile.BeklemeListesinde);
    }

    private void lkAile_EditValueChanged(object? sender, EventArgs e)
    {
        if (lkAile.EditValue is null)
            return;
        oneriPaneli.Temizle("“Çadır öner”e basın: sistem uygun çadırları önerir, görevli onaylar.");
    }

    // ---------------------------------------------------------------- Tanımlar: bölge

    private void TanimlariDoldur()
    {
        var seciliBolge = (viewBolgeler.GetFocusedRow() as BolgeSatiri)?.Id;
        var seciliCadir = (viewCadirTanim.GetFocusedRow() as CadirTanimSatiri)?.Id;

        gridBolgeler.DataSource = _bolgeler.Select(b => new BolgeSatiri(b.Id, b.Kod, b.Ad, b.Aciklama, _plan.Count(c => c.BolgeId == b.Id),
            b.Aktif ? "Aktif" : "Pasif", b)).ToList();
        gridCadirTanim.DataSource = _plan.Select(c => new CadirTanimSatiri(c.Id, c.Kod, $"{c.BolgeKodu} – {c.BolgeAdi}", c.Kapasite,
            c.Doluluk, c.Durum.Metin(), c.Erisilebilir, c.SaglikNoktasinaYakin, c.Isitmali, c.Aktif ? "Aktif" : "Pasif", c)).ToList();

        Odakla(viewBolgeler, nameof(BolgeSatiri.Id), seciliBolge);
        Odakla(viewCadirTanim, nameof(CadirTanimSatiri.Id), seciliCadir);
        BolgeSeciminiGoster();
        CadirTanimSeciminiGoster();
    }

    private static void Odakla(DevExpress.XtraGrid.Views.Grid.GridView gorunum, string alan, int? id)
    {
        if (id is null)
            return;
        var satir = gorunum.LocateByValue(alan, id);
        if (satir >= 0)
            gorunum.FocusedRowHandle = satir;
    }

    private BolgeSatiri? SeciliBolge => viewBolgeler.GetFocusedRow() as BolgeSatiri;

    private void viewBolgeler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => BolgeSeciminiGoster();

    private void BolgeSeciminiGoster()
    {
        if (SeciliBolge is not { } b)
        {
            YeniBolgeDurumu();
            return;
        }

        _yeniBolge = false;
        txtBolgeKod.Text = b.Kod;
        txtBolgeAd.Text = b.Ad;
        txtBolgeAciklama.Text = b.Aciklama;
        grpBolgeDuzen.Text = $"Bölge {b.Kod} düzenleniyor";
        btnBolgeSil.Enabled = btnBolgeAktiflik.Enabled = true;
        btnBolgeAktiflik.Text = b.Bolge.Aktif ? "Pasif yap" : "Aktif yap";
    }

    private void YeniBolgeDurumu()
    {
        _yeniBolge = true;
        viewBolgeler.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        txtBolgeKod.Text = txtBolgeAd.Text = txtBolgeAciklama.Text = "";
        grpBolgeDuzen.Text = "Yeni bölge";
        btnBolgeSil.Enabled = btnBolgeAktiflik.Enabled = false;
        btnBolgeAktiflik.Text = "Pasif yap";
    }

    private void btnBolgeYeni_Click(object? sender, EventArgs e)
    {
        YeniBolgeDurumu();
        txtBolgeKod.Focus();
    }

    private async void btnBolgeKaydet_Click(object? sender, EventArgs e)
    {
        var id = _yeniBolge ? null : SeciliBolge?.Id;
        Bolge? kayit = null;
        if (await Bekleme.CalistirAsync(this, async () =>
                kayit = await _cadirServisi.BolgeKaydetAsync(id, txtBolgeKod.Text, txtBolgeAd.Text, txtBolgeAciklama.Text)))
        {
            HataYonetimi.Bilgi(this, id is null ? "Bölge eklendi." : "Bölge güncellendi.");
            await YenileAsync();
            Odakla(viewBolgeler, nameof(BolgeSatiri.Id), kayit!.Id);
        }
    }

    private async void btnBolgeSil_Click(object? sender, EventArgs e)
    {
        if (SeciliBolge is not { } b || !HataYonetimi.Onayla(this, $"{b.Kod} – {b.Ad} bölgesi silinsin mi?", "Bölge sil"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _cadirServisi.BolgeSilAsync(b.Id)))
            await YenileAsync();
    }

    private async void btnBolgeAktiflik_Click(object? sender, EventArgs e)
    {
        if (SeciliBolge is not { } b)
            return;
        var aktif = !b.Bolge.Aktif;
        if (!aktif && !HataYonetimi.Onayla(this, $"{b.Kod} bölgesi pasif yapılırsa çadırlarına yeni yerleşim yapılamaz ve planda görünmez. " +
                                                  "Çadırlardaki mevcut kişiler etkilenmez. Devam edilsin mi?", "Bölgeyi pasif yap"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _cadirServisi.BolgeAktiflikDegistirAsync(b.Id, aktif)))
            await YenileAsync();
    }

    // ---------------------------------------------------------------- Tanımlar: çadır

    private CadirTanimSatiri? SeciliCadirTanim => viewCadirTanim.GetFocusedRow() as CadirTanimSatiri;

    private void viewCadirTanim_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => CadirTanimSeciminiGoster();

    private void CadirTanimSeciminiGoster()
    {
        if (SeciliCadirTanim?.Oge is not { } c)
        {
            YeniCadirDurumu();
            return;
        }

        _yeniCadir = false;
        cmbCadirBolge.EditValue = c.BolgeId;
        spnSira.Value = c.Sira;
        spnNumara.Value = c.Numara;
        spnKapasite.Value = c.Kapasite;
        cmbCadirDurum.EditValue = c.Durum;
        chkErisilebilir.Checked = c.Erisilebilir;
        chkSaglik.Checked = c.SaglikNoktasinaYakin;
        chkIsitmali.Checked = c.Isitmali;
        grpCadirDuzen.Text = $"Çadır {c.Kod} düzenleniyor";
        btnCadirSil.Enabled = btnCadirAktiflik.Enabled = true;
        btnCadirAktiflik.Text = c.Aktif ? "Pasif yap" : "Aktif yap";
    }

    private void YeniCadirDurumu()
    {
        _yeniCadir = true;
        viewCadirTanim.FocusedRowHandle = DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        cmbCadirBolge.EditValue = _bolgeler.FirstOrDefault(b => b.Aktif)?.Id;
        spnSira.Value = 1;
        spnNumara.Value = 1;
        spnKapasite.Value = 4;
        cmbCadirDurum.EditValue = CadirDurumu.Kullanilabilir;
        chkErisilebilir.Checked = chkSaglik.Checked = chkIsitmali.Checked = false;
        grpCadirDuzen.Text = "Yeni çadır";
        btnCadirSil.Enabled = btnCadirAktiflik.Enabled = false;
        btnCadirAktiflik.Text = "Pasif yap";
    }

    private void btnCadirYeni_Click(object? sender, EventArgs e)
    {
        YeniCadirDurumu();
        cmbCadirBolge.Focus();
    }

    private async void btnCadirKaydet_Click(object? sender, EventArgs e)
    {
        if (cmbCadirBolge.EditValue is not int bolgeId)
        {
            HataYonetimi.Uyari(this, "Çadırın bölgesini seçin.", "Eksik bilgi");
            return;
        }

        var bilgi = new CadirBilgisi
        {
            BolgeId = bolgeId, Sira = (int)spnSira.Value, Numara = (int)spnNumara.Value, Kapasite = (int)spnKapasite.Value,
            Durum = cmbCadirDurum.EditValue as CadirDurumu? ?? CadirDurumu.Kullanilabilir,
            Erisilebilir = chkErisilebilir.Checked, SaglikNoktasinaYakin = chkSaglik.Checked, Isitmali = chkIsitmali.Checked,
        };
        var id = _yeniCadir ? null : SeciliCadirTanim?.Id;
        var kaydedilenId = id;
        if (await Bekleme.CalistirAsync(this, async () =>
            {
                if (id is { } mevcut)
                    await _cadirServisi.CadirGuncelleAsync(mevcut, bilgi);
                else
                    kaydedilenId = (await _cadirServisi.CadirEkleAsync(bilgi)).Id;
            }))
        {
            HataYonetimi.Bilgi(this, id is null ? "Çadır eklendi." : "Çadır güncellendi.");
            await YenileAsync();
            Odakla(viewCadirTanim, nameof(CadirTanimSatiri.Id), kaydedilenId);
        }
    }

    private async void btnCadirSil_Click(object? sender, EventArgs e)
    {
        if (SeciliCadirTanim is not { } c || !HataYonetimi.Onayla(this, $"{c.Kod} çadırı silinsin mi?", "Çadır sil"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _cadirServisi.CadirSilAsync(c.Id)))
            await YenileAsync();
    }

    private async void btnCadirAktiflik_Click(object? sender, EventArgs e)
    {
        if (SeciliCadirTanim is not { } c)
            return;
        var aktif = !c.Oge.Aktif;
        if (await Bekleme.CalistirAsync(this, () => _cadirServisi.CadirAktiflikDegistirAsync(c.Id, aktif)))
            await YenileAsync();
    }
}
