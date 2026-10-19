using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.DataAccess.Konfigurasyonlar;

/// <summary>
/// Migration ile gelen sabit referans verisi: roller, rol yetkileri ve gerekçe tanımları.
/// Kullanıcı ve parola İÇERMEZ; ilk yönetici uygulama ilk açıldığında oluşturulur.
/// </summary>
public static class ReferansVerisi
{
    private static readonly Izin[] TumIzinler = [Izin.Gorme, Izin.Degistirme, Izin.Rapor];
    private static readonly Izin[] GorDegistir = [Izin.Gorme, Izin.Degistirme];
    private static readonly Izin[] Gor = [Izin.Gorme];

    public static readonly IReadOnlyList<Rol> Roller =
    [
        new() { Id = (int)RolTuru.Koordinator, Tur = RolTuru.Koordinator, Ad = "Koordinatör", Aciklama = "Her şey ve rapor; gönüllü yönetimi ve duyurular" },
        new() { Id = (int)RolTuru.KayitGorevlisi, Tur = RolTuru.KayitGorevlisi, Ad = "Kayıt görevlisi", Aciklama = "Kayıt ve çadır" },
        new() { Id = (int)RolTuru.DepoSorumlusu, Tur = RolTuru.DepoSorumlusu, Ad = "Depo sorumlusu", Aciklama = "Depo, dağıtım ve rapor" },
        new() { Id = (int)RolTuru.SaglikPersoneli, Tur = RolTuru.SaglikPersoneli, Ad = "Sağlık personeli", Aciklama = "Sağlık bilgileri ve sağlık talepleri" },
        new() { Id = (int)RolTuru.SahaGorevlisi, Tur = RolTuru.SahaGorevlisi, Ad = "Saha görevlisi", Aciklama = "Kendine atanan talepler" },
        new() { Id = (int)RolTuru.Kiosk, Tur = RolTuru.Kiosk, Ad = "Kiosk", Aciklama = "Kendi ailesi ve talep oluşturma" },
    ];

    /// <summary>Rol → modül → izinler. Ayrıntılı gerekçe: PROJECT_DECISIONS K-059.</summary>
    public static readonly IReadOnlyDictionary<RolTuru, Dictionary<Modul, Izin[]>> YetkiMatrisi =
        new Dictionary<RolTuru, Dictionary<Modul, Izin[]>>
        {
            [RolTuru.Koordinator] = Enum.GetValues<Modul>().ToDictionary(m => m, _ => TumIzinler),
            [RolTuru.KayitGorevlisi] = new()
            {
                [Modul.Kayit] = TumIzinler,
                [Modul.Cadir] = TumIzinler,
                [Modul.Talep] = GorDegistir,
                [Modul.YakinBulma] = GorDegistir,
                [Modul.SaglikBilgisi] = GorDegistir,
            },
            [RolTuru.DepoSorumlusu] = new()
            {
                [Modul.Depo] = TumIzinler,
                [Modul.Dagitim] = TumIzinler,
                [Modul.Kayit] = Gor,
                [Modul.Talep] = GorDegistir,
                [Modul.SaglikBilgisi] = Gor,
            },
            [RolTuru.SaglikPersoneli] = new()
            {
                [Modul.SaglikBilgisi] = GorDegistir,
                [Modul.Talep] = GorDegistir,
                [Modul.Kayit] = Gor,
            },
            [RolTuru.SahaGorevlisi] = new()
            {
                [Modul.Talep] = GorDegistir,
                [Modul.Cadir] = Gor,
                [Modul.SaglikBilgisi] = GorDegistir,
            },
            [RolTuru.Kiosk] = new()
            {
                [Modul.Kiosk] = GorDegistir,
            },
        };

    /// <summary>
    /// Id rol, modül ve izinden türetilir (rol·1000 + modül·10 + izin); matris değişince
    /// diğer satırların Id'si kaymaz, migration yalnızca eklenen/çıkan satırları içerir.
    /// </summary>
    public static IReadOnlyList<Yetki> Yetkiler()
    {
        var liste = new List<Yetki>();
        foreach (var (rol, moduller) in YetkiMatrisi.OrderBy(x => x.Key))
            foreach (var (modul, izinler) in moduller.OrderBy(x => x.Key))
                foreach (var izin in izinler)
                    liste.Add(new Yetki
                    {
                        Id = (int)rol * 1000 + (int)modul * 10 + (int)izin,
                        RolId = (int)rol, Modul = modul, Izin = izin,
                    });
        return liste;
    }

    /// <summary>Şartname M3 tablosu.</summary>
    public static readonly IReadOnlyList<Gerekce> Gerekceler =
    [
        G(GerekceKodu.Bebek, "0–2 yaş bebek", DestekOnceligi.Yuksek, YerlesimEtkisi.SicakCadir),
        G(GerekceKodu.Hamile, "Hamile", DestekOnceligi.Yuksek, YerlesimEtkisi.SaglikNoktasinaYakin),
        G(GerekceKodu.TekerlekliSandalye, "Tekerlekli sandalye", DestekOnceligi.Yuksek, YerlesimEtkisi.ErisilebilirCadir),
        G(GerekceKodu.KronikHasta, "Kronik hasta", DestekOnceligi.Yuksek, YerlesimEtkisi.SaglikNoktasinaYakin),
        G(GerekceKodu.RefakatsizCocuk, "Refakatsiz çocuk", DestekOnceligi.Yuksek, YerlesimEtkisi.KoordinatorTakibi),
        G(GerekceKodu.Yasli, "65 yaş üstü", DestekOnceligi.Orta, YerlesimEtkisi.ErisilebilirCadir),
        G(GerekceKodu.TekEbeveyn, "Tek ebeveyn", DestekOnceligi.Orta, YerlesimEtkisi.GorevliTakibi),
        G(GerekceKodu.KalabalikAile, "5 kişiden kalabalık", DestekOnceligi.Orta, YerlesimEtkisi.BuyukCadir),
        G(GerekceKodu.UcGundurYardimAlmadi, "3 gündür yardım almadı", DestekOnceligi.Orta, YerlesimEtkisi.Yok, tumUrunler: true),
        G(GerekceKodu.OzelDurumYok, "Özel durumu yok", DestekOnceligi.Normal, YerlesimEtkisi.Yok),
    ];

    /// <summary>Gerekçe → öncelikli ürün kategorileri (GerekceUrunleri demo verisinde bu eşlemeyle doldurulur).</summary>
    public static readonly IReadOnlyDictionary<GerekceKodu, UrunKategorisi[]> OncelikliKategoriler =
        new Dictionary<GerekceKodu, UrunKategorisi[]>
        {
            [GerekceKodu.Bebek] = [UrunKategorisi.Mama, UrunKategorisi.Bez, UrunKategorisi.Battaniye],
            [GerekceKodu.Hamile] = [UrunKategorisi.Gida, UrunKategorisi.Hijyen],
            [GerekceKodu.RefakatsizCocuk] = [UrunKategorisi.Gida, UrunKategorisi.Giyim],
            [GerekceKodu.Yasli] = [UrunKategorisi.Battaniye],
            [GerekceKodu.TekEbeveyn] = [UrunKategorisi.Hijyen],
            [GerekceKodu.KalabalikAile] = [UrunKategorisi.Gida],
        };

    internal static void Ekle(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rol>().HasData(Roller);
        modelBuilder.Entity<Yetki>().HasData(Yetkiler());
        modelBuilder.Entity<Gerekce>().HasData(Gerekceler);
    }

    private static Gerekce G(GerekceKodu kod, string ad, DestekOnceligi oncelik, YerlesimEtkisi etki, bool tumUrunler = false) =>
        new() { Id = (int)kod, Kod = kod, Ad = ad, DestekOnceligi = oncelik, YerlesimEtkisi = etki, TumUrunler = tumUrunler };
}
