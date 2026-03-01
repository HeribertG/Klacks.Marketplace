// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Models;

namespace Klacks.Marketplace.Services;

public interface IAdminService
{
    Task<List<LanguagePackage>> GetPendingPackagesAsync();
    Task ApprovePackageAsync(int packageId);
    Task RejectPackageAsync(int packageId, string reason);
    Task<List<User>> GetUsersAsync();
    Task ToggleAdminAsync(int userId);
}
