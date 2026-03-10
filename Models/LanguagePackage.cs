// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Models;

public class LanguagePackage
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string SpeechLocale { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
    public double Coverage { get; set; }
    public int TranslationCount { get; set; }
    public string Description { get; set; } = string.Empty;
    public PackageStatus Status { get; set; } = PackageStatus.Draft;
    public string MinKlacksVersion { get; set; } = "1.0.0";
    public int Downloads { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<PackageVersion> Versions { get; set; } = [];
    public List<DownloadLog> DownloadLogs { get; set; } = [];
}
