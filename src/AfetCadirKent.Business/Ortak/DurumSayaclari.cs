using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.Ortak;

/// <summary>Ana ekran göstergeleri ve günlük özetin ortak sayımları. Yetki kontrolü çağıran serviste yapılır.</summary>
internal static class DurumSayaclari
{
    public sealed record Nufus(int Burada, int Hastanede, int AileSayisi);

    public sealed record Yatak(int ToplamKapasite, int BosYatak, int KullanilamayanCadir);

    /// <summary>Çadır kentte bulunan (aktif, Burada/Hastanede) kişiler ve aileleri.</summary>
    public static async Task<Nufus> NufusAsync(IVeriOturumu veri, CancellationToken iptal)
    {
        var buradakiler = veri.Repo<Afetzede>().Sorgu()
            .Where(k => k.Aktif && (k.Durum == KisiDurumu.Burada || k.Durum == KisiDurumu.Hastanede));
        var kisiler = await buradakiler.GroupBy(k => k.Durum).Select(g => new { Durum = g.Key, Sayi = g.Count() })
            .ToListAsync(iptal);
        var aileSayisi = await buradakiler.Select(k => k.AileId).Distinct().CountAsync(iptal);
        return new Nufus(
            kisiler.Where(k => k.Durum == KisiDurumu.Burada).Sum(k => k.Sayi),
            kisiler.Where(k => k.Durum == KisiDurumu.Hastanede).Sum(k => k.Sayi),
            aileSayisi);
    }

    /// <summary>Kullanılabilir çadırların kapasitesi ve boş yatağı; bakımda/kapalı çadır sayısı.</summary>
    public static async Task<Yatak> YatakAsync(IVeriOturumu veri, CancellationToken iptal)
    {
        var cadirlar = await veri.Repo<Cadir>().Sorgu().Where(c => c.Aktif)
            .Select(c => new { c.Durum, c.Kapasite, c.Doluluk }).ToListAsync(iptal);
        var kullanilabilir = cadirlar.Where(c => c.Durum == CadirDurumu.Kullanilabilir).ToList();
        return new Yatak(kullanilabilir.Sum(c => c.Kapasite), kullanilabilir.Sum(c => c.Kapasite - c.Doluluk),
            cadirlar.Count - kullanilabilir.Count);
    }

    public static Task<int> BekleyenAileAsync(IVeriOturumu veri, CancellationToken iptal) =>
        veri.Repo<BeklemeListesiKaydi>().Sorgu().CountAsync(b => b.CikisZamani == null, iptal);
}
