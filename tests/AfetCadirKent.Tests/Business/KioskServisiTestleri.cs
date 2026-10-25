using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.KioskIslemleri;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Entities.Sistem;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class KioskServisiTestleri(VeritabaniFixture fixture)
{
    private const int AnneYasi = 30;

    private static int AnneDogumYili => DateTime.Today.AddYears(-AnneYasi).AddDays(-10).Year;

    private async Task<KioskServisi> KioskAsync(TimeProvider? zaman = null, ITalepSiniflandirmaKuyrugu? kuyruk = null) =>
        new(fixture.OturumFabrikasi(), await fixture.KullaniciOturumuAsync(RolTuru.Kiosk), zaman ?? TimeProvider.System,
            kuyruk ?? new SahteKuyruk());

    /// <summary>Teste özel bölgede tek çadıra yerleşmiş aile ve aile numarası.</summary>
    private async Task<(int AileId, string AileNo, int CadirId, int CocukId)> CadirdakiAileAsync()
    {
        var cadirServisi = fixture.CadirServisi(RolTuru.KayitGorevlisi);
        var bolge = await cadirServisi.BolgeKaydetAsync(null, BenzersizBolgeKodu(), "Kiosk testi", null);
        var cadir = await cadirServisi.CadirEkleAsync(new CadirBilgisi { BolgeId = bolge.Id, Sira = 1, Numara = 1, Kapasite = 4 });
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Anne", soyad, AnneYasi), Kisi("Çocuk", soyad, 4)]);
        await cadirServisi.YerlestirAsync(aile.Id, [new CadirAtamasi(cadir.Id, "?", 2)]);

        await using var db = fixture.YeniContext();
        var aileNo = await db.Aileler.Where(a => a.Id == aile.Id).Select(a => a.AileNo).SingleAsync();
        var cocukId = await db.Kisiler.Where(k => k.AileId == aile.Id && k.Ad == "Çocuk").Select(k => k.Id).SingleAsync();
        return (aile.Id, aileNo, cadir.Id, cocukId);
    }

    [Fact]
    public async Task AileGirisi_AileNoVeUyeninDogumYiliylaYapilir_HataliBilgiAyniMesajiVerir()
    {
        var (aileId, aileNo, _, _) = await CadirdakiAileAsync();
        var kiosk = await KioskAsync();

        var hataliYil = await Assert.ThrowsAsync<IsKuraliException>(() => kiosk.AileGirisiAsync(aileNo, AnneDogumYili - 5));
        var olmayanAile = await Assert.ThrowsAsync<IsKuraliException>(() => kiosk.AileGirisiAsync("A-999999", AnneDogumYili));
        Assert.Equal(hataliYil.Message, olmayanAile.Message);
        Assert.Null(kiosk.Aile);

        await Assert.ThrowsAsync<DogrulamaException>(() => kiosk.AileGirisiAsync(" ", AnneDogumYili));
        await Assert.ThrowsAsync<DogrulamaException>(() => kiosk.AileGirisiAsync(aileNo, 85));

        // Küçük harf ve boşluklar önemsenmez; çadır/iletişim bilgisi dönmez.
        var aile = await kiosk.AileGirisiAsync($"  {aileNo.ToLowerInvariant()} ", AnneDogumYili);
        Assert.Equal((aileId, aileNo), (aile.AileId, aile.AileNo));
        Assert.Same(aile, kiosk.Aile);

        kiosk.AileCikisi();
        Assert.Null(kiosk.Aile);
        await Assert.ThrowsAsync<IsKuraliException>(() => kiosk.TaleplerimAsync());
    }

    [Fact]
    public async Task AileGirisi_PasifUyeninDogumYiliylaYapilamaz()
    {
        var (_, aileNo, _, cocukId) = await CadirdakiAileAsync();
        var cocukYili = DateTime.Today.AddYears(-4).AddDays(-10).Year;
        var kiosk = await KioskAsync();
        Assert.NotNull(await kiosk.AileGirisiAsync(aileNo, cocukYili));

        await fixture.KayitServisi(RolTuru.KayitGorevlisi).KisiPasifYapAsync(cocukId, "Mükerrer kayıt");
        await Assert.ThrowsAsync<IsKuraliException>(() => kiosk.AileGirisiAsync(aileNo, cocukYili));
    }

    [Fact]
    public async Task TalepBirak_KioskKaynakliSiniflandirilmamisAcilir_AileYalnizcaKendiTaleplerini_Gorur()
    {
        var (aileId, aileNo, cadirId, _) = await CadirdakiAileAsync();
        var (_, baskaAileNo, _, _) = await CadirdakiAileAsync();
        var kuyruk = new SahteKuyruk();
        var kiosk = await KioskAsync(kuyruk: kuyruk);

        await Assert.ThrowsAsync<IsKuraliException>(() => kiosk.TalepBirakAsync("Battaniye lazım"));

        await kiosk.AileGirisiAsync(baskaAileNo, AnneDogumYili);
        var baskaTalep = await kiosk.TalepBirakAsync("Başka ailenin talebi");
        kiosk.AileCikisi();

        await kiosk.AileGirisiAsync(aileNo, AnneDogumYili);
        await Assert.ThrowsAsync<DogrulamaException>(() => kiosk.TalepBirakAsync("  "));
        var talep = await kiosk.TalepBirakAsync("Çadırın fermuarı bozuldu");
        Assert.Equal((TalepDurumu.Yeni, (TalepTuru?)null, "Sınıflandırılmamış", "Yeni"),
            (talep.Durum, talep.Tur, talep.TurMetni, talep.DurumMetni));
        Assert.Equal([baskaTalep.Id, talep.Id], kuyruk.Eklenenler); // yapay zeka sınıflandırmasına sıraya alındı (K-080)

        var taleplerim = await kiosk.TaleplerimAsync();
        Assert.Equal([talep.Id], taleplerim.Select(t => t.Id));
        Assert.DoesNotContain(taleplerim, t => t.Id == baskaTalep.Id);

        // Görevli tarafında talep kiosk kaynaklı, sınıflandırılmamış ve ailenin çadırıyla görünür.
        var koordinator = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.Koordinator));
        var ozet = await koordinator.GetirAsync(talep.Id);
        Assert.Equal((TalepKaynagi.Kiosk, SiniflandirmaDurumu.Siniflandirilmamis, (int?)aileId, (int?)cadirId),
            (ozet.Kaynak, ozet.SiniflandirmaDurumu, ozet.AileId, ozet.CadirId));
        var kayit = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.KayitGorevlisi));
        Assert.Contains(await kayit.ListeleAsync(new TalepFiltresi { SadeceSiniflandirilmamis = true }), t => t.Id == talep.Id);

        // Durum değişikliği kioskta görünür.
        await koordinator.SiniflandirAsync(talep.Id, TalepTuru.CadirAriza, DestekOnceligi.Normal);
        Assert.Equal("Çadır/arıza", (await kiosk.TaleplerimAsync()).Single().TurMetni);

        await using var db = fixture.YeniContext();
        Assert.True(await db.IslemGecmisi.AnyAsync(i => i.VarlikAdi == "Talep" && i.VarlikId == talep.Id
            && i.IslemTuru == IslemTuru.Olusturma && i.Aciklama.Contains(aileNo)));
    }

    [Fact]
    public async Task ArtArdaHataliGiris_KisaSureBekletir_SureDolunca_GirisYapilir()
    {
        var (_, aileNo, _, _) = await CadirdakiAileAsync();
        var zaman = new IlerleyenZaman(DateTime.Now);
        var kiosk = await KioskAsync(zaman);

        for (var i = 0; i < KioskServisi.HataliGirisSiniri; i++)
            await Assert.ThrowsAsync<IsKuraliException>(() => kiosk.AileGirisiAsync(aileNo, AnneDogumYili - 1));

        var bekle = await Assert.ThrowsAsync<IsKuraliException>(() => kiosk.AileGirisiAsync(aileNo, AnneDogumYili));
        Assert.Contains("saniye sonra", bekle.Message);

        zaman.Ilerlet(KioskServisi.BeklemeSuresi + TimeSpan.FromSeconds(1));
        Assert.NotNull(await kiosk.AileGirisiAsync(aileNo, AnneDogumYili));
    }

    [Fact]
    public async Task Duyurular_YalnizcaYayindaOlanAktifDuyurularGorunur()
    {
        var onEk = BenzersizSoyad("Duyuru");
        var simdi = DateTime.Now;
        await using (var db = fixture.YeniContext())
        {
            db.Duyurular.AddRange(
                new Duyuru { Baslik = $"{onEk} yayında", Metin = "Su dağıtımı 10:00'da", BaslangicTarihi = simdi.AddDays(-1), BitisTarihi = simdi.AddDays(1) },
                new Duyuru { Baslik = $"{onEk} süresi dolmuş", Metin = "Eski", BaslangicTarihi = simdi.AddDays(-5), BitisTarihi = simdi.AddDays(-1) },
                new Duyuru { Baslik = $"{onEk} ileride", Metin = "Yarın", BaslangicTarihi = simdi.AddDays(1), BitisTarihi = simdi.AddDays(2) },
                new Duyuru { Baslik = $"{onEk} pasif", Metin = "Kaldırıldı", BaslangicTarihi = simdi.AddDays(-1), BitisTarihi = simdi.AddDays(1), Aktif = false });
            await db.SaveChangesAsync();
        }

        var duyurular = await (await KioskAsync()).DuyurulariGetirAsync();
        Assert.Equal([$"{onEk} yayında"], duyurular.Where(d => d.Baslik.StartsWith(onEk)).Select(d => d.Baslik));
    }

    [Fact]
    public async Task Yetki_KioskRoluTalepEkraniniKullanamaz_GorevliKioskIslemiYapamaz()
    {
        var kioskOturumu = await fixture.KullaniciOturumuAsync(RolTuru.Kiosk);
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.TalepServisi(kioskOturumu).ListeleAsync(new TalepFiltresi()));

        var gorevliKiosku = new KioskServisi(fixture.OturumFabrikasi(), OturumAc(RolTuru.KayitGorevlisi), TimeProvider.System, new SahteKuyruk());
        await Assert.ThrowsAsync<YetkisizIslemException>(() => gorevliKiosku.AileGirisiAsync("A-000001", 1980));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => gorevliKiosku.DuyurulariGetirAsync());
    }

    /// <summary>Elle ilerletilebilen zaman (bekleme süresi testi için).</summary>
    private sealed class IlerleyenZaman(DateTime baslangic) : TimeProvider
    {
        private DateTimeOffset _simdi = new DateTimeOffset(baslangic).ToUniversalTime();

        public void Ilerlet(TimeSpan sure) => _simdi += sure;

        public override DateTimeOffset GetUtcNow() => _simdi;
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Local;
    }
}
