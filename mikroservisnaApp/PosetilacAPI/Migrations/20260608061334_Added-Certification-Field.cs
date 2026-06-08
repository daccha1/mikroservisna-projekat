using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedCertificationField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Certificate",
                table: "Posetilac",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 1,
                column: "Certificate",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 2,
                column: "Certificate",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 3,
                column: "Certificate",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 4,
                column: "Certificate",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 5,
                column: "Certificate",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "Posetilac");
        }
    }
}
