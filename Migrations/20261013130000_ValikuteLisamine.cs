using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FunktsiooniUurija.Migrations
{
    /// <inheritdoc />
    public partial class ValikuteLisamine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Lemmik",
                table: "FunktsiooniUurimised",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Valik",
                table: "FunktsiooniUurimised",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Lemmik",
                table: "FunktsiooniUurimised");

            migrationBuilder.DropColumn(
                name: "Valik",
                table: "FunktsiooniUurimised");
        }
    }
}
