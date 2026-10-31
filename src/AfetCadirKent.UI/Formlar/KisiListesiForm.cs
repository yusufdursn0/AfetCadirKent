using System.Text;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 5 – Kişi listesi ve arama: filtreleme (Türkçe karakter farkı yok sayılır), durum değiştirme, pasif yapma ve
/// kişinin hareket geçmişi (REQ-204). Refakatsiz çocuk ve sağlık notu süzmesini Business yapar.
/// </summary>
public partial class KisiListesiForm : XtraForm
{
    private sealed record KisiSatiri(int Id, int AileId, string KisiNo, string AdSoyad, int Yas, string Cinsiyet, string Aile,
        string Durum, string? Memleket, string? TcKimlikNo, DateTime KayitZamani, string KayitDurumu);

    private sealed record HareketSatiri(DateTime Zaman, string Tur, string Cadir, string Aile, string? Aciklama, string Kullanici);

    private readonly IKayitServisi _kayitServisi;
    private readonly IOturum _oturum;
    private readonly FormAcici _formAcici;
    private readonly Dictionary<int, Afetzede> _kisiler = [];
    private int _secimSayaci;

    public KisiListesiForm(IKayitServisi kayitServisi, IOturum oturum, FormAcici formAcici)
    {
        InitializeComponent();
        _kayitServisi = kayitServisi;
        _oturum = oturum;
        _formAcici = formAcici;

        cmbDurum.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbDurum.Properties.Items.Add(new ImageComboBoxItem("Tümü", null));
        foreach (var durum in Enum.GetValues<KisiDurumu>())
            cmbDurum.Properties.Items.Add(new ImageComboBoxItem(durum.Metin(), durum));
        cmbDurum.SelectedIndex = 0;

        GridYardimcisi.BosMetinGoster(viewKisiler, "Aramaya uyan kişi yok.");
        GridYardimcisi.BosMetinGoster(viewHareketler, "Kişi seçildiğinde hareket geçmişi burada görünür.");

        var degistirebilir = _oturum.IzinVarMi(Modul.Kayit, Izin.Degistirme);
        foreach (var oge in new[] { lciDurumDugme, lciPasifDugme, lciYeniAile })
            oge.Visibility = degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        SecimiGoster(null);
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await ListeleAsync();
    }

    private KisiSatiri? SeciliSatir => viewKisiler.GetFocusedRow() as KisiSatiri;

    private async Task ListeleAsync(int? secilecekKisiId = null)
    {
        var filtre = new KisiFiltresi
        {
            Metin = txtAra.Text,
            Durum = cmbDurum.EditValue as KisiDurumu?,
            PasifleriDeGoster = chkPasifler.Checked,
        };

        IReadOnlyList<Afetzede> kisiler = [];
        if (!await Bekleme.CalistirAsync(this, async () => kisiler = await _kayitServisi.KisileriListeleAsync(filtre)))
            return;

        var bugun = DateOnly.FromDateTime(DateTime.Today);
        _kisiler.Clear();
        foreach (var k in kisiler)
            _kisiler[k.Id] = k;
        gridKisiler.DataSource = kisiler.Select(k => new KisiSatiri(k.Id, k.AileId, k.KisiNo, k.AdSoyad, k.Yas(bugun),
            k.Cinsiyet.Metin(), k.Aile is null ? "" : $"{k.Aile.AileNo} – {k.Aile.AileAdi}", k.Durum.Metin(), k.Memleket,
            k.TcKimlikNo, k.KayitZamani, k.Aktif ? "Aktif" : "Pasif")).ToList();
        viewKisiler.BestFitColumns();

        if (secilecekKisiId is { } id)
        {
            var satir = viewKisiler.LocateByValue(nameof(KisiSatiri.Id), id);
            if (satir >= 0)
                viewKisiler.FocusedRowHandle = satir;
        }

        await SecimiYukleAsync();
    }

    private async Task SecimiYukleAsync()
    {
        var sayac = ++_secimSayaci;
        var satir = SeciliSatir;
        SecimiGoster(satir is null ? null : _kisiler.GetValueOrDefault(satir.Id));
        if (satir is null)
        {
            gridHareketler.DataSource = null;
            return;
        }

        IReadOnlyList<KisiHareketi> hareketler = [];
        if (!await Bekleme.CalistirAsync(gridHareketler, async () => hareketler = await _kayitServisi.KisiHareketleriAsync(satir.Id)))
            return;
        if (sayac != _secimSayaci)
            return; // bu arada başka kişi seçildi

        gridHareketler.DataSource = hareketler.OrderByDescending(h => h.Zaman).Select(h => new HareketSatiri(
            h.Zaman, h.Tur.Metin(), Degisim(h.EskiCadir?.Kod, h.YeniCadir?.Kod), Degisim(h.EskiAile?.AileNo, h.YeniAile?.AileNo),
            h.Aciklama, h.Kullanici?.AdSoyad ?? "sistem")).ToList();
        viewHareketler.BestFitColumns();
    }

    private static string Degisim(string? eski, string? yeni) => (eski, yeni) switch
    {
        (null, null) => "",
        (null, _) => yeni!,
        (_, null) => eski,
        _ => $"{eski} → {yeni}",
    };

    private void SecimiGoster(Afetzede? kisi)
    {
        var eskiFoto = picFoto.Image;
        picFoto.Image = FotografIslemleri.Goruntu(kisi?.Fotograf);
        eskiFoto?.Dispose();

        btnDurum.Enabled = btnPasif.Enabled = kisi is { Aktif: true };
        btnAileyiAc.Enabled = kisi is not null;
        if (kisi is null)
        {
            lblKisiOzet.Text = "<color=gray>Listeden bir kişi seçin.</color>";
            return;
        }

        var bugun = DateOnly.FromDateTime(DateTime.Today);
        var metin = new StringBuilder();
        metin.Append($"<b>{kisi.KisiNo} – {kisi.AdSoyad}</b><br>");
        if (kisi.Aile is { } aile)
            metin.Append($"Aile: {aile.AileNo} – {aile.AileAdi}<br>");
        metin.Append($"Doğum: {kisi.DogumTarihi:dd.MM.yyyy}{(kisi.DogumTarihiTahmini ? " (tahmini)" : "")} · {kisi.Yas(bugun)} yaş<br>");
        metin.Append($"Durum: <b>{kisi.Durum.Metin()}</b>{(kisi.Aktif ? "" : " · <color=red>Pasif kayıt</color>")}<br>");
        var ozel = OzelDurumlar(kisi);
        if (ozel.Length > 0)
            metin.Append($"Özel durum: {ozel}<br>");
        if (!string.IsNullOrWhiteSpace(kisi.Telefon))
            metin.Append($"Telefon: {kisi.Telefon}<br>");
        if (!string.IsNullOrWhiteSpace(kisi.SaglikNotu))
            metin.Append($"Sağlık notu: {kisi.SaglikNotu}");
        lblKisiOzet.Text = metin.ToString();
    }

    /// <summary>Kişinin gerekçe hesabını etkileyen özel durumları (ör. "Hamile, Kronik hasta").</summary>
    public static string OzelDurumlar(Afetzede kisi)
    {
        var liste = new List<string>();
        if (kisi.Hamile) liste.Add("Hamile");
        if (kisi.TekerlekliSandalye) liste.Add("Tekerlekli sandalye");
        if (kisi.KronikHasta) liste.Add("Kronik hasta");
        if (kisi.RefakatsizCocuk) liste.Add("Refakatsiz çocuk");
        return string.Join(", ", liste);
    }

    private async void btnAra_Click(object? sender, EventArgs e) => await ListeleAsync();

    private async void viewKisiler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => await SecimiYukleAsync();

    private void viewKisiler_DoubleClick(object? sender, EventArgs e)
    {
        if (SeciliSatir is not null)
            AileyiAc(SeciliSatir.AileId);
    }

    private void btnAileyiAc_Click(object? sender, EventArgs e)
    {
        if (SeciliSatir is { } satir)
            AileyiAc(satir.AileId);
    }

    private void btnYeniAile_Click(object? sender, EventArgs e) => _formAcici.EkranAc<AileKayitForm>(Owner ?? this).YeniAile();

    private void AileyiAc(int aileId) => _formAcici.EkranAc<AileKayitForm>(Owner ?? this).AileyiAc(aileId);

    private async void btnDurum_Click(object? sender, EventArgs e)
    {
        if (SeciliSatir is not { } satir || !_kisiler.TryGetValue(satir.Id, out var kisi))
            return;

        var secenekler = KayitServisi.GecilebilirDurumlar(kisi.Durum).Select(d => ((object)d, d.Metin())).ToList();
        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Durum değiştir",
            Mesaj = $"<b>{kisi.KisiNo} – {kisi.AdSoyad}</b><br>Şu anki durum: {kisi.Durum.Metin()}. " +
                    "Hastaneye sevkte çadırdaki yer ve dağıtım hakları korunur; ayrılış ve nakilde çadırdaki yer boşalır.",
            SecimEtiketi = "Yeni durum",
            Secenekler = secenekler,
            AciklamaZorunlu = false,
            TamamMetni = "Durumu değiştir",
        });
        if (sonuc?.Secim is not KisiDurumu yeni)
            return;

        if (await Bekleme.CalistirAsync(this, () => _kayitServisi.DurumDegistirAsync(kisi.Id, yeni, sonuc.Aciklama)))
            await ListeleAsync(kisi.Id);
    }

    private async void btnPasif_Click(object? sender, EventArgs e)
    {
        if (SeciliSatir is not { } satir || !_kisiler.TryGetValue(satir.Id, out var kisi))
            return;

        var sonuc = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Kişiyi pasif yap",
            Mesaj = $"<b>{kisi.KisiNo} – {kisi.AdSoyad}</b> pasif yapılacak. Kayıt silinmez, geçmişiyle korunur; ancak " +
                    "<b>tekrar aktif yapılamaz</b> ve aynı TC kimlik numarasıyla yeniden kayıt açılamaz. Çadırdaki yeri boşalır.",
            TamamMetni = "Pasif yap",
        });
        if (sonuc?.Aciklama is not { } aciklama)
            return;

        if (await Bekleme.CalistirAsync(this, () => _kayitServisi.KisiPasifYapAsync(kisi.Id, aciklama)))
            await ListeleAsync(kisi.Id);
    }
}
