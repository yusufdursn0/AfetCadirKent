using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Business.DepoYonetimi;

public interface IStokServisi
{
    /// <summary>Yeni parti açar ve giriş hareketi yazar.</summary>
    Task<Parti> StokGirisiAsync(StokGirisiBilgisi bilgi, CancellationToken iptal = default);

    /// <summary>Daha önce çıkmış ürünün depoya geri dönmesi.</summary>
    Task IadeAsync(int partiId, decimal miktar, string aciklama, CancellationToken iptal = default);

    /// <summary>Hasar / fire: ayrılmamış stoktan düşer.</summary>
    Task HasarAsync(int partiId, decimal miktar, string aciklama, CancellationToken iptal = default);

    /// <summary>Sayılan fiziksel miktarı girer; fark kadar sayım farkı hareketi yazılır.</summary>
    Task<SayimFarkiHareketi> SayimAsync(int partiId, decimal sayilanMiktar, string aciklama, CancellationToken iptal = default);

    /// <summary>Hatalı hareketi pasif yapar ve etkisini geri alır (stok hareketleri silinmez). Dağıtım hareketi geri alınamaz.</summary>
    Task HareketPasifYapAsync(int hareketId, string aciklama, CancellationToken iptal = default);

    /// <summary>SKT'si geçmiş partilerin ayrılmamış miktarını hasar/fire hareketiyle düşer. Düşülen parti sayısını döndürür.</summary>
    Task<int> SktGecenleriFireyeDusAsync(CancellationToken iptal = default);

    /// <summary>Ürün bazında fiziksel / ayrılan / kullanılabilir stok ve partiler (FEFO sırasıyla).</summary>
    Task<IReadOnlyList<UrunStokDurumu>> StokDurumuAsync(CancellationToken iptal = default);

    Task<IReadOnlyList<StokHareketSatiri>> HareketleriListeleAsync(StokHareketFiltresi filtre, CancellationToken iptal = default);

    /// <summary>Kritik seviye altındaki ürünler, SKT'si yaklaşan ve geçmiş partiler.</summary>
    Task<IReadOnlyList<StokUyarisi>> UyarilariGetirAsync(CancellationToken iptal = default);

    /// <summary>Ürün bazında yeterlilik tahmini (tüketime ve hakka göre).</summary>
    Task<IReadOnlyList<StokTahmini>> TahminleriGetirAsync(CancellationToken iptal = default);
}

public class StokServisi(IVeriOturumuFabrikasi veriFabrikasi, IOturum oturum, TimeProvider zaman, DepoAyarlari ayarlar) : IStokServisi
{
    private int? KullaniciId => oturum.Kullanici is { Id: > 0 } k ? k.Id : null;

    public async Task<Parti> StokGirisiAsync(StokGirisiBilgisi bilgi, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        ArgumentNullException.ThrowIfNull(bilgi);
        var bugun = zaman.Bugun();
        if (bilgi.SonKullanmaTarihi is { } skt && skt < bugun)
            throw new DogrulamaException("Son kullanma tarihi geçmiş ürün stoğa girilemez.");

        await using var veri = veriFabrikasi.Ac();
        var urun = await veri.Repo<Urun>().BulAsync(bilgi.UrunId, iptal) ?? throw new KayitBulunamadiException("Ürün");
        if (!urun.Aktif)
            throw new IsKuraliException($"'{urun.Ad}' pasif bir ürün; stok girişi yapılamaz.");
        if (bilgi.BagisciId is { } bagisciId
            && !await veri.Repo<Bagisci>().VarMiAsync(b => b.Id == bagisciId && b.Aktif, iptal))
            throw new KayitBulunamadiException("Bağışçı");

        var simdi = zaman.Simdi();
        var parti = new Parti
        {
            UrunId = urun.Id, BagisciId = bilgi.BagisciId, PartiNo = bilgi.PartiNo,
            SonKullanmaTarihi = bilgi.SonKullanmaTarihi, GirisZamani = simdi,
        };
        if (await veri.Repo<Parti>().VarMiAsync(p => p.UrunId == urun.Id && p.PartiNo == parti.PartiNo, iptal))
            throw new IsKuraliException($"'{urun.Ad}' için {parti.PartiNo} numaralı parti zaten var.");

        var giris = new GirisHareketi
        {
            Parti = parti, Miktar = bilgi.Miktar, Zaman = simdi, KullaniciId = KullaniciId,
            Aciklama = Dogrula.IstegeBagliMetin(bilgi.Aciklama, "Açıklama", 500),
        };
        giris.PartiyeUygula(parti);

        await using var islem = await veri.IslemBaslatAsync(iptal);
        veri.Repo<Parti>().Ekle(parti);
        veri.Repo<StokHareketi>().Ekle(giris);
        await veri.GuvenliKaydetAsync(iptal);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Olusturma, nameof(Parti), parti.Id,
            $"Stok girişi: {urun.Ad}, parti {parti.PartiNo}, {giris.Miktar:0.##} {urun.Birim.Metin()}"
            + (parti.SonKullanmaTarihi is { } s ? $", SKT {s:dd.MM.yyyy}." : "."));
        await veri.GuvenliKaydetAsync(iptal);
        await islem.CommitAsync(iptal);
        return parti;
    }

    public Task IadeAsync(int partiId, decimal miktar, string aciklama, CancellationToken iptal = default) =>
        HareketYazAsync(partiId, new IadeHareketi { Miktar = miktar }, aciklama, iptal);

    public Task HasarAsync(int partiId, decimal miktar, string aciklama, CancellationToken iptal = default) =>
        HareketYazAsync(partiId, new HasarHareketi { Miktar = miktar }, aciklama, iptal);

    public async Task<SayimFarkiHareketi> SayimAsync(int partiId, decimal sayilanMiktar, string aciklama, CancellationToken iptal = default)
    {
        Dogrula.NegatifOlmayan(sayilanMiktar, "Sayılan miktar");
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        decimal mevcut;
        await using (var veri = veriFabrikasi.Ac())
        {
            var parti = await veri.Repo<Parti>().BulAsync(partiId, iptal) ?? throw new KayitBulunamadiException("Parti");
            mevcut = parti.Miktar;
        }

        var fark = sayilanMiktar - mevcut;
        if (fark == 0)
            throw new IsKuraliException("Sayılan miktar kayıttaki miktarla aynı; sayım farkı yok.");

        var hareket = new SayimFarkiHareketi { Miktar = Math.Abs(fark), FazlaMi = fark > 0 };
        await HareketYazAsync(partiId, hareket, aciklama, iptal, beklenenMiktar: mevcut);
        return hareket;
    }

    /// <param name="beklenenMiktar">Sayımda: fark hesaplandıktan sonra parti değiştiyse işlem reddedilir.</param>
    private async Task HareketYazAsync(int partiId, StokHareketi hareket, string aciklama, CancellationToken iptal,
        decimal? beklenenMiktar = null)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);

        await using var veri = veriFabrikasi.Ac();
        var parti = await veri.Repo<Parti>().Sorgu().Include(p => p.Urun).FirstOrDefaultAsync(p => p.Id == partiId, iptal)
            ?? throw new KayitBulunamadiException("Parti");
        if (beklenenMiktar is { } beklenen && parti.Miktar != beklenen)
            throw new EszamanlilikException();

        var simdi = zaman.Simdi();
        hareket.Parti = parti;
        hareket.Zaman = simdi;
        hareket.KullaniciId = KullaniciId;
        hareket.Aciklama = temizAciklama;
        hareket.PartiyeUygula(parti);

        veri.Repo<StokHareketi>().Ekle(hareket);
        veri.IslemKaydet(oturum, simdi, IslemTuru.Olusturma, nameof(Parti), partiId,
            $"{hareket.TurAdi}: {parti.Urun!.Ad}, parti {parti.PartiNo}, {hareket.StokEtkisi:+0.##;-0.##} {parti.Urun.Birim.Metin()}. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task HareketPasifYapAsync(int hareketId, string aciklama, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        var temizAciklama = Dogrula.ZorunluMetin(aciklama, "Açıklama", 500);

        await using var veri = veriFabrikasi.Ac();
        var hareket = await veri.Repo<StokHareketi>().Sorgu().Include(h => h.Parti!.Urun)
            .FirstOrDefaultAsync(h => h.Id == hareketId, iptal) ?? throw new KayitBulunamadiException("Stok hareketi");
        if (!hareket.Aktif)
            throw new IsKuraliException("Bu hareket zaten pasif.");

        var parti = hareket.Parti!;
        if (hareket is GirisHareketi && await veri.Repo<StokHareketi>().VarMiAsync(
                h => h.PartiId == parti.Id && h.Id != hareket.Id && h.Aktif, iptal))
            throw new IsKuraliException("Bu partide başka hareketler var; giriş hareketi geri alınamaz. Düzeltme için sayım farkı girin.");

        hareket.PartidenGeriAl(parti); // dağıtım hareketinde istisna fırlatır
        hareket.Aktif = false;
        if (hareket is GirisHareketi)
            parti.Aktif = false;

        veri.IslemKaydet(oturum, zaman.Simdi(), IslemTuru.PasifYapma, nameof(StokHareketi), hareketId,
            $"{hareket.TurAdi} hareketi geri alındı: {parti.Urun!.Ad}, parti {parti.PartiNo}, {hareket.Miktar:0.##}. Açıklama: {temizAciklama}");
        await veri.GuvenliKaydetAsync(iptal);
    }

    public async Task<int> SktGecenleriFireyeDusAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Degistirme);
        await using var veri = veriFabrikasi.Ac();
        var bugun = zaman.Bugun();
        var simdi = zaman.Simdi();

        var partiler = await veri.Repo<Parti>().Sorgu().Include(p => p.Urun)
            .Where(p => p.SonKullanmaTarihi < bugun && p.Miktar > p.AyrilanMiktar)
            .ToListAsync(iptal);
        foreach (var parti in partiler)
        {
            var hareket = new HasarHareketi
            {
                Parti = parti, Miktar = parti.Miktar - parti.AyrilanMiktar, Zaman = simdi, KullaniciId = KullaniciId,
                Aciklama = $"SKT geçti ({parti.SonKullanmaTarihi:dd.MM.yyyy}); fireye düşüldü.",
            };
            hareket.PartiyeUygula(parti);
            veri.Repo<StokHareketi>().Ekle(hareket);
            veri.IslemKaydet(oturum, simdi, IslemTuru.Olusturma, nameof(StokHareketi), parti.Id,
                $"SKT geçen parti fireye düşüldü: {parti.Urun!.Ad}, parti {parti.PartiNo}, {hareket.Miktar:0.##} {parti.Urun.Birim.Metin()}.");
        }

        await veri.GuvenliKaydetAsync(iptal);
        return partiler.Count;
    }

    public async Task<IReadOnlyList<UrunStokDurumu>> StokDurumuAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var bugun = zaman.Bugun();

        var urunler = await veri.Repo<Urun>().Sorgu().AsNoTracking().Where(u => u.Aktif).OrderBy(u => u.Ad).ToListAsync(iptal);
        var partiler = (await veri.Repo<Parti>().Sorgu().Include(p => p.Bagisci).AsNoTracking()
                .Where(p => p.Aktif && p.Miktar > 0)
                .ToListAsync(iptal))
            .ToLookup(p => p.UrunId);

        return urunler.Select(u =>
        {
            var urunPartileri = partiler[u.Id].ToList();
            var fefo = FefoSecici.UygunPartiler(urunPartileri, bugun).Select((p, i) => (p.Id, Sira: i + 1)).ToDictionary(x => x.Id, x => x.Sira);
            var satirlar = urunPartileri
                .OrderBy(p => fefo.GetValueOrDefault(p.Id, int.MaxValue)).ThenBy(p => p.SonKullanmaTarihi)
                .Select(p => PartiSatiri(p, u, bugun, fefo.TryGetValue(p.Id, out var s) ? s : null))
                .ToList();
            return new UrunStokDurumu
            {
                UrunId = u.Id, UrunAdi = u.Ad, Birim = u.Birim, KritikSeviye = u.KritikSeviye,
                FizikselMiktar = urunPartileri.Sum(p => p.Miktar),
                AyrilanMiktar = urunPartileri.Sum(p => p.AyrilanMiktar),
                KullanilabilirMiktar = urunPartileri.Sum(p => p.KullanilabilirMiktar(bugun)),
                SktGecmisMiktar = urunPartileri.Where(p => p.SktGectiMi(bugun)).Sum(p => p.Miktar - p.AyrilanMiktar),
                Partiler = satirlar,
            };
        }).ToList();
    }

    private PartiDurumu PartiSatiri(Parti p, Urun u, DateOnly bugun, int? fefoSirasi) => new()
    {
        Id = p.Id, UrunId = u.Id, UrunAdi = u.Ad, PartiNo = p.PartiNo, BagisciAdi = p.Bagisci?.Ad, GirisZamani = p.GirisZamani,
        SonKullanmaTarihi = p.SonKullanmaTarihi, Miktar = p.Miktar, AyrilanMiktar = p.AyrilanMiktar,
        KullanilabilirMiktar = p.KullanilabilirMiktar(bugun), SktGecti = p.SktGectiMi(bugun),
        SktYaklasiyor = p.SktYaklasiyorMu(bugun, ayarlar.SktUyariGunu), FefoSirasi = fefoSirasi, Aktif = p.Aktif,
    };

    public async Task<IReadOnlyList<StokHareketSatiri>> HareketleriListeleAsync(StokHareketFiltresi filtre, CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Gorme);
        ArgumentNullException.ThrowIfNull(filtre);
        await using var veri = veriFabrikasi.Ac();

        var sorgu = veri.Repo<StokHareketi>().Sorgu().Include(h => h.Parti!.Urun).Include(h => h.Kullanici).AsNoTracking();
        if (!filtre.PasifleriDeGoster)
            sorgu = sorgu.Where(h => h.Aktif);
        if (filtre.UrunId is { } urunId)
            sorgu = sorgu.Where(h => h.Parti!.UrunId == urunId);
        if (filtre.Tur is { } tur)
            sorgu = sorgu.Where(h => EF.Property<StokHareketTuru>(h, "HareketTuru") == tur);
        if (filtre.Baslangic is { } bas)
            sorgu = sorgu.Where(h => h.Zaman >= bas.ToDateTime(TimeOnly.MinValue));
        if (filtre.Bitis is { } bit)
            sorgu = sorgu.Where(h => h.Zaman < bit.AddDays(1).ToDateTime(TimeOnly.MinValue));

        var hareketler = await sorgu.OrderByDescending(h => h.Zaman).ThenByDescending(h => h.Id).ToListAsync(iptal);

        var dagitimIdleri = hareketler.OfType<DagitimHareketi>().Select(h => h.Id).ToList();
        var refakatsizGorebilir = oturum.Rol == RolTuru.Koordinator;
        var dagitimBilgisi = await veri.Repo<StokHareketi>().Sorgu().OfType<DagitimHareketi>()
            .Where(h => dagitimIdleri.Contains(h.Id))
            .Select(h => new
            {
                h.Id, h.DagitimDetay!.Aile!.AileNo,
                Gizli = h.DagitimDetay.Aile.Uyeler.Any() && h.DagitimDetay.Aile.Uyeler.All(u => u.RefakatsizCocuk),
                TeslimAlan = h.TeslimAlanKisi == null ? null : h.TeslimAlanKisi.Ad + " " + h.TeslimAlanKisi.Soyad,
                TeslimAlanRefakatsiz = h.TeslimAlanKisi != null && h.TeslimAlanKisi.RefakatsizCocuk,
            })
            .ToDictionaryAsync(x => x.Id, iptal);

        return hareketler.Select(h =>
        {
            var d = dagitimBilgisi.GetValueOrDefault(h.Id);
            return new StokHareketSatiri
            {
                Id = h.Id, Zaman = h.Zaman, Tur = h.Tur, TurAdi = h.TurAdi, UrunAdi = h.Parti!.Urun!.Ad, PartiNo = h.Parti.PartiNo,
                Miktar = h.Miktar, StokEtkisi = h.StokEtkisi, Aciklama = h.Aciklama, KullaniciAdi = h.Kullanici?.KullaniciAdi,
                AileNo = d is null ? null : refakatsizGorebilir || !d.Gizli ? d.AileNo : Gosterim.GizliKayit,
                TeslimAlan = d is null ? null : refakatsizGorebilir || !d.TeslimAlanRefakatsiz ? d.TeslimAlan : Gosterim.GizliKayit,
                Aktif = h.Aktif,
            };
        }).ToList();
    }

    public async Task<IReadOnlyList<StokUyarisi>> UyarilariGetirAsync(CancellationToken iptal = default)
    {
        var durum = await StokDurumuAsync(iptal);
        var uyarilar = new List<StokUyarisi>();
        foreach (var u in durum)
        {
            var birim = u.Birim.Metin();
            if (u.KritikAltinda)
                uyarilar.Add(new StokUyarisi(StokUyariTuru.KritikSeviye, u.UrunId, u.UrunAdi, null,
                    $"{u.UrunAdi}: kullanılabilir stok {u.KullanilabilirMiktar:0.##} {birim}, kritik seviye {u.KritikSeviye:0.##} {birim}."));
            foreach (var p in u.Partiler.Where(p => p.SktGecti && p.Miktar > p.AyrilanMiktar))
                uyarilar.Add(new StokUyarisi(StokUyariTuru.SktGecti, u.UrunId, u.UrunAdi, p.Id,
                    $"{u.UrunAdi} – parti {p.PartiNo}: SKT {p.SonKullanmaTarihi:dd.MM.yyyy} tarihinde geçti; {p.Miktar - p.AyrilanMiktar:0.##} {birim} fireye düşülmeli."));
            foreach (var p in u.Partiler.Where(p => p.SktYaklasiyor && p.KullanilabilirMiktar > 0))
                uyarilar.Add(new StokUyarisi(StokUyariTuru.SktYaklasiyor, u.UrunId, u.UrunAdi, p.Id,
                    $"{u.UrunAdi} – parti {p.PartiNo}: SKT {p.SonKullanmaTarihi:dd.MM.yyyy} ({p.SonKullanmaTarihi!.Value.DayNumber - zaman.Bugun().DayNumber} gün kaldı), {p.KullanilabilirMiktar:0.##} {birim}."));
        }

        return uyarilar.OrderBy(u => u.Tur).ThenBy(u => u.UrunAdi).ToList();
    }

    public async Task<IReadOnlyList<StokTahmini>> TahminleriGetirAsync(CancellationToken iptal = default)
    {
        oturum.IzinGerekli(Modul.Depo, Izin.Gorme);
        await using var veri = veriFabrikasi.Ac();
        var bugun = zaman.Bugun();
        var donemBaslangici = bugun.AddDays(-(StokTahminMotoru.DonemGunu - 1)).ToDateTime(TimeOnly.MinValue);

        var urunler = await veri.Repo<Urun>().Sorgu().AsNoTracking().Where(u => u.Aktif).OrderBy(u => u.Ad).ToListAsync(iptal);
        var partiler = (await veri.Repo<Parti>().Sorgu().AsNoTracking().Where(p => p.Aktif && p.Miktar > 0).ToListAsync(iptal))
            .ToLookup(p => p.UrunId);
        var dagitimlar = (await veri.Repo<StokHareketi>().Sorgu().OfType<DagitimHareketi>()
                .Where(h => h.Aktif && h.Zaman >= donemBaslangici)
                .Select(h => new { h.Parti!.UrunId, h.Zaman, h.Miktar })
                .ToListAsync(iptal))
            .ToLookup(h => h.UrunId);
        var aileler = await veri.Repo<Aile>().Sorgu().Include(a => a.Uyeler).AsNoTracking().Where(a => a.Aktif).ToListAsync(iptal);

        return urunler.Select(u => StokTahminMotoru.Hesapla(
            u,
            partiler[u.Id].Sum(p => p.KullanilabilirMiktar(bugun)),
            dagitimlar[u.Id].Select(d => (DateOnly.FromDateTime(d.Zaman), d.Miktar)),
            aileler.Sum(a => HakHesaplayici.Hesapla(u, a.Uyeler, bugun)),
            bugun)).ToList();
    }
}
