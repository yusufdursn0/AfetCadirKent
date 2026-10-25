using System.Text.Json;
using System.Text.RegularExpressions;

namespace AfetCadirKent.Business.YapayZeka;

/// <summary>
/// Model cevaplarını doğrulayan saf kurallar (veritabanı ve ağ gerektirmez). Beklenen biçimde olmayan cevap
/// <c>null</c> döner; çağıran taraf talebi "Sınıflandırılmamış" bırakır (REQ-134).
/// </summary>
public static partial class YzCevapAyristirici
{
    public const int GerekceUzunlugu = 1000;
    public const int OzetUzunlugu = 4000;

    /// <summary>
    /// <c>{"tur": "...", "oncelik": "...", "gerekce": "..."}</c> cevabını okur. Tür zorunludur; tanınmayan öncelik boş sayılır.
    /// Sağlık önerisinde öncelik yok sayılır (K-051). Model JSON'u metin veya kod bloğu içine koymuşsa ilk nesne alınır.
    /// </summary>
    public static TalepSiniflandirmaOnerisi? Siniflandirma(string? cevap)
    {
        if (JsonNesnesi(cevap) is not { } kok)
            return null;

        using (kok)
        {
            if (Alan(kok.RootElement, "tur") is not { } turMetni || TurCoz(turMetni) is not { } tur)
                return null;

            var oncelik = tur == TalepTuru.Saglik ? null : Alan(kok.RootElement, "oncelik") is { } o ? OncelikCoz(o) : null;
            var gerekce = Alan(kok.RootElement, "gerekce") is { } g && !string.IsNullOrWhiteSpace(g)
                ? AcilKelimesiniTemizle(g.Trim()).Kisalt(GerekceUzunlugu)
                : null;
            return new TalepSiniflandirmaOnerisi(tur, oncelik, gerekce);
        }
    }

    /// <summary>Özet metni: boşsa okunamadı sayılır; "acil" kelimesi ekrana çıkmaz.</summary>
    public static string? Ozet(string? cevap) =>
        string.IsNullOrWhiteSpace(cevap) ? null : AcilKelimesiniTemizle(cevap.Trim()).Kisalt(OzetUzunlugu);

    /// <summary>"Saglik", "Sağlık", "çadır/arıza" gibi yazımları tanır (Türkçe karakter ve büyük/küçük harf önemsenmez).</summary>
    internal static TalepTuru? TurCoz(string metin) => SadeceHarf(metin) switch
    {
        "saglik" => TalepTuru.Saglik,
        "malzeme" => TalepTuru.Malzeme,
        "cadirariza" or "cadir" or "ariza" => TalepTuru.CadirAriza,
        "diger" => TalepTuru.Diger,
        _ => null,
    };

    internal static DestekOnceligi? OncelikCoz(string metin) => SadeceHarf(metin) switch
    {
        "yuksek" => DestekOnceligi.Yuksek,
        "orta" => DestekOnceligi.Orta,
        "normal" or "dusuk" => DestekOnceligi.Normal,
        _ => null,
    };

    /// <summary>
    /// Ekranlarda "Acil" kelimesi kullanılmaz (şartname M3, K-051): "acil/acilen/aciliyet" kelimeleri
    /// "öncelikli/öncelikle/öncelik" ile değiştirilir.
    /// </summary>
    internal static string AcilKelimesiniTemizle(string metin) => AcilKelimesi().Replace(metin, m =>
    {
        var kelime = TurkceMetin.Normallestir(m.Value);
        var karsilik = kelime.StartsWith("aciliyet", StringComparison.Ordinal) ? "öncelik"
            : kelime.StartsWith("acilen", StringComparison.Ordinal) ? "öncelikle"
            : "öncelikli";
        return char.IsUpper(m.Value[0]) ? char.ToUpper(karsilik[0], Tr) + karsilik[1..] : karsilik;
    });

    private static readonly System.Globalization.CultureInfo Tr = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");

    [GeneratedRegex(@"\b[Aa][Cc][İIıi][Ll]\w*")]
    private static partial Regex AcilKelimesi();

    private static JsonDocument? JsonNesnesi(string? cevap)
    {
        if (string.IsNullOrWhiteSpace(cevap))
            return null;
        var bas = cevap.IndexOf('{');
        var son = cevap.LastIndexOf('}');
        if (bas < 0 || son <= bas)
            return null;

        try
        {
            var belge = JsonDocument.Parse(cevap.AsMemory(bas, son - bas + 1));
            if (belge.RootElement.ValueKind == JsonValueKind.Object)
                return belge;
            belge.Dispose();
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>Alan adını büyük/küçük harf ve Türkçe karakter gözetmeden bulur ("Tür", "tur" …); yalnızca metin değer kabul edilir.</summary>
    private static string? Alan(JsonElement nesne, string ad)
    {
        foreach (var alan in nesne.EnumerateObject())
            if (SadeceHarf(alan.Name) == ad)
                return alan.Value.ValueKind == JsonValueKind.String ? alan.Value.GetString() : null;
        return null;
    }

    private static string SadeceHarf(string metin) =>
        string.Concat(TurkceMetin.Normallestir(metin).Where(char.IsLetter));
}
