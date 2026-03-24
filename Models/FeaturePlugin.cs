// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Marketplace entity for a feature plugin package with metadata, versioning and download tracking.
/// </summary>
/// <param name="Name">Unique plugin identifier matching the manifest name field</param>
/// <param name="Category">Plugin category from PluginCategory constants</param>
/// <param name="RequiredPermissionsJson">JSON array of permission strings</param>
/// <param name="ProvidedSkillsJson">JSON array of skill name strings</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Models;

public class FeaturePlugin
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    public string MinKlacksVersion { get; set; } = "1.0.0";
    public string RequiredPermissionsJson { get; set; } = "[]";
    public string ProvidedSkillsJson { get; set; } = "[]";
    public PackageStatus Status { get; set; } = PackageStatus.Draft;
    public int Downloads { get; set; }
    public string ReadmeMarkdown { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<FeaturePluginVersion> Versions { get; set; } = [];
    public List<PluginDownloadLog> DownloadLogs { get; set; } = [];
}
