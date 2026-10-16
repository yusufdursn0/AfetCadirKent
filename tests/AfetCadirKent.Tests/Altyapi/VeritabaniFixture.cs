using AfetCadirKent.DataAccess;
using AfetCadirKent.DataAccess.Tohumlama;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.Tests.Altyapi;

/// <summary>
/// Ayrı LocalDB test veritabanı (AfetCadirKent_Test). Geliştirme veritabanına dokunulmaz.
/// Test oturumu başında silinip migration + demo verisiyle yeniden oluşturulur.
/// Sunucu AFETCADIRKENT_TEST_SUNUCU ortam değişkeniyle değiştirilebilir (varsayılan: LocalDB, Windows kimlik doğrulaması).
/// </summary>
public sealed class VeritabaniFixture : IAsyncLifetime
{
    public const string VeritabaniAdi = "AfetCadirKent_Test";

    public static readonly DateTime TohumlamaZamani = DateTime.Now;

    public string BaglantiDizesi { get; } = new SqlConnectionStringBuilder
    {
        DataSource = Environment.GetEnvironmentVariable("AFETCADIRKENT_TEST_SUNUCU") ?? @"(localdb)\MSSQLLocalDB",
        InitialCatalog = VeritabaniAdi,
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

[CollectionDefinition(Ad)]
public class VeritabaniKoleksiyonu : ICollectionFixture<VeritabaniFixture>
{
    public const string Ad = "Veritabani";
}
