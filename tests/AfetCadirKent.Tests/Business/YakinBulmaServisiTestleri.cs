using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.YakinBulmaIslemleri;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class YakinBulmaServisiTestleri(VeritabaniFixture fixture)
{
    private YakinBulmaServisi Servis(RolTuru rol = RolTuru.KayitGorevlisi) => fixture.YakinBulmaServisi(rol);

    private Task<YakinBulmaOzeti> BasvuruAsync(string ad, string soyad, int? yas = null, string? memleket = null,
        RolTuru rol = RolTuru.KayitGorevlisi) =>
        Servis(rol).BasvuruAcAsync(new YakinBulmaBilgisi
        {
            BasvuranAdSoyad = "Ayşe Arayan", BasvuranTelefon = "0555 111 22 33",
            ArananAd = ad, ArananSoyad = soyad, ArananYaklasikYas = yas, ArananMemleket = memleket,
        });

    [Fact]
    public async Task Arama_TurkceKarakterFarkiniYokSayar_YasVeMemleketleDaraltir()
    {
        var soyad = BenzersizSoyad("Çelik");
        var kayit = fixture.KayitServisi(RolTuru.KayitGorevlisi);
        var aile = await kayit.AileOlusturAsync($"{soyad} Ailesi", false,
        [
            Kisi("Şükrü", soyad, 40),                                     // aranan
            Kisi("Şükrü", soyad, 70),                                     // yaş tutmuyor
            Kisi("Şükrücan", soyad, 40),                                  // ad tam kelime değil
            Kisi("Şükrü", soyad, 42) with { Memleket = "İzmir" },        // memleket tutmuyor
            Kisi("Şükrü", soyad, 38) with { Memleket = null },           // memleketi bilinmiyor → elenmez
        ]);
        var kisiler = aile.Uyeler.OrderBy(u => u.Id).ToList();

        // Aranan bilgiler Türkçe karaktersiz ve küçük harfle yazılmış.
        var basvuru = await BasvuruAsync("sukru", TurkceMetin.Normallestir(soyad), yas: 41, memleket: "hatay");
        var adaylar = await Servis().AraAsync(basvuru.Id);

        Assert.Equal([kisiler[0].Id, kisiler[4].Id], adaylar.Select(a => a.KisiId).Order());
        Assert.Equal("Aynı", adaylar.Single(a => a.KisiId == kisiler[0].Id).MemleketUyumu);
        Assert.Equal("Bilinmiyor", adaylar.Single(a => a.KisiId == kisiler[4].Id).MemleketUyumu);

        // Yaş ve memleket verilmezse yalnızca ad-soyad eşleşmesi aranır (tam kelime).
        var genis = await BasvuruAsync("Şükrü", soyad);
        Assert.Equal(4, (await Servis().AraAsync(genis.Id)).Count);
    }

    [Fact]
    public async Task Durumlar_AcikOlasiEslesmeDogrulandiKapandi_FotografTekBasinaYetmez()
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Mehmet", soyad, 30)]);
        var kisiId = aile.Uyeler.Single().Id;
        var servis = Servis();
        var basvuru = await BasvuruAsync("Mehmet", soyad, 30);
        Assert.Equal(YakinBulmaDurumu.Acik, basvuru.Durum);

        // Eşleşme işaretlenmeden doğrulanamaz.
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.DogrulaAsync(basvuru.Id, "Görüşüldü", true));

        await servis.EslesmeIsaretleAsync(basvuru.Id, kisiId);
        var olasi = (await servis.ListeleAsync(sadeceAcik: true)).Single(b => b.Id == basvuru.Id);
        Assert.Equal((YakinBulmaDurumu.OlasiEslesme, kisiId), (olasi.Durum, olasi.EslesenKisiId!.Value));

        // Fotoğraf benzerliği tek başına doğrulama sayılmaz; açıklama da zorunlu.
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.DogrulaAsync(basvuru.Id, "Fotoğraflar birbirine benziyor", false));
        await Assert.ThrowsAsync<DogrulamaException>(() => servis.DogrulaAsync(basvuru.Id, "  ", true));

        await servis.DogrulaAsync(basvuru.Id, "Kişiyle görüşüldü, kardeşini tanıdı; kimlik bilgileri tutuyor.", true);
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.EslesmeIsaretleAsync(basvuru.Id, kisiId));

        // Sonuç yazılmadan kapanmaz; kapanan başvuru açıklar listesinde görünmez.
        await Assert.ThrowsAsync<DogrulamaException>(() => servis.KapatAsync(basvuru.Id, ""));
        await servis.KapatAsync(basvuru.Id, "Kardeşler kayıt masasında görevli eşliğinde buluştu.");
        Assert.DoesNotContain(await servis.ListeleAsync(sadeceAcik: true), b => b.Id == basvuru.Id);
        var kapanan = (await servis.ListeleAsync(sadeceAcik: false)).Single(b => b.Id == basvuru.Id);
        Assert.Equal(YakinBulmaDurumu.Kapandi, kapanan.Durum);
        Assert.NotNull(kapanan.KapanisZamani);
        Assert.NotNull(kapanan.DogrulamaAciklamasi);
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.KapatAsync(basvuru.Id, "Tekrar"));
    }

    [Fact]
    public async Task YanlisAday_EslesmeKaldirilir_BasvuruYenidenAcikOlur()
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Zeynep", soyad, 20)]);
        var servis = Servis();
        var basvuru = await BasvuruAsync("Zeynep", soyad);

        await servis.EslesmeIsaretleAsync(basvuru.Id, aile.Uyeler.Single().Id);
        await Assert.ThrowsAsync<DogrulamaException>(() => servis.EslesmeyiKaldirAsync(basvuru.Id, ""));
        await servis.EslesmeyiKaldirAsync(basvuru.Id, "Başvuran bu kişiyi tanımadı");

        var ozet = (await servis.ListeleAsync(sadeceAcik: true)).Single(b => b.Id == basvuru.Id);
        Assert.Equal((YakinBulmaDurumu.Acik, (int?)null), (ozet.Durum, ozet.EslesenKisiId));
        await Assert.ThrowsAsync<IsKuraliException>(() => servis.EslesmeyiKaldirAsync(basvuru.Id, "Tekrar"));
    }

    [Theory]
    [InlineData(RolTuru.DepoSorumlusu)]
    [InlineData(RolTuru.SaglikPersoneli)]
    [InlineData(RolTuru.SahaGorevlisi)]
    [InlineData(RolTuru.Kiosk)]
    public async Task AramaVeBasvuruYalnizcaYetkiliGorevlide(RolTuru rol)
    {
        var basvuru = await BasvuruAsync("Ali", BenzersizSoyad());
        var yetkisiz = Servis(rol);

        await Assert.ThrowsAsync<YetkisizIslemException>(() => yetkisiz.AraAsync(basvuru.Id));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => yetkisiz.ListeleAsync(false));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => BasvuruAsync("Ali", "Veli", rol: rol));
    }

    [Fact]
    public async Task RefakatsizCocuk_YalnizcaKoordinatorunAramasindaCikar()
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.Koordinator).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Emre", soyad, 9, refakatsiz: true)]);
        var cocukId = aile.Uyeler.Single().Id;
        var basvuru = await BasvuruAsync("Emre", soyad, 9);

        Assert.Empty(await Servis(RolTuru.KayitGorevlisi).AraAsync(basvuru.Id));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => Servis(RolTuru.KayitGorevlisi).EslesmeIsaretleAsync(basvuru.Id, cocukId));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => Servis(RolTuru.KayitGorevlisi).FotografAsync(cocukId));

        var koordinator = Servis(RolTuru.Koordinator);
        Assert.Equal(cocukId, Assert.Single(await koordinator.AraAsync(basvuru.Id)).KisiId);
        await koordinator.EslesmeIsaretleAsync(basvuru.Id, cocukId);

        // Eşleşen kişi Koordinatör dışındaki rolde gizli gösterilir.
        Assert.Equal(Gosterim.GizliKayit, (await Servis(RolTuru.KayitGorevlisi).ListeleAsync(true)).Single(b => b.Id == basvuru.Id).EslesenKisi);
        Assert.Contains("Emre", (await koordinator.ListeleAsync(true)).Single(b => b.Id == basvuru.Id).EslesenKisi);
    }

    [Fact]
    public void AdayBilgisinde_CadirVeIletisimBilgisiYok()
    {
        var alanlar = typeof(YakinAdayi).GetProperties().Select(p => p.Name).ToList();
        Assert.DoesNotContain(alanlar, a => a.Contains("Cadir") || a.Contains("Telefon") || a.Contains("Aile"));
    }

    [Fact]
    public async Task GecersizGirdiler_AnlasilirMesajlaReddedilir()
    {
        await Assert.ThrowsAsync<DogrulamaException>(() => BasvuruAsync("", "Yılmaz"));
        await Assert.ThrowsAsync<DogrulamaException>(() => BasvuruAsync("Ali", "Yılmaz", yas: 150));
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().BasvuruAcAsync(new YakinBulmaBilgisi
        {
            BasvuranAdSoyad = "Ayşe", BasvuranTelefon = "telefon yok", ArananAd = "Ali", ArananSoyad = "Yılmaz",
        }));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => Servis().AraAsync(int.MaxValue));
    }

    [Theory]
    [InlineData("ali yilmaz", "ali", "yilmaz", true)]
    [InlineData("alican yilmaz", "ali", "yilmaz", false)]
    [InlineData("ayse nur kaya", "ayse nur", "kaya", true)]
    [InlineData("ayse kaya", "ayse nur", "kaya", false)]
    public void AdSoyad_TamKelimeOlarakEslesir(string aramaAdi, string ad, string soyad, bool beklenen) =>
        Assert.Equal(beklenen, YakinBulmaServisi.AdSoyadUyar(aramaAdi, ad, soyad));
}
