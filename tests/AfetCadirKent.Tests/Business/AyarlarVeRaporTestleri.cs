using AfetCadirKent.Business.Ayarlar;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class AyarlarVeRaporTestleri(VeritabaniFixture fixture)
{
    // ---------------------------------------------------------------- duyurular

    [Fact]
    public async Task Duyuru_EklenirGuncellenir_YayinDurumuHesaplanir_PasifYapilir()
    {
        var servis = fixture.DuyuruServisi();
        var simdi = DateTime.Now;
        var yayinda = await servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Test su saati", Metin = "Su dağıtımı 09:00'da.", BaslangicTarihi = simdi.AddHours(-1), BitisTarihi = simdi.AddDays(1),
        });
        var ileri = await servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Test ileri", Metin = "Yarın başlar.", BaslangicTarihi = simdi.AddDays(1), BitisTarihi = simdi.AddDays(2),
        });
        var bitmis = await servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Test bitmiş", Metin = "Dün bitti.", BaslangicTarihi = simdi.AddDays(-3), BitisTarihi = simdi.AddDays(-1),
        });
        Assert.Equal(("Yayında", "Henüz başlamadı", "Süresi doldu"), (yayinda.YayinDurumu, ileri.YayinDurumu, bitmis.YayinDurumu));

        var guncel = await servis.KaydetAsync(yayinda.Id, new DuyuruBilgisi
        {
            Baslik = "Test su saati (güncel)", Metin = "Su dağıtımı 10:00'da.", BaslangicTarihi = yayinda.BaslangicTarihi,
            BitisTarihi = yayinda.BitisTarihi,
        });
        Assert.Equal((yayinda.Id, "Test su saati (güncel)"), (guncel.Id, guncel.Baslik));

        await servis.AktiflikDegistirAsync(yayinda.Id, false);
        Assert.DoesNotContain(await servis.ListeleAsync(), d => d.Id == yayinda.Id);
        Assert.Equal("Pasif", (await servis.ListeleAsync(pasifleriDeGoster: true)).Single(d => d.Id == yayinda.Id).YayinDurumu);

        await Assert.ThrowsAsync<DogrulamaException>(() => servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = "Ters", Metin = "x", BaslangicTarihi = simdi, BitisTarihi = simdi.AddMinutes(-1),
        }));
        await Assert.ThrowsAsync<DogrulamaException>(() => servis.KaydetAsync(null, new DuyuruBilgisi
        {
            Baslik = " ", Metin = "x", BaslangicTarihi = simdi, BitisTarihi = simdi.AddDays(1),
        }));
    }

    [Theory]
    [InlineData(RolTuru.KayitGorevlisi)]
    [InlineData(RolTuru.DepoSorumlusu)]
    [InlineData(RolTuru.Kiosk)]
    public async Task DuyuruVeGerekceAyarlariYalnizcaKoordinatorde(RolTuru rol)
    {
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.DuyuruServisi(rol).ListeleAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.GerekceAyarServisi(rol).GerekceleriGetirAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            fixture.GerekceAyarServisi(rol).EslesmeleriKaydetAsync(GerekceKodu.Yasli, [], "deneme"));
    }

    // ---------------------------------------------------------------- gerekçe ayarları

    [Fact]
    public async Task GerekceEslesmesiDegisince_AileninMalzemeIhtiyaciDegisir()
    {
        var ayar = fixture.GerekceAyarServisi();
        var urun = await fixture.TestUrunuAsync();
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Hasan", soyad, 72)]);
        var gerekceServisi = fixture.GerekceServisi(RolTuru.KayitGorevlisi);
        bool Oncelikli(global::AfetCadirKent.Business.Gerekceler.AileOncelikSonucu s) => s.MalzemeIhtiyaclari.Single(m => m.UrunId == urun.Id).Oncelikli;

        Assert.False(Oncelikli(await gerekceServisi.DegerlendirAsync(aile.Id)));
        var orijinal = (await ayar.GerekceleriGetirAsync()).Single(g => g.Kod == GerekceKodu.Yasli).UrunIdler;

        await Assert.ThrowsAsync<DogrulamaException>(() => ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, [.. orijinal, urun.Id], " "));
        try
        {
            await ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, [.. orijinal, urun.Id], "Yaşlılara bu ürün de öncelikli verilsin");
            Assert.Contains(urun.Id, (await ayar.GerekceleriGetirAsync()).Single(g => g.Kod == GerekceKodu.Yasli).UrunIdler);
            Assert.True(Oncelikli(await gerekceServisi.DegerlendirAsync(aile.Id)));
        }
        finally
        {
            await ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, orijinal, "Test sonrası eski haline döndü");
        }

        Assert.Equal(orijinal, (await ayar.GerekceleriGetirAsync()).Single(g => g.Kod == GerekceKodu.Yasli).UrunIdler);
        await Assert.ThrowsAsync<IsKuraliException>(() => ayar.EslesmeleriKaydetAsync(GerekceKodu.UcGundurYardimAlmadi, [urun.Id], "x"));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => ayar.EslesmeleriKaydetAsync(GerekceKodu.Yasli, [int.MaxValue], "x"));
    }

    // ---------------------------------------------------------------- raporlar

    [Fact]
    public async Task DagitimTutanagi_AileSatirlariVeImzaAlanlariyla_DepoSorumlusunda()
    {
        var urun = await fixture.TestUrunuAsync(HesaplamaTuru.AileBasi, hak: 1, tekrar: 1);
        await fixture.StokServisi().StokGirisiAsync(new StokGirisiBilgisi { UrunId = urun.Id, PartiNo = "TUT-1", Miktar = 10_000 });
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false, [Kisi("Ayten", soyad, 40)]);
        var dagitim = fixture.DagitimServisi();
        var liste = await dagitim.ListeOlusturAsync(urun.Id);
        await dagitim.OnaylaAsync(liste.Ozet.Id);
        var detay = liste.Satirlar.Single(s => s.AileId == aile.Id);
        await dagitim.TeslimEtAsync(detay.DetayId, 1, aile.Uyeler[0].Id);

        var depo = await fixture.KullaniciOturumuAsync(RolTuru.DepoSorumlusu);
        var tutanak = await fixture.RaporServisi(depo).DagitimTutanagiAsync(liste.Ozet.Id);

        Assert.Equal("Dağıtım Tutanağı", tutanak.Baslik);
        Assert.Contains(urun.Ad, tutanak.AltBasliklar[0]);
        var satir = tutanak.Satirlar.Single(s => s[0] == aile.AileNo);
        Assert.Equal(("1", "1", "0"), (satir[3], satir[4], satir[5]));
        Assert.Contains("Ayten", satir[6]);
        Assert.Equal(tutanak.Sutunlar.Count, satir.Count);
        Assert.NotEmpty(tutanak.ImzaAlanlari);

        var kayitGorevlisi = await fixture.KullaniciOturumuAsync(RolTuru.KayitGorevlisi);
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.RaporServisi(kayitGorevlisi).DagitimTutanagiAsync(liste.Ozet.Id));
        await dagitim.GunSonuAsync();
    }

    [Fact]
    public async Task StokNufusVeTalepRaporlari_IzneGoreAlinir()
    {
        var urun = await fixture.TestUrunuAsync(kritik: 50);
        await fixture.StokServisi().StokGirisiAsync(new StokGirisiBilgisi { UrunId = urun.Id, PartiNo = "RAP-1", Miktar = 20 });
        var koordinator = await fixture.KullaniciOturumuAsync(RolTuru.Koordinator);
        var depo = await fixture.KullaniciOturumuAsync(RolTuru.DepoSorumlusu);
        var rapor = fixture.RaporServisi(koordinator);

        var stok = await fixture.RaporServisi(depo).StokRaporuAsync();
        var stokSatiri = stok.Satirlar.Single(s => s[0] == urun.Ad);
        Assert.Equal(("20", "Kritik seviyenin altında"), (stokSatiri[4], stokSatiri[9]));

        var nufus = await rapor.NufusRaporuAsync();
        Assert.Contains(nufus.Satirlar, s => s[0] == "Kişi durumu" && s[1] == "Burada" && int.Parse(s[2]) > 0);
        Assert.Contains(nufus.Satirlar, s => s[0] == "Yaş grubu (çadır kentte)" && s[1] == "0–2 yaş");
        Assert.All(nufus.Satirlar, s => Assert.Equal(nufus.Sutunlar.Count, s.Count));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.RaporServisi(depo).NufusRaporuAsync());

        var talep = await fixture.TalepServisi(koordinator).OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Rapor testi talebi", Kaynak = TalepKaynagi.Gorevli, Tur = TalepTuru.Diger,
        });
        var bugun = DateOnly.FromDateTime(DateTime.Today);
        var talepRaporu = await rapor.TalepRaporuAsync(bugun, bugun);
        Assert.Contains(talepRaporu.Satirlar, s => s[0] == talep.Id.ToString() && s[8] == "Rapor testi talebi");
        Assert.DoesNotContain((await rapor.TalepRaporuAsync(bugun.AddDays(-10), bugun.AddDays(-9))).Satirlar, s => s[0] == talep.Id.ToString());
        await Assert.ThrowsAsync<DogrulamaException>(() => rapor.TalepRaporuAsync(bugun, bugun.AddDays(-1)));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.RaporServisi(depo).TalepRaporuAsync(bugun, bugun));
    }

    [Fact]
    public async Task AileFisi_KayitGorevlisiYazdirir_AileNoCadirVeKioskBilgisiVar()
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Elif", soyad, 35), Kisi("Can", soyad, 8)]);
        var kayit = await fixture.KullaniciOturumuAsync(RolTuru.KayitGorevlisi);

        var fis = await fixture.RaporServisi(kayit).AileFisiAsync(aile.Id);

        Assert.Equal((aile.AileNo, 2), (fis.AileNo, fis.Uyeler.Count));
        Assert.Contains(aile.AileNo, fis.KioskBilgisi);
        Assert.Contains("Henüz çadır verilmedi", Assert.Single(fis.Cadirlar));
        var kiosk = await fixture.KullaniciOturumuAsync(RolTuru.Kiosk);
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.RaporServisi(kiosk).AileFisiAsync(aile.Id));
    }
}
