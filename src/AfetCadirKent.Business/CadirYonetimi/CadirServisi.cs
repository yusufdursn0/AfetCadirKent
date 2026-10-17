using AfetCadirKent.Business.Gerekceler;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.CadirYonetimi;

public interface ICadirServisi
{
    Task<IReadOnlyList<Bolge>> BolgeleriListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default);

    /// <summary>Yeni bölge ekler (id null) veya mevcut bölgeyi günceller.</summary>
    Task<Bolge> BolgeKaydetAsync(int? id, string kod, string ad, string? aciklama, CancellationToken iptal = default);

    /// <summary>Çadırı olan bölge silinemez; pasif yapılabilir.</summary>
    Task BolgeSilAsync(int id, CancellationToken iptal = default);

    Task BolgeAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default);

    /// <summary>Renkli, tıklanabilir çadır planı verisi.</summary>
    Task<IReadOnlyList<CadirPlanOgesi>> CadirPlaniAsync(int? bolgeId = null, CancellationToken iptal = default);

    Task<Cadir> CadirEkleAsync(CadirBilgisi bilgi, CancellationToken iptal = default);
    Task CadirGuncelleAsync(int id, CadirBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Kullanılmış (yerleşim, hareket veya talep kaydı olan) çadır silinemez; pasif yapılabilir.</summary>
    Task CadirSilAsync(int id, CancellationToken iptal = default);

    Task CadirAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default);

    /// <summary>Ailenin yerleşmemiş üyeleri için çadır önerisi. Sistem önerir, görevli onaylar.</summary>
    Task<YerlesimOnerisiSonucu> OneriGetirAsync(int aileId, CancellationToken iptal = default);

    /// <summary>Görevli onayı: ailenin yerleşmemiş üyelerini seçilen çadır(lar)a yerleştirir.</summary>
    Task YerlestirAsync(int aileId, IReadOnlyList<CadirAtamasi> atamalar, CancellationToken iptal = default);

    /// <summary>Yerleşik kişileri başka çadıra taşır (çadır değişikliği geçmişe yazılır).</summary>
    Task CadirDegistirAsync(IReadOnlyCollection<int> kisiIdler, int yeniCadirId, string aciklama, CancellationToken iptal = default);
}

public class CadirServisi(
    IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman, GerekceDegerlendirici degerlendirici) : ICadirServisi
{
    private int? KullaniciId => oturum.Kullanici is { Id: > 0 } k ? k.Id : null;

    // ---------------------------------------------------------------- Bölge

    public async Task<IReadOnlyList<Bolge>> BolgeleriListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Bolge>().Sorgu().AsNoTracking()
            .Where(b => pasifleriDeGoster || b.Aktif)
            .OrderBy(b => b.Kod)
            .ToListAsync(iptal);
    }

    public async Task<Bolge> BolgeKaydetAsync(int? id, string kod, string ad, string? aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var repo = veri.Repo<Bolge>();

        var bolge = id is { } mevcutId
            ? await repo.BulAsync(mevcutId, iptal) ?? throw new KayitBulunamadiException("Bölge")
            : new Bolge();
        bolge.Kod = kod;
        bolge.Ad = ad;
        bolge.Aciklama = Dogrula.IstegeBagliMetin(aciklama, "Açıklama", 500);

        if (await repo.VarMiAsync(b => b.Kod == bolge.Kod && b.Id != bolge.Id, iptal))
            throw new IsKuraliException($"'{bolge.Kod}' kodlu bir bölge zaten var.");

        await using var islem = await veri.IslemBaslatAsync(iptal);
        if (id is null)
            repo.Ekle(bolge);
        await veri.GuvenliKaydetAsync(iptal);

        veri.IslemKaydet(oturum, zaman.Simdi(), id is null ? IslemTuru.Olusturma : IslemTuru.Guncelleme, nameof(Bolge), bolge.Id,
            $"Bölge {bolge.Kod} – {bolge.Ad} {(id is null ? "eklendi" : "güncellendi")}.");
        await veri.GuvenliKaydetAsync(iptal);
        await islem.CommitAsync(iptal);
        return bolge;
    }

    public async Task BolgeSilAsync(int id, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var bolge = await veri.Repo<Bolge>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Bölge");

        if (await veri.Repo<Cadir>().VarMiAsync(c => c.BolgeId == id, iptal)
            || await veri.Repo<Vardiya>().VarMiAsync(v => v.BolgeId == id, iptal))
            throw new SilinemezException();

        veri.Repo<Bolge>().Sil(bolge);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Silme, nameof(Bolge), id, $"Bölge {bolge.Kod} – {bolge.Ad} silindi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task BolgeAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var bolge = await veri.Repo<Bolge>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Bölge");
        bolge.Aktif = aktif;
        veri.IslemKaydet(oturum, zaman.Simdi(), aktif ? IslemTuru.Guncelleme : IslemTuru.PasifYapma, nameof(Bolge), id,
            $"Bölge {bolge.Kod} {(aktif ? "aktif" : "pasif")} yapıldı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    // ---------------------------------------------------------------- Çadır

    public async Task<IReadOnlyList<CadirPlanOgesi>> CadirPlaniAsync(int? bolgeId = null, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();

        var cadirlar = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge).AsNoTracking()
            .Where(c => bolgeId == null || c.BolgeId == bolgeId)
            .OrderBy(c => c.Bolge!.Kod).ThenBy(c => c.Sira).ThenBy(c => c.Numara)
            .ToListAsync(iptal);

        var refakatsizGorebilir = oturum.RefakatsizCocukGorebilir();
        var cadirIdleri = cadirlar.Select(c => c.Id).ToList();
        var aileler = (await veri.Repo<Yerlesim>().Sorgu()
                .Where(y => y.Bitis == null && cadirIdleri.Contains(y.CadirId)
                            && (refakatsizGorebilir || !y.Kisi!.RefakatsizCocuk))
                .Select(y => new { y.CadirId, y.Kisi!.Aile!.AileNo, y.Kisi.Aile.AileAdi })
                .Distinct()
                .ToListAsync(iptal))
            .GroupBy(x => x.CadirId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<string>)g.Select(x => $"{x.AileNo} {x.AileAdi}").Order().ToList());

        return cadirlar.Select(c => new CadirPlanOgesi
        {
            Id = c.Id, Kod = c.Kod, BolgeId = c.BolgeId, BolgeKodu = c.Bolge!.Kod, BolgeAdi = c.Bolge.Ad,
            Sira = c.Sira, Numara = c.Numara, Kapasite = c.Kapasite, Doluluk = c.Doluluk, Durum = c.Durum, Aktif = c.Aktif,
            Erisilebilir = c.Erisilebilir, SaglikNoktasinaYakin = c.SaglikNoktasinaYakin, Isitmali = c.Isitmali,
            Aileler = aileler.GetValueOrDefault(c.Id) ?? [],
        }).ToList();
    }

    public async Task<Cadir> CadirEkleAsync(CadirBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var cadir = new Cadir();
        await BilgileriUygulaAsync(veri, cadir, bilgi, iptal);

        await using var islem = await veri.IslemBaslatAsync(iptal);
        veri.Repo<Cadir>().Ekle(cadir);
        await veri.GuvenliKaydetAsync(iptal);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Olusturma, nameof(Cadir), cadir.Id,
            $"Çadır {cadir.Kod} eklendi (kapasite {cadir.Kapasite}).");
        await veri.GuvenliKaydetAsync(iptal);
        await islem.CommitAsync(iptal);
        return cadir;
    }

    public async Task CadirGuncelleAsync(int id, CadirBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var cadir = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge).FirstOrDefaultAsync(c => c.Id == id, iptal)
            ?? throw new KayitBulunamadiException("Çadır");

        await BilgileriUygulaAsync(veri, cadir, bilgi, iptal);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Cadir), cadir.Id,
            $"Çadır {cadir.Kod} güncellendi (kapasite {cadir.Kapasite}, durum {cadir.Durum}).");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task CadirSilAsync(int id, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var cadir = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge).FirstOrDefaultAsync(c => c.Id == id, iptal)
            ?? throw new KayitBulunamadiException("Çadır");

        var kullanildi = await veri.Repo<Yerlesim>().VarMiAsync(y => y.CadirId == id, iptal)
            || await veri.Repo<KisiHareketi>().VarMiAsync(h => h.EskiCadirId == id || h.YeniCadirId == id, iptal)
            || await veri.Repo<Talep>().VarMiAsync(t => t.CadirId == id, iptal);
        if (kullanildi)
            throw new SilinemezException();

        veri.Repo<Cadir>().Sil(cadir);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Silme, nameof(Cadir), id, $"Çadır {cadir.Kod} silindi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task CadirAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var cadir = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge).FirstOrDefaultAsync(c => c.Id == id, iptal)
            ?? throw new KayitBulunamadiException("Çadır");
        if (!aktif && cadir.Doluluk > 0)
            throw new IsKuraliException($"{cadir.Kod} çadırında {cadir.Doluluk} kişi var. Önce kişileri başka çadıra taşıyın.");

        cadir.Aktif = aktif;
        veri.IslemKaydet(oturum, zaman.Simdi(), aktif ? IslemTuru.Guncelleme : IslemTuru.PasifYapma, nameof(Cadir), id,
            $"Çadır {cadir.Kod} {(aktif ? "aktif" : "pasif")} yapıldı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    private static async Task BilgileriUygulaAsync(IVeriOturumu veri, Cadir cadir, CadirBilgisi b, CancellationToken iptal)
    {
        var bolge = await veri.Repo<Bolge>().BulAsync(b.BolgeId, iptal) ?? throw new KayitBulunamadiException("Bölge");
        if (await veri.Repo<Cadir>().VarMiAsync(c => c.BolgeId == b.BolgeId && c.Sira == b.Sira && c.Numara == b.Numara && c.Id != cadir.Id, iptal))
            throw new IsKuraliException($"{bolge.Kod}{b.Sira}-{b.Numara:00} numaralı çadır zaten var.");
        if (b.Durum == CadirDurumu.Kapali && cadir.Doluluk > 0)
            throw new IsKuraliException($"Çadırda {cadir.Doluluk} kişi varken çadır kapatılamaz. Önce kişileri başka çadıra taşıyın.");

        cadir.Bolge = bolge;
        cadir.Sira = b.Sira;
        cadir.Numara = b.Numara;
        cadir.Kapasite = b.Kapasite; // doluluğun altına düşürülemez (entity kuralı)
        cadir.Durum = b.Durum;
        cadir.Erisilebilir = b.Erisilebilir;
        cadir.SaglikNoktasinaYakin = b.SaglikNoktasinaYakin;
        cadir.Isitmali = b.Isitmali;
    }

    // ---------------------------------------------------------------- Öneri ve yerleştirme

    public async Task<YerlesimOnerisiSonucu> OneriGetirAsync(int aileId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var aile = await GorunurAileAsync(veri, aileId, iptal);

        var simdi = zaman.Simdi();
        var oncelik = (await GerekceServisi.DegerlendirAsync(veri, [aileId], simdi, degerlendirici, iptal))[aileId];
        var etkiler = oncelik.YerlesimEtkileri.Where(CadirOneriMotoru.CadirEtkileri.Contains).OrderBy(e => e).ToList();
        var kisiSayisi = (await Yerlestirici.YerlesmemisUyelerAsync(veri, aileId, iptal)).Count;

        IReadOnlyList<CadirOnerisi> oneriler = [];
        if (kisiSayisi > 0)
        {
            var cadirlar = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge).AsNoTracking()
                .Where(c => c.Aktif && c.Bolge!.Aktif && c.Durum == CadirDurumu.Kullanilabilir && c.Doluluk < c.Kapasite)
                .ToListAsync(iptal);
            oneriler = CadirOneriMotoru.OneriUret(kisiSayisi, etkiler, cadirlar);
        }

        return new YerlesimOnerisiSonucu
        {
            AileId = aile.Id, AileNo = aile.AileNo, AileAdi = aile.AileAdi, YerlesecekKisiSayisi = kisiSayisi,
            DestekOnceligi = oncelik.DestekOnceligi, IstenenEtkiler = etkiler, Oneriler = oneriler,
        };
    }

    public async Task YerlestirAsync(int aileId, IReadOnlyList<CadirAtamasi> atamalar, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var aile = await GorunurAileAsync(veri, aileId, iptal);
        await Yerlestirici.YerlestirAsync(veri, oturum, aile, atamalar, zaman.Simdi(), "Görevli onayıyla yerleşim", iptal);
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task CadirDegistirAsync(IReadOnlyCollection<int> kisiIdler, int yeniCadirId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Cadir, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Çadır değişikliği açıklaması", 500);
        var idler = kisiIdler?.Distinct().ToList() ?? [];
        if (idler.Count == 0)
            throw new DogrulamaException("Taşınacak en az bir kişi seçilmelidir.");

        await using var veri = veriFabrikasi.Ac();
        var yerlesimler = await veri.Repo<Yerlesim>().Sorgu().Include(y => y.Cadir!.Bolge).Include(y => y.Kisi)
            .Where(y => idler.Contains(y.KisiId) && y.Bitis == null)
            .ToListAsync(iptal);
        if (yerlesimler.Count != idler.Count)
            throw new IsKuraliException("Seçilen kişilerin hepsi şu an bir çadırda yerleşik olmalıdır. Yerleşmemiş kişiler için çadır önerisini kullanın.");
        oturum.KisilerGorunurOlmali(yerlesimler.Select(y => y.Kisi!));

        var yeniCadir = await veri.Repo<Cadir>().Sorgu().Include(c => c.Bolge).FirstOrDefaultAsync(c => c.Id == yeniCadirId, iptal)
            ?? throw new KayitBulunamadiException("Çadır");
        if (yerlesimler.Any(y => y.CadirId == yeniCadirId))
            throw new IsKuraliException($"Seçilen kişilerden bazıları zaten {yeniCadir.Kod} çadırında.");

        var simdi = zaman.Simdi();
        yeniCadir.Yerlestir(yerlesimler.Count);
        foreach (var eski in yerlesimler)
        {
            eski.Bitis = simdi;
            eski.Cadir!.Bosalt(1);
            veri.Repo<Yerlesim>().Ekle(new Yerlesim { KisiId = eski.KisiId, Cadir = yeniCadir, Baslangic = simdi, KullaniciId = KullaniciId });
            veri.Repo<KisiHareketi>().Ekle(new KisiHareketi
            {
                KisiId = eski.KisiId, Tur = KisiHareketTuru.CadirDegisikligi, EskiCadirId = eski.CadirId, YeniCadir = yeniCadir,
                Zaman = simdi, KullaniciId = KullaniciId, Aciklama = temizAciklama,
            });
        }

        veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(Yerlesim), yeniCadirId,
            $"{string.Join(", ", yerlesimler.Select(y => y.Kisi!.KisiNo))} → {yeniCadir.Kod}. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    private async Task<Aile> GorunurAileAsync(IVeriOturumu veri, int aileId, CancellationToken iptal)
    {
        var aile = await veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler)
            .FirstOrDefaultAsync(a => a.Id == aileId && a.Aktif, iptal) ?? throw new KayitBulunamadiException("Aile");
        oturum.AileGorunurOlmali(aile);
        return aile;
    }
}
