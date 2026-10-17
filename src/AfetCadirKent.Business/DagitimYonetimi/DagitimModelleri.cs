namespace AfetCadirKent.Business.DagitimYonetimi;

/// <summary>Dağıtım listeleri ekranındaki bir liste.</summary>
public sealed record DagitimOzeti
{
    public required int Id { get; init; }
    public required int UrunId { get; init; }
    public required string UrunAdi { get; init; }
    public required Birim Birim { get; init; }
    public required DateOnly Tarih { get; init; }
    public required DagitimDurumu Durum { get; init; }
    public string? Aciklama { get; init; }
    public required int SatirSayisi { get; init; }
    public required decimal ToplamHak { get; init; }
    public required decimal ToplamTeslim { get; init; }

    /// <summary>Bu liste için ayrılmış, henüz teslim edilmemiş miktar.</summary>
    public required decimal AcikAyrilan { get; init; }

    public decimal ToplamBekleyen => ToplamHak - ToplamTeslim;
}

/// <summary>Teslim alabilecek aile üyesi.</summary>
public sealed record KisiSecenegi(int KisiId, string KisiNo, string AdSoyad);

/// <summary>Bir satırdaki teslim kaydı: teslim eden, teslim alan, miktar, zaman.</summary>
public sealed record TeslimKaydi(int HareketId, DateTime Zaman, decimal Miktar, string PartiNo, string? TeslimEden, string? TeslimAlan);

/// <summary>Dağıtım listesindeki aile satırı.</summary>
public sealed record DagitimSatiri
{
    public required int DetayId { get; init; }
    public required int AileId { get; init; }
    public required string AileNo { get; init; }
    public required string AileAdi { get; init; }

    /// <summary>Refakatsiz çocuk ailesi: Koordinatör dışındaki rollerde kimlik gizlenir, teslimi Koordinatör yapar (K-068).</summary>
    public required bool KoordinatorTakibinde { get; init; }

    public required int HakSahibiKisiSayisi { get; init; }
    public required decimal HakMiktari { get; init; }
    public required decimal TeslimEdilenMiktar { get; init; }
    public decimal BekleyenHak => HakMiktari - TeslimEdilenMiktar;

    /// <summary>Bu satır için hâlâ ayrılmış miktar (teslim iptali / gün sonunda 0 olur).</summary>
    public required decimal AyrilanMiktar { get; init; }

    /// <summary>Ailenin bu üründe malzemeye özel ihtiyacı var (liste sıralamasında öne alınır).</summary>
    public required bool OncelikliIhtiyac { get; init; }

    public required DestekOnceligi DestekOnceligi { get; init; }
    public string? IstisnaGerekcesi { get; init; }
    public required bool Aktif { get; init; }
    public required IReadOnlyList<KisiSecenegi> TeslimAlabilecekler { get; init; }
    public required IReadOnlyList<TeslimKaydi> Teslimler { get; init; }

    /// <summary>"Malzeme ihtiyacı ✔" gösterimi.</summary>
    public string OncelikMetni => OncelikliIhtiyac ? "Malzeme ihtiyacı ✔" : "—";
}

/// <summary>Bir dağıtım listesi ve satırları. Satırlar: malzemeye özel ihtiyacı olanlar önde, sonra destek önceliği.</summary>
public sealed record DagitimListesi
{
    public required DagitimOzeti Ozet { get; init; }
    public required IReadOnlyList<DagitimSatiri> Satirlar { get; init; }
}

/// <summary>
/// Taslak listeye sonradan eklenebilecek aile. Tekrar alma süresi dolmamışsa <see cref="IstisnaGerekir"/> true'dur
/// ve görevli gerekçe yazmadan ekleyemez.
/// </summary>
public sealed record EklenebilirAile(int AileId, string AileNo, string AileAdi, decimal Hak, bool IstisnaGerekir, DateOnly? SonrakiHakTarihi);

/// <summary>Gün sonu işleminin sonucu.</summary>
public sealed record GunSonuSonucu(int KapananListe, int IptalEdilenTaslak, decimal SerbestBirakilanMiktar);
