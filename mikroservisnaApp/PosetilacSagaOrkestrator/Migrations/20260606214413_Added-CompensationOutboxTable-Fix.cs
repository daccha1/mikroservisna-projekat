using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacSagaOrkestrator.Migrations
{
    /// <inheritdoc />
    public partial class AddedCompensationOutboxTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CompensationOutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CompensationOutboxMessages");
        }
    }
}
