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
/// yeni hak = <see cref="HakHesaplayici"/>; tekrar alma süresi son teslim alınan listenin tarihinden sayılır;
/// en son satır kısmen teslim alındıysa kalanı devreder (hiç teslim alınmayan satır devretmez, aile yeni dönemde yeniden hak alır).
/// İptal edilen listeler ve listeden çıkarılan satırlar hesaba katılmaz.
/// </summary>
internal static class HakDurumlari
{
    public static async Task<Dictionary<int, AileHakDurumu>> HesaplaAsync(
        IVeriOturumu veri, Urun urun, DateOnly bugun, IReadOnlyCollection<int>? aileIdler, CancellationToken iptal)
    {
        var aileSorgu = veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).AsNoTracking().Where(a => a.Aktif);
        var satirSorgu = veri.Repo<DagitimDetay>().Sorgu()
            .Where(d => d.Aktif && d.Dagitim!.Aktif && d.Dagitim.Durum != DagitimDurumu.Iptal && d.Dagitim.UrunId == urun.Id);
        if (aileIdler is not null)
        {
            var idler = aileIdler.Distinct().ToList();
            aileSorgu = aileSorgu.Where(a => idler.Contains(a.Id));
            satirSorgu = satirSorgu.Where(d => idler.Contains(d.AileId));
        }

        var aileler = await aileSorgu.ToListAsync(iptal);
        var satirlar = (await satirSorgu
                .Select(d => new { d.Id, d.AileId, d.Dagitim!.Tarih, d.Dagitim.Durum, d.HakMiktari, d.TeslimEdilenMiktar })
                .ToListAsync(iptal))
            .ToLookup(s => s.AileId);

        var sonuc = new Dictionary<int, AileHakDurumu>();
        foreach (var aile in aileler)
        {
            var aileSatirlari = satirlar[aile.Id].ToList();
            var acikSatir = aileSatirlari.Any(s => s.Durum is DagitimDurumu.Taslak or DagitimDurumu.Onaylandi);
            var teslimliler = aileSatirlari.Where(s => s.TeslimEdilenMiktar > 0).ToList();
            DateOnly? sonTeslim = teslimliler.Count > 0 ? teslimliler.Max(s => s.Tarih) : null;

            var enSon = aileSatirlari.Where(s => s.Durum == DagitimDurumu.Kapandi)
                .OrderByDescending(s => s.Tarih).ThenByDescending(s => s.Id).FirstOrDefault();
            var devreden = enSon is { TeslimEdilenMiktar: > 0 } ? enSon.HakMiktari - enSon.TeslimEdilenMiktar : 0;

            var tamHak = HakHesaplayici.Hesapla(urun, aile.Uyeler, bugun);
            var yeniHak = HakHesaplayici.TekrarAlabilirMi(urun, sonTeslim, bugun) ? tamHak : 0;
            sonuc[aile.Id] = new AileHakDurumu(aile, tamHak, yeniHak, devreden, sonTeslim,
                HakHesaplayici.SonrakiHakTarihi(urun, sonTeslim, bugun), acikSatir);
        }

        return sonuc;
    }
}
