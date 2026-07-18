// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service interface for region package search, detail, upload, download and user-specific listing.
/// </summary>
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;

namespace Klacks.Marketplace.Services;

public interface IRegionPackageService
{
    Task<(List<RegionPackage> Items, int TotalCount)> SearchRegionPackagesAsync(string? search, string? countryCode, PackageStatus? status, int page, int pageSize);
    Task<RegionPackage?> GetRegionPackageByCountryAsync(string countryCode);
    Task<RegionPackageVersion?> GetLatestPublishedVersionAsync(string countryCode);
    Task<RegionPackage> CreateRegionPackageAsync(RegionPackage package, string profileJson, string? changeLog);
    Task<RegionPackage> UpdateRegionPackageAsync(string countryCode, RegionPackage updated, string profileJson, string? changeLog, PackageStatus versionStatus);
    Task<(string ProfileJson, string Version)> DownloadRegionProfileAsync(string countryCode, string industry, string artifactType, string ipAddress);
    Task<List<RegionPackage>> GetUserRegionPackagesAsync(int userId);
}
