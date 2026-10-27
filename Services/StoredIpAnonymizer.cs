// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

/// <summary>
/// One-time clean-up at startup: truncates the full IP addresses that older versions stored in the three download
/// log tables. Rows that are already truncated (IPv4 ending in ".0", IPv6 ending in "::") or hold no address are skipped,
/// so later runs find nothing to do.
/// </summary>
public static class StoredIpAnonymizer
{
    private const string Ipv4Suffix = ".0";
    private const string Ipv6Suffix = "::";

    /// <param name="db">Store database holding the three download log tables</param>
    /// <returns>Number of log rows that were truncated</returns>
    public static async Task<int> AnonymizeAsync(MarketplaceDbContext db)
    {
        var changed = 0;

        var packageLogs = await db.DownloadLogs
            .Where(l => !l.IpAddress.EndsWith(Ipv4Suffix) && !l.IpAddress.EndsWith(Ipv6Suffix) && l.IpAddress != AppConstants.UnknownClientIp)
            .ToListAsync();
        foreach (var log in packageLogs)
        {
            log.IpAddress = ClientIpAnonymizer.Anonymize(log.IpAddress);
        }

        var pluginLogs = await db.PluginDownloadLogs
            .Where(l => !l.IpAddress.EndsWith(Ipv4Suffix) && !l.IpAddress.EndsWith(Ipv6Suffix) && l.IpAddress != AppConstants.UnknownClientIp)
            .ToListAsync();
        foreach (var log in pluginLogs)
        {
            log.IpAddress = ClientIpAnonymizer.Anonymize(log.IpAddress);
        }

        var regionLogs = await db.RegionDownloadLogs
            .Where(l => !l.IpAddress.EndsWith(Ipv4Suffix) && !l.IpAddress.EndsWith(Ipv6Suffix) && l.IpAddress != AppConstants.UnknownClientIp)
            .ToListAsync();
        foreach (var log in regionLogs)
        {
            log.IpAddress = ClientIpAnonymizer.Anonymize(log.IpAddress);
        }

        changed = packageLogs.Count + pluginLogs.Count + regionLogs.Count;
        if (changed > 0)
        {
            await db.SaveChangesAsync();
        }

        return changed;
    }
}
