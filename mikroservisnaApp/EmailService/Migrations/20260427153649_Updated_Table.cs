using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailService.Migrations
{
    /// <inheritdoc />
    public partial class Updated_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventType",
                table: "ProcessedMessages");

            migrationBuilder.RenameColumn(
                name: "JsonObject",
                table: "ProcessedMessages",
                newName: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "ProcessedMessages",
                newName: "JsonObject");

            migrationBuilder.AddColumn<int>(
                name: "EventType",
                table: "ProcessedMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
