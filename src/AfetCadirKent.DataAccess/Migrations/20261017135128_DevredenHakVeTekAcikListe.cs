using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AfetCadirKent.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class DevredenHakVeTekAcikListe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DagitimDetaylari_Miktar",
                table: "DagitimDetaylari");

            migrationBuilder.AddColumn<decimal>(
                name: "DevredenMiktar",
                table: "DagitimDetaylari",
                type: "decimal(12,2)",
                precision: 12,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Dagitimlar_UrunId_AcikListe",
                table: "Dagitimlar",
                column: "UrunId",
                unique: true,
                filter: "[Aktif] = 1 AND [Durum] IN (0, 1)");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DagitimDetaylari_Miktar",
                table: "DagitimDetaylari",
                sql: "[HakMiktari] > 0 AND [TeslimEdilenMiktar] >= 0 AND [TeslimEdilenMiktar] <= [HakMiktari] AND [DevredenMiktar] >= 0 AND [DevredenMiktar] <= [HakMiktari]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dagitimlar_UrunId_AcikListe",
                table: "Dagitimlar");

            migrationBuilder.DropCheckConstraint(
                name: "CK_DagitimDetaylari_Miktar",
                table: "DagitimDetaylari");

            migrationBuilder.DropColumn(
                name: "DevredenMiktar",
                table: "DagitimDetaylari");

            migrationBuilder.AddCheckConstraint(
                name: "CK_DagitimDetaylari_Miktar",
                table: "DagitimDetaylari",
                sql: "[HakMiktari] > 0 AND [TeslimEdilenMiktar] >= 0 AND [TeslimEdilenMiktar] <= [HakMiktari]");
        }
    }
}
