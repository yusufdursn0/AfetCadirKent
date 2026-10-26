using AfetCadirKent.DataAccess.Depolar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AfetCadirKent.DataAccess;

/// <summary>Veri erişim katmanının bağımlılık kayıtları (uygulama açılışında UI çağırır).</summary>
public static class VeriErisimiKayitlari
{
    /// <param name="baglantiDizesi">appsettings.json → ConnectionStrings:AfetDb. Kodda bağlantı dizesi tutulmaz.</param>
    public static IServiceCollection VeriErisiminiEkle(this IServiceCollection servisler, string baglantiDizesi)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baglantiDizesi);
        // Tek sorgu, EF Core'un varsayılanıdır; açıkça belirtilerek her sorguda yazılan uyarı kaldırılır.
        servisler.AddDbContextFactory<AfetDbContext>(o =>
            o.UseSqlServer(baglantiDizesi, s => s.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery)));
        servisler.AddSingleton<IVeriOturumuFabrikasi, VeriOturumuFabrikasi>();
        return servisler;
    }
}
