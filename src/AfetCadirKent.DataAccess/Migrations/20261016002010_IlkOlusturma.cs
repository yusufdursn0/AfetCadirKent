using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AfetCadirKent.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IlkOlusturma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aileler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AileNo = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false, computedColumnSql: "'A-' + RIGHT('000000' + CAST([Id] AS varchar(10)), 6)", stored: true),
                    AileAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TekEbeveyn = table.Column<bool>(type: "bit", nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    KayitZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aileler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bagiscilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bagiscilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bolgeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolgeler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gerekceler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kod = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DestekOnceligi = table.Column<int>(type: "int", nullable: false),
                    YerlesimEtkisi = table.Column<int>(type: "int", nullable: false),
                    TumUrunler = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gerekceler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gonulluler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UzmanlikAlani = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    KayitZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gonulluler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Urunler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Kategori = table.Column<int>(type: "int", nullable: false),
                    Birim = table.Column<int>(type: "int", nullable: false),
                    HesaplamaTuru = table.Column<int>(type: "int", nullable: false),
                    HakMiktari = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    TekrarAlmaGunu = table.Column<int>(type: "int", nullable: false),
                    KritikSeviye = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Urunler", x => x.Id);
                    table.CheckConstraint("CK_Urunler_Hak", "[HakMiktari] > 0");
                    table.CheckConstraint("CK_Urunler_Kritik", "[KritikSeviye] >= 0");
                    table.CheckConstraint("CK_Urunler_Tekrar", "[TekrarAlmaGunu] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Kisiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KisiNo = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false, computedColumnSql: "'K-' + RIGHT('000000' + CAST([Id] AS varchar(10)), 6)", stored: true),
                    AileId = table.Column<int>(type: "int", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "char(11)", unicode: false, fixedLength: true, maxLength: 11, nullable: true),
                    DogumTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    DogumTarihiTahmini = table.Column<bool>(type: "bit", nullable: false),
                    Cinsiyet = table.Column<int>(type: "int", nullable: false),
                    Memleket = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Yakinlik = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Hamile = table.Column<bool>(type: "bit", nullable: false),
                    TekerlekliSandalye = table.Column<bool>(type: "bit", nullable: false),
                    KronikHasta = table.Column<bool>(type: "bit", nullable: false),
                    RefakatsizCocuk = table.Column<bool>(type: "bit", nullable: false),
                    SaglikNotu = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Fotograf = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    AramaAdi = table.Column<string>(type: "nvarchar(101)", maxLength: 101, nullable: false),
                    KayitZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kisiler", x => x.Id);
                    table.CheckConstraint("CK_Kisiler_TcUzunluk", "[TcKimlikNo] IS NULL OR LEN([TcKimlikNo]) = 11");
                    table.ForeignKey(
                        name: "FK_Kisiler_Aileler_AileId",
                        column: x => x.AileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cadirlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BolgeId = table.Column<int>(type: "int", nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    Numara = table.Column<int>(type: "int", nullable: false),
                    Kapasite = table.Column<int>(type: "int", nullable: false),
                    Doluluk = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Erisilebilir = table.Column<bool>(type: "bit", nullable: false),
                    SaglikNoktasinaYakin = table.Column<bool>(type: "bit", nullable: false),
                    Isitmali = table.Column<bool>(type: "bit", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cadirlar", x => x.Id);
                    table.CheckConstraint("CK_Cadirlar_Doluluk", "[Doluluk] >= 0 AND [Doluluk] <= [Kapasite]");
                    table.CheckConstraint("CK_Cadirlar_Kapasite", "[Kapasite] > 0");
                    table.ForeignKey(
                        name: "FK_Cadirlar_Bolgeler_BolgeId",
                        column: x => x.BolgeId,
                        principalTable: "Bolgeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParolaHash = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    OlusturmaZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SonGirisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Roller_RolId",
                        column: x => x.RolId,
                        principalTable: "Roller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Yetkiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RolId = table.Column<int>(type: "int", nullable: false),
                    Modul = table.Column<int>(type: "int", nullable: false),
                    Izin = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yetkiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yetkiler_Roller_RolId",
                        column: x => x.RolId,
                        principalTable: "Roller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GerekceUrunleri",
                columns: table => new
                {
                    GerekceId = table.Column<int>(type: "int", nullable: false),
                    UrunId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GerekceUrunleri", x => new { x.GerekceId, x.UrunId });
                    table.ForeignKey(
                        name: "FK_GerekceUrunleri_Gerekceler_GerekceId",
                        column: x => x.GerekceId,
                        principalTable: "Gerekceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GerekceUrunleri_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partiler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    BagisciId = table.Column<int>(type: "int", nullable: true),
                    PartiNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    AyrilanMiktar = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    SonKullanmaTarihi = table.Column<DateOnly>(type: "date", nullable: true),
                    GirisZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partiler", x => x.Id);
                    table.CheckConstraint("CK_Partiler_Ayrilan", "[AyrilanMiktar] >= 0 AND [AyrilanMiktar] <= [Miktar]");
                    table.CheckConstraint("CK_Partiler_Miktar", "[Miktar] >= 0");
                    table.ForeignKey(
                        name: "FK_Partiler_Bagiscilar_BagisciId",
                        column: x => x.BagisciId,
                        principalTable: "Bagiscilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partiler_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AileGecmisi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    KaynakAileId = table.Column<int>(type: "int", nullable: false),
                    HedefAileId = table.Column<int>(type: "int", nullable: false),
                    Zaman = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TasinanKisiler = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AileGecmisi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AileGecmisi_Aileler_HedefAileId",
                        column: x => x.HedefAileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AileGecmisi_Aileler_KaynakAileId",
                        column: x => x.KaynakAileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AileGecmisi_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AileGerekceleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AileId = table.Column<int>(type: "int", nullable: false),
                    GerekceId = table.Column<int>(type: "int", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Gecerli = table.Column<bool>(type: "bit", nullable: false),
                    Zaman = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AileGerekceleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AileGerekceleri_Aileler_AileId",
                        column: x => x.AileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AileGerekceleri_Gerekceler_GerekceId",
                        column: x => x.GerekceId,
                        principalTable: "Gerekceler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AileGerekceleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BeklemeListesi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AileId = table.Column<int>(type: "int", nullable: false),
                    EklenmeZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CikisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeklemeListesi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BeklemeListesi_Aileler_AileId",
                        column: x => x.AileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BeklemeListesi_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dagitimlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UrunId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateOnly>(type: "date", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OlusturmaZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<int>(type: "int", nullable: true),
                    OnayZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OnaylayanKullaniciId = table.Column<int>(type: "int", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dagitimlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dagitimlar_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dagitimlar_Kullanicilar_OnaylayanKullaniciId",
                        column: x => x.OnaylayanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dagitimlar_Urunler_UrunId",
                        column: x => x.UrunId,
                        principalTable: "Urunler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Duyurular",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Baslik = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Metin = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OlusturanKullaniciId = table.Column<int>(type: "int", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Duyurular", x => x.Id);
                    table.CheckConstraint("CK_Duyurular_Tarih", "[BitisTarihi] >= [BaslangicTarihi]");
                    table.ForeignKey(
                        name: "FK_Duyurular_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IslemGecmisi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Zaman = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IslemTuru = table.Column<int>(type: "int", nullable: false),
                    VarlikAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VarlikId = table.Column<int>(type: "int", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IslemGecmisi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IslemGecmisi_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KisiHareketleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KisiId = table.Column<int>(type: "int", nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: false),
                    Zaman = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EskiCadirId = table.Column<int>(type: "int", nullable: true),
                    YeniCadirId = table.Column<int>(type: "int", nullable: true),
                    EskiAileId = table.Column<int>(type: "int", nullable: true),
                    YeniAileId = table.Column<int>(type: "int", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KisiHareketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KisiHareketleri_Aileler_EskiAileId",
                        column: x => x.EskiAileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KisiHareketleri_Aileler_YeniAileId",
                        column: x => x.YeniAileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KisiHareketleri_Cadirlar_EskiCadirId",
                        column: x => x.EskiCadirId,
                        principalTable: "Cadirlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KisiHareketleri_Cadirlar_YeniCadirId",
                        column: x => x.YeniCadirId,
                        principalTable: "Cadirlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KisiHareketleri_Kisiler_KisiId",
                        column: x => x.KisiId,
                        principalTable: "Kisiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KisiHareketleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Talepler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AileId = table.Column<int>(type: "int", nullable: true),
                    CadirId = table.Column<int>(type: "int", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tur = table.Column<int>(type: "int", nullable: true),
                    Kaynak = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    YardimOnceligi = table.Column<int>(type: "int", nullable: true),
                    SaglikPersoneliDegerlendirecek = table.Column<bool>(type: "bit", nullable: false),
                    HedefRol = table.Column<int>(type: "int", nullable: true),
                    SorumluKullaniciId = table.Column<int>(type: "int", nullable: true),
                    AcilisZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KapanisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sonuc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SiniflandirmaDurumu = table.Column<int>(type: "int", nullable: false),
                    YzOnerilenTur = table.Column<int>(type: "int", nullable: true),
                    YzOnerilenOncelik = table.Column<int>(type: "int", nullable: true),
                    YzGerekce = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OlusturanKullaniciId = table.Column<int>(type: "int", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Talepler", x => x.Id);
                    table.CheckConstraint("CK_Talepler_SaglikOnceligi", "[Tur] IS NULL OR [Tur] <> 0 OR [YardimOnceligi] IS NULL");
                    table.ForeignKey(
                        name: "FK_Talepler_Aileler_AileId",
                        column: x => x.AileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Talepler_Cadirlar_CadirId",
                        column: x => x.CadirId,
                        principalTable: "Cadirlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Talepler_Kullanicilar_OlusturanKullaniciId",
                        column: x => x.OlusturanKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Talepler_Kullanicilar_SorumluKullaniciId",
                        column: x => x.SorumluKullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "YakinBulmaBasvurulari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BasvuranAdSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BasvuranTelefon = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BasvuranAileId = table.Column<int>(type: "int", nullable: true),
                    ArananAd = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArananSoyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ArananYaklasikYas = table.Column<int>(type: "int", nullable: true),
                    ArananMemleket = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    EslesenKisiId = table.Column<int>(type: "int", nullable: true),
                    DogrulamaAciklamasi = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Sonuc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AcilisZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KapanisZamani = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YakinBulmaBasvurulari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YakinBulmaBasvurulari_Aileler_BasvuranAileId",
                        column: x => x.BasvuranAileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YakinBulmaBasvurulari_Kisiler_EslesenKisiId",
                        column: x => x.EslesenKisiId,
                        principalTable: "Kisiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_YakinBulmaBasvurulari_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Yerlesimler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KisiId = table.Column<int>(type: "int", nullable: false),
                    CadirId = table.Column<int>(type: "int", nullable: false),
                    Baslangic = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bitis = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Yerlesimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Yerlesimler_Cadirlar_CadirId",
                        column: x => x.CadirId,
                        principalTable: "Cadirlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yerlesimler_Kisiler_KisiId",
                        column: x => x.KisiId,
                        principalTable: "Kisiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Yerlesimler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DagitimDetaylari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DagitimId = table.Column<int>(type: "int", nullable: false),
                    AileId = table.Column<int>(type: "int", nullable: false),
                    HakMiktari = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    TeslimEdilenMiktar = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    OncelikliIhtiyac = table.Column<bool>(type: "bit", nullable: false),
                    IstisnaGerekcesi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DagitimDetaylari", x => x.Id);
                    table.CheckConstraint("CK_DagitimDetaylari_Miktar", "[HakMiktari] > 0 AND [TeslimEdilenMiktar] >= 0 AND [TeslimEdilenMiktar] <= [HakMiktari]");
                    table.ForeignKey(
                        name: "FK_DagitimDetaylari_Aileler_AileId",
                        column: x => x.AileId,
                        principalTable: "Aileler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DagitimDetaylari_Dagitimlar_DagitimId",
                        column: x => x.DagitimId,
                        principalTable: "Dagitimlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vardiyalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GonulluId = table.Column<int>(type: "int", nullable: false),
                    BolgeId = table.Column<int>(type: "int", nullable: true),
                    Baslangic = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Bitis = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GorevTanimi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TalepId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vardiyalar", x => x.Id);
                    table.CheckConstraint("CK_Vardiyalar_Zaman", "[Bitis] > [Baslangic]");
                    table.ForeignKey(
                        name: "FK_Vardiyalar_Bolgeler_BolgeId",
                        column: x => x.BolgeId,
                        principalTable: "Bolgeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vardiyalar_Gonulluler_GonulluId",
                        column: x => x.GonulluId,
                        principalTable: "Gonulluler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vardiyalar_Talepler_TalepId",
                        column: x => x.TalepId,
                        principalTable: "Talepler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StokAyirmalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DagitimDetayId = table.Column<int>(type: "int", nullable: false),
                    PartiId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    TeslimEdilenMiktar = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    SerbestBirakilanMiktar = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    OlusturmaZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KapanisZamani = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StokAyirmalari", x => x.Id);
                    table.CheckConstraint("CK_StokAyirmalari_Miktar", "[Miktar] > 0 AND [TeslimEdilenMiktar] >= 0 AND [SerbestBirakilanMiktar] >= 0 AND [TeslimEdilenMiktar] + [SerbestBirakilanMiktar] <= [Miktar]");
                    table.ForeignKey(
                        name: "FK_StokAyirmalari_DagitimDetaylari_DagitimDetayId",
                        column: x => x.DagitimDetayId,
                        principalTable: "DagitimDetaylari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StokAyirmalari_Partiler_PartiId",
                        column: x => x.PartiId,
                        principalTable: "Partiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StokHareketleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PartiId = table.Column<int>(type: "int", nullable: false),
                    Miktar = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Zaman = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KullaniciId = table.Column<int>(type: "int", nullable: true),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    HareketTuru = table.Column<int>(type: "int", nullable: false),
                    DagitimDetayId = table.Column<int>(type: "int", nullable: true),
                    TeslimAlanKisiId = table.Column<int>(type: "int", nullable: true),
                    FazlaMi = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StokHareketleri", x => x.Id);
                    table.CheckConstraint("CK_StokHareketleri_Miktar", "[Miktar] > 0");
                    table.ForeignKey(
                        name: "FK_StokHareketleri_DagitimDetaylari_DagitimDetayId",
                        column: x => x.DagitimDetayId,
                        principalTable: "DagitimDetaylari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StokHareketleri_Kisiler_TeslimAlanKisiId",
                        column: x => x.TeslimAlanKisiId,
                        principalTable: "Kisiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StokHareketleri_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StokHareketleri_Partiler_PartiId",
                        column: x => x.PartiId,
                        principalTable: "Partiler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Gerekceler",
                columns: new[] { "Id", "Ad", "DestekOnceligi", "Kod", "TumUrunler", "YerlesimEtkisi" },
                values: new object[,]
                {
                    { 1, "0–2 yaş bebek", 2, 1, false, 1 },
                    { 2, "Hamile", 2, 2, false, 2 },
                    { 3, "Tekerlekli sandalye", 2, 3, false, 3 },
                    { 4, "Kronik hasta", 2, 4, false, 2 },
                    { 5, "Refakatsiz çocuk", 2, 5, false, 4 },
                    { 6, "65 yaş üstü", 1, 6, false, 3 },
                    { 7, "Tek ebeveyn", 1, 7, false, 5 },
                    { 8, "5 kişiden kalabalık", 1, 8, false, 6 },
                    { 9, "3 gündür yardım almadı", 1, 9, true, 0 },
                    { 10, "Özel durumu yok", 0, 10, false, 0 }
                });

            migrationBuilder.InsertData(
                table: "Roller",
                columns: new[] { "Id", "Aciklama", "Ad", "Tur" },
                values: new object[,]
                {
                    { 1, "Her şey ve rapor; gönüllü yönetimi ve duyurular", "Koordinatör", 1 },
                    { 2, "Kayıt ve çadır", "Kayıt görevlisi", 2 },
                    { 3, "Depo, dağıtım ve rapor", "Depo sorumlusu", 3 },
                    { 4, "Sağlık bilgileri ve sağlık talepleri", "Sağlık personeli", 4 },
                    { 5, "Kendine atanan talepler", "Saha görevlisi", 5 },
                    { 6, "Kendi ailesi ve talep oluşturma", "Kiosk", 6 }
                });

            migrationBuilder.InsertData(
                table: "Yetkiler",
                columns: new[] { "Id", "Izin", "Modul", "RolId" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 2, 1, 1 },
                    { 3, 3, 1, 1 },
                    { 4, 1, 2, 1 },
                    { 5, 2, 2, 1 },
                    { 6, 3, 2, 1 },
                    { 7, 1, 3, 1 },
                    { 8, 2, 3, 1 },
                    { 9, 3, 3, 1 },
                    { 10, 1, 4, 1 },
                    { 11, 2, 4, 1 },
                    { 12, 3, 4, 1 },
                    { 13, 1, 5, 1 },
                    { 14, 2, 5, 1 },
                    { 15, 3, 5, 1 },
                    { 16, 1, 6, 1 },
                    { 17, 2, 6, 1 },
                    { 18, 3, 6, 1 },
                    { 19, 1, 7, 1 },
                    { 20, 2, 7, 1 },
                    { 21, 3, 7, 1 },
                    { 22, 1, 8, 1 },
                    { 23, 2, 8, 1 },
                    { 24, 3, 8, 1 },
                    { 25, 1, 9, 1 },
                    { 26, 2, 9, 1 },
                    { 27, 3, 9, 1 },
                    { 28, 1, 10, 1 },
                    { 29, 2, 10, 1 },
                    { 30, 3, 10, 1 },
                    { 31, 1, 11, 1 },
                    { 32, 2, 11, 1 },
                    { 33, 3, 11, 1 },
                    { 34, 1, 12, 1 },
                    { 35, 2, 12, 1 },
                    { 36, 3, 12, 1 },
                    { 37, 1, 13, 1 },
                    { 38, 2, 13, 1 },
                    { 39, 3, 13, 1 },
                    { 40, 1, 1, 2 },
                    { 41, 2, 1, 2 },
                    { 42, 3, 1, 2 },
                    { 43, 1, 2, 2 },
                    { 44, 2, 2, 2 },
                    { 45, 3, 2, 2 },
                    { 46, 1, 5, 2 },
                    { 47, 2, 5, 2 },
                    { 48, 1, 7, 2 },
                    { 49, 2, 7, 2 },
                    { 50, 1, 1, 3 },
                    { 51, 1, 3, 3 },
                    { 52, 2, 3, 3 },
                    { 53, 3, 3, 3 },
                    { 54, 1, 4, 3 },
                    { 55, 2, 4, 3 },
                    { 56, 3, 4, 3 },
                    { 57, 1, 5, 3 },
                    { 58, 2, 5, 3 },
                    { 59, 1, 1, 4 },
                    { 60, 1, 5, 4 },
                    { 61, 2, 5, 4 },
                    { 62, 1, 6, 4 },
                    { 63, 2, 6, 4 },
                    { 64, 1, 2, 5 },
                    { 65, 1, 5, 5 },
                    { 66, 2, 5, 5 },
                    { 67, 1, 13, 6 },
                    { 68, 2, 13, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AileGecmisi_HedefAileId",
                table: "AileGecmisi",
                column: "HedefAileId");

            migrationBuilder.CreateIndex(
                name: "IX_AileGecmisi_KaynakAileId",
                table: "AileGecmisi",
                column: "KaynakAileId");

            migrationBuilder.CreateIndex(
                name: "IX_AileGecmisi_KullaniciId",
                table: "AileGecmisi",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_AileGerekceleri_AileId_Gecerli",
                table: "AileGerekceleri",
                columns: new[] { "AileId", "Gecerli" });

            migrationBuilder.CreateIndex(
                name: "IX_AileGerekceleri_GerekceId",
                table: "AileGerekceleri",
                column: "GerekceId");

            migrationBuilder.CreateIndex(
                name: "IX_AileGerekceleri_KullaniciId",
                table: "AileGerekceleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Aileler_AileNo",
                table: "Aileler",
                column: "AileNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeklemeListesi_AileId",
                table: "BeklemeListesi",
                column: "AileId",
                unique: true,
                filter: "[CikisZamani] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BeklemeListesi_KullaniciId",
                table: "BeklemeListesi",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Bolgeler_Kod",
                table: "Bolgeler",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cadirlar_BolgeId_Sira_Numara",
                table: "Cadirlar",
                columns: new[] { "BolgeId", "Sira", "Numara" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DagitimDetaylari_AileId",
                table: "DagitimDetaylari",
                column: "AileId");

            migrationBuilder.CreateIndex(
                name: "IX_DagitimDetaylari_DagitimId_AileId",
                table: "DagitimDetaylari",
                columns: new[] { "DagitimId", "AileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dagitimlar_OlusturanKullaniciId",
                table: "Dagitimlar",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Dagitimlar_OnaylayanKullaniciId",
                table: "Dagitimlar",
                column: "OnaylayanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Dagitimlar_UrunId_Tarih",
                table: "Dagitimlar",
                columns: new[] { "UrunId", "Tarih" });

            migrationBuilder.CreateIndex(
                name: "IX_Duyurular_OlusturanKullaniciId",
                table: "Duyurular",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Gerekceler_Kod",
                table: "Gerekceler",
                column: "Kod",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GerekceUrunleri_UrunId",
                table: "GerekceUrunleri",
                column: "UrunId");

            migrationBuilder.CreateIndex(
                name: "IX_IslemGecmisi_KullaniciId",
                table: "IslemGecmisi",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_IslemGecmisi_VarlikAdi_VarlikId",
                table: "IslemGecmisi",
                columns: new[] { "VarlikAdi", "VarlikId" });

            migrationBuilder.CreateIndex(
                name: "IX_IslemGecmisi_Zaman",
                table: "IslemGecmisi",
                column: "Zaman");

            migrationBuilder.CreateIndex(
                name: "IX_KisiHareketleri_EskiAileId",
                table: "KisiHareketleri",
                column: "EskiAileId");

            migrationBuilder.CreateIndex(
                name: "IX_KisiHareketleri_EskiCadirId",
                table: "KisiHareketleri",
                column: "EskiCadirId");

            migrationBuilder.CreateIndex(
                name: "IX_KisiHareketleri_KisiId_Zaman",
                table: "KisiHareketleri",
                columns: new[] { "KisiId", "Zaman" });

            migrationBuilder.CreateIndex(
                name: "IX_KisiHareketleri_KullaniciId",
                table: "KisiHareketleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_KisiHareketleri_YeniAileId",
                table: "KisiHareketleri",
                column: "YeniAileId");

            migrationBuilder.CreateIndex(
                name: "IX_KisiHareketleri_YeniCadirId",
                table: "KisiHareketleri",
                column: "YeniCadirId");

            migrationBuilder.CreateIndex(
                name: "IX_Kisiler_AileId",
                table: "Kisiler",
                column: "AileId");

            migrationBuilder.CreateIndex(
                name: "IX_Kisiler_AramaAdi",
                table: "Kisiler",
                column: "AramaAdi");

            migrationBuilder.CreateIndex(
                name: "IX_Kisiler_Durum",
                table: "Kisiler",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_Kisiler_KisiNo",
                table: "Kisiler",
                column: "KisiNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kisiler_TcKimlikNo",
                table: "Kisiler",
                column: "TcKimlikNo",
                unique: true,
                filter: "[TcKimlikNo] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_RolId",
                table: "Kullanicilar",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Partiler_BagisciId",
                table: "Partiler",
                column: "BagisciId");

            migrationBuilder.CreateIndex(
                name: "IX_Partiler_UrunId_SonKullanmaTarihi",
                table: "Partiler",
                columns: new[] { "UrunId", "SonKullanmaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Roller_Tur",
                table: "Roller",
                column: "Tur",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StokAyirmalari_DagitimDetayId",
                table: "StokAyirmalari",
                column: "DagitimDetayId");

            migrationBuilder.CreateIndex(
                name: "IX_StokAyirmalari_Durum",
                table: "StokAyirmalari",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_StokAyirmalari_PartiId",
                table: "StokAyirmalari",
                column: "PartiId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_DagitimDetayId",
                table: "StokHareketleri",
                column: "DagitimDetayId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_KullaniciId",
                table: "StokHareketleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_PartiId",
                table: "StokHareketleri",
                column: "PartiId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_TeslimAlanKisiId",
                table: "StokHareketleri",
                column: "TeslimAlanKisiId");

            migrationBuilder.CreateIndex(
                name: "IX_StokHareketleri_Zaman",
                table: "StokHareketleri",
                column: "Zaman");

            migrationBuilder.CreateIndex(
                name: "IX_Talepler_AileId",
                table: "Talepler",
                column: "AileId");

            migrationBuilder.CreateIndex(
                name: "IX_Talepler_CadirId",
                table: "Talepler",
                column: "CadirId");

            migrationBuilder.CreateIndex(
                name: "IX_Talepler_Durum",
                table: "Talepler",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_Talepler_OlusturanKullaniciId",
                table: "Talepler",
                column: "OlusturanKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Talepler_SorumluKullaniciId",
                table: "Talepler",
                column: "SorumluKullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_Ad",
                table: "Urunler",
                column: "Ad",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vardiyalar_BolgeId",
                table: "Vardiyalar",
                column: "BolgeId");

            migrationBuilder.CreateIndex(
                name: "IX_Vardiyalar_GonulluId_Baslangic",
                table: "Vardiyalar",
                columns: new[] { "GonulluId", "Baslangic" });

            migrationBuilder.CreateIndex(
                name: "IX_Vardiyalar_TalepId",
                table: "Vardiyalar",
                column: "TalepId");

            migrationBuilder.CreateIndex(
                name: "IX_YakinBulmaBasvurulari_BasvuranAileId",
                table: "YakinBulmaBasvurulari",
                column: "BasvuranAileId");

            migrationBuilder.CreateIndex(
                name: "IX_YakinBulmaBasvurulari_Durum",
                table: "YakinBulmaBasvurulari",
                column: "Durum");

            migrationBuilder.CreateIndex(
                name: "IX_YakinBulmaBasvurulari_EslesenKisiId",
                table: "YakinBulmaBasvurulari",
                column: "EslesenKisiId");

            migrationBuilder.CreateIndex(
                name: "IX_YakinBulmaBasvurulari_KullaniciId",
                table: "YakinBulmaBasvurulari",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Yerlesimler_CadirId_Bitis",
                table: "Yerlesimler",
                columns: new[] { "CadirId", "Bitis" });

            migrationBuilder.CreateIndex(
                name: "IX_Yerlesimler_KisiId",
                table: "Yerlesimler",
                column: "KisiId",
                unique: true,
                filter: "[Bitis] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Yerlesimler_KullaniciId",
                table: "Yerlesimler",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_Yetkiler_RolId_Modul_Izin",
                table: "Yetkiler",
                columns: new[] { "RolId", "Modul", "Izin" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AileGecmisi");

            migrationBuilder.DropTable(
                name: "AileGerekceleri");

            migrationBuilder.DropTable(
                name: "BeklemeListesi");

            migrationBuilder.DropTable(
                name: "Duyurular");

            migrationBuilder.DropTable(
                name: "GerekceUrunleri");

            migrationBuilder.DropTable(
                name: "IslemGecmisi");

            migrationBuilder.DropTable(
                name: "KisiHareketleri");

            migrationBuilder.DropTable(
                name: "StokAyirmalari");

            migrationBuilder.DropTable(
                name: "StokHareketleri");

            migrationBuilder.DropTable(
                name: "Vardiyalar");

            migrationBuilder.DropTable(
                name: "YakinBulmaBasvurulari");

            migrationBuilder.DropTable(
                name: "Yerlesimler");

            migrationBuilder.DropTable(
                name: "Yetkiler");

            migrationBuilder.DropTable(
                name: "Gerekceler");

            migrationBuilder.DropTable(
                name: "DagitimDetaylari");

            migrationBuilder.DropTable(
                name: "Partiler");

            migrationBuilder.DropTable(
                name: "Gonulluler");

            migrationBuilder.DropTable(
                name: "Talepler");

            migrationBuilder.DropTable(
                name: "Kisiler");

            migrationBuilder.DropTable(
                name: "Dagitimlar");

            migrationBuilder.DropTable(
                name: "Bagiscilar");

            migrationBuilder.DropTable(
                name: "Cadirlar");

            migrationBuilder.DropTable(
                name: "Aileler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Urunler");

            migrationBuilder.DropTable(
                name: "Bolgeler");

            migrationBuilder.DropTable(
                name: "Roller");
        }
    }
}
