using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunktsiooniUurija.Migrations
{
    /// <inheritdoc />
    public partial class Laiendus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kaanupunkt",
                table: "FunktsiooniUurimised",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kahanemine",
                table: "FunktsiooniUurimised",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Kasvamine",
                table: "FunktsiooniUurimised",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LoigeYTeljega",
                table: "FunktsiooniUurimised",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "FunktsiooniUurimised",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Kaanupunkt", "Kahanemine", "Kasvamine", "LoigeYTeljega" },
                values: new object[] { "", "", "", "" });

            migrationBuilder.UpdateData(
                table: "FunktsiooniUurimised",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Kaanupunkt", "Kahanemine", "Kasvamine", "LoigeYTeljega" },
                values: new object[] { "", "", "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kaanupunkt",
                table: "FunktsiooniUurimised");

            migrationBuilder.DropColumn(
                name: "Kahanemine",
                table: "FunktsiooniUurimised");

            migrationBuilder.DropColumn(
                name: "Kasvamine",
                table: "FunktsiooniUurimised");

            migrationBuilder.DropColumn(
                name: "LoigeYTeljega",
                table: "FunktsiooniUurimised");
        }
    }
}
