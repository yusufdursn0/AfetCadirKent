namespace AfetCadirKent.Business.Ortak;

/// <summary>
/// Giriş yapmış kullanıcı ve izinleri. UI yetkisiz öğeleri gizler; Business her işlemde
/// <see cref="IzinGerekli"/> ile ayrıca kontrol eder.
/// </summary>
public interface IOturum
{
    Gorevli? Kullanici { get; }
    RolTuru? Rol { get; }
    bool AcikMi { get; }
    bool IzinVarMi(Modul modul, Izin izin);

    /// <exception cref="YetkisizIslemException">İzin yoksa.</exception>
    void IzinGerekli(Modul modul, Izin izin);
}

public class Oturum : IOturum
{
    private readonly HashSet<(Modul Modul, Izin Izin)> _izinler = [];

    public Gorevli? Kullanici { get; private set; }
    public RolTuru? Rol { get; private set; }
    public bool AcikMi => Kullanici is not null;

    public void Ac(Gorevli kullanici, RolTuru rol, IEnumerable<Yetki> yetkiler)
    {
        ArgumentNullException.ThrowIfNull(kullanici);
        Kullanici = kullanici;
        Rol = rol;
        _izinler.Clear();
        foreach (var yetki in yetkiler)
            _izinler.Add((yetki.Modul, yetki.Izin));
    }

    public void Kapat()
    {
        Kullanici = null;
        Rol = null;
        _izinler.Clear();
    }

    public bool IzinVarMi(Modul modul, Izin izin) => AcikMi && _izinler.Contains((modul, izin));

    public void IzinGerekli(Modul modul, Izin izin)
    {
        if (!AcikMi)
            throw new YetkisizIslemException("Bu işlem için giriş yapmalısınız.");
        if (!IzinVarMi(modul, izin))
            throw new YetkisizIslemException();
    }
}
