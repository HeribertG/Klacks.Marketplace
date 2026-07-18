// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Marketplace entity for a country region package whose payload is the region setup profile JSON.
/// </summary>
/// <param name="CountryCode">Unique ISO 3166-1 alpha-2 country code (lowercase)</param>
/// <param name="Version">Semver string of the currently published version</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Models;

public class RegionPackage
{
    public int Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public PackageStatus Status { get; set; } = PackageStatus.Draft;
    public string MinKlacksVersion { get; set; } = "1.0.0";
    public int Downloads { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<RegionPackageVersion> Versions { get; set; } = [];
    public List<RegionDownloadLog> DownloadLogs { get; set; } = [];
}
