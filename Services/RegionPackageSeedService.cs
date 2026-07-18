// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Idempotent startup seeder that imports the bundled region profile JSON files as published region packages.
/// New countries are created as version 1.0.0; changed bundled content advances only seeded versions with a patch bump.
/// </summary>
/// <param name="db">Database context for region package tables</param>
/// <param name="environment">Host environment used to resolve the bundled profile directory</param>
/// <param name="logger">Logger for seeding progress and skip reasons</param>
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

public class RegionPackageSeedService : IRegionPackageSeedService
{
    private const string ProfileFileSearchPattern = "*.json";
    private const string SeedChangeLog = "Seeded from bundled region profile";

    private readonly MarketplaceDbContext _db;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<RegionPackageSeedService> _logger;

    public RegionPackageSeedService(
        MarketplaceDbContext db,
        IWebHostEnvironment environment,
        ILogger<RegionPackageSeedService> logger)
    {
        _db = db;
        _environment = environment;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var profilesDirectory = Path.Combine(_environment.ContentRootPath, AppConstants.RegionProfilesDirectory);
        if (!Directory.Exists(profilesDirectory))
        {
            _logger.LogWarning("Region profile directory not found, skipping region package seeding: {Directory}", profilesDirectory);
            return;
        }

        var adminUser = await _db.Users
            .Where(u => u.IsAdmin)
            .OrderBy(u => u.Id)
            .FirstOrDefaultAsync();

        if (adminUser is null)
        {
            _logger.LogWarning("No admin user exists, skipping region package seeding");
            return;
        }

        foreach (var filePath in Directory.GetFiles(profilesDirectory, ProfileFileSearchPattern).OrderBy(f => f))
        {
            var countryCode = Path.GetFileNameWithoutExtension(filePath).ToLowerInvariant();
            var profileJson = await File.ReadAllTextAsync(filePath);
            var contentHash = RegionPackageService.ComputeContentHash(profileJson);

            var package = await _db.RegionPackages
                .Include(p => p.Versions)
                .FirstOrDefaultAsync(p => p.CountryCode == countryCode);

            if (package is null)
            {
                await CreateSeededPackageAsync(countryCode, profileJson, contentHash, adminUser.Id);
                continue;
            }

            await UpdateSeededPackageAsync(package, profileJson, contentHash);
        }
    }

    private async Task CreateSeededPackageAsync(string countryCode, string profileJson, string contentHash, int authorId)
    {
        var countryName = ResolveCountryName(countryCode);

        var package = new RegionPackage
        {
            CountryCode = countryCode,
            CountryName = countryName,
            Version = AppConstants.RegionSeedVersion,
            AuthorId = authorId,
            Description = $"Official Klacks region setup profile for {countryName}. Pre-configures languages, locale, holiday calendar, working-time limits, surcharges, compliance rules and payroll export for a fresh installation.",
            Status = PackageStatus.Published,
            MinKlacksVersion = AppConstants.RegionSeedMinKlacksVersion,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        package.Versions.Add(new RegionPackageVersion
        {
            Version = AppConstants.RegionSeedVersion,
            ProfileJson = profileJson,
            ChangeLog = SeedChangeLog,
            ContentHash = contentHash,
            IsSeeded = true,
            Status = PackageStatus.Published,
            CreatedAt = DateTime.UtcNow
        });

        _db.RegionPackages.Add(package);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded region package {CountryCode} at version {Version}", countryCode, AppConstants.RegionSeedVersion);
    }

    private async Task UpdateSeededPackageAsync(RegionPackage package, string profileJson, string contentHash)
    {
        var latestVersion = package.Versions
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefault();

        if (latestVersion is null || latestVersion.ContentHash == contentHash)
        {
            return;
        }

        if (!latestVersion.IsSeeded)
        {
            _logger.LogInformation("Latest version of region package {CountryCode} was uploaded manually, skipping seeded update", package.CountryCode);
            return;
        }

        var newVersion = BumpPatchVersion(latestVersion.Version);
        package.Versions.Add(new RegionPackageVersion
        {
            Version = newVersion,
            ProfileJson = profileJson,
            ChangeLog = SeedChangeLog,
            ContentHash = contentHash,
            IsSeeded = true,
            Status = PackageStatus.Published,
            CreatedAt = DateTime.UtcNow
        });

        package.Version = newVersion;
        package.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        _logger.LogInformation("Advanced seeded region package {CountryCode} to version {Version}", package.CountryCode, newVersion);
    }

    private static string ResolveCountryName(string countryCode)
    {
        return RegionCountry.CountryNames.TryGetValue(countryCode, out var name)
            ? name
            : countryCode.ToUpperInvariant();
    }

    private static string BumpPatchVersion(string version)
    {
        if (System.Version.TryParse(version, out var parsed) && parsed.Build >= 0)
        {
            return $"{parsed.Major}.{parsed.Minor}.{parsed.Build + 1}";
        }

        return $"{AppConstants.RegionSeedVersion.Split('.')[0]}.0.1";
    }
}
