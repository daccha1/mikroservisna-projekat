using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace mikroservisnaApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Lokacija",
                columns: new[] { "Id", "Adresa", "Kapacitet", "Naziv" },
                values: new object[,]
                {
                    { 1, "Veljka Dugoševića 54, Beograd", 300, "Tehnološki park Beograd" },
                    { 2, "Bulevar Oslobođenja 12, Novi Sad", 150, "Novi Sad IT Hub" },
                    { 3, "Nemanjina 4, Beograd", 80, "Smart City Centar" }
                });

            migrationBuilder.InsertData(
                table: "Organizator",
                columns: new[] { "Id", "Ime", "Prezime" },
                values: new object[,]
                {
                    { 1, "Marko", "Nikolić" },
                    { 2, "Ana", "Jovanović" },
                    { 3, "Stefan", "Petrović" }
                });

            migrationBuilder.InsertData(
                table: "Predavac",
                columns: new[] { "Id", "Email", "Ime", "OblastStrucnosti", "Password", "Prezime", "Titula" },
                values: new object[,]
                {
                    { 1, "nikola.djordjevic@gmail.com", "Nikola", "Veštačka inteligencija", "hashed_pass_1", "Đorđević", "Dr." },
                    { 2, "jelena.stojanovic@gmail.com", "Jelena", "Web razvoj", "hashed_pass_2", "Stojanović", "Prof." },
                    { 3, "milan.vasic@gmail.com", "Milan", "Baze podataka", "hashed_pass_3", "Vasić", "Mr." },
                    { 4, "ivana.lukic@gmail.com", "Ivana", "Kibernetička bezbednost", "hashed_pass_4", "Lukić", "Dr." }
                });

            migrationBuilder.InsertData(
                table: "TipDogadjaja",
                columns: new[] { "Id", "Naziv" },
                values: new object[,]
                {
                    { 1, "Konferencija" },
                    { 2, "Radionica" },
                    { 3, "Seminar" },
                    { 4, "Webinar" }
                });

            migrationBuilder.InsertData(
                table: "Dogadjaj",
                columns: new[] { "Id", "Agenda", "Cena", "DatumVreme", "LokacijaId", "Naziv", "OrganizatorId", "TipId", "Trajanje" },
                values: new object[,]
                {
                    { 1, "Panel diskusije o primeni AI u industriji, radionice, networking", 4999.9899999999998, new DateTime(2025, 9, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, "AI Summit Srbija 2025", 1, 1, 480 },
                    { 2, "React, Next.js, moderne prakse u web razvoju", 2499.9899999999998, new DateTime(2025, 10, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), 2, "Web Dev Radionica", 2, 2, 240 },
                    { 3, "Query optimizacija, indeksiranje, NoSQL vs SQL", 1999.99, new DateTime(2025, 11, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), 3, "Baze Podataka — napredne tehnike", 3, 3, 180 }
                });

            migrationBuilder.InsertData(
                table: "Dogadjaj_Predavac",
                columns: new[] { "Id", "PredavacId", "RasporedPredavanja", "StrucniDogadjajId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 9, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 4, new DateTime(2025, 9, 15, 13, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 2, new DateTime(2025, 10, 5, 9, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, 3, new DateTime(2025, 11, 20, 11, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Dogadjaj_Predavac",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dogadjaj_Predavac",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dogadjaj_Predavac",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Dogadjaj_Predavac",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TipDogadjaja",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Dogadjaj",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Dogadjaj",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Dogadjaj",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Predavac",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Predavac",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Predavac",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Predavac",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Lokacija",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Lokacija",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Lokacija",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Organizator",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Organizator",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Organizator",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TipDogadjaja",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TipDogadjaja",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TipDogadjaja",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
