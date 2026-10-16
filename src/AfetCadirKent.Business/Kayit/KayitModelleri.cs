namespace AfetCadirKent.Business.Kayit;

/// <summary>Kayıt formundan gelen kişi bilgisi. Doğrulama entity property'lerinde yapılır.</summary>
public sealed record KisiBilgisi
{
    public required string Ad { get; init; }
    public required string Soyad { get; init; }
    public required DateOnly DogumTarihi { get; init; }
    public bool DogumTarihiTahmini { get; init; }
    public string? TcKimlikNo { get; init; }
    public Cinsiyet Cinsiyet { get; init; }
    public string? Memleket { get; init; }
    public string? Yakinlik { get; init; }
    public string? Telefon { get; init; }
    public bool Hamile { get; init; }
    public bool TekerlekliSandalye { get; init; }
    public bool KronikHasta { get; init; }
    public bool RefakatsizCocuk { get; init; }
    public string? SaglikNotu { get; init; }

    /// <summary>Arayüzde 300×300'e küçültülmüş fotoğraf (isteğe bağlı).</summary>
    public byte[]? Fotograf { get; init; }
}

/// <summary>Mükerrer kontrol sonucu.</summary>
/// <param name="TcSahibi">Aynı TC ile kayıtlı kişi (varsa kayıt engellenir).</param>
/// <param name="OlasiEslesmeler">Ad-soyad ve yaşı benzeyen kişiler (görevli karar verir).</param>
public sealed record MukerrerSonucu(Afetzede? TcSahibi, IReadOnlyList<Afetzede> OlasiEslesmeler)
{
    public bool KayitEngellenir => TcSahibi is not null;
    public bool UyariVar => OlasiEslesmeler.Count > 0;
}

/// <summary>Kişi listesi filtresi.</summary>
public sealed record KisiFiltresi
{
    /// <summary>Ad-soyad (Türkçe karakter farkı yok sayılır), kişi no veya TC içinde aranır.</summary>
    public string? Metin { get; init; }
    public KisiDurumu? Durum { get; init; }
    public int? AileId { get; init; }
    public bool PasifleriDeGoster { get; init; }
}
