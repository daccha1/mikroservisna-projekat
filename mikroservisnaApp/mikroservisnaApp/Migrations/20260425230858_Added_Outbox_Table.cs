using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mikroservisnaApp.Migrations
{
    /// <inheritdoc />
    public partial class Added_Outbox_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutboxTable",
                columns: table => new
                {
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxTable");
        }
    }
}
