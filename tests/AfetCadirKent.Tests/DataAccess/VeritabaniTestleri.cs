using AfetCadirKent.DataAccess;
using AfetCadirKent.DataAccess.Depolar;
using AfetCadirKent.DataAccess.Konfigurasyonlar;
using AfetCadirKent.Entities.Cadirlar;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Tests.DataAccess;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class DemoVeriTestleri(VeritabaniFixture fixture)
{
    [Fact]
    public async Task DemoVerisiSartnamedekiMiktarlardaYuklenir()
    {
        await using var db = fixture.YeniContext();
        Assert.Equal(3, await db.Bolgeler.CountAsync(b => new[] { "A", "B", "C" }.Contains(b.Kod)));
        Assert.Equal(30, await db.Cadirlar.CountAsync(c => new[] { "A", "B", "C" }.Contains(c.Bolge!.Kod)));
        Assert.Equal(20, await DemoAileleri(db).CountAsync());
        Assert.Equal(10, await db.Urunler.CountAsync(u => !u.Ad.StartsWith(TestYardimcilari.TestUrunOnEki)));
        Assert.Equal(0, await db.Kullanicilar.CountAsync(k => k.OlusturmaZamani < VeritabaniFixture.TohumlamaZamani)); // demo verisi kullanıcı içermez
        Assert.Equal(6, await db.Roller.CountAsync());
        Assert.Equal(10, await db.Gerekceler.CountAsync());
    }

    [Fact]
    public async Task DemoVerisiBebekliEngelliYasliAileIcerir()
    {
        await using var db = fixture.YeniContext();
        var bugun = DateOnly.FromDateTime(VeritabaniFixture.TohumlamaZamani);
        var kisiler = await db.Kisiler.ToListAsync();
        Assert.Contains(kisiler, k => k.BebekMi(bugun));
        Assert.Contains(kisiler, k => k.TekerlekliSandalye);
        Assert.Contains(kisiler, k => k.YasliMi(bugun));
        Assert.Contains(kisiler, k => k.TcKimlikNo is null); // kimliksiz kayıt
    }

    [Fact]
    public async Task SktYaklasmisVeGecmisPartiVardir()
    {
        await using var db = fixture.YeniContext();
        var bugun = DateOnly.FromDateTime(VeritabaniFixture.TohumlamaZamani);
        var partiler = await db.Partiler.ToListAsync();
        Assert.Contains(partiler, p => p.SktYaklasiyorMu(bugun, 7));
        Assert.Contains(partiler, p => p.SktGectiMi(bugun));
    }

    [Fact]
    public async Task NumaralarBenzersizVeBicimliUretilir()
    {
        await using var db = fixture.YeniContext();
        var aileNolar = await db.Aileler.Select(a => a.AileNo).ToListAsync();
        var kisiNolar = await db.Kisiler.Select(k => k.KisiNo).ToListAsync();
        Assert.All(aileNolar, n => Assert.Matches(@"^A-\d{6}$", n));
        Assert.All(kisiNolar, n => Assert.Matches(@"^K-\d{6}$", n));
        Assert.Equal(aileNolar.Count, aileNolar.Distinct().Count());
        Assert.Equal(kisiNolar.Count, kisiNolar.Distinct().Count());
    }

    [Fact]
    public async Task CadirDolulukleriYerlesimlerleTutarlidirVeKapasiteAsilmaz()
    {
        await using var db = fixture.YeniContext();
        var cadirlar = await db.Cadirlar
            .Select(c => new { c.Kapasite, c.Doluluk, Aktif = c.Yerlesimler.Count(y => y.Bitis == null) })
            .ToListAsync();
        Assert.All(cadirlar, c =>
        {
            Assert.Equal(c.Aktif, c.Doluluk);
            Assert.True(c.Doluluk <= c.Kapasite);
        });

        var demo = DemoAileleri(db);
        var yerlesenAile = await demo.CountAsync(a => a.Uyeler.Any(u => u.Yerlesimler.Any(y => y.Bitis == null)));
        var bekleyenAile = await db.BeklemeListesi.CountAsync(b => b.CikisZamani == null && demo.Any(a => a.Id == b.AileId));
        Assert.Equal(await demo.CountAsync(), yerlesenAile + bekleyenAile);
    }

    /// <summary>Demo verisindeki aileler (diğer testlerin eklediği aileler hariç).</summary>
    private static IQueryable<Aile> DemoAileleri(AfetDbContext db) =>
        db.Aileler.Where(a => a.KayitZamani < VeritabaniFixture.TohumlamaZamani);

    [Fact]
    public async Task GerekceUrunEslesmeleriKategorilereGoreKurulur()
    {
        await using var db = fixture.YeniContext();
        var bebekUrunleri = await db.GerekceUrunleri
            .Where(g => g.GerekceId == (int)GerekceKodu.Bebek && !g.Urun!.Ad.StartsWith(TestYardimcilari.TestUrunOnEki))
            .Select(g => g.Urun!.Ad).ToListAsync();
        Assert.Equal(["Battaniye", "Bebek bezi", "Bebek maması"], bebekUrunleri.Order());
    }

    [Fact]
    public async Task YetkiMatrisiVeritabaniylaAyni()
    {
        await using var db = fixture.YeniContext();
        Assert.Equal(ReferansVerisi.Yetkiler().Count, await db.Yetkiler.CountAsync());
        Assert.True(await db.Yetkiler.AnyAsync(y => y.RolId == (int)RolTuru.DepoSorumlusu && y.Modul == Modul.Dagitim && y.Izin == Izin.Rapor));
        Assert.False(await db.Yetkiler.AnyAsync(y => y.RolId == (int)RolTuru.SahaGorevlisi && y.Izin == Izin.Rapor));
    }

    [Fact]
    public async Task StokMiktarlariHareketlerleTutarlidir()
    {
        await using var db = fixture.YeniContext();
        // Pasif yapılan (geri alınan) hareketin etkisi partiden geri alındığı için yalnızca aktif hareketler toplanır.
        var partiler = await db.Partiler.Include(p => p.Hareketler).ToListAsync();
        Assert.All(partiler, p => Assert.Equal(p.Miktar, p.Hareketler.Where(h => h.Aktif).Sum(h => h.StokEtkisi)));
    }
}

[Collection(VeritabaniKoleksiyonu.Ad)]
public class VeritabaniKisitTestleri(VeritabaniFixture fixture)
{
    [Fact]
    public async Task AyniTcIleIkinciKayitVeritabanindaEngellenir()
    {
        await using var db = fixture.YeniContext();
        await using var islem = await db.Database.BeginTransactionAsync();
        var mevcut = await db.Kisiler.FirstAsync(k => k.TcKimlikNo != null);
        db.Kisiler.Add(new Afetzede
        {
            Ad = "Test", Soyad = "Kişi", AileId = mevcut.AileId, DogumTarihi = new DateOnly(1990, 1, 1),
            TcKimlikNo = mevcut.TcKimlikNo, KayitZamani = DateTime.Now,
        });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task KapasiteKisitiVeritabaniDuzeyindeKorunur()
    {
        await using var db = fixture.YeniContext();
        await using var islem = await db.Database.BeginTransactionAsync();
        var cadir = await db.Cadirlar.FirstAsync();
        await Assert.ThrowsAsync<Microsoft.Data.SqlClient.SqlException>(() =>
            db.Database.ExecuteSqlInterpolatedAsync($"UPDATE Cadirlar SET Doluluk = Kapasite + 1 WHERE Id = {cadir.Id}"));
    }

    [Fact]
    public async Task SaglikTalebineYardimOnceligiVerilemez()
    {
        await using var db = fixture.YeniContext();
        await using var islem = await db.Database.BeginTransactionAsync();
        db.Talepler.Add(new AfetCadirKent.Entities.Talepler.Talep
        {
            Aciklama = "Çocuk ateşleniyor", Tur = TalepTuru.Saglik, YardimOnceligi = DestekOnceligi.Yuksek,
            AcilisZamani = DateTime.Now,
        });
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public async Task AyniCadiraEsZamanliYerlesimdeIkinciKayitCakismaAlir()
    {
        await using var db1 = fixture.YeniContext();
        await using var db2 = fixture.YeniContext();
        var bolge = new Bolge { Kod = "TST", Ad = "Test bölgesi" };
        var cadir = new Cadir { Bolge = bolge, Sira = 1, Numara = 1, Kapasite = 4 };
        db1.Cadirlar.Add(cadir);
        await db1.SaveChangesAsync();

        try
        {
            var kopya = await db2.Cadirlar.Include(c => c.Bolge).SingleAsync(c => c.Id == cadir.Id);
            cadir.Yerlestir(3);
            kopya.Yerlestir(3);

            await db1.SaveChangesAsync();
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => db2.SaveChangesAsync());
        }
        finally
        {
            await using var temizle = fixture.YeniContext();
            await temizle.Cadirlar.Where(c => c.Id == cadir.Id).ExecuteDeleteAsync();
            await temizle.Bolgeler.Where(b => b.Id == bolge.Id).ExecuteDeleteAsync();
        }
    }
}

[Collection(VeritabaniKoleksiyonu.Ad)]
public class RepositoryTestleri(VeritabaniFixture fixture)
{
    [Fact]
    public async Task PasifYapilmasiGerekenKayitFizikselSilinemez()
    {
        await using var oturum = new VeriOturumu(fixture.YeniContext());
        var kisi = await oturum.Repo<Afetzede>().Sorgu().FirstAsync();
        var aile = await oturum.Repo<Aile>().Sorgu().FirstAsync();
        Assert.Throws<InvalidOperationException>(() => oturum.Repo<Afetzede>().Sil(kisi));
        Assert.Throws<InvalidOperationException>(() => oturum.Repo<Aile>().Sil(aile));
    }

    [Fact]
    public async Task SilinebilirKayitSilinirVeAyniRepoTekrarKullanilir()
    {
        await using var oturum = new VeriOturumu(fixture.YeniContext());
        await using var islem = await oturum.IslemBaslatAsync();
        var repo = oturum.Repo<Bolge>();
        Assert.Same(repo, oturum.Repo<Bolge>());

        var bolge = new Bolge { Kod = "SIL", Ad = "Silinecek bölge" };
        repo.Ekle(bolge);
        await oturum.KaydetAsync();
        Assert.True(await repo.VarMiAsync(b => b.Kod == "SIL"));

        repo.Sil(bolge);
        await oturum.KaydetAsync();
        Assert.False(await repo.VarMiAsync(b => b.Kod == "SIL"));
    }

    [Fact]
    public async Task KullanilmisKayitSilinmeyeCalisilincaVeritabaniEngeller()
    {
        await using var oturum = new VeriOturumu(fixture.YeniContext());
        await using var islem = await oturum.IslemBaslatAsync();
        var doluBolge = await oturum.Repo<Bolge>().Sorgu().FirstAsync(b => b.Kod == "A");
        oturum.Repo<Bolge>().Sil(doluBolge);
        await Assert.ThrowsAsync<DbUpdateException>(() => oturum.KaydetAsync());
    }
}


