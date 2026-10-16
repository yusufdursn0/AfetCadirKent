using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.DataAccess.Depolar;

/// <summary>Tüm entity'ler için ortak veri erişim sözleşmesi (generic repository).</summary>
public interface IRepository<T> where T : class
{
    /// <summary>Filtreleme, sıralama ve Include için sorgulanabilir kaynak.</summary>
    IQueryable<T> Sorgu();

    Task<T?> BulAsync(int id, CancellationToken iptal = default);

    Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null, CancellationToken iptal = default);

    Task<bool> VarMiAsync(Expression<Func<T, bool>> filtre, CancellationToken iptal = default);

    void Ekle(T varlik);

    void Guncelle(T varlik);

    /// <summary>
    /// Fiziksel silme. <see cref="IFizikselSilinemez"/> kayıtlar (kişi, aile, dağıtım, stok hareketi)
    /// silinemez; bunlar pasif yapılır.
    /// </summary>
    void Sil(T varlik);
}

public class Repository<T>(AfetDbContext context) : IRepository<T> where T : class
{
    protected readonly AfetDbContext Context = context;
    protected DbSet<T> Kume => Context.Set<T>();

    public IQueryable<T> Sorgu() => Kume;

    public async Task<T?> BulAsync(int id, CancellationToken iptal = default) =>
        await Kume.FindAsync([id], iptal);

    public Task<List<T>> ListeleAsync(Expression<Func<T, bool>>? filtre = null, CancellationToken iptal = default) =>
        (filtre is null ? Kume : Kume.Where(filtre)).ToListAsync(iptal);

    public Task<bool> VarMiAsync(Expression<Func<T, bool>> filtre, CancellationToken iptal = default) =>
        Kume.AnyAsync(filtre, iptal);

    public void Ekle(T varlik) => Kume.Add(varlik);

    public void Guncelle(T varlik) => Kume.Update(varlik);

    public void Sil(T varlik)
    {
        if (varlik is IFizikselSilinemez)
            throw new InvalidOperationException(
                $"{typeof(T).Name} kayıtları fiziksel olarak silinemez; pasif yapılmalıdır.");
        Kume.Remove(varlik);
    }
}
