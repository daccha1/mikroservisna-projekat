using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PosetilacAPI.Migrations
{
    /// <inheritdoc />
    public partial class PosetilacAddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DogadjajId",
                table: "Posetilac",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Interesovanje",
                table: "Posetilac",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatusZaposlenja",
                table: "Posetilac",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "DogadjajId", "Interesovanje", "StatusZaposlenja" },
                values: new object[] { 1, "Vestacka Inteligencija", "stalnom radnom odnosu" });

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "DogadjajId", "Interesovanje", "StatusZaposlenja" },
                values: new object[] { 2, "Web development", "student" });

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "DogadjajId", "Interesovanje", "StatusZaposlenja" },
                values: new object[] { 3, "Machine Learning", "nezaposlen" });

            migrationBuilder.InsertData(
                table: "Posetilac",
                columns: new[] { "Id", "DogadjajId", "Ime", "Interesovanje", "Prezime", "StatusZaposlenja" },
                values: new object[,]
                {
                    { 4, 1, "Ana", "Embedded", "Pavlović", "stalnom radnom odnosu" },
                    { 5, 2, "Nikola", "Cloud computing", "Savić", "student" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "DogadjajId",
                table: "Posetilac");

            migrationBuilder.DropColumn(
                name: "Interesovanje",
                table: "Posetilac");

            migrationBuilder.DropColumn(
                name: "StatusZaposlenja",
                table: "Posetilac");
        }
    }
}
