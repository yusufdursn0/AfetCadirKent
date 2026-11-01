using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AfetCadirKent.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class KayitGorevlisiRaporIzniKaldirildi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 2013);

            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 2023);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Yetkiler",
                columns: new[] { "Id", "Izin", "Modul", "RolId" },
                values: new object[,]
                {
                    { 2013, 3, 1, 2 },
                    { 2023, 3, 2, 2 }
                });
        }
    }
}
