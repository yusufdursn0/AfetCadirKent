namespace AfetCadirKent.Business.DepoYonetimi;

/// <summary>
/// Ürün ayarlarına göre ailenin dağıtım hakkı (şartname M6). Hak sahibi üyeler: aktif ve Burada/Hastanede
/// (hastanedeki kişinin hakkı ailesinde kalır). Veritabanına bağlı değildir.
/// </summary>
public static class HakHesaplayici
{
    /// <summary>Kişi başı: hak sahibi üye × miktar · aile başı: 1 × miktar · bebek başı: 0–2 yaş üye × miktar.</summary>
    public static decimal Hesapla(Urun urun, IEnumerable<Afetzede> uyeler, DateOnly bugun)
    {
        var sahipler = uyeler.Where(u => u.HakSahibiMi).ToList();
        if (sahipler.Count == 0)
            return 0;

        var carpan = urun.HesaplamaTuru switch
        {
            HesaplamaTuru.KisiBasi => sahipler.Count,
            HesaplamaTuru.AileBasi => 1,
            HesaplamaTuru.BebekBasi => sahipler.Count(u => u.BebekMi(bugun)),
            _ => throw new ArgumentOutOfRangeException(nameof(urun), urun.HesaplamaTuru, "Bilinmeyen hesaplama türü."),
        };
        return carpan * urun.HakMiktari;
    }

    /// <summary>
    /// Tekrar alma süresi doldu mu? Son teslim alınan dağıtımın tarihi + süre ≤ bugün ise yeni hak doğar.
    /// Hiç teslim almamış aile için her zaman doludur.
    /// </summary>
    public static bool TekrarAlabilirMi(Urun urun, DateOnly? sonTeslimDonemi, DateOnly bugun) =>
        sonTeslimDonemi is not { } son || son.AddDays(urun.TekrarAlmaGunu) <= bugun;

    /// <summary>Yeni hakkın doğacağı ilk gün (süre dolmuşsa null).</summary>
    public static DateOnly? SonrakiHakTarihi(Urun urun, DateOnly? sonTeslimDonemi, DateOnly bugun) =>
        TekrarAlabilirMi(urun, sonTeslimDonemi, bugun) ? null : sonTeslimDonemi!.Value.AddDays(urun.TekrarAlmaGunu);
}
