using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventActivityService.Migrations
{
    /// <inheritdoc />
    public partial class SnapshotTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentHall = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContactedCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CheckedInAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckedOutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Snapshots");
        }
    }
}
