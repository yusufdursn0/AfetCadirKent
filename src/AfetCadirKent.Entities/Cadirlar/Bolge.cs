namespace AfetCadirKent.Entities.Cadirlar;

public class Bolge : VarlikTemel, IAktiflik
{
    private string _kod = string.Empty;
    private string _ad = string.Empty;

    /// <summary>Kısa bölge kodu (ör. "A"); çadır kodunun başında kullanılır.</summary>
    public string Kod
    {
        get => _kod;
        set => _kod = Dogrula.ZorunluMetin(value, "Bölge kodu", 5).ToUpperInvariant();
    }

    public string Ad
    {
        get => _ad;
        set => _ad = Dogrula.ZorunluMetin(value, "Bölge adı", 100);
    }

    public string? Aciklama { get; set; }
    public bool Aktif { get; set; } = true;

    public List<Cadir> Cadirlar { get; set; } = [];

    public override string ToString() => $"{Kod} – {Ad}";
}
