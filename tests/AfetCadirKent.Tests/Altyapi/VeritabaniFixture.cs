using AfetCadirKent.DataAccess;
using AfetCadirKent.DataAccess.Tohumlama;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Tests.Altyapi;

/// <summary>
/// Ayrı LocalDB test veritabanı. Geliştirme veritabanına dokunulmaz.
/// Test oturumu başında silinip migration + demo verisiyle yeniden oluşturulur (uygulamanın ilk açılışıyla aynı yol).
/// Sunucu AFETCADIRKENT_TEST_SUNUCU ortam değişkeniyle değiştirilebilir (varsayılan: LocalDB, Windows kimlik doğrulaması).
/// </summary>
public abstract class TestVeritabaniTemel(string veritabaniAdi) : IAsyncLifetime
{
    public static readonly DateTime TohumlamaZamani = DateTime.Now;

    public string VeritabaniAdi { get; } = veritabaniAdi;

    public string BaglantiDizesi { get; } = new SqlConnectionStringBuilder
    {
        DataSource = Environment.GetEnvironmentVariable("AFETCADIRKENT_TEST_SUNUCU") ?? @"(localdb)\MSSQLLocalDB",
        InitialCatalog = veritabaniAdi,
        IntegratedSecurity = true,
        TrustServerCertificate = true,
    }.ConnectionString;

    public AfetDbContext YeniContext() =>
        new(new DbContextOptionsBuilder<AfetDbContext>().UseSqlServer(BaglantiDizesi).Options);

    public async Task InitializeAsync()
    {
        await using var db = YeniContext();
        await db.Database.EnsureDeletedAsync();
        await VeritabaniHazirlayici.HazirlaAsync(db, TohumlamaZamani);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

/// <summary>Testlerin çoğunun paylaştığı veritabanı (AfetCadirKent_Test). Testler kendi verilerini benzersiz adlarla oluşturur.</summary>
public sealed class VeritabaniFixture() : TestVeritabaniTemel(Ad)
{
    public const string Ad = "AfetCadirKent_Test";
}

/// <summary>
/// İlk kurulum testlerinin veritabanı (AfetCadirKent_Test_Kurulum): "hiç kullanıcı yok" durumu ancak
/// başka testlerin kullanıcı eklemediği ayrı bir veritabanında sınanabilir.
/// </summary>
public sealed class KurulumVeritabaniFixture() : TestVeritabaniTemel(Ad)
{
    public const string Ad = "AfetCadirKent_Test_Kurulum";
}

[CollectionDefinition(Ad)]
public class VeritabaniKoleksiyonu : ICollectionFixture<VeritabaniFixture>
{
    public const string Ad = "Veritabani";
}

[CollectionDefinition(Ad)]
public class KurulumKoleksiyonu : ICollectionFixture<KurulumVeritabaniFixture>
{
    public const string Ad = "Kurulum";
}
