namespace AfetCadirKent.Entities.Talepler;

/// <summary>
/// Talep (kayıt masası, görevli veya kiosktan). Durum geçişleri yalnızca bu sınıfın metotlarıyla yapılır (K-050):
/// Yeni → Atandı → İşlemde → Tamamlandı; açık talep her durumda İptal edilebilir. Kapatırken sonuç zorunludur.
/// Sağlık taleplerinde yardım önceliği verilmez; "Sağlık personeli değerlendirecek" olarak işaretlenir.
/// </summary>
public class Talep : VarlikTemel
{
    public const int SonucUzunlugu = 1000;

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
    public TalepTuru? Tur { get; private set; }

    public TalepKaynagi Kaynak { get; set; }
    public TalepDurumu Durum { get; private set; } = TalepDurumu.Yeni;

    /// <summary>Yardım önceliği (tıbbi değerlendirme değildir). Sağlık taleplerinde daima boştur.</summary>
    public DestekOnceligi? YardimOnceligi { get; private set; }

    public bool SaglikPersoneliDegerlendirecek { get; private set; }

    /// <summary>Yönlendirilen rol (Sağlık → sağlık personeli, Malzeme → depo …).</summary>
    public RolTuru? HedefRol { get; private set; }

    public int? SorumluKullaniciId { get; private set; }
    public Gorevli? SorumluKullanici { get; set; }

    public DateTime AcilisZamani { get; set; }
    public DateTime? KapanisZamani { get; private set; }

    /// <summary>Tamamlandı/İptal yapılırken zorunlu.</summary>
    public string? Sonuc { get; private set; }

    public SiniflandirmaDurumu SiniflandirmaDurumu { get; private set; } = SiniflandirmaDurumu.Siniflandirilmamis;
    public TalepTuru? YzOnerilenTur { get; private set; }
    public DestekOnceligi? YzOnerilenOncelik { get; private set; }
    public string? YzGerekce { get; private set; }

    public int? OlusturanKullaniciId { get; set; }
    public Gorevli? OlusturanKullanici { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public bool AcikMi => Durum is not (TalepDurumu.Tamamlandi or TalepDurumu.Iptal);

    /// <summary>Yönlendirme (şartname M9): Sağlık → sağlık personeli, Malzeme → depo, Çadır/arıza → saha görevlisi, Diğer → koordinatör.</summary>
    public static RolTuru HedefRolu(TalepTuru tur) => tur switch
    {
        TalepTuru.Saglik => RolTuru.SaglikPersoneli,
        TalepTuru.Malzeme => RolTuru.DepoSorumlusu,
        TalepTuru.CadirAriza => RolTuru.SahaGorevlisi,
        TalepTuru.Diger => RolTuru.Koordinator,
        _ => throw new ArgumentOutOfRangeException(nameof(tur), tur, "Bilinmeyen talep türü."),
    };

    /// <summary>
    /// Türü ve yardım önceliğini belirler, talebi hedef role yönlendirir. Sağlık talebine öncelik verilmez;
    /// görevli öncelik girerse reddedilir. Yönlendirilen rol değişirse önceki atama kalkar ve talep "Yeni"ye döner.
    /// </summary>
    public void Siniflandir(TalepTuru tur, DestekOnceligi? yardimOnceligi, SiniflandirmaDurumu kaynak)
    {
        AcikOlmali();
        if (Durum == TalepDurumu.Islemde)
            throw new IsKuraliException("İşlemdeki talebin türü değiştirilemez.");
        if (kaynak == SiniflandirmaDurumu.Siniflandirilmamis)
            throw new ArgumentOutOfRangeException(nameof(kaynak), "Sınıflandırma kaynağı belirtilmelidir.");

        var saglik = tur == TalepTuru.Saglik;
        if (saglik && yardimOnceligi is not null)
            throw new IsKuraliException("Sağlık taleplerine yardım önceliği verilmez; talebi sağlık personeli değerlendirir.");

        var hedef = HedefRolu(tur);
        if (Durum == TalepDurumu.Atandi && hedef != HedefRol)
        {
            SorumluKullaniciId = null;
            SorumluKullanici = null;
            Durum = TalepDurumu.Yeni;
        }

        Tur = tur;
        HedefRol = hedef;
        YardimOnceligi = saglik ? null : yardimOnceligi;
        SaglikPersoneliDegerlendirecek = saglik;
        SiniflandirmaDurumu = kaynak;
    }

    /// <summary>Yapay zeka önerisi yalnızca açık ve henüz türü belirlenmemiş talep için alınır.</summary>
    public void YapayZekaOnerisiAlinabilirOlmali()
    {
        AcikOlmali();
        if (Tur is not null)
            throw new IsKuraliException("Talep zaten sınıflandırılmış; yapay zeka önerisi yalnızca sınıflandırılmamış talepler için alınır.");
    }

    /// <summary>
    /// Yapay zeka önerisini kaydeder (K-077). Son karar görevlidedir: tür ve öncelik görevli onaylayınca uygulanır.
    /// İstisna: sağlık önerisi beklemeden sağlık personeline yönlendirilir ve önerilen öncelik yok sayılır (K-051).
    /// </summary>
    public void YapayZekaOnerisiKaydet(TalepTuru tur, DestekOnceligi? oncelik, string? gerekce)
    {
        YapayZekaOnerisiAlinabilirOlmali();
        var saglik = tur == TalepTuru.Saglik;
        YzOnerilenTur = tur;
        YzOnerilenOncelik = saglik ? null : oncelik;
        YzGerekce = Dogrula.IstegeBagliMetin(gerekce, "Yapay zeka gerekçesi", SonucUzunlugu);
        if (saglik)
            Siniflandir(TalepTuru.Saglik, null, SiniflandirmaDurumu.YapayZekaOnerdi);
        else
            SiniflandirmaDurumu = SiniflandirmaDurumu.YapayZekaOnerdi;
    }

    /// <summary>Görevli yapay zeka önerisini olduğu gibi onaylar; talep önerilen türe yönlendirilir.</summary>
    public void YapayZekaOnerisiniOnayla()
    {
        if (SiniflandirmaDurumu != SiniflandirmaDurumu.YapayZekaOnerdi || YzOnerilenTur is not { } tur)
            throw new IsKuraliException("Onaylanacak bir yapay zeka önerisi yok.");
        Siniflandir(tur, YzOnerilenOncelik, SiniflandirmaDurumu.GorevliBelirledi);
    }

    /// <summary>Yeni → Atandı (veya atanmış talebi başka görevliye verme).</summary>
    public void Ata(int kullaniciId)
    {
        AcikOlmali();
        if (Tur is null)
            throw new IsKuraliException("Talep atanmadan önce türü belirlenmelidir.");
        if (Durum is not (TalepDurumu.Yeni or TalepDurumu.Atandi))
            throw new IsKuraliException($"'{DurumAdi(Durum)}' durumundaki talep yeniden atanamaz.");
        SorumluKullaniciId = kullaniciId;
        Durum = TalepDurumu.Atandi;
    }

    /// <summary>Atandı → İşlemde.</summary>
    public void IslemeAl()
    {
        if (Durum != TalepDurumu.Atandi)
            throw new IsKuraliException($"Yalnızca atanmış talep işleme alınabilir (şu an: {DurumAdi(Durum)}).");
        Durum = TalepDurumu.Islemde;
    }

    /// <summary>İşlemde → Tamamlandı. Sonuç zorunludur.</summary>
    public void Tamamla(string sonuc, DateTime zaman)
    {
        if (Durum != TalepDurumu.Islemde)
            throw new IsKuraliException($"Yalnızca işlemdeki talep tamamlanabilir (şu an: {DurumAdi(Durum)}).");
        Kapat(TalepDurumu.Tamamlandi, sonuc, zaman);
    }

    /// <summary>Açık talep → İptal. Sonuç (iptal nedeni) zorunludur.</summary>
    public void IptalEt(string sonuc, DateTime zaman)
    {
        AcikOlmali();
        Kapat(TalepDurumu.Iptal, sonuc, zaman);
    }

    private void Kapat(TalepDurumu durum, string sonuc, DateTime zaman)
    {
        Sonuc = Dogrula.ZorunluMetin(sonuc, "Sonuç", SonucUzunlugu);
        Durum = durum;
        KapanisZamani = zaman;
    }

    private void AcikOlmali()
    {
        if (!AcikMi)
            throw new IsKuraliException($"Talep kapanmış ({DurumAdi(Durum)}); değiştirilemez.");
    }

    private static string DurumAdi(TalepDurumu durum) => durum switch
    {
        TalepDurumu.Yeni => "Yeni",
        TalepDurumu.Atandi => "Atandı",
        TalepDurumu.Islemde => "İşlemde",
        TalepDurumu.Tamamlandi => "Tamamlandı",
        TalepDurumu.Iptal => "İptal",
        _ => durum.ToString(),
    };
}

/// <summary>Yakın bulma başvurusu. Çadır ve iletişim bilgisi otomatik paylaşılmaz.</summary>
public class YakinBulmaBasvurusu : VarlikTemel
{
    private string _basvuranAdSoyad = string.Empty;
    private string _arananAd = string.Empty;
    private string _arananSoyad = string.Empty;
    private int? _arananYaklasikYas;
    private string? _arananMemleket;
    private string? _basvuranTelefon;

    public string BasvuranAdSoyad
    {
        get => _basvuranAdSoyad;
        set => _basvuranAdSoyad = Dogrula.ZorunluMetin(value, "Başvuran adı soyadı", 100);
    }

    public string? BasvuranTelefon
    {
        get => _basvuranTelefon;
        set => _basvuranTelefon = Dogrula.Telefon(value);
    }

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

    public int? ArananYaklasikYas
    {
        get => _arananYaklasikYas;
        set => _arananYaklasikYas = value is null or (>= 0 and <= 120)
            ? value
            : throw new DogrulamaException("Aranan kişinin yaklaşık yaşı 0 ile 120 arasında olmalıdır.");
    }

    public string? ArananMemleket
    {
        get => _arananMemleket;
        set => _arananMemleket = Dogrula.IstegeBagliMetin(value, "Memleket", 50);
    }

    public YakinBulmaDurumu Durum { get; private set; } = YakinBulmaDurumu.Acik;

    public int? EslesenKisiId { get; private set; }
    public Afetzede? EslesenKisi { get; set; }

    /// <summary>Doğrulamanın nasıl yapıldığı (fotoğraf tek başına yeterli değildir).</summary>
    public string? DogrulamaAciklamasi { get; private set; }

    public string? Sonuc { get; private set; }

    public DateTime AcilisZamani { get; set; }
    public DateTime? KapanisZamani { get; private set; }

    public bool AcikMi => Durum != YakinBulmaDurumu.Kapandi;

    /// <summary>Açık → Olası eşleşme (veya olası eşleşmede başka adaya geçiş). Doğrulanmış başvuruda eşleşme değişmez.</summary>
    public void EslesmeIsaretle(int kisiId)
    {
        if (Durum is not (YakinBulmaDurumu.Acik or YakinBulmaDurumu.OlasiEslesme))
            throw new IsKuraliException($"Yalnızca açık veya olası eşleşmedeki başvuruda kişi işaretlenebilir (şu an: {DurumAdi(Durum)}).");
        EslesenKisiId = kisiId;
        Durum = YakinBulmaDurumu.OlasiEslesme;
    }

    /// <summary>Olası eşleşme → Açık (yanlış aday). Aramaya devam edilir.</summary>
    public void EslesmeyiKaldir()
    {
        if (Durum != YakinBulmaDurumu.OlasiEslesme)
            throw new IsKuraliException($"Yalnızca olası eşleşmedeki başvurunun eşleşmesi kaldırılabilir (şu an: {DurumAdi(Durum)}).");
        EslesenKisiId = null;
        Durum = YakinBulmaDurumu.Acik;
    }

    /// <summary>
    /// Olası eşleşme → Doğrulandı. Doğrulama yazılı açıklamayla ve kişinin (ya da yanındaki yetişkinin) kendisiyle görüşülerek
    /// yapılır; fotoğraf benzerliği tek başına doğrulama sayılmaz (şartname M8).
    /// </summary>
    public void Dogrulandi(string aciklama, bool kisiyleGorusuldu)
    {
        if (Durum != YakinBulmaDurumu.OlasiEslesme || EslesenKisiId is null)
            throw new IsKuraliException($"Doğrulama için önce olası eşleşen kişi işaretlenmelidir (şu an: {DurumAdi(Durum)}).");
        var metin = Dogrula.ZorunluMetin(aciklama, "Doğrulama açıklaması", 1000);
        if (!kisiyleGorusuldu)
            throw new IsKuraliException("Fotoğraf benzerliği tek başına doğrulama sayılmaz. Bulunan kişiyle (çocuksa yanındaki yetişkinle) " +
                                        "görüşüp kimliği teyit ettikten sonra doğrulayın.");
        DogrulamaAciklamasi = metin;
        Durum = YakinBulmaDurumu.Dogrulandi;
    }

    /// <summary>Herhangi bir açık durum → Kapandı. Sonuç zorunludur.</summary>
    public void Kapat(string sonuc, DateTime zaman)
    {
        if (!AcikMi)
            throw new IsKuraliException("Başvuru zaten kapalı.");
        Sonuc = Dogrula.ZorunluMetin(sonuc, "Sonuç", 1000);
        KapanisZamani = zaman;
        Durum = YakinBulmaDurumu.Kapandi;
    }

    public static string DurumAdi(YakinBulmaDurumu durum) => durum switch
    {
        YakinBulmaDurumu.Acik => "Açık",
        YakinBulmaDurumu.OlasiEslesme => "Olası eşleşme",
        YakinBulmaDurumu.Dogrulandi => "Doğrulandı",
        YakinBulmaDurumu.Kapandi => "Kapandı",
        _ => durum.ToString(),
    };

    public int? KullaniciId { get; set; }
    public Gorevli? Kullanici { get; set; }
}
