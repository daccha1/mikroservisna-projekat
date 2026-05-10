using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedcorrelationidfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                table: "Posetilac",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 1,
                column: "CorrelationId",
                value: new Guid("d95a2dc4-fda5-4a8b-9e1d-55704f99bbe9"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 2,
                column: "CorrelationId",
                value: new Guid("1c4b43e9-683e-4b05-a517-a9f420839479"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 3,
                column: "CorrelationId",
                value: new Guid("3cb49b18-df27-4a66-ad21-87e8cf7b054e"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 4,
                column: "CorrelationId",
                value: new Guid("daa8e113-960c-4c1d-addc-2b4f396895b1"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 5,
                column: "CorrelationId",
                value: new Guid("f492bd80-4540-4d04-b85c-40b1234f1fec"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "Posetilac");
        }
    }
}
