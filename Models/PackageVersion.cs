// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Models;

public class PackageVersion
{
    public int Id { get; set; }
    public int PackageId { get; set; }
    public LanguagePackage Package { get; set; } = null!;
    public string Version { get; set; } = "1.0.0";
    public string TranslationsJson { get; set; } = string.Empty;
    public string ManifestJson { get; set; } = string.Empty;
    public string ChangeLog { get; set; } = string.Empty;
    public PackageStatus Status { get; set; } = PackageStatus.PendingReview;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
