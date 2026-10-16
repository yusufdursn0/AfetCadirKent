namespace AfetCadirKent.Entities.Kisiler;

/// <summary>Çadır kentte kayıtlı afetzede. Tablo: Kisiler.</summary>
public class Afetzede : Kisi, IFizikselSilinemez
{
    public const int BebekYasSiniri = 2;
    public const int YasliYasSiniri = 65;
    public const int FotografKenarPiksel = 300;

    private string? _tcKimlikNo;
    private DateOnly _dogumTarihi;
    private string? _memleket;

    /// <summary>Sistemin verdiği benzersiz kişi numarası (K-000001). Veritabanında Id'den hesaplanır.</summary>
    public string KisiNo { get; private set; } = null!;

    public int AileId { get; set; }
    public Aile? Aile { get; set; }

    /// <summary>İsteğe bağlı; kimliği olmayan kişi de kaydedilebilir.</summary>
    public string? TcKimlikNo
    {
        get => _tcKimlikNo;
        set
        {
            var temiz = value?.Trim();
            if (string.IsNullOrEmpty(temiz))
            {
                _tcKimlikNo = null;
                return;
            }

            if (!TcKimlik.GecerliMi(temiz))
                throw new DogrulamaException("TC kimlik numarası geçersiz. 11 haneli geçerli bir numara girin veya boş bırakın.");
            _tcKimlikNo = temiz;
        }
    }

    public DateOnly DogumTarihi
    {
        get => _dogumTarihi;
        set
        {
            if (value.Year < 1900 || value > DateOnly.FromDateTime(DateTime.Today))
                throw new DogrulamaException("Doğum tarihi geçerli bir tarih olmalıdır (gelecekte olamaz).");
            _dogumTarihi = value;
        }
    }

    /// <summary>Tam tarih bilinmiyorsa true; tarih yaklaşık olarak girilmiştir.</summary>
    public bool DogumTarihiTahmini { get; set; }

    public Cinsiyet Cinsiyet { get; set; }

    public string? Memleket
    {
        get => _memleket;
        set => _memleket = Dogrula.IstegeBagliMetin(value, "Memleket", 50);
    }

    /// <summary>Aile içindeki yakınlık (ör. "Anne", "Çocuk"). İsteğe bağlı.</summary>
    public string? Yakinlik { get; set; }

    public KisiDurumu Durum { get; set; } = KisiDurumu.Burada;

    public bool Hamile { get; set; }
    public bool TekerlekliSandalye { get; set; }
    public bool KronikHasta { get; set; }

    /// <summary>Refakatsiz çocuk kaydını yalnızca Koordinatör görür.</summary>
    public bool RefakatsizCocuk { get; set; }

    /// <summary>Sağlık bilgisi notu; sağlık personeli ve koordinatör görür.</summary>
    public string? SaglikNotu { get; set; }

    /// <summary>300×300 küçültülmüş fotoğraf (isteğe bağlı).</summary>
    public byte[]? Fotograf { get; set; }

    /// <summary>Türkçe karakter farkları yok sayılarak arama/mükerrer kontrolü için normalleştirilmiş "ad soyad".</summary>
    public string AramaAdi { get; private set; } = string.Empty;

    public DateTime KayitZamani { get; set; }
    public bool Aktif { get; set; } = true;

    public List<KisiHareketi> Hareketler { get; set; } = [];
    public List<Yerlesim> Yerlesimler { get; set; } = [];

    public override string KisiTuru => "Afetzede";

    public int Yas(DateOnly bugun)
    {
        var yas = bugun.Year - DogumTarihi.Year;
        if (DogumTarihi > bugun.AddYears(-yas))
            yas--;
        return Math.Max(yas, 0);
    }

    public bool BebekMi(DateOnly bugun) => Yas(bugun) <= BebekYasSiniri;

    public bool YasliMi(DateOnly bugun) => Yas(bugun) >= YasliYasSiniri;

    /// <summary>Çadır kentte sayılan üye: aktif ve ayrılmamış/nakil edilmemiş.</summary>
    public bool CadirKenttemi => Aktif && Durum is KisiDurumu.Burada or KisiDurumu.Hastanede;

    /// <summary>
    /// Dağıtım hakkına sayılır mı? Hastanedeki kişinin hakkı ailesinde kalır.
    /// </summary>
    public bool HakSahibiMi => CadirKenttemi;

    protected override void AdSoyadDegisti() => AramaAdi = TurkceMetin.Normallestir($"{Ad} {Soyad}");
}
