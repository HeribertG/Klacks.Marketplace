using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Klacks.Marketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguagePackExtraFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExtraFilesJson",
                table: "PackageVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraFilesJson",
                table: "PackageVersions");
        }
    }
}
