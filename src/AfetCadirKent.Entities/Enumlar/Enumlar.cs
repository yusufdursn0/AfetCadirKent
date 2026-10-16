namespace AfetCadirKent.Entities.Enumlar;

// Enum değerleri veritabanında int olarak saklanır; ekranda Türkçe adları gösterilir.

public enum KisiDurumu
{
    Burada = 0,
    Ayrildi = 1,
    Nakil = 2,
    Hastanede = 3,
}

public enum Cinsiyet
{
    Belirtilmemis = 0,
    Kadin = 1,
    Erkek = 2,
}

public enum KisiHareketTuru
{
    Gelis = 0,
    Ayrilis = 1,
    Nakil = 2,
    CadirDegisikligi = 3,
    HastaneyeSevk = 4,
    HastanedenDonus = 5,
    AileDegisikligi = 6,
}

public enum AileGecmisiTuru
{
    Ayirma = 0,
    Birlesme = 1,
}

/// <summary>Ailenin destek (yardım) önceliği. Tıbbi değerlendirme değildir.</summary>
public enum DestekOnceligi
{
    Normal = 0,
    Orta = 1,
    Yuksek = 2,
}

public enum YerlesimEtkisi
{
    Yok = 0,
    SicakCadir = 1,
    SaglikNoktasinaYakin = 2,
    ErisilebilirCadir = 3,
    KoordinatorTakibi = 4,
    GorevliTakibi = 5,
    BuyukCadir = 6,
}

public enum GerekceKodu
{
    Bebek = 1,
    Hamile = 2,
    TekerlekliSandalye = 3,
    KronikHasta = 4,
    RefakatsizCocuk = 5,
    Yasli = 6,
    TekEbeveyn = 7,
    KalabalikAile = 8,
    UcGundurYardimAlmadi = 9,
    OzelDurumYok = 10,
}

/// <summary>Görevlinin bir gerekçeyi elle eklemesi veya otomatik gerekçeyi kaldırması.</summary>
public enum GerekceDegisiklikTuru
{
    ElleEklendi = 0,
    ElleKaldirildi = 1,
}

public enum CadirDurumu
{
    Kullanilabilir = 0,
    Bakimda = 1,
    Kapali = 2,
}

public enum Birim
{
    Adet = 0,
    Paket = 1,
    Kg = 2,
    Litre = 3,
}

public enum HesaplamaTuru
{
    KisiBasi = 0,
    AileBasi = 1,
    BebekBasi = 2,
}

public enum UrunKategorisi
{
    Su = 0,
    Gida = 1,
    Mama = 2,
    Bez = 3,
    Hijyen = 4,
    Battaniye = 5,
    Giyim = 6,
    Diger = 7,
}

public enum StokHareketTuru
{
    Giris = 0,
    Dagitim = 1,
    Iade = 2,
    Hasar = 3,
    SayimFarki = 4,
}

public enum AyirmaDurumu
{
    Acik = 0,
    Kapandi = 1,
}

public enum DagitimDurumu
{
    Taslak = 0,
    Onaylandi = 1,
    Kapandi = 2,
    Iptal = 3,
}

public enum TalepTuru
{
    Saglik = 0,
    Malzeme = 1,
    CadirAriza = 2,
    Diger = 3,
}

public enum TalepDurumu
{
    Yeni = 0,
    Atandi = 1,
    Islemde = 2,
    Tamamlandi = 3,
    Iptal = 4,
}

public enum TalepKaynagi
{
    KayitMasasi = 0,
    Gorevli = 1,
    Kiosk = 2,
}

public enum SiniflandirmaDurumu
{
    Siniflandirilmamis = 0,
    YapayZekaOnerdi = 1,
    GorevliBelirledi = 2,
}

public enum YakinBulmaDurumu
{
    Acik = 0,
    OlasiEslesme = 1,
    Dogrulandi = 2,
    Kapandi = 3,
}

public enum RolTuru
{
    Koordinator = 1,
    KayitGorevlisi = 2,
    DepoSorumlusu = 3,
    SaglikPersoneli = 4,
    SahaGorevlisi = 5,
    Kiosk = 6,
}

public enum Modul
{
    Kayit = 1,
    Cadir = 2,
    Depo = 3,
    Dagitim = 4,
    Talep = 5,
    SaglikBilgisi = 6,
    YakinBulma = 7,
    Gonullu = 8,
    Duyuru = 9,
    KullaniciYonetimi = 10,
    Ayarlar = 11,
    IslemGecmisi = 12,
    Kiosk = 13,
}

/// <summary>Görme, değiştirme ve rapor alma ayrı izinlerdir.</summary>
public enum Izin
{
    Gorme = 1,
    Degistirme = 2,
    Rapor = 3,
}

public enum IslemTuru
{
    Olusturma = 0,
    Guncelleme = 1,
    Iptal = 2,
    Silme = 3,
    PasifYapma = 4,
    Giris = 5,
    Diger = 6,
}
