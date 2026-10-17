using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AfetCadirKent.DataAccess.Konfigurasyonlar;

internal class BolgeKonfigurasyonu : IEntityTypeConfiguration<Bolge>
{
    public void Configure(EntityTypeBuilder<Bolge> b)
    {
        b.ToTable("Bolgeler");
        b.Property(x => x.Kod).HasMaxLength(5).IsRequired();
        b.HasIndex(x => x.Kod).IsUnique();
        b.Property(x => x.Ad).HasMaxLength(100).IsRequired();
    }
}

internal class CadirKonfigurasyonu : IEntityTypeConfiguration<Cadir>
{
    public void Configure(EntityTypeBuilder<Cadir> b)
    {
        b.ToTable("Cadirlar", t =>
        {
            t.HasCheckConstraint("CK_Cadirlar_Kapasite", "[Kapasite] > 0");
            // Kapasite asla aşılamaz: veritabanı düzeyinde son savunma hattı.
            t.HasCheckConstraint("CK_Cadirlar_Doluluk", "[Doluluk] >= 0 AND [Doluluk] <= [Kapasite]");
        });
        b.Property(c => c.Doluluk);
        b.Property(c => c.RowVersion).IsRowVersion();
        b.HasOne(c => c.Bolge).WithMany(x => x.Cadirlar).HasForeignKey(c => c.BolgeId);
        b.HasIndex(c => new { c.BolgeId, c.Sira, c.Numara }).IsUnique();
        b.Ignore(c => c.Kod);
    }
}

internal class YerlesimKonfigurasyonu : IEntityTypeConfiguration<Yerlesim>
{
    public void Configure(EntityTypeBuilder<Yerlesim> b)
    {
        b.ToTable("Yerlesimler");
        b.HasOne(y => y.Kisi).WithMany(k => k.Yerlesimler).HasForeignKey(y => y.KisiId);
        b.HasOne(y => y.Cadir).WithMany(c => c.Yerlesimler).HasForeignKey(y => y.CadirId);
        b.HasOne(y => y.Kullanici).WithMany().HasForeignKey(y => y.KullaniciId);
        // Bir kişinin aynı anda tek aktif yerleşimi olabilir.
        b.HasIndex(y => y.KisiId).IsUnique().HasFilter("[Bitis] IS NULL");
        b.HasIndex(y => new { y.CadirId, y.Bitis });
    }
}

internal class BeklemeListesiKonfigurasyonu : IEntityTypeConfiguration<BeklemeListesiKaydi>
{
    public void Configure(EntityTypeBuilder<BeklemeListesiKaydi> b)
    {
        b.ToTable("BeklemeListesi");
        b.Property(x => x.Aciklama).HasMaxLength(500);
        b.HasOne(x => x.Aile).WithMany().HasForeignKey(x => x.AileId);
        b.HasOne(x => x.Kullanici).WithMany().HasForeignKey(x => x.KullaniciId);
        // Bir aile listede aynı anda bir kez bekleyebilir.
        b.HasIndex(x => x.AileId).IsUnique().HasFilter("[CikisZamani] IS NULL");
    }
}

internal class UrunKonfigurasyonu : IEntityTypeConfiguration<Urun>
{
    public void Configure(EntityTypeBuilder<Urun> b)
    {
        b.ToTable("Urunler", t =>
        {
            t.HasCheckConstraint("CK_Urunler_Hak", "[HakMiktari] > 0");
            t.HasCheckConstraint("CK_Urunler_Tekrar", "[TekrarAlmaGunu] >= 0");
            t.HasCheckConstraint("CK_Urunler_Kritik", "[KritikSeviye] >= 0");
        });
        b.Property(u => u.Ad).HasMaxLength(100).IsRequired();
        b.HasIndex(u => u.Ad).IsUnique();
    }
}

internal class BagisciKonfigurasyonu : IEntityTypeConfiguration<Bagisci>
{
    public void Configure(EntityTypeBuilder<Bagisci> b)
    {
        b.ToTable("Bagiscilar");
        b.Property(x => x.Ad).HasMaxLength(150).IsRequired();
        b.Property(x => x.Telefon).HasMaxLength(20);
    }
}

internal class PartiKonfigurasyonu : IEntityTypeConfiguration<Parti>
{
    public void Configure(EntityTypeBuilder<Parti> b)
    {
        b.ToTable("Partiler", t =>
        {
            t.HasCheckConstraint("CK_Partiler_Miktar", "[Miktar] >= 0");
            t.HasCheckConstraint("CK_Partiler_Ayrilan", "[AyrilanMiktar] >= 0 AND [AyrilanMiktar] <= [Miktar]");
        });
        b.Property(p => p.PartiNo).HasMaxLength(50).IsRequired();
        b.Property(p => p.Miktar);
        b.Property(p => p.AyrilanMiktar);
        b.Property(p => p.RowVersion).IsRowVersion();
        b.HasOne(p => p.Urun).WithMany(u => u.Partiler).HasForeignKey(p => p.UrunId);
        b.HasOne(p => p.Bagisci).WithMany().HasForeignKey(p => p.BagisciId);
        b.HasIndex(p => new { p.UrunId, p.SonKullanmaTarihi });
    }
}

internal class StokHareketiKonfigurasyonu : IEntityTypeConfiguration<StokHareketi>
{
    public void Configure(EntityTypeBuilder<StokHareketi> b)
    {
        b.ToTable("StokHareketleri", t => t.HasCheckConstraint("CK_StokHareketleri_Miktar", "[Miktar] > 0"));
        b.HasDiscriminator<StokHareketTuru>("HareketTuru")
            .HasValue<GirisHareketi>(StokHareketTuru.Giris)
            .HasValue<DagitimHareketi>(StokHareketTuru.Dagitim)
            .HasValue<IadeHareketi>(StokHareketTuru.Iade)
            .HasValue<HasarHareketi>(StokHareketTuru.Hasar)
            .HasValue<SayimFarkiHareketi>(StokHareketTuru.SayimFarki);
        b.HasOne(h => h.Parti).WithMany(p => p.Hareketler).HasForeignKey(h => h.PartiId);
        b.HasOne(h => h.Kullanici).WithMany().HasForeignKey(h => h.KullaniciId);
        b.HasIndex(h => h.Zaman);
    }
}

internal class DagitimHareketiKonfigurasyonu : IEntityTypeConfiguration<DagitimHareketi>
{
    public void Configure(EntityTypeBuilder<DagitimHareketi> b)
    {
        b.HasOne(h => h.DagitimDetay).WithMany(d => d.Teslimler).HasForeignKey(h => h.DagitimDetayId);
        b.HasOne(h => h.TeslimAlanKisi).WithMany().HasForeignKey(h => h.TeslimAlanKisiId);
    }
}

internal class StokAyirmaKonfigurasyonu : IEntityTypeConfiguration<StokAyirma>
{
    public void Configure(EntityTypeBuilder<StokAyirma> b)
    {
        b.ToTable("StokAyirmalari", t => t.HasCheckConstraint(
            "CK_StokAyirmalari_Miktar",
            "[Miktar] > 0 AND [TeslimEdilenMiktar] >= 0 AND [SerbestBirakilanMiktar] >= 0 " +
            "AND [TeslimEdilenMiktar] + [SerbestBirakilanMiktar] <= [Miktar]"));
        b.HasOne(a => a.DagitimDetay).WithMany(d => d.Ayirmalar).HasForeignKey(a => a.DagitimDetayId);
        b.HasOne(a => a.Parti).WithMany().HasForeignKey(a => a.PartiId);
        b.HasIndex(a => a.Durum);
    }
}

internal class DagitimKonfigurasyonu : IEntityTypeConfiguration<Dagitim>
{
    public void Configure(EntityTypeBuilder<Dagitim> b)
    {
        b.ToTable("Dagitimlar");
        b.Property(d => d.Aciklama).HasMaxLength(500);
        b.HasOne(d => d.Urun).WithMany().HasForeignKey(d => d.UrunId);
        b.HasOne(d => d.OlusturanKullanici).WithMany().HasForeignKey(d => d.OlusturanKullaniciId);
        b.HasOne(d => d.OnaylayanKullanici).WithMany().HasForeignKey(d => d.OnaylayanKullaniciId);
        b.HasIndex(d => new { d.UrunId, d.Tarih });

        // Bir ürün için aynı anda tek açık (taslak/onaylı) liste: eşzamanlı iki liste hazırlanamaz (K-049a).
        b.HasIndex(d => d.UrunId)
            .IsUnique()
            .HasFilter($"[Aktif] = 1 AND [Durum] IN ({(int)DagitimDurumu.Taslak}, {(int)DagitimDurumu.Onaylandi})")
            .HasDatabaseName("IX_Dagitimlar_UrunId_AcikListe");
    }
}

internal class DagitimDetayKonfigurasyonu : IEntityTypeConfiguration<DagitimDetay>
{
    public void Configure(EntityTypeBuilder<DagitimDetay> b)
    {
        b.ToTable("DagitimDetaylari", t => t.HasCheckConstraint(
            "CK_DagitimDetaylari_Miktar",
            "[HakMiktari] > 0 AND [TeslimEdilenMiktar] >= 0 AND [TeslimEdilenMiktar] <= [HakMiktari] " +
            "AND [DevredenMiktar] >= 0 AND [DevredenMiktar] <= [HakMiktari]"));
        b.Property(d => d.HakMiktari).HasField("_hakMiktari");
        b.Property(d => d.TeslimEdilenMiktar);
        b.Property(d => d.DevredenMiktar);
        b.Property(d => d.IstisnaGerekcesi).HasMaxLength(500);
        b.Property(d => d.RowVersion).IsRowVersion();
        b.HasOne(d => d.Dagitim).WithMany(x => x.Detaylar).HasForeignKey(d => d.DagitimId);
        b.HasOne(d => d.Aile).WithMany().HasForeignKey(d => d.AileId);
        b.HasIndex(d => new { d.DagitimId, d.AileId }).IsUnique();
    }
}
