using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrganizatorAPI.Migrations
{
    /// <inheritdoc />
    public partial class OrganizatorInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organizator",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizator", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Organizator",
                columns: new[] { "Id", "Email", "Ime", "Password", "Prezime" },
                values: new object[,]
                {
                    { 1, "marko.petrovic@email.com", "Marko", "password123", "Petrović" },
                    { 2, "ana.jovanovic@email.com", "Ana", "password123", "Jovanović" },
                    { 3, "stefan.nikolic@email.com", "Stefan", "password123", "Nikolić" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Organizator");
        }
    }
}
