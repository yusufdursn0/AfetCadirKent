namespace AfetCadirKent.Business.Gerekceler;

/// <summary>Ailede bulunan tek bir gerekçe.</summary>
public sealed record GerekceSonucu(
    GerekceKodu Kod, string Ad, string Aciklama, DestekOnceligi DestekOnceligi, YerlesimEtkisi YerlesimEtkisi,
    bool ElleEklendi);

/// <summary>Bir ürün için malzemeye özel ihtiyaç bilgisi.</summary>
public sealed record MalzemeIhtiyaci(int UrunId, string UrunAdi, bool Oncelikli);

/// <summary>
/// Ailenin iki ayrı bilgisi: A) destek önceliği (en yüksek gerekçe), B) malzemeye özel ihtiyaçlar.
/// "Acil" kelimesi kullanılmaz; bu bir yardım önceliğidir, tıbbi değerlendirme değildir.
/// </summary>
public sealed class AileOncelikSonucu
{
    public required int AileId { get; init; }
    public required DestekOnceligi DestekOnceligi { get; init; }
    public required IReadOnlyList<GerekceSonucu> Gerekceler { get; init; }

    /// <summary>Gerekçelerin yerleşim etkileri (çadır önerisinde kullanılır).</summary>
    public IReadOnlySet<YerlesimEtkisi> YerlesimEtkileri =>
        Gerekceler.Select(g => g.YerlesimEtkisi).Where(e => e != YerlesimEtkisi.Yok).ToHashSet();

    /// <summary>Tüm aktif ürünler için öncelik bilgisi (servis veritabanı eşleşmelerinden doldurur).</summary>
    public IReadOnlyList<MalzemeIhtiyaci> MalzemeIhtiyaclari { get; init; } = [];

    public bool UrunIcinOncelikliMi(int urunId) => MalzemeIhtiyaclari.Any(m => m.UrunId == urunId && m.Oncelikli);

    /// <summary>Ör. "Destek önceliği: Yüksek (0–2 yaş bebek var, hamile birey var)".</summary>
    public string DestekOnceligiMetni =>
        $"Destek önceliği: {DestekOnceligi.Metin()} ({string.Join(", ", Gerekceler.Select(g => g.Aciklama))})";

    /// <summary>Ör. "Malzeme ihtiyaçları: Bebek maması ✔ · Bebek bezi ✔" — öncelikli ürün yoksa "—".</summary>
    public string MalzemeIhtiyaclariMetni
    {
        get
        {
            var oncelikli = MalzemeIhtiyaclari.Where(m => m.Oncelikli).Select(m => $"{m.UrunAdi} ✔").ToList();
            return "Malzeme ihtiyaçları: " + (oncelikli.Count == 0 ? "—" : string.Join(" · ", oncelikli));
        }
    }
}

/// <summary>
/// Kuralları aileye uygular ve görevlinin elle yaptığı ekleme/kaldırmaları işler.
/// Veritabanına bağlı değildir; doğrudan birim testiyle sınanır.
/// </summary>
public class GerekceDegerlendirici
{
    private readonly IReadOnlyList<IGerekceKurali> _kurallar;
    private readonly IGerekceKurali _varsayilan;

    public GerekceDegerlendirici(IEnumerable<IGerekceKurali> kurallar)
    {
        var liste = kurallar.ToList();
        _varsayilan = liste.SingleOrDefault(k => k.Kod == GerekceKodu.OzelDurumYok)
            ?? throw new ArgumentException("'Özel durumu yok' kuralı tanımlı olmalıdır.", nameof(kurallar));
        _kurallar = liste.Where(k => k != _varsayilan).ToList();
    }

    /// <summary>Şartnamedeki 10 kuralın tamamıyla çalışan değerlendirici.</summary>
    public static GerekceDegerlendirici Varsayilan() => new(TumKurallar());

    public static IReadOnlyList<IGerekceKurali> TumKurallar() =>
    [
        new BebekKurali(), new HamileKurali(), new TekerlekliSandalyeKurali(), new KronikHastaKurali(),
        new RefakatsizCocukKurali(), new YasliKurali(), new TekEbeveynKurali(), new KalabalikAileKurali(),
        new UcGundurYardimAlmadiKurali(), new OzelDurumYokKurali(),
    ];

    public IReadOnlyList<IGerekceKurali> Kurallar => [.. _kurallar, _varsayilan];

    /// <param name="baglam">Aile bilgisi.</param>
    /// <param name="elleDegisiklikler">Geçerli elle değişiklikler (kod → eklendi/kaldırıldı).</param>
    public AileOncelikSonucu Degerlendir(AileBaglami baglam, IReadOnlyDictionary<GerekceKodu, GerekceDegisiklikTuru>? elleDegisiklikler = null)
    {
        elleDegisiklikler ??= new Dictionary<GerekceKodu, GerekceDegisiklikTuru>();
        var sonuc = new List<GerekceSonucu>();

        foreach (var kural in _kurallar)
        {
            elleDegisiklikler.TryGetValue(kural.Kod, out var degisiklik);
            var elleVar = elleDegisiklikler.ContainsKey(kural.Kod);

            if (elleVar && degisiklik == GerekceDegisiklikTuru.ElleKaldirildi)
                continue;

            if (kural.UygulanirMi(baglam))
                sonuc.Add(Sonuc(kural, kural.Aciklama(baglam), elleEklendi: false));
            else if (elleVar && degisiklik == GerekceDegisiklikTuru.ElleEklendi)
                sonuc.Add(Sonuc(kural, $"{kural.Ad} (görevli ekledi)", elleEklendi: true));
        }

        if (sonuc.Count == 0)
            sonuc.Add(Sonuc(_varsayilan, _varsayilan.Ad, elleEklendi: false));

        return new AileOncelikSonucu
        {
            AileId = baglam.Aile.Id,
            DestekOnceligi = sonuc.Max(g => g.DestekOnceligi),
            Gerekceler = sonuc.OrderByDescending(g => g.DestekOnceligi).ToList(),
        };
    }

    private static GerekceSonucu Sonuc(IGerekceKurali k, string aciklama, bool elleEklendi) =>
        new(k.Kod, k.Ad, aciklama, k.DestekOnceligi, k.YerlesimEtkisi, elleEklendi);
}
