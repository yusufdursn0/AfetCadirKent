using AfetCadirKent.Business.Raporlama;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace AfetCadirKent.UI.Raporlar;

/// <summary>
/// Aile fişi (şartname M2): kayıt bitince verilen küçük fiş (A6). Aile numarası büyük yazılır; çadır, üyeler ve kiosk giriş bilgisi
/// yer alır. Doğum yılı gibi kişisel bilgi fişe yazılmaz.
/// </summary>
public sealed class AileFisiBelgesi : XtraReport
{
    public AileFisiBelgesi(AileFisi fis)
    {
        ArgumentNullException.ThrowIfNull(fis);
        DisplayName = $"Aile fişi {fis.AileNo}";
        PaperKind = DXPaperKind.A6;
        Margins = new DXMargins(24, 24, 24, 24);
        Font = new DXFont("Segoe UI", 9f);
        var genislik = 413f - Margins.Left - Margins.Right;

        var bant = new DetailBand();
        var y = 0f;

        void Yazi(string metin, float boy, float yukseklik, DXFontStyle stil = DXFontStyle.Regular, TextAlignment hiza = TextAlignment.TopLeft,
            Color? renk = null)
        {
            bant.Controls.Add(new XRLabel
            {
                Text = metin, LocationF = new PointF(0, y), SizeF = new SizeF(genislik, yukseklik), Font = new DXFont("Segoe UI", boy, stil),
                TextAlignment = hiza, Multiline = true, CanGrow = true, ForeColor = renk ?? Color.Black,
            });
            y += yukseklik;
        }

        Yazi("AFET ÇADIR KENT – AİLE FİŞİ", 10, 18, DXFontStyle.Bold, TextAlignment.TopCenter);
        Yazi(fis.AileNo, 26, 44, DXFontStyle.Bold, TextAlignment.TopCenter, Color.FromArgb(31, 78, 121));
        Yazi(fis.AileAdi, 12, 22, DXFontStyle.Bold, TextAlignment.TopCenter);
        Yazi($"Kayıt tarihi: {fis.KayitZamani:dd.MM.yyyy}", 8.5f, 18, hiza: TextAlignment.TopCenter, renk: Color.Gray);
        y += 6;
        Yazi("Çadır", 9, 16, DXFontStyle.Bold);
        Yazi(string.Join("\n", fis.Cadirlar), 11, 18 * fis.Cadirlar.Count);
        y += 4;
        Yazi($"Aile üyeleri ({fis.Uyeler.Count})", 9, 16, DXFontStyle.Bold);
        Yazi(string.Join("\n", fis.Uyeler), 8.5f, 14 * Math.Max(1, fis.Uyeler.Count));
        y += 6;
        Yazi("Kiosk girişi", 9, 16, DXFontStyle.Bold);
        Yazi(fis.KioskBilgisi, 8.5f, 30);
        y += 6;
        Yazi("Bu fişi saklayın. Dağıtım ve taleplerde aile numaranız sorulur.", 8, 26, renk: Color.Gray);
        Yazi($"Yazdırma: {fis.YazdirmaZamani:dd.MM.yyyy HH:mm}", 7.5f, 14, hiza: TextAlignment.TopRight, renk: Color.Gray);

        bant.HeightF = y;
        Bands.Add(bant);
    }
}
