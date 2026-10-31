using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.UI.Altyapi;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout.Utils;

namespace AfetCadirKent.UI.Formlar;

/// <summary>
/// Ekran 7 – Bekleme listesi (REQ-075…077, REQ-206): yer bekleyen aileler (önce yardım önceliği yüksek olan, eşitlikte en uzun
/// bekleyen), yer açılan çadırlar ve her biri için sistemin önerdiği aile, görevli onayıyla yerleştirme. Tek çadıra sığmayan
/// aile için iki çadırlık öneri ve kişi seçimi ortak öneri panelinden yapılır. Sıralama ve öneri kuralı Business'tadır.
/// </summary>
public partial class BeklemeListesiForm : XtraForm
{
    private sealed record BekleyenSatiri(int Sira, int KayitId, int AileId, string AileNo, string AileAdi, int KisiSayisi, string Oncelik,
        string Gerekceler, DateTime EklenmeZamani, string BeklemeSuresi, string? Aciklama);

    private sealed record YerAcilanSatiri(int CadirId, string Cadir, int BosYatak, int KayitId, string Aile, int KisiSayisi, string Oncelik,
        DateTime EklenmeZamani);

    private readonly ICadirServisi _cadirServisi;
    private readonly IBeklemeListesiServisi _beklemeServisi;
    private readonly bool _degistirebilir;

    public BeklemeListesiForm(ICadirServisi cadirServisi, IBeklemeListesiServisi beklemeServisi, IOturum oturum)
    {
        InitializeComponent();
        _cadirServisi = cadirServisi;
        _beklemeServisi = beklemeServisi;
        _degistirebilir = oturum.IzinVarMi(Modul.Cadir, Izin.Degistirme);
        oneriPaneli.Baglan(cadirServisi, beklemeServisi, oturum);
        oneriPaneli.KayitDegisti += async (_, _) => await YenileAsync(paneliKoru: true);

        GridYardimcisi.BosMetinGoster(viewBekleyenler, "Bekleme listesinde aile yok.");
        GridYardimcisi.BosMetinGoster(viewYerAcilan, "Listedeki ailelerden birine yetecek boş yeri olan çadır yok.");
        foreach (var oge in new[] { lciListeyeEkle, lciListedenCikar, lciYerAcilanYerlestir })
            oge.Visibility = _degistirebilir ? LayoutVisibility.Always : LayoutVisibility.Never;
        DugmeleriGuncelle();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);
        await YenileAsync();
    }

    private BekleyenSatiri? SeciliBekleyen => viewBekleyenler.GetFocusedRow() as BekleyenSatiri;
    private YerAcilanSatiri? SeciliYerAcilan => viewYerAcilan.GetFocusedRow() as YerAcilanSatiri;

    private async Task YenileAsync(bool paneliKoru = false)
    {
        IReadOnlyList<BekleyenAile> bekleyenler = [];
        IReadOnlyList<YerAcilanCadirOnerisi> yerAcilanlar = [];
        if (!await Bekleme.CalistirAsync(this, async () =>
            {
                bekleyenler = await _beklemeServisi.ListeleAsync();
                yerAcilanlar = await _beklemeServisi.YerAcilanCadirlarAsync();
            }))
            return;

        var seciliKayit = SeciliBekleyen?.KayitId;
        var simdi = DateTime.Now;
        gridBekleyenler.DataSource = bekleyenler.Select((b, i) => new BekleyenSatiri(i + 1, b.KayitId, b.AileId, b.AileNo, b.AileAdi,
            b.KisiSayisi, b.DestekOnceligi.Metin(), b.Gerekceler, b.EklenmeZamani, Sure(simdi - b.EklenmeZamani), b.Aciklama)).ToList();
        gridYerAcilan.DataSource = yerAcilanlar.Select(y => new YerAcilanSatiri(y.Cadir.Id, y.Cadir.Kod, y.Cadir.BosYatak,
            y.OnerilenAile.KayitId, $"{y.OnerilenAile.AileNo} – {y.OnerilenAile.AileAdi}", y.OnerilenAile.KisiSayisi,
            y.OnerilenAile.DestekOnceligi.Metin(), y.OnerilenAile.EklenmeZamani)).ToList();
        viewBekleyenler.BestFitColumns();

        if (seciliKayit is { } kayitId)
        {
            var satir = viewBekleyenler.LocateByValue(nameof(BekleyenSatiri.KayitId), kayitId);
            if (satir >= 0)
                viewBekleyenler.FocusedRowHandle = satir;
        }

        grpBekleyenler.Text = $"Bekleme listesi – {bekleyenler.Count} aile (önce yardım önceliği yüksek olan, eşitlikte en uzun bekleyen)";
        if (!paneliKoru && (SeciliBekleyen is null || SeciliBekleyen.KayitId != seciliKayit))
            oneriPaneli.Temizle("Listeden bir aile seçip “Çadır öner”e basın. Tek çadıra sığmayan aileye iki çadırlık öneri yapılır.");
        DugmeleriGuncelle();
    }

    private static string Sure(TimeSpan sure) =>
        sure.TotalDays >= 1 ? $"{(int)sure.TotalDays} gün {sure.Hours} sa" : sure.TotalHours >= 1 ? $"{(int)sure.TotalHours} sa" : "1 saatten az";

    private void DugmeleriGuncelle()
    {
        btnOner.Enabled = SeciliBekleyen is not null;
        btnListedenCikar.Enabled = SeciliBekleyen is not null;
        btnYerAcilanYerlestir.Enabled = SeciliYerAcilan is not null;
    }

    private void viewBekleyenler_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e)
    {
        DugmeleriGuncelle();
        if (SeciliBekleyen is { } b)
            oneriPaneli.Temizle($"{b.AileNo} – {b.AileAdi}: “Çadır öner”e basın; sistem uygun çadırları önerir, görevli onaylar.");
    }

    private void viewYerAcilan_FocusedRowChanged(object? sender, FocusedRowChangedEventArgs e) => DugmeleriGuncelle();

    private async void btnYenile_Click(object? sender, EventArgs e) => await YenileAsync();

    private async void btnOner_Click(object? sender, EventArgs e)
    {
        if (SeciliBekleyen is { } b)
            await oneriPaneli.YukleAsync(b.AileId, b.KayitId, beklemeListesinde: true);
    }

    private async void viewBekleyenler_DoubleClick(object? sender, EventArgs e)
    {
        if (SeciliBekleyen is { } b)
            await oneriPaneli.YukleAsync(b.AileId, b.KayitId, beklemeListesinde: true);
    }

    private async void btnYerAcilanYerlestir_Click(object? sender, EventArgs e)
    {
        if (SeciliYerAcilan is not { } y)
            return;
        if (!HataYonetimi.Onayla(this, $"{y.Aile} ({y.KisiSayisi} kişi) {y.Cadir} çadırına yerleştirilecek ve bekleme listesinden çıkacak. " +
                                       "Onaylıyor musunuz?", "Bekleme listesinden yerleştir"))
            return;

        // Hata olsa da yenilenir: çadır bu arada dolmuş olabilir, güncel durum gösterilir.
        if (await Bekleme.CalistirAsync(this, () => _beklemeServisi.OnaylaVeYerlestirAsync(y.KayitId, y.CadirId)))
            HataYonetimi.Bilgi(this, $"{y.Aile} {y.Cadir} çadırına yerleştirildi.");
        await YenileAsync();
    }

    private async void btnListedenCikar_Click(object? sender, EventArgs e)
    {
        if (SeciliBekleyen is not { } b)
            return;
        var cevap = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Listeden çıkar",
            Mesaj = $"<b>{b.AileNo} – {b.AileAdi}</b> yerleştirilmeden bekleme listesinden çıkarılacak (ör. başka yere gitti). " +
                    "Kayıt silinmez; işlem geçmişine yazılır.",
            TamamMetni = "Listeden çıkar",
        });
        if (cevap?.Aciklama is not { } aciklama)
            return;

        if (await Bekleme.CalistirAsync(this, () => _beklemeServisi.ListedenCikarAsync(b.KayitId, aciklama)))
            await YenileAsync();
    }

    private async void btnListeyeEkle_Click(object? sender, EventArgs e)
    {
        IReadOnlyList<YerlesmemisAile> aileler = [];
        if (!await Bekleme.CalistirAsync(this, async () => aileler = await _cadirServisi.YerlesmemisAilelerAsync()))
            return;

        var secenekler = aileler.Where(a => !a.BeklemeListesinde)
            .Select(a => ((object)a.AileId, $"{a.AileNo} – {a.AileAdi} · {a.YerlesmemisKisiSayisi} kişi"))
            .ToList();
        if (secenekler.Count == 0)
        {
            HataYonetimi.Bilgi(this, "Çadıra yerleşmemiş ve listede olmayan aile yok.");
            return;
        }

        var cevap = IslemDiyalogu.Goster(this, new DiyalogAyarlari
        {
            Baslik = "Bekleme listesine al",
            Mesaj = "Uygun çadır bulunamayan aile listeye alınır. Yer açılınca sistem aileyi önerir.",
            SecimEtiketi = "Aile",
            Secenekler = secenekler,
            AciklamaZorunlu = false,
            TamamMetni = "Listeye al",
        });
        if (cevap is not { Secim: int aileId })
            return;

        if (await Bekleme.CalistirAsync(this, () => _beklemeServisi.EkleAsync(aileId, cevap.Aciklama)))
            await YenileAsync();
    }
}
