using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AfetCadirKent.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SaglikBilgisiTumGorevliler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 48,
                column: "Modul",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 49,
                column: "Modul",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Modul", "RolId" },
                values: new object[] { 7, 2 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 2, 7, 2 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 53,
                column: "Izin",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 2, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 3, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 56,
                column: "Izin",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 2, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 3, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "Modul", "RolId" },
                values: new object[] { 5, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "Izin", "RolId" },
                values: new object[] { 2, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 1, 6, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "Izin", "RolId" },
                values: new object[] { 2, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 1, 1 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "Modul", "RolId" },
                values: new object[] { 5, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "Izin", "RolId" },
                values: new object[] { 2, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 1, 6, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 2, 6, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 1, 2, 5 });

            migrationBuilder.InsertData(
                table: "Yetkiler",
                columns: new[] { "Id", "Izin", "Modul", "RolId" },
                values: new object[,]
                {
                    { 69, 1, 5, 5 },
                    { 70, 2, 5, 5 },
                    { 71, 1, 6, 5 },
                    { 72, 2, 6, 5 },
                    { 73, 1, 13, 6 },
                    { 74, 2, 13, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 48,
                column: "Modul",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 49,
                column: "Modul",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 50,
                columns: new[] { "Modul", "RolId" },
                values: new object[] { 1, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 51,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 1, 3, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 52,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 2, 3 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 53,
                column: "Izin",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 54,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 1, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 55,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 2, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 56,
                column: "Izin",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 57,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 1, 5 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 58,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 2, 5 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 59,
                columns: new[] { "Modul", "RolId" },
                values: new object[] { 1, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 60,
                columns: new[] { "Izin", "RolId" },
                values: new object[] { 1, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 61,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 2, 5, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 62,
                columns: new[] { "Izin", "RolId" },
                values: new object[] { 1, 4 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 63,
                columns: new[] { "Izin", "Modul" },
                values: new object[] { 2, 6 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 64,
                columns: new[] { "Modul", "RolId" },
                values: new object[] { 2, 5 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "Izin", "RolId" },
                values: new object[] { 1, 5 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 2, 5, 5 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 1, 13, 6 });

            migrationBuilder.UpdateData(
                table: "Yetkiler",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "Izin", "Modul", "RolId" },
                values: new object[] { 2, 13, 6 });
        }
    }
}
