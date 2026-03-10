using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Klacks.Marketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDocsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocsJson",
                table: "PackageVersions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocsJson",
                table: "PackageVersions");
        }
    }
}
