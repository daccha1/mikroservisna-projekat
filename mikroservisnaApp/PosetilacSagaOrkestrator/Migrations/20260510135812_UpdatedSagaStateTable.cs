using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacSagaOrkestrator.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSagaStateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Interesovanje",
                table: "PosetilacSagaStates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Interesovanje",
                table: "PosetilacSagaStates");
        }
    }
}
