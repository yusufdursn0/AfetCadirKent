using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Ortak;
using AfetCadirKent.Entities.Talepler;

namespace AfetCadirKent.Tests.Business;

/// <summary>Yapay zeka cevap ayrıştırma, "acil" temizliği ve talep üzerindeki öneri kuralları (veritabanısız).</summary>
public class YapayZekaKuralTestleri
{
    [Fact]
    public void GecerliJson_TurOncelikVeGerekceOkunur()
    {
        var oneri = YzCevapAyristirici.Siniflandirma("""{"tur":"CadirAriza","oncelik":"Yuksek","gerekce":"Çadır su alıyor, barınma bozuluyor."}""");

        Assert.Equal(new TalepSiniflandirmaOnerisi(TalepTuru.CadirAriza, DestekOnceligi.Yuksek, "Çadır su alıyor, barınma bozuluyor."), oneri);
    }

    [Theory]
    [InlineData("Sağlık", TalepTuru.Saglik)]
    [InlineData("SAGLIK", TalepTuru.Saglik)]
    [InlineData("malzeme", TalepTuru.Malzeme)]
    [InlineData("Çadır/arıza", TalepTuru.CadirAriza)]
    [InlineData("cadir_ariza", TalepTuru.CadirAriza)]
    [InlineData("Diğer", TalepTuru.Diger)]
    public void TurYazimFarklari_Taninir(string tur, TalepTuru beklenen)
    {
        var oneri = YzCevapAyristirici.Siniflandirma($$"""{"tur":"{{tur}}","oncelik":"Normal"}""");
        Assert.Equal(beklenen, oneri!.Tur);
    }

    [Fact]
    public void AlanAdlariTurkceVeBuyukHarfliOlabilir_KodBlogununIcindenOkunur()
    {
        var oneri = YzCevapAyristirici.Siniflandirma("İşte sonuç:\n```json\n{\"Tür\": \"Malzeme\", \"Öncelik\": \"Orta\", \"Gerekçe\": \"Battaniye ihtiyacı\"}\n```");

        Assert.Equal((TalepTuru.Malzeme, (DestekOnceligi?)DestekOnceligi.Orta, "Battaniye ihtiyacı"), (oneri!.Tur, oneri.YardimOnceligi, oneri.Gerekce));
    }

    [Fact]
    public void SaglikOnerisi_ModelOncelikVerseBileYokSayilir()
    {
        var oneri = YzCevapAyristirici.Siniflandirma("""{"tur":"Saglik","oncelik":"Yuksek","gerekce":"Çocukta ateş var."}""");

        Assert.Equal(TalepTuru.Saglik, oneri!.Tur);
        Assert.Null(oneri.YardimOnceligi);
    }

    [Theory]
    [InlineData("""{"oncelik":"Yuksek"}""")]
    [InlineData("""{"tur":"Bilinmeyen","oncelik":"Yuksek"}""")]
    [InlineData("""{"tur":3}""")]
    [InlineData("""["Saglik"]""")]
    [InlineData("""{"tur":"Saglik" """)]
    [InlineData("Talep sağlıkla ilgili görünüyor.")]
    [InlineData("")]
    [InlineData(null)]
    public void OkunamayanCevap_NullDoner(string? cevap) => Assert.Null(YzCevapAyristirici.Siniflandirma(cevap));

    [Theory]
    [InlineData("""{"tur":"Malzeme","oncelik":"Kritik"}""")]
    [InlineData("""{"tur":"Malzeme","oncelik":null}""")]
    [InlineData("""{"tur":"Malzeme"}""")]
    public void TaninmayanVeyaEksikOncelik_TurKorunurOncelikBosKalir(string cevap)
    {
        var oneri = YzCevapAyristirici.Siniflandirma(cevap);
        Assert.Equal((TalepTuru.Malzeme, (DestekOnceligi?)null, (string?)null), (oneri!.Tur, oneri.YardimOnceligi, oneri.Gerekce));
    }

    [Fact]
    public void UzunGerekce_SutunUzunluguncaKisaltilir()
    {
        var oneri = YzCevapAyristirici.Siniflandirma($$"""{"tur":"Diger","oncelik":"Normal","gerekce":"{{new string('x', 3000)}}"}""");
        Assert.Equal(YzCevapAyristirici.GerekceUzunlugu, oneri!.Gerekce!.Length);
    }

    [Theory]
    [InlineData("Acil battaniye gerekiyor.", "Öncelikli battaniye gerekiyor.")]
    [InlineData("ACİL durum", "Öncelikli durum")]
    [InlineData("Su acilen ulaştırılmalı; aciliyet yüksek.", "Su öncelikle ulaştırılmalı; öncelik yüksek.")]
    [InlineData("Kacil bir kelime değil", "Kacil bir kelime değil")]
    public void AcilKelimesi_EkranaCikmaz(string metin, string beklenen)
    {
        Assert.Equal(beklenen, YzCevapAyristirici.Ozet(metin));
        var oneri = YzCevapAyristirici.Siniflandirma($$"""{"tur":"Malzeme","gerekce":"{{metin}}"}""");
        Assert.Equal(beklenen, oneri!.Gerekce);
    }

    [Fact]
    public void BosOzet_OkunamadiSayilir()
    {
        Assert.Null(YzCevapAyristirici.Ozet("  \n "));
        Assert.Equal("Bugün durum sakin.", YzCevapAyristirici.Ozet("\n Bugün durum sakin. \n"));
    }

    [Fact]
    public void SiniflandirmaIstemi_TibbiDegerlendirmeYasaginiVeTalepMetniniIcerir()
    {
        Assert.Contains("tıbbi aciliyet değerlendirmesi yapma", YzIstemleri.SiniflandirmaSistemi);
        Assert.Contains("\"oncelik\" her zaman null", YzIstemleri.SiniflandirmaSistemi);
        Assert.Contains("Çocuk ateşleniyor", YzIstemleri.SiniflandirmaKullanici("  Çocuk ateşleniyor "));
    }

    // ---------------------------------------------------------------- Talep üzerindeki öneri kuralları (K-077)

    [Fact]
    public void YapayZekaOnerisi_TuruUygulamaz_GorevliOnayiniBekler()
    {
        var talep = new Talep { Aciklama = "Çadır su alıyor" };

        talep.YapayZekaOnerisiKaydet(TalepTuru.CadirAriza, DestekOnceligi.Yuksek, "Barınma bozuluyor.");

        Assert.Equal(SiniflandirmaDurumu.YapayZekaOnerdi, talep.SiniflandirmaDurumu);
        Assert.Equal((TalepTuru?)TalepTuru.CadirAriza, talep.YzOnerilenTur);
        Assert.Equal(DestekOnceligi.Yuksek, talep.YzOnerilenOncelik);
        Assert.Null(talep.Tur);
        Assert.Null(talep.HedefRol);
        Assert.Throws<IsKuraliException>(() => talep.Ata(1)); // türü belirlenmeden atanamaz

        talep.YapayZekaOnerisiniOnayla();

        Assert.Equal((TalepTuru.CadirAriza, DestekOnceligi.Yuksek, RolTuru.SahaGorevlisi, SiniflandirmaDurumu.GorevliBelirledi),
            (talep.Tur!.Value, talep.YardimOnceligi!.Value, talep.HedefRol!.Value, talep.SiniflandirmaDurumu));
    }

    [Fact]
    public void SaglikOnerisi_OncelikAlmadanDogrudanSaglikPersonelineGider()
    {
        var talep = new Talep { Aciklama = "Çocuk ateşleniyor" };

        talep.YapayZekaOnerisiKaydet(TalepTuru.Saglik, DestekOnceligi.Yuksek, "Ateş");

        Assert.Equal(TalepTuru.Saglik, talep.Tur);
        Assert.Equal(RolTuru.SaglikPersoneli, talep.HedefRol);
        Assert.True(talep.SaglikPersoneliDegerlendirecek);
        Assert.Null(talep.YardimOnceligi);
        Assert.Null(talep.YzOnerilenOncelik);
        Assert.Equal(SiniflandirmaDurumu.YapayZekaOnerdi, talep.SiniflandirmaDurumu);
    }

    [Fact]
    public void SiniflandirilmisVeyaKapaliTalep_YapayZekaOnerisiAlmaz()
    {
        var siniflandirilmis = new Talep { Aciklama = "x" };
        siniflandirilmis.Siniflandir(TalepTuru.Malzeme, DestekOnceligi.Normal, SiniflandirmaDurumu.GorevliBelirledi);
        Assert.Throws<IsKuraliException>(() => siniflandirilmis.YapayZekaOnerisiKaydet(TalepTuru.Diger, null, null));

        var kapali = new Talep { Aciklama = "x" };
        kapali.IptalEt("Mükerrer talep", DateTime.Now);
        Assert.Throws<IsKuraliException>(() => kapali.YapayZekaOnerisiAlinabilirOlmali());
    }

    [Fact]
    public void OneriYokken_OnaylanamazGorevliKarariUzerineOnaylanamaz()
    {
        var talep = new Talep { Aciklama = "x" };
        Assert.Throws<IsKuraliException>(talep.YapayZekaOnerisiniOnayla);

        talep.YapayZekaOnerisiKaydet(TalepTuru.Malzeme, DestekOnceligi.Orta, null);
        talep.Siniflandir(TalepTuru.Diger, DestekOnceligi.Normal, SiniflandirmaDurumu.GorevliBelirledi); // görevli farklı karar verdi
        Assert.Throws<IsKuraliException>(talep.YapayZekaOnerisiniOnayla);
        Assert.Equal(TalepTuru.Diger, talep.Tur);
    }
}
