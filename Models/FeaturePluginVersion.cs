// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Versioned content of a feature plugin including manifest, i18n translations and the ZIP bundle.
/// </summary>
/// <param name="ManifestJson">Full manifest.json content as string</param>
/// <param name="I18nJson">Combined i18n translations as {"en": {...}, "de": {...}}</param>
/// <param name="BundleData">ZIP archive bytes containing all plugin files</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Models;

public class FeaturePluginVersion
{
    public int Id { get; set; }
    public int PluginId { get; set; }
    public FeaturePlugin Plugin { get; set; } = null!;
    public string Version { get; set; } = "1.0.0";
    public string ManifestJson { get; set; } = string.Empty;
    public string I18nJson { get; set; } = string.Empty;
    public string ChangeLog { get; set; } = string.Empty;
    public byte[] BundleData { get; set; } = [];
    public PackageStatus Status { get; set; } = PackageStatus.PendingReview;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
