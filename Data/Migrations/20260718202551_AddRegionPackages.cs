using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Klacks.Marketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionPackages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegionPackages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CountryName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    MinKlacksVersion = table.Column<string>(type: "TEXT", nullable: false),
                    Downloads = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionPackages_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RegionDownloadLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegionPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    ArtifactType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Industry = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionDownloadLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionDownloadLogs_RegionPackages_RegionPackageId",
                        column: x => x.RegionPackageId,
                        principalTable: "RegionPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegionPackageVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegionPackageId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    ProfileJson = table.Column<string>(type: "TEXT", nullable: false),
                    ChangeLog = table.Column<string>(type: "TEXT", nullable: false),
                    ContentHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    IsSeeded = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegionPackageVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegionPackageVersions_RegionPackages_RegionPackageId",
                        column: x => x.RegionPackageId,
                        principalTable: "RegionPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegionDownloadLogs_RegionPackageId",
                table: "RegionDownloadLogs",
                column: "RegionPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionPackages_AuthorId",
                table: "RegionPackages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegionPackages_CountryCode",
                table: "RegionPackages",
                column: "CountryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegionPackageVersions_RegionPackageId",
                table: "RegionPackageVersions",
                column: "RegionPackageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegionDownloadLogs");

            migrationBuilder.DropTable(
                name: "RegionPackageVersions");

            migrationBuilder.DropTable(
                name: "RegionPackages");
        }
    }
}
