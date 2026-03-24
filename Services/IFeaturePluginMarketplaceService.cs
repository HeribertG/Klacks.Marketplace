// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service interface for feature plugin CRUD operations, search and download in the marketplace.
/// </summary>
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;

namespace Klacks.Marketplace.Services;

public interface IFeaturePluginMarketplaceService
{
    Task<(List<FeaturePlugin> Items, int TotalCount)> SearchPluginsAsync(string? search, string? category, PackageStatus? status, int page, int pageSize);
    Task<FeaturePlugin?> GetPluginByNameAsync(string name);
    Task<FeaturePlugin> CreatePluginAsync(FeaturePlugin plugin, string manifestJson, string? i18nJson, byte[]? bundleData, string? changeLog);
    Task<FeaturePlugin> UpdatePluginAsync(string name, FeaturePlugin updated, string manifestJson, string? i18nJson, byte[]? bundleData, string? changeLog);
    Task<byte[]> DownloadPluginAsync(string name, string ipAddress);
    Task<List<FeaturePlugin>> GetUserPluginsAsync(int userId);
}
