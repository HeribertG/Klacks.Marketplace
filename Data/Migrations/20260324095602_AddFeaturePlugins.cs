using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Klacks.Marketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeaturePlugins : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeaturePlugins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    MinKlacksVersion = table.Column<string>(type: "TEXT", nullable: false),
                    RequiredPermissionsJson = table.Column<string>(type: "TEXT", nullable: false),
                    ProvidedSkillsJson = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Downloads = table.Column<int>(type: "INTEGER", nullable: false),
                    ReadmeMarkdown = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturePlugins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturePlugins_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeaturePluginVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PluginId = table.Column<int>(type: "INTEGER", nullable: false),
                    Version = table.Column<string>(type: "TEXT", nullable: false),
                    ManifestJson = table.Column<string>(type: "TEXT", nullable: false),
                    I18nJson = table.Column<string>(type: "TEXT", nullable: false),
                    ChangeLog = table.Column<string>(type: "TEXT", nullable: false),
                    BundleData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturePluginVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturePluginVersions_FeaturePlugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "FeaturePlugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PluginDownloadLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PluginId = table.Column<int>(type: "INTEGER", nullable: false),
                    DownloadedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IpAddress = table.Column<string>(type: "TEXT", maxLength: 45, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PluginDownloadLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PluginDownloadLogs_FeaturePlugins_PluginId",
                        column: x => x.PluginId,
                        principalTable: "FeaturePlugins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeaturePlugins_AuthorId",
                table: "FeaturePlugins",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturePlugins_Name",
                table: "FeaturePlugins",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeaturePluginVersions_PluginId",
                table: "FeaturePluginVersions",
                column: "PluginId");

            migrationBuilder.CreateIndex(
                name: "IX_PluginDownloadLogs_PluginId",
                table: "PluginDownloadLogs",
                column: "PluginId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeaturePluginVersions");

            migrationBuilder.DropTable(
                name: "PluginDownloadLogs");

            migrationBuilder.DropTable(
                name: "FeaturePlugins");
        }
    }
}
