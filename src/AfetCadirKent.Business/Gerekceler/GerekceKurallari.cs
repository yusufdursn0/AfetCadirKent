namespace AfetCadirKent.Business.Gerekceler;

// Şartname M3 tablosundaki 10 gerekçe. Her sınıf yalnızca kendi tespit mantığını içerir.

public sealed class BebekKurali() : GerekceKuraliTemel(
    GerekceKodu.Bebek, "0–2 yaş bebek", DestekOnceligi.Yuksek, YerlesimEtkisi.SicakCadir)
{
    public override bool UygulanirMi(AileBaglami b) => b.Uyeler.Any(u => u.BebekMi(b.Bugun));

    public override string Aciklama(AileBaglami b) =>
        SayiliAciklama(b.Uyeler.Count(u => u.BebekMi(b.Bugun)), "0–2 yaş bebek");
}

public sealed class HamileKurali() : GerekceKuraliTemel(
    GerekceKodu.Hamile, "Hamile", DestekOnceligi.Yuksek, YerlesimEtkisi.SaglikNoktasinaYakin)
{
    public override bool UygulanirMi(AileBaglami b) => b.Uyeler.Any(u => u.Hamile);

    public override string Aciklama(AileBaglami b) => SayiliAciklama(b.Uyeler.Count(u => u.Hamile), "hamile birey");
}

public sealed class TekerlekliSandalyeKurali() : GerekceKuraliTemel(
    GerekceKodu.TekerlekliSandalye, "Tekerlekli sandalye", DestekOnceligi.Yuksek, YerlesimEtkisi.ErisilebilirCadir)
{
    public override bool UygulanirMi(AileBaglami b) => b.Uyeler.Any(u => u.TekerlekliSandalye);

    public override string Aciklama(AileBaglami b) =>
        SayiliAciklama(b.Uyeler.Count(u => u.TekerlekliSandalye), "tekerlekli sandalye kullanan birey");
}

public sealed class KronikHastaKurali() : GerekceKuraliTemel(
    GerekceKodu.KronikHasta, "Kronik hasta", DestekOnceligi.Yuksek, YerlesimEtkisi.SaglikNoktasinaYakin)
{
    public override bool UygulanirMi(AileBaglami b) => b.Uyeler.Any(u => u.KronikHasta);

    public override string Aciklama(AileBaglami b) =>
        SayiliAciklama(b.Uyeler.Count(u => u.KronikHasta), "kronik hastalığı olan birey");
}

public sealed class RefakatsizCocukKurali() : GerekceKuraliTemel(
    GerekceKodu.RefakatsizCocuk, "Refakatsiz çocuk", DestekOnceligi.Yuksek, YerlesimEtkisi.KoordinatorTakibi)
{
    public override bool UygulanirMi(AileBaglami b) => b.Uyeler.Any(u => u.RefakatsizCocuk);
}

public sealed class YasliKurali() : GerekceKuraliTemel(
    GerekceKodu.Yasli, "65 yaş üstü", DestekOnceligi.Orta, YerlesimEtkisi.ErisilebilirCadir)
{
    public override bool UygulanirMi(AileBaglami b) => b.Uyeler.Any(u => u.YasliMi(b.Bugun));

    public override string Aciklama(AileBaglami b) =>
        SayiliAciklama(b.Uyeler.Count(u => u.YasliMi(b.Bugun)), "65 yaş üstü birey");
}

public sealed class TekEbeveynKurali() : GerekceKuraliTemel(
    GerekceKodu.TekEbeveyn, "Tek ebeveyn", DestekOnceligi.Orta, YerlesimEtkisi.GorevliTakibi)
{
    public override bool UygulanirMi(AileBaglami b) => b.Aile.TekEbeveyn;
}

public sealed class KalabalikAileKurali() : GerekceKuraliTemel(
    GerekceKodu.KalabalikAile, "5 kişiden kalabalık", DestekOnceligi.Orta, YerlesimEtkisi.BuyukCadir)
{
    public const int KisiSiniri = 5;

    public override bool UygulanirMi(AileBaglami b) => b.Uyeler.Count > KisiSiniri;

    public override string Aciklama(AileBaglami b) => $"{b.Uyeler.Count} kişilik aile";
}

/// <summary>Aile en az 3 gündür kayıtlı ve son 3 günde hiçbir üründe teslim almamış (bkz. K-040).</summary>
public sealed class UcGundurYardimAlmadiKurali() : GerekceKuraliTemel(
    GerekceKodu.UcGundurYardimAlmadi, "3 gündür yardım almadı", DestekOnceligi.Orta, YerlesimEtkisi.Yok,
    tumUrunler: true)
{
    public const int GunSayisi = 3;

    public override bool UygulanirMi(AileBaglami b)
    {
        var sinir = b.Simdi.AddDays(-GunSayisi);
        return b.Aile.KayitZamani <= sinir && (b.SonTeslimZamani is null || b.SonTeslimZamani <= sinir);
    }
}

/// <summary>Başka hiçbir gerekçe yoksa kullanılan varsayılan gerekçe (değerlendirici en sona bırakır).</summary>
public sealed class OzelDurumYokKurali() : GerekceKuraliTemel(
    GerekceKodu.OzelDurumYok, "Özel durumu yok", DestekOnceligi.Normal, YerlesimEtkisi.Yok)
{
    public override bool UygulanirMi(AileBaglami b) => true;
}
