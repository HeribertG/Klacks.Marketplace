// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// DTO for displaying a region package in the marketplace list view.
/// </summary>
namespace Klacks.Marketplace.Api.DTOs;

public class RegionPackageListItemDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MinKlacksVersion { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int Downloads { get; set; }
    public DateTime UpdatedAt { get; set; }
}
