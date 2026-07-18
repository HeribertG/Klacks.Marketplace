// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Versioned content of a region package holding the complete region setup profile JSON.
/// </summary>
/// <param name="ProfileJson">Full region setup profile JSON content</param>
/// <param name="ContentHash">SHA-256 hex hash of ProfileJson used by the seeder for change detection</param>
/// <param name="IsSeeded">True when the version was created by the startup seeder rather than a manual upload</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Models;

public class RegionPackageVersion
{
    public int Id { get; set; }
    public int RegionPackageId { get; set; }
    public RegionPackage RegionPackage { get; set; } = null!;
    public string Version { get; set; } = "1.0.0";
    public string ProfileJson { get; set; } = string.Empty;
    public string ChangeLog { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public bool IsSeeded { get; set; }
    public PackageStatus Status { get; set; } = PackageStatus.PendingReview;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
