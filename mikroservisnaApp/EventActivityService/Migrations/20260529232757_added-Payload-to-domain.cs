using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventActivityService.Migrations
{
    /// <inheritdoc />
    public partial class addedPayloadtodomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Payload",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payload",
                table: "Events");
        }
    }
}
