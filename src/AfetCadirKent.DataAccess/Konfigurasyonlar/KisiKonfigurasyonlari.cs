using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfetCadirKent.DataAccess.Konfigurasyonlar;

/// <summary>Kisi soyut sınıfından gelen ortak alanların eşlemesi.</summary>
internal static class KisiEslemesi
{
    public static void OrtakAlanlar<T>(EntityTypeBuilder<T> b) where T : Kisi
    {
        b.Property(k => k.Ad).HasMaxLength(50).IsRequired();
        b.Property(k => k.Soyad).HasMaxLength(50).IsRequired();
        b.Property(k => k.Telefon).HasMaxLength(20);
    }
}

internal class AfetzedeKonfigurasyonu : IEntityTypeConfiguration<Afetzede>
{
    public void Configure(EntityTypeBuilder<Afetzede> b)
    {
        b.ToTable("Kisiler");
        KisiEslemesi.OrtakAlanlar(b);

        // Kişi numarası Id'den deterministik olarak hesaplanır: K-000001
        b.Property(k => k.KisiNo)
            .HasMaxLength(12)
            .HasComputedColumnSql("'K-' + RIGHT('000000' + CAST([Id] AS varchar(10)), 6)", stored: true);
        b.HasIndex(k => k.KisiNo).IsUnique();

        b.Property(k => k.TcKimlikNo).HasMaxLength(11).IsFixedLength().IsUnicode(false);
        b.HasIndex(k => k.TcKimlikNo).IsUnique().HasFilter("[TcKimlikNo] IS NOT NULL");

        b.Property(k => k.Memleket).HasMaxLength(50);
        b.Property(k => k.Yakinlik).HasMaxLength(30);
        b.Property(k => k.SaglikNotu).HasMaxLength(1000);
        b.Property(k => k.Fotograf).HasColumnType("varbinary(max)");
        b.Property(k => k.AramaAdi).HasMaxLength(101).IsRequired();
        b.HasIndex(k => k.AramaAdi);
        b.HasIndex(k => k.Durum);

        b.HasOne(k => k.Aile).WithMany(a => a.Uyeler).HasForeignKey(k => k.AileId);

        b.ToTable(t => t.HasCheckConstraint("CK_Kisiler_TcUzunluk", "[TcKimlikNo] IS NULL OR LEN([TcKimlikNo]) = 11"));
    }
}

internal class GorevliKonfigurasyonu : IEntityTypeConfiguration<Gorevli>
{
    public void Configure(EntityTypeBuilder<Gorevli> b)
    {
        b.ToTable("Kullanicilar");
        KisiEslemesi.OrtakAlanlar(b);

        b.Property(k => k.KullaniciAdi).HasMaxLength(50).IsRequired();
        b.HasIndex(k => k.KullaniciAdi).IsUnique();
        b.Property(k => k.ParolaHash).HasField("_parolaHash").HasMaxLength(100).IsRequired();

        b.HasOne(k => k.Rol).WithMany().HasForeignKey(k => k.RolId);
    }
}

internal class GonulluKonfigurasyonu : IEntityTypeConfiguration<Gonullu>
{
    public void Configure(EntityTypeBuilder<Gonullu> b)
    {
        b.ToTable("Gonulluler");
        KisiEslemesi.OrtakAlanlar(b);

        b.Property(g => g.UzmanlikAlani).HasMaxLength(100).IsRequired();
        b.Property(g => g.Notlar).HasMaxLength(1000);
    }
}

internal class AileKonfigurasyonu : IEntityTypeConfiguration<Aile>
{
    public void Configure(EntityTypeBuilder<Aile> b)
    {
        b.ToTable("Aileler");
        b.Property(a => a.AileNo)
            .HasMaxLength(12)
            .HasComputedColumnSql("'A-' + RIGHT('000000' + CAST([Id] AS varchar(10)), 6)", stored: true);
        b.HasIndex(a => a.AileNo).IsUnique();
        b.Property(a => a.AileAdi).HasMaxLength(100).IsRequired();
        b.Property(a => a.Notlar).HasMaxLength(1000);
        b.Property(a => a.RowVersion).IsRowVersion();
        b.Ignore(a => a.CadirKenttekiUyeler);
    }
}

internal class KisiHareketiKonfigurasyonu : IEntityTypeConfiguration<KisiHareketi>
{
    public void Configure(EntityTypeBuilder<KisiHareketi> b)
    {
        b.ToTable("KisiHareketleri");
        b.HasOne(h => h.Kisi).WithMany(k => k.Hareketler).HasForeignKey(h => h.KisiId);
        b.HasOne(h => h.EskiCadir).WithMany().HasForeignKey(h => h.EskiCadirId);
        b.HasOne(h => h.YeniCadir).WithMany().HasForeignKey(h => h.YeniCadirId);
        b.HasOne(h => h.EskiAile).WithMany().HasForeignKey(h => h.EskiAileId);
        b.HasOne(h => h.YeniAile).WithMany().HasForeignKey(h => h.YeniAileId);
        b.HasOne(h => h.Kullanici).WithMany().HasForeignKey(h => h.KullaniciId);
        b.HasIndex(h => new { h.KisiId, h.Zaman });
    }
}

internal class AileGecmisiKonfigurasyonu : IEntityTypeConfiguration<AileGecmisi>
{
    public void Configure(EntityTypeBuilder<AileGecmisi> b)
    {
        b.ToTable("AileGecmisi");
        b.Property(g => g.Aciklama).HasMaxLength(1000).IsRequired();
        b.Property(g => g.TasinanKisiler).HasMaxLength(1000);
        b.HasOne(g => g.KaynakAile).WithMany().HasForeignKey(g => g.KaynakAileId);
        b.HasOne(g => g.HedefAile).WithMany().HasForeignKey(g => g.HedefAileId);
        b.HasOne(g => g.Kullanici).WithMany().HasForeignKey(g => g.KullaniciId);
    }
}

internal class GerekceKonfigurasyonu : IEntityTypeConfiguration<Gerekce>
{
    public void Configure(EntityTypeBuilder<Gerekce> b)
    {
        b.ToTable("Gerekceler");
        b.Property(g => g.Ad).HasMaxLength(100).IsRequired();
        b.HasIndex(g => g.Kod).IsUnique();
    }
}

internal class GerekceUrunKonfigurasyonu : IEntityTypeConfiguration<GerekceUrun>
{
    public void Configure(EntityTypeBuilder<GerekceUrun> b)
    {
        b.ToTable("GerekceUrunleri");
        b.HasKey(g => new { g.GerekceId, g.UrunId });
        b.HasOne(g => g.Gerekce).WithMany(g => g.OncelikliUrunler).HasForeignKey(g => g.GerekceId);
        b.HasOne(g => g.Urun).WithMany().HasForeignKey(g => g.UrunId);
    }
}

internal class AileGerekcesiKonfigurasyonu : IEntityTypeConfiguration<AileGerekcesi>
{
    public void Configure(EntityTypeBuilder<AileGerekcesi> b)
    {
        b.ToTable("AileGerekceleri");
        b.Property(g => g.Aciklama).HasMaxLength(500).IsRequired();
        b.HasOne(g => g.Aile).WithMany(a => a.GerekceDegisiklikleri).HasForeignKey(g => g.AileId);
        b.HasOne(g => g.Gerekce).WithMany().HasForeignKey(g => g.GerekceId);
        b.HasOne(g => g.Kullanici).WithMany().HasForeignKey(g => g.KullaniciId);
        b.HasIndex(g => new { g.AileId, g.Gecerli });
    }
}
