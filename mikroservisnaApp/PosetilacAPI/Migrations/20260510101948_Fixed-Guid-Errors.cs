using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosetilacAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixedGuidErrors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 1,
                column: "CorrelationId",
                value: new Guid("c5db59d7-9e3d-48fd-89c5-e433b5eaf01a"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 2,
                column: "CorrelationId",
                value: new Guid("98230c8d-f5db-4202-9734-3385e7d11c1f"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 3,
                column: "CorrelationId",
                value: new Guid("945a2aea-0eca-4322-936c-0608b7142372"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 4,
                column: "CorrelationId",
                value: new Guid("9fa651d7-3dce-4c73-aa03-b0eca675b7de"));

            migrationBuilder.UpdateData(
                table: "Posetilac",
                keyColumn: "Id",
                keyValue: 5,
                column: "CorrelationId",
                value: new Guid("29dfd66c-644f-409e-8b2b-1dfc4a0cb386"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
