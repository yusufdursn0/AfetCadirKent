using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Depo;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class DepoServisTestleri(VeritabaniFixture fixture)
{
    private StokServisi Stok(TimeProvider? zaman = null) => fixture.StokServisi(zaman: zaman);

    private Task<Parti> GirisAsync(int urunId, decimal miktar, int? sktGun, TimeProvider? zaman = null) =>
        Stok(zaman).StokGirisiAsync(new StokGirisiBilgisi
        {
            UrunId = urunId, PartiNo = BenzersizSoyad("P"), Miktar = miktar,
            SonKullanmaTarihi = sktGun is { } g ? DateOnly.FromDateTime(DateTime.Today).AddDays(g) : null,
        });

    private async Task<Parti> PartiOkuAsync(int id)
    {
        await using var db = fixture.YeniContext();
        return await db.Partiler.AsNoTracking().SingleAsync(p => p.Id == id);
    }

    // ---------------------------------------------------------------- Ürün

    [Fact]
    public async Task UrunAyarlariGuncellenir_AyniAdlaIkinciUrunEklenemez()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.KisiBasi, hak: 3, tekrar: 1, kritik: 500);
        var servis = fixture.UrunServisi();

        await servis.UrunKaydetAsync(urun.Id, new UrunBilgisi
        {
            Ad = urun.Ad, Kategori = UrunKategorisi.Su, Birim = Birim.Litre, HesaplamaTuru = HesaplamaTuru.AileBasi,
            HakMiktari = 10, TekrarAlmaGunu = 2, KritikSeviye = 100,
        });
        var guncel = (await servis.UrunleriListeleAsync()).Single(u => u.Id == urun.Id);
        Assert.Equal((HesaplamaTuru.AileBasi, 10m, 2, 100m), (guncel.HesaplamaTuru, guncel.HakMiktari, guncel.TekrarAlmaGunu, guncel.KritikSeviye));

        var hata = await Assert.ThrowsAsync<IsKuraliException>(() => servis.UrunKaydetAsync(null, new UrunBilgisi
        {
            Ad = urun.Ad, Kategori = UrunKategorisi.Su, Birim = Birim.Litre, HesaplamaTuru = HesaplamaTuru.KisiBasi,
            HakMiktari = 1, TekrarAlmaGunu = 1, KritikSeviye = 0,
        }));
        Assert.Contains("zaten var", hata.Message);
        await Assert.ThrowsAsync<DogrulamaException>(() => servis.UrunKaydetAsync(urun.Id, new UrunBilgisi
        {
            Ad = urun.Ad, Kategori = UrunKategorisi.Su, Birim = Birim.Litre, HesaplamaTuru = HesaplamaTuru.KisiBasi,
            HakMiktari = 0, TekrarAlmaGunu = 1, KritikSeviye = 0,
        }));

        await using var db = fixture.YeniContext();
        Assert.True(await db.IslemGecmisi.AnyAsync(i => i.VarlikAdi == nameof(Urun) && i.VarlikId == urun.Id && i.IslemTuru == IslemTuru.Guncelleme));
    }

    [Fact]
    public async Task YeniUrundeGerekceEslesmeleriKategoridenKurulur_KategoriDegisinceYenilenir()
    {
        async Task<List<GerekceKodu>> EslesmelerAsync(int urunId)
        {
            await using var db = fixture.YeniContext();
            return await db.GerekceUrunleri.Where(e => e.UrunId == urunId).Select(e => (GerekceKodu)e.GerekceId).OrderBy(k => k).ToListAsync();
        }

        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.BebekBasi, kategori: UrunKategorisi.Mama);
        Assert.Equal([GerekceKodu.Bebek], await EslesmelerAsync(urun.Id));

        var bilgi = new UrunBilgisi
        {
            Ad = urun.Ad, Kategori = UrunKategorisi.Hijyen, Birim = Birim.Paket, HesaplamaTuru = HesaplamaTuru.AileBasi,
            HakMiktari = 1, TekrarAlmaGunu = 7, KritikSeviye = 0,
        };
        await fixture.UrunServisi().UrunKaydetAsync(urun.Id, bilgi);
        Assert.Equal([GerekceKodu.Hamile, GerekceKodu.TekEbeveyn], await EslesmelerAsync(urun.Id));

        await fixture.UrunServisi().UrunKaydetAsync(urun.Id, bilgi with { Kategori = UrunKategorisi.Diger });
        Assert.Empty(await EslesmelerAsync(urun.Id));
    }

    [Fact]
    public async Task KullanilmamisUrunSilinir_PartisiOlanUrunSilinemezPasifYapilir()
    {
        var bos = await fixture.TestUrunuAsync();
        await fixture.UrunServisi().UrunSilAsync(bos.Id);
        await using (var db = fixture.YeniContext())
            Assert.False(await db.Urunler.AnyAsync(u => u.Id == bos.Id));

        var kullanilan = await fixture.TestUrunuAsync();
        await GirisAsync(kullanilan.Id, 10, null);
        var hata = await Assert.ThrowsAsync<SilinemezException>(() => fixture.UrunServisi().UrunSilAsync(kullanilan.Id));
        Assert.Equal("Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz.", hata.Message);

        await fixture.UrunServisi().UrunAktiflikDegistirAsync(kullanilan.Id, false);
        Assert.DoesNotContain(await fixture.UrunServisi().UrunleriListeleAsync(), u => u.Id == kullanilan.Id);
        await Assert.ThrowsAsync<IsKuraliException>(() => GirisAsync(kullanilan.Id, 5, null));
    }

    [Fact]
    public async Task BagisciStokGirisindenEklenirVeSecilir()
    {
        var ad = BenzersizSoyad("Bagisci");
        var bagisci = await fixture.UrunServisi().BagisciKaydetAsync(null, ad, "0555 111 22 33", null);
        await Assert.ThrowsAsync<IsKuraliException>(() => fixture.UrunServisi().BagisciKaydetAsync(null, ad, null, null));

        var urun = await fixture.TestUrunuAsync();
        var parti = await Stok().StokGirisiAsync(new StokGirisiBilgisi { UrunId = urun.Id, PartiNo = "B-1", Miktar = 20, BagisciId = bagisci.Id });

        var durum = (await Stok().StokDurumuAsync()).Single(u => u.UrunId == urun.Id);
        Assert.Equal(ad, durum.Partiler.Single(p => p.Id == parti.Id).BagisciAdi);
    }

    // ---------------------------------------------------------------- Stok girişi ve hareketler

    [Fact]
    public async Task StokGirisi_PartiVeGirisHareketiOlusur_SktGecmisUrunGirilemez()
    {
        var urun = await fixture.TestUrunuAsync();
        var parti = await GirisAsync(urun.Id, 40, 30);

        Assert.Equal(40, (await PartiOkuAsync(parti.Id)).Miktar);
        var hareketler = await Stok().HareketleriListeleAsync(new StokHareketFiltresi { UrunId = urun.Id });
        var giris = Assert.Single(hareketler);
        Assert.Equal((StokHareketTuru.Giris, 40m, 40m), (giris.Tur, giris.Miktar, giris.StokEtkisi));

        await Assert.ThrowsAsync<DogrulamaException>(() => GirisAsync(urun.Id, 10, -1));
        await Assert.ThrowsAsync<DogrulamaException>(() => GirisAsync(urun.Id, 0, null));
    }

    [Fact]
    public async Task IadeHasarVeSayimFarki_StoguDogruDegistirir()
    {
        var urun = await fixture.TestUrunuAsync();
        var parti = await GirisAsync(urun.Id, 100, null);

        await Stok().IadeAsync(parti.Id, 5, "Aile fazla battaniyeyi geri getirdi");
        await Stok().HasarAsync(parti.Id, 8, "Islandı");
        var eksik = await Stok().SayimAsync(parti.Id, 95, "Haftalık sayım");
        Assert.False(eksik.FazlaMi);
        var fazla = await Stok().SayimAsync(parti.Id, 96, "Tekrar sayım");
        Assert.True(fazla.FazlaMi);

        Assert.Equal(96, (await PartiOkuAsync(parti.Id)).Miktar);
        await Assert.ThrowsAsync<IsKuraliException>(() => Stok().SayimAsync(parti.Id, 96, "Fark yok"));
        await Assert.ThrowsAsync<IsKuraliException>(() => Stok().HasarAsync(parti.Id, 1000, "Fazla"));
        await Assert.ThrowsAsync<DogrulamaException>(() => Stok().HasarAsync(parti.Id, 1, " "));

        var turler = (await Stok().HareketleriListeleAsync(new StokHareketFiltresi { UrunId = urun.Id })).Select(h => h.Tur).ToList();
        Assert.Equal(5, turler.Count);
        Assert.Contains(StokHareketTuru.Iade, turler);
        Assert.Contains(StokHareketTuru.Hasar, turler);
        Assert.Equal(2, turler.Count(t => t == StokHareketTuru.SayimFarki));
    }

    [Fact]
    public async Task HataliHareketPasifYapilir_EtkisiGeriAlinir_KayitSilinmez()
    {
        var urun = await fixture.TestUrunuAsync();
        var parti = await GirisAsync(urun.Id, 50, null);
        await Stok().HasarAsync(parti.Id, 10, "Yanlış girildi");
        var hasar = (await Stok().HareketleriListeleAsync(new StokHareketFiltresi { UrunId = urun.Id, Tur = StokHareketTuru.Hasar })).Single();

        var girisId = (await Stok().HareketleriListeleAsync(new StokHareketFiltresi { UrunId = urun.Id, Tur = StokHareketTuru.Giris })).Single().Id;
        await Assert.ThrowsAsync<IsKuraliException>(() => Stok().HareketPasifYapAsync(girisId, "Parti hatalı"));

        await Stok().HareketPasifYapAsync(hasar.Id, "Hasar yanlış partiye girildi");
        Assert.Equal(50, (await PartiOkuAsync(parti.Id)).Miktar);
        await Assert.ThrowsAsync<IsKuraliException>(() => Stok().HareketPasifYapAsync(hasar.Id, "Tekrar"));

        Assert.DoesNotContain(await Stok().HareketleriListeleAsync(new StokHareketFiltresi { UrunId = urun.Id }), h => h.Id == hasar.Id);
        var tum = await Stok().HareketleriListeleAsync(new StokHareketFiltresi { UrunId = urun.Id, PasifleriDeGoster = true });
        Assert.False(tum.Single(h => h.Id == hasar.Id).Aktif);
    }

    [Fact]
    public async Task SktGecenParti_KullanilabilireSayilmaz_UyariVerir_FireyeDusulur()
    {
        var urun = await fixture.TestUrunuAsync(kritik: 25);
        var gecmis = new SabitZaman(DateTime.Now.AddDays(-10));
        var eski = await Stok(gecmis).StokGirisiAsync(new StokGirisiBilgisi
        {
            UrunId = urun.Id, PartiNo = "ESKI", Miktar = 30, SonKullanmaTarihi = DateOnly.FromDateTime(DateTime.Today).AddDays(-2),
        });
        var yakin = await GirisAsync(urun.Id, 20, 3);
        var uzak = await GirisAsync(urun.Id, 10, 60);

        var durum = (await Stok().StokDurumuAsync()).Single(u => u.UrunId == urun.Id);
        Assert.Equal(60, durum.FizikselMiktar);
        Assert.Equal(30, durum.KullanilabilirMiktar);
        Assert.Equal(30, durum.SktGecmisMiktar);
        Assert.True(durum.KritikAltinda == false);
        Assert.Equal([yakin.Id, uzak.Id, eski.Id], durum.Partiler.Select(p => p.Id));
        Assert.Equal([1, 2], durum.Partiler.Where(p => p.FefoSirasi != null).Select(p => p.FefoSirasi!.Value));
        Assert.Null(durum.Partiler.Single(p => p.Id == eski.Id).FefoSirasi);

        var uyarilar = (await Stok().UyarilariGetirAsync()).Where(u => u.UrunId == urun.Id).ToList();
        Assert.Contains(uyarilar, u => u.Tur == StokUyariTuru.SktGecti && u.PartiId == eski.Id);
        Assert.Contains(uyarilar, u => u.Tur == StokUyariTuru.SktYaklasiyor && u.PartiId == yakin.Id);
        Assert.DoesNotContain(uyarilar, u => u.PartiId == uzak.Id);

        Assert.True(await Stok().SktGecenleriFireyeDusAsync() >= 1);
        Assert.Equal(0, (await PartiOkuAsync(eski.Id)).Miktar);
        var fire = (await Stok().HareketleriListeleAsync(new StokHareketFiltresi { UrunId = urun.Id, Tur = StokHareketTuru.Hasar })).Single();
        Assert.Equal(30, fire.Miktar);
        Assert.Contains("SKT geçti", fire.Aciklama);

        await Stok().HasarAsync(yakin.Id, 6, "Kırıldı");
        Assert.Contains(await Stok().UyarilariGetirAsync(), u => u.UrunId == urun.Id && u.Tur == StokUyariTuru.KritikSeviye);
    }

    [Fact]
    public async Task Tahmin_DagitimYoksaHesaplanamiyor_HakaGoreHesaplanir()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 1, tekrar: 7);
        await GirisAsync(urun.Id, 70, null);

        var tahmin = (await Stok().TahminleriGetirAsync()).Single(t => t.UrunId == urun.Id);
        Assert.Equal(70, tahmin.KullanilabilirMiktar);
        Assert.False(tahmin.TuketimeGore.HesaplanabildiMi);
        Assert.Equal("Dönem: dağıtım verisi yok", tahmin.DonemMetni);
        Assert.True(tahmin.HakaGore.HesaplanabildiMi); // demo verisinde aileler var
    }

    [Fact]
    public async Task AyniStokEsZamanliIkiKisiyeAyrilamaz_IkinciyeAnlasilirMesaj()
    {
        var urun = await fixture.TestUrunuAsync();
        var parti = await GirisAsync(urun.Id, 10, null);
        var bugun = DateOnly.FromDateTime(DateTime.Today);
        var fabrika = fixture.OturumFabrikasi();

        await using var birinci = fabrika.Ac();
        await using var ikinci = fabrika.Ac();
        var p1 = await birinci.Repo<Parti>().BulAsync(parti.Id);
        var p2 = await ikinci.Repo<Parti>().BulAsync(parti.Id);
        p1!.Ayir(8, bugun);
        p2!.Ayir(8, bugun); // ikisi de 10 birimlik eski değeri gördü

        await birinci.GuvenliKaydetAsync();
        var hata = await Assert.ThrowsAsync<EszamanlilikException>(() => ikinci.GuvenliKaydetAsync());
        Assert.Contains("başka bir kullanıcı", hata.Message);
        Assert.Equal(8, (await PartiOkuAsync(parti.Id)).AyrilanMiktar);

        p1.AyirmaKaldir(8);
        await birinci.GuvenliKaydetAsync();
    }

    [Fact]
    public async Task Yetki_DepoDisindakiRollerStokGoremezVeDegistiremez()
    {
        var urun = await fixture.TestUrunuAsync();
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.StokServisi(RolTuru.SahaGorevlisi).StokDurumuAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.StokServisi(RolTuru.KayitGorevlisi).StokGirisiAsync(
            new StokGirisiBilgisi { UrunId = urun.Id, PartiNo = "X", Miktar = 1 }));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.UrunServisi(RolTuru.SaglikPersoneli).UrunSilAsync(urun.Id));
        Assert.NotEmpty(await fixture.StokServisi(RolTuru.Koordinator).StokDurumuAsync());
    }
}
