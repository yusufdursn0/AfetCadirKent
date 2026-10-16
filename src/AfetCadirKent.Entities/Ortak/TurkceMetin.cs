using System.Text;

namespace AfetCadirKent.Entities.Ortak;

/// <summary>
/// Türkçe karakter farklarını (ı/i, ş/s, ç/c, ğ/g, ö/o, ü/u) ve büyük/küçük harfi yok sayan karşılaştırma için
/// metni normalleştirir. Örn. "ŞÜKRÜ Çağlı" → "sukru cagli".
/// </summary>
public static class TurkceMetin
{
    private static readonly Dictionary<char, char> Donusum = new()
    {
        ['ı'] = 'i', ['İ'] = 'i', ['I'] = 'i',
        ['ş'] = 's', ['Ş'] = 's',
        ['ç'] = 'c', ['Ç'] = 'c',
        ['ğ'] = 'g', ['Ğ'] = 'g',
        ['ö'] = 'o', ['Ö'] = 'o',
        ['ü'] = 'u', ['Ü'] = 'u',
        ['â'] = 'a', ['Â'] = 'a',
        ['î'] = 'i', ['Î'] = 'i',
        ['û'] = 'u', ['Û'] = 'u',
    };

    public static string Normallestir(string? metin)
    {
        if (string.IsNullOrWhiteSpace(metin))
            return string.Empty;

        var sb = new StringBuilder(metin.Length);
        var oncekiBosluk = false;
        foreach (var c in metin.Trim())
        {
            if (char.IsWhiteSpace(c))
            {
                if (!oncekiBosluk)
                    sb.Append(' ');
                oncekiBosluk = true;
                continue;
            }

            oncekiBosluk = false;
            sb.Append(Donusum.TryGetValue(c, out var karsilik) ? karsilik : char.ToLowerInvariant(c));
        }

        return sb.ToString();
    }

    public static bool Esit(string? a, string? b) => Normallestir(a) == Normallestir(b);

    public static bool Icerir(string? metin, string? aranan)
    {
        var normalAranan = Normallestir(aranan);
        return normalAranan.Length > 0 && Normallestir(metin).Contains(normalAranan, StringComparison.Ordinal);
    }
}
