namespace AfetCadirKent.Entities.Kayit;

/// <summary>Aile. Üyeleri afetzedelerdir; bir aile birden fazla çadıra yerleşebilir.</summary>
public class Aile : VarlikTemel, IFizikselSilinemez
{
    private string _aileAdi = string.Empty;

    /// <summary>Sistemin verdiği benzersiz aile numarası (A-000001). Kiosk girişinde kullanılır.</summary>
    public string AileNo { get; private set; } = null!;

    /// <summary>Ekranda gösterilen ad (ör. "Yılmaz Ailesi").</summary>
    public string AileAdi
    {
        get => _aileAdi;
        set => _aileAdi = Dogrula.ZorunluMetin(value, "Aile adı", 100);
    }

    /// <summary>Tek ebeveyn gerekçesi için görevlinin işaretlediği bilgi.</summary>
    public bool TekEbeveyn { get; set; }

    public string? Notlar { get; set; }
    public DateTime KayitZamani { get; set; }
    public bool Aktif { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public List<Afetzede> Uyeler { get; set; } = [];
    public List<AileGerekcesi> GerekceDegisiklikleri { get; set; } = [];

    /// <summary>Çadır kentte bulunan (aktif, ayrılmamış) üyeler.</summary>
    public IEnumerable<Afetzede> CadirKenttekiUyeler => Uyeler.Where(u => u.CadirKenttemi);

    public override string ToString() => $"{AileNo} – {AileAdi}";
}
