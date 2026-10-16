using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfetCadirKent.DataAccess.Konfigurasyonlar;

internal class TalepKonfigurasyonu : IEntityTypeConfiguration<Talep>
{
    public void Configure(EntityTypeBuilder<Talep> b)
    {
        b.ToTable("Talepler", t => t.HasCheckConstraint(
            "CK_Talepler_SaglikOnceligi",
            // Sağlık talepleri yardım önceliği alamaz (tıbbi aciliyet belirlenmez).
            $"[Tur] IS NULL OR [Tur] <> {(int)TalepTuru.Saglik} OR [YardimOnceligi] IS NULL"));
        b.Property(x => x.Aciklama).HasMaxLength(1000).IsRequired();
        b.Property(x => x.Sonuc).HasMaxLength(1000);
        b.Property(x => x.YzGerekce).HasMaxLength(1000);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasOne(x => x.Aile).WithMany().HasForeignKey(x => x.AileId);
        b.HasOne(x => x.Cadir).WithMany().HasForeignKey(x => x.CadirId);
        b.HasOne(x => x.SorumluKullanici).WithMany().HasForeignKey(x => x.SorumluKullaniciId);
        b.HasOne(x => x.OlusturanKullanici).WithMany().HasForeignKey(x => x.OlusturanKullaniciId);
        b.HasIndex(x => x.Durum);
        b.HasIndex(x => x.SorumluKullaniciId);
    }
}

internal class YakinBulmaBasvurusuKonfigurasyonu : IEntityTypeConfiguration<YakinBulmaBasvurusu>
{
    public void Configure(EntityTypeBuilder<YakinBulmaBasvurusu> b)
    {
        b.ToTable("YakinBulmaBasvurulari");
        b.Property(x => x.BasvuranAdSoyad).HasMaxLength(100).IsRequired();
        b.Property(x => x.BasvuranTelefon).HasMaxLength(20);
        b.Property(x => x.ArananAd).HasMaxLength(50).IsRequired();
        b.Property(x => x.ArananSoyad).HasMaxLength(50).IsRequired();
        b.Property(x => x.ArananMemleket).HasMaxLength(50);
        b.Property(x => x.DogrulamaAciklamasi).HasMaxLength(1000);
        b.Property(x => x.Sonuc).HasMaxLength(1000);
        b.HasOne(x => x.BasvuranAile).WithMany().HasForeignKey(x => x.BasvuranAileId);
        b.HasOne(x => x.EslesenKisi).WithMany().HasForeignKey(x => x.EslesenKisiId);
        b.HasOne(x => x.Kullanici).WithMany().HasForeignKey(x => x.KullaniciId);
        b.HasIndex(x => x.Durum);
    }
}

internal class VardiyaKonfigurasyonu : IEntityTypeConfiguration<Vardiya>
{
    public void Configure(EntityTypeBuilder<Vardiya> b)
    {
        b.ToTable("Vardiyalar", t => t.HasCheckConstraint("CK_Vardiyalar_Zaman", "[Bitis] > [Baslangic]"));
        b.Property(x => x.GorevTanimi).HasMaxLength(500);
        b.HasOne(x => x.Gonullu).WithMany(g => g.Vardiyalar).HasForeignKey(x => x.GonulluId);
        b.HasOne(x => x.Bolge).WithMany().HasForeignKey(x => x.BolgeId);
        b.HasOne(x => x.Talep).WithMany().HasForeignKey(x => x.TalepId);
        b.HasIndex(x => new { x.GonulluId, x.Baslangic });
    }
}

internal class DuyuruKonfigurasyonu : IEntityTypeConfiguration<Duyuru>
{
    public void Configure(EntityTypeBuilder<Duyuru> b)
    {
        b.ToTable("Duyurular", t => t.HasCheckConstraint("CK_Duyurular_Tarih", "[BitisTarihi] >= [BaslangicTarihi]"));
        b.Property(x => x.Baslik).HasMaxLength(150).IsRequired();
        b.Property(x => x.Metin).HasMaxLength(2000).IsRequired();
        b.HasOne(x => x.OlusturanKullanici).WithMany().HasForeignKey(x => x.OlusturanKullaniciId);
    }
}

internal class RolKonfigurasyonu : IEntityTypeConfiguration<Rol>
{
    public void Configure(EntityTypeBuilder<Rol> b)
    {
        b.ToTable("Roller");
        b.Property(x => x.Ad).HasMaxLength(50).IsRequired();
        b.HasIndex(x => x.Tur).IsUnique();
    }
}

internal class YetkiKonfigurasyonu : IEntityTypeConfiguration<Yetki>
{
    public void Configure(EntityTypeBuilder<Yetki> b)
    {
        b.ToTable("Yetkiler");
        b.HasOne(x => x.Rol).WithMany(r => r.Yetkiler).HasForeignKey(x => x.RolId);
        b.HasIndex(x => new { x.RolId, x.Modul, x.Izin }).IsUnique();
    }
}

internal class IslemKaydiKonfigurasyonu : IEntityTypeConfiguration<IslemKaydi>
{
    public void Configure(EntityTypeBuilder<IslemKaydi> b)
    {
        b.ToTable("IslemGecmisi");
        b.Property(x => x.KullaniciAdi).HasMaxLength(50).IsRequired();
        b.Property(x => x.VarlikAdi).HasMaxLength(50).IsRequired();
        b.Property(x => x.Aciklama).HasMaxLength(2000).IsRequired();
        b.HasOne(x => x.Kullanici).WithMany().HasForeignKey(x => x.KullaniciId);
        b.HasIndex(x => x.Zaman);
        b.HasIndex(x => new { x.VarlikAdi, x.VarlikId });
    }
}
