using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.DataAccess.Konfigurasyonlar;
using AfetCadirKent.Entities.Depo;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Entities.Sistem;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class DagitimServisiTestleri(VeritabaniFixture fixture)
{
    private DagitimServisi Dagitim(RolTuru rol = RolTuru.DepoSorumlusu, TimeProvider? zaman = null) =>
        fixture.DagitimServisi(rol, zaman);

    private async Task<Aile> AileAsync(RolTuru rol, params (string Ad, int Yas, bool Refakatsiz)[] uyeler)
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(rol).AileOlusturAsync($"{soyad} Ailesi", false,
            uyeler.Select(u => Kisi(u.Ad, soyad, u.Yas, refakatsiz: u.Refakatsiz)).ToList());
        await using var db = fixture.YeniContext();
        aile.Uyeler = await db.Kisiler.AsNoTracking().Where(k => k.AileId == aile.Id).OrderBy(k => k.Id).ToListAsync();
        return aile;
    }

    private Task<Aile> AileAsync(params (string Ad, int Yas)[] uyeler) =>
        AileAsync(RolTuru.KayitGorevlisi, uyeler.Select(u => (u.Ad, u.Yas, false)).ToArray());

    private Task<Parti> GirisAsync(int urunId, decimal miktar, int? sktGun = null) =>
        fixture.StokServisi().StokGirisiAsync(new StokGirisiBilgisi
        {
            UrunId = urunId, PartiNo = BenzersizSoyad("P"), Miktar = miktar,
            SonKullanmaTarihi = sktGun is { } g ? DateOnly.FromDateTime(DateTime.Today).AddDays(g) : null,
        });

    private async Task<UrunStokDurumu> StokAsync(int urunId) =>
        (await fixture.StokServisi().StokDurumuAsync()).Single(u => u.UrunId == urunId);

    private static DagitimSatiri Satir(DagitimListesi liste, int aileId) => liste.Satirlar.Single(s => s.AileId == aileId);

    /// <summary>Veritabanına kaydedilmiş bir görevliyle açılmış oturum (teslim eden bilgisinin yazıldığını görmek için).</summary>
    private async Task<Oturum> KayitliGorevliOturumuAsync(RolTuru rol)
    {
        var gorevli = new Gorevli
        {
            Ad = "Depo", Soyad = "Görevlisi", KullaniciAdi = BenzersizSoyad("depo").ToLowerInvariant(),
            RolId = (int)rol, OlusturmaZamani = DateTime.Now,
        };
        gorevli.ParolaHashAta("test-ozeti");
        await using (var db = fixture.YeniContext())
        {
            db.Kullanicilar.Add(gorevli);
            await db.SaveChangesAsync();
        }

        var oturum = new Oturum();
        oturum.Ac(gorevli, rol, ReferansVerisi.YetkiMatrisi[rol]
            .SelectMany(m => m.Value.Select(i => new Yetki { RolId = (int)rol, Modul = m.Key, Izin = i })));
        return oturum;
    }

    // ---------------------------------------------------------------- Ana akış (senaryo 3)

    [Fact]
    public async Task MalzemeIhtiyaciOlanAileOnde_OnaydaAyrilir_KismiTeslimBekleyenKalir_IptalVeGunSonuStoguDondurur()
    {
        // Battaniye kategorisindeki yeni ürün, bebek ve 65 yaş üstü gerekçelerinin öncelikli malzemesi olur (elle eşleşme gerekmez).
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.KisiBasi, hak: 1, tekrar: 1, kategori: UrunKategorisi.Battaniye);
        await GirisAsync(urun.Id, 10_000);
        var normal = await AileAsync(("Ali", 40), ("Ayşe", 38));
        var bebekli = await AileAsync(("Anne", 30), ("Baba", 32), ("Bebek", 1));

        var oturum = await KayitliGorevliOturumuAsync(RolTuru.DepoSorumlusu);
        var servis = new DagitimServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, GerekceDegerlendirici.Varsayilan());
        var liste = await servis.ListeOlusturAsync(urun.Id, "Sabah dağıtımı");

        Assert.Equal(DagitimDurumu.Taslak, liste.Ozet.Durum);
        var b = Satir(liste, bebekli.Id);
        Assert.True(b.OncelikliIhtiyac);
        Assert.Equal(DestekOnceligi.Yuksek, b.DestekOnceligi);
        Assert.Equal(3, b.HakMiktari);
        Assert.False(Satir(liste, normal.Id).OncelikliIhtiyac);
        var sira = liste.Satirlar.Select(s => s.AileId).ToList();
        Assert.True(sira.IndexOf(bebekli.Id) < sira.IndexOf(normal.Id));
        Assert.True(liste.Satirlar.SkipWhile(s => s.OncelikliIhtiyac).All(s => !s.OncelikliIhtiyac));

        await servis.OnaylaAsync(liste.Ozet.Id);
        var stok = await StokAsync(urun.Id);
        Assert.Equal(liste.Ozet.ToplamHak, stok.AyrilanMiktar);
        Assert.Equal(10_000 - liste.Ozet.ToplamHak, stok.KullanilabilirMiktar);
        Assert.Equal(10_000, stok.FizikselMiktar);

        var bekleyen = await servis.TeslimEtAsync(b.DetayId, 2, bebekli.Uyeler[0].Id);
        Assert.Equal(1, bekleyen);
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.TeslimEtAsync(b.DetayId, 2, bebekli.Uyeler[0].Id));

        liste = await servis.ListeGetirAsync(liste.Ozet.Id);
        b = Satir(liste, bebekli.Id);
        Assert.Equal((3m, 2m, 1m, 1m), (b.HakMiktari, b.TeslimEdilenMiktar, b.BekleyenHak, b.AyrilanMiktar));
        var teslim = Assert.Single(b.Teslimler);
        Assert.Equal(2, teslim.Miktar);
        Assert.Equal(oturum.Kullanici!.KullaniciAdi, teslim.TeslimEden);
        Assert.Equal(bebekli.Uyeler[0].AdSoyad, teslim.TeslimAlan);
        Assert.True((DateTime.Now - teslim.Zaman).TotalMinutes < 5);

        stok = await StokAsync(urun.Id);
        Assert.Equal(9_998, stok.FizikselMiktar);
        Assert.Equal(liste.Ozet.ToplamHak - 2, stok.AyrilanMiktar);

        // Aile kalanını alamayacak: teslim iptali ayırmayı kaldırır, kalan hak bekleyen olarak durur.
        await servis.TeslimIptalAsync(b.DetayId, "Aile bugün gelemeyecek");
        b = Satir(await servis.ListeGetirAsync(liste.Ozet.Id), bebekli.Id);
        Assert.Equal((1m, 0m), (b.BekleyenHak, b.AyrilanMiktar));
        Assert.Equal(liste.Ozet.ToplamHak - 3, (await StokAsync(urun.Id)).AyrilanMiktar);
        var hata = await Assert.ThrowsAsync<IsKuraliException>(() => servis.TeslimEtAsync(b.DetayId, 1, bebekli.Uyeler[0].Id));
        Assert.Contains("ayırması kaldırılmış", hata.Message);

        // Gün sonu: teslim edilmeyen tüm ayırmalar kullanılabilir stoğa döner.
        var sonuc = await servis.GunSonuAsync();
        Assert.True(sonuc.KapananListe >= 1);
        stok = await StokAsync(urun.Id);
        Assert.Equal(0, stok.AyrilanMiktar);
        Assert.Equal(9_998, stok.KullanilabilirMiktar);
        var ozet = (await servis.ListeleriGetirAsync(DateOnly.FromDateTime(DateTime.Today))).Single(l => l.Id == liste.Ozet.Id);
        Assert.Equal((DagitimDurumu.Kapandi, 0m, 2m), (ozet.Durum, ozet.AcikAyrilan, ozet.ToplamTeslim));
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.TeslimEtAsync(b.DetayId, 1, bebekli.Uyeler[0].Id));

        await using var db = fixture.YeniContext();
        var hareket = await db.StokHareketleri.OfType<DagitimHareketi>().SingleAsync(h => h.DagitimDetayId == b.DetayId);
        Assert.Equal((oturum.Kullanici.Id, bebekli.Uyeler[0].Id), (hareket.KullaniciId!.Value, hareket.TeslimAlanKisiId!.Value));
    }

    [Fact]
    public async Task Onay_FefoIleSktEnYakinPartidenAyirir_SktGecmisPartiKullanilmaz()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 1);
        var gecmis = await fixture.StokServisi(zaman: new SabitZaman(DateTime.Now.AddDays(-10))).StokGirisiAsync(new StokGirisiBilgisi
        {
            UrunId = urun.Id, PartiNo = "ESKI", Miktar = 1000, SonKullanmaTarihi = DateOnly.FromDateTime(DateTime.Today).AddDays(-1),
        });
        var uzak = await GirisAsync(urun.Id, 1000, 40);
        var yakin = await GirisAsync(urun.Id, 5, 3);

        var liste = await Dagitim().ListeOlusturAsync(urun.Id);
        await Dagitim().OnaylaAsync(liste.Ozet.Id);

        var partiler = (await StokAsync(urun.Id)).Partiler;
        Assert.Equal(5, partiler.Single(p => p.Id == yakin.Id).AyrilanMiktar);
        Assert.Equal(liste.Ozet.ToplamHak - 5, partiler.Single(p => p.Id == uzak.Id).AyrilanMiktar);
        Assert.Equal(0, partiler.Single(p => p.Id == gecmis.Id).AyrilanMiktar);

        await Dagitim().ListeIptalAsync(liste.Ozet.Id, "Test sonu");
        Assert.Equal(0, (await StokAsync(urun.Id)).AyrilanMiktar);
    }

    [Fact]
    public async Task YetersizStoktaOnayYapilmaz_HicbirSeyAyrilmaz()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 1);
        await GirisAsync(urun.Id, 1);
        var liste = await Dagitim().ListeOlusturAsync(urun.Id);

        var hata = await Assert.ThrowsAsync<IsKuraliException>(() => Dagitim().OnaylaAsync(liste.Ozet.Id));
        Assert.Contains("Yetersiz stok", hata.Message);
        Assert.Equal(0, (await StokAsync(urun.Id)).AyrilanMiktar);
        Assert.Equal(DagitimDurumu.Taslak, (await Dagitim().ListeGetirAsync(liste.Ozet.Id)).Ozet.Durum);

        await Dagitim().ListeIptalAsync(liste.Ozet.Id, "Stok yetersiz");
    }

    [Fact]
    public async Task TekrarAlmaSuresi_KismiTeslimDevreder_IstisnaGerekceyleYapilir()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 2, tekrar: 3);
        await GirisAsync(urun.Id, 10_000);
        var kismi = await AileAsync(("Kısmi", 40));
        var tam = await AileAsync(("Tam", 40));
        var gelmeyen = await AileAsync(("Gelmeyen", 40));

        // Gün 0: kısmi aile 1/2, tam aile 2/2 alır, üçüncü aile gelmez.
        var gun0 = await Dagitim().ListeOlusturAsync(urun.Id);
        await Dagitim().OnaylaAsync(gun0.Ozet.Id);
        await Dagitim().TeslimEtAsync(Satir(gun0, kismi.Id).DetayId, 1, kismi.Uyeler[0].Id);
        await Dagitim().TeslimEtAsync(Satir(gun0, tam.Id).DetayId, 2, tam.Uyeler[0].Id);
        await Dagitim().GunSonuAsync();

        // Gün 1: kısmi ailenin kalan 1'i devreder; tam aile süresi dolmadığı için listede yok; gelmeyen aile hakkını alır.
        var yarin = new SabitZaman(DateTime.Now.AddDays(1));
        var gun1 = await Dagitim(zaman: yarin).ListeOlusturAsync(urun.Id);
        Assert.Equal(1, Satir(gun1, kismi.Id).HakMiktari);
        Assert.Equal(2, Satir(gun1, gelmeyen.Id).HakMiktari);
        Assert.DoesNotContain(gun1.Satirlar, s => s.AileId == tam.Id);
        await Assert.ThrowsAsync<IsKuraliException>(() => Dagitim(zaman: yarin).ListeOlusturAsync(urun.Id)); // açık liste varken ikincisi yok

        var aday = (await Dagitim(zaman: yarin).EklenebilirAilelerAsync(gun1.Ozet.Id)).Single(a => a.AileId == tam.Id);
        Assert.True(aday.IstisnaGerekir);
        Assert.Equal(DateOnly.FromDateTime(DateTime.Today).AddDays(3), aday.SonrakiHakTarihi);

        var hata = await Assert.ThrowsAsync<DogrulamaException>(() => Dagitim(zaman: yarin).AileEkleAsync(gun1.Ozet.Id, tam.Id, null));
        Assert.Contains("gerekçe", hata.Message);
        await Dagitim(zaman: yarin).AileEkleAsync(gun1.Ozet.Id, tam.Id, "Çadırı su aldı, battaniyeleri ıslandı");
        var istisna = Satir(await Dagitim(zaman: yarin).ListeGetirAsync(gun1.Ozet.Id), tam.Id);
        Assert.Equal(2, istisna.HakMiktari);
        Assert.Equal("Çadırı su aldı, battaniyeleri ıslandı", istisna.IstisnaGerekcesi);
        await Assert.ThrowsAsync<IsKuraliException>(() => Dagitim(zaman: yarin).AileEkleAsync(gun1.Ozet.Id, tam.Id, "Tekrar"));
        await Dagitim(zaman: yarin).GunSonuAsync(); // onaylanmayan taslak iptal olur

        // Gün 3: süre doldu, tam aile yeniden hak alır.
        var gun3 = await Dagitim(zaman: new SabitZaman(DateTime.Now.AddDays(3))).ListeOlusturAsync(urun.Id);
        Assert.Equal(2, Satir(gun3, tam.Id).HakMiktari);
        Assert.Equal(3, Satir(gun3, kismi.Id).HakMiktari); // yeni hak 2 + gün 0'dan devreden 1 (gün 1 listesi iptal oldu)
        await Dagitim(zaman: new SabitZaman(DateTime.Now.AddDays(3))).GunSonuAsync();
    }

    [Fact]
    public async Task DevredenHak_AileGelmeseDeKaybolmaz_TeslimiTekrarAlmaSuresiniKaydirmaz()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 2, tekrar: 3);
        await GirisAsync(urun.Id, 10_000);
        var aile = await AileAsync(("Kısmi", 40));
        DagitimServisi Gun(int g) => Dagitim(zaman: new SabitZaman(DateTime.Now.AddDays(g)));

        // Gün 0: 2'nin 1'i teslim; 1 devreder.
        var gun0 = await Gun(0).ListeOlusturAsync(urun.Id);
        await Gun(0).OnaylaAsync(gun0.Ozet.Id);
        await Gun(0).TeslimEtAsync(Satir(gun0, aile.Id).DetayId, 1, aile.Uyeler[0].Id);
        await Gun(0).GunSonuAsync();

        // Gün 1: yalnız devreden 1 listede; aile gelmez, liste kapanır.
        var gun1 = await Gun(1).ListeOlusturAsync(urun.Id);
        Assert.Equal(1, Satir(gun1, aile.Id).HakMiktari);
        await Gun(1).OnaylaAsync(gun1.Ozet.Id);
        await Gun(1).GunSonuAsync();

        // Gün 2: devreden kaybolmadı; teslim alınır.
        var gun2 = await Gun(2).ListeOlusturAsync(urun.Id);
        Assert.Equal(1, Satir(gun2, aile.Id).HakMiktari);
        await Gun(2).OnaylaAsync(gun2.Ozet.Id);
        await Gun(2).TeslimEtAsync(Satir(gun2, aile.Id).DetayId, 1, aile.Uyeler[0].Id);
        await Gun(2).GunSonuAsync();

        // Gün 3: süre gün 0'dan sayılır (devredenin gün 2'deki teslimi süreyi kaydırmaz) → yeni hak 2.
        var gun3 = await Gun(3).ListeOlusturAsync(urun.Id);
        Assert.Equal(2, Satir(gun3, aile.Id).HakMiktari);
        await Gun(3).GunSonuAsync();

        await using var db = fixture.YeniContext();
        var satirlar = await db.DagitimDetaylari.Where(d => d.AileId == aile.Id).OrderBy(d => d.Id).ToListAsync();
        Assert.Equal([0m, 1m, 1m, 0m], satirlar.Select(d => d.DevredenMiktar));
    }

    [Fact]
    public async Task BirUrundeIkinciAcikListeVeritabanindaEngellenir()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 1);
        var liste = await Dagitim().ListeOlusturAsync(urun.Id);

        // Servis kontrolünü atlayan eşzamanlı bir ikinci liste (yarış durumu) veritabanı indeksine takılır.
        await using (var db = fixture.YeniContext())
        {
            db.Dagitimlar.Add(new Dagitim { UrunId = urun.Id, Tarih = DateOnly.FromDateTime(DateTime.Today), OlusturmaZamani = DateTime.Now });
            await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
        }

        // İptal edilen veya kapanan liste engel olmaz.
        await Dagitim().ListeIptalAsync(liste.Ozet.Id, "Test");
        var yeni = await Dagitim().ListeOlusturAsync(urun.Id);
        await Dagitim().ListeIptalAsync(yeni.Ozet.Id, "Test sonu");
    }

    [Fact]
    public async Task HastanedekiUyeninHakkiAiledeKalir_TeslimiAiledenBuradakiBiriAlir()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.KisiBasi, hak: 3);
        await GirisAsync(urun.Id, 10_000);
        var aile = await AileAsync(("Anne", 40), ("Dede", 75));
        var baska = await AileAsync(("Komşu", 30));
        await fixture.KayitServisi(RolTuru.KayitGorevlisi).DurumDegistirAsync(aile.Uyeler[1].Id, KisiDurumu.Hastanede, "Sevk edildi");

        var liste = await Dagitim().ListeOlusturAsync(urun.Id);
        var satir = Satir(liste, aile.Id);
        Assert.Equal(6, satir.HakMiktari);
        Assert.Equal([aile.Uyeler[0].Id], satir.TeslimAlabilecekler.Select(k => k.KisiId));
        await Dagitim().OnaylaAsync(liste.Ozet.Id);

        var hata = await Assert.ThrowsAsync<IsKuraliException>(() => Dagitim().TeslimEtAsync(satir.DetayId, 3, aile.Uyeler[1].Id));
        Assert.Contains("çadır kentte değil", hata.Message);
        await Assert.ThrowsAsync<IsKuraliException>(() => Dagitim().TeslimEtAsync(satir.DetayId, 3, baska.Uyeler[0].Id));
        Assert.Equal(0, await Dagitim().TeslimEtAsync(satir.DetayId, 6, aile.Uyeler[0].Id));

        await Assert.ThrowsAsync<IsKuraliException>(() => Dagitim().ListeIptalAsync(liste.Ozet.Id, "Teslim yapılmış"));
        await Dagitim().GunSonuAsync();
    }

    [Fact]
    public async Task TaslaktanSatirCikarilir_OnaydaAyrilmaz_OnayliListedenCikarilamaz()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 1);
        await GirisAsync(urun.Id, 10_000);
        var aile = await AileAsync(("Tek", 50));

        var liste = await Dagitim().ListeOlusturAsync(urun.Id);
        var satir = Satir(liste, aile.Id);
        await Assert.ThrowsAsync<DogrulamaException>(() => Dagitim().SatirCikarAsync(satir.DetayId, ""));
        await Dagitim().SatirCikarAsync(satir.DetayId, "Aile başka kampa nakil olacak");

        liste = await Dagitim().ListeGetirAsync(liste.Ozet.Id);
        Assert.False(Satir(liste, aile.Id).Aktif);
        Assert.Equal(liste.Satirlar.Count - 1, liste.Ozet.SatirSayisi);
        Assert.False(liste.Satirlar[^1].Aktif); // pasif satır en sonda

        await Dagitim().OnaylaAsync(liste.Ozet.Id);
        Assert.Equal(liste.Ozet.ToplamHak, (await StokAsync(urun.Id)).AyrilanMiktar);
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => Dagitim().SatirCikarAsync(satir.DetayId, "Tekrar"));
        var digerSatir = liste.Satirlar.First(s => s.Aktif && !s.KoordinatorTakibinde).DetayId;
        await Assert.ThrowsAsync<IsKuraliException>(() => Dagitim().SatirCikarAsync(digerSatir, "Onaylı"));

        await using (var db = fixture.YeniContext())
            Assert.True(await db.DagitimDetaylari.AnyAsync(d => d.Id == satir.DetayId)); // silinmedi, pasif

        await Dagitim().ListeIptalAsync(liste.Ozet.Id, "Test sonu");
        await using var db2 = fixture.YeniContext();
        var iptal = await db2.Dagitimlar.SingleAsync(d => d.Id == liste.Ozet.Id);
        Assert.Equal((DagitimDurumu.Iptal, false), (iptal.Durum, iptal.Aktif));
    }

    [Fact]
    public async Task RefakatsizCocukAilesi_KoordinatorDisindaGizlenir_TeslimiKoordinatorYapar()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.KisiBasi, hak: 1);
        await GirisAsync(urun.Id, 10_000);
        var cocuk = await AileAsync(RolTuru.Koordinator, ("Çocuk", 12, true));

        var liste = await Dagitim().ListeOlusturAsync(urun.Id);
        var gizli = Satir(liste, cocuk.Id);
        Assert.True(gizli.KoordinatorTakibinde);
        Assert.Equal(Gosterim.GizliKayit, gizli.AileAdi);
        Assert.DoesNotContain(cocuk.AileNo, gizli.AileNo);
        Assert.Empty(gizli.TeslimAlabilecekler);

        await Dagitim().OnaylaAsync(liste.Ozet.Id);
        await Assert.ThrowsAsync<YetkisizIslemException>(() => Dagitim().TeslimEtAsync(gizli.DetayId, 1, cocuk.Uyeler[0].Id));

        var koordinator = Satir(await Dagitim(RolTuru.Koordinator).ListeGetirAsync(liste.Ozet.Id), cocuk.Id);
        Assert.Equal(cocuk.AileNo, koordinator.AileNo);
        Assert.Equal(0, await Dagitim(RolTuru.Koordinator).TeslimEtAsync(koordinator.DetayId, 1, cocuk.Uyeler[0].Id));
        await Dagitim().GunSonuAsync();
    }

    [Fact]
    public async Task Yetki_DagitimiYalnizDepoVeKoordinatorYapar()
    {
        var urun = await fixture.TestUrunuAsync();
        await Assert.ThrowsAsync<YetkisizIslemException>(() => Dagitim(RolTuru.KayitGorevlisi).ListeOlusturAsync(urun.Id));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => Dagitim(RolTuru.SahaGorevlisi).ListeleriGetirAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() => Dagitim(RolTuru.SaglikPersoneli).GunSonuAsync());

        var liste = await Dagitim(RolTuru.Koordinator).ListeOlusturAsync(urun.Id);
        await Dagitim(RolTuru.Koordinator).ListeIptalAsync(liste.Ozet.Id, "Test sonu");
    }
}
