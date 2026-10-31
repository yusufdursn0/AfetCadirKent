using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.Kayit;

public interface IKayitServisi
{
    Task<MukerrerSonucu> MukerrerKontrolAsync(KisiBilgisi kisi, int? haricKisiId = null, CancellationToken iptal = default);

    /// <summary>Yeni aile ve üyelerini kaydeder; aileye ve kişilere benzersiz numara verilir.</summary>
    Task<Aile> AileOlusturAsync(string aileAdi, bool tekEbeveyn, IReadOnlyList<KisiBilgisi> uyeler, string? notlar = null,
        CancellationToken iptal = default);

    Task AileGuncelleAsync(int aileId, string aileAdi, bool tekEbeveyn, string? notlar, CancellationToken iptal = default);

    /// <summary>Aileye sonradan gelen yakını ekler.</summary>
    Task<Afetzede> KisiEkleAsync(int aileId, KisiBilgisi kisi, CancellationToken iptal = default);

    Task KisiGuncelleAsync(int kisiId, KisiBilgisi kisi, CancellationToken iptal = default);

    /// <summary>Sağlık bilgileri (sağlık personeli / koordinatör).</summary>
    Task SaglikBilgisiGuncelleAsync(int kisiId, bool kronikHasta, bool hamile, string? saglikNotu, CancellationToken iptal = default);

    /// <summary>Burada / Ayrıldı / Nakil / Hastanede geçişleri; geçmişe hareket olarak yazılır.</summary>
    Task DurumDegistirAsync(int kisiId, KisiDurumu yeniDurum, string? aciklama, CancellationToken iptal = default);

    /// <summary>Seçilen kişileri mevcut aileden ayırıp yeni aile oluşturur.</summary>
    Task<Aile> AiledenAyirAsync(int kaynakAileId, IReadOnlyCollection<int> kisiIdler, string yeniAileAdi, string aciklama,
        CancellationToken iptal = default);

    Task KisiPasifYapAsync(int kisiId, string aciklama, CancellationToken iptal = default);
    Task AilePasifYapAsync(int aileId, string aciklama, CancellationToken iptal = default);

    Task<IReadOnlyList<Afetzede>> KisileriListeleAsync(KisiFiltresi filtre, CancellationToken iptal = default);
    Task<IReadOnlyList<Aile>> AileleriListeleAsync(string? metin = null, CancellationToken iptal = default);

    /// <summary>Aile ve üyeleri (yetkiye göre süzülmüş).</summary>
    Task<Aile> AileGetirAsync(int aileId, CancellationToken iptal = default);

    Task<IReadOnlyList<KisiHareketi>> KisiHareketleriAsync(int kisiId, CancellationToken iptal = default);
    Task<IReadOnlyList<AileGecmisi>> AileGecmisiAsync(int aileId, CancellationToken iptal = default);
}

public class KayitServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman) : IKayitServisi
{
    private bool SaglikBilgisiGorebilir => oturum.IzinVarMi(Modul.SaglikBilgisi, Izin.Gorme);

    // ---------------------------------------------------------------- Mükerrer kontrol

    public async Task<MukerrerSonucu> MukerrerKontrolAsync(KisiBilgisi kisi, int? haricKisiId = null, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await MukerrerKontrolAsync(veri, kisi, haricKisiId, iptal);
    }

    private async Task<MukerrerSonucu> MukerrerKontrolAsync(IVeriOturumu veri, KisiBilgisi kisi, int? haricKisiId, CancellationToken iptal)
    {
        var kisiler = veri.Repo<Afetzede>().Sorgu().AsNoTracking().Where(k => k.Id != haricKisiId);

        Afetzede? tcSahibi = null;
        var tc = kisi.TcKimlikNo?.Trim();
        if (!string.IsNullOrEmpty(tc))
        {
            if (!TcKimlik.GecerliMi(tc))
                throw new DogrulamaException("TC kimlik numarası geçersiz. 11 haneli geçerli bir numara girin veya boş bırakın.");
            tcSahibi = await kisiler.FirstOrDefaultAsync(k => k.TcKimlikNo == tc, iptal);
        }

        var aramaAdi = TurkceMetin.Normallestir($"{kisi.Ad} {kisi.Soyad}");
        var adaylar = await kisiler.Include(k => k.Aile)
            .Where(k => k.AramaAdi == aramaAdi && k.Aktif)
            .ToListAsync(iptal);
        var olasi = MukerrerKontrol.OlasiEslesmeler(kisi.Ad, kisi.Soyad, kisi.DogumTarihi, adaylar, zaman.Bugun())
            .Where(k => oturum.RefakatsizCocukGorebilir() || !k.RefakatsizCocuk)
            .ToList();

        return new MukerrerSonucu(tcSahibi, olasi);
    }

    private async Task TcEngeliniKontrolEtAsync(IVeriOturumu veri, KisiBilgisi kisi, int? haricKisiId, CancellationToken iptal)
    {
        var sonuc = await MukerrerKontrolAsync(veri, kisi, haricKisiId, iptal);
        if (sonuc.TcSahibi is { } sahip)
            throw new IsKuraliException(
                $"{sahip.TcKimlikNo} TC kimlik numarasıyla kayıtlı bir kişi var ({sahip.KisiNo}). Aynı kişi ikinci kez kaydedilemez.");
    }

    // ---------------------------------------------------------------- Aile ve kişi kaydı

    public async Task<Aile> AileOlusturAsync(string aileAdi, bool tekEbeveyn, IReadOnlyList<KisiBilgisi> uyeler, string? notlar = null,
        CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        if (uyeler is null || uyeler.Count == 0)
            throw new DogrulamaException("Aileye en az bir kişi eklenmelidir.");

        var tcTekrari = uyeler.Where(u => !string.IsNullOrWhiteSpace(u.TcKimlikNo))
            .GroupBy(u => u.TcKimlikNo!.Trim()).FirstOrDefault(g => g.Count() > 1);
        if (tcTekrari is not null)
            throw new DogrulamaException($"{tcTekrari.Key} TC kimlik numarası listede birden fazla kişiye girilmiş.");

        var simdi = zaman.Simdi();
        var aile = new Aile { AileAdi = aileAdi, TekEbeveyn = tekEbeveyn, Notlar = notlar, KayitZamani = simdi };

        await using var veri = veriFabrikasi.Ac();
        foreach (var bilgi in uyeler)
        {
            await TcEngeliniKontrolEtAsync(veri, bilgi, null, iptal);
            aile.Uyeler.Add(YeniKisi(bilgi, simdi));
        }

        // Aile ve kişi numaraları veritabanında üretilir; işlem geçmişi metni onları kullandığı için iki adımda, tek transaction.
        veri.Repo<Aile>().Ekle(aile);
        await veri.KaydetVeGecmiseYazAsync(oturum, simdi, IslemTuru.Olusturma, nameof(Aile), () => aile.Id,
            () => $"{aile.AileNo} {aile.AileAdi} kaydedildi ({aile.Uyeler.Count} kişi: {string.Join(", ", aile.Uyeler.Select(u => u.KisiNo))}).",
            iptal);
        return aile;
    }

    public async Task AileGuncelleAsync(int aileId, string aileAdi, bool tekEbeveyn, string? notlar, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var aile = await AktifAileAsync(veri, aileId, iptal);

        var eski = $"{aile.AileAdi}, tek ebeveyn: {(aile.TekEbeveyn ? "evet" : "hayır")}";
        aile.AileAdi = aileAdi;
        aile.TekEbeveyn = tekEbeveyn;
        aile.Notlar = notlar;

        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Aile), aile.Id,
            $"{aile.AileNo} güncellendi. Önceki: {eski}; yeni: {aile.AileAdi}, tek ebeveyn: {(aile.TekEbeveyn ? "evet" : "hayır")}.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<Afetzede> KisiEkleAsync(int aileId, KisiBilgisi kisi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var aile = await AktifAileAsync(veri, aileId, iptal);
        await TcEngeliniKontrolEtAsync(veri, kisi, null, iptal);

        var simdi = zaman.Simdi();
        var yeni = YeniKisi(kisi, simdi);

        aile.Uyeler.Add(yeni);
        await veri.KaydetVeGecmiseYazAsync(oturum, simdi, IslemTuru.Olusturma, nameof(Afetzede), () => yeni.Id,
            () => $"{yeni.KisiNo} {yeni.AdSoyad}, {aile.AileNo} ailesine sonradan eklendi.", iptal);
        return yeni;
    }

    public async Task KisiGuncelleAsync(int kisiId, KisiBilgisi kisi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var mevcut = await GorunurKisiAsync(veri, kisiId, iptal);
        await TcEngeliniKontrolEtAsync(veri, kisi, kisiId, iptal);

        BilgileriUygula(mevcut, kisi, saglikNotuDahil: false);
        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Afetzede), mevcut.Id,
            $"{mevcut.KisiNo} {mevcut.AdSoyad} bilgileri güncellendi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task SaglikBilgisiGuncelleAsync(int kisiId, bool kronikHasta, bool hamile, string? saglikNotu,
        CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.SaglikBilgisi, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var kisi = await GorunurKisiAsync(veri, kisiId, iptal);

        kisi.KronikHasta = kronikHasta;
        kisi.Hamile = hamile;
        kisi.SaglikNotu = Dogrula.IstegeBagliMetin(saglikNotu, "Sağlık notu", 1000);

        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.Guncelleme, nameof(Afetzede), kisi.Id,
            $"{kisi.KisiNo} sağlık bilgileri güncellendi.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    // ---------------------------------------------------------------- Durum ve hareketler

    public async Task DurumDegistirAsync(int kisiId, KisiDurumu yeniDurum, string? aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var kisi = await GorunurKisiAsync(veri, kisiId, iptal);
        if (!kisi.Aktif)
            throw new IsKuraliException("Pasif kayıtların durumu değiştirilemez.");

        var eski = kisi.Durum;
        var hareketTuru = DurumGecisi(eski, yeniDurum);
        var simdi = zaman.Simdi();
        var hareket = new KisiHareketi
        {
            Kisi = kisi, Tur = hareketTuru, Zaman = simdi, KullaniciId = oturum.KullaniciId,
            Aciklama = Dogrula.IstegeBagliMetin(aciklama, "Açıklama", 500),
        };

        // Ayrılış ve nakilde çadırdaki yer boşalır; hastaneye sevkte yer korunur.
        if (yeniDurum is KisiDurumu.Ayrildi or KisiDurumu.Nakil)
            hareket.EskiCadirId = (await AktifYerlesimiKapatAsync(veri, kisi, simdi, iptal))?.Id;

        kisi.Durum = yeniDurum;
        veri.Repo<KisiHareketi>().Ekle(hareket);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Guncelleme, nameof(Afetzede), kisi.Id,
            $"{kisi.KisiNo} durumu {eski.Metin()} → {yeniDurum.Metin()}.");
        await veri.GuvenliKaydetAsync(iptal);
    }

    /// <summary>İzin verilen durum geçişleri ve karşılık gelen hareket türü.</summary>
    internal static KisiHareketTuru DurumGecisi(KisiDurumu eski, KisiDurumu yeni)
    {
        if (eski == yeni)
            throw new IsKuraliException($"Kişinin durumu zaten '{yeni.Metin()}'.");
        return GecisHareketi(eski, yeni) ?? throw new IsKuraliException(
            $"'{eski.Metin()}' durumundaki kişi doğrudan '{yeni.Metin()}' yapılamaz. Önce kişinin geri geldiğini ('Burada') kaydedin.");
    }

    /// <summary>Kişinin şu anki durumundan geçilebilecek durumlar (ekran yalnızca bunları sunar; kural <see cref="DurumGecisi"/> ile aynı).</summary>
    public static IReadOnlyList<KisiDurumu> GecilebilirDurumlar(KisiDurumu eski) =>
        Enum.GetValues<KisiDurumu>().Where(y => y != eski && GecisHareketi(eski, y) is not null).ToList();

    private static KisiHareketTuru? GecisHareketi(KisiDurumu eski, KisiDurumu yeni) => (eski, yeni) switch
    {
        (KisiDurumu.Burada, KisiDurumu.Hastanede) => KisiHareketTuru.HastaneyeSevk,
        (KisiDurumu.Hastanede, KisiDurumu.Burada) => KisiHareketTuru.HastanedenDonus,
        (KisiDurumu.Burada or KisiDurumu.Hastanede, KisiDurumu.Ayrildi) => KisiHareketTuru.Ayrilis,
        (KisiDurumu.Burada or KisiDurumu.Hastanede, KisiDurumu.Nakil) => KisiHareketTuru.Nakil,
        (KisiDurumu.Ayrildi or KisiDurumu.Nakil, KisiDurumu.Burada) => KisiHareketTuru.Gelis,
        _ => null,
    };

    public async Task<Aile> AiledenAyirAsync(int kaynakAileId, IReadOnlyCollection<int> kisiIdler, string yeniAileAdi, string aciklama,
        CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Ayırma gerekçesi", 1000);
        var secilenler = kisiIdler?.Distinct().ToList() ?? [];
        if (secilenler.Count == 0)
            throw new DogrulamaException("Yeni aileye geçecek en az bir kişi seçilmelidir.");

        await using var veri = veriFabrikasi.Ac();
        var kaynak = await AktifAileAsync(veri, kaynakAileId, iptal);

        var tasinanlar = kaynak.Uyeler.Where(u => secilenler.Contains(u.Id)).ToList();
        oturum.KisilerGorunurOlmali(tasinanlar);
        if (tasinanlar.Count != secilenler.Count)
            throw new IsKuraliException("Seçilen kişilerin tamamı bu aileye ait olmalıdır.");
        if (tasinanlar.Any(u => !u.CadirKenttemi))
            throw new IsKuraliException("Ayrılmış, nakil edilmiş veya pasif kişiler yeni aileye taşınamaz.");
        if (!kaynak.CadirKenttekiUyeler.Except(tasinanlar).Any())
            throw new IsKuraliException("Eski ailede en az bir kişi kalmalıdır. Tüm aile için ayırma gerekmez.");

        var simdi = zaman.Simdi();
        var yeni = new Aile
        {
            AileAdi = yeniAileAdi,
            KayitZamani = simdi,
            Notlar = $"{kaynak.AileNo} ailesinden ayrıldı.",
        };
        veri.Repo<Aile>().Ekle(yeni);

        foreach (var kisi in tasinanlar)
        {
            kisi.Aile = yeni;
            veri.Repo<KisiHareketi>().Ekle(new KisiHareketi
            {
                Kisi = kisi, Tur = KisiHareketTuru.AileDegisikligi, Zaman = simdi, KullaniciId = oturum.KullaniciId,
                EskiAileId = kaynak.Id, YeniAile = yeni, Aciklama = temizAciklama,
            });
        }

        veri.Repo<AileGecmisi>().Ekle(new AileGecmisi
        {
            Tur = AileGecmisiTuru.Ayirma, KaynakAile = kaynak, HedefAile = yeni, Zaman = simdi, KullaniciId = oturum.KullaniciId,
            Aciklama = temizAciklama, TasinanKisiler = string.Join(", ", tasinanlar.Select(k => k.KisiNo)),
        });

        // Kaynak ailenin RowVersion'ı kontrol edilsin: aynı aile eşzamanlı ikinci kez ayrılamaz.
        veri.Repo<Aile>().Guncelle(kaynak);

        // Gerekçe ve dağıtım hakları kayıtlı veriden her okumada hesaplandığı için iki aile için de kendiliğinden yenilenir.
        await veri.KaydetVeGecmiseYazAsync(oturum, simdi, IslemTuru.Olusturma, nameof(Aile), () => yeni.Id,
            () => $"{kaynak.AileNo} ailesinden {string.Join(", ", tasinanlar.Select(k => k.KisiNo))} ayrılarak {yeni.AileNo} oluşturuldu. Gerekçe: {temizAciklama}",
            iptal);
        return yeni;
    }

    // ---------------------------------------------------------------- Pasif yapma

    public async Task KisiPasifYapAsync(int kisiId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);
        await using var veri = veriFabrikasi.Ac();
        var kisi = await GorunurKisiAsync(veri, kisiId, iptal);
        if (!kisi.Aktif)
            throw new IsKuraliException("Kayıt zaten pasif.");

        var simdi = zaman.Simdi();
        var cadir = await AktifYerlesimiKapatAsync(veri, kisi, simdi, iptal);
        if (cadir is not null)
        {
            veri.Repo<KisiHareketi>().Ekle(new KisiHareketi
            {
                Kisi = kisi, Tur = KisiHareketTuru.Ayrilis, EskiCadirId = cadir.Id, Zaman = simdi, KullaniciId = oturum.KullaniciId,
                Aciklama = $"Kayıt pasif yapıldı: {temizAciklama}".Kisalt(500),
            });
        }

        kisi.Aktif = false;
        veri.IslemKaydet(oturum, simdi, IslemTuru.PasifYapma, nameof(Afetzede), kisi.Id,
            $"{kisi.KisiNo} {kisi.AdSoyad} pasif yapıldı. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task AilePasifYapAsync(int aileId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);
        await using var veri = veriFabrikasi.Ac();
        var aile = await AktifAileAsync(veri, aileId, iptal);
        if (aile.CadirKenttekiUyeler.Any())
            throw new IsKuraliException("Ailede çadır kentte bulunan kişiler var. Önce kişilerin durumunu değiştirin veya kişileri pasif yapın.");

        var simdi = zaman.Simdi();
        var bekleme = await veri.Repo<BeklemeListesiKaydi>().ListeleAsync(b => b.AileId == aileId && b.CikisZamani == null, iptal);
        foreach (var kayit in bekleme)
            kayit.CikisZamani = simdi;

        aile.Aktif = false;
        veri.IslemKaydet(oturum, simdi, IslemTuru.PasifYapma, nameof(Aile), aile.Id,
            $"{aile.AileNo} {aile.AileAdi} pasif yapıldı. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    // ---------------------------------------------------------------- Sorgular

    public async Task<IReadOnlyList<Afetzede>> KisileriListeleAsync(KisiFiltresi filtre, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();

        var sorgu = GorunurKisiler(veri).Include(k => k.Aile).AsNoTracking();
        if (!filtre.PasifleriDeGoster)
            sorgu = sorgu.Where(k => k.Aktif);
        if (filtre.Durum is { } durum)
            sorgu = sorgu.Where(k => k.Durum == durum);
        if (filtre.AileId is { } aileId)
            sorgu = sorgu.Where(k => k.AileId == aileId);

        var metin = filtre.Metin?.Trim();
        if (!string.IsNullOrEmpty(metin))
        {
            var normal = TurkceMetin.Normallestir(metin);
            var buyuk = metin.ToUpperInvariant();
            sorgu = sorgu.Where(k => k.AramaAdi.Contains(normal) || k.KisiNo == buyuk || k.TcKimlikNo == metin);
        }

        var liste = await sorgu.OrderBy(k => k.Soyad).ThenBy(k => k.Ad).ToListAsync(iptal);
        return SaglikBilgisiniSuz(liste);
    }

    public async Task<IReadOnlyList<Aile>> AileleriListeleAsync(string? metin = null, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();

        var sorgu = veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).AsNoTracking().Where(a => a.Aktif);
        var temiz = metin?.Trim();
        if (!string.IsNullOrEmpty(temiz))
        {
            var normal = TurkceMetin.Normallestir(temiz);
            var buyuk = temiz.ToUpperInvariant();
            sorgu = sorgu.Where(a => a.AileNo == buyuk || a.Uyeler.Any(u => u.AramaAdi.Contains(normal)));
        }

        var aileler = await sorgu.OrderBy(a => a.Id).ToListAsync(iptal);
        foreach (var aile in aileler)
            UyeleriSuz(aile);
        return aileler.Where(a => a.Uyeler.Count > 0).ToList();
    }

    public async Task<Aile> AileGetirAsync(int aileId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var aile = await veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == aileId, iptal) ?? throw new KayitBulunamadiException("Aile");

        UyeleriSuz(aile);
        if (aile.Uyeler.Count == 0)
            throw new KayitBulunamadiException("Aile");
        return aile;
    }

    public async Task<IReadOnlyList<KisiHareketi>> KisiHareketleriAsync(int kisiId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        await GorunurKisiAsync(veri, kisiId, iptal);
        return await veri.Repo<KisiHareketi>().Sorgu()
            .Include(h => h.EskiCadir!.Bolge).Include(h => h.YeniCadir!.Bolge)
            .Include(h => h.EskiAile).Include(h => h.YeniAile).Include(h => h.Kullanici)
            .Where(h => h.KisiId == kisiId)
            .OrderBy(h => h.Zaman)
            .AsNoTracking()
            .ToListAsync(iptal);
    }

    public async Task<IReadOnlyList<AileGecmisi>> AileGecmisiAsync(int aileId, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Kayit, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        return await veri.Repo<AileGecmisi>().Sorgu()
            .Include(g => g.KaynakAile).Include(g => g.HedefAile).Include(g => g.Kullanici)
            .Where(g => g.KaynakAileId == aileId || g.HedefAileId == aileId)
            .OrderByDescending(g => g.Zaman)
            .AsNoTracking()
            .ToListAsync(iptal);
    }

    // ---------------------------------------------------------------- Yardımcılar

    private IQueryable<Afetzede> GorunurKisiler(IVeriOturumu veri) => veri.Repo<Afetzede>().Sorgu().GorunurKisiler(oturum);

    private async Task<Afetzede> GorunurKisiAsync(IVeriOturumu veri, int kisiId, CancellationToken iptal) =>
        await GorunurKisiler(veri).FirstOrDefaultAsync(k => k.Id == kisiId, iptal) ?? throw new KayitBulunamadiException("Kişi");

    private async Task<Aile> AktifAileAsync(IVeriOturumu veri, int aileId, CancellationToken iptal)
    {
        var aile = await veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).FirstOrDefaultAsync(a => a.Id == aileId && a.Aktif, iptal)
            ?? throw new KayitBulunamadiException("Aile");
        oturum.AileGorunurOlmali(aile);
        return aile;
    }

    /// <summary>Kişinin aktif yerleşimini kapatır ve çadırda yeri boşaltır. Yerleşim yoksa null.</summary>
    private static async Task<Cadir?> AktifYerlesimiKapatAsync(IVeriOturumu veri, Afetzede kisi, DateTime simdi, CancellationToken iptal)
    {
        var yerlesim = await veri.Repo<Yerlesim>().Sorgu().Include(y => y.Cadir)
            .FirstOrDefaultAsync(y => y.KisiId == kisi.Id && y.Bitis == null, iptal);
        if (yerlesim is null)
            return null;

        yerlesim.Bitis = simdi;
        yerlesim.Cadir!.Bosalt(1);
        return yerlesim.Cadir;
    }

    private Afetzede YeniKisi(KisiBilgisi bilgi, DateTime simdi)
    {
        var kisi = new Afetzede { KayitZamani = simdi };
        BilgileriUygula(kisi, bilgi, saglikNotuDahil: true);
        kisi.Hareketler.Add(new KisiHareketi
        {
            Tur = KisiHareketTuru.Gelis, Zaman = simdi, KullaniciId = oturum.KullaniciId, Aciklama = "Çadır kente kayıt",
        });
        return kisi;
    }

    private void BilgileriUygula(Afetzede kisi, KisiBilgisi b, bool saglikNotuDahil)
    {
        kisi.Ad = b.Ad;
        kisi.Soyad = b.Soyad;
        kisi.DogumTarihi = b.DogumTarihi;
        kisi.DogumTarihiTahmini = b.DogumTarihiTahmini;
        kisi.TcKimlikNo = b.TcKimlikNo;
        kisi.Cinsiyet = b.Cinsiyet;
        kisi.Memleket = b.Memleket;
        kisi.Yakinlik = Dogrula.IstegeBagliMetin(b.Yakinlik, "Yakınlık", 30);
        kisi.Telefon = b.Telefon;
        kisi.Hamile = b.Hamile;
        kisi.TekerlekliSandalye = b.TekerlekliSandalye;
        kisi.KronikHasta = b.KronikHasta;
        kisi.RefakatsizCocuk = b.RefakatsizCocuk;
        if (b.Fotograf is not null)
            kisi.Fotograf = b.Fotograf;

        if (!saglikNotuDahil || string.IsNullOrWhiteSpace(b.SaglikNotu))
            return;
        if (!oturum.IzinVarMi(Modul.SaglikBilgisi, Izin.Degistirme))
            throw new YetkisizIslemException("Sağlık notu girmek için yetkiniz yok.");
        kisi.SaglikNotu = Dogrula.IstegeBagliMetin(b.SaglikNotu, "Sağlık notu", 1000);
    }

    /// <summary>Takip edilmeyen sonuçlardan, yetkisi olmayan role sağlık notunu ve refakatsiz çocukları çıkarır.</summary>
    private void UyeleriSuz(Aile aile)
    {
        if (!oturum.RefakatsizCocukGorebilir())
            aile.Uyeler.RemoveAll(u => u.RefakatsizCocuk);
        SaglikBilgisiniSuz(aile.Uyeler);
    }

    private IReadOnlyList<Afetzede> SaglikBilgisiniSuz(List<Afetzede> kisiler)
    {
        if (!SaglikBilgisiGorebilir)
            foreach (var kisi in kisiler)
                kisi.SaglikNotu = null;
        return kisiler;
    }
}
