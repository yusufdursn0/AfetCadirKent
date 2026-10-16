using AfetCadirKent.DataAccess.Konfigurasyonlar;
using Microsoft.EntityFrameworkCore;

namespace AfetCadirKent.DataAccess;

public class AfetDbContext(DbContextOptions<AfetDbContext> options) : DbContext(options)
{
    // Kullanıcı ve yetki
    public DbSet<Gorevli> Kullanicilar => Set<Gorevli>();
    public DbSet<Rol> Roller => Set<Rol>();
    public DbSet<Yetki> Yetkiler => Set<Yetki>();

    // Kayıt
    public DbSet<Aile> Aileler => Set<Aile>();
    public DbSet<Afetzede> Kisiler => Set<Afetzede>();
    public DbSet<KisiHareketi> KisiHareketleri => Set<KisiHareketi>();
    public DbSet<AileGecmisi> AileGecmisi => Set<AileGecmisi>();

    // Gerekçe
    public DbSet<Gerekce> Gerekceler => Set<Gerekce>();
    public DbSet<GerekceUrun> GerekceUrunleri => Set<GerekceUrun>();
    public DbSet<AileGerekcesi> AileGerekceleri => Set<AileGerekcesi>();

    // Çadır
    public DbSet<Bolge> Bolgeler => Set<Bolge>();
    public DbSet<Cadir> Cadirlar => Set<Cadir>();
    public DbSet<Yerlesim> Yerlesimler => Set<Yerlesim>();
    public DbSet<BeklemeListesiKaydi> BeklemeListesi => Set<BeklemeListesiKaydi>();

    // Depo ve dağıtım
    public DbSet<Urun> Urunler => Set<Urun>();
    public DbSet<Bagisci> Bagiscilar => Set<Bagisci>();
    public DbSet<Parti> Partiler => Set<Parti>();
    public DbSet<StokHareketi> StokHareketleri => Set<StokHareketi>();
    public DbSet<StokAyirma> StokAyirmalari => Set<StokAyirma>();
    public DbSet<Dagitim> Dagitimlar => Set<Dagitim>();
    public DbSet<DagitimDetay> DagitimDetaylari => Set<DagitimDetay>();

    // Talep, yakın bulma, gönüllü, sistem
    public DbSet<Talep> Talepler => Set<Talep>();
    public DbSet<YakinBulmaBasvurusu> YakinBulmaBasvurulari => Set<YakinBulmaBasvurusu>();
    public DbSet<Gonullu> Gonulluler => Set<Gonullu>();
    public DbSet<Vardiya> Vardiyalar => Set<Vardiya>();
    public DbSet<Duyuru> Duyurular => Set<Duyuru>();
    public DbSet<IslemKaydi> IslemGecmisi => Set<IslemKaydi>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(12, 2);
        configurationBuilder.Properties<string>().HaveMaxLength(500);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AfetDbContext).Assembly);

        // Varsayılan olarak hiçbir ilişki zincirleme silme yapmaz; geçmiş korunur.
        // Yalnızca saf bağlı tablolar (yetki, gerekçe-ürün eşleşmesi) üst kayıtla birlikte silinir.
        foreach (var iliski in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            var bagliTip = iliski.DeclaringEntityType.ClrType;
            iliski.DeleteBehavior = bagliTip == typeof(Yetki) || bagliTip == typeof(GerekceUrun)
                ? DeleteBehavior.Cascade
                : DeleteBehavior.Restrict;
        }

        ReferansVerisi.Ekle(modelBuilder);
    }
}
