namespace AfetCadirKent.Business.Gerekceler;

/// <summary>Bir ailenin gerekçe değerlendirmesi için gereken bilgiler.</summary>
/// <param name="Aile">Değerlendirilen aile.</param>
/// <param name="Uyeler">Çadır kentte bulunan üyeler (Burada veya Hastanede, aktif).</param>
/// <param name="Simdi">Değerlendirme anı.</param>
/// <param name="SonTeslimZamani">Ailenin herhangi bir üründe son teslim aldığı zaman (hiç almadıysa null).</param>
public sealed record AileBaglami(Aile Aile, IReadOnlyList<Afetzede> Uyeler, DateTime Simdi, DateTime? SonTeslimZamani)
{
    public DateOnly Bugun => DateOnly.FromDateTime(Simdi);
}

/// <summary>
/// Gerekçe kuralı. Her kural kendi destek önceliğini, yerleşim etkisini ve öncelikli malzemelerini döndürür
/// ve aileye uygulanıp uygulanmadığına kendisi karar verir (interface + çok biçimlilik).
/// </summary>
public interface IGerekceKurali
{
    GerekceKodu Kod { get; }
    string Ad { get; }
    DestekOnceligi DestekOnceligi { get; }
    YerlesimEtkisi YerlesimEtkisi { get; }

    /// <summary>Varsayılan öncelikli ürün kategorileri (ürün eşleşmeleri Ayarlar'dan düzenlenebilir, bkz. K-041).</summary>
    IReadOnlyCollection<UrunKategorisi> OncelikliKategoriler { get; }

    /// <summary>"Tüm ürünler" önceliği.</summary>
    bool TumUrunler { get; }

    bool UygulanirMi(AileBaglami baglam);

    /// <summary>Ekranda gerekçe listesinde görünen kısa açıklama (ör. "0–2 yaş bebek var").</summary>
    string Aciklama(AileBaglami baglam);
}

/// <summary>Kuralların ortak tabanı: sabit özellikler kurucuda verilir, tespit mantığı alt sınıftadır.</summary>
public abstract class GerekceKuraliTemel(
    GerekceKodu kod, string ad, DestekOnceligi oncelik, YerlesimEtkisi etki,
    UrunKategorisi[]? kategoriler = null, bool tumUrunler = false) : IGerekceKurali
{
    public GerekceKodu Kod { get; } = kod;
    public string Ad { get; } = ad;
    public DestekOnceligi DestekOnceligi { get; } = oncelik;
    public YerlesimEtkisi YerlesimEtkisi { get; } = etki;
    public IReadOnlyCollection<UrunKategorisi> OncelikliKategoriler { get; } = kategoriler ?? [];
    public bool TumUrunler { get; } = tumUrunler;

    public abstract bool UygulanirMi(AileBaglami baglam);

    public virtual string Aciklama(AileBaglami baglam) => Ad;

    /// <summary>Koşulu sağlayan üye sayısına göre "… var" / "N … var" metni.</summary>
    protected static string SayiliAciklama(int sayi, string tekil) => sayi > 1 ? $"{sayi} {tekil} var" : $"{tekil} var";
}
