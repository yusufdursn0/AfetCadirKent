using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.GonulluYonetimi;

/// <summary>Gönüllü ekle/güncelle bilgisi.</summary>
public sealed record GonulluBilgisi
{
    public required string Ad { get; init; }
    public required string Soyad { get; init; }
    public string? Telefon { get; init; }
    public required string UzmanlikAlani { get; init; }
    public string? Notlar { get; init; }
}

/// <summary>Gönüllü listesi satırı.</summary>
public sealed record GonulluOzeti(int Id, string Ad, string Soyad, string? Telefon, string UzmanlikAlani, string? Notlar, bool Aktif,
    int VardiyaSayisi, DateTime? SonrakiVardiya)
{
    public string AdSoyad => $"{Ad} {Soyad}";
}

/// <summary>Vardiya ve görev ataması bilgisi. Görev, görev tanımı yazılarak ve/veya bir talebe bağlanarak verilir (K-053).</summary>
public sealed record VardiyaBilgisi
{
    public required int GonulluId { get; init; }
    public required DateTime Baslangic { get; init; }
    public required DateTime Bitis { get; init; }
    public int? BolgeId { get; init; }
    public string? GorevTanimi { get; init; }
    public int? TalepId { get; init; }
}

/// <summary>Takvimde gösterilen vardiya.</summary>
public sealed record VardiyaOzeti(int Id, int GonulluId, string GonulluAdi, string UzmanlikAlani, int? BolgeId, string? BolgeAdi,
    DateTime Baslangic, DateTime Bitis, string? GorevTanimi, int? TalepId, string? TalepOzeti)
{
    /// <summary>Takvimdeki başlık: "Ali Kaya – Çadır kurulumu".</summary>
    public string Baslik => GorevTanimi is { Length: > 0 } g ? $"{GonulluAdi} – {g}"
        : TalepId is { } t ? $"{GonulluAdi} – Talep #{t}" : GonulluAdi;
}

/// <summary>Gönüllüye görev olarak bağlanabilecek açık talep.</summary>
public sealed record AtanabilirTalep(int Id, string Ozet);

public interface IGonulluServisi
{
    Task<IReadOnlyList<GonulluOzeti>> ListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default);

    Task<Gonullu> KaydetAsync(int? id, GonulluBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Vardiyası olan gönüllü silinmez: "Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz."</summary>
    Task SilAsync(int id, CancellationToken iptal = default);

    /// <summary>Gelecekte vardiyası olan gönüllü pasif yapılamaz (önce vardiyaları silinir).</summary>
    Task AktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default);

    /// <summary>Aralıkla kesişen vardiyalar (takvim görünümü için).</summary>
    Task<IReadOnlyList<VardiyaOzeti>> VardiyalariGetirAsync(DateTime baslangic, DateTime bitis, CancellationToken iptal = default);

    /// <summary>Vardiya ekler/günceller: süre en fazla <see cref="GonulluServisi.EnUzunVardiyaSaati"/> saat; aynı gönüllünün vardiyaları çakışmaz.</summary>
    Task<VardiyaOzeti> VardiyaKaydetAsync(int? id, VardiyaBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Henüz bitmemiş vardiya silinir; geçmiş vardiya kayıt olarak kalır.</summary>
    Task VardiyaSilAsync(int id, CancellationToken iptal = default);

    /// <summary>Görev olarak bağlanabilecek açık talepler (en yeni önce).</summary>
    Task<IReadOnlyList<AtanabilirTalep>> AtanabilirTaleplerAsync(CancellationToken iptal = default);
}

/// <summary>
/// Gönüllü kaydı, vardiya planı ve görev ataması (şartname M12). Gönüllüleri yalnızca Koordinatör yönetir ("Gönüllü" izni);
/// gönüllüler programa giriş yapmaz (kullanıcı hesabı yoktur).
/// </summary>
public class GonulluServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IGonulluServisi
{
    public const int EnUzunVardiyaSaati = 24;

    public async Task<IReadOnlyList<GonulluOzeti>> ListeleAsync(bool pasifleriDeGoster = false, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var simdi = zaman.Simdi();
        return await veri.Repo<Gonullu>().Sorgu().AsNoTracking()
            .Where(g => pasifleriDeGoster || g.Aktif)
            .OrderBy(g => g.Ad).ThenBy(g => g.Soyad)
            .Select(g => new GonulluOzeti(g.Id, g.Ad, g.Soyad, g.Telefon, g.UzmanlikAlani, g.Notlar, g.Aktif, g.Vardiyalar.Count,
                g.Vardiyalar.Where(v => v.Bitis > simdi).Min(v => (DateTime?)v.Baslangic)))
            .ToListAsync(iptal);
    }

    public async Task<Gonullu> KaydetAsync(int? id, GonulluBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(bilgi);
        await using var veri = veriFabrikasi.Ac();
        var repo = veri.Repo<Gonullu>();
        var gonullu = id is { } mevcutId
            ? await repo.BulAsync(mevcutId, iptal) ?? throw new KayitBulunamadiException("Gönüllü")
            : new Gonullu { KayitZamani = zaman.Simdi() };

        gonullu.Ad = bilgi.Ad;
        gonullu.Soyad = bilgi.Soyad;
        gonullu.Telefon = bilgi.Telefon;
        gonullu.UzmanlikAlani = bilgi.UzmanlikAlani;
        gonullu.Notlar = bilgi.Notlar;

        if (gonullu.Telefon is { } telefon && await repo.VarMiAsync(g => g.Telefon == telefon && g.Id != gonullu.Id, iptal))
            throw new IsKuraliException($"{telefon} telefonuyla kayıtlı başka bir gönüllü var.");

        if (id is null)
            repo.Ekle(gonullu);
        await veri.KaydetVeGecmiseYazAsync(oturum, zaman.Simdi(), id is null ? IslemTuru.Olusturma : IslemTuru.Guncelleme,
            nameof(Gonullu), () => gonullu.Id,
            () => $"Gönüllü {gonullu.AdSoyad} ({gonullu.UzmanlikAlani}) {(id is null ? "eklendi" : "güncellendi")}.", iptal);
        return gonullu;
    }

    public async Task SilAsync(int id, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var gonullu = await veri.Repo<Gonullu>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Gönüllü");
        if (await veri.Repo<Vardiya>().VarMiAsync(v => v.GonulluId == id, iptal))
            throw new SilinemezException();

        veri.Repo<Gonullu>().Sil(gonullu);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Silme, nameof(Gonullu), id, $"Gönüllü {gonullu.AdSoyad} silindi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task AktiflikDegistirAsync(int id, bool aktif, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var gonullu = await veri.Repo<Gonullu>().BulAsync(id, iptal) ?? throw new KayitBulunamadiException("Gönüllü");
        var simdi = zaman.Simdi();
        if (!aktif && await veri.Repo<Vardiya>().VarMiAsync(v => v.GonulluId == id && v.Bitis > simdi, iptal))
            throw new IsKuraliException($"{gonullu.AdSoyad} için bitmemiş vardiya var. Önce vardiyalarını silin veya başka gönüllüye verin.");

        gonullu.Aktif = aktif;
        veri.IslemKaydet(oturum, simdi, aktif ? IslemTuru.Guncelleme : IslemTuru.PasifYapma, nameof(Gonullu), id,
            $"Gönüllü {gonullu.AdSoyad} {(aktif ? "aktif" : "pasif")} yapıldı.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<IReadOnlyList<VardiyaOzeti>> VardiyalariGetirAsync(DateTime baslangic, DateTime bitis, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await VardiyaSorgusu(veri, v => v.Baslangic < bitis && v.Bitis > baslangic).ToListAsync(iptal);
    }

    public async Task<VardiyaOzeti> VardiyaKaydetAsync(int? id, VardiyaBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(bilgi);
        VardiyaZamaniniDogrula(bilgi.Baslangic, bilgi.Bitis);
        var gorev = Dogrula.IstegeBagliMetin(bilgi.GorevTanimi, "Görev tanımı", 500);

        await using var veri = veriFabrikasi.Ac();
        var repo = veri.Repo<Vardiya>();
        var vardiya = id is { } mevcutId
            ? await repo.BulAsync(mevcutId, iptal) ?? throw new KayitBulunamadiException("Vardiya")
            : new Vardiya();
        var simdi = zaman.Simdi();
        if (id is not null && vardiya.Bitis <= simdi)
            throw new IsKuraliException("Bitmiş vardiya değiştirilemez; kayıt olarak kalır.");

        var gonullu = await veri.Repo<Gonullu>().BulAsync(bilgi.GonulluId, iptal) ?? throw new KayitBulunamadiException("Gönüllü");
        if (!gonullu.Aktif)
            throw new IsKuraliException($"{gonullu.AdSoyad} pasif; pasif gönüllüye vardiya verilemez.");
        if (bilgi.BolgeId is { } bolgeId && !await veri.Repo<Bolge>().VarMiAsync(b => b.Id == bolgeId && b.Aktif, iptal))
            throw new IsKuraliException("Seçilen bölge bulunamadı veya pasif.");
        if (bilgi.TalepId is { } talepId && talepId != vardiya.TalepId && !await veri.Repo<Talep>().VarMiAsync(
                t => t.Id == talepId && t.Durum != TalepDurumu.Tamamlandi && t.Durum != TalepDurumu.Iptal, iptal))
            throw new IsKuraliException("Görev olarak yalnızca açık bir talep bağlanabilir.");

        var cakisan = await repo.Sorgu().AsNoTracking()
            .Where(v => v.GonulluId == bilgi.GonulluId && v.Id != vardiya.Id && v.Baslangic < bilgi.Bitis && v.Bitis > bilgi.Baslangic)
            .OrderBy(v => v.Baslangic).FirstOrDefaultAsync(iptal);
        if (cakisan is not null)
            throw new IsKuraliException($"{gonullu.AdSoyad} için {cakisan.Baslangic:dd.MM HH:mm}–{cakisan.Bitis:HH:mm} arasında başka vardiya var; " +
                                        "vardiyalar çakışamaz.");

        vardiya.GonulluId = bilgi.GonulluId;
        vardiya.Baslangic = bilgi.Baslangic;
        vardiya.Bitis = bilgi.Bitis;
        vardiya.BolgeId = bilgi.BolgeId;
        vardiya.GorevTanimi = gorev;
        vardiya.TalepId = bilgi.TalepId;
        if (id is null)
            repo.Ekle(vardiya);

        var gorevMetni = gorev ?? (bilgi.TalepId is { } t ? $"talep #{t}" : "görev yazılmadı");
        await veri.KaydetVeGecmiseYazAsync(oturum, simdi, id is null ? IslemTuru.Olusturma : IslemTuru.Guncelleme, nameof(Vardiya),
            () => vardiya.Id,
            () => $"Vardiya {(id is null ? "eklendi" : "güncellendi")}: {gonullu.AdSoyad}, {bilgi.Baslangic:dd.MM.yyyy HH:mm}–{bilgi.Bitis:HH:mm}, {gorevMetni}.",
            iptal);
        return await VardiyaSorgusu(veri, v => v.Id == vardiya.Id).FirstAsync(iptal);
    }

    /// <summary>Bitiş başlangıçtan sonra olmalı ve vardiya en fazla bir gün sürmeli.</summary>
    internal static void VardiyaZamaniniDogrula(DateTime baslangic, DateTime bitis)
    {
        if (bitis <= baslangic)
            throw new DogrulamaException("Vardiyanın bitişi başlangıcından sonra olmalıdır.");
        if (bitis - baslangic > TimeSpan.FromHours(EnUzunVardiyaSaati))
            throw new DogrulamaException($"Bir vardiya en fazla {EnUzunVardiyaSaati} saat sürebilir.");
    }

    public async Task VardiyaSilAsync(int id, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var vardiya = await veri.Repo<Vardiya>().Sorgu().Include(v => v.Gonullu).FirstOrDefaultAsync(v => v.Id == id, iptal)
            ?? throw new KayitBulunamadiException("Vardiya");
        var simdi = zaman.Simdi();
        if (vardiya.Bitis <= simdi)
            throw new IsKuraliException("Bitmiş vardiya silinemez; kayıt olarak kalır.");

        veri.Repo<Vardiya>().Sil(vardiya);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Silme, nameof(Vardiya), id,
            $"Vardiya silindi: {vardiya.Gonullu!.AdSoyad}, {vardiya.Baslangic:dd.MM.yyyy HH:mm}–{vardiya.Bitis:HH:mm}.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<IReadOnlyList<AtanabilirTalep>> AtanabilirTaleplerAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Gonullu, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var talepler = await veri.Repo<Talep>().Sorgu().AsNoTracking()
            .Where(t => t.Durum != TalepDurumu.Tamamlandi && t.Durum != TalepDurumu.Iptal)
            .OrderByDescending(t => t.AcilisZamani)
            .Select(t => new { t.Id, t.Tur, t.Aciklama })
            .ToListAsync(iptal);
        return talepler.Select(t => new AtanabilirTalep(t.Id, $"#{t.Id} {t.Tur.Metin()} – {t.Aciklama.Kisalt(60)}")).ToList();
    }

    private static IQueryable<VardiyaOzeti> VardiyaSorgusu(IVeriOturumu veri, System.Linq.Expressions.Expression<Func<Vardiya, bool>> kosul) =>
        veri.Repo<Vardiya>().Sorgu().AsNoTracking()
            .Where(kosul)
            .OrderBy(v => v.Baslangic)
            .Select(v => new VardiyaOzeti(v.Id, v.GonulluId, v.Gonullu!.Ad + " " + v.Gonullu.Soyad, v.Gonullu.UzmanlikAlani, v.BolgeId,
                v.Bolge == null ? null : v.Bolge.Kod + " – " + v.Bolge.Ad, v.Baslangic, v.Bitis, v.GorevTanimi, v.TalepId,
                v.Talep == null ? null : v.Talep.Aciklama));
}
