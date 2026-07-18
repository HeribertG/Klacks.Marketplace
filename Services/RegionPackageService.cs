// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service for region package search, creation, versioning, profile download and user-specific listing.
/// </summary>
/// <param name="db">Database context for accessing region package tables</param>
using System.Security.Cryptography;
using System.Text;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

public class RegionPackageService : IRegionPackageService
{
    private readonly MarketplaceDbContext _db;

    public RegionPackageService(MarketplaceDbContext db)
    {
        _db = db;
    }

    public async Task<(List<RegionPackage> Items, int TotalCount)> SearchRegionPackagesAsync(string? search, string? countryCode, PackageStatus? status, int page, int pageSize)
    {
        page = Math.Max(page, AppConstants.MinSearchPage);
        pageSize = Math.Clamp(pageSize, AppConstants.MinSearchPageSize, AppConstants.MaxSearchPageSize);

        var query = _db.RegionPackages.Include(p => p.Author).AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }
        else
        {
            query = query.Where(p => p.Status == PackageStatus.Published);
        }

        if (!string.IsNullOrWhiteSpace(countryCode))
        {
            var codeLower = countryCode.ToLowerInvariant();
            query = query.Where(p => p.CountryCode == codeLower);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            query = query.Where(p =>
                p.CountryName.ToLower().Contains(searchLower) ||
                p.CountryCode.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(p => p.CountryName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<RegionPackage?> GetRegionPackageByCountryAsync(string countryCode)
    {
        var codeLower = countryCode.ToLowerInvariant();
        return await _db.RegionPackages
            .Include(p => p.Author)
            .Include(p => p.Versions.OrderByDescending(v => v.CreatedAt))
            .FirstOrDefaultAsync(p => p.CountryCode == codeLower);
    }

    public async Task<RegionPackageVersion?> GetLatestPublishedVersionAsync(string countryCode)
    {
        var codeLower = countryCode.ToLowerInvariant();
        return await _db.RegionPackageVersions
            .Where(v => v.RegionPackage.CountryCode == codeLower && v.Status == PackageStatus.Published)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<RegionPackage> CreateRegionPackageAsync(RegionPackage package, string profileJson, string? changeLog)
    {
        package.CountryCode = package.CountryCode.ToLowerInvariant();

        if (!RegionCountry.IsValidCountryCode(package.CountryCode))
        {
            throw new ArgumentException("Invalid country code. Expected a two-letter ISO code.", nameof(package));
        }

        var existing = await _db.RegionPackages.FirstOrDefaultAsync(p => p.CountryCode == package.CountryCode);
        if (existing is not null)
        {
            throw new InvalidOperationException($"A region package for country '{package.CountryCode}' already exists");
        }

        if (package.Status == PackageStatus.Draft)
        {
            package.Status = PackageStatus.PendingReview;
        }
        package.CreatedAt = DateTime.UtcNow;
        package.UpdatedAt = DateTime.UtcNow;

        var version = new RegionPackageVersion
        {
            Version = package.Version,
            ProfileJson = profileJson,
            ChangeLog = changeLog ?? string.Empty,
            ContentHash = ComputeContentHash(profileJson),
            Status = package.Status == PackageStatus.Published ? PackageStatus.Published : PackageStatus.PendingReview,
            CreatedAt = DateTime.UtcNow
        };
        package.Versions.Add(version);

        _db.RegionPackages.Add(package);
        await _db.SaveChangesAsync();
        return package;
    }

    public async Task<RegionPackage> UpdateRegionPackageAsync(string countryCode, RegionPackage updated, string profileJson, string? changeLog, PackageStatus versionStatus)
    {
        var codeLower = countryCode.ToLowerInvariant();
        var existing = await _db.RegionPackages
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.CountryCode == codeLower);

        if (existing is null)
        {
            throw new InvalidOperationException($"Region package for country '{countryCode}' not found");
        }

        existing.Version = updated.Version;
        existing.Description = updated.Description;
        existing.MinKlacksVersion = updated.MinKlacksVersion;
        existing.UpdatedAt = DateTime.UtcNow;

        if (versionStatus == PackageStatus.PendingReview)
        {
            existing.Status = PackageStatus.PendingReview;
        }

        var version = new RegionPackageVersion
        {
            Version = updated.Version,
            ProfileJson = profileJson,
            ChangeLog = changeLog ?? string.Empty,
            ContentHash = ComputeContentHash(profileJson),
            Status = versionStatus,
            CreatedAt = DateTime.UtcNow
        };
        existing.Versions.Add(version);

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<(string ProfileJson, string Version)> DownloadRegionProfileAsync(string countryCode, string industry, string artifactType, string ipAddress)
    {
        var codeLower = countryCode.ToLowerInvariant();
        var package = await _db.RegionPackages
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.CountryCode == codeLower && p.Status == PackageStatus.Published);

        if (package is null)
        {
            throw new InvalidOperationException("Region package not found or not published");
        }

        var latestPublished = package.Versions
            .Where(v => v.Status == PackageStatus.Published)
            .OrderByDescending(v => v.CreatedAt)
            .FirstOrDefault();

        if (latestPublished is null)
        {
            throw new InvalidOperationException("No published version available");
        }

        package.Downloads++;
        _db.RegionDownloadLogs.Add(new RegionDownloadLog
        {
            RegionPackageId = package.Id,
            Version = latestPublished.Version,
            ArtifactType = artifactType,
            Industry = industry,
            DownloadedAt = DateTime.UtcNow,
            IpAddress = ipAddress
        });
        await _db.SaveChangesAsync();

        return (latestPublished.ProfileJson, latestPublished.Version);
    }

    public async Task<List<RegionPackage>> GetUserRegionPackagesAsync(int userId)
    {
        return await _db.RegionPackages
            .Include(p => p.Author)
            .Where(p => p.AuthorId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();
    }

    public static string ComputeContentHash(string content)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(content));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
