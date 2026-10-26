using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.DataAccess.Tohumlama;

/// <summary>
/// Uygulama açılışında veritabanını hazırlar: bekleyen migration'ları uygular, veritabanı boşsa demo verisini yükler.
/// Böylece kurulum tek adımda (uygulamayı başlatmak) tamamlanır.
/// </summary>
public static class VeritabaniHazirlayici
{
    public static async Task HazirlaAsync(AfetDbContext db, DateTime simdi, CancellationToken iptal = default)
    {
        await db.Database.MigrateAsync(iptal);

        if (!await db.Bolgeler.AnyAsync(iptal))
            await DemoVeriYukleyici.YukleAsync(db, simdi, iptal);
    }

    public static async Task HazirlaAsync(IDbContextFactory<AfetDbContext> fabrika, DateTime simdi, CancellationToken iptal = default)
    {
        await using var db = await fabrika.CreateDbContextAsync(iptal);
        await HazirlaAsync(db, simdi, iptal);
    }
}
