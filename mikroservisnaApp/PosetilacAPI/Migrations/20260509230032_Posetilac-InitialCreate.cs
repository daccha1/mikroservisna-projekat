using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PosetilacAPI.Migrations
{
    /// <inheritdoc />
    public partial class PosetilacInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posetilac",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posetilac", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Posetilac",
                columns: new[] { "Id", "Ime", "Prezime" },
                values: new object[,]
                {
                    { 1, "Petar", "Ilić" },
                    { 2, "Milica", "Stanković" },
                    { 3, "Jovan", "Marković" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posetilac");
        }
    }
}
