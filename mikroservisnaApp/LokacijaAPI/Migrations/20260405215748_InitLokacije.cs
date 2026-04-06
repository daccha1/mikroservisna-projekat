using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LokacijaAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitLokacije : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lokacije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kapacitet = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lokacije", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Lokacije",
                columns: new[] { "Id", "Adresa", "Kapacitet", "Naziv" },
                values: new object[,]
                {
                    { 1, "Bulevar Mihajla Pupina 10, Beograd", 500, "Skladište Beograd" },
                    { 2, "Bulevar Oslobođenja 45, Novi Sad", 300, "Skladište Novi Sad" },
                    { 3, "Obrenovićeva 22, Niš", 200, "Skladište Niš" },
                    { 4, "Kralja Petra I 8, Kragujevac", 150, "Skladište Kragujevac" },
                    { 5, "Korzo 3, Subotica", 250, "Skladište Subotica" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lokacije");
        }
    }
}
