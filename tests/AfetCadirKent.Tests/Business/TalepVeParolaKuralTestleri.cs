using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Entities.Talepler;

namespace AfetCadirKent.Tests.Business;

/// <summary>Veritabanısız kurallar: talep durum makinesi, yönlendirme, sağlık talebinin öncelik almaması, parola kuralı ve BCrypt.</summary>
public class TalepVeParolaKuralTestleri
{
    private static readonly DateTime Simdi = new(2026, 10, 18, 10, 0, 0);

    private static Talep YeniTalep() => new() { Aciklama = "Çadır su alıyor", Kaynak = TalepKaynagi.KayitMasasi, AcilisZamani = Simdi };

    [Theory]
    [InlineData(TalepTuru.Saglik, RolTuru.SaglikPersoneli)]
    [InlineData(TalepTuru.Malzeme, RolTuru.DepoSorumlusu)]
    [InlineData(TalepTuru.CadirAriza, RolTuru.SahaGorevlisi)]
    [InlineData(TalepTuru.Diger, RolTuru.Koordinator)]
    public void Yonlendirme_TureGoreHedefRol(TalepTuru tur, RolTuru beklenen)
    {
        Assert.Equal(beklenen, Talep.HedefRolu(tur));
        var talep = YeniTalep();
        talep.Siniflandir(tur, null, SiniflandirmaDurumu.GorevliBelirledi);
        Assert.Equal(beklenen, talep.HedefRol);
    }

    [Fact]
    public void Akis_YeniAtandiIslemdeTamamlandi_SonucVeKapanisZamaniYazilir()
    {
        var talep = YeniTalep();
        talep.Siniflandir(TalepTuru.CadirAriza, DestekOnceligi.Yuksek, SiniflandirmaDurumu.GorevliBelirledi);
        Assert.Equal(TalepDurumu.Yeni, talep.Durum);

        talep.Ata(7);
        Assert.Equal((TalepDurumu.Atandi, 7), (talep.Durum, talep.SorumluKullaniciId!.Value));
        talep.IslemeAl();
        Assert.Equal(TalepDurumu.Islemde, talep.Durum);

        Assert.Throws<DogrulamaException>(() => talep.Tamamla("   ", Simdi)); // sonuç zorunlu
        Assert.Equal(TalepDurumu.Islemde, talep.Durum);
        talep.Tamamla("Çadır zemini yükseltildi, branda değiştirildi.", Simdi.AddHours(2));

        Assert.Equal(TalepDurumu.Tamamlandi, talep.Durum);
        Assert.Equal(Simdi.AddHours(2), talep.KapanisZamani);
        Assert.False(talep.AcikMi);
        Assert.Throws<IsKuraliException>(() => talep.IptalEt("Geç", Simdi)); // kapanmış talep değişmez
    }

    [Fact]
    public void GecersizGecisler_Reddedilir()
    {
        var talep = YeniTalep();
        Assert.Throws<IsKuraliException>(() => talep.Ata(1));               // tür belirlenmeden atanamaz
        Assert.Throws<IsKuraliException>(() => talep.IslemeAl());           // Yeni → İşlemde yok
        Assert.Throws<IsKuraliException>(() => talep.Tamamla("x", Simdi));  // Yeni → Tamamlandı yok

        talep.Siniflandir(TalepTuru.Malzeme, null, SiniflandirmaDurumu.GorevliBelirledi);
        talep.Ata(1);
        Assert.Throws<IsKuraliException>(() => talep.Tamamla("x", Simdi));  // Atandı → Tamamlandı yok (önce işleme alınır)
        talep.IslemeAl();
        Assert.Throws<IsKuraliException>(() => talep.Ata(2));               // işlemdeki talep yeniden atanamaz
        Assert.Throws<IsKuraliException>(() =>
            talep.Siniflandir(TalepTuru.Diger, null, SiniflandirmaDurumu.GorevliBelirledi));
    }

    [Theory]
    [InlineData(TalepDurumu.Yeni)]
    [InlineData(TalepDurumu.Atandi)]
    [InlineData(TalepDurumu.Islemde)]
    public void Iptal_HerAcikDurumdanYapilir_SonucZorunlu(TalepDurumu durum)
    {
        var talep = YeniTalep();
        talep.Siniflandir(TalepTuru.Diger, null, SiniflandirmaDurumu.GorevliBelirledi);
        if (durum >= TalepDurumu.Atandi)
            talep.Ata(1);
        if (durum == TalepDurumu.Islemde)
            talep.IslemeAl();

        Assert.Throws<DogrulamaException>(() => talep.IptalEt("", Simdi));
        talep.IptalEt("Aile başka kampa nakil oldu.", Simdi);
        Assert.Equal(TalepDurumu.Iptal, talep.Durum);
        Assert.Equal("Aile başka kampa nakil oldu.", talep.Sonuc);
    }

    [Fact]
    public void SaglikTalebi_OncelikAlmaz_SaglikPersoneliDegerlendirecekIsaretlenir()
    {
        var talep = YeniTalep();
        var hata = Assert.Throws<IsKuraliException>(() =>
            talep.Siniflandir(TalepTuru.Saglik, DestekOnceligi.Yuksek, SiniflandirmaDurumu.GorevliBelirledi));
        Assert.Contains("yardım önceliği verilmez", hata.Message);

        talep.Siniflandir(TalepTuru.Saglik, null, SiniflandirmaDurumu.GorevliBelirledi);
        Assert.Null(talep.YardimOnceligi);
        Assert.True(talep.SaglikPersoneliDegerlendirecek);
        Assert.Equal(RolTuru.SaglikPersoneli, talep.HedefRol);

        // Sağlık dışı türe çevrilince işaret kalkar.
        talep.Siniflandir(TalepTuru.Malzeme, DestekOnceligi.Orta, SiniflandirmaDurumu.GorevliBelirledi);
        Assert.False(talep.SaglikPersoneliDegerlendirecek);
        Assert.Equal(DestekOnceligi.Orta, talep.YardimOnceligi);
    }

    [Fact]
    public void YenidenSiniflandirma_HedefRolDegisirseAtamaKalkar()
    {
        var talep = YeniTalep();
        talep.Siniflandir(TalepTuru.CadirAriza, null, SiniflandirmaDurumu.GorevliBelirledi);
        talep.Ata(5);

        talep.Siniflandir(TalepTuru.CadirAriza, DestekOnceligi.Orta, SiniflandirmaDurumu.GorevliBelirledi); // aynı hedef
        Assert.Equal((TalepDurumu.Atandi, 5), (talep.Durum, talep.SorumluKullaniciId!.Value));

        talep.Siniflandir(TalepTuru.Saglik, null, SiniflandirmaDurumu.GorevliBelirledi); // hedef değişti
        Assert.Equal(TalepDurumu.Yeni, talep.Durum);
        Assert.Null(talep.SorumluKullaniciId);
    }

    // ---------------------------------------------------------------- Parola

    [Theory]
    [InlineData("kisa1", "en az 8")]
    [InlineData("sadeceharf", "harf ve bir rakam")]
    [InlineData("12345678", "harf ve bir rakam")]
    [InlineData("ahmet123", "kullanıcı adıyla aynı")]
    public void ParolaKurali_ZayifParolaReddedilir(string parola, string mesaj)
    {
        var hata = Assert.Throws<DogrulamaException>(() => ParolaGuvenligi.KuralaUymali(parola, "Ahmet123"));
        Assert.Contains(mesaj, hata.Message);
    }

    [Fact]
    public void ParolaKurali_CokUzunParolaReddedilir_UygunParolaGecer()
    {
        Assert.Throws<DogrulamaException>(() => ParolaGuvenligi.KuralaUymali(new string('a', 64) + "1", "kullanici"));
        ParolaGuvenligi.KuralaUymali("Çadır2026kent", "kullanici");
    }

    [Fact]
    public void Bcrypt_DuzParolaSaklanmaz_HerOzetFarkliTuzlu_YanlisParolaVeBozukOzetReddedilir()
    {
        var ozet1 = ParolaGuvenligi.Ozetle("Guvenli123");
        var ozet2 = ParolaGuvenligi.Ozetle("Guvenli123");

        Assert.StartsWith("$2", ozet1);
        Assert.DoesNotContain("Guvenli123", ozet1);
        Assert.NotEqual(ozet1, ozet2);
        Assert.True(ParolaGuvenligi.Dogrula("Guvenli123", ozet1));
        Assert.True(ParolaGuvenligi.Dogrula("Guvenli123", ozet2));
        Assert.False(ParolaGuvenligi.Dogrula("guvenli123", ozet1));
        Assert.False(ParolaGuvenligi.Dogrula("Guvenli123", "bozuk-ozet"));
    }
}
