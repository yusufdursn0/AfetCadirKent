using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.DepoYonetimi;

/// <summary>Bir ailenin bir üründeki hak durumu.</summary>
/// <param name="TamHak">Bugünkü nüfusa göre bir dönemlik hak (tekrar alma süresine bakılmaz).</param>
/// <param name="YeniHak">Tekrar alma süresi dolduysa tam hak, dolmadıysa 0.</param>
/// <param name="Devreden">Son kısmi teslimattan kalan bekleyen hak (K-046).</param>
/// <param name="AcikSatirVar">Aile bu ürün için taslak veya onaylı bir listede zaten var.</param>
internal sealed record AileHakDurumu(
    Aile Aile, decimal TamHak, decimal YeniHak, decimal Devreden, DateOnly? SonTeslimDonemi, DateOnly? SonrakiHakTarihi,
    bool AcikSatirVar)
{
    /// <summary>Yeni bir dağıtım listesine girecek miktar.</summary>
    public decimal ListeHakki => AcikSatirVar ? 0 : YeniHak + Devreden;

    /// <summary>Görevli istisnasıyla tanınabilecek miktar (süre dolmamış olsa bile).</summary>
    public decimal IstisnaHakki => AcikSatirVar ? 0 : TamHak + Devreden;
}

/// <summary>
/// Hak durumlarını veritabanından toplu hesaplar. Kurallar:
/// yeni hak = <see cref="HakHesaplayici"/>; tekrar alma süresi, yeni haktan teslim alınan son listenin tarihinden sayılır
/// (yalnızca devreden kalanın teslimi süreyi yeniden başlatmaz); devreden = en son kapanan satırın
/// <see cref="DagitimDetay.Devredecek"/> değeri (ödenmemiş devreden hiç kaybolmaz).
/// İptal edilen listeler ve listeden çıkarılan satırlar hesaba katılmaz (K-046).
/// Aileden ayrılan aile, kaynak ailenin ayırmadan önceki teslimlerini tekrar alma süresi için devralır; kaynak ailenin
/// ayırmadan önce hazırlanmış açık satırı ayrılanların payını da içerdiğinden yeni aile için de açık satır sayılır (K-084).
/// </summary>
internal static class HakDurumlari
{
    public static async Task<Dictionary<int, AileHakDurumu>> HesaplaAsync(
        IVeriOturumu veri, Urun urun, DateOnly bugun, IReadOnlyCollection<int>? aileIdler, CancellationToken iptal)
    {
        var kaynaklar = await AyirmaKaynaklariAsync(veri, iptal);
        var aileSorgu = veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).AsNoTracking().Where(a => a.Aktif);
        var satirSorgu = veri.Repo<DagitimDetay>().Sorgu()
            .Where(d => d.Aktif && d.Dagitim!.Aktif && d.Dagitim.Durum != DagitimDurumu.Iptal && d.Dagitim.UrunId == urun.Id);
        if (aileIdler is not null)
        {
            var idler = aileIdler.Distinct().ToList();
            var satirAileleri = idler.Concat(idler.SelectMany(id => Atalar(id, kaynaklar).Select(a => a.AileId))).Distinct().ToList();
            aileSorgu = aileSorgu.Where(a => idler.Contains(a.Id));
            satirSorgu = satirSorgu.Where(d => satirAileleri.Contains(d.AileId));
        }

        var aileler = await aileSorgu.ToListAsync(iptal);
        var satirlar = (await satirSorgu
                .Select(d => new
                {
                    d.Id, d.AileId, d.Dagitim!.Tarih, d.Dagitim.Durum, d.Dagitim.OlusturmaZamani, d.HakMiktari, d.DevredenMiktar,
                    d.TeslimEdilenMiktar,
                    IlkTeslim = d.Teslimler.Where(t => t.Aktif).Min(t => (DateTime?)t.Zaman),
                })
                .ToListAsync(iptal))
            .ToLookup(s => s.AileId);

        var sonuc = new Dictionary<int, AileHakDurumu>();
        foreach (var aile in aileler)
        {
            var aileSatirlari = satirlar[aile.Id].ToList();
            var donemler = aileSatirlari.Where(s => DagitimDetay.YeniHakKullanildi(s.DevredenMiktar, s.TeslimEdilenMiktar)).ToList();
            var acikSatir = aileSatirlari.Any(s => s.Durum is DagitimDurumu.Taslak or DagitimDurumu.Onaylandi);

            // Devralınan geçmiş: kaynak ailelerin ayırma anından önceki teslimleri ve o andan önce hazırlanmış açık satırları.
            foreach (var (ataId, sinir) in Atalar(aile.Id, kaynaklar))
            {
                var ataSatirlari = satirlar[ataId].ToList();
                donemler.AddRange(ataSatirlari.Where(s => DagitimDetay.YeniHakKullanildi(s.DevredenMiktar, s.TeslimEdilenMiktar)
                                                          && s.IlkTeslim <= sinir));
                acikSatir |= ataSatirlari.Any(s => (s.Durum is DagitimDurumu.Taslak or DagitimDurumu.Onaylandi) && s.OlusturmaZamani <= sinir);
            }

            DateOnly? sonTeslim = donemler.Count > 0 ? donemler.Max(s => s.Tarih) : null;

            var enSon = aileSatirlari.Where(s => s.Durum == DagitimDurumu.Kapandi)
                .OrderByDescending(s => s.Tarih).ThenByDescending(s => s.Id).FirstOrDefault();
            var devreden = enSon is null ? 0 : DagitimDetay.Devredecek(enSon.HakMiktari, enSon.DevredenMiktar, enSon.TeslimEdilenMiktar);

            var tamHak = HakHesaplayici.Hesapla(urun, aile.Uyeler, bugun);
            var yeniHak = HakHesaplayici.TekrarAlabilirMi(urun, sonTeslim, bugun) ? tamHak : 0;
            sonuc[aile.Id] = new AileHakDurumu(aile, tamHak, yeniHak, devreden, sonTeslim,
                HakHesaplayici.SonrakiHakTarihi(urun, sonTeslim, bugun), acikSatir);
        }

        return sonuc;
    }

    /// <summary>Ayırmayla oluşan aile → (kaynak aile, ayırma zamanı).</summary>
    private static async Task<Dictionary<int, (int KaynakAileId, DateTime Zaman)>> AyirmaKaynaklariAsync(
        IVeriOturumu veri, CancellationToken iptal) =>
        (await veri.Repo<AileGecmisi>().Sorgu().AsNoTracking()
            .Where(g => g.Tur == AileGecmisiTuru.Ayirma)
            .Select(g => new { g.HedefAileId, g.KaynakAileId, g.Zaman })
            .ToListAsync(iptal))
        .GroupBy(g => g.HedefAileId)
        .ToDictionary(g => g.Key, g => g.OrderBy(x => x.Zaman).Select(x => (x.KaynakAileId, x.Zaman)).First());

    /// <summary>
    /// Ailenin ayırma zincirindeki kaynak aileleri ve her birinden devralınan geçmişin son anı. Zincirde geriye gidildikçe
    /// sınır en erken ayırma anıdır (ör. C, B'den; B, A'dan ayrıldıysa C, A'nın ancak A→B ayırmasından önceki geçmişini alır).
    /// </summary>
    internal static IEnumerable<(int AileId, DateTime Sinir)> Atalar(int aileId, IReadOnlyDictionary<int, (int KaynakAileId, DateTime Zaman)> kaynaklar)
    {
        var gorulen = new HashSet<int> { aileId };
        DateTime? sinir = null;
        var id = aileId;
        while (kaynaklar.TryGetValue(id, out var k) && gorulen.Add(k.KaynakAileId))
        {
            sinir = sinir is { } s && s < k.Zaman ? s : k.Zaman;
            yield return (k.KaynakAileId, sinir.Value);
            id = k.KaynakAileId;
        }
    }
}
