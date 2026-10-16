namespace AfetCadirKent.Entities.Ortak;

/// <summary>
/// İş kuralı ihlali. Mesajı kullanıcıya doğrudan gösterilebilecek, teknik olmayan bir metindir.
/// </summary>
public class IsKuraliException : Exception
{
    public IsKuraliException(string mesaj) : base(mesaj) { }
    public IsKuraliException(string mesaj, Exception icHata) : base(mesaj, icHata) { }
}

/// <summary>Girdi doğrulama hatası (boş alan, hatalı TC, geçersiz miktar …).</summary>
public class DogrulamaException : IsKuraliException
{
    public DogrulamaException(string mesaj) : base(mesaj) { }
}
