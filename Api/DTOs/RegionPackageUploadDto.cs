// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// DTO for uploading a region package via REST API.
/// </summary>
/// <param name="CountryCode">ISO 3166-1 alpha-2 country code of the package</param>
/// <param name="Version">Semver version of the uploaded profile</param>
/// <param name="ProfileJson">Full region setup profile JSON content</param>
/// <param name="ChangeLog">Optional release notes for the uploaded version</param>
namespace Klacks.Marketplace.Api.DTOs;

public class RegionPackageUploadDto
{
    public string CountryCode { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = string.Empty;
    public string MinKlacksVersion { get; set; } = "1.0.0";
    public string ProfileJson { get; set; } = string.Empty;
    public string? ChangeLog { get; set; }
}
