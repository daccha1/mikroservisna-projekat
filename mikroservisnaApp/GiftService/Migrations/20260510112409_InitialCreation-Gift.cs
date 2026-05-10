using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreationGift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gifts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Prirucnik = table.Column<int>(type: "int", nullable: false),
                    Interesovanje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vaucer = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Instrukcije = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifts", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gifts");
        }
    }
}
