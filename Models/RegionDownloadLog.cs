// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Tracks individual download events for region packages including artifact type, chosen industry, IP and timestamp.
/// </summary>
/// <param name="Version">Region package version that was downloaded; empty for office-PC download clicks</param>
/// <param name="ArtifactType">Downloaded artifact kind from RegionArtifactType constants, or an office-PC download click from NativeInstallPlatform</param>
/// <param name="Industry">Industry slug chosen for the download or "all"; empty for office-PC download clicks</param>
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
