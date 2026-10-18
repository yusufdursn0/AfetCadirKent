using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kayit;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;
using Microsoft.EntityFrameworkCore;
using static AfetCadirKent.Tests.Altyapi.TestYardimcilari;

namespace AfetCadirKent.Tests.Business;

[Collection(VeritabaniKoleksiyonu.Ad)]
public class TalepServisiTestleri(VeritabaniFixture fixture)
{
    private async Task<(TalepServisi Servis, Oturum Oturum)> RolAsync(RolTuru rol)
    {
        var oturum = await fixture.KullaniciOturumuAsync(rol);
        return (fixture.TalepServisi(oturum), oturum);
    }

    /// <summary>Teste özel bölgede tek çadıra yerleşmiş aile.</summary>
    private async Task<(Aile Aile, int CadirId)> CadirdakiAileAsync()
    {
        var cadirServisi = fixture.CadirServisi(RolTuru.KayitGorevlisi);
        var bolge = await cadirServisi.BolgeKaydetAsync(null, BenzersizBolgeKodu(), "Talep testi", null);
        var cadir = await cadirServisi.CadirEkleAsync(new CadirBilgisi { BolgeId = bolge.Id, Sira = 1, Numara = 1, Kapasite = 4 });
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.KayitGorevlisi).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Anne", soyad, 30), Kisi("Çocuk", soyad, 4)]);
        await cadirServisi.YerlestirAsync(aile.Id, [new CadirAtamasi(cadir.Id, "?", 2)]);
        return (aile, cadir.Id);
    }

    [Fact]
    public async Task Senaryo4_CadirSuAliyor_SiniflandirilirAtanirSonucYazilarakKapanir()
    {
        var (aile, cadirId) = await CadirdakiAileAsync();
        var (kayit, _) = await RolAsync(RolTuru.KayitGorevlisi);
        var (koordinator, _) = await RolAsync(RolTuru.Koordinator);
        var (saha, sahaOturumu) = await RolAsync(RolTuru.SahaGorevlisi);
        var (baskaSaha, _) = await RolAsync(RolTuru.SahaGorevlisi);

        // Kayıt masası türü bilmeden açar: çadır, ailenin tek çadırından kendiliğinden gelir.
        var talep = await kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Çadır su alıyor", Kaynak = TalepKaynagi.KayitMasasi, AileId = aile.Id,
        });
        Assert.Equal((TalepDurumu.Yeni, (TalepTuru?)null, cadirId), (talep.Durum, talep.Tur, talep.CadirId!.Value));
        Assert.Equal("Sınıflandırılmamış", talep.TurMetni);
        Assert.Matches(@"^T[A-Z]{4}1-01$", talep.CadirKodu!);
        Assert.Contains(await kayit.ListeleAsync(new TalepFiltresi { SadeceSiniflandirilmamis = true }), t => t.Id == talep.Id);

        await koordinator.SiniflandirAsync(talep.Id, TalepTuru.CadirAriza, DestekOnceligi.Yuksek);
        var atanabilir = await koordinator.AtanabilirGorevlilerAsync(talep.Id);
        Assert.Contains(atanabilir, g => g.Id == sahaOturumu.KullaniciId);
        Assert.All(atanabilir, g => Assert.Contains(g.Rol, new[] { RolTuru.SahaGorevlisi, RolTuru.Koordinator }));

        await koordinator.AtaAsync(talep.Id, sahaOturumu.KullaniciId!.Value);

        // Saha görevlisi yalnızca kendine atananları görür.
        var sahaninListesi = await saha.ListeleAsync(new TalepFiltresi());
        Assert.Equal([talep.Id], sahaninListesi.Select(t => t.Id));
        Assert.Empty(await baskaSaha.ListeleAsync(new TalepFiltresi()));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => baskaSaha.IslemeAlAsync(talep.Id));

        await saha.IslemeAlAsync(talep.Id);
        await Assert.ThrowsAsync<DogrulamaException>(() => saha.TamamlaAsync(talep.Id, " "));
        await saha.TamamlaAsync(talep.Id, "Çadırın çevresine drenaj kanalı açıldı, branda değiştirildi.");

        var kapali = await kayit.GetirAsync(talep.Id); // açan görevli kendi talebini görmeye devam eder
        Assert.Equal(TalepDurumu.Tamamlandi, kapali.Durum);
        Assert.Equal("Yardım önceliği: Yüksek", kapali.OncelikMetni);
        Assert.NotNull(kapali.KapanisZamani);
        Assert.StartsWith("Çadırın çevresine", kapali.Sonuc);
        Assert.Equal(RolTuru.SahaGorevlisi, kapali.HedefRol);

        await using var db = fixture.YeniContext();
        Assert.Equal(5, await db.IslemGecmisi.CountAsync(i => i.VarlikAdi == "Talep" && i.VarlikId == talep.Id));
    }

    [Fact]
    public async Task Senaryo5_CocukAtesleniyor_OncelikAlmadanSaglikPersoneline_Gider()
    {
        var (aile, _) = await CadirdakiAileAsync();
        var (kayit, _) = await RolAsync(RolTuru.KayitGorevlisi);
        var (saglik, saglikOturumu) = await RolAsync(RolTuru.SaglikPersoneli);
        var (baskaSaglik, baskaSaglikOturumu) = await RolAsync(RolTuru.SaglikPersoneli);
        var (depo, depoOturumu) = await RolAsync(RolTuru.DepoSorumlusu);
        var (saha, _) = await RolAsync(RolTuru.SahaGorevlisi);

        await Assert.ThrowsAsync<IsKuraliException>(() => kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Çocuk ateşleniyor", Kaynak = TalepKaynagi.KayitMasasi, AileId = aile.Id,
            Tur = TalepTuru.Saglik, YardimOnceligi = DestekOnceligi.Yuksek,
        }));
        var talep = await kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Çocuk ateşleniyor", Kaynak = TalepKaynagi.KayitMasasi, AileId = aile.Id, Tur = TalepTuru.Saglik,
        });

        Assert.Null(talep.YardimOnceligi);
        Assert.True(talep.SaglikPersoneliDegerlendirecek);
        Assert.Equal("Sağlık personeli değerlendirecek", talep.OncelikMetni);
        Assert.Equal(RolTuru.SaglikPersoneli, talep.HedefRol);

        // Sağlık personeli görür; depo ve saha görmez.
        Assert.Contains(await saglik.ListeleAsync(new TalepFiltresi { SadeceAcik = true }), t => t.Id == talep.Id);
        Assert.DoesNotContain(await depo.ListeleAsync(new TalepFiltresi()), t => t.Id == talep.Id);
        Assert.DoesNotContain(await saha.ListeleAsync(new TalepFiltresi()), t => t.Id == talep.Id);

        // Sağlık personeli talebi üstlenir; başkasına atayamaz; depo kendi rolü olmayan talebi üstlenemez.
        await Assert.ThrowsAsync<YetkisizIslemException>(() => saglik.AtaAsync(talep.Id, baskaSaglikOturumu.KullaniciId!.Value));
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => depo.AtaAsync(talep.Id, depoOturumu.KullaniciId!.Value));
        await saglik.AtaAsync(talep.Id, saglikOturumu.KullaniciId!.Value);
        Assert.Equal(TalepDurumu.Atandi, (await saglik.GetirAsync(talep.Id)).Durum);

        // Başka sağlık personeli görür ama sorumlu olmadığı için ilerletemez.
        await Assert.ThrowsAsync<YetkisizIslemException>(() => baskaSaglik.IslemeAlAsync(talep.Id));
        await saglik.IslemeAlAsync(talep.Id);
        await saglik.TamamlaAsync(talep.Id, "Sağlık noktasında muayene edildi.");
        Assert.Equal(TalepDurumu.Tamamlandi, (await kayit.GetirAsync(talep.Id)).Durum);
    }

    [Fact]
    public async Task Atama_YalnizHedefRoldekiVeyaKoordinatoreYapilir_TurDegisinceAtamaKalkar()
    {
        var (koordinator, koordinatorOturumu) = await RolAsync(RolTuru.Koordinator);
        var (_, depoOturumu) = await RolAsync(RolTuru.DepoSorumlusu);
        var (_, sahaOturumu) = await RolAsync(RolTuru.SahaGorevlisi);

        var talep = await koordinator.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Battaniye lazım", Kaynak = TalepKaynagi.Gorevli, Tur = TalepTuru.Malzeme, YardimOnceligi = DestekOnceligi.Orta,
        });
        var hata = await Assert.ThrowsAsync<IsKuraliException>(() => koordinator.AtaAsync(talep.Id, sahaOturumu.KullaniciId!.Value));
        Assert.Contains("Depo sorumlusu", hata.Message);

        await koordinator.AtaAsync(talep.Id, depoOturumu.KullaniciId!.Value);
        await koordinator.SiniflandirAsync(talep.Id, TalepTuru.CadirAriza, null); // depo artık uygun değil
        var guncel = await koordinator.GetirAsync(talep.Id);
        Assert.Equal((TalepDurumu.Yeni, (int?)null), (guncel.Durum, guncel.SorumluKullaniciId));

        await koordinator.AtaAsync(talep.Id, koordinatorOturumu.KullaniciId!.Value); // Koordinatör her türü üstlenebilir
        await koordinator.IptalEtAsync(talep.Id, "Talep mükerrer açılmış.");
        await Assert.ThrowsAsync<IsKuraliException>(() => koordinator.SiniflandirAsync(talep.Id, TalepTuru.Diger, null));
    }

    [Fact]
    public async Task Iptal_AcanGorevliYeniTalebiniIptalEder_SonucZorunlu_BaskasiIptalEdemez()
    {
        var (kayit, _) = await RolAsync(RolTuru.KayitGorevlisi);
        var (baskaKayit, _) = await RolAsync(RolTuru.KayitGorevlisi);

        var talep = await kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Su deposu boşaldı", Kaynak = TalepKaynagi.KayitMasasi, Tur = TalepTuru.Malzeme,
        });
        Assert.DoesNotContain(await baskaKayit.ListeleAsync(new TalepFiltresi()), t => t.Id == talep.Id); // sınıflandırılmış, başkasının
        await Assert.ThrowsAsync<KayitBulunamadiException>(() => baskaKayit.IptalEtAsync(talep.Id, "Deneme"));

        await Assert.ThrowsAsync<DogrulamaException>(() => kayit.IptalEtAsync(talep.Id, ""));
        await kayit.IptalEtAsync(talep.Id, "Yanlışlıkla açıldı.");
        Assert.Equal(TalepDurumu.Iptal, (await kayit.GetirAsync(talep.Id)).Durum);
    }

    [Fact]
    public async Task Yetki_KioskKaynagiVeKioskRoluBuradanTalepAcamaz_OncelikTurdenOnceVerilemez()
    {
        var (kayit, _) = await RolAsync(RolTuru.KayitGorevlisi);
        var (kiosk, _) = await RolAsync(RolTuru.Kiosk);

        await Assert.ThrowsAsync<IsKuraliException>(() => kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Kiosktan", Kaynak = TalepKaynagi.Kiosk,
        }));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => kiosk.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Deneme", Kaynak = TalepKaynagi.KayitMasasi,
        }));
        await Assert.ThrowsAsync<YetkisizIslemException>(() => kiosk.ListeleAsync(new TalepFiltresi()));
        await Assert.ThrowsAsync<DogrulamaException>(() => kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Türsüz öncelik", Kaynak = TalepKaynagi.KayitMasasi, YardimOnceligi = DestekOnceligi.Orta,
        }));
    }

    [Fact]
    public async Task RefakatsizCocukAilesininTalebi_YalnizKoordinatoreGorunur()
    {
        var soyad = BenzersizSoyad();
        var aile = await fixture.KayitServisi(RolTuru.Koordinator).AileOlusturAsync($"{soyad} Ailesi", false,
            [Kisi("Çocuk", soyad, 13, refakatsiz: true)]);
        var (koordinator, _) = await RolAsync(RolTuru.Koordinator);
        var (kayit, _) = await RolAsync(RolTuru.KayitGorevlisi);

        await Assert.ThrowsAsync<KayitBulunamadiException>(() => kayit.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Kıyafet lazım", Kaynak = TalepKaynagi.KayitMasasi, AileId = aile.Id,
        }));
        var talep = await koordinator.OlusturAsync(new TalepBilgisi
        {
            Aciklama = "Kıyafet lazım", Kaynak = TalepKaynagi.Gorevli, AileId = aile.Id,
        });

        Assert.DoesNotContain(await kayit.ListeleAsync(new TalepFiltresi { SadeceSiniflandirilmamis = true }), t => t.Id == talep.Id);
        Assert.Contains(await koordinator.ListeleAsync(new TalepFiltresi { AileId = aile.Id }), t => t.Id == talep.Id);
    }
}
