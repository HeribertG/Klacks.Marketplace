// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;
using Klacks.Marketplace.Services;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// Startup clean-up of the download logs: old rows lose their full IP address once, and rows older than the retention
/// period are deleted.
/// </summary>
[TestFixture]
public class DownloadLogMaintenanceTests
{
    [Test]
    public async Task StoredIpAnonymizer_truncates_full_addresses_once_and_leaves_the_rest()
    {
        using var database = await TestDatabase.CreateAsync();
        var db = database.Context;
        var package = await AddPackageAsync(db);
        db.DownloadLogs.AddRange(
            new DownloadLog { PackageId = package.Id, IpAddress = "203.0.113.57" },
            new DownloadLog { PackageId = package.Id, IpAddress = "2001:db8:abcd:1234::1" },
            new DownloadLog { PackageId = package.Id, IpAddress = AppConstants.UnknownClientIp },
            new DownloadLog { PackageId = package.Id, IpAddress = "198.51.100.0" });
        await db.SaveChangesAsync();

        var changed = await StoredIpAnonymizer.AnonymizeAsync(db);

        changed.ShouldBe(2);
        var stored = await db.DownloadLogs.OrderBy(l => l.Id).Select(l => l.IpAddress).ToListAsync();
        stored.ShouldBe(["203.0.113.0", "2001:db8:abcd::", AppConstants.UnknownClientIp, "198.51.100.0"]);
        (await StoredIpAnonymizer.AnonymizeAsync(db)).ShouldBe(0);
    }

    [Test]
    public async Task DownloadLogRetention_deletes_only_rows_older_than_the_retention_period()
    {
        using var database = await TestDatabase.CreateAsync();
        var db = database.Context;
        var package = await AddPackageAsync(db);
        db.DownloadLogs.AddRange(
            new DownloadLog { PackageId = package.Id, IpAddress = "203.0.113.0", DownloadedAt = DateTime.UtcNow.AddDays(-(AppConstants.DownloadLogRetentionDays + 1)) },
            new DownloadLog { PackageId = package.Id, IpAddress = "203.0.113.0", DownloadedAt = DateTime.UtcNow.AddDays(-(AppConstants.DownloadLogRetentionDays - 1)) },
            new DownloadLog { PackageId = package.Id, IpAddress = "203.0.113.0", DownloadedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();

        var deleted = await DownloadLogRetention.PurgeAsync(db);

        deleted.ShouldBe(1);
        (await db.DownloadLogs.CountAsync()).ShouldBe(2);
        (await DownloadLogRetention.PurgeAsync(db)).ShouldBe(0);
    }

    private static async Task<LanguagePackage> AddPackageAsync(Klacks.Marketplace.Data.MarketplaceDbContext db)
    {
        var author = new User { Email = "author@test.invalid", PasswordHash = "x", DisplayName = "Author" };
        db.Users.Add(author);
        await db.SaveChangesAsync();

        var package = new LanguagePackage { Code = "xx", Name = "Test", AuthorId = author.Id };
        db.Packages.Add(package);
        await db.SaveChangesAsync();

        return package;
    }
}
