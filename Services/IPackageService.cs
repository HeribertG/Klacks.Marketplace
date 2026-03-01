// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;

namespace Klacks.Marketplace.Services;

public interface IPackageService
{
    Task<(List<LanguagePackage> Items, int TotalCount)> SearchPackagesAsync(string? search, PackageStatus? status, int page, int pageSize);
    Task<LanguagePackage?> GetPackageByCodeAsync(string code);
    Task<LanguagePackage> CreatePackageAsync(LanguagePackage package, string manifestJson, string translationsJson);
    Task<byte[]> DownloadPackageAsync(string code, string ipAddress);
    Task<List<LanguagePackage>> GetUserPackagesAsync(int userId);
}
