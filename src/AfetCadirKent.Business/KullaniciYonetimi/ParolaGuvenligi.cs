namespace AfetCadirKent.Business.KullaniciYonetimi;

/// <summary>
/// Parola kuralı ve BCrypt özeti (şartname M1). Düz parola hiçbir yerde saklanmaz ve loglanmaz.
/// </summary>
public static class ParolaGuvenligi
{
    /// <summary>BCrypt maliyet çarpanı (2^11 tur; masaüstü girişte ~0,1–0,3 sn).</summary>
    public const int Maliyet = 11;

    public const int EnAzUzunluk = 8;

    /// <summary>BCrypt 72 bayttan sonrasını yok sayar; Türkçe karakterler 2 bayt olabildiği için sınır 64 karakter.</summary>
    public const int EnFazlaUzunluk = 64;

    /// <summary>Parola giriş ekranlarında gösterilen kural metni (<see cref="KuralaUymali"/> ile aynı kurallar).</summary>
    public static string KuralAciklamasi =>
        $"Parola en az {EnAzUzunluk}, en fazla {EnFazlaUzunluk} karakter olmalı; en az bir harf ve bir rakam içermeli; kullanıcı adıyla aynı olmamalıdır.";

    // Kullanıcı bulunamadığında da aynı sürede cevap vermek için (kullanıcı adı tahmin edilemesin).
    private static readonly Lazy<string> SahteOzet = new(() => BCrypt.Net.BCrypt.HashPassword("zamanlama-esitleme", Maliyet));

    /// <exception cref="DogrulamaException">Parola kurala uymuyorsa (en az 8 karakter, harf ve rakam, kullanıcı adından farklı).</exception>
    public static void KuralaUymali(string? parola, string kullaniciAdi)
    {
        if (string.IsNullOrEmpty(parola) || parola.Length < EnAzUzunluk)
            throw new DogrulamaException($"Parola en az {EnAzUzunluk} karakter olmalıdır.");
        if (parola.Length > EnFazlaUzunluk)
            throw new DogrulamaException($"Parola en fazla {EnFazlaUzunluk} karakter olabilir.");
        if (!parola.Any(char.IsLetter) || !parola.Any(char.IsDigit))
            throw new DogrulamaException("Parola en az bir harf ve bir rakam içermelidir.");
        if (string.Equals(parola.Trim(), kullaniciAdi.Trim(), StringComparison.OrdinalIgnoreCase))
            throw new DogrulamaException("Parola kullanıcı adıyla aynı olamaz.");
    }

    public static string Ozetle(string parola) => BCrypt.Net.BCrypt.HashPassword(parola, Maliyet);

    public static bool Dogrula(string parola, string ozet)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(parola, ozet);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false; // bozuk/geçersiz özet: giriş reddedilir
        }
    }

    /// <summary>Kullanıcı yokken de doğrulama yapılarak cevap süresi eşitlenir.</summary>
    public static void SahteDogrula(string parola) => BCrypt.Net.BCrypt.Verify(parola, SahteOzet.Value);
}
