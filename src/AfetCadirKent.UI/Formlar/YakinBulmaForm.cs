using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.YakinBulmaIslemleri;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 13 – Yakın bulma (REQ-212, şartname M8): başvuru, Türkçe karakter farkı yok sayılan ve yaş/memleketle daraltılan arama,
/// olası eşleşme, yazılı ve görüşmeye dayalı doğrulama, sonuçla kapatma. Bulunan kişinin çadır ve iletişim bilgisi gösterilmez.
/// Kurallar <see cref="IYakinBulmaServisi"/>'ndedir.
/// </summary>
public partial class YakinBulmaForm : XtraForm
{
    private sealed record AdaySatiri(int KisiId, string KisiNo, string AdSoyad, int Yas, string Cinsiyet, string? Memleket,
        string MemleketUyumu, string Durum, string Fotograf, bool FotografVar);

    private readonly IYakinBulmaServisi _servis;
    private readonly IKayitServisi _kayitServisi;
    private readonly bool _degistirebilir;
    private readonly bool _aileGorur;
    private List<YakinBulmaOzeti> _basvurular = [];
    private bool _yukleniyor;
    private int _fotografIstegi;

    public YakinBulmaForm(IYakinBulmaServisi servis, IKayitServisi kayitServisi, IOturum oturum)
    {
        InitializeComponent();
        _servis = servis;
        _kayitServisi = kayitServisi;
        _degistirebilir = oturum.IzinVarMi(Modul.YakinBulma, Izin.Degistirme);
        _aileGorur = oturum.IzinVarMi(Modul.Kayit, Izin.Gorme);

        cmbBasvuranAile.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbBasvuranAile.Properties.NullText = "(çadır kentte kayıtlı değil)";
        spnArananYas.Properties.NullText = "(bilinmiyor)";
        spnArananYas.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        spnArananYas.EditValue = null;
        memoDogrulama.Properties.NullValuePrompt = "Örn. “Kişiyle görüşüldü, ablasını ve doğduğu köyü doğru söyledi.”";
        memoSonuc.Properties.NullValuePrompt = "Örn. “Kardeşler kayıt masasında görevli eşliğinde buluştu.”";

        GridYardimcisi.BosMetinGoster(viewBasvurular, "Açık yakın bulma başvurusu yok.");
        GridYardimcisi.BosMetinGoster(viewAdaylar, "Aday yok. Açık bir başvuru seçilince kayıtlı kişilerde aranır.");
        viewBasvurular.RowStyle += (_, e) =>
        {
            if (viewBasvurular.GetRow(e.RowHandle) is YakinBulmaOzeti b)
                e.Appearance.ForeColor = !b.AcikMi ? Color.Gray : b.Durum == YakinBulmaDurumu.Dogrulandi ? Color.DarkGreen : e.Appearance.ForeColor;
        };

        lciBasvuranAile.Visibility = _aileGorur ? LayoutVisibility.Always : LayoutVisibility.Never;
        if (!_degistirebilir)
        {
            grpYeni.Visibility = LayoutVisibility.Never;
            foreach (var oge in new[] { lciEslesme, lciEslesmeKaldir, bosEslesme, lciDogrulama, lciGorusuldu, lciDogrula, bosDogrula, lciSonuc,
                         lciKapat, bosKapat })
                oge.Visibility = LayoutVisibility.Never;
        }

        DetayiGoster();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await AileleriYukleAsync();
        await YenileAsync();
    }

    private YakinBulmaOzeti? SeciliBasvuru => viewBasvurular.GetFocusedRow() as YakinBulmaOzeti;
    private AdaySatiri? SeciliAday => viewAdaylar.GetFocusedRow() as AdaySatiri;

    private async Task AileleriYukleAsync()
    {
        if (!_degistirebilir || !_aileGorur)
            return;
        IReadOnlyList<Entities.Kayit.Aile> aileler = [];
        if (!await Bekleme.CalistirAsync(this, async () => aileler = await _kayitServisi.AileleriListeleAsync()))
            return;
        cmbBasvuranAile.Properties.Items.Clear();
        cmbBasvuranAile.Properties.Items.Add(new ImageComboBoxItem("(çadır kentte kayıtlı değil)", (object?)null));
        foreach (var a in aileler.OrderBy(a => a.AileNo))
            cmbBasvuranAile.Properties.Items.Add(new ImageComboBoxItem($"{a.AileNo} – {a.AileAdi}", (object)a.Id));
        cmbBasvuranAile.EditValue = null;
    }

    private async Task YenileAsync(int? basvuruId = null)
    {
        IReadOnlyList<YakinBulmaOzeti> basvurular = [];
        var sadeceAcik = !chkKapananlar.Checked;
        if (!await Bekleme.CalistirAsync(this, async () => basvurular = await _servis.ListeleAsync(sadeceAcik)))
            return;

        basvuruId ??= SeciliBasvuru?.Id;
        _basvurular = basvurular.ToList();
        _yukleniyor = true;
        gridBasvurular.DataSource = _basvurular;
        var satir = basvuruId is { } id ? viewBasvurular.LocateByValue(nameof(YakinBulmaOzeti.Id), id) : -1;
        viewBasvurular.FocusedRowHandle = satir >= 0 ? satir : _basvurular.Count > 0 ? 0 : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        _yukleniyor = false;
        grpBasvurular.Text = $"Başvurular – {_basvurular.Count(b => b.AcikMi)} açık";
        await BasvuruyuGosterAsync();
    }

    /// <summary>Seçili başvurunun ayrıntısı; açık başvuruda adaylar kendiliğinden aranır.</summary>
    private async Task BasvuruyuGosterAsync()
    {
        DetayiGoster();
        gridAdaylar.DataSource = null;
        picAday.Image = null;
        if (SeciliBasvuru is not { AcikMi: true } basvuru)
            return;
        await AdaylariAraAsync(basvuru);
    }

    private async Task AdaylariAraAsync(YakinBulmaOzeti basvuru)
    {
        IReadOnlyList<YakinAdayi> adaylar = [];
        if (!await Bekleme.CalistirAsync(this, async () => adaylar = await _servis.AraAsync(basvuru.Id)))
            return;

        _yukleniyor = true;
        gridAdaylar.DataSource = adaylar.Select(a => new AdaySatiri(a.KisiId, a.KisiNo, a.AdSoyad, a.Yas, a.Cinsiyet.Metin(), a.Memleket,
            a.MemleketUyumu, a.Durum.Metin(), a.FotografVar ? "Var" : "Yok", a.FotografVar)).ToList();
        var satir = basvuru.EslesenKisiId is { } kId ? viewAdaylar.LocateByValue(nameof(AdaySatiri.KisiId), kId) : -1;
        viewAdaylar.FocusedRowHandle = satir >= 0 ? satir : adaylar.Count > 0 ? 0 : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        _yukleniyor = false;
        grpSecili.Text = $"Seçili başvuru – #{basvuru.Id} · {adaylar.Count} aday";
        await FotografiGosterAsync();
        DugmeleriGuncelle();
    }

    private async Task FotografiGosterAsync()
    {
        picAday.Image = null;
        if (SeciliAday is not { FotografVar: true } aday)
            return;

        // Hızlı satır değişiminde yalnızca son seçilen adayın fotoğrafı gösterilir.
        var istek = ++_fotografIstegi;
        byte[]? foto = null;
        if (!await Bekleme.CalistirAsync(picAday, async () => foto = await _servis.FotografAsync(aday.KisiId)) || istek != _fotografIstegi)
            return;
        picAday.Image = FotografIslemleri.Goruntu(foto);
    }

    private void DetayiGoster()
    {
        var b = SeciliBasvuru;
        if (b is null)
        {
            lblDetay.Text = "Soldan bir başvuru seçin veya yeni başvuru açın.";
            grpSecili.Text = "Seçili başvuru";
            DugmeleriGuncelle();
            return;
        }

        grpSecili.Text = $"Seçili başvuru – #{b.Id}";
        lblDetay.Text = $"<b>Başvuru #{b.Id}</b> · <b>{b.DurumMetni}</b> · {b.AcilisZamani:dd.MM.yyyy HH:mm}" +
                        (b.AcanKullanici is null ? "" : $" · Açan: {Html(b.AcanKullanici)}") + "<br>" +
                        $"Aranan: <b>{Html(b.ArananAdSoyad)}</b> · yaklaşık yaş: {(b.ArananYaklasikYas?.ToString() ?? "—")} · memleket: {Html(b.ArananMemleket ?? "—")}<br>" +
                        $"Başvuran: {Html(b.BasvuranAdSoyad)}" + (b.BasvuranTelefon is null ? "" : $" · {Html(b.BasvuranTelefon)}") +
                        (b.BasvuranAile is null ? "" : $" · {Html(b.BasvuranAile)}") +
                        (b.EslesenKisi is null ? "" : $"<br>Eşleşen kişi: <b>{Html(b.EslesenKisi)}</b>") +
                        (b.DogrulamaAciklamasi is null ? "" : $"<br>Doğrulama: {Html(b.DogrulamaAciklamasi)}") +
                        (b.Sonuc is null ? "" : $"<br>Sonuç ({b.KapanisZamani:dd.MM.yyyy HH:mm}): {Html(b.Sonuc)}");
        memoDogrulama.Text = "";
        chkGorusuldu.Checked = false;
        memoSonuc.Text = "";
        DugmeleriGuncelle();
    }

    private void DugmeleriGuncelle()
    {
        var b = SeciliBasvuru;
        var durum = b?.Durum;
        btnAra.Enabled = b is { AcikMi: true };
        btnEslesme.Enabled = durum is YakinBulmaDurumu.Acik or YakinBulmaDurumu.OlasiEslesme && SeciliAday is not null &&
                             SeciliAday.KisiId != b!.EslesenKisiId;
        btnEslesmeKaldir.Enabled = durum == YakinBulmaDurumu.OlasiEslesme;
        memoDogrulama.Enabled = chkGorusuldu.Enabled = btnDogrula.Enabled = durum == YakinBulmaDurumu.OlasiEslesme;
        memoSonuc.Enabled = btnKapat.Enabled = b is { AcikMi: true };
    }

    /// <summary>Kullanıcı metnindeki etiket karakterlerini etkisizleştirir (Türkçe harfler olduğu gibi kalır, K-086).</summary>
    private static string Html(string metin) => metin.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");

    // ---------------------------------------------------------------- olaylar

    private async void btnYenile_Click(object? sender, EventArgs e)
    {
        await AileleriYukleAsync();
        await YenileAsync();
    }

    private async void chkKapananlar_CheckedChanged(object? sender, EventArgs e) => await YenileAsync();

    private async void viewBasvurular_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            await BasvuruyuGosterAsync();
    }

    private async void viewAdaylar_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (_yukleniyor)
            return;
        DugmeleriGuncelle();
        await FotografiGosterAsync();
    }

    private async void btnBasvuruAc_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBasvuranAd.Text) || string.IsNullOrWhiteSpace(txtArananAd.Text) ||
            string.IsNullOrWhiteSpace(txtArananSoyad.Text))
        {
            HataYonetimi.Uyari(this, "Başvuranın adı soyadı ile aranan kişinin adı ve soyadı zorunludur.", "Eksik bilgi");
            (string.IsNullOrWhiteSpace(txtBasvuranAd.Text) ? txtBasvuranAd : string.IsNullOrWhiteSpace(txtArananAd.Text) ? txtArananAd : txtArananSoyad).Focus();
            return;
        }

        var bilgi = new YakinBulmaBilgisi
        {
            BasvuranAdSoyad = txtBasvuranAd.Text, BasvuranTelefon = BosIseNull(txtBasvuranTelefon.Text),
            BasvuranAileId = cmbBasvuranAile.EditValue as int?, ArananAd = txtArananAd.Text, ArananSoyad = txtArananSoyad.Text,
            ArananYaklasikYas = spnArananYas.EditValue is null ? null : (int)spnArananYas.Value,
            ArananMemleket = BosIseNull(txtArananMemleket.Text),
        };
        YakinBulmaOzeti? basvuru = null;
        if (!await Bekleme.CalistirAsync(this, async () => basvuru = await _servis.BasvuruAcAsync(bilgi)))
            return;

        foreach (var t in new[] { txtBasvuranAd, txtBasvuranTelefon, txtArananAd, txtArananSoyad, txtArananMemleket })
            t.Text = "";
        spnArananYas.EditValue = null;
        cmbBasvuranAile.EditValue = null;
        await YenileAsync(basvuru!.Id);
    }

    private static string? BosIseNull(string? metin) => string.IsNullOrWhiteSpace(metin) ? null : metin;

    private async void btnAra_Click(object? sender, EventArgs e)
    {
        if (SeciliBasvuru is { } b)
            await AdaylariAraAsync(b);
    }

    private async void btnEslesme_Click(object? sender, EventArgs e)
    {
        if (SeciliBasvuru is not { } b || SeciliAday is not { } aday)
            return;
        if (!HataYonetimi.Onayla(this, $"{aday.KisiNo} {aday.AdSoyad}, başvuru #{b.Id} için olası eşleşme olarak işaretlensin mi?\n\n" +
                                       "Bu henüz doğrulama değildir; kişiyle görüşüp kimliği teyit ettikten sonra “Doğrulandı” yapılır.",
                "Olası eşleşme"))
            return;
        if (await Bekleme.CalistirAsync(this, () => _servis.EslesmeIsaretleAsync(b.Id, aday.KisiId)))
            await YenileAsync(b.Id);
    }

    private async void btnEslesmeKaldir_Click(object? sender, EventArgs e)
    {
        if (SeciliBasvuru is not { } b)
            return;
        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Eşleşmeyi kaldır",
            Mesaj = $"Başvuru #{b.Id} için işaretlenen kişi ({b.EslesenKisi}) kaldırılacak ve başvuru yeniden “Açık” olacak.",
            TamamMetni = "Kaldır",
        });
        if (sonuc is not null && await Bekleme.CalistirAsync(this, () => _servis.EslesmeyiKaldirAsync(b.Id, sonuc.Aciklama!)))
            await YenileAsync(b.Id);
    }

    private async void btnDogrula_Click(object? sender, EventArgs e)
    {
        if (SeciliBasvuru is not { } b)
            return;
        if (string.IsNullOrWhiteSpace(memoDogrulama.Text))
        {
            HataYonetimi.Uyari(this, "Doğrulamanın nasıl yapıldığını yazın.", "Eksik bilgi");
            memoDogrulama.Focus();
            return;
        }

        if (!chkGorusuldu.Checked)
        {
            HataYonetimi.Uyari(this, "Fotoğraf benzerliği tek başına doğrulama sayılmaz. Bulunan kişiyle (çocuksa yanındaki yetişkinle) " +
                                     "görüşüp kimliği teyit ettikten sonra kutuyu işaretleyin.", "Doğrulama");
            chkGorusuldu.Focus();
            return;
        }

        var aciklama = memoDogrulama.Text;
        if (await Bekleme.CalistirAsync(this, () => _servis.DogrulaAsync(b.Id, aciklama, kisiyleGorusuldu: true)))
            await YenileAsync(b.Id);
    }

    private async void btnKapat_Click(object? sender, EventArgs e)
    {
        if (SeciliBasvuru is not { } b)
            return;
        if (string.IsNullOrWhiteSpace(memoSonuc.Text))
        {
            HataYonetimi.Uyari(this, "Sonuç yazılmadan başvuru kapatılamaz.", "Sonuç zorunlu");
            memoSonuc.Focus();
            return;
        }

        if (!HataYonetimi.Onayla(this, $"Başvuru #{b.Id} kapatılsın mı?", "Başvuruyu kapat"))
            return;
        var sonuc = memoSonuc.Text;
        if (await Bekleme.CalistirAsync(this, () => _servis.KapatAsync(b.Id, sonuc)))
            await YenileAsync(b.Id);
    }
}
