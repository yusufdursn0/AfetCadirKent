using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.Ortak;

/// <summary>Servislerin ortak yardımcıları: işlem geçmişi, hata çevirili kaydetme, zaman, gösterim metinleri.</summary>
public static class ServisYardimcilari
{
    /// <summary>İşlem geçmişine (kim, ne zaman, neyi) kayıt ekler; kayıt aynı SaveChanges ile yazılır.</summary>
    public static void IslemKaydet(this IVeriOturumu veri, IOturum oturum, DateTime zaman,
        IslemTuru tur, string varlikAdi, int? varlikId, string aciklama)
    {
        var kullanici = oturum.Kullanici;
        veri.Repo<IslemKaydi>().Ekle(new IslemKaydi
        {
            Zaman = zaman,
            KullaniciId = oturum.KullaniciId,
            KullaniciAdi = kullanici?.KullaniciAdi ?? "sistem",
            IslemTuru = tur,
            VarlikAdi = varlikAdi,
            VarlikId = varlikId,
            Aciklama = aciklama.Kisalt(2000),
        });
    }

    /// <summary>
    /// Bekleyen değişiklikleri ve işlem geçmişi kaydını tek transaction içinde yazar. İki adımlıdır: önce kayıt yazılır
    /// (Id ve aile/kişi numarası gibi veritabanında üretilen değerler oluşur), sonra bu değerleri kullanan işlem geçmişi.
    /// Açıklama bu yüzden ilk kayıttan sonra hesaplanır.
    /// </summary>
    public static async Task KaydetVeGecmiseYazAsync(this IVeriOturumu veri, IOturum oturum, DateTime zaman,
        IslemTuru tur, string varlikAdi, Func<int?> varlikId, Func<string> aciklama, CancellationToken iptal = default)
    {
        await using var islem = await veri.IslemBaslatAsync(iptal);
        await veri.GuvenliKaydetAsync(iptal);
        veri.IslemKaydet(oturum, zaman, tur, varlikAdi, varlikId(), aciklama());
        await veri.GuvenliKaydetAsync(iptal);
        await islem.CommitAsync(iptal);
    }

    /// <summary>
    /// Kaydeder; eşzamanlılık ve tekillik ihlallerini kullanıcıya anlaşılır istisnalara çevirir.
    /// Diğer veritabanı hataları olduğu gibi yükselir (UI loglar ve "kaydedilemedi" der).
    /// </summary>
    public static async Task GuvenliKaydetAsync(this IVeriOturumu veri, CancellationToken iptal = default)
    {
        try
        {
            await veri.KaydetAsync(iptal);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new EszamanlilikException();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new IsKuraliException("Aynı bilgilerle kayıtlı bir kayıt zaten var.", ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2628 or 8152 })
        {
            // Son savunma hattı: alan uzunlukları entity/servis doğrulamasında kontrol edilir.
            throw new DogrulamaException("Girilen metinlerden biri izin verilen uzunluğu aşıyor. Lütfen kısaltıp tekrar deneyin.", ex);
        }
    }

    /// <summary>
    /// Sistemin kendi oluşturduğu birleşik metinleri (ön ek + kullanıcı açıklaması) sütun uzunluğuna sığdırır.
    /// Kullanıcının tek başına girdiği metin kısaltılmaz, <see cref="Dogrula"/> ile reddedilir.
    /// </summary>
    public static string Kisalt(this string metin, int enFazla) =>
        metin.Length <= enFazla ? metin : metin[..(enFazla - 1)] + "…";

    public static DateTime Simdi(this TimeProvider zaman) => zaman.GetLocalNow().DateTime;

    public static DateOnly Bugun(this TimeProvider zaman) => DateOnly.FromDateTime(zaman.GetLocalNow().DateTime);
}

/// <summary>Enum değerlerinin ekranda gösterilen Türkçe karşılıkları.</summary>
public static class Gosterim
{
    /// <summary>Refakatsiz çocuk kayıtları Koordinatör dışındaki rollere bu metinle gösterilir (K-068).</summary>
    public const string GizliKayit = "Koordinatör takibinde";

    public static string Metin(this DestekOnceligi oncelik) => oncelik switch
    {
        DestekOnceligi.Yuksek => "Yüksek",
        DestekOnceligi.Orta => "Orta",
        _ => "Normal",
    };

    public static string Metin(this KisiDurumu durum) => durum switch
    {
        KisiDurumu.Burada => "Burada",
        KisiDurumu.Ayrildi => "Ayrıldı",
        KisiDurumu.Nakil => "Nakil",
        KisiDurumu.Hastanede => "Hastanede",
        _ => durum.ToString(),
    };

    public static string Metin(this YerlesimEtkisi etki) => etki switch
    {
        YerlesimEtkisi.SicakCadir => "Sıcak çadır",
        YerlesimEtkisi.SaglikNoktasinaYakin => "Sağlık noktasına yakın",
        YerlesimEtkisi.ErisilebilirCadir => "Erişilebilir çadır",
        YerlesimEtkisi.KoordinatorTakibi => "Koordinatör takibi",
        YerlesimEtkisi.GorevliTakibi => "Görevli takibi",
        YerlesimEtkisi.BuyukCadir => "Büyük çadır",
        _ => "—",
    };

    public static string Metin(this HesaplamaTuru tur) => tur switch
    {
        HesaplamaTuru.KisiBasi => "Kişi başı",
        HesaplamaTuru.AileBasi => "Aile başı",
        HesaplamaTuru.BebekBasi => "Bebek başı",
        _ => tur.ToString(),
    };

    public static string Metin(this Birim birim) => birim switch
    {
        Birim.Adet => "adet",
        Birim.Paket => "paket",
        Birim.Kg => "kg",
        Birim.Litre => "lt",
        _ => birim.ToString(),
    };

    public static string Metin(this DagitimDurumu durum) => durum switch
    {
        DagitimDurumu.Taslak => "Taslak",
        DagitimDurumu.Onaylandi => "Onaylandı",
        DagitimDurumu.Kapandi => "Kapandı",
        DagitimDurumu.Iptal => "İptal",
        _ => durum.ToString(),
    };
}
