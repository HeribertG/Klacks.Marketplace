// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service for feature plugin search, creation, update, download and user-specific listing.
/// </summary>
/// <param name="db">Database context for accessing feature plugin tables</param>
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

public class FeaturePluginMarketplaceService : IFeaturePluginMarketplaceService
{
    private readonly MarketplaceDbContext _db;

    public FeaturePluginMarketplaceService(MarketplaceDbContext db)
    {
        _db = db;
    }

    public async Task<(List<FeaturePlugin> Items, int TotalCount)> SearchPluginsAsync(string? search, string? category, PackageStatus? status, int page, int pageSize)
    {
        var query = _db.FeaturePlugins.Include(p => p.Author).AsQueryable();

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }
        else
        {
            query = query.Where(p => p.Status == PackageStatus.Published);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLowerInvariant();
            query = query.Where(p =>
                p.DisplayName.ToLower().Contains(searchLower) ||
                p.Name.ToLower().Contains(searchLower) ||
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

    public async Task<FeaturePlugin?> GetPluginByNameAsync(string name)
    {
        return await _db.FeaturePlugins
            .Include(p => p.Author)
            .Include(p => p.Versions.OrderByDescending(v => v.CreatedAt))
            .FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<FeaturePlugin> CreatePluginAsync(FeaturePlugin plugin, string manifestJson, string? i18nJson, byte[]? bundleData, string? changeLog)
    {
        var existing = await _db.FeaturePlugins.FirstOrDefaultAsync(p => p.Name == plugin.Name && p.AuthorId == plugin.AuthorId);
        if (existing is not null)
        {
            throw new InvalidOperationException($"You already have a plugin with name '{plugin.Name}'");
        }

        if (plugin.Status == PackageStatus.Draft)
        {
            plugin.Status = PackageStatus.PendingReview;
        }
        plugin.CreatedAt = DateTime.UtcNow;
        plugin.UpdatedAt = DateTime.UtcNow;

        var version = new FeaturePluginVersion
        {
            Version = plugin.Version,
            ManifestJson = manifestJson,
            I18nJson = i18nJson ?? string.Empty,
            ChangeLog = changeLog ?? string.Empty,
            BundleData = bundleData ?? [],
            Status = PackageStatus.PendingReview,
            CreatedAt = DateTime.UtcNow
        };
        plugin.Versions.Add(version);

        _db.FeaturePlugins.Add(plugin);
        await _db.SaveChangesAsync();
        return plugin;
    }

    public async Task<FeaturePlugin> UpdatePluginAsync(string name, FeaturePlugin updated, string manifestJson, string? i18nJson, byte[]? bundleData, string? changeLog)
    {
        var existing = await _db.FeaturePlugins
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Name == name);

        if (existing is null)
        {
            throw new InvalidOperationException($"Plugin with name '{name}' not found");
        }

        existing.DisplayName = updated.DisplayName;
        existing.Category = updated.Category;
        existing.Version = updated.Version;
        existing.Description = updated.Description;
        existing.MinKlacksVersion = updated.MinKlacksVersion;
        existing.RequiredPermissionsJson = updated.RequiredPermissionsJson;
        existing.ProvidedSkillsJson = updated.ProvidedSkillsJson;
        existing.UpdatedAt = DateTime.UtcNow;

        var version = new FeaturePluginVersion
        {
            Version = updated.Version,
            ManifestJson = manifestJson,
            I18nJson = i18nJson ?? string.Empty,
            ChangeLog = changeLog ?? string.Empty,
            BundleData = bundleData ?? [],
            Status = existing.Status,
            CreatedAt = DateTime.UtcNow
        };
        existing.Versions.Add(version);

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<byte[]> DownloadPluginAsync(string name, string ipAddress)
    {
        var plugin = await _db.FeaturePlugins
            .Include(p => p.Versions.OrderByDescending(v => v.CreatedAt).Take(1))
            .FirstOrDefaultAsync(p => p.Name == name && p.Status == PackageStatus.Published);

        if (plugin is null)
        {
            throw new InvalidOperationException("Plugin not found or not published");
        }

        var latestVersion = plugin.Versions.FirstOrDefault();
        if (latestVersion is null)
        {
            throw new InvalidOperationException("No version available");
        }

        plugin.Downloads++;
        _db.PluginDownloadLogs.Add(new PluginDownloadLog
        {
            PluginId = plugin.Id,
            DownloadedAt = DateTime.UtcNow,
            IpAddress = ipAddress
        });
        await _db.SaveChangesAsync();

        return latestVersion.BundleData;
    }

    public async Task<List<FeaturePlugin>> GetUserPluginsAsync(int userId)
    {
        return await _db.FeaturePlugins
            .Include(p => p.Author)
            .Where(p => p.AuthorId == userId)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();
    }
}
