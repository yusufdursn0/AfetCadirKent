namespace AfetCadirKent.Entities.Sistem;

public class Rol : VarlikTemel
{
    public RolTuru Tur { get; set; }
    public string Ad { get; set; } = string.Empty;
    public string? Aciklama { get; set; }

    public List<Yetki> Yetkiler { get; set; } = [];

    public override string ToString() => Ad;
}

/// <summary>Bir rolün bir modüldeki izni (görme / değiştirme / rapor).</summary>
public class Yetki : VarlikTemel
{
    public int RolId { get; set; }
    public Rol? Rol { get; set; }

    public Modul Modul { get; set; }
    public Izin Izin { get; set; }
}

/// <summary>Kiosk ekranında gösterilen duyuru. Bitiş tarihi geçen duyuru gösterilmez.</summary>
public class Duyuru : VarlikTemel, IAktiflik
{
    private string _baslik = string.Empty;
    private string _metin = string.Empty;

    public string Baslik
    {
        get => _baslik;
        set => _baslik = Dogrula.ZorunluMetin(value, "Başlık", 150);
    }

    public string Metin
    {
        get => _metin;
        set => _metin = Dogrula.ZorunluMetin(value, "Duyuru metni", 2000);
    }

    public DateTime BaslangicTarihi { get; set; }
    public DateTime BitisTarihi { get; set; }

    public int? OlusturanKullaniciId { get; set; }
    public Gorevli? OlusturanKullanici { get; set; }

    public bool Aktif { get; set; } = true;

    public bool YayindaMi(DateTime simdi) => Aktif && BaslangicTarihi <= simdi && simdi <= BitisTarihi;
}

/// <summary>Gönüllü vardiyası ve görev ataması.</summary>
public class Vardiya : VarlikTemel
{
    public int GonulluId { get; set; }
    public Gonullu? Gonullu { get; set; }

    public int? BolgeId { get; set; }
    public Bolge? Bolge { get; set; }

    public DateTime Baslangic { get; set; }
    public DateTime Bitis { get; set; }

    public string? GorevTanimi { get; set; }

    public int? TalepId { get; set; }
    public Talep? Talep { get; set; }
}

/// <summary>İşlem geçmişi: kim, ne zaman, neyi oluşturdu/değiştirdi/iptal etti. Tablo: IslemGecmisi.</summary>
public class IslemKaydi : VarlikTemel
{
    public DateTime Zaman { get; set; }

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }

    /// <summary>İşlem anındaki kullanıcı adı (kullanıcı sonradan silinse de okunabilir kalır).</summary>
    public string KullaniciAdi { get; set; } = string.Empty;

    public IslemTuru IslemTuru { get; set; }
    public string VarlikAdi { get; set; } = string.Empty;
    public int? VarlikId { get; set; }
    public string Aciklama { get; set; } = string.Empty;
}
