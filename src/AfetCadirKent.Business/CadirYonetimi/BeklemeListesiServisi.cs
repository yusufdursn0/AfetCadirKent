using AfetCadirKent.Business.Gerekceler;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.CadirYonetimi;

public interface IBeklemeListesiServisi
{
    /// <summary>Yer bulunamayan aileyi listeye alır.</summary>
    Task<BeklemeListesiKaydi> EkleAsync(int aileId, string? aciklama, CancellationToken iptal = default);

    /// <summary>Bekleyen aileler: önce destek önceliği yüksek olan, eşitlikte en uzun bekleyen.</summary>
    Task<IReadOnlyList<BekleyenAile>> ListeleAsync(CancellationToken iptal = default);

    /// <summary>Yeri olan çadıra sığan bekleyen aileler, öneri sırasıyla (ilk eleman sistemin önerisi).</summary>
    Task<IReadOnlyList<BekleyenAile>> CadirIcinOneriAsync(int cadirId, CancellationToken iptal = default);

    /// <summary>Yer açılmış ve listedeki bir aileye uyan çadırlar (her çadır için önerilen aile).</summary>
    Task<IReadOnlyList<YerAcilanCadirOnerisi>> YerAcilanCadirlarAsync(CancellationToken iptal = default);

    /// <summary>Görevli onayı: aile çadıra yerleşir ve listeden çıkar.</summary>
    Task OnaylaVeYerlestirAsync(int kayitId, int cadirId, CancellationToken iptal = default);

    /// <summary>Aileyi yerleştirmeden listeden çıkarır (ör. başka yere gitti). Açıklama zorunlu.</summary>
    Task ListedenCikarAsync(int kayitId, string aciklama, CancellationToken iptal = default);
}

public class BeklemeListesiServisi(
    IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman, GerekceDegerlendirici degerlendirici,
    ICadirServisi cadirServisi) : IBeklemeListesiServisi
{
    public async Task<BeklemeListesiKaydi> EkleAsync(int aileId, string? aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var aile = await veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).FirstOrDefaultAsync(a => a.Id == aileId && a.Aktif, iptal)
            ?? throw new KayitBulunamadiException("Aile");
        oturum.AileGorunurOlmali(aile);

        if ((await Yerlestirici.YerlesmemisUyelerAsync(veri, aileId, iptal)).Count == 0)
            throw new IsKuraliException($"{aile.AileNo} ailesinin çadıra yerleşmemiş üyesi yok.");
        if (await veri.Repo<BeklemeListesiKaydi>().VarMiAsync(b => b.AileId == aileId && b.CikisZamani == null, iptal))
            throw new IsKuraliException($"{aile.AileNo} ailesi zaten bekleme listesinde.");

        var simdi = zaman.Simdi();
        var kayit = new BeklemeListesiKaydi
        {
            AileId = aileId, EklenmeZamani = simdi, Aciklama = Dogrula.IstegeBagliMetin(aciklama, "Açıklama", 500),
            KullaniciId = oturum.KullaniciId,
        };

        veri.Repo<BeklemeListesiKaydi>().Ekle(kayit);
        await veri.KaydetVeGecmiseYazAsync(oturum, simdi, IslemTuru.Olusturma, nameof(BeklemeListesiKaydi), () => kayit.Id,
            () => $"{aile.AileNo} {aile.AileAdi} bekleme listesine alındı.", iptal);
        return kayit;
    }

    public async Task<IReadOnlyList<BekleyenAile>> ListeleAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return BeklemeSiralamasi.Sirala(await BekleyenleriGetirAsync(veri, iptal)).ToList();
    }

    public async Task<IReadOnlyList<BekleyenAile>> CadirIcinOneriAsync(int cadirId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var cadir = await veri.Repo<Cadir>().Sorgu().AsNoTracking().FirstOrDefaultAsync(c => c.Id == cadirId, iptal)
            ?? throw new KayitBulunamadiException("Çadır");
        if (!cadir.KullanilabilirMi || cadir.BosYatak == 0)
            return [];
        return BeklemeSiralamasi.CadiraUyanlar(await BekleyenleriGetirAsync(veri, iptal), cadir.BosYatak).ToList();
    }

    public async Task<IReadOnlyList<YerAcilanCadirOnerisi>> YerAcilanCadirlarAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Gorme);
        List<BekleyenAile> bekleyenler;
        await using (var veri = veriFabrikasi.Ac())
            bekleyenler = await BekleyenleriGetirAsync(veri, iptal);
        if (bekleyenler.Count == 0)
            return [];

        var plan = await cadirServisi.CadirPlaniAsync(iptal: iptal);
        return plan
            .Where(c => c.PlanDurumu is PlanDurumu.Bos or PlanDurumu.KismenDolu)
            .Select(c => (Cadir: c, Aile: BeklemeSiralamasi.CadiraUyanlar(bekleyenler, c.BosYatak).FirstOrDefault()))
            .Where(x => x.Aile is not null)
            .Select(x => new YerAcilanCadirOnerisi(x.Cadir, x.Aile!))
            .ToList();
    }

    public async Task OnaylaVeYerlestirAsync(int kayitId, int cadirId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var kayit = await BekleyenKayitAsync(veri, kayitId, iptal);
        var cadir = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge).FirstOrDefaultAsync(c => c.Id == cadirId, iptal)
            ?? throw new KayitBulunamadiException("Çadır");

        var kisiSayisi = (await Yerlestirici.YerlesmemisUyelerAsync(veri, kayit.AileId, iptal)).Count;
        await Yerlestirici.YerlestirAsync(veri, oturum, kayit.Aile!, [new CadirAtamasi(cadir.Id, cadir.Kod, kisiSayisi)],
            zaman.Simdi(), "Bekleme listesinden görevli onayıyla yerleşim", iptal);
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task ListedenCikarAsync(int kayitId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        var temiz = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);
        await using var veri = veriFabrikasi.Ac();
        var kayit = await BekleyenKayitAsync(veri, kayitId, iptal);

        var simdi = zaman.Simdi();
        kayit.CikisZamani = simdi;
        kayit.Aciklama = $"{kayit.Aciklama} | Listeden çıkarıldı: {temiz}".Trim(' ', '|').Kisalt(500);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Iptal, nameof(BeklemeListesiKaydi), kayit.Id,
            $"{kayit.Aile!.AileNo} bekleme listesinden yerleştirilmeden çıkarıldı. Açıklama: {temiz}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    /// <summary>Bekleyen aileler, destek önceliği ve yerleşmemiş kişi sayısıyla (sırasız).</summary>
    private async Task<BeklemeListesiKaydi> BekleyenKayitAsync(IVeriOturumu veri, int kayitId, CancellationToken iptal)
    {
        var kayit = await veri.Repo<BeklemeListesiKaydi>().Sorgu().Include(b => b.Aile!.Uyeler)
            .FirstOrDefaultAsync(b => b.Id == kayitId && b.CikisZamani == null, iptal)
            ?? throw new KayitBulunamadiException("Bekleme kaydı");
        oturum.AileGorunurOlmali(kayit.Aile!, "Bekleme kaydı");
        return kayit;
    }

    private async Task<List<BekleyenAile>> BekleyenleriGetirAsync(IVeriOturumu veri, CancellationToken iptal)
    {
        var refakatsizGorebilir = oturum.RefakatsizCocukGorebilir();
        var kayitlar = await veri.Repo<BeklemeListesiKaydi>().Sorgu()
            .Where(b => b.CikisZamani == null && b.Aile!.Aktif)
            .Select(b => new
            {
                b.Id, b.AileId, b.Aile!.AileNo, b.Aile.AileAdi, b.EklenmeZamani, b.Aciklama,
                KisiSayisi = b.Aile.Uyeler.Count(k => k.Aktif
                    && (k.Durum == KisiDurumu.Burada || k.Durum == KisiDurumu.Hastanede)
                    && !k.Yerlesimler.Any(y => y.Bitis == null)),
                YalnizRefakatsiz = b.Aile.Uyeler.Any() && b.Aile.Uyeler.All(k => k.RefakatsizCocuk),
            })
            .ToListAsync(iptal);
        kayitlar = kayitlar.Where(k => refakatsizGorebilir || !k.YalnizRefakatsiz).ToList();
        if (kayitlar.Count == 0)
            return [];

        var oncelikler = await GerekceServisi.DegerlendirAsync(
            veri, kayitlar.Select(k => k.AileId).ToList(), zaman.Simdi(), degerlendirici, iptal);

        return kayitlar.Select(k => new BekleyenAile
        {
            KayitId = k.Id, AileId = k.AileId, AileNo = k.AileNo, AileAdi = k.AileAdi, KisiSayisi = k.KisiSayisi,
            DestekOnceligi = oncelikler[k.AileId].DestekOnceligi,
            Gerekceler = string.Join(", ", oncelikler[k.AileId].Gerekceler.Select(g => g.Aciklama)),
            EklenmeZamani = k.EklenmeZamani, Aciklama = k.Aciklama,
        }).ToList();
    }
}
