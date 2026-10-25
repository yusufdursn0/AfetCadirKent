using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Tests.Business;

/// <summary>Türsüz açılan taleplerin arka planda yapay zekaya gönderilmesi (K-080).</summary>
[Collection(VeritabaniKoleksiyonu.Ad)]
public class OtomatikSiniflandiriciTestleri(VeritabaniFixture fixture)
{
    private async Task<TalepOzeti> KayitMasasiTalebiAsync(string aciklama)
    {
        var kayit = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.KayitGorevlisi));
        return await kayit.OlusturAsync(new TalepBilgisi { Aciklama = aciklama, Kaynak = TalepKaynagi.KayitMasasi });
    }

    private async Task<TalepServisi> KoordinatorAsync() =>
        fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.Koordinator));

    private OtomatikSiniflandirici Siniflandirici(IYapayZekaServisi yapayZeka) =>
        new(fixture.OturumFabrikasi(), TimeProvider.System, yapayZeka);

    [Fact]
    public async Task TursuzTalepSirayaAlinir_TuruBelliTalepAlinmaz()
    {
        var kuyruk = new SahteKuyruk();
        var kayit = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.KayitGorevlisi), kuyruk: kuyruk);

        var tursuz = await kayit.OlusturAsync(new TalepBilgisi { Aciklama = "Çocuk ateşleniyor", Kaynak = TalepKaynagi.KayitMasasi });
        await kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Su bitti", Kaynak = TalepKaynagi.KayitMasasi, Tur = TalepTuru.Malzeme, YardimOnceligi = DestekOnceligi.Normal,
        });

        Assert.Equal([tursuz.Id], kuyruk.Eklenenler);
    }

    [Fact]
    public async Task Senaryo5_SaglikTalebi_GorevliBeklemedenSaglikPersonelineDuser()
    {
        var talep = await KayitMasasiTalebiAsync("Çocuk ateşleniyor");
        var yz = new SahteYapayZeka(new TalepSiniflandirmaOnerisi(TalepTuru.Saglik, DestekOnceligi.Yuksek, "Ateş"));

        Assert.True(await Siniflandirici(yz).IsleAsync(talep.Id));

        var saglik = fixture.TalepServisi(await fixture.KullaniciOturumuAsync(RolTuru.SaglikPersoneli));
        var gorunen = Assert.Single(await saglik.ListeleAsync(new TalepFiltresi()), t => t.Id == talep.Id);
        Assert.Equal((TalepTuru.Saglik, (DestekOnceligi?)null, "Sağlık personeli değerlendirecek"),
            (gorunen.Tur!.Value, gorunen.YardimOnceligi, gorunen.OncelikMetni));

        await using var db = fixture.YeniContext();
        var kayit = await db.IslemGecmisi.Where(i => i.VarlikAdi == "Talep" && i.VarlikId == talep.Id).OrderBy(i => i.Id).LastAsync();
        Assert.Equal("sistem", kayit.KullaniciAdi);
        Assert.Null(kayit.KullaniciId);
        Assert.Contains("yapay zeka önerisi: Sağlık", kayit.Aciklama);
    }

    [Fact]
    public async Task SaglikDisiOneri_GorevliOnayiniBekler_IkinciKezSorulmaz()
    {
        var talep = await KayitMasasiTalebiAsync("Çadır su alıyor");
        var yz = new SahteYapayZeka(new TalepSiniflandirmaOnerisi(TalepTuru.CadirAriza, DestekOnceligi.Yuksek, null));

        Assert.True(await Siniflandirici(yz).IsleAsync(talep.Id));
        Assert.False(await Siniflandirici(yz).IsleAsync(talep.Id));

        var sonuc = await (await KoordinatorAsync()).GetirAsync(talep.Id);
        Assert.Equal((SiniflandirmaDurumu.YapayZekaOnerdi, (TalepTuru?)null, (TalepTuru?)TalepTuru.CadirAriza),
            (sonuc.SiniflandirmaDurumu, sonuc.Tur, sonuc.YzOnerilenTur));
        Assert.Single(yz.GelenTalepler);
    }

    [Fact]
    public async Task OllamaKapaliysa_TalepDegismez()
    {
        var talep = await KayitMasasiTalebiAsync("Battaniye lazım");
        var log = new ListeLogger<OtomatikSiniflandirici>();

        Assert.False(await new OtomatikSiniflandirici(fixture.OturumFabrikasi(), TimeProvider.System, new SahteYapayZeka(), log)
            .IsleAsync(talep.Id));

        var sonuc = await (await KoordinatorAsync()).GetirAsync(talep.Id);
        Assert.Equal((SiniflandirmaDurumu.Siniflandirilmamis, (TalepTuru?)null), (sonuc.SiniflandirmaDurumu, sonuc.Tur));
        Assert.Contains(log.Kayitlar, k => k.Mesaj.Contains($"#{talep.Id}") && k.Mesaj.Contains("ulaşılamadı"));
    }

    [Fact]
    public async Task SiniflandirilmisKapaliVeOlmayanTalebeDokunmaz()
    {
        var koordinator = await KoordinatorAsync();
        var siniflandirilmis = await KayitMasasiTalebiAsync("Isıtıcı bozuk");
        await koordinator.SiniflandirAsync(siniflandirilmis.Id, TalepTuru.CadirAriza, DestekOnceligi.Orta);
        var kapali = await KayitMasasiTalebiAsync("Yanlışlıkla açıldı");
        await koordinator.IptalEtAsync(kapali.Id, "Mükerrer talep");
        var yz = new SahteYapayZeka(new TalepSiniflandirmaOnerisi(TalepTuru.Diger, null, null));

        Assert.False(await Siniflandirici(yz).IsleAsync(siniflandirilmis.Id));
        Assert.False(await Siniflandirici(yz).IsleAsync(kapali.Id));
        Assert.False(await Siniflandirici(yz).IsleAsync(int.MaxValue));
        Assert.Empty(yz.GelenTalepler);
    }

    [Fact]
    public async Task GorevliAyniAndaSiniflandirirsa_KarariKorunur_HataLoglanir()
    {
        var talep = await KayitMasasiTalebiAsync("Çadır yırtıldı");
        var koordinator = await KoordinatorAsync();
        var log = new ListeLogger<OtomatikSiniflandirici>();
        var yz = new GecikmeliYapayZeka(() => koordinator.SiniflandirAsync(talep.Id, TalepTuru.CadirAriza, DestekOnceligi.Normal),
            new TalepSiniflandirmaOnerisi(TalepTuru.Diger, null, null));

        Assert.False(await new OtomatikSiniflandirici(fixture.OturumFabrikasi(), TimeProvider.System, yz, log).IsleAsync(talep.Id));

        Assert.Contains(log.Kayitlar, k => k.Hata is EszamanlilikException);
        var sonuc = await koordinator.GetirAsync(talep.Id);
        Assert.Equal((TalepTuru.CadirAriza, (TalepTuru?)null), (sonuc.Tur!.Value, sonuc.YzOnerilenTur));
    }

    [Fact]
    public async Task ArkaPlanDongusu_AcilistaBekleyenleriVeSonradanEklenenleriSiraylaIsler()
    {
        var bekleyen = await KayitMasasiTalebiAsync($"Bekleyen talep {Guid.NewGuid():N}");
        var siniflandirilmis = await KayitMasasiTalebiAsync($"Sınıflandırılmış talep {Guid.NewGuid():N}");
        await (await KoordinatorAsync()).SiniflandirAsync(siniflandirilmis.Id, TalepTuru.Diger, DestekOnceligi.Normal);

        var yz = new SahteYapayZeka(); // Ollama kapalı: hiçbir talep değişmez, yalnızca sorulan metinler görülür
        var siniflandirici = Siniflandirici(yz);
        var sayi = await siniflandirici.BekleyenleriEkleAsync();
        var sonradan = await KayitMasasiTalebiAsync($"Sonradan gelen {Guid.NewGuid():N}");
        siniflandirici.Ekle(sonradan.Id);

        using var iptal = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var dongu = siniflandirici.CalistirAsync(iptal.Token);
        while (yz.GelenTalepler.Count < sayi + 1 && !iptal.IsCancellationRequested)
            await Task.Delay(20);
        await iptal.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => dongu);

        Assert.True(sayi >= 1);
        Assert.Contains(bekleyen.Aciklama, yz.GelenTalepler);
        Assert.Equal(sonradan.Aciklama, yz.GelenTalepler.Last());
        Assert.DoesNotContain(siniflandirilmis.Aciklama, yz.GelenTalepler);
    }

    /// <summary>Cevap vermeden önce verilen işi yapan (ör. görevlinin aynı anda kayıt yapması) sahte yapay zeka.</summary>
    private sealed class GecikmeliYapayZeka(Func<Task> once, TalepSiniflandirmaOnerisi oneri) : IYapayZekaServisi
    {
        public async Task<YzSonucu<TalepSiniflandirmaOnerisi>> TalepSiniflandirAsync(string talepMetni, CancellationToken iptal = default)
        {
            await once();
            return YzSonucu<TalepSiniflandirmaOnerisi>.Basari(oneri);
        }

        public Task<YzSonucu<string>> GunlukOzetYazAsync(GunlukDurum durum, CancellationToken iptal = default) =>
            Task.FromResult(YzSonucu<string>.Basari("-"));
    }
}
