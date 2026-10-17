namespace AfetCadirKent.Business.Kayit;

/// <summary>
/// Mükerrer kayıt kuralı (şartname M2):
/// TC aynıysa kayıt engellenir; TC yoksa ad-soyad aynı ve yaş farkı 2'den az olanlar olası eşleşme olarak gösterilir.
/// Karşılaştırmada Türkçe karakter farkları yok sayılır.
/// </summary>
public static class MukerrerKontrol
{
    /// <summary>Yaş farkı bu değerden KÜÇÜK olanlar olası eşleşmedir.</summary>
    public const int YasFarkiSiniri = 2;

    public static IReadOnlyList<Afetzede> OlasiEslesmeler(
        string ad, string soyad, DateOnly dogumTarihi, IEnumerable<Afetzede> adaylar, DateOnly bugun)
    {
        var aramaAdi = TurkceMetin.Normallestir($"{ad} {soyad}");
        var yas = Afetzede.YasHesapla(dogumTarihi, bugun);
        return adaylar
            .Where(k => k.AramaAdi == aramaAdi && Math.Abs(k.Yas(bugun) - yas) < YasFarkiSiniri)
            .ToList();
    }
}
