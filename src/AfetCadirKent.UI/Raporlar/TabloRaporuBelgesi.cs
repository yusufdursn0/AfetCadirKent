using AfetCadirKent.Business.Raporlama;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace AfetCadirKent.UI.Raporlar;

/// <summary>
/// İş katmanının hazırladığı <see cref="TabloRaporu"/>'nu XtraReports belgesine çevirir: başlık ve alt başlıklar, her sayfada
/// tekrar eden sütun başlıkları, satırlar (veri bağlamalı), özet, imza alanları, hazırlayan ve sayfa numarası.
/// Aynı belge ekranda gösterilir, yazdırılır ve PDF/Excel'e aktarılır (şartname M13).
/// </summary>
public sealed class TabloRaporuBelgesi : XtraReport
{
    /// <summary>Veri bağlama satırı; hücreler H0…H11 alanlarına bağlanır.</summary>
    public sealed class Satir
    {
        private readonly IReadOnlyList<string> _hucreler;

        public Satir(IReadOnlyList<string> hucreler) => _hucreler = hucreler;

        private string H(int i) => i < _hucreler.Count ? _hucreler[i] : "";

        public string H0 => H(0);
        public string H1 => H(1);
        public string H2 => H(2);
        public string H3 => H(3);
        public string H4 => H(4);
        public string H5 => H(5);
        public string H6 => H(6);
        public string H7 => H(7);
        public string H8 => H(8);
        public string H9 => H(9);
        public string H10 => H(10);
        public string H11 => H(11);
    }

    public const int EnFazlaSutun = 12;
    private static readonly Color BaslikRengi = Color.FromArgb(31, 78, 121);
    private static readonly Color CizgiRengi = Color.FromArgb(200, 200, 200);

    public TabloRaporuBelgesi(TabloRaporu rapor)
    {
        ArgumentNullException.ThrowIfNull(rapor);
        if (rapor.Sutunlar.Count is 0 or > EnFazlaSutun)
            throw new ArgumentException($"Rapor 1–{EnFazlaSutun} sütun içermelidir.", nameof(rapor));

        DisplayName = rapor.Baslik;
        PaperKind = DXPaperKind.A4;
        Landscape = rapor.Sutunlar.Count > 6;
        Margins = new DXMargins(40, 40, 40, 40);
        Font = new DXFont("Segoe UI", 8.5f);
        var genislik = (Landscape ? 1169f : 827f) - Margins.Left - Margins.Right;

        Bands.Add(UstBilgi(rapor, genislik));
        Bands.Add(new PageHeaderBand { HeightF = 24, Controls = { Tablo(rapor, genislik, baslik: true) } });
        Bands.Add(new DetailBand { HeightF = 20, Controls = { Tablo(rapor, genislik, baslik: false) } });
        Bands.Add(AltBilgi(rapor, genislik));
        Bands.Add(SayfaAltligi(rapor, genislik));
        DataSource = rapor.Satirlar.Select(s => new Satir(s)).ToList();
    }

    private static ReportHeaderBand UstBilgi(TabloRaporu rapor, float genislik)
    {
        var bant = new ReportHeaderBand();
        var baslik = new XRLabel
        {
            Text = rapor.Baslik, LocationF = new PointF(0, 0), SizeF = new SizeF(genislik, 30), Font = new DXFont("Segoe UI", 16f, DXFontStyle.Bold),
            ForeColor = BaslikRengi,
        };
        var ust = new XRLabel
        {
            Text = "Afet Çadır Kent Yönetim Otomasyonu", LocationF = new PointF(0, 30), SizeF = new SizeF(genislik, 16), ForeColor = Color.Gray,
        };
        bant.Controls.AddRange([baslik, ust]);
        var y = 50f;
        foreach (var alt in rapor.AltBasliklar)
        {
            bant.Controls.Add(new XRLabel { Text = alt, LocationF = new PointF(0, y), SizeF = new SizeF(genislik, 18), Multiline = true, CanGrow = true });
            y += 18;
        }

        bant.HeightF = y + 8;
        return bant;
    }

    private static XRTable Tablo(TabloRaporu rapor, float genislik, bool baslik)
    {
        var toplam = rapor.Sutunlar.Sum(s => s.Genislik);
        var satir = new XRTableRow();
        for (var i = 0; i < rapor.Sutunlar.Count; i++)
        {
            var sutun = rapor.Sutunlar[i];
            var hucre = new XRTableCell
            {
                WidthF = genislik * sutun.Genislik / toplam, Multiline = true, CanGrow = true,
                Padding = new PaddingInfo(3, 3, 2, 2),
                TextAlignment = sutun.Sayisal ? TextAlignment.MiddleRight : TextAlignment.MiddleLeft,
                Borders = BorderSide.Bottom, BorderColor = CizgiRengi,
            };
            if (baslik)
            {
                hucre.Text = sutun.Baslik;
                hucre.Font = new DXFont("Segoe UI", 8.5f, DXFontStyle.Bold);
                hucre.BackColor = BaslikRengi;
                hucre.ForeColor = Color.White;
            }
            else
            {
                hucre.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"[H{i}]"));
            }

            satir.Cells.Add(hucre);
        }

        var tablo = new XRTable { LocationF = new PointF(0, 0), SizeF = new SizeF(genislik, baslik ? 24 : 20) };
        tablo.Rows.Add(satir);
        return tablo;
    }

    private static ReportFooterBand AltBilgi(TabloRaporu rapor, float genislik)
    {
        var bant = new ReportFooterBand();
        var y = 10f;
        if (rapor.Satirlar.Count == 0)
        {
            bant.Controls.Add(new XRLabel { Text = "Bu rapor için kayıt yok.", LocationF = new PointF(0, y), SizeF = new SizeF(genislik, 18), ForeColor = Color.Gray });
            y += 22;
        }

        foreach (var ozet in rapor.Ozet)
        {
            bant.Controls.Add(new XRLabel { Text = ozet, LocationF = new PointF(0, y), SizeF = new SizeF(genislik, 18), Multiline = true, CanGrow = true });
            y += 18;
        }

        if (rapor.ImzaAlanlari.Count > 0)
        {
            y += 24;
            var imzaGenislik = genislik / rapor.ImzaAlanlari.Count;
            for (var i = 0; i < rapor.ImzaAlanlari.Count; i++)
            {
                bant.Controls.Add(new XRLabel
                {
                    Text = $"{rapor.ImzaAlanlari[i]}\nAd soyad / imza:", LocationF = new PointF(i * imzaGenislik, y),
                    SizeF = new SizeF(imzaGenislik - 20, 50), Multiline = true, Borders = BorderSide.Top, BorderColor = Color.Black,
                    Padding = new PaddingInfo(2, 2, 4, 2),
                });
            }

            y += 56;
        }

        bant.HeightF = y + 4;
        return bant;
    }

    private static PageFooterBand SayfaAltligi(TabloRaporu rapor, float genislik)
    {
        var hazirlayan = new XRLabel
        {
            Text = $"Hazırlayan: {rapor.Hazirlayan} · {rapor.HazirlanmaZamani:dd.MM.yyyy HH:mm}", LocationF = new PointF(0, 4),
            SizeF = new SizeF(genislik - 120, 16), ForeColor = Color.Gray,
        };
        var sayfa = new XRPageInfo
        {
            PageInfo = PageInfo.NumberOfTotal, TextFormatString = "Sayfa {0} / {1}", LocationF = new PointF(genislik - 120, 4),
            SizeF = new SizeF(120, 16), TextAlignment = TextAlignment.MiddleRight, ForeColor = Color.Gray,
        };
        return new PageFooterBand { HeightF = 24, Controls = { hazirlayan, sayfa } };
    }
}
