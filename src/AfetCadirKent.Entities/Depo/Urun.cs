namespace AfetCadirKent.Entities.Depo;

/// <summary>Ürün tanımı ve dağıtım ayarları (hak, tekrar alma süresi, kritik seviye).</summary>
public class Urun : VarlikTemel, IAktiflik
{
    private string _ad = string.Empty;
    private decimal _hakMiktari;
    private int _tekrarAlmaGunu;
    private decimal _kritikSeviye;

    public string Ad
    {
        get => _ad;
        set => _ad = Dogrula.ZorunluMetin(value, "Ürün adı", 100);
    }

    public UrunKategorisi Kategori { get; set; }
    public Birim Birim { get; set; }
    public HesaplamaTuru HesaplamaTuru { get; set; }

    /// <summary>Hesaplama türüne göre kişi/aile/bebek başına verilen miktar.</summary>
    public decimal HakMiktari
    {
        get => _hakMiktari;
        set => _hakMiktari = Dogrula.Pozitif(value, "Hak miktarı");
    }

    /// <summary>Aynı aileye tekrar verilebilmesi için geçmesi gereken gün sayısı.</summary>
    public int TekrarAlmaGunu
    {
        get => _tekrarAlmaGunu;
        set => _tekrarAlmaGunu = Dogrula.NegatifOlmayan(value, "Tekrar alma süresi");
    }

    public decimal KritikSeviye
    {
        get => _kritikSeviye;
        set => _kritikSeviye = Dogrula.NegatifOlmayan(value, "Kritik seviye");
    }

    public bool Aktif { get; set; } = true;

    public List<Parti> Partiler { get; set; } = [];

    public override string ToString() => Ad;
}

/// <summary>Bağışçı (kurum veya kişi). Stok girişinde seçilir veya oradan eklenir.</summary>
public class Bagisci : VarlikTemel, IAktiflik
{
    private string _ad = string.Empty;
    private string? _telefon;

    public string Ad
    {
        get => _ad;
        set => _ad = Dogrula.ZorunluMetin(value, "Bağışçı adı", 150);
    }

    public string? Telefon
    {
        get => _telefon;
        set => _telefon = Dogrula.Telefon(value);
    }

    public string? Aciklama { get; set; }
    public bool Aktif { get; set; } = true;

    public override string ToString() => Ad;
}
