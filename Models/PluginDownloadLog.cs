// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Tracks individual download events for feature plugins including IP and timestamp.
/// </summary>
namespace Klacks.Marketplace.Models;

public class PluginDownloadLog
{
    public int Id { get; set; }
    public int PluginId { get; set; }
    public FeaturePlugin Plugin { get; set; } = null!;
    public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; } = string.Empty;
}
