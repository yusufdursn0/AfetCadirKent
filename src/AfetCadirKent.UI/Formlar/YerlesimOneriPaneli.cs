using System.Text;
using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Çadır önerisi ve görevli onayı (şartname M4: "Sistem önerir, görevli onaylar"). Çadır planı (Ekran 6) ve bekleme
/// listesi (Ekran 7) aynı paneli kullanır. İki çadırlık öneride görevli kimin hangi çadıra gideceğini seçebilir.
/// Kapasite, kullanılabilirlik ve seçim kuralları Business'ta yeniden doğrulanır.
/// </summary>
public partial class YerlesimOneriPaneli : XtraUserControl
{
    private sealed record OneriSatiri(int Sira, string Cadirlar, string Etkiler, int BosYatak, CadirOnerisi Oneri);

    /// <summary>Yerleşecek kişi; <see cref="CadirId"/> görevlinin seçtiği çadırdır (grid'de düzenlenir).</summary>
    private sealed class KisiSatiri(YerlesecekKisi kisi)
    {
        public int Id { get; } = kisi.Id;
        public string KisiNo { get; } = kisi.KisiNo;
        public string AdSoyad { get; } = kisi.AdSoyad;
        public int Yas { get; } = kisi.Yas;
        public string Durum { get; } = kisi.Durum.Metin();
        public int? CadirId { get; set; }
    }

    private ICadirServisi? _cadirServisi;
    private IBeklemeListesiServisi? _beklemeServisi;
    private bool _degistirebilir;
    private YerlesimOnerisiSonucu? _sonuc;
    private int? _beklemeKaydiId;
    private bool _beklemeListesinde;
    private List<KisiSatiri> _kisiler = [];

    public YerlesimOneriPaneli()
    {
        InitializeComponent();
        GridYardimcisi.BosMetinGoster(viewOneriler, "Önerilecek çadır yok.");
        GridYardimcisi.BosMetinGoster(viewKisiler, "Yerleşecek kişi yok.");
        repoCadir.TextEditStyle = TextEditStyles.DisableTextEditor;
        Temizle("Yerleştirilecek aileyi seçip “Çadır öner”e basın.");
    }

    /// <summary>Yerleştirme yapıldığında veya aile bekleme listesine alındığında.</summary>
    public event EventHandler? KayitDegisti;

    /// <summary>Servisleri bağlar; panel tasarımcıda oluşturulduğu için bağımlılıklar kurucu yerine burada verilir.</summary>
    public void Baglan(ICadirServisi cadirServisi, IBeklemeListesiServisi beklemeServisi, IOturum oturum)
    {
        _cadirServisi = cadirServisi;
        _beklemeServisi = beklemeServisi;
        _degistirebilir = oturum.IzinVarMi(Modul.Cadir, Izin.Degistirme);
        lciOnayla.Visibility = _degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        DugmeleriGuncelle();
    }

    public void Temizle(string mesaj)
    {
        _sonuc = null;
        _beklemeKaydiId = null;
        _beklemeListesinde = false;
        _kisiler = [];
        lblBilgi.Text = $"<color=gray>{mesaj}</color>";
        gridOneriler.DataSource = null;
        gridKisiler.DataSource = null;
        DugmeleriGuncelle();
    }

    /// <param name="beklemeKaydiId">Aile bekleme listesinden yerleştiriliyorsa liste kaydı (onay listeden çıkarır).</param>
    /// <param name="beklemeListesinde">Aile zaten listede mi ("Bekleme listesine al" düğmesi için).</param>
    public async Task YukleAsync(int aileId, int? beklemeKaydiId, bool beklemeListesinde)
    {
        if (_cadirServisi is null)
            return;

        YerlesimOnerisiSonucu? sonuc = null;
        if (!await Bekleme.CalistirAsync(this, async () => sonuc = await _cadirServisi.OneriGetirAsync(aileId)))
            return;

        _sonuc = sonuc!;
        _beklemeKaydiId = beklemeKaydiId;
        _beklemeListesinde = beklemeListesinde || beklemeKaydiId is not null;
        lblBilgi.Text = BilgiMetni(_sonuc);
        gridOneriler.DataSource = _sonuc.Oneriler.Select((o, i) => new OneriSatiri(i + 1,
            string.Join(" + ", o.Atamalar.Select(a => $"{a.CadirKodu} ({a.KisiSayisi} kişi)")),
            string.Join(" · ", o.KarsilananEtkiler.Select(e => $"{e.Metin()} ✔").Concat(o.KarsilanmayanEtkiler.Select(e => $"{e.Metin()} ✘"))),
            o.BosYatakIsrafi, o)).ToList();
        _kisiler = _sonuc.YerlesecekKisiler.Select(k => new KisiSatiri(k)).ToList();
        gridKisiler.DataSource = _kisiler;
        viewOneriler.FocusedRowHandle = 0;
        OneriyiUygula();
    }

    private string BilgiMetni(YerlesimOnerisiSonucu s)
    {
        var metin = new StringBuilder();
        metin.Append($"<b>{s.AileNo} – {s.AileAdi}</b> · yerleşecek {s.YerlesecekKisiSayisi} kişi<br>");
        metin.Append($"Yardım önceliği: <b>{s.DestekOnceligi.Metin()}</b>");
        if (s.IstenenEtkiler.Count > 0)
            metin.Append($" · Yerleşim ihtiyacı: {string.Join(", ", s.IstenenEtkiler.Select(e => e.Metin()))}");
        metin.Append("<br>");
        metin.Append(s.BeklemeListesiOnerilir ? $"<color=red>{s.Mesaj}</color>" : s.Mesaj);
        if (_beklemeListesinde && _beklemeKaydiId is null)
            metin.Append("<br><color=gray>Aile bekleme listesinde; yerleşince listeden çıkar.</color>");
        if (s.YerlesecekKisiSayisi > 0 && !s.KisiSecimiYapilabilir)
            metin.Append("<br><color=gray>Bazı kişiler Koordinatör takibinde olduğundan kişiler kayıt sırasıyla dağıtılır.</color>");
        return metin.ToString();
    }

    private CadirOnerisi? SeciliOneri => (viewOneriler.GetFocusedRow() as OneriSatiri)?.Oneri;

    /// <summary>Seçilen önerinin çadırlarını kişi listesine uygular: varsayılan dağıtım kayıt sırasıyladır.</summary>
    private void OneriyiUygula()
    {
        var oneri = SeciliOneri;
        repoCadir.Items.Clear();
        if (oneri is not null)
        {
            foreach (var a in oneri.Atamalar)
                repoCadir.Items.Add(new ImageComboBoxItem(a.CadirKodu, (object)a.CadirId));
            var sira = 0;
            foreach (var a in oneri.Atamalar)
                foreach (var kisi in _kisiler.Skip(sira).Take(a.KisiSayisi))
                {
                    kisi.CadirId = a.CadirId;
                    sira++;
                }
        }
        else
        {
            _kisiler.ForEach(k => k.CadirId = null);
        }

        // Kişi seçimi yalnızca iki çadırlık öneride anlamlıdır; tek çadırda herkes aynı çadıra gider.
        colKCadir.OptionsColumn.AllowEdit = _degistirebilir && oneri is { TekCadir: false } && _sonuc is { KisiSecimiYapilabilir: true };
        viewKisiler.RefreshData();
        lciKisiler.Text = colKCadir.OptionsColumn.AllowEdit
            ? "Yerleşecek kişiler — iki çadıra bölünürken her kişinin çadırını “Çadır” sütunundan seçebilirsiniz"
            : "Yerleşecek kişiler";
        DugmeleriGuncelle();
    }

    private void DugmeleriGuncelle()
    {
        btnOnayla.Enabled = _degistirebilir && SeciliOneri is not null;
        var bekletilebilir = _degistirebilir && _sonuc is { YerlesecekKisiSayisi: > 0 } && !_beklemeListesinde;
        lciBeklemeyeAl.Visibility = bekletilebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
    }

    private void viewOneriler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => OneriyiUygula();

    private void viewKisiler_CellValueChanged(object? sender, CellValueChangedEventArgs e) => viewKisiler.RefreshData();

    private async void btnOnayla_Click(object? sender, EventArgs e)
    {
        if (_sonuc is not { } sonuc || SeciliOneri is not { } oneri || _cadirServisi is null || _beklemeServisi is null)
            return;
        viewKisiler.CloseEditor();
        viewKisiler.UpdateCurrentRow();

        if (Atamalar(oneri) is not { } atamalar)
            return;

        var dagilim = string.Join("\n", atamalar.Select(a => a.KisiIdler is { } idler
            ? $"• {a.CadirKodu}: {string.Join(", ", _kisiler.Where(k => idler.Contains(k.Id)).Select(k => k.AdSoyad))}"
            : $"• {a.CadirKodu}: {a.KisiSayisi} kişi"));
        if (!HataYonetimi.Onayla(FindForm(), $"{sonuc.AileNo} – {sonuc.AileAdi} şu çadır(lar)a yerleştirilecek:\n{dagilim}\n\nOnaylıyor musunuz?",
                "Yerleşimi onayla"))
            return;

        var aileId = sonuc.AileId;
        var kayitId = _beklemeKaydiId;
        var tamam = await Bekleme.CalistirAsync(this, () => kayitId is { } k
            ? _beklemeServisi.OnaylaVeYerlestirAsync(k, atamalar)
            : _cadirServisi.YerlestirAsync(aileId, atamalar));
        if (!tamam)
        {
            // Çadır bu arada dolmuş olabilir: öneriyi güncel veriyle yenile.
            await YukleAsync(aileId, kayitId, _beklemeListesinde);
            return;
        }

        Temizle($"{sonuc.AileNo} – {sonuc.AileAdi} yerleştirildi: {string.Join(" + ", atamalar.Select(a => a.CadirKodu))}.");
        KayitDegisti?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Onaylanacak atamalar; kişi seçimi tutarsızsa görevliyi uyarır ve null döner.</summary>
    private IReadOnlyList<CadirAtamasi>? Atamalar(CadirOnerisi oneri)
    {
        if (!colKCadir.OptionsColumn.AllowEdit)
            return oneri.Atamalar;

        if (_kisiler.Any(k => k.CadirId is null))
        {
            HataYonetimi.Uyari(FindForm(), "Her kişi için bir çadır seçin.", "Eksik bilgi");
            return null;
        }

        var hatali = oneri.Atamalar.FirstOrDefault(a => _kisiler.Count(k => k.CadirId == a.CadirId) != a.KisiSayisi);
        if (hatali is not null)
        {
            HataYonetimi.Uyari(FindForm(),
                $"{hatali.CadirKodu} çadırına {hatali.KisiSayisi} kişi yerleşmeli; şu an {_kisiler.Count(k => k.CadirId == hatali.CadirId)} kişi seçili. " +
                "Öneri, çadırların boş yataklarına göre hesaplanmıştır.", "Kişi seçimi");
            return null;
        }

        return oneri.Atamalar
            .Select(a => a with { KisiIdler = _kisiler.Where(k => k.CadirId == a.CadirId).Select(k => k.Id).ToList() })
            .ToList();
    }

    private async void btnBeklemeyeAl_Click(object? sender, EventArgs e)
    {
        if (_sonuc is not { } sonuc || _beklemeServisi is null)
            return;

        var cevap = IslemDiyalogu.Goster(FindForm()!, new DiyalogAyarlari
        {
            Baslik = "Bekleme listesine al",
            Mesaj = $"<b>{sonuc.AileNo} – {sonuc.AileAdi}</b> ({sonuc.YerlesecekKisiSayisi} kişi) bekleme listesine alınacak. " +
                    "Uygun bir çadırda yer açılınca sistem aileyi önerir; önce yardım önceliği yüksek olan, eşitlikte en uzun bekleyen.",
            AciklamaZorunlu = false,
            TamamMetni = "Listeye al",
        });
        if (cevap is null)
            return;

        if (!await Bekleme.CalistirAsync(this, () => _beklemeServisi.EkleAsync(sonuc.AileId, cevap.Aciklama)))
            return;

        Temizle($"{sonuc.AileNo} – {sonuc.AileAdi} bekleme listesine alındı.");
        KayitDegisti?.Invoke(this, EventArgs.Empty);
    }
}
