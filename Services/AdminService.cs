// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

public class AdminService : IAdminService
{
    private readonly MarketplaceDbContext _db;

    public AdminService(MarketplaceDbContext db)
    {
        _db = db;
    }

    public async Task<List<LanguagePackage>> GetPendingPackagesAsync()
    {
        return await _db.Packages
            .Include(p => p.Author)
            .Where(p => p.Status == PackageStatus.PendingReview)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task ApprovePackageAsync(int packageId)
    {
        var package = await _db.Packages
            .Include(p => p.Versions)
            .FirstOrDefaultAsync(p => p.Id == packageId);

        if (package is null)
        {
            throw new InvalidOperationException("Package not found");
        }

        package.Status = PackageStatus.Published;
        package.UpdatedAt = DateTime.UtcNow;

        foreach (var version in package.Versions.Where(v => v.Status == PackageStatus.PendingReview))
        {
            version.Status = PackageStatus.Published;
        }

        await _db.SaveChangesAsync();
    }

    public async Task RejectPackageAsync(int packageId, string reason)
    {
        var package = await _db.Packages.FindAsync(packageId);
        if (package is null)
        {
            throw new InvalidOperationException("Package not found");
        }

        package.Status = PackageStatus.Rejected;
        package.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        return await _db.Users.OrderBy(u => u.DisplayName).ToListAsync();
    }

    public async Task ToggleAdminAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.IsAdmin = !user.IsAdmin;
        await _db.SaveChangesAsync();
    }
}
