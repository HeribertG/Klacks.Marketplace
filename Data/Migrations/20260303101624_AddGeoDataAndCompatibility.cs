using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Klacks.Marketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGeoDataAndCompatibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CalendarRulesJson",
                table: "PackageVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountriesJson",
                table: "PackageVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatesJson",
                table: "PackageVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MinKlacksVersion",
                table: "Packages",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendarRulesJson",
                table: "PackageVersions");

            migrationBuilder.DropColumn(
                name: "CountriesJson",
                table: "PackageVersions");

            migrationBuilder.DropColumn(
                name: "StatesJson",
                table: "PackageVersions");

            migrationBuilder.DropColumn(
                name: "MinKlacksVersion",
                table: "Packages");
        }
    }
}
