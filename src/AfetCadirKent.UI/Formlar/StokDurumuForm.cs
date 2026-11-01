using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Depo;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 10 – Stok durumu (REQ-209): ürün bazında fiziksel / dağıtıma ayrılmış / kullanılabilir stok, partiler (dağıtım sırası = FEFO),
/// iade, hasar/fire ve sayım; stok hareketleri ve geri alma; kritik seviye ve SKT uyarıları, SKT'si geçenlerin fireye düşülmesi;
/// iki ayrı stok tahmini ve hesabın dayandığı dönem. Hesapların tamamı <see cref="IStokServisi"/>'ndedir.
/// </summary>
public partial class StokDurumuForm : XtraForm
{
    private enum PartiIslemi
    {
        Iade,
        Hasar,
        Sayim,
    }

    private sealed record UrunStokSatiri(int UrunId, string UrunAdi, string Birim, decimal FizikselMiktar, decimal AyrilanMiktar,
        decimal KullanilabilirMiktar, decimal SktGecmisMiktar, decimal KritikSeviye, string Durum, bool KritikAltinda);

    private sealed record PartiSatiri(int Id, string FefoSirasi, string PartiNo, string? BagisciAdi, DateTime GirisZamani, DateTime? Skt,
        decimal Miktar, decimal AyrilanMiktar, decimal KullanilabilirMiktar, string Durum, bool SktGecti, bool SktYaklasiyor);

    private sealed record HareketSatiri(int Id, DateTime Zaman, StokHareketTuru Tur, string TurAdi, string UrunAdi, string PartiNo,
        decimal Miktar, decimal StokEtkisi, string? AileNo, string? TeslimAlan, string? Aciklama, string? KullaniciAdi, string KayitDurumu,
        bool Aktif);

    private sealed record UyariSatiri(string Tur, int UrunId, string UrunAdi, int? PartiId, string Mesaj);

    private sealed record TahminSatiri(string Urun, string Kullanilabilir, string TuketimGunluk, string TuketimeGore, string HakGunluk,
        string HakaGore, string Donem, string Aciklama);

    private readonly IStokServisi _stokServisi;
    private readonly bool _degistirebilir;
    private List<UrunStokDurumu> _durum = [];

    public StokDurumuForm(IStokServisi stokServisi, IOturum oturum)
    {
        InitializeComponent();
        _stokServisi = stokServisi;
        _degistirebilir = oturum.IzinVarMi(Modul.Depo, Izin.Degistirme);

        cmbIslem.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbIslem.Properties.Items.Add(new ImageComboBoxItem("İade (depoya geri dönen)", PartiIslemi.Iade));
        cmbIslem.Properties.Items.Add(new ImageComboBoxItem("Hasar / fire", PartiIslemi.Hasar));
        cmbIslem.Properties.Items.Add(new ImageComboBoxItem("Sayım (sayılan miktarı girin)", PartiIslemi.Sayim));
        cmbIslem.EditValue = PartiIslemi.Hasar;

        cmbHUrun.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbHTur.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbHTur.Properties.Items.Add(new ImageComboBoxItem("(tüm türler)", (object?)null));
        foreach (var tur in Enum.GetValues<StokHareketTuru>())
            cmbHTur.Properties.Items.Add(new ImageComboBoxItem(tur.Metin(), tur));
        cmbHTur.EditValue = null;
        dteHBaslangic.EditValue = DateTime.Today.AddDays(-6);
        dteHBitis.EditValue = null;
        dteHBitis.Properties.NullText = "(bugün)";

        lblTahminBilgi.Text = "<b>Tüketime göre:</b> kullanılabilir stok ÷ son 7 günün günlük ortalama dağıtımı.  " +
                              "<b>Hakka göre:</b> kullanılabilir stok ÷ bugünkü kişi sayısına göre günlük toplam hak " +
                              "(hak miktarı ÷ tekrar alma süresi). Veri yoksa “hesaplanamıyor” yazar.";

        GridYardimcisi.BosMetinGoster(viewUrunStok, "Aktif ürün yok.");
        GridYardimcisi.BosMetinGoster(viewPartiler, "Seçili ürünün depoda partisi yok.");
        GridYardimcisi.BosMetinGoster(viewHareketler, "Bu filtreye uyan stok hareketi yok.");
        GridYardimcisi.BosMetinGoster(viewUyarilar, "Stok uyarısı yok: kritik seviyenin altında ürün ve SKT'si yaklaşan/geçen parti bulunmuyor.");
        GridYardimcisi.BosMetinGoster(viewTahmin, "Aktif ürün yok.");

        viewUrunStok.RowStyle += (_, e) =>
        {
            if (viewUrunStok.GetRow(e.RowHandle) is UrunStokSatiri { KritikAltinda: true })
                e.Appearance.ForeColor = Color.Firebrick;
        };
        viewPartiler.RowStyle += (_, e) =>
        {
            if (viewPartiler.GetRow(e.RowHandle) is PartiSatiri p && (p.SktGecti || p.SktYaklasiyor))
                e.Appearance.ForeColor = p.SktGecti ? Color.Firebrick : Color.DarkOrange;
        };
        viewHareketler.RowStyle += (_, e) =>
        {
            if (viewHareketler.GetRow(e.RowHandle) is HareketSatiri { Aktif: false })
                e.Appearance.ForeColor = Color.Gray;
        };
        viewUyarilar.DoubleClick += viewUyarilar_DoubleClick;

        foreach (var oge in new[] { lciIslem, lciIslemMiktar, lciIslemAciklama, lciIslemUygula, lciHareketPasif, lciFireyeDus })
            oge.Visibility = _degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        cmbIslem_EditValueChanged(null, EventArgs.Empty);
        DugmeleriGuncelle();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await YenileAsync();
        await HareketleriYukleAsync();
    }

    private UrunStokSatiri? SeciliUrun => viewUrunStok.GetFocusedRow() as UrunStokSatiri;
    private PartiSatiri? SeciliParti => viewPartiler.GetFocusedRow() as PartiSatiri;
    private HareketSatiri? SeciliHareket => viewHareketler.GetFocusedRow() as HareketSatiri;

    /// <summary>Stok durumu, uyarılar ve tahminleri yeniden okur; seçili ürün ve parti korunur.</summary>
    private async Task YenileAsync(int? urunId = null, int? partiId = null)
    {
        IReadOnlyList<UrunStokDurumu> durum = [];
        IReadOnlyList<StokUyarisi> uyarilar = [];
        IReadOnlyList<StokTahmini> tahminler = [];
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                durum = await _stokServisi.StokDurumuAsync();
                uyarilar = await _stokServisi.UyarilariGetirAsync();
                tahminler = await _stokServisi.TahminleriGetirAsync();
            }))
            return;

        urunId ??= SeciliUrun?.UrunId;
        partiId ??= SeciliParti?.Id;
        _durum = durum.ToList();

        gridUrunStok.DataSource = _durum.Select(u => new UrunStokSatiri(u.UrunId, u.UrunAdi, u.Birim.Metin(), u.FizikselMiktar,
            u.AyrilanMiktar, u.KullanilabilirMiktar, u.SktGecmisMiktar, u.KritikSeviye, UrunDurumu(u), u.KritikAltinda)).ToList();
        Odakla(viewUrunStok, nameof(UrunStokSatiri.UrunId), urunId);
        PartileriGoster(partiId);

        gridUyarilar.DataSource = uyarilar.Select(u => new UyariSatiri(u.Tur.Metin(), u.UrunId, u.UrunAdi, u.PartiId, u.Mesaj)).ToList();
        gridTahmin.DataSource = tahminler.Select(TahminSatiriOlustur).ToList();
        viewTahmin.BestFitColumns();

        var kritik = _durum.Count(u => u.KritikAltinda);
        var yaklasan = _durum.Sum(u => u.Partiler.Count(p => p.SktYaklasiyor && p.KullanilabilirMiktar > 0));
        var gecen = _durum.Sum(u => u.Partiler.Count(p => p.SktGecti));
        lblStokOzet.Text = $"{_durum.Count} ürün · kritik seviyenin altında: <b>{kritik}</b> · SKT'si yaklaşan parti: <b>{yaklasan}</b> · " +
                           $"SKT'si geçen parti: <b>{gecen}</b>" + (gecen > 0 ? " (“Uyarılar” sekmesinden fireye düşülebilir)" : "");
        tabUyarilar.Text = uyarilar.Count > 0 ? $"Uyarılar ve stok tahmini ({uyarilar.Count})" : "Uyarılar ve stok tahmini";
        btnFireyeDus.Enabled = gecen > 0;

        var hareketUrunu = cmbHUrun.EditValue;
        cmbHUrun.Properties.Items.Clear();
        cmbHUrun.Properties.Items.Add(new ImageComboBoxItem("(tüm ürünler)", (object?)null));
        foreach (var u in _durum)
            cmbHUrun.Properties.Items.Add(new ImageComboBoxItem(u.UrunAdi, (object)u.UrunId));
        cmbHUrun.EditValue = hareketUrunu is int hId && _durum.Any(u => u.UrunId == hId) ? hId : null;
    }

    private static string UrunDurumu(UrunStokDurumu u)
    {
        var durumlar = new List<string>();
        if (u.KritikAltinda)
            durumlar.Add("Kritik seviyenin altında");
        if (u.SktGecmisMiktar > 0)
            durumlar.Add("SKT geçmiş parti var");
        return durumlar.Count == 0 ? "Yeterli" : string.Join(" · ", durumlar);
    }

    private static TahminSatiri TahminSatiriOlustur(StokTahmini t)
    {
        var birim = t.Birim.Metin();
        string Gunluk(YeterlilikTahmini y) => y.GunlukMiktar is { } g ? $"{g:#,##0.##} {birim}/gün" : "—";
        return new TahminSatiri(t.UrunAdi, $"{t.KullanilabilirMiktar:#,##0.##} {birim}", Gunluk(t.TuketimeGore), t.TuketimeGore.Metin,
            Gunluk(t.HakaGore), t.HakaGore.Metin, t.DonemMetni, $"Tüketim: {t.TuketimeGore.Aciklama} · Hak: {t.HakaGore.Aciklama}");
    }

    private static void Odakla(GridView gorunum, string alan, int? id)
    {
        if (id is null)
            return;
        var satir = gorunum.LocateByValue(alan, id);
        if (satir >= 0)
            gorunum.FocusedRowHandle = satir;
    }

    private void viewUrunStok_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => PartileriGoster(null);

    private void PartileriGoster(int? partiId)
    {
        var urun = SeciliUrun is { } s ? _durum.FirstOrDefault(u => u.UrunId == s.UrunId) : null;
        gridPartiler.DataSource = urun?.Partiler.Select(p => new PartiSatiri(p.Id, p.FefoSirasi?.ToString() ?? "—", p.PartiNo, p.BagisciAdi,
            p.GirisZamani, p.SonKullanmaTarihi?.ToDateTime(TimeOnly.MinValue), p.Miktar, p.AyrilanMiktar, p.KullanilabilirMiktar,
            PartiDurumMetni(p), p.SktGecti, p.SktYaklasiyor)).ToList();
        grpPartiler.Text = urun is null
            ? "Partiler"
            : $"{urun.UrunAdi} partileri ({urun.Birim.Metin()}) – dağıtımda SKT'si en yakın uygun parti önce önerilir (dağıtım sırası 1)";
        Odakla(viewPartiler, nameof(PartiSatiri.Id), partiId);
        DugmeleriGuncelle();
    }

    private static string PartiDurumMetni(PartiDurumu p) =>
        p.SktGecti ? "SKT geçti – dağıtıma çıkamaz"
        : p.SktYaklasiyor ? "SKT yaklaşıyor"
        : p.KullanilabilirMiktar <= 0 ? "Tamamı dağıtıma ayrıldı"
        : "Uygun";

    private void viewPartiler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => DugmeleriGuncelle();

    private void viewHareketler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => DugmeleriGuncelle();

    private void DugmeleriGuncelle()
    {
        btnIslemUygula.Enabled = SeciliParti is not null;
        btnHareketPasif.Enabled = SeciliHareket is { Aktif: true, Tur: not StokHareketTuru.Dagitim };
    }

    private async void btnYenile_Click(object? sender, EventArgs e) => await YenileAsync();

    // ---------------------------------------------------------------- parti işlemleri

    private void cmbIslem_EditValueChanged(object? sender, EventArgs e) =>
        lciIslemMiktar.Text = cmbIslem.EditValue is PartiIslemi.Sayim ? "Sayılan miktar" : "Miktar";

    private async void btnIslemUygula_Click(object? sender, EventArgs e)
    {
        if (SeciliParti is not { } p || SeciliUrun is not { } u || cmbIslem.EditValue is not PartiIslemi islem)
            return;
        var miktar = spnIslemMiktar.Value;
        var aciklama = txtIslemAciklama.Text;
        var soru = islem switch
        {
            PartiIslemi.Iade => $"{u.UrunAdi} – parti {p.PartiNo}: {miktar:#,##0.##} {u.Birim} iade olarak stoğa eklenecek.",
            PartiIslemi.Hasar => $"{u.UrunAdi} – parti {p.PartiNo}: {miktar:#,##0.##} {u.Birim} hasar/fire olarak stoktan düşülecek.",
            _ => $"{u.UrunAdi} – parti {p.PartiNo}: sayılan miktar {miktar:#,##0.##} {u.Birim} (kayıtlı {p.Miktar:#,##0.##}). " +
                 "Fark kadar sayım farkı hareketi yazılacak.",
        };
        if (!HataYonetimi.Onayla(this, soru + " Onaylıyor musunuz?", "Parti işlemi"))
            return;

        SayimFarkiHareketi? sayim = null;
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                switch (islem)
                {
                    case PartiIslemi.Iade:
                        await _stokServisi.IadeAsync(p.Id, miktar, aciklama);
                        break;
                    case PartiIslemi.Hasar:
                        await _stokServisi.HasarAsync(p.Id, miktar, aciklama);
                        break;
                    default:
                        sayim = await _stokServisi.SayimAsync(p.Id, miktar, aciklama);
                        break;
                }
            }))
            return;

        HataYonetimi.Bilgi(this, sayim is null ? "İşlem kaydedildi." : $"Sayım kaydedildi: {sayim.TurAdi}, {sayim.Miktar:#,##0.##} {u.Birim}.");
        spnIslemMiktar.Value = 0;
        txtIslemAciklama.Text = "";
        await YenileAsync(u.UrunId, p.Id);
        await HareketleriYukleAsync();
    }

    // ---------------------------------------------------------------- hareketler

    private async Task HareketleriYukleAsync()
    {
        var filtre = new StokHareketFiltresi
        {
            UrunId = cmbHUrun.EditValue as int?,
            Tur = cmbHTur.EditValue as StokHareketTuru?,
            Baslangic = dteHBaslangic.EditValue is DateTime bas ? DateOnly.FromDateTime(bas) : null,
            Bitis = dteHBitis.EditValue is DateTime bit ? DateOnly.FromDateTime(bit) : null,
            PasifleriDeGoster = chkHPasif.Checked,
        };
        if (filtre.Baslangic > filtre.Bitis)
        {
            HataYonetimi.Uyari(this, "Başlangıç tarihi bitiş tarihinden sonra olamaz.", "Hatalı tarih");
            return;
        }

        IReadOnlyList<StokHareketSatiri> hareketler = [];
        if (!await Bekleme.CalistirAsync(this, async () => hareketler = await _stokServisi.HareketleriListeleAsync(filtre)))
            return;

        gridHareketler.DataSource = hareketler.Select(h => new HareketSatiri(h.Id, h.Zaman, h.Tur, h.TurAdi, h.UrunAdi, h.PartiNo, h.Miktar,
            h.StokEtkisi, h.AileNo, h.TeslimAlan, h.Aciklama, h.KullaniciAdi, h.Aktif ? "Aktif" : "Geri alındı", h.Aktif)).ToList();
        DugmeleriGuncelle();
    }

    private async void btnListele_Click(object? sender, EventArgs e) => await HareketleriYukleAsync();

    private async void btnHareketPasif_Click(object? sender, EventArgs e)
    {
        if (SeciliHareket is not { } h)
            return;
        var cevap = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Stok hareketini geri al",
            Mesaj = $"<b>{h.TurAdi}</b>: {h.UrunAdi}, parti {h.PartiNo}, {h.Miktar:#,##0.##}. Hareket silinmez, pasif yapılır ve stoğa etkisi " +
                    "geri alınır. Dağıtım hareketleri buradan geri alınamaz (teslim iptali dağıtım ekranından yapılır).",
            TamamMetni = "Geri al",
        });
        if (cevap?.Aciklama is not { } aciklama)
            return;

        if (await Bekleme.CalistirAsync(this, () => _stokServisi.HareketPasifYapAsync(h.Id, aciklama)))
        {
            await YenileAsync();
            await HareketleriYukleAsync();
        }
    }

    // ---------------------------------------------------------------- uyarılar

    private async void btnFireyeDus_Click(object? sender, EventArgs e)
    {
        if (!HataYonetimi.Onayla(this, "SKT'si geçmiş tüm partilerin dağıtıma ayrılmamış miktarı “hasar/fire” hareketiyle stoktan düşülecek. " +
                                       "Devam edilsin mi?", "SKT'si geçenleri fireye düş"))
            return;

        var sayi = 0;
        if (!await Bekleme.CalistirAsync(this, async () => sayi = await _stokServisi.SktGecenleriFireyeDusAsync()))
            return;
        HataYonetimi.Bilgi(this, sayi == 0 ? "Fireye düşülecek parti yok." : $"{sayi} parti hasar/fire olarak stoktan düşüldü.");
        await YenileAsync();
        await HareketleriYukleAsync();
    }

    /// <summary>Uyarıya çift tıklanınca ilgili ürün (ve parti) stok sekmesinde açılır.</summary>
    private void viewUyarilar_DoubleClick(object? sender, EventArgs e)
    {
        if (viewUyarilar.GetFocusedRow() is not UyariSatiri u)
            return;
        tabControl.SelectedTabPage = tabStok;
        Odakla(viewUrunStok, nameof(UrunStokSatiri.UrunId), u.UrunId);
        PartileriGoster(u.PartiId);
    }
}
