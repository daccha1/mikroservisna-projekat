using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacSagaOrkestrator.Migrations
{
    /// <inheritdoc />
    public partial class FixedTransactionState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedService",
                table: "TransactionConfirmationOutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailedService",
                table: "TransactionConfirmationOutboxMessages");
        }
    }
}
