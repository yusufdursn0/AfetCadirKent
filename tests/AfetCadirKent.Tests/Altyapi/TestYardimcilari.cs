using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.DagitimYonetimi;
using AfetCadirKent.Business.DepoYonetimi;
using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Denetim;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.KullaniciYonetimi;
using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.Business.Ortak;
using AfetCadirKent.Business.YapayZeka;
using AfetCadirKent.DataAccess.Depolar;
using AfetCadirKent.DataAccess.Konfigurasyonlar;
using AfetCadirKent.Entities.Enumlar;
using AfetCadirKent.Entities.Kisiler;
using AfetCadirKent.Entities.Sistem;

namespace AfetCadirKent.Tests.Altyapi;

/// <summary>Testler için rol bazlı oturum ve servis üretimi.</summary>
public static class TestYardimcilari
{
    /// <summary>Verilen rolün yetki matrisindeki izinleriyle açılmış oturum (kullanıcı veritabanına yazılmaz).</summary>
    public static Oturum OturumAc(RolTuru rol)
    {
        var yetkiler = ReferansVerisi.YetkiMatrisi[rol]
            .SelectMany(m => m.Value.Select(i => new Yetki { RolId = (int)rol, Modul = m.Key, Izin = i }));
        var oturum = new Oturum();
        oturum.Ac(new Gorevli { Ad = "Test", Soyad = rol.ToString(), KullaniciAdi = $"test_{rol}".ToLowerInvariant() }, rol, yetkiler);
        return oturum;
    }

    public static IVeriOturumuFabrikasi OturumFabrikasi(this TestVeritabaniTemel fixture) => new TestVeriFabrikasi(fixture);

    public static KayitServisi KayitServisi(this TestVeritabaniTemel fixture, RolTuru rol) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), TimeProvider.System);

    public static GerekceServisi GerekceServisi(this TestVeritabaniTemel fixture, RolTuru rol) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), TimeProvider.System, GerekceDegerlendirici.Varsayilan());

    public static CadirServisi CadirServisi(this TestVeritabaniTemel fixture, RolTuru rol) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), TimeProvider.System, GerekceDegerlendirici.Varsayilan());

    public static BeklemeListesiServisi BeklemeListesiServisi(this TestVeritabaniTemel fixture, RolTuru rol)
    {
        var oturum = OturumAc(rol);
        var cadir = new CadirServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, GerekceDegerlendirici.Varsayilan());
        return new BeklemeListesiServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, GerekceDegerlendirici.Varsayilan(), cadir);
    }

    public static UrunServisi UrunServisi(this TestVeritabaniTemel fixture, RolTuru rol = RolTuru.DepoSorumlusu,
        TimeProvider? zaman = null) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), zaman ?? TimeProvider.System);

    public static StokServisi StokServisi(this TestVeritabaniTemel fixture, RolTuru rol = RolTuru.DepoSorumlusu,
        TimeProvider? zaman = null) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), zaman ?? TimeProvider.System, new DepoAyarlari());

    public static DagitimServisi DagitimServisi(this TestVeritabaniTemel fixture, RolTuru rol = RolTuru.DepoSorumlusu,
        TimeProvider? zaman = null) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), zaman ?? TimeProvider.System, GerekceDegerlendirici.Varsayilan());

    /// <summary>
    /// Veritabanına kaydedilmiş bir görevliyle açılmış oturum. Kendi açtığı/kendine atanan talepler, teslim eden
    /// gibi kullanıcıya bağlı kurallar kayıtlı kullanıcı Id'si gerektirir.
    /// </summary>
    public static async Task<Oturum> KullaniciOturumuAsync(this TestVeritabaniTemel fixture, RolTuru rol)
    {
        var gorevli = new Gorevli
        {
            Ad = "Test", Soyad = rol.ToString(), KullaniciAdi = BenzersizSoyad(rol.ToString()[..4]).ToLowerInvariant(),
            RolId = (int)rol, OlusturmaZamani = DateTime.Now,
        };
        gorevli.ParolaHashAta("test-ozeti"); // bu kullanıcıyla parola girişi yapılmaz
        await using (var db = fixture.YeniContext())
        {
            db.Kullanicilar.Add(gorevli);
            await db.SaveChangesAsync();
        }

        var oturum = new Oturum();
        oturum.Ac(gorevli, rol, ReferansVerisi.YetkiMatrisi[rol]
            .SelectMany(m => m.Value.Select(i => new Yetki { RolId = (int)rol, Modul = m.Key, Izin = i })));
        return oturum;
    }

    /// <summary>Yapay zeka verilmezse Ollama kapalıymış gibi davranan sahte servis kullanılır.</summary>
    public static TalepServisi TalepServisi(this TestVeritabaniTemel fixture, IOturum oturum, TimeProvider? zaman = null,
        IYapayZekaServisi? yapayZeka = null, ITalepSiniflandirmaKuyrugu? kuyruk = null) =>
        new(fixture.OturumFabrikasi(), oturum, zaman ?? TimeProvider.System, yapayZeka ?? new SahteYapayZeka(), kuyruk ?? new SahteKuyruk());

    public static KullaniciServisi KullaniciServisi(this TestVeritabaniTemel fixture, IOturum oturum) =>
        new(fixture.OturumFabrikasi(), oturum, TimeProvider.System);

    public static OturumServisi OturumServisi(this TestVeritabaniTemel fixture, Oturum oturum) =>
        new(fixture.OturumFabrikasi(), oturum, TimeProvider.System);

    public static KurulumServisi KurulumServisi(this TestVeritabaniTemel fixture) =>
        new(fixture.OturumFabrikasi(), new Oturum(), TimeProvider.System);

    public static IslemGecmisiServisi IslemGecmisiServisi(this TestVeritabaniTemel fixture, IOturum oturum) =>
        new(fixture.OturumFabrikasi(), oturum);

    /// <summary>Kurala uygun yeni kullanıcı bilgisi (benzersiz kullanıcı adı).</summary>
    public static YeniKullaniciBilgisi YeniKullanici(string parola = "Guvenli123", string? kullaniciAdi = null) => new()
    {
        Ad = "Ayşe", Soyad = "Görevli", KullaniciAdi = kullaniciAdi ?? BenzersizSoyad("kul").ToLowerInvariant(),
        Parola = parola, ParolaTekrar = parola,
    };

    /// <summary>Testlerin eklediği ürünlerin ad öneki (demo verisi testleri bunları saymaz).</summary>
    public const string TestUrunOnEki = "TestUrun";

    /// <summary>Testlere özel (adı benzersiz) ürün.</summary>
    public static Task<global::AfetCadirKent.Entities.Depo.Urun> TestUrunuAsync(this TestVeritabaniTemel fixture, HesaplamaTuru tur = HesaplamaTuru.AileBasi,
        decimal hak = 1, int tekrar = 1, decimal kritik = 0, UrunKategorisi kategori = UrunKategorisi.Diger) =>
        fixture.UrunServisi(RolTuru.Koordinator).UrunKaydetAsync(null, new UrunBilgisi
        {
            Ad = BenzersizSoyad(TestUrunOnEki), Kategori = kategori, Birim = Birim.Adet, HesaplamaTuru = tur,
            HakMiktari = hak, TekrarAlmaGunu = tekrar, KritikSeviye = kritik,
        });

    /// <summary>Testler arasında çakışmayan 5 harfli bölge kodu.</summary>
    public static string BenzersizBolgeKodu() =>
        "T" + string.Concat(Guid.NewGuid().ToString("N")[..4].Select(c => (char)('A' + c % 26)));

    /// <summary>Testlerde tekrar etmeyen geçerli TC üretir.</summary>
    public static string YeniTc()
    {
        var taban = Random.Shared.Next(100_000_000, 899_999_999);
        return global::AfetCadirKent.Entities.Ortak.TcKimlik.KontrolHaneleriyleTamamla(taban.ToString());
    }

    /// <summary>Testler arasında çakışmasın diye benzersiz soyad (yalnızca harf).</summary>
    public static string BenzersizSoyad(string onEk = "Test") =>
        onEk + string.Concat(Guid.NewGuid().ToString("N")[..8].Select(c => (char)('a' + c % 26)));

    public static KisiBilgisi Kisi(string ad, string soyad, int yas, string? tc = null, bool hamile = false,
        bool refakatsiz = false, string? saglikNotu = null) => new()
    {
        Ad = ad,
        Soyad = soyad,
        DogumTarihi = DateOnly.FromDateTime(DateTime.Today).AddYears(-yas).AddDays(-10),
        TcKimlikNo = tc,
        Hamile = hamile,
        RefakatsizCocuk = refakatsiz,
        SaglikNotu = saglikNotu,
        Memleket = "Hatay",
    };

    private sealed class TestVeriFabrikasi(TestVeritabaniTemel fixture) : IVeriOturumuFabrikasi
    {
        public IVeriOturumu Ac() => new VeriOturumu(fixture.YeniContext());
    }
}

/// <summary>Sabit zaman (kural testleri için).</summary>
public sealed class SabitZaman(DateTime simdi) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => new DateTimeOffset(simdi).ToUniversalTime();
    public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Local;
}
