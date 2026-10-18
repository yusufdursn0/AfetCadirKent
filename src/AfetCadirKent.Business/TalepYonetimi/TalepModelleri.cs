namespace AfetCadirKent.Business.TalepYonetimi;

/// <summary>Yeni talep. Tür biliniyorsa görevli açarken belirleyebilir; bilinmiyorsa talep "Sınıflandırılmamış" açılır.</summary>
public sealed record TalepBilgisi
{
    public required string Aciklama { get; init; }
    public required TalepKaynagi Kaynak { get; init; }
    public int? AileId { get; init; }

    /// <summary>Boşsa ve ailenin tüm üyeleri tek çadırdaysa o çadır kendiliğinden yazılır.</summary>
    public int? CadirId { get; init; }

    public TalepTuru? Tur { get; init; }
    public DestekOnceligi? YardimOnceligi { get; init; }
}

/// <summary>Talep listesi filtresi; boş alanlar filtrelenmez. Sonuçlar ayrıca role göre süzülür (K-072).</summary>
public sealed record TalepFiltresi
{
    public TalepDurumu? Durum { get; init; }
    public TalepTuru? Tur { get; init; }
    public bool SadeceAcik { get; init; }
    public bool SadeceSiniflandirilmamis { get; init; }
    public int? AileId { get; init; }

    /// <summary>Yalnızca oturumdaki görevliye atanmış talepler.</summary>
    public bool SadeceBanaAtanan { get; init; }
}

/// <summary>Talep ekranındaki satır.</summary>
public sealed record TalepOzeti
{
    public required int Id { get; init; }
    public required string Aciklama { get; init; }
    public TalepTuru? Tur { get; init; }
    public required TalepKaynagi Kaynak { get; init; }
    public required TalepDurumu Durum { get; init; }
    public DestekOnceligi? YardimOnceligi { get; init; }
    public required bool SaglikPersoneliDegerlendirecek { get; init; }
    public RolTuru? HedefRol { get; init; }
    public required SiniflandirmaDurumu SiniflandirmaDurumu { get; init; }

    public int? AileId { get; init; }
    public string? AileNo { get; init; }
    public string? AileAdi { get; init; }
    public int? CadirId { get; init; }
    public string? CadirKodu { get; init; }

    public int? SorumluKullaniciId { get; init; }
    public string? SorumluAdi { get; init; }
    public string? OlusturanAdi { get; init; }

    public required DateTime AcilisZamani { get; init; }
    public DateTime? KapanisZamani { get; init; }
    public string? Sonuc { get; init; }

    public TalepTuru? YzOnerilenTur { get; init; }
    public DestekOnceligi? YzOnerilenOncelik { get; init; }
    public string? YzGerekce { get; init; }

    public bool AcikMi => Durum is not (TalepDurumu.Tamamlandi or TalepDurumu.Iptal);

    public string TurMetni => Tur.Metin();

    /// <summary>"Yardım önceliği: Yüksek", sağlık talebinde "Sağlık personeli değerlendirecek". "Acil" kelimesi kullanılmaz.</summary>
    public string OncelikMetni => SaglikPersoneliDegerlendirecek
        ? "Sağlık personeli değerlendirecek"
        : YardimOnceligi is { } o ? $"Yardım önceliği: {o.Metin()}" : "Yardım önceliği: —";
}

/// <summary>Talebin atanabileceği görevli (hedef roldeki veya Koordinatör, aktif).</summary>
public sealed record AtanabilirGorevli(int Id, string AdSoyad, string KullaniciAdi, RolTuru Rol);
