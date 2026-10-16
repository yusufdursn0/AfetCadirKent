using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AfetCadirKent.DataAccess.Depolar;

/// <summary>
/// Tek bir iş işleminin veri oturumu (unit of work): aynı DbContext'i paylaşan repository'ler,
/// kaydetme ve transaction. Her iş işlemi için yeni bir oturum açılır ve dispose edilir.
/// </summary>
public interface IVeriOturumu : IAsyncDisposable, IDisposable
{
    IRepository<T> Repo<T>() where T : class;

    Task<int> KaydetAsync(CancellationToken iptal = default);

    Task<IDbContextTransaction> IslemBaslatAsync(CancellationToken iptal = default);
}

public sealed class VeriOturumu(AfetDbContext context) : IVeriOturumu
{
    private readonly Dictionary<Type, object> _repolar = [];

    public AfetDbContext Context { get; } = context;

    public IRepository<T> Repo<T>() where T : class
    {
        if (!_repolar.TryGetValue(typeof(T), out var repo))
        {
            repo = new Repository<T>(Context);
            _repolar[typeof(T)] = repo;
        }

        return (IRepository<T>)repo;
    }

    public Task<int> KaydetAsync(CancellationToken iptal = default) => Context.SaveChangesAsync(iptal);

    public Task<IDbContextTransaction> IslemBaslatAsync(CancellationToken iptal = default) =>
        Context.Database.BeginTransactionAsync(iptal);

    public void Dispose() => Context.Dispose();

    public ValueTask DisposeAsync() => Context.DisposeAsync();
}

/// <summary>Business servislerinin yeni veri oturumu açmak için kullandığı fabrika.</summary>
public interface IVeriOturumuFabrikasi
{
    IVeriOturumu Ac();
}

public class VeriOturumuFabrikasi(IDbContextFactory<AfetDbContext> contextFabrikasi) : IVeriOturumuFabrikasi
{
    public IVeriOturumu Ac() => new VeriOturumu(contextFabrikasi.CreateDbContext());
}
