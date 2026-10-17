namespace AfetCadirKent.Business.Ortak;

/// <summary>
/// Refakatsiz çocuk kaydını yalnızca Koordinatör görür (şartname M2, K-055, K-068). Kuralın tek yeri:
/// Koordinatör dışındaki rollerde refakatsiz çocuklar ve tüm üyeleri refakatsiz çocuk olan aileler "bulunamadı" gibi davranır.
/// </summary>
public static class RefakatsizGizlilik
{
    public static bool RefakatsizCocukGorebilir(this IOturum oturum) => oturum.Rol == RolTuru.Koordinator;

    /// <summary>Ailenin tüm üyeleri refakatsiz çocuk mu? <see cref="Aile.Uyeler"/> yüklenmiş olmalıdır.</summary>
    public static bool YalnizRefakatsizMi(this Aile aile) => aile.Uyeler.Count > 0 && aile.Uyeler.All(u => u.RefakatsizCocuk);

    /// <summary>Bu aile oturumdaki kullanıcıdan gizlenmeli mi?</summary>
    public static bool GizliMi(this IOturum oturum, Aile aile) => !oturum.RefakatsizCocukGorebilir() && aile.YalnizRefakatsizMi();

    /// <exception cref="KayitBulunamadiException">Aile kullanıcıdan gizliyse.</exception>
    public static void AileGorunurOlmali(this IOturum oturum, Aile aile, string kayitAdi = "Aile")
    {
        if (oturum.GizliMi(aile))
            throw new KayitBulunamadiException(kayitAdi);
    }

    /// <exception cref="KayitBulunamadiException">Kişilerden biri kullanıcıdan gizliyse.</exception>
    public static void KisilerGorunurOlmali(this IOturum oturum, IEnumerable<Afetzede> kisiler)
    {
        if (!oturum.RefakatsizCocukGorebilir() && kisiler.Any(k => k.RefakatsizCocuk))
            throw new KayitBulunamadiException("Kişi");
    }

    /// <summary>Veritabanı sorgusunda gizli kişileri süzer.</summary>
    public static IQueryable<Afetzede> GorunurKisiler(this IQueryable<Afetzede> sorgu, IOturum oturum) =>
        oturum.RefakatsizCocukGorebilir() ? sorgu : sorgu.Where(k => !k.RefakatsizCocuk);

    /// <summary>Veritabanı sorgusunda gizli aileleri (tüm üyeleri refakatsiz çocuk) süzer.</summary>
    public static IQueryable<Aile> GorunurAileler(this IQueryable<Aile> sorgu, IOturum oturum) =>
        oturum.RefakatsizCocukGorebilir() ? sorgu : sorgu.Where(a => !(a.Uyeler.Any() && a.Uyeler.All(u => u.RefakatsizCocuk)));
}
