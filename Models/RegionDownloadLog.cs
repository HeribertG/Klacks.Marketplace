// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Tracks individual download events for region packages including artifact type, chosen industry, IP and timestamp.
/// </summary>
/// <param name="ArtifactType">Downloaded artifact kind from RegionArtifactType constants</param>
/// <param name="Industry">Industry slug chosen for the download or "all"</param>
namespace Klacks.Marketplace.Models;

public class RegionDownloadLog
{
    public int Id { get; set; }
    public int RegionPackageId { get; set; }
    public RegionPackage RegionPackage { get; set; } = null!;
    public string Version { get; set; } = string.Empty;
    public string ArtifactType { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;
    public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; } = string.Empty;
}
