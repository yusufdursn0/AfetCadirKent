using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 11 – Dağıtım (REQ-210): ürün için günlük liste (malzeme ihtiyacı olan aileler önde), gerekçeli istisna ile aile ekleme,
/// onay ve FEFO ile partilerden ayırma, teslim ve kısmi teslimat, teslim iptali, liste iptali ve gün sonu.
/// Hak, tekrar alma süresi, ayırma ve stok hesaplarının tamamı <see cref="IDagitimServisi"/>'ndedir.
/// </summary>
public partial class DagitimForm : XtraForm
{
    private sealed record ListeSatiri(int Id, string UrunAdi, string Durum, DagitimDurumu DurumDegeri, int SatirSayisi, decimal ToplamHak,
        decimal ToplamTeslim, decimal ToplamBekleyen, decimal AcikAyrilan, DateTime Tarih);

    private sealed record AileSatiri(int DetayId, string AileNo, string AileAdi, int HakSahibiKisiSayisi, decimal HakMiktari,
        decimal TeslimEdilenMiktar, decimal BekleyenHak, decimal AyrilanMiktar, string Ihtiyac, string Oncelik, string? Istisna,
        string Durum, bool Aktif, bool OncelikliIhtiyac);

    private readonly IDagitimServisi _dagitimServisi;
    private readonly IUrunServisi _urunServisi;
    private readonly bool _degistirebilir;
    private readonly Font _kalinYazi;
    private DagitimListesi? _liste;
    private bool _yukleniyor;

    public DagitimForm(IDagitimServisi dagitimServisi, IUrunServisi urunServisi, IOturum oturum)
    {
        InitializeComponent();
        _dagitimServisi = dagitimServisi;
        _urunServisi = urunServisi;
        _degistirebilir = oturum.IzinVarMi(Modul.Dagitim, Izin.Degistirme);
        _kalinYazi = new Font(viewSatirlar.Appearance.Row.Font, FontStyle.Bold);
        Disposed += (_, _) => _kalinYazi.Dispose();

        _yukleniyor = true;
        dteTarih.EditValue = DateTime.Today;
        dteTarih.Properties.NullText = "(tüm günler)";
        _yukleniyor = false;
        cmbYeniUrun.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbTeslimAlan.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;

        GridYardimcisi.BosMetinGoster(viewListeler, "Bu gün için dağıtım listesi yok. Ürünü seçip “Liste oluştur”a basın.");
        GridYardimcisi.BosMetinGoster(viewSatirlar, "Soldan bir dağıtım listesi seçin.");
        GridYardimcisi.BosMetinGoster(viewTeslimler, "Bu aileye bu listede henüz teslim yapılmadı.");

        // Malzemeye özel ihtiyacı olan aileler kalın; listeden çıkarılan satırlar gri.
        viewSatirlar.RowStyle += (_, e) =>
        {
            if (viewSatirlar.GetRow(e.RowHandle) is not AileSatiri s)
                return;
            if (!s.Aktif)
                e.Appearance.ForeColor = Color.Gray;
            else if (s.OncelikliIhtiyac)
                e.Appearance.Font = _kalinYazi;
        };

        foreach (var oge in new[] { lciYeniUrun, lciListeAciklama, lciListeOlustur, bosOlustur, lciListeNot, lciOnayla, lciListeIptal,
                     lciGunSonu, lciAileEkle, lciSatirCikar, bosSatir, lciTeslimAlan, lciTeslimMiktar, lciTeslimBirim, lciTeslimAciklama,
                     lciTeslimEt, lciTeslimIptal, bosTeslim })
            oge.Visibility = _degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        DugmeleriGuncelle();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        // Önceki günlerden açık kalan listeler kapatılır (K-047); yalnızca değiştirme izni olan için.
        if (_degistirebilir)
            await Bekleme.CalistirAsync(this, () => _dagitimServisi.OncekiGunleriKapatAsync());
        await YenileAsync();
    }

    private ListeSatiri? SeciliListe => viewListeler.GetFocusedRow() as ListeSatiri;
    private AileSatiri? SeciliSatir => viewSatirlar.GetFocusedRow() as AileSatiri;
    private DagitimSatiri? SeciliDetay => SeciliSatir is { } s ? _liste?.Satirlar.FirstOrDefault(d => d.DetayId == s.DetayId) : null;

    private async Task YenileAsync(int? listeId = null, int? detayId = null)
    {
        IReadOnlyList<DagitimOzeti> listeler = [];
        IReadOnlyList<Entities.Depo.Urun> urunler = [];
        var tarih = dteTarih.EditValue is DateTime t ? DateOnly.FromDateTime(t) : (DateOnly?)null;
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                listeler = await _dagitimServisi.ListeleriGetirAsync(tarih);
                if (_degistirebilir)
                    urunler = await _urunServisi.UrunleriListeleAsync();
            }))
            return;

        if (_degistirebilir)
        {
            var seciliUrun = cmbYeniUrun.EditValue;
            cmbYeniUrun.Properties.Items.Clear();
            foreach (var u in urunler)
                cmbYeniUrun.Properties.Items.Add(new ImageComboBoxItem($"{u.Ad} ({u.Birim.Metin()})", (object)u.Id));
            cmbYeniUrun.EditValue = seciliUrun is int id && urunler.Any(u => u.Id == id) ? id : null;
        }

        listeId ??= SeciliListe?.Id;
        detayId ??= SeciliSatir?.DetayId;
        _yukleniyor = true;
        gridListeler.DataSource = listeler.Select(l => new ListeSatiri(l.Id, $"{l.UrunAdi} ({l.Birim.Metin()})", l.Durum.Metin(), l.Durum,
            l.SatirSayisi, l.ToplamHak, l.ToplamTeslim, l.ToplamBekleyen, l.AcikAyrilan, l.Tarih.ToDateTime(TimeOnly.MinValue))).ToList();
        var satir = listeId is { } lId ? viewListeler.LocateByValue(nameof(ListeSatiri.Id), lId) : -1;
        viewListeler.FocusedRowHandle = satir >= 0 ? satir : listeler.Count > 0 ? 0 : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        viewListeler.BestFitColumns();
        _yukleniyor = false;
        grpListeler.Text = tarih is { } g ? $"Dağıtım listeleri – {g:dd.MM.yyyy} ({listeler.Count})" : $"Dağıtım listeleri – tüm günler ({listeler.Count})";
        await ListeyiGosterAsync(detayId);
    }

    /// <summary>Seçili listenin satırlarını okur; seçili aile korunur.</summary>
    private async Task ListeyiGosterAsync(int? detayId = null)
    {
        if (SeciliListe is not { } secili)
        {
            _liste = null;
            SatirlariBagla(null);
            return;
        }

        DagitimListesi? liste = null;
        if (!await Bekleme.CalistirAsync(this, async () => liste = await _dagitimServisi.ListeGetirAsync(secili.Id)))
            return;
        _liste = liste;
        SatirlariBagla(detayId);
    }

    private void SatirlariBagla(int? detayId)
    {
        _yukleniyor = true;
        var satirlar = _liste?.Satirlar ?? [];
        gridSatirlar.DataSource = satirlar.Select(s => new AileSatiri(s.DetayId, s.AileNo, s.AileAdi, s.HakSahibiKisiSayisi, s.HakMiktari,
            s.TeslimEdilenMiktar, s.BekleyenHak, s.AyrilanMiktar, s.OncelikliIhtiyac ? "✔" : "—", s.DestekOnceligi.Metin(),
            s.IstisnaGerekcesi, s.Aktif ? "Listede" : "Çıkarıldı", s.Aktif, s.OncelikliIhtiyac)).ToList();
        var satir = detayId is { } dId ? viewSatirlar.LocateByValue(nameof(AileSatiri.DetayId), dId) : -1;
        viewSatirlar.FocusedRowHandle = satir >= 0 ? satir : satirlar.Count > 0 ? 0 : DevExpress.XtraGrid.GridControl.InvalidRowHandle;
        viewSatirlar.BestFitColumns();
        _yukleniyor = false;

        if (_liste?.Ozet is { } o)
        {
            var birim = o.Birim.Metin();
            var aktif = satirlar.Count(s => s.Aktif);
            var ihtiyacli = satirlar.Count(s => s.Aktif && s.OncelikliIhtiyac);
            lblListeOzet.Text = $"<b>{o.UrunAdi}</b> – {o.Tarih:dd.MM.yyyy} – <b>{o.Durum.Metin()}</b>.  {aktif} aile ({ihtiyacli} malzeme ihtiyacı ✔, " +
                                $"kalın yazılı ve önde) · hak {o.ToplamHak:#,##0.##} {birim} · teslim {o.ToplamTeslim:#,##0.##} · bekleyen " +
                                $"{o.ToplamBekleyen:#,##0.##} · ayrılan {o.AcikAyrilan:#,##0.##}" +
                                (string.IsNullOrWhiteSpace(o.Aciklama) ? "" : $"<br>Açıklama: {o.Aciklama}");
            grpSatirlar.Text = $"Seçili liste – {o.UrunAdi}";
        }
        else
        {
            lblListeOzet.Text = "";
            grpSatirlar.Text = "Seçili liste";
        }

        TeslimPaneliniGoster();
    }

    private void TeslimPaneliniGoster()
    {
        var detay = SeciliDetay;
        cmbTeslimAlan.Properties.Items.Clear();
        // Listeden çıkarılmış satıra teslim yapılmaz; teslim alan seçenekleri yalnızca aktif satırda gelir.
        foreach (var k in detay is { Aktif: true } ? detay.TeslimAlabilecekler : [])
            cmbTeslimAlan.Properties.Items.Add(new ImageComboBoxItem($"{k.AdSoyad} ({k.KisiNo})", (object)k.KisiId));
        cmbTeslimAlan.EditValue = detay is { Aktif: true } && detay.TeslimAlabilecekler.Count == 1 ? detay.TeslimAlabilecekler[0].KisiId : null;
        spnTeslimMiktar.Value = detay is null ? 0 : Math.Min(detay.BekleyenHak, detay.AyrilanMiktar);
        lblTeslimBirim.Text = _liste is null ? "" : _liste.Ozet.Birim.Metin() +
            (detay is null ? "" : $"  (bekleyen hak {detay.BekleyenHak:#,##0.##}, ayrılan {detay.AyrilanMiktar:#,##0.##})");
        txtTeslimAciklama.Text = "";
        gridTeslimler.DataSource = detay?.Teslimler.ToList() ?? [];
        grpTeslim.Text = (detay is null ? "Teslim (seçili aile)" : $"Teslim – {detay.AileNo} {detay.AileAdi}") + TeslimKapaliNedeni(detay);
        DugmeleriGuncelle();
    }

    /// <summary>Teslim düğmeleri kapalıyken nedeni başlıkta gösterilir (ör. liste henüz onaylanmadı).</summary>
    private string TeslimKapaliNedeni(DagitimSatiri? detay) => _liste?.Ozet.Durum switch
    {
        null => "",
        DagitimDurumu.Taslak => "  ·  Teslim için önce soldan “Listeyi onayla”ya basın.",
        DagitimDurumu.Kapandi => "  ·  Liste kapandı; kalan hak sonraki listeye bekleyen hak olarak gelir.",
        DagitimDurumu.Iptal => "  ·  Liste iptal edildi.",
        _ when detay is { Aktif: false } => "  ·  Bu satır listeden çıkarıldı.",
        _ when detay is { AyrilanMiktar: 0, BekleyenHak: > 0 } => "  ·  Bu satırın ayırması kaldırıldı; kalan hak sonraki listeye devreder.",
        _ when detay is { BekleyenHak: 0 } => "  ·  Hakkın tamamı teslim edildi.",
        _ => "",
    };

    private void DugmeleriGuncelle()
    {
        var durum = _liste?.Ozet.Durum;
        var bugun = _liste?.Ozet.Tarih == DateOnly.FromDateTime(DateTime.Today);
        var detay = SeciliDetay;
        var taslak = durum == DagitimDurumu.Taslak && bugun;
        var onayli = durum == DagitimDurumu.Onaylandi;

        btnOnayla.Enabled = taslak;
        btnListeIptal.Enabled = (taslak || onayli) && _liste!.Ozet.ToplamTeslim == 0;
        btnAileEkle.Enabled = taslak;
        btnSatirCikar.Enabled = taslak && detay is { Aktif: true };
        btnTeslimEt.Enabled = onayli && detay is { Aktif: true, AyrilanMiktar: > 0 };
        btnTeslimIptal.Enabled = onayli && detay is { AyrilanMiktar: > 0 };
    }

    // ---------------------------------------------------------------- olaylar

    private async void btnYenile_Click(object? sender, EventArgs e) => await YenileAsync();

    private async void dteTarih_EditValueChanged(object? sender, EventArgs e)
    {
        if (!_yukleniyor)
            await YenileAsync();
    }

    private async void viewListeler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            await ListeyiGosterAsync();
    }

    private void viewSatirlar_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        if (!_yukleniyor)
            TeslimPaneliniGoster();
    }

    private async void btnListeOlustur_Click(object? sender, EventArgs e)
    {
        if (cmbYeniUrun.EditValue is not int urunId)
        {
            HataYonetimi.Uyari(this, "Liste hazırlanacak ürünü seçin.", "Eksik bilgi");
            cmbYeniUrun.Focus();
            return;
        }

        DagitimListesi? liste = null;
        var aciklama = string.IsNullOrWhiteSpace(txtListeAciklama.Text) ? null : txtListeAciklama.Text;
        if (!await Bekleme.CalistirAsync(this, async () => liste = await _dagitimServisi.ListeOlusturAsync(urunId, aciklama)))
            return;

        txtListeAciklama.Text = "";
        var ihtiyacli = liste!.Satirlar.Count(s => s.OncelikliIhtiyac);
        HataYonetimi.Bilgi(this, $"{liste.Ozet.UrunAdi} için taslak liste hazırlandı: {liste.Satirlar.Count} aile, toplam hak " +
                                 $"{liste.Ozet.ToplamHak:#,##0.##} {liste.Ozet.Birim.Metin()}. Malzeme ihtiyacı olan {ihtiyacli} aile önde.\n\n" +
                                 "Kontrol edip “Listeyi onayla”ya basın.");
        _yukleniyor = true;
        dteTarih.EditValue = DateTime.Today;
        _yukleniyor = false;
        await YenileAsync(liste.Ozet.Id);
    }

    private async void btnOnayla_Click(object? sender, EventArgs e)
    {
        if (_liste is not { } liste || !HataYonetimi.Onayla(this,
                $"{liste.Ozet.UrunAdi} listesi onaylansın mı?\n\nToplam {liste.Ozet.ToplamBekleyen:#,##0.##} {liste.Ozet.Birim.Metin()}, son kullanma " +
                "tarihi en yakın uygun partilerden ayrılır ve kullanılabilir stoktan düşer. Onaydan sonra listeye aile eklenemez.",
                "Listeyi onayla"))
            return;

        if (await Bekleme.CalistirAsync(this, () => _dagitimServisi.OnaylaAsync(liste.Ozet.Id)))
            await YenileAsync(liste.Ozet.Id);
    }

    private async void btnListeIptal_Click(object? sender, EventArgs e)
    {
        if (_liste is not { } liste)
            return;
        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Listeyi iptal et",
            Mesaj = $"{liste.Ozet.UrunAdi} listesi iptal edilecek. Liste silinmez, iptal olarak geçmişte kalır; ayrılan miktar stoğa döner.",
            AciklamaEtiketi = "İptal nedeni", TamamMetni = "İptal et",
        });
        if (sonuc is null)
            return;

        if (await Bekleme.CalistirAsync(this, () => _dagitimServisi.ListeIptalAsync(liste.Ozet.Id, sonuc.Aciklama!)))
            await YenileAsync(liste.Ozet.Id);
    }

    private async void btnGunSonu_Click(object? sender, EventArgs e)
    {
        if (!HataYonetimi.Onayla(this, "Gün sonu yapılsın mı?\n\nTeslim edilmemiş ayırmalar kaldırılır ve miktar kullanılabilir stoğa döner; " +
                                       "onaylı listeler kapanır, onaylanmamış taslaklar iptal olur. Kalan haklar sonraki listeye bekleyen hak olarak gelir.",
                "Gün sonu"))
            return;

        GunSonuSonucu? sonuc = null;
        if (!await Bekleme.CalistirAsync(this, async () => sonuc = await _dagitimServisi.GunSonuAsync()))
            return;

        HataYonetimi.Bilgi(this, $"Gün sonu tamamlandı.\n\nKapanan liste: {sonuc!.KapananListe}\nİptal edilen taslak: {sonuc.IptalEdilenTaslak}\n" +
                                 $"Stoğa dönen ayrılmış miktar: {sonuc.SerbestBirakilanMiktar:#,##0.##}");
        await YenileAsync();
    }

    private async void btnAileEkle_Click(object? sender, EventArgs e)
    {
        if (_liste is not { } liste)
            return;
        IReadOnlyList<EklenebilirAile> aileler = [];
        if (!await Bekleme.CalistirAsync(this, async () => aileler = await _dagitimServisi.EklenebilirAilelerAsync(liste.Ozet.Id)))
            return;
        if (aileler.Count == 0)
        {
            HataYonetimi.Bilgi(this, "Listeye eklenebilecek başka aile yok (hak sahibi üyesi olan tüm aileler listede).");
            return;
        }

        // Tekrar alma süresi dolmamış aile gerekçesiz seçilirse diyalog aynı seçimle yeniden açılır.
        var birim = liste.Ozet.Birim.Metin();
        object? oncekiSecim = null;
        DiyalogSonucu? sonuc;
        int aileId;
        while (true)
        {
            sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
            {
                Baslik = "Listeye aile ekle",
                Mesaj = "Tekrar alma süresi dolmamış aile ancak gerekçe yazılarak (istisna) eklenebilir. Gerekçe işlem geçmişine kaydedilir.",
                SecimEtiketi = "Aile",
                Secenekler = aileler.Select(a => ((object)a.AileId, $"{a.AileNo} {a.AileAdi} – hak {a.Hak:#,##0.##} {birim}" +
                    (a.IstisnaGerekir ? $" (istisna gerekir{(a.SonrakiHakTarihi is { } t ? $", sonraki hak {t:dd.MM.yyyy}" : "")})" : ""))).ToList(),
                VarsayilanSecim = oncekiSecim,
                AciklamaEtiketi = "İstisna gerekçesi", AciklamaZorunlu = aileler.All(a => a.IstisnaGerekir), TamamMetni = "Ekle",
            });
            if (sonuc?.Secim is not int secim)
                return;

            aileId = secim;
            var secilen = aileler.First(a => a.AileId == aileId);
            if (!secilen.IstisnaGerekir || !string.IsNullOrWhiteSpace(sonuc.Aciklama))
                break;
            HataYonetimi.Uyari(this, $"{secilen.AileNo} {secilen.AileAdi} için tekrar alma süresi dolmamış; istisna gerekçesi yazılmadan eklenemez.",
                "Gerekçe zorunlu");
            oncekiSecim = aileId;
        }

        if (await Bekleme.CalistirAsync(this, () => _dagitimServisi.AileEkleAsync(liste.Ozet.Id, aileId, sonuc.Aciklama)))
            await YenileAsync(liste.Ozet.Id);
    }

    private async void btnSatirCikar_Click(object? sender, EventArgs e)
    {
        if (SeciliDetay is not { } detay)
            return;
        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Satırı listeden çıkar",
            Mesaj = $"{detay.AileNo} {detay.AileAdi} bu listeden çıkarılacak. Satır silinmez, “çıkarıldı” olarak kalır.",
            TamamMetni = "Çıkar",
        });
        if (sonuc is null)
            return;

        if (await Bekleme.CalistirAsync(this, () => _dagitimServisi.SatirCikarAsync(detay.DetayId, sonuc.Aciklama!)))
            await YenileAsync(detayId: detay.DetayId);
    }

    private async void btnTeslimEt_Click(object? sender, EventArgs e)
    {
        if (SeciliDetay is not { } detay || _liste is not { } liste)
            return;
        if (cmbTeslimAlan.EditValue is not int kisiId)
        {
            HataYonetimi.Uyari(this, "Teslim alan aile üyesini seçin.", "Eksik bilgi");
            cmbTeslimAlan.Focus();
            return;
        }

        var miktar = spnTeslimMiktar.Value;
        var aciklama = string.IsNullOrWhiteSpace(txtTeslimAciklama.Text) ? null : txtTeslimAciklama.Text;
        decimal kalan = 0;
        if (!await Bekleme.CalistirAsync(this, async () => kalan = await _dagitimServisi.TeslimEtAsync(detay.DetayId, miktar, kisiId, aciklama)))
            return;

        var birim = liste.Ozet.Birim.Metin();
        HataYonetimi.Bilgi(this, kalan > 0
            ? $"{detay.AileNo} {detay.AileAdi}: {miktar:#,##0.##} {birim} teslim edildi (kısmi teslimat).\nKalan {kalan:#,##0.##} {birim} bekleyen hak olarak kaldı."
            : $"{detay.AileNo} {detay.AileAdi}: {miktar:#,##0.##} {birim} teslim edildi. Hakkın tamamı teslim edildi.");
        await YenileAsync(liste.Ozet.Id, detay.DetayId);
    }

    private async void btnTeslimIptal_Click(object? sender, EventArgs e)
    {
        if (SeciliDetay is not { } detay || _liste is not { } liste)
            return;
        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Kalan teslimi iptal et",
            Mesaj = $"{detay.AileNo} {detay.AileAdi} için ayrılmış {detay.AyrilanMiktar:#,##0.##} {liste.Ozet.Birim.Metin()} kullanılabilir stoğa döner. " +
                    "Yapılmış teslimler geçerli kalır; teslim edilmeyen hak bekleyen hak olarak kalır.",
            AciklamaEtiketi = "İptal nedeni", TamamMetni = "Teslimi iptal et",
        });
        if (sonuc is null)
            return;

        if (await Bekleme.CalistirAsync(this, () => _dagitimServisi.TeslimIptalAsync(detay.DetayId, sonuc.Aciklama!)))
            await YenileAsync(liste.Ozet.Id, detay.DetayId);
    }
}
