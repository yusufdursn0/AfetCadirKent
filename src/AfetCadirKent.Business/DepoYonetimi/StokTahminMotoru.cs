namespace AfetCadirKent.Business.DepoYonetimi;

/// <summary>Tek bir yeterlilik tahmini. Hesaplanamıyorsa <see cref="YeterlilikGunu"/> null ve <see cref="Aciklama"/> nedenini söyler.</summary>
public sealed record YeterlilikTahmini(decimal? GunlukMiktar, decimal? YeterlilikGunu, string Aciklama)
{
    public bool HesaplanabildiMi => YeterlilikGunu is not null;

    /// <summary>Ör. "12,5 gün" veya "hesaplanamıyor".</summary>
    public string Metin => YeterlilikGunu is { } gun ? $"{gun:0.#} gün" : "hesaplanamıyor";
}

/// <summary>Bir ürünün stok tahmini: tüketime göre ve kişi başı hakka göre iki ayrı tahmin (şartname M7).</summary>
public sealed record StokTahmini
{
    public required int UrunId { get; init; }
    public required string UrunAdi { get; init; }
    public required Birim Birim { get; init; }
    public required decimal KullanilabilirMiktar { get; init; }

    /// <summary>Günlük ortalama tüketime dayanan tahminin hesap dönemi (veri yoksa null).</summary>
    public DateOnly? DonemBaslangici { get; init; }
    public DateOnly? DonemBitisi { get; init; }

    public required YeterlilikTahmini TuketimeGore { get; init; }
    public required YeterlilikTahmini HakaGore { get; init; }

    /// <summary>Ör. "Dönem: 14.10.2026 – 17.10.2026 (4 gün)" veya "Dönem: dağıtım verisi yok".</summary>
    public string DonemMetni => DonemBaslangici is { } bas && DonemBitisi is { } bit
        ? $"Dönem: {bas:dd.MM.yyyy} – {bit:dd.MM.yyyy} ({bit.DayNumber - bas.DayNumber + 1} gün)"
        : "Dönem: dağıtım verisi yok";
}

/// <summary>
/// Stok yeterlilik hesabı (K-049). Veritabanına bağlı değildir.
/// 1) Tüketime göre: kullanılabilir stok ÷ günlük ortalama dağıtım (son 7 gün; dönem ilk dağıtım gününden bugüne).
/// 2) Hakka göre: kullanılabilir stok ÷ (bugünkü nüfusa göre toplam hak ÷ tekrar alma süresi).
/// </summary>
public static class StokTahminMotoru
{
    public const int DonemGunu = 7;

    /// <param name="dagitimlar">Ürünün dağıtım hareketleri (gün, miktar); dönem dışındakiler yok sayılır.</param>
    /// <param name="toplamHak">Hak sahibi tüm ailelerin bir dönemlik toplam hakkı.</param>
    public static StokTahmini Hesapla(Urun urun, decimal kullanilabilir, IEnumerable<(DateOnly Gun, decimal Miktar)> dagitimlar,
        decimal toplamHak, DateOnly bugun)
    {
        var donemIlkGun = bugun.AddDays(-(DonemGunu - 1));
        var donemdekiler = dagitimlar.Where(d => d.Gun >= donemIlkGun && d.Gun <= bugun && d.Miktar > 0).ToList();

        DateOnly? baslangic = donemdekiler.Count > 0 ? donemdekiler.Min(d => d.Gun) : null;
        YeterlilikTahmini tuketim;
        if (baslangic is not { } bas)
        {
            tuketim = new YeterlilikTahmini(null, null, $"Son {DonemGunu} günde dağıtım verisi yok; hesaplanamıyor.");
        }
        else
        {
            var gunSayisi = bugun.DayNumber - bas.DayNumber + 1;
            var gunluk = donemdekiler.Sum(d => d.Miktar) / gunSayisi;
            tuketim = new YeterlilikTahmini(gunluk, Bol(kullanilabilir, gunluk),
                $"Günlük ortalama tüketim {gunluk:0.##} ({gunSayisi} günlük dağıtıma göre).");
        }

        YeterlilikTahmini hak;
        if (toplamHak <= 0)
        {
            hak = new YeterlilikTahmini(null, null, "Hak sahibi aile yok; hesaplanamıyor.");
        }
        else
        {
            var gunluk = toplamHak / Math.Max(urun.TekrarAlmaGunu, 1);
            hak = new YeterlilikTahmini(gunluk, Bol(kullanilabilir, gunluk),
                $"Bugünkü nüfusa göre günlük ihtiyaç {gunluk:0.##} ({urun.HesaplamaTuru.Metin().ToLowerInvariant()} {urun.HakMiktari:0.##}, {Math.Max(urun.TekrarAlmaGunu, 1)} günde bir).");
        }

        return new StokTahmini
        {
            UrunId = urun.Id, UrunAdi = urun.Ad, Birim = urun.Birim, KullanilabilirMiktar = kullanilabilir,
            DonemBaslangici = baslangic, DonemBitisi = baslangic is null ? null : bugun,
            TuketimeGore = tuketim, HakaGore = hak,
        };
    }

    private static decimal Bol(decimal stok, decimal gunluk) => Math.Round(stok / gunluk, 1);
}
