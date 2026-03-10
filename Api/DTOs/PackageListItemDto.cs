// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// DTO für die Listenansicht eines Sprachpakets im Marketplace.
/// </summary>
namespace Klacks.Marketplace.Api.DTOs;

public class PackageListItemDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string SpeechLocale { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public double Coverage { get; set; }
    public int TranslationCount { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Downloads { get; set; }
    public string MinKlacksVersion { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; }
}
