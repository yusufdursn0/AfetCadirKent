namespace AfetCadirKent.Business.CadirYonetimi;

/// <summary>Önerideki bir çadıra kaç kişinin yerleşeceği.</summary>
public sealed record CadirAtamasi(int CadirId, string CadirKodu, int KisiSayisi);

/// <summary>Sistemin çadır önerisi. Görevli onaylamadan yerleşim yapılmaz.</summary>
public sealed record CadirOnerisi(
    IReadOnlyList<CadirAtamasi> Atamalar,
    IReadOnlyList<YerlesimEtkisi> KarsilananEtkiler,
    IReadOnlyList<YerlesimEtkisi> KarsilanmayanEtkiler,
    int BosYatakIsrafi)
{
    public bool TekCadir => Atamalar.Count == 1;

    /// <summary>Ör. "A1-02 (3 kişi) · Sıcak çadır ✔ · Sağlık noktasına yakın ✘ · boş kalacak yatak: 1".</summary>
    public string Ozet
    {
        get
        {
            var parcalar = new List<string> { string.Join(" + ", Atamalar.Select(a => $"{a.CadirKodu} ({a.KisiSayisi} kişi)")) };
            parcalar.AddRange(KarsilananEtkiler.Select(e => $"{e.Metin()} ✔"));
            parcalar.AddRange(KarsilanmayanEtkiler.Select(e => $"{e.Metin()} ✘"));
            parcalar.Add($"boş kalacak yatak: {BosYatakIsrafi}");
            return string.Join(" · ", parcalar);
        }
    }
}

/// <summary>
/// Çadır öneri kuralı (şartname M4). Sıra: kullanılabilir olması → yeterli yer → yerleşim etkisi
/// (erişilebilirlik, sağlık noktasına yakınlık, sıcak/büyük çadır) → en az boş yatak israfı.
/// Tek çadır yetmezse aynı bölgede iki çadırlık öneriler üretilir (bir aile iki çadıra yerleşebilir).
/// Veritabanına bağlı değildir.
/// </summary>
public static class CadirOneriMotoru
{
    public const int EnFazlaOneri = 5;

    /// <summary>Çadır seçimini etkileyen yerleşim etkileri (takip etkileri çadır seçmez).</summary>
    public static readonly IReadOnlySet<YerlesimEtkisi> CadirEtkileri = new HashSet<YerlesimEtkisi>
    {
        YerlesimEtkisi.SicakCadir, YerlesimEtkisi.SaglikNoktasinaYakin, YerlesimEtkisi.ErisilebilirCadir, YerlesimEtkisi.BuyukCadir,
    };

    /// <param name="kisiSayisi">Yerleşecek kişi sayısı.</param>
    /// <param name="istenenEtkiler">Ailenin gerekçelerinden gelen yerleşim etkileri.</param>
    /// <param name="cadirlar">Değerlendirilecek çadırlar (Bolge yüklü olmalı).</param>
    public static IReadOnlyList<CadirOnerisi> OneriUret(int kisiSayisi, IEnumerable<YerlesimEtkisi> istenenEtkiler, IEnumerable<Cadir> cadirlar)
    {
        Dogrula.Pozitif(kisiSayisi, "Yerleşecek kişi sayısı");
        var etkiler = istenenEtkiler.Where(CadirEtkileri.Contains).Distinct().OrderBy(e => e).ToList();

        // 1) Kullanılabilir olması
        var uygunlar = cadirlar.Where(c => c.KullanilabilirMi && c.BosYatak > 0).ToList();

        // 2) Yeterli yer — tek çadır
        var tekli = uygunlar
            .Where(c => c.BosYatak >= kisiSayisi)
            .Select(c => Oneri([(c, kisiSayisi)], etkiler, kisiSayisi))
            .ToList();
        if (tekli.Count > 0)
            return Sirala(tekli).Take(EnFazlaOneri).ToList();

        // Tek çadır yetmiyor: aynı bölgede iki çadır
        var ikili = new List<CadirOnerisi>();
        foreach (var bolge in uygunlar.GroupBy(c => c.BolgeId))
        {
            var liste = bolge.OrderBy(c => c.Sira).ThenBy(c => c.Numara).ToList();
            for (var i = 0; i < liste.Count; i++)
                for (var j = i + 1; j < liste.Count; j++)
                {
                    var (buyuk, kucuk) = liste[i].BosYatak >= liste[j].BosYatak ? (liste[i], liste[j]) : (liste[j], liste[i]);
                    if (buyuk.BosYatak + kucuk.BosYatak < kisiSayisi)
                        continue;
                    var ilkine = Math.Min(buyuk.BosYatak, kisiSayisi);
                    var ikinciye = kisiSayisi - ilkine;
                    if (ikinciye == 0)
                        continue; // tek çadır yeterdi; buraya düşmez ama güvenlik için
                    ikili.Add(Oneri([(buyuk, ilkine), (kucuk, ikinciye)], etkiler, kisiSayisi));
                }
        }

        return Sirala(ikili).Take(EnFazlaOneri).ToList();
    }

    private static CadirOnerisi Oneri(IReadOnlyList<(Cadir Cadir, int Kisi)> secim, List<YerlesimEtkisi> etkiler, int kisiSayisi)
    {
        // Etki, seçilen çadırların hepsi karşılıyorsa karşılanmış sayılır.
        var karsilanan = etkiler.Where(e => secim.All(s => s.Cadir.EtkiyiKarsilarMi(e))).ToList();
        return new CadirOnerisi(
            secim.Select(s => new CadirAtamasi(s.Cadir.Id, s.Cadir.Kod, s.Kisi)).ToList(),
            karsilanan,
            etkiler.Except(karsilanan).ToList(),
            secim.Sum(s => s.Cadir.BosYatak) - kisiSayisi);
    }

    // 3) Yerleşim etkisi uyumu (çok olan önce) → 4) en az boş yatak israfı → kod sırası
    private static IEnumerable<CadirOnerisi> Sirala(IEnumerable<CadirOnerisi> oneriler) =>
        oneriler
            .OrderByDescending(o => o.KarsilananEtkiler.Count)
            .ThenBy(o => o.BosYatakIsrafi)
            .ThenBy(o => o.Atamalar[0].CadirKodu, StringComparer.Ordinal);
}
