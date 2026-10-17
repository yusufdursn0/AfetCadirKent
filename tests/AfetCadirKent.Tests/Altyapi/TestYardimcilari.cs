using AfetCadirKent.Business.CadirYonetimi;
using AfetCadirKent.Business.Gerekceler;
using AfetCadirKent.Business.Kayit;
using AfetCadirKent.Business.Ortak;
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

    public static IVeriOturumuFabrikasi OturumFabrikasi(this VeritabaniFixture fixture) => new TestVeriFabrikasi(fixture);

    public static KayitServisi KayitServisi(this VeritabaniFixture fixture, RolTuru rol) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), TimeProvider.System);

    public static GerekceServisi GerekceServisi(this VeritabaniFixture fixture, RolTuru rol) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), TimeProvider.System, GerekceDegerlendirici.Varsayilan());

    public static CadirServisi CadirServisi(this VeritabaniFixture fixture, RolTuru rol) =>
        new(fixture.OturumFabrikasi(), OturumAc(rol), TimeProvider.System, GerekceDegerlendirici.Varsayilan());

    public static BeklemeListesiServisi BeklemeListesiServisi(this VeritabaniFixture fixture, RolTuru rol)
    {
        var oturum = OturumAc(rol);
        var cadir = new CadirServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, GerekceDegerlendirici.Varsayilan());
        return new BeklemeListesiServisi(fixture.OturumFabrikasi(), oturum, TimeProvider.System, GerekceDegerlendirici.Varsayilan(), cadir);
    }

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

    private sealed class TestVeriFabrikasi(VeritabaniFixture fixture) : IVeriOturumuFabrikasi
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
