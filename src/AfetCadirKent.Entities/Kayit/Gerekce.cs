namespace AfetCadirKent.Entities.Kayit;

/// <summary>
/// Gerekçe tanımı (0–2 yaş bebek, hamile …). Tespit mantığı Business katmanındaki gerekçe kurallarındadır;
/// bu tablo gerekçe ↔ öncelikli ürün eşleşmelerini ve elle yapılan değişikliklerin referansını tutar.
/// </summary>
public class Gerekce : VarlikTemel
{
    public GerekceKodu Kod { get; set; }
    public string Ad { get; set; } = string.Empty;
    public DestekOnceligi DestekOnceligi { get; set; }
    public YerlesimEtkisi YerlesimEtkisi { get; set; }

    /// <summary>"Tüm ürünler" önceliği (ör. 3 gündür yardım almadı).</summary>
    public bool TumUrunler { get; set; }

    public List<GerekceUrun> OncelikliUrunler { get; set; } = [];
}

/// <summary>Gerekçe → öncelikli malzeme eşleşmesi.</summary>
public class GerekceUrun
{
    public int GerekceId { get; set; }
    public Gerekce? Gerekce { get; set; }

    public int UrunId { get; set; }
    public Urun? Urun { get; set; }
}

/// <summary>
/// Görevlinin bir aile için gerekçeyi elle eklemesi veya otomatik gerekçeyi kaldırması.
/// Açıklama zorunludur; kayıt değişiklik geçmişi olarak saklanır.
/// </summary>
public class AileGerekcesi : VarlikTemel
{
    private string _aciklama = string.Empty;

    public int AileId { get; set; }
    public Aile? Aile { get; set; }

    public int GerekceId { get; set; }
    public Gerekce? Gerekce { get; set; }

    public GerekceDegisiklikTuru Tur { get; set; }

    public string Aciklama
    {
        get => _aciklama;
        set => _aciklama = Dogrula.ZorunluMetin(value, "Açıklama", 500);
    }

    /// <summary>Değişiklik geçerli mi? Yeni bir değişiklikle geri alınınca false olur.</summary>
    public bool Gecerli { get; set; } = true;

    public DateTime Zaman { get; set; }
    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }
}
