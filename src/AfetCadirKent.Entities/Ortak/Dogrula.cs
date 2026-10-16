namespace AfetCadirKent.Entities.Ortak;

/// <summary>Entity property'lerinde kullanılan ortak doğrulama yardımcıları.</summary>
public static class Dogrula
{
    public static string ZorunluMetin(string? deger, string alanAdi, int enFazlaUzunluk)
    {
        var temiz = deger?.Trim();
        if (string.IsNullOrEmpty(temiz))
            throw new DogrulamaException($"{alanAdi} boş bırakılamaz.");
        if (temiz.Length > enFazlaUzunluk)
            throw new DogrulamaException($"{alanAdi} en fazla {enFazlaUzunluk} karakter olabilir.");
        return temiz;
    }

    public static string? IstegeBagliMetin(string? deger, string alanAdi, int enFazlaUzunluk)
    {
        var temiz = deger?.Trim();
        if (string.IsNullOrEmpty(temiz))
            return null;
        if (temiz.Length > enFazlaUzunluk)
            throw new DogrulamaException($"{alanAdi} en fazla {enFazlaUzunluk} karakter olabilir.");
        return temiz;
    }

    public static string? Telefon(string? deger)
    {
        var temiz = IstegeBagliMetin(deger, "Telefon", 20);
        if (temiz is null)
            return null;
        if (!temiz.All(c => char.IsDigit(c) || c is ' ' or '+' or '(' or ')' or '-'))
            throw new DogrulamaException("Telefon numarası yalnızca rakam, boşluk ve + ( ) - içerebilir.");
        return temiz;
    }

    public static int Pozitif(int deger, string alanAdi)
    {
        if (deger <= 0)
            throw new DogrulamaException($"{alanAdi} sıfırdan büyük olmalıdır.");
        return deger;
    }

    public static int NegatifOlmayan(int deger, string alanAdi)
    {
        if (deger < 0)
            throw new DogrulamaException($"{alanAdi} negatif olamaz.");
        return deger;
    }

    public static decimal Pozitif(decimal deger, string alanAdi)
    {
        if (deger <= 0)
            throw new DogrulamaException($"{alanAdi} sıfırdan büyük olmalıdır.");
        return deger;
    }

    public static decimal NegatifOlmayan(decimal deger, string alanAdi)
    {
        if (deger < 0)
            throw new DogrulamaException($"{alanAdi} negatif olamaz.");
        return deger;
    }
}
