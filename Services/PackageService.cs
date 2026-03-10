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

    public async Task<LanguagePackage> CreatePackageAsync(LanguagePackage package, string manifestJson, string translationsJson, string? docsJson = null, string? countriesJson = null, string? statesJson = null, string? calendarRulesJson = null)
    {
        var existing = await _db.Packages.FirstOrDefaultAsync(p => p.Code == package.Code && p.AuthorId == package.AuthorId);
        if (existing is not null)
        {
            throw new InvalidOperationException($"You already have a package with code '{package.Code}'");
        }

        using var translationsDoc = JsonDocument.Parse(translationsJson);
        package.TranslationCount = translationsDoc.RootElement.EnumerateObject().Count();
        if (package.Status == PackageStatus.Draft)
        {
            package.Status = PackageStatus.PendingReview;
        }
        package.CreatedAt = DateTime.UtcNow;
        package.UpdatedAt = DateTime.UtcNow;

        var version = new PackageVersion
        {
            Version = package.Version,
            ManifestJson = manifestJson,
            TranslationsJson = translationsJson,
            DocsJson = docsJson ?? string.Empty,
            CountriesJson = countriesJson ?? string.Empty,
            StatesJson = statesJson ?? string.Empty,
            CalendarRulesJson = calendarRulesJson ?? string.Empty,
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

        var bundleDict = new Dictionary<string, JsonElement>
        {
            ["manifest"] = JsonSerializer.Deserialize<JsonElement>(latestVersion.ManifestJson),
            ["translations"] = JsonSerializer.Deserialize<JsonElement>(latestVersion.TranslationsJson)
        };

        if (!string.IsNullOrWhiteSpace(latestVersion.DocsJson))
        {
            bundleDict["docs"] = JsonSerializer.Deserialize<JsonElement>(latestVersion.DocsJson);
        }

        if (!string.IsNullOrWhiteSpace(latestVersion.CountriesJson))
        {
            bundleDict["countries"] = JsonSerializer.Deserialize<JsonElement>(latestVersion.CountriesJson);
        }

        if (!string.IsNullOrWhiteSpace(latestVersion.StatesJson))
        {
            bundleDict["states"] = JsonSerializer.Deserialize<JsonElement>(latestVersion.StatesJson);
        }

        if (!string.IsNullOrWhiteSpace(latestVersion.CalendarRulesJson))
        {
            bundleDict["calendarRules"] = JsonSerializer.Deserialize<JsonElement>(latestVersion.CalendarRulesJson);
        }

        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bundleDict, new JsonSerializerOptions { WriteIndented = true }));
    }

    public async Task<LanguagePackage> UpdatePackageAsync(string code, LanguagePackage updated, string manifestJson, string translationsJson, string? docsJson, string? countriesJson, string? statesJson, string? calendarRulesJson)
    {
        var existing = await _db.Packages
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Code == code);

        if (existing is null)
        {
            throw new InvalidOperationException($"Package with code '{code}' not found");
        }

        existing.Name = updated.Name;
        existing.DisplayName = updated.DisplayName;
        existing.SpeechLocale = updated.SpeechLocale;
        existing.Version = updated.Version;
        existing.Coverage = updated.Coverage;
        existing.Description = updated.Description;
        existing.MinKlacksVersion = updated.MinKlacksVersion;
        existing.UpdatedAt = DateTime.UtcNow;

        using var translationsDoc = JsonDocument.Parse(translationsJson);
        existing.TranslationCount = translationsDoc.RootElement.EnumerateObject().Count();

        var version = new PackageVersion
        {
            Version = updated.Version,
            ManifestJson = manifestJson,
            TranslationsJson = translationsJson,
            DocsJson = docsJson ?? string.Empty,
            CountriesJson = countriesJson ?? string.Empty,
            StatesJson = statesJson ?? string.Empty,
            CalendarRulesJson = calendarRulesJson ?? string.Empty,
            Status = existing.Status,
            CreatedAt = DateTime.UtcNow
        };
        existing.Versions.Add(version);

        await _db.SaveChangesAsync();
        return existing;
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
