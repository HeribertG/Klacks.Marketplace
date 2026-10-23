using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Klacks.Marketplace.Data.Migrations
{
    /// <inheritdoc />
    public partial class PruneLanguagePackageVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "PackageVersions"
                WHERE "Id" NOT IN (
                    SELECT (
                        SELECT v."Id" FROM "PackageVersions" v
                        WHERE v."PackageId" = p."PackageId"
                        ORDER BY v."CreatedAt" DESC, v."Id" DESC
                        LIMIT 1)
                    FROM (SELECT DISTINCT "PackageId" FROM "PackageVersions") p);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
