using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Entities.Cadirlar;
using AfetCadirKent.Entities.Enumlar;

namespace AfetCadirKent.Tests.Business;

public class CadirOneriMotoruTestleri
{
    private static int _id;

    private static Cadir C(string bolge, int numara, int kapasite, int dolu = 0, bool isitmali = false, bool saglik = false,
        bool erisilebilir = false, CadirDurumu durum = CadirDurumu.Kullanilabilir)
    {
        var cadir = new Cadir
        {
            Id = Interlocked.Increment(ref _id), BolgeId = bolge[0], Bolge = new Bolge { Kod = bolge, Ad = bolge },
            Sira = 1, Numara = numara, Kapasite = kapasite,
            Isitmali = isitmali, SaglikNoktasinaYakin = saglik, Erisilebilir = erisilebilir,
        };
        if (dolu > 0)
            cadir.Yerlestir(dolu);
        cadir.Durum = durum;
        return cadir;
    }

    [Fact]
    public void KullanilamayanVeYetersizCadirlarElenir()
    {
        var cadirlar = new[]
        {
            C("A", 1, 6, durum: CadirDurumu.Bakimda),
            C("A", 2, 6, durum: CadirDurumu.Kapali),
            C("A", 3, 4, dolu: 2),          // 2 boş yatak, 3 kişi sığmaz
            C("A", 4, 4),
        };
        var oneri = Assert.Single(CadirOneriMotoru.OneriUret(3, [], cadirlar));
        Assert.Equal("A1-04", oneri.Atamalar.Single().CadirKodu);
    }

    [Fact]
    public void EsitUyumdaEnAzBosYatakIsrafiOncelenir()
    {
        var oneriler = CadirOneriMotoru.OneriUret(3, [], [C("A", 1, 8), C("A", 2, 4), C("A", 3, 6)]);
        Assert.Equal(["A1-02", "A1-03", "A1-01"], oneriler.Select(o => o.Atamalar[0].CadirKodu));
        Assert.Equal(1, oneriler[0].BosYatakIsrafi);
    }

    [Fact]
    public void YerlesimEtkisiIsraftanOnceGelir()
    {
        // Bebekli aile: ısıtmalı çadır daha fazla israf etse de önce gelir.
        var oneriler = CadirOneriMotoru.OneriUret(3, [YerlesimEtkisi.SicakCadir], [C("A", 1, 4), C("B", 1, 8, isitmali: true)]);
        Assert.Equal("B1-01", oneriler[0].Atamalar[0].CadirKodu);
        Assert.Equal([YerlesimEtkisi.SicakCadir], oneriler[0].KarsilananEtkiler);
        Assert.Equal([YerlesimEtkisi.SicakCadir], oneriler[1].KarsilanmayanEtkiler);
    }

    [Fact]
    public void YeterliYerYerlesimEtkisindenOnceGelir()
    {
        // Etkiyi karşılayan ama sığmayan çadır önerilmez.
        var oneriler = CadirOneriMotoru.OneriUret(4, [YerlesimEtkisi.ErisilebilirCadir], [C("A", 1, 4, dolu: 1, erisilebilir: true), C("A", 2, 4)]);
        Assert.Equal("A1-02", Assert.Single(oneriler).Atamalar[0].CadirKodu);
    }

    [Fact]
    public void TakipEtkileriCadirSeciminiEtkilemez()
    {
        var oneriler = CadirOneriMotoru.OneriUret(2, [YerlesimEtkisi.KoordinatorTakibi, YerlesimEtkisi.GorevliTakibi], [C("A", 1, 2)]);
        Assert.Empty(oneriler[0].KarsilanmayanEtkiler);
        Assert.Empty(oneriler[0].KarsilananEtkiler);
    }

    [Fact]
    public void TekCadirYetmezseAyniBolgedeIkiCadirOnerilir()
    {
        var oneriler = CadirOneriMotoru.OneriUret(9, [], [C("A", 1, 6), C("A", 2, 4), C("B", 1, 8), C("C", 1, 2)]);
        var oneri = Assert.Single(oneriler); // A: 6 + 4 = 10 ≥ 9; B ve C tek başına ve birbirleriyle (farklı bölge) eşleşmez
        Assert.False(oneri.TekCadir);
        Assert.Equal([6, 3], oneri.Atamalar.Select(a => a.KisiSayisi));
        Assert.Equal(1, oneri.BosYatakIsrafi);
    }

    [Fact]
    public void HicYerYoksaOneriBosDoner_KapasiteAsilmaz()
    {
        Assert.Empty(CadirOneriMotoru.OneriUret(5, [], [C("A", 1, 2), C("B", 1, 2), C("A", 2, 4, dolu: 2)]));
    }

    [Fact]
    public void OzetMetniEtkileriVeIsrafiGosterir()
    {
        var oneri = CadirOneriMotoru.OneriUret(3, [YerlesimEtkisi.SicakCadir, YerlesimEtkisi.SaglikNoktasinaYakin],
            [C("A", 2, 4, saglik: true)])[0];
        Assert.Equal("A1-02 (3 kişi) · Sağlık noktasına yakın ✔ · Sıcak çadır ✘ · boş kalacak yatak: 1", oneri.Ozet);
    }
}

public class BeklemeSiralamasiTestleri
{
    private static BekleyenAile B(int id, DestekOnceligi oncelik, int gunOnce, int kisi = 2) => new()
    {
        KayitId = id, AileId = id, AileNo = $"A-{id:000000}", AileAdi = $"Aile {id}", KisiSayisi = kisi,
        DestekOnceligi = oncelik, Gerekceler = "", EklenmeZamani = new DateTime(2026, 10, 16).AddDays(-gunOnce),
    };

    [Fact]
    public void OnceDestekOnceligiSonraEnUzunBekleyen()
    {
        var liste = new[] { B(1, DestekOnceligi.Normal, 10), B(2, DestekOnceligi.Yuksek, 1), B(3, DestekOnceligi.Orta, 5), B(4, DestekOnceligi.Yuksek, 3) };
        Assert.Equal([4, 2, 3, 1], BeklemeSiralamasi.Sirala(liste).Select(b => b.KayitId));
    }

    [Fact]
    public void YalnizcaCadiraSiganAilelerOnerilir()
    {
        var liste = new[] { B(1, DestekOnceligi.Yuksek, 5, kisi: 5), B(2, DestekOnceligi.Normal, 1, kisi: 3), B(3, DestekOnceligi.Orta, 1, kisi: 0) };
        Assert.Equal([2], BeklemeSiralamasi.CadiraUyanlar(liste, bosYatak: 4).Select(b => b.KayitId));
    }
}
