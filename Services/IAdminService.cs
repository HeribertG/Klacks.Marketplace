// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Models;
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public interface IAdminService
{
    Task<List<LanguagePackage>> GetPendingPackagesAsync();
    Task ApprovePackageAsync(int packageId);
    Task RejectPackageAsync(int packageId, string reason);
    Task<List<FeaturePlugin>> GetPendingPluginsAsync();
    Task ApprovePluginAsync(int pluginId);
    Task RejectPluginAsync(int pluginId, string reason);
    Task<List<User>> GetUsersAsync();
    Task ToggleAdminAsync(int userId);
}
