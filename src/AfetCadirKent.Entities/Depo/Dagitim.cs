namespace AfetCadirKent.Entities.Depo;

/// <summary>Bir ürün için günlük dağıtım listesi. Onaylanınca stoktan ayırma yapılır. Fiziksel olarak silinmez.</summary>
public class Dagitim : VarlikTemel, IFizikselSilinemez
{
    public int UrunId { get; set; }
    public Urun? Urun { get; set; }

    public DateOnly Tarih { get; set; }
    public DagitimDurumu Durum { get; set; } = DagitimDurumu.Taslak;
    public string? Aciklama { get; set; }

    public DateTime OlusturmaZamani { get; set; }
    public int? OlusturanKullaniciId { get; set; }
    public Gorevli? OlusturanKullanici { get; set; }

    public DateTime? OnayZamani { get; set; }
    public int? OnaylayanKullaniciId { get; set; }
    public Gorevli? OnaylayanKullanici { get; set; }

    public bool Aktif { get; set; } = true;

    public List<DagitimDetay> Detaylar { get; set; } = [];
}

/// <summary>
/// Dağıtım listesindeki bir aile satırı: hak, teslim edilen ve bekleyen hak.
/// Kısmi teslimatta kalan miktar bekleyen hak olarak kalır.
/// </summary>
public class DagitimDetay : VarlikTemel, IFizikselSilinemez
{
    private decimal _hakMiktari;

    public int DagitimId { get; set; }
    public Dagitim? Dagitim { get; set; }

    public int AileId { get; set; }
    public Aile? Aile { get; set; }

    public decimal HakMiktari
    {
        get => _hakMiktari;
        set
        {
            Dogrula.Pozitif(value, "Hak miktarı");
            if (value < TeslimEdilenMiktar)
                throw new IsKuraliException("Hak miktarı teslim edilen miktardan az olamaz.");
            _hakMiktari = value;
        }
    }

    public decimal TeslimEdilenMiktar { get; private set; }

    /// <summary>Aile bu ürün için malzemeye özel ihtiyaç sahibi mi (liste sıralamasında öne alınır).</summary>
    public bool OncelikliIhtiyac { get; set; }

    /// <summary>Tekrar alma süresi dolmadan hak tanındıysa görevlinin yazdığı gerekçe.</summary>
    public string? IstisnaGerekcesi { get; set; }

    public bool Aktif { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public List<StokAyirma> Ayirmalar { get; set; } = [];
    public List<DagitimHareketi> Teslimler { get; set; } = [];

    public decimal BekleyenHak => HakMiktari - TeslimEdilenMiktar;

    public bool TamamlandiMi => BekleyenHak <= 0;

    public void TeslimKaydet(decimal miktar)
    {
        Dogrula.Pozitif(miktar, "Teslim miktarı");
        if (miktar > BekleyenHak)
            throw new IsKuraliException($"Teslim miktarı bekleyen haktan ({BekleyenHak}) fazla olamaz.");
        TeslimEdilenMiktar += miktar;
    }
}

/// <summary>Bir dağıtım satırı için bir partiden yapılan ayırma (FEFO ile birden fazla parti olabilir).</summary>
public class StokAyirma : VarlikTemel
{
    public int DagitimDetayId { get; set; }
    public DagitimDetay? DagitimDetay { get; set; }

    public int PartiId { get; set; }
    public Parti? Parti { get; set; }

    public decimal Miktar { get; set; }
    public decimal TeslimEdilenMiktar { get; set; }
    public decimal SerbestBirakilanMiktar { get; set; }

    public AyirmaDurumu Durum { get; set; } = AyirmaDurumu.Acik;
    public DateTime OlusturmaZamani { get; set; }
    public DateTime? KapanisZamani { get; set; }

    /// <summary>Hâlâ ayrılmış duran (teslim edilmemiş, serbest bırakılmamış) miktar.</summary>
    public decimal AcikMiktar => Miktar - TeslimEdilenMiktar - SerbestBirakilanMiktar;
}
