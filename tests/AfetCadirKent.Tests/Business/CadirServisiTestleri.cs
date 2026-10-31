using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class CadirServisiTestleri(VeritabaniFixture fixture)
{
    private CadirServisi Servis(RolTuru rol = RolTuru.KayitGorevlisi) => fixture.CadirServisi(rol);

    /// <summary>Teste özel bölge ve verilen kapasitelerde çadırlar oluşturur.</summary>
    private async Task<(int BolgeId, List<int> CadirIdler)> TestBolgesiAsync(params CadirBilgisi[] cadirlar)
    {
        var bolge = await Servis().BolgeKaydetAsync(null, BenzersizBolgeKodu(), "Test bölgesi", null);
        var idler = new List<int>();
        foreach (var c in cadirlar)
            idler.Add((await Servis().CadirEkleAsync(c with { BolgeId = bolge.Id })).Id);
        return (bolge.Id, idler);
    }

    private static CadirBilgisi Cadir(int numara, int kapasite, bool isitmali = false, bool saglik = false) =>
        new() { BolgeId = 0, Sira = 1, Numara = numara, Kapasite = kapasite, Isitmali = isitmali, SaglikNoktasinaYakin = saglik };

    private Task<Aile> AileAsync(params (string Ad, int Yas, bool Hamile)[] uyeler)
    {
        var soyad = BenzersizSoyad();
        return fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false,
            uyeler.Select(u => Kisi(u.Ad, soyad, u.Yas, hamile: u.Hamile)).ToList());
    }

    private async Task<int> DolulukAsync(int cadirId)
    {
        await using var db = fixture.YeniContext();
        return await db.Cadirlar.Where(c => c.Id == cadirId).Select(c => c.Doluluk).SingleAsync();
    }

    [Fact]
    public async Task BebekliVeHamileAileyeEtkileriKarsilayanCadirOnerilir_GorevliOnaylayincaYerlesir()
    {
        var (_, cadirlar) = await TestBolgesiAsync(Cadir(1, 3, isitmali: true, saglik: true));
        var aile = await AileAsync(("Anne", 30, true), ("Baba", 33, false), ("Bebek", 1, false));

        var sonuc = await Servis().OneriGetirAsync(aile.Id);
        Assert.Equal(DestekOnceligi.Yuksek, sonuc.DestekOnceligi);
        Assert.Equal([YerlesimEtkisi.SicakCadir, YerlesimEtkisi.SaglikNoktasinaYakin], sonuc.IstenenEtkiler);
        Assert.Equal(3, sonuc.YerlesecekKisiSayisi);
        Assert.False(sonuc.BeklemeListesiOnerilir);

        var ilk = sonuc.Oneriler[0];
        Assert.Equal(2, ilk.KarsilananEtkiler.Count);
        Assert.Equal(0, ilk.BosYatakIsrafi);
        var bizimki = sonuc.Oneriler.Single(o => o.Atamalar[0].CadirId == cadirlar[0]);

        await Servis().YerlestirAsync(aile.Id, bizimki.Atamalar);
        Assert.Equal(3, await DolulukAsync(cadirlar[0]));
        Assert.Equal(0, (await Servis().OneriGetirAsync(aile.Id)).YerlesecekKisiSayisi);

        await using var db = fixture.YeniContext();
        Assert.Equal(3, await db.KisiHareketleri.CountAsync(h => h.Kisi!.AileId == aile.Id
            && h.Tur == KisiHareketTuru.CadirDegisikligi && h.YeniCadirId == cadirlar[0]));
    }

    [Fact]
    public async Task KapasiteAsilamaz_BakimdakiCadiraYerlesilemez()
    {
        var (bolgeId, cadirlar) = await TestBolgesiAsync(Cadir(1, 2), Cadir(2, 6));
        var aile = await AileAsync(("A", 40, false), ("B", 38, false), ("C", 10, false));

        var hata = await Assert.ThrowsAsync<IsKuraliException>(() =>
            Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[0], "?", 3)]));
        Assert.Contains("yeterli yer yok", hata.Message);

        await Servis().CadirGuncelleAsync(cadirlar[1], Cadir(2, 6) with { BolgeId = bolgeId, Durum = CadirDurumu.Bakimda });
        await Assert.ThrowsAsync<IsKuraliException>(() =>
            Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[1], "?", 3)]));

        // Atanan kişi sayısı yerleşecek kişi sayısıyla tutmalı
        await Assert.ThrowsAsync<IsKuraliException>(() =>
            Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[0], "?", 2)]));
        Assert.Equal(0, await DolulukAsync(cadirlar[0]));
    }

    [Fact]
    public async Task AileIkiCadiraYerlesebilir()
    {
        var (_, cadirlar) = await TestBolgesiAsync(Cadir(1, 4), Cadir(2, 4));
        var aile = await AileAsync(("A", 50, false), ("B", 48, false), ("C", 25, false), ("D", 22, false), ("E", 20, false), ("F", 18, false));

        await Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[0], "?", 4), new CadirAtamasi(cadirlar[1], "?", 2)]);
        Assert.Equal(4, await DolulukAsync(cadirlar[0]));
        Assert.Equal(2, await DolulukAsync(cadirlar[1]));

        var plan = await Servis().CadirPlaniAsync();
        var ilk = plan.Single(c => c.Id == cadirlar[0]);
        Assert.Equal(PlanDurumu.Dolu, ilk.PlanDurumu);
        Assert.Equal(PlanDurumu.KismenDolu, plan.Single(c => c.Id == cadirlar[1]).PlanDurumu);
        Assert.Equal([$"{aile.AileNo} {aile.AileAdi}"], ilk.Aileler);
    }

    [Fact]
    public async Task AyniCadiraEsZamanliYerlestirmedeYalnizcaBiriBasarili()
    {
        var (_, cadirlar) = await TestBolgesiAsync(Cadir(1, 4));
        var aile1 = await AileAsync(("A", 40, false), ("B", 38, false), ("C", 10, false));
        var aile2 = await AileAsync(("D", 40, false), ("E", 38, false), ("F", 10, false));

        var sonuclar = await Task.WhenAll(
            Deneme(() => Servis().YerlestirAsync(aile1.Id, [new CadirAtamasi(cadirlar[0], "?", 3)])),
            Deneme(() => Servis().YerlestirAsync(aile2.Id, [new CadirAtamasi(cadirlar[0], "?", 3)])));

        Assert.Equal(1, sonuclar.Count(s => s is null));
        Assert.IsAssignableFrom<IsKuraliException>(sonuclar.Single(s => s is not null));
        Assert.Equal(3, await DolulukAsync(cadirlar[0]));

        static async Task<Exception?> Deneme(Func<Task> islem)
        {
            try { await islem(); return null; }
            catch (Exception ex) { return ex; }
        }
    }

    [Fact]
    public async Task CadirDegisikligiEskiYeriBosaltirVeGecmiseYazilir()
    {
        var (_, cadirlar) = await TestBolgesiAsync(Cadir(1, 4), Cadir(2, 4));
        var aile = await AileAsync(("A", 40, false), ("B", 38, false));
        await Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[0], "?", 2)]);

        var kisiId = aile.Uyeler[1].Id;
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().CadirDegistirAsync([kisiId], cadirlar[1], ""));
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().CadirDegistirAsync([kisiId], cadirlar[0], "aynı çadır"));

        await Servis().CadirDegistirAsync([kisiId], cadirlar[1], "Çadır su aldı");
        Assert.Equal(1, await DolulukAsync(cadirlar[0]));
        Assert.Equal(1, await DolulukAsync(cadirlar[1]));

        var hareket = (await fixture.KayitServisi(RolTuru.KayitGorevlisi).KisiHareketleriAsync(kisiId)).Last();
        Assert.Equal(KisiHareketTuru.CadirDegisikligi, hareket.Tur);
        Assert.Equal(cadirlar[0], hareket.EskiCadirId);
        Assert.Equal(cadirlar[1], hareket.YeniCadirId);
    }

    [Fact]
    public async Task KullanilmisCadirVeBolgeSilinemez_BosCadirSilinir()
    {
        var (bolgeId, cadirlar) = await TestBolgesiAsync(Cadir(1, 4), Cadir(2, 4));
        var aile = await AileAsync(("A", 40, false));
        await Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[0], "?", 1)]);

        var hata = await Assert.ThrowsAsync<SilinemezException>(() => Servis().CadirSilAsync(cadirlar[0]));
        Assert.Equal("Bu kayıt kullanıldığı için silinemez, pasif yapabilirsiniz.", hata.Message);
        await Assert.ThrowsAsync<SilinemezException>(() => Servis().BolgeSilAsync(bolgeId));

        await Servis().CadirSilAsync(cadirlar[1]);
        Assert.DoesNotContain(await Servis().CadirPlaniAsync(bolgeId), c => c.Id == cadirlar[1]);

        // Dolu çadır pasif veya kapalı yapılamaz (kapasite kuralı entity testlerinde: CadirTestleri)
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().CadirAktiflikDegistirAsync(cadirlar[0], false));
        await Assert.ThrowsAsync<IsKuraliException>(() =>
            Servis().CadirGuncelleAsync(cadirlar[0], Cadir(1, 4) with { BolgeId = bolgeId, Durum = CadirDurumu.Kapali }));
    }

    [Fact]
    public async Task AyniNumaraliCadirVeAyniKodluBolgeEklenemez()
    {
        var (bolgeId, _) = await TestBolgesiAsync(Cadir(1, 4));
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().CadirEkleAsync(Cadir(1, 6) with { BolgeId = bolgeId }));
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().BolgeKaydetAsync(null, "A", "Kopya A", null));
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().CadirEkleAsync(Cadir(2, 0) with { BolgeId = bolgeId }));
    }

    [Fact]
    public async Task YetkisizRollerCadirIslemiYapamaz()
    {
        var aile = await AileAsync(("A", 40, false));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => fixture.CadirServisi(RolTuru.DepoSorumlusu).CadirPlaniAsync());
        await Assert.ThrowsAsync<YetkisizIslemException>(() =>
            fixture.CadirServisi(RolTuru.SahaGorevlisi).YerlestirAsync(aile.Id, [new CadirAtamasi(1, "?", 1)]));
        Assert.NotEmpty(await fixture.CadirServisi(RolTuru.SahaGorevlisi).CadirPlaniAsync());
    }

    [Fact]
    public async Task IkiCadiraYerlesimdeGorevliKisileriSecebilir()
    {
        var (_, cadirlar) = await TestBolgesiAsync(Cadir(1, 4), Cadir(2, 4));
        var aile = await AileAsync(("A", 50, false), ("B", 48, false), ("C", 25, false), ("D", 22, false), ("E", 20, false));
        var idler = aile.Uyeler.Select(u => u.Id).ToList();

        var oneri = await Servis().OneriGetirAsync(aile.Id);
        Assert.True(oneri.KisiSecimiYapilabilir);
        Assert.Equal(idler, oneri.YerlesecekKisiler.Select(k => k.Id).Order().ToList());

        // Hatalı seçimler: kişi iki çadırda, bir kişi eksik, bir çadırda seçim yok, sayı tutmuyor
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().YerlestirAsync(aile.Id,
            [new CadirAtamasi(cadirlar[0], "?", 3, [idler[0], idler[1], idler[2]]), new CadirAtamasi(cadirlar[1], "?", 2, [idler[2], idler[3]])]));
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().YerlestirAsync(aile.Id,
            [new CadirAtamasi(cadirlar[0], "?", 3, [idler[0], idler[1], idler[2]]), new CadirAtamasi(cadirlar[1], "?", 2, [idler[3]])]));
        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().YerlestirAsync(aile.Id,
            [new CadirAtamasi(cadirlar[0], "?", 3, [idler[0], idler[1], idler[2]]), new CadirAtamasi(cadirlar[1], "?", 2)]));
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().YerlestirAsync(aile.Id,
            [new CadirAtamasi(cadirlar[0], "?", 3, [idler[0], idler[1]]), new CadirAtamasi(cadirlar[1], "?", 2, [idler[2], idler[3], idler[4]])]));
        Assert.Equal(0, await DolulukAsync(cadirlar[0]));

        // Kayıt sırasına göre değil, görevlinin seçimine göre: A, C, E birinci; B, D ikinci çadıra
        await Servis().YerlestirAsync(aile.Id,
            [new CadirAtamasi(cadirlar[0], "?", 3, [idler[0], idler[2], idler[4]]), new CadirAtamasi(cadirlar[1], "?", 2, [idler[1], idler[3]])]);
        Assert.Equal([idler[0], idler[2], idler[4]], (await Servis().CadirdakiKisilerAsync(cadirlar[0])).Select(k => k.Id).Order().ToList());
        var ikinci = await Servis().CadirdakiKisilerAsync(cadirlar[1]);
        Assert.Equal([idler[1], idler[3]], ikinci.Select(k => k.Id).Order().ToList());
        Assert.All(ikinci, k => Assert.Equal(aile.AileNo, k.AileNo));
    }

    [Fact]
    public async Task YerlesmemisAilelerListelenir_YerlesinceListedenCikar()
    {
        var (_, cadirlar) = await TestBolgesiAsync(Cadir(1, 4));
        var aile = await AileAsync(("A", 40, false), ("B", 38, false));
        await fixture.BeklemeListesiServisi(RolTuru.KayitGorevlisi).EkleAsync(aile.Id, null);

        var satir = Assert.Single(await Servis().YerlesmemisAilelerAsync(), a => a.AileId == aile.Id);
        Assert.Equal(2, satir.YerlesmemisKisiSayisi);
        Assert.True(satir.BeklemeListesinde);

        await Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[0], "?", 2)]);
        Assert.DoesNotContain(await Servis().YerlesmemisAilelerAsync(), a => a.AileId == aile.Id);
    }

    [Fact]
    public async Task PasifBolgedekiCadiraYerlesilemez_PlandaKullanilamazGorunur()
    {
        var (bolgeId, cadirlar) = await TestBolgesiAsync(Cadir(1, 4));
        var aile = await AileAsync(("A", 40, false));
        await Servis().BolgeAktiflikDegistirAsync(bolgeId, false);

        var hata = await Assert.ThrowsAsync<IsKuraliException>(() =>
            Servis().YerlestirAsync(aile.Id, [new CadirAtamasi(cadirlar[0], "?", 1)]));
        Assert.Contains("bölgesi pasif", hata.Message);
        Assert.Equal(PlanDurumu.Kullanilamaz, (await Servis().CadirPlaniAsync(bolgeId)).Single().PlanDurumu);
        Assert.DoesNotContain((await Servis().OneriGetirAsync(aile.Id)).Oneriler, o => o.Atamalar.Any(a => a.CadirId == cadirlar[0]));
    }
}

[Collection(VeritabaniKoleksiyonu.Ad)]
public class BeklemeListesiServisiTestleri(VeritabaniFixture fixture)
{
    private BeklemeListesiServisi Servis() => fixture.BeklemeListesiServisi(RolTuru.KayitGorevlisi);

    private Task<Aile> AileAsync(params (string Ad, int Yas)[] uyeler)
    {
        var soyad = BenzersizSoyad();
        return fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false,
            uyeler.Select(u => Kisi(u.Ad, soyad, u.Yas)).ToList());
    }

    [Fact]
    public async Task YerAcilincaOncelikliAileOnerilir_OnaylanincaYerlesipListedenCikar()
    {
        var normalAile = await AileAsync(("A", 40), ("B", 38));     // önce eklenen, Normal
        var bebekliAile = await AileAsync(("C", 30), ("D", 1));     // sonra eklenen, Yüksek
        var normalKayit = await Servis().EkleAsync(normalAile.Id, "Uygun çadır yok");
        var bebekliKayit = await Servis().EkleAsync(bebekliAile.Id, null);

        var cadirServisi = fixture.CadirServisi(RolTuru.KayitGorevlisi);
        var bolge = await cadirServisi.BolgeKaydetAsync(null, BenzersizBolgeKodu(), "Bekleme testi", null);
        var cadir = await cadirServisi.CadirEkleAsync(new CadirBilgisi { BolgeId = bolge.Id, Sira = 1, Numara = 1, Kapasite = 2 });

        var liste = (await Servis().ListeleAsync()).Select(b => b.KayitId).ToList();
        Assert.True(liste.IndexOf(bebekliKayit.Id) < liste.IndexOf(normalKayit.Id));

        var oneriler = (await Servis().CadirIcinOneriAsync(cadir.Id)).Select(b => b.KayitId).ToList();
        Assert.Contains(bebekliKayit.Id, oneriler);
        Assert.True(oneriler.IndexOf(bebekliKayit.Id) < oneriler.IndexOf(normalKayit.Id));
        var yerAcilan = await Servis().YerAcilanCadirlarAsync();
        Assert.Contains(yerAcilan, y => y.Cadir.Id == cadir.Id);

        await Servis().OnaylaVeYerlestirAsync(bebekliKayit.Id, cadir.Id);
        Assert.DoesNotContain(await Servis().ListeleAsync(), b => b.KayitId == bebekliKayit.Id);
        Assert.Empty(await Servis().CadirIcinOneriAsync(cadir.Id)); // çadır doldu

        await using var db = fixture.YeniContext();
        Assert.NotNull((await db.BeklemeListesi.SingleAsync(b => b.Id == bebekliKayit.Id)).CikisZamani);
        Assert.Equal(2, await db.Cadirlar.Where(c => c.Id == cadir.Id).Select(c => c.Doluluk).SingleAsync());

        await Assert.ThrowsAsync<DogrulamaException>(() => Servis().ListedenCikarAsync(normalKayit.Id, " "));
        await Servis().ListedenCikarAsync(normalKayit.Id, "Akrabasının yanına gitti");
        Assert.DoesNotContain(await Servis().ListeleAsync(), b => b.KayitId == normalKayit.Id);
    }

    [Fact]
    public async Task AyniAileIkiKezVeyaYerlesmisAileListeyeAlinamaz()
    {
        var aile = await AileAsync(("A", 40));
        await Servis().EkleAsync(aile.Id, null);
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().EkleAsync(aile.Id, null));

        var yerlesmis = await AileAsync(("B", 40));
        var cadirServisi = fixture.CadirServisi(RolTuru.KayitGorevlisi);
        var oneri = (await cadirServisi.OneriGetirAsync(yerlesmis.Id)).Oneriler[0];
        await cadirServisi.YerlestirAsync(yerlesmis.Id, oneri.Atamalar);
        await Assert.ThrowsAsync<IsKuraliException>(() => Servis().EkleAsync(yerlesmis.Id, null));
    }

    [Fact]
    public async Task DogrudanYerlesenAileBeklemeListesindenDeCikar()
    {
        var aile = await AileAsync(("A", 40));
        var kayit = await Servis().EkleAsync(aile.Id, null);
        var cadirServisi = fixture.CadirServisi(RolTuru.KayitGorevlisi);
        await cadirServisi.YerlestirAsync(aile.Id, (await cadirServisi.OneriGetirAsync(aile.Id)).Oneriler[0].Atamalar);
        Assert.DoesNotContain(await Servis().ListeleAsync(), b => b.KayitId == kayit.Id);
    }

    [Fact]
    public async Task TekCadiraSigmayanBekleyenAileIkiCadiraYerlestirilir()
    {
        var cadirServisi = fixture.CadirServisi(RolTuru.KayitGorevlisi);
        var bolge = await cadirServisi.BolgeKaydetAsync(null, BenzersizBolgeKodu(), "İki çadır testi", null);
        var c1 = await cadirServisi.CadirEkleAsync(new CadirBilgisi { BolgeId = bolge.Id, Sira = 1, Numara = 1, Kapasite = 3 });
        var c2 = await cadirServisi.CadirEkleAsync(new CadirBilgisi { BolgeId = bolge.Id, Sira = 1, Numara = 2, Kapasite = 3 });
        var aile = await AileAsync(("A", 45), ("B", 43), ("C", 20), ("D", 18), ("E", 15));
        var kayit = await Servis().EkleAsync(aile.Id, "Tek çadıra sığmıyor");

        // Hiçbir çadır tek başına yetmediği için "yer açılan çadır" önerisinde bu aile çıkmaz; iki çadırlık öneri kullanılır.
        Assert.DoesNotContain(await Servis().CadirIcinOneriAsync(c1.Id), b => b.KayitId == kayit.Id);
        var idler = aile.Uyeler.Select(u => u.Id).ToList();
        await Servis().OnaylaVeYerlestirAsync(kayit.Id,
            [new CadirAtamasi(c1.Id, c1.Kod, 3, [idler[0], idler[1], idler[4]]), new CadirAtamasi(c2.Id, c2.Kod, 2, [idler[2], idler[3]])]);

        Assert.DoesNotContain(await Servis().ListeleAsync(), b => b.KayitId == kayit.Id);
        Assert.Equal(3, (await cadirServisi.CadirdakiKisilerAsync(c1.Id)).Count);
        Assert.Equal([idler[2], idler[3]], (await cadirServisi.CadirdakiKisilerAsync(c2.Id)).Select(k => k.Id).Order().ToList());
    }
}
