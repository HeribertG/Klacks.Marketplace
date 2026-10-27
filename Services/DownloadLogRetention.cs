// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

/// <summary>
/// Deletes download log rows older than the retention period (AppConstants.DownloadLogRetentionDays) from the three
/// download log tables. Runs once at startup; the per-package counters and the 90-day admin statistics are not affected.
/// </summary>
public static class DownloadLogRetention
{
    /// <param name="db">Store database holding the three download log tables</param>
    /// <returns>Number of deleted log rows</returns>
    public static async Task<int> PurgeAsync(MarketplaceDbContext db)
    {
        var cutoff = DateTime.UtcNow.AddDays(-AppConstants.DownloadLogRetentionDays);

        var deleted = await db.DownloadLogs.Where(l => l.DownloadedAt < cutoff).ExecuteDeleteAsync();
        deleted += await db.PluginDownloadLogs.Where(l => l.DownloadedAt < cutoff).ExecuteDeleteAsync();
        deleted += await db.RegionDownloadLogs.Where(l => l.DownloadedAt < cutoff).ExecuteDeleteAsync();

        return deleted;
    }
}
