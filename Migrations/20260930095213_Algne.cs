using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FunktsiooniUurija.Migrations
{
    /// <inheritdoc />
    public partial class Algne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FunktsiooniUurimised",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Valem = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Maaramispiirkond = table.Column<string>(type: "TEXT", nullable: false),
                    Nullkohad = table.Column<string>(type: "TEXT", nullable: false),
                    Tuletis = table.Column<string>(type: "TEXT", nullable: false),
                    KriitilisedPunktid = table.Column<string>(type: "TEXT", nullable: false),
                    Ekstreemumid = table.Column<string>(type: "TEXT", nullable: false),
                    LuodudAeg = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunktsiooniUurimised", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FunktsiooniUurimised",
                columns: new[] { "Id", "Ekstreemumid", "KriitilisedPunktid", "LuodudAeg", "Maaramispiirkond", "Nullkohad", "Tuletis", "Valem" },
                values: new object[,]
                {
                    { 1, "max: f(-1) = 2; min: f(1) = -2", "x = -1; x = 1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "(-∞; ∞)", "x = -1.7321; x = 0; x = 1.7321", "-3 + 3*x^2", "x^3 - 3*x" },
                    { 2, "min: f(2) = -1", "x = 2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "(-∞; ∞)", "x = 1; x = 3", "-4 + 2*x", "x^2 - 4*x + 3" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FunktsiooniUurimised");
        }
    }
}
