using AfetCadirKent.DataAccess.Konfigurasyonlar;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.DepoYonetimi;

public interface IUrunServisi
{
    Task<IReadOnlyList<Urun>> UrunleriListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default);

    /// <summary>
    /// Yeni ürün ekler (id null) veya ürünü ve ayarlarını (hak, tekrar alma, kritik seviye) günceller.
    /// Yeni üründe ve kategori değiştiğinde gerekçe ↔ ürün eşleşmeleri kategoriye göre kurulur (K-041).
    /// </summary>
    Task<Urun> UrunKaydetAsync(int? id, UrunBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Partisi veya dağıtım listesi olan ürün silinemez; pasif yapılabilir.</summary>
    Task UrunSilAsync(int id, CancellationToken iptal = default);

    Task UrunAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default);

    Task<IReadOnlyList<Bagisci>> BagiscilariListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default);

    /// <summary>Yeni bağışçı ekler (id null) veya günceller. Stok girişi ekranından da çağrılır.</summary>
    Task<Bagisci> BagisciKaydetAsync(int? id, string ad, string? telefon, string? aciklama, CancellationToken iptal = default);

    Task BagisciAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default);
}

public class UrunServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IUrunServisi
{
    public async Task<IReadOnlyList<Urun>> UrunleriListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Urun>().Sorgu().AsNoTracking()
            .Where(u => pasifleriDeGoster || u.Aktif)
            .OrderBy(u => u.Ad)
            .ToListAsync(iptal);
    }

    public async Task<Urun> UrunKaydetAsync(int? id, UrunBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(bilgi);
        await using var veri = veriFabrikasi.Ac();
        var repo = veri.Repo<Urun>();

        var urun = id is { } mevcutId
            ? await repo.BulAsync(mevcutId, iptal) ?? throw new KayitBulunamadiException("Ürün")
            : new Urun();
        var eski = id is null ? null : $"{urun.HesaplamaTuru.Metin()} {urun.HakMiktari:0.##}, {urun.TekrarAlmaGunu} gün, kritik {urun.KritikSeviye:0.##}";
        var kategoriDegisti = id is null || urun.Kategori != bilgi.Kategori;

        urun.Ad = bilgi.Ad;
        urun.Kategori = bilgi.Kategori;
        urun.Birim = bilgi.Birim;
        urun.HesaplamaTuru = bilgi.HesaplamaTuru;
        urun.HakMiktari = bilgi.HakMiktari;
        urun.TekrarAlmaGunu = bilgi.TekrarAlmaGunu;
        urun.KritikSeviye = bilgi.KritikSeviye;

        if (await repo.VarMiAsync(u => u.Ad == urun.Ad && u.Id != urun.Id, iptal))
            throw new IsKuraliException($"'{urun.Ad}' adlı bir ürün zaten var.");

        var yeni = $"{urun.HesaplamaTuru.Metin()} {urun.HakMiktari:0.##}, {urun.TekrarAlmaGunu} gün, kritik {urun.KritikSeviye:0.##}";
        if (id is null)
            repo.Ekle(urun);
        var eslesmeler = kategoriDegisti ? await EslesmeleriKurAsync(veri, urun, iptal) : null;

        var eslesmeMetni = eslesmeler is null ? "" : $" Öncelikli olduğu gerekçeler: {(eslesmeler.Count == 0 ? "yok" : string.Join(", ", eslesmeler))}.";
        await veri.KaydetVeGecmiseYazAsync(oturum, zaman.Simdi(), id is null ? IslemTuru.Olusturma : IslemTuru.Guncelleme,
            nameof(Urun), () => urun.Id,
            () => (id is null ? $"Ürün '{urun.Ad}' eklendi ({yeni})." : $"Ürün '{urun.Ad}' güncellendi. Ayarlar: {eski} → {yeni}.") + eslesmeMetni,
            iptal);
        return urun;
    }

    /// <summary>
    /// Ürünün gerekçe eşleşmelerini kategorisine göre yeniden kurar (ör. Mama → 0–2 yaş bebek).
    /// Eşleşen gerekçe adlarını döndürür.
    /// </summary>
    private static async Task<IReadOnlyList<string>> EslesmeleriKurAsync(IVeriOturumu veri, Urun urun, CancellationToken iptal)
    {
        var repo = veri.Repo<GerekceUrun>();
        if (urun.Id > 0)
            foreach (var eski in await repo.ListeleAsync(e => e.UrunId == urun.Id, iptal))
                repo.Sil(eski);

        var kodlar = ReferansVerisi.OncelikliKategoriler.Where(k => k.Value.Contains(urun.Kategori)).Select(k => k.Key).ToList();
        foreach (var kod in kodlar)
            repo.Ekle(new GerekceUrun { GerekceId = (int)kod, Urun = urun });
        return ReferansVerisi.Gerekceler.Where(g => kodlar.Contains(g.Kod)).Select(g => g.Ad).ToList();
    }

    public async Task UrunSilAsync(int id, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var urun = await veri.Repo<Urun>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Ürün");

        if (await veri.Repo<Parti>().VarMiAsync(p => p.UrunId == id, iptal)
            || await veri.Repo<Dagitim>().VarMiAsync(d => d.UrunId == id, iptal))
            throw new SilinemezException();

        // Gerekçe ↔ ürün eşleşmeleri ayardır; ürünle birlikte silinir (cascade).
        veri.Repo<Urun>().Sil(urun);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Silme, nameof(Urun), id, $"Ürün '{urun.Ad}' silindi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task UrunAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var urun = await veri.Repo<Urun>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Ürün");
        if (!aktif && await veri.Repo<Dagitim>().VarMiAsync(
                d => d.UrunId == id && d.Aktif && (d.Durum == DagitimDurumu.Taslak || d.Durum == DagitimDurumu.Onaylandi), iptal))
            throw new IsKuraliException($"'{urun.Ad}' için açık bir dağıtım listesi var. Önce listeyi kapatın veya iptal edin.");

        urun.Aktif = aktif;
        veri.IslemKaydet(oturum, zaman.Simdi(), aktif ? IslemTuru.Guncelleme : IslemTuru.PasifYapma, nameof(Urun), id,
            $"Ürün '{urun.Ad}' {(aktif ? "aktif" : "pasif")} yapıldı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<IReadOnlyList<Bagisci>> BagiscilariListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<Bagisci>().Sorgu().AsNoTracking()
            .Where(b => pasifleriDeGoster || b.Aktif)
            .OrderBy(b => b.Ad)
            .ToListAsync(iptal);
    }

    public async Task<Bagisci> BagisciKaydetAsync(int? id, string ad, string? telefon, string? aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var repo = veri.Repo<Bagisci>();

        var bagisci = id is { } mevcutId
            ? await repo.BulAsync(mevcutId, iptal) ?? throw new KayitBulunamadiException("Bağışçı")
            : new Bagisci();
        bagisci.Ad = ad;
        bagisci.Telefon = telefon;
        bagisci.Aciklama = Dogrula.IstegeBagliMetin(aciklama, "Açıklama", 500);

        if (await repo.VarMiAsync(b => b.Ad == bagisci.Ad && b.Id != bagisci.Id, iptal))
            throw new IsKuraliException($"'{bagisci.Ad}' adlı bir bağışçı zaten kayıtlı. Listeden seçebilirsiniz.");

        if (id is null)
            repo.Ekle(bagisci);
        await veri.KaydetVeGecmiseYazAsync(oturum, zaman.Simdi(), id is null ? IslemTuru.Olusturma : IslemTuru.Guncelleme,
            nameof(Bagisci), () => bagisci.Id, () => $"Bağışçı '{bagisci.Ad}' {(id is null ? "eklendi" : "güncellendi")}.", iptal);
        return bagisci;
    }

    public async Task BagisciAktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var bagisci = await veri.Repo<Bagisci>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Bağışçı");
        bagisci.Aktif = aktif;
        veri.IslemKaydet(oturum, zaman.Simdi(), aktif ? IslemTuru.Guncelleme : IslemTuru.PasifYapma, nameof(Bagisci), id,
            $"Bağışçı '{bagisci.Ad}' {(aktif ? "aktif" : "pasif")} yapıldı.");
        await veri.GuvenliKaydetAsync(iptal);
    }
}
