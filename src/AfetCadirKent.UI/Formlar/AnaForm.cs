using AfetCadirKent.Business.AnaEkran;
using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraCharts;
using DevExpress.XtraGauges.Win.Gauges.Circular;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 3 – Ana ekran: Ribbon menü ve özet göstergeler (REQ-160, REQ-202). Göstergeler ve menü öğeleri oturumdaki
/// rolün izinlerine göre gösterilir (K-081); Business her işlemde izni ayrıca kontrol eder.
/// </summary>
public partial class AnaForm : RibbonForm
{
    /// <summary>Ana ekranda gösterilen stok uyarısı satırı.</summary>
    private sealed record StokUyariSatiri(string TurMetni, string UrunAdi, string Mesaj);

    private readonly IOturum _oturum;
    private readonly IAnaEkranServisi _anaEkranServisi;
    private readonly IGunlukOzetServisi _gunlukOzetServisi;
    private readonly IDagitimServisi _dagitimServisi;
    private readonly FormAcici _formAcici;
    private readonly RibbonYetkileri _yetkiler = new();
    private readonly ArcScaleComponent _dolulukOlcegi;
    private bool _uyariGosterildi;

    public AnaForm(IOturum oturum, IAnaEkranServisi anaEkranServisi, IGunlukOzetServisi gunlukOzetServisi,
        IDagitimServisi dagitimServisi, FormAcici formAcici)
    {
        InitializeComponent();
        _oturum = oturum;
        _anaEkranServisi = anaEkranServisi;
        _gunlukOzetServisi = gunlukOzetServisi;
        _dagitimServisi = dagitimServisi;
        _formAcici = formAcici;

        SimgeAta(bbiYenile, "svgimages/icon%20builder/actions_refresh.svg");
        SimgeAta(bbiGunlukOzet, "svgimages/snap/summary.svg");
        SimgeAta(bbiParolaDegistir, "svgimages/xaf/action_security_changepassword.svg");
        SimgeAta(bbiOturumuKapat, "svgimages/xaf/action_logoff.svg");

        _dolulukOlcegi = DolulukGostergesiOlustur();
        GridYardimcisi.BosMetinGoster(gridViewUyarilar, "Stok uyarısı yok: kritik seviyenin altında ürün ve son kullanma tarihi yaklaşan parti bulunmuyor.");

        // Rol bazlı menü: sonraki ekranlar eklendikçe Ribbon öğeleri burada izinleriyle eşlenir.
        _yetkiler.Rol(bbiGunlukOzet, RolTuru.Koordinator);
        _yetkiler.Uygula(ribbonControl, _oturum);

        var kullanici = _oturum.Kullanici;
        bsiKullanici.Caption = kullanici is null ? "" : $"Kullanıcı: {kullanici.AdSoyad} ({_oturum.Rol?.Metin()})";
    }

    /// <summary>Kullanıcı "Oturumu kapat" dediyse true: uygulama kapanmaz, giriş ekranına döner.</summary>
    public bool OturumKapatildi { get; private set; }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await OncekiGunleriKapatAsync();
        await YenileAsync();
    }

    private static void SimgeAta(BarButtonItem oge, string yol) =>
        oge.ImageOptions.SvgImage = ImageResourceCache.Default.GetSvgImage(yol);

    private ArcScaleComponent DolulukGostergesiOlustur()
    {
        var gosterge = gaugeDoluluk.AddCircularGauge();
        gosterge.AddDefaultElements();
        var olcek = gosterge.Scales[0];
        olcek.MinValue = 0;
        olcek.MaxValue = 100;
        olcek.Value = 0;
        return olcek;
    }

    /// <summary>
    /// Önceki günlerden kalan dağıtım ayırmalarını kaldırır (şartname M5 "gün sonunda teslim edilmemişse ayırma kaldırılır",
    /// K-047). Yalnızca dağıtım değiştirme izni olan kullanıcının girişinde çalışır.
    /// </summary>
    private async Task OncekiGunleriKapatAsync()
    {
        if (!_oturum.IzinVarMi(Modul.Dagitim, Izin.Degistirme))
            return;

        await Bekleme.CalistirAsync(layoutControl, async () =>
        {
            var sonuc = await _dagitimServisi.OncekiGunleriKapatAsync();
            if (sonuc.KapananListe + sonuc.IptalEdilenTaslak > 0)
                alertControl.Show(this, "Önceki günlerin dağıtımları kapatıldı",
                    $"{sonuc.KapananListe} liste kapandı, {sonuc.IptalEdilenTaslak} taslak iptal edildi; " +
                    $"teslim edilmeyen {sonuc.SerbestBirakilanMiktar:0.##} birim stoğa döndü.");
        });
    }

    private async Task YenileAsync()
    {
        AnaEkranOzeti? ozet = null;
        if (!await Bekleme.CalistirAsync(layoutControl, async () => ozet = await _anaEkranServisi.GetirAsync()) || ozet is null)
            return;

        Goster(ozet);
        bsiSonGuncelleme.Caption = $"Son güncelleme: {DateTime.Now:HH:mm}";

        if (!_uyariGosterildi && ozet.StokUyarilari is { Count: > 0 } uyarilar)
        {
            _uyariGosterildi = true;
            var kritik = uyarilar.Count(u => u.Tur == StokUyariTuru.KritikSeviye);
            var skt = uyarilar.Count - kritik;
            alertControl.Show(this, "Stok uyarısı", $"{kritik} ürün kritik seviyenin altında, {skt} partide son kullanma tarihi uyarısı var.");
        }
    }

    private void Goster(AnaEkranOzeti ozet)
    {
        layoutControl.BeginUpdate();
        try
        {
            Gorunur(lciKisi, ozet.ToplamKisi is not null);
            lblKisi.Text = Gosterge(ozet.ToplamKisi, $"{ozet.AileSayisi} aile · {ozet.HastanedekiKisi} kişi hastanede");

            Gorunur(lciYatak, ozet.BosYatak is not null);
            Gorunur(lciBekleme, ozet.BekleyenAile is not null);
            Gorunur(lciDoluluk, ozet.BosYatak is not null);
            lblYatak.Text = Gosterge(ozet.BosYatak,
                $"Kapasite {ozet.ToplamKapasite} · bakımda/kapalı çadır {ozet.KullanilamayanCadir}");
            lblBekleme.Text = Gosterge(ozet.BekleyenAile, "Yer bekleyen aile");
            _dolulukOlcegi.Value = (float)(ozet.DolulukYuzdesi ?? 0);
            lciDoluluk.Text = ozet.DolulukYuzdesi is { } yuzde ? $"Yatak doluluğu: %{yuzde:0.#}" : "Yatak doluluğu: kullanılabilir çadır yok";

            Gorunur(lciTalep, ozet.AcikTalepler is not null);
            Gorunur(lciTalepGrafigi, ozet.AcikTalepler is not null);
            if (ozet.AcikTalepler is { } talepler)
            {
                var toplam = talepler.Values.Sum();
                lblTalep.Text = Gosterge(toplam, toplam == 0 ? "Açık talep yok"
                    : talepler.TryGetValue("Sınıflandırılmamış", out var sinifsiz) ? $"{sinifsiz} sınıflandırılmamış" : "Tamamı sınıflandırılmış");
                TalepGrafiginiDoldur(talepler);
            }

            Gorunur(lciUyarilar, ozet.StokUyarilari is not null);
            gridUyarilar.DataSource = ozet.StokUyarilari?
                .Select(u => new StokUyariSatiri(u.Tur.Metin(), u.UrunAdi, u.Mesaj)).ToList();

            Gorunur(lciOzet, ozet.GunlukOzetGorebilir);
            Gorunur(lciBos, ozet.BosMu);
        }
        finally
        {
            layoutControl.EndUpdate();
        }
    }

    private void TalepGrafiginiDoldur(IReadOnlyDictionary<string, int> talepler)
    {
        chartTalepler.Series.Clear();
        if (talepler.Count == 0)
            return;

        var seri = new Series("Açık talepler", ViewType.Bar) { LabelsVisibility = DefaultBoolean.True };
        foreach (var (tur, sayi) in talepler)
            seri.Points.Add(new SeriesPoint(tur, sayi));
        chartTalepler.Series.Add(seri);
    }

    private static void Gorunur(DevExpress.XtraLayout.LayoutControlItem oge, bool gorunur) =>
        oge.Visibility = gorunur ? LayoutVisibility.Always : LayoutVisibility.Never;

    private static string Gosterge(int? deger, string aciklama) =>
        $"<size=+14><b>{deger}</b></size><br><color=gray>{aciklama}</color>";

    private async void bbiYenile_ItemClick(object? sender, ItemClickEventArgs e) => await YenileAsync();

    private async void bbiGunlukOzet_ItemClick(object? sender, ItemClickEventArgs e)
    {
        bbiGunlukOzet.Enabled = false;
        try
        {
            // Yapay zeka cevabı uzun sürebilir; yalnızca özet alanı beklemeye alınır, diğer işlemler sürer.
            GunlukOzet? sonuc = null;
            if (!await Bekleme.CalistirAsync(memoOzet, async () => sonuc = await _gunlukOzetServisi.OlusturAsync()) || sonuc is null)
                return;

            if (sonuc.YapayZekaYazdi)
            {
                memoOzet.Text = sonuc.Metin;
                lciOzet.Text = $"Yapay zeka günlük özeti ({DateTime.Now:dd.MM.yyyy HH:mm}) – yapay zeka önerisidir, kararı koordinatör verir";
            }
            else
            {
                HataYonetimi.Uyari(this, (sonuc.HataMesaji ?? "Yapay zeka özeti oluşturulamadı.") +
                    "\n\nGöstergeler yapay zeka olmadan da güncel bilgiyi gösterir.", "Günlük özet");
            }
        }
        finally
        {
            bbiGunlukOzet.Enabled = true;
        }
    }

    private void bbiParolaDegistir_ItemClick(object? sender, ItemClickEventArgs e) =>
        _formAcici.DiyalogAc<ParolaDegistirForm>(this);

    private void bbiOturumuKapat_ItemClick(object? sender, ItemClickEventArgs e)
    {
        if (!HataYonetimi.Onayla(this, "Oturumu kapatıp giriş ekranına dönmek istiyor musunuz?"))
            return;
        OturumKapatildi = true;
        Close();
    }
}
