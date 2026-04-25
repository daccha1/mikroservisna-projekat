using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mikroservisnaApp.Migrations
{
    /// <inheritdoc />
    public partial class Added_Outbox_Table_WithPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "OutboxTable",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Event",
                table: "OutboxTable",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxTable",
                table: "OutboxTable",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxTable",
                table: "OutboxTable");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "OutboxTable");

            migrationBuilder.DropColumn(
                name: "Event",
                table: "OutboxTable");
        }
    }
}
