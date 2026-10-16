namespace AfetCadirKent.Entities.Talepler;

/// <summary>
/// Talep (kayıt masası, görevli veya kiosktan). Sağlık taleplerinde yardım önceliği verilmez;
/// "Sağlık personeli değerlendirecek" olarak işaretlenir.
/// </summary>
public class Talep : VarlikTemel
{
    private string _aciklama = string.Empty;

    public int? AileId { get; set; }
    public Aile? Aile { get; set; }

    public int? CadirId { get; set; }
    public Cadir? Cadir { get; set; }

    public string Aciklama
    {
        get => _aciklama;
        set => _aciklama = Dogrula.ZorunluMetin(value, "Talep açıklaması", 1000);
    }

    /// <summary>Boşsa talep henüz sınıflandırılmamıştır.</summary>
    public TalepTuru? Tur { get; set; }

    public TalepKaynagi Kaynak { get; set; }
    public TalepDurumu Durum { get; set; } = TalepDurumu.Yeni;

    /// <summary>Yardım önceliği (tıbbi değerlendirme değildir). Sağlık taleplerinde daima boştur.</summary>
    public DestekOnceligi? YardimOnceligi { get; set; }

    public bool SaglikPersoneliDegerlendirecek { get; set; }

    /// <summary>Yönlendirilen rol (Sağlık → sağlık personeli, Malzeme → depo …).</summary>
    public RolTuru? HedefRol { get; set; }

    public int? SorumluKullaniciId { get; set; }
    public Gorevli? SorumluKullanici { get; set; }

    public DateTime AcilisZamani { get; set; }
    public DateTime? KapanisZamani { get; set; }

    /// <summary>Tamamlandı/İptal yapılırken zorunlu.</summary>
    public string? Sonuc { get; set; }

    public SiniflandirmaDurumu SiniflandirmaDurumu { get; set; } = SiniflandirmaDurumu.Siniflandirilmamis;
    public TalepTuru? YzOnerilenTur { get; set; }
    public DestekOnceligi? YzOnerilenOncelik { get; set; }
    public string? YzGerekce { get; set; }

    public int? OlusturanKullaniciId { get; set; }
    public Gorevli? OlusturanKullanici { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public bool AcikMi => Durum is not (TalepDurumu.Tamamlandi or TalepDurumu.Iptal);
}

/// <summary>Yakın bulma başvurusu. Çadır ve iletişim bilgisi otomatik paylaşılmaz.</summary>
public class YakinBulmaBasvurusu : VarlikTemel
{
    private string _basvuranAdSoyad = string.Empty;
    private string _arananAd = string.Empty;
    private string _arananSoyad = string.Empty;

    public string BasvuranAdSoyad
    {
        get => _basvuranAdSoyad;
        set => _basvuranAdSoyad = Dogrula.ZorunluMetin(value, "Başvuran adı soyadı", 100);
    }

    public string? BasvuranTelefon { get; set; }

    public int? BasvuranAileId { get; set; }
    public Aile? BasvuranAile { get; set; }

    public string ArananAd
    {
        get => _arananAd;
        set => _arananAd = Dogrula.ZorunluMetin(value, "Aranan kişinin adı", 50);
    }

    public string ArananSoyad
    {
        get => _arananSoyad;
        set => _arananSoyad = Dogrula.ZorunluMetin(value, "Aranan kişinin soyadı", 50);
    }

    public int? ArananYaklasikYas { get; set; }
    public string? ArananMemleket { get; set; }

    public YakinBulmaDurumu Durum { get; set; } = YakinBulmaDurumu.Acik;

    public int? EslesenKisiId { get; set; }
    public Afetzede? EslesenKisi { get; set; }

    /// <summary>Doğrulamanın nasıl yapıldığı (fotoğraf tek başına yeterli değildir).</summary>
    public string? DogrulamaAciklamasi { get; set; }

    public string? Sonuc { get; set; }

    public DateTime AcilisZamani { get; set; }
    public DateTime? KapanisZamani { get; set; }

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }
}
