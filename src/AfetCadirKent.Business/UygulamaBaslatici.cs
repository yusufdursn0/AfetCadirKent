using AfetCadirKent.Business.TalepYonetimi;
using AfetCadirKent.DataAccess.Tohumlama;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AfetCadirKent.Business;

/// <summary>Uygulama açılış işleri: veritabanı hazırlığı ve arka plan talep sınıflandırıcısı (K-060, K-080).</summary>
public interface IUygulamaBaslatici
{
    /// <summary>Bekleyen migration'ları uygular; veritabanı boşsa demo verisini yükler.</summary>
    Task VeritabaniniHazirlaAsync(CancellationToken iptal = default);

    /// <summary>
    /// Daha önce sınıflandırılamamış açık talepleri sıraya alır ve sınıflandırıcıyı uygulama kapanana kadar çalıştırır.
    /// Uygulama kapanışında (iptal) normal biçimde biter; diğer hatalar loglanır, uygulamayı durdurmaz.
    /// </summary>
    Task ArkaPlanIsleriniCalistirAsync(CancellationToken kapanis);
}

public class UygulamaBaslatici(IDbContextFactory<AfetDbContext> contextFabrikasi, TimeProvider zaman,
    OtomatikSiniflandirici siniflandirici, ILogger<UygulamaBaslatici>? log = null) : IUygulamaBaslatici
{
    private readonly ILogger _log = log ?? NullLogger<UygulamaBaslatici>.Instance;

    public Task VeritabaniniHazirlaAsync(CancellationToken iptal = default) =>
        VeritabaniHazirlayici.HazirlaAsync(contextFabrikasi, zaman.Simdi(), iptal);

    public async Task ArkaPlanIsleriniCalistirAsync(CancellationToken kapanis)
    {
        try
        {
            var sayi = await siniflandirici.BekleyenleriEkleAsync(kapanis);
            if (sayi > 0)
                _log.LogInformation("{Sayi} sınıflandırılmamış talep yapay zeka sırasına alındı", sayi);
            await siniflandirici.CalistirAsync(kapanis);
        }
        catch (OperationCanceledException) when (kapanis.IsCancellationRequested)
        {
            // Uygulama kapanıyor.
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Arka plan talep sınıflandırıcısı durdu");
        }
    }
}
