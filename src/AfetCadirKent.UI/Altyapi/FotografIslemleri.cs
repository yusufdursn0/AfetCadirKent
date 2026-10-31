using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using AfetCadirKent.Entities.Kisiler;

namespace AfetCadirKent.UI.Altyapi;

/// <summary>
/// Kişi fotoğrafını şartnamedeki boyuta getirir (M2: 300×300, veritabanında varbinary). Görüntü işleme Windows'a özgü
/// olduğundan arayüz katmanındadır; Business gelen baytları saklar (K-069).
/// </summary>
public static class FotografIslemleri
{
    private const long JpegKalitesi = 85;

    /// <summary>Ortadan kare kırpar, 300×300'e küçültür ve JPEG baytlarına çevirir.</summary>
    public static byte[] KareyeKucult(Image kaynak)
    {
        ArgumentNullException.ThrowIfNull(kaynak);
        var kenar = Math.Min(kaynak.Width, kaynak.Height);
        var kirpma = new Rectangle((kaynak.Width - kenar) / 2, (kaynak.Height - kenar) / 2, kenar, kenar);
        var hedefKenar = Afetzede.FotografKenarPiksel;

        using var hedef = new Bitmap(hedefKenar, hedefKenar);
        using (var g = Graphics.FromImage(hedef))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.DrawImage(kaynak, new Rectangle(0, 0, hedefKenar, hedefKenar), kirpma, GraphicsUnit.Pixel);
        }

        using var akis = new MemoryStream();
        var jpeg = ImageCodecInfo.GetImageEncoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
        using var parametreler = new EncoderParameters(1);
        parametreler.Param[0] = new EncoderParameter(Encoder.Quality, JpegKalitesi);
        hedef.Save(akis, jpeg, parametreler);
        return akis.ToArray();
    }

    /// <summary>Kayıtlı fotoğraf baytlarından görüntü üretir; fotoğraf yoksa null.</summary>
    public static Image? Goruntu(byte[]? baytlar)
    {
        if (baytlar is not { Length: > 0 })
            return null;
        using var akis = new MemoryStream(baytlar);
        using var okunan = Image.FromStream(akis);
        return new Bitmap(okunan); // akış kapandıktan sonra da kullanılabilsin
    }
}
