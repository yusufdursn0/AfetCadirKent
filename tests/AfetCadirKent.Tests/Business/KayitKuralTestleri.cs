using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Tests.Altyapi;

namespace AfetCadirKent.Tests.Business;

public class MukerrerKontrolTestleri
{
    private static readonly DateOnly Bugun = new(2026, 10, 16);

    private static Afetzede Kayitli(string ad, string soyad, int yas) =>
        new() { Ad = ad, Soyad = soyad, DogumTarihi = Bugun.AddYears(-yas).AddDays(-30) };

    [Fact]
    public void TurkceKarakterFarkliAyniIsimVeYakinYasEslesir()
    {
        var adaylar = new[] { Kayitli("Şükrü", "Çağlı", 30) };
        var sonuc = MukerrerKontrol.OlasiEslesmeler("SUKRU", "cagli", Bugun.AddYears(-31).AddDays(-30), adaylar, Bugun);
        Assert.Single(sonuc);
    }

    [Theory]
    [InlineData(30, true)]
    [InlineData(31, true)]
    [InlineData(29, true)]
    [InlineData(32, false)] // yaş farkı 2: "2'den az" değil
    [InlineData(28, false)]
    public void YasFarkiIkidenAzOlmali(int yeniYas, bool eslesir)
    {
        var adaylar = new[] { Kayitli("Ayşe", "Kaya", 30) };
        var sonuc = MukerrerKontrol.OlasiEslesmeler("Ayşe", "Kaya", Bugun.AddYears(-yeniYas).AddDays(-30), adaylar, Bugun);
        Assert.Equal(eslesir, sonuc.Count == 1);
    }

    [Fact]
    public void FarkliSoyadEslesmez() =>
        Assert.Empty(MukerrerKontrol.OlasiEslesmeler("Ayşe", "Kara", Bugun.AddYears(-30), [Kayitli("Ayşe", "Kaya", 30)], Bugun));
}

public class KisiDurumGecisTestleri
{
    [Theory]
    [InlineData(KisiDurumu.Burada, KisiDurumu.Hastanede, KisiHareketTuru.HastaneyeSevk)]
    [InlineData(KisiDurumu.Hastanede, KisiDurumu.Burada, KisiHareketTuru.HastanedenDonus)]
    [InlineData(KisiDurumu.Burada, KisiDurumu.Ayrildi, KisiHareketTuru.Ayrilis)]
    [InlineData(KisiDurumu.Hastanede, KisiDurumu.Nakil, KisiHareketTuru.Nakil)]
    [InlineData(KisiDurumu.Ayrildi, KisiDurumu.Burada, KisiHareketTuru.Gelis)]
    public void IzinVerilenGecisler(KisiDurumu eski, KisiDurumu yeni, KisiHareketTuru hareket) =>
        Assert.Equal(hareket, KayitServisi.DurumGecisi(eski, yeni));

    [Theory]
    [InlineData(KisiDurumu.Burada, KisiDurumu.Burada)]
    [InlineData(KisiDurumu.Ayrildi, KisiDurumu.Hastanede)]
    [InlineData(KisiDurumu.Nakil, KisiDurumu.Ayrildi)]
    public void IzinVerilmeyenGecisler(KisiDurumu eski, KisiDurumu yeni) =>
        Assert.Throws<IsKuraliException>(() => KayitServisi.DurumGecisi(eski, yeni));

    [Theory]
    [InlineData(KisiDurumu.Burada, new[] { KisiDurumu.Ayrildi, KisiDurumu.Nakil, KisiDurumu.Hastanede })]
    [InlineData(KisiDurumu.Hastanede, new[] { KisiDurumu.Burada, KisiDurumu.Ayrildi, KisiDurumu.Nakil })]
    [InlineData(KisiDurumu.Ayrildi, new[] { KisiDurumu.Burada })]
    [InlineData(KisiDurumu.Nakil, new[] { KisiDurumu.Burada })]
    public void EkrandaSunulanDurumlar_GecisKuraliylaAyni(KisiDurumu eski, KisiDurumu[] beklenen)
    {
        var gecilebilir = KayitServisi.GecilebilirDurumlar(eski);

        Assert.Equal(beklenen.Order(), gecilebilir.Order());
        // Sunulan her durum gerçekten geçilebilir, sunulmayanlar reddedilir.
        foreach (var yeni in Enum.GetValues<KisiDurumu>())
        {
            if (gecilebilir.Contains(yeni))
                KayitServisi.DurumGecisi(eski, yeni);
            else
                Assert.Throws<IsKuraliException>(() => KayitServisi.DurumGecisi(eski, yeni));
        }
    }
}

public class OturumTestleri
{
    [Fact]
    public void RolIzinleriYetkiMatrisineGoredir()
    {
        var saha = TestYardimcilari.OturumAc(RolTuru.SahaGorevlisi);
        Assert.True(saha.IzinVarMi(Modul.Talep, Izin.Degistirme));
        Assert.False(saha.IzinVarMi(Modul.Kayit, Izin.Gorme));
        Assert.Throws<YetkisizIslemException>(() => saha.IzinGerekli(Modul.Depo, Izin.Gorme));

        var koordinator = TestYardimcilari.OturumAc(RolTuru.Koordinator);
        Assert.All(Enum.GetValues<Modul>(), m => Assert.True(koordinator.IzinVarMi(m, Izin.Rapor)));
    }

    [Theory]
    [InlineData(RolTuru.Koordinator, true, true)]
    [InlineData(RolTuru.KayitGorevlisi, true, true)]
    [InlineData(RolTuru.DepoSorumlusu, true, false)]
    [InlineData(RolTuru.SaglikPersoneli, true, true)]
    [InlineData(RolTuru.SahaGorevlisi, true, true)]
    [InlineData(RolTuru.Kiosk, false, false)]
    public void SaglikBilgisiIzinleriRoleGoredir(RolTuru rol, bool gorur, bool degistirir)
    {
        var oturum = TestYardimcilari.OturumAc(rol);
        Assert.Equal(gorur, oturum.IzinVarMi(Modul.SaglikBilgisi, Izin.Gorme));
        Assert.Equal(degistirir, oturum.IzinVarMi(Modul.SaglikBilgisi, Izin.Degistirme));
    }

    [Fact]
    public void KapaliOturumHicbirIzneSahipDegildir()
    {
        var oturum = TestYardimcilari.OturumAc(RolTuru.Koordinator);
        oturum.Kapat();
        Assert.False(oturum.AcikMi);
        var hata = Assert.Throws<YetkisizIslemException>(() => oturum.IzinGerekli(Modul.Kayit, Izin.Gorme));
        Assert.Contains("giriş", hata.Message);
    }
}
