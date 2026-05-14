using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedOutboxState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutboxState",
                table: "SagaResultOutbox",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutboxState",
                table: "SagaResultOutbox");
        }
    }
}
