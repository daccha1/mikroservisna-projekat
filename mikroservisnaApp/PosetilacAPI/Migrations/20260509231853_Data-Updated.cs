using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacAPI.Migrations
{
    /// <inheritdoc />
    public partial class DataUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 1,
                column: "StatusZaposlenja",
                value: "Zaposlen");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 2,
                column: "StatusZaposlenja",
                value: "Student");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 3,
                column: "StatusZaposlenja",
                value: "Nezaposlen");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 4,
                column: "StatusZaposlenja",
                value: "Stalni radni odnos");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Interesovanje", "StatusZaposlenja" },
                values: new object[] { "Mikroservisi", "Student" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 1,
                column: "StatusZaposlenja",
                value: "stalnom radnom odnosu");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 2,
                column: "StatusZaposlenja",
                value: "student");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 3,
                column: "StatusZaposlenja",
                value: "nezaposlen");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 4,
                column: "StatusZaposlenja",
                value: "stalnom radnom odnosu");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Interesovanje", "StatusZaposlenja" },
                values: new object[] { "Cloud computing", "student" });
        }
    }
}
