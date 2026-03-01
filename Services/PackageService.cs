// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using System.Text;
using System.Text.Json;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

public class PackageService : IPackageService
{
    private readonly MarketplaceDbContext _db;

    public PackageService(MarketplaceDbContext db)
    {
        _db = db;
    }

    public async Task<(List<LanguagePackage> Items, int TotalCount)> SearchPackagesAsync(string? search, PackageStatus? status, int page, int pageSize)
    {
        var query = _db.Packages.Include(p => p.Author).AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }
        else
        {
            query = query.Where(p => p.Status == PackageStatus.Published);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            query = query.Where(p =>
                p.DisplayName.ToLower().Contains(searchLower) ||
                p.Code.ToLower().Contains(searchLower) ||
                p.Description.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(p => p.Downloads)
            .ThenByDescending(p => p.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<LanguagePackage?> GetPackageByCodeAsync(string code)
    {
        return await _db.Packages
            .Include(p => p.Author)
            .Include(p => p.Versions.OrderByDescending(v => v.CreatedAt))
            .FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<LanguagePackage> CreatePackageAsync(LanguagePackage package, string manifestJson, string translationsJson)
    {
        var existing = await _db.Packages.FirstOrDefaultAsync(p => p.Code == package.Code && p.AuthorId == package.AuthorId);
        if (existing is not null)
        {
            throw new InvalidOperationException($"You already have a package with code '{package.Code}'");
        }

        using var translationsDoc = JsonDocument.Parse(translationsJson);
        package.TranslationCount = translationsDoc.RootElement.EnumerateObject().Count();
        package.Status = PackageStatus.PendingReview;
        package.CreatedAt = DateTime.UtcNow;
        package.UpdatedAt = DateTime.UtcNow;

        var version = new PackageVersion
        {
            Version = package.Version,
            ManifestJson = manifestJson,
            TranslationsJson = translationsJson,
            Status = PackageStatus.PendingReview,
            CreatedAt = DateTime.UtcNow
        };
        package.Versions.Add(version);

        _db.Packages.Add(package);
        await _db.SaveChangesAsync();
        return package;
    }

    public async Task<byte[]> DownloadPackageAsync(string code, string ipAddress)
    {
        var package = await _db.Packages
            .Include(p => p.Versions.OrderByDescending(v => v.CreatedAt).Take(1))
            .FirstOrDefaultAsync(p => p.Code == code && p.Status == PackageStatus.Published);

        if (package is null)
        {
            throw new InvalidOperationException("Package not found or not published");
        }

        var latestVersion = package.Versions.FirstOrDefault();
        if (latestVersion is null)
        {
            throw new InvalidOperationException("No version available");
        }

        package.Downloads++;
        _db.DownloadLogs.Add(new DownloadLog
        {
            PackageId = package.Id,
            DownloadedAt = DateTime.UtcNow,
            IpAddress = ipAddress
        });
        await _db.SaveChangesAsync();

        var bundle = new
        {
            manifest = JsonSerializer.Deserialize<JsonElement>(latestVersion.ManifestJson),
            translations = JsonSerializer.Deserialize<JsonElement>(latestVersion.TranslationsJson)
        };

        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bundle, new JsonSerializerOptions { WriteIndented = true }));
    }

    public async Task<List<LanguagePackage>> GetUserPackagesAsync(int userId)
    {
        return await _db.Packages
            .Include(p => p.Author)
            .Where(p => p.AuthorId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();
    }
}
