namespace AfetCadirKent.Entities.Cadirlar;

/// <summary>
/// Çadır. Doluluk yerleşimlerle birlikte güncellenir ve kapasiteyi asla aşamaz
/// (entity kuralı + veritabanı CHECK kısıtı + RowVersion ile eşzamanlılık koruması).
/// </summary>
public class Cadir : VarlikTemel, IAktiflik
{
    public const int BuyukCadirKapasitesi = 6;

    private int _sira;
    private int _numara;
    private int _kapasite;

    public int BolgeId { get; set; }
    public Bolge? Bolge { get; set; }

    public int Sira
    {
        get => _sira;
        set => _sira = Dogrula.Pozitif(value, "Sıra");
    }

    public int Numara
    {
        get => _numara;
        set => _numara = Dogrula.Pozitif(value, "Numara");
    }

    public int Kapasite
    {
        get => _kapasite;
        set
        {
            Dogrula.Pozitif(value, "Kapasite");
            if (value < Doluluk)
                throw new IsKuraliException($"Kapasite, çadırdaki mevcut kişi sayısından ({Doluluk}) az olamaz.");
            _kapasite = value;
        }
    }

    /// <summary>Çadırda yerleşik kişi sayısı. Yalnızca <see cref="Yerlestir"/> ve <see cref="Bosalt"/> ile değişir.</summary>
    public int Doluluk { get; private set; }

    public CadirDurumu Durum { get; set; } = CadirDurumu.Kullanilabilir;
    public bool Erisilebilir { get; set; }
    public bool SaglikNoktasinaYakin { get; set; }

    /// <summary>"Sıcak çadır" yerleşim etkisi için (bkz. K-027).</summary>
    public bool Isitmali { get; set; }

    public bool Aktif { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public List<Yerlesim> Yerlesimler { get; set; } = [];

    public int BosYatak => Kapasite - Doluluk;

    public bool BuyukMu => Kapasite >= BuyukCadirKapasitesi;

    /// <summary>Yeni yerleşime açık mı? Bölgesi yüklüyse bölgenin de aktif olması gerekir.</summary>
    public bool KullanilabilirMi => Aktif && Durum == CadirDurumu.Kullanilabilir && Bolge?.Aktif != false;

    private string KullanilamamaNedeni =>
        !Aktif ? "pasif"
        : Bolge?.Aktif == false ? "bölgesi pasif"
        : Durum switch { CadirDurumu.Bakimda => "bakımda", CadirDurumu.Kapali => "kapalı", _ => "kullanılabilir" };

    /// <summary>Ekranda gösterilen çadır kodu, ör. "A1-03".</summary>
    public string Kod => $"{Bolge?.Kod}{Sira}-{Numara:00}";

    public bool EtkiyiKarsilarMi(YerlesimEtkisi etki) => etki switch
    {
        YerlesimEtkisi.SicakCadir => Isitmali,
        YerlesimEtkisi.SaglikNoktasinaYakin => SaglikNoktasinaYakin,
        YerlesimEtkisi.ErisilebilirCadir => Erisilebilir,
        YerlesimEtkisi.BuyukCadir => BuyukMu,
        _ => true,
    };

    public void Yerlestir(int kisiSayisi)
    {
        Dogrula.Pozitif(kisiSayisi, "Yerleşecek kişi sayısı");
        if (!KullanilabilirMi)
            throw new IsKuraliException($"{Kod} çadırı şu an kullanılamaz ({KullanilamamaNedeni}).");
        if (kisiSayisi > BosYatak)
            throw new IsKuraliException($"{Kod} çadırında yeterli yer yok. Boş yatak: {BosYatak}, gereken: {kisiSayisi}.");
        Doluluk += kisiSayisi;
    }

    public void Bosalt(int kisiSayisi)
    {
        Dogrula.Pozitif(kisiSayisi, "Çıkacak kişi sayısı");
        if (kisiSayisi > Doluluk)
            throw new IsKuraliException($"{Kod} çadırında {Doluluk} kişi var; {kisiSayisi} kişi çıkarılamaz.");
        Doluluk -= kisiSayisi;
    }
}
