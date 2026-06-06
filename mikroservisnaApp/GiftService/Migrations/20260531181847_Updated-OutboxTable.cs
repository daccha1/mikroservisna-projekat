using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiftService.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOutboxTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "SuccessfulCreation",
                table: "GiftCreatedOutboxTable",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuccessfulCreation",
                table: "GiftCreatedOutboxTable");
        }
    }
}
