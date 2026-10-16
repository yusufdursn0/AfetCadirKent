namespace AfetCadirKent.Entities.Kisiler;

/// <summary>
/// Soyut kişi sınıfı. Afetzede, görevli (kullanıcı) ve gönüllünün ortak alanlarını ve doğrulamasını taşır.
/// Veritabanında kendi tablosu yoktur; her alt sınıf ayrı tabloya eşlenir (bkz. PROJECT_DECISIONS K-020).
/// </summary>
public abstract class Kisi : VarlikTemel
{
    private string _ad = string.Empty;
    private string _soyad = string.Empty;
    private string? _telefon;

    public string Ad
    {
        get => _ad;
        set
        {
            _ad = Dogrula.ZorunluMetin(value, "Ad", 50);
            AdSoyadDegisti();
        }
    }

    public string Soyad
    {
        get => _soyad;
        set
        {
            _soyad = Dogrula.ZorunluMetin(value, "Soyad", 50);
            AdSoyadDegisti();
        }
    }

    public string? Telefon
    {
        get => _telefon;
        set => _telefon = Dogrula.Telefon(value);
    }

    public string AdSoyad => $"{Ad} {Soyad}";

    /// <summary>Kişinin türü; her alt sınıf kendi adını döndürür (çok biçimlilik).</summary>
    public abstract string KisiTuru { get; }

    /// <summary>Ad veya soyad değiştiğinde alt sınıfların türetilmiş alanlarını güncellemesi için kanca.</summary>
    protected virtual void AdSoyadDegisti()
    {
    }

    public override string ToString() => $"{AdSoyad} ({KisiTuru})";
}
