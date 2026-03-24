// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// DTO for uploading a feature plugin via REST API.
/// </summary>
/// <param name="Name">Unique plugin identifier</param>
/// <param name="ManifestJson">Full manifest.json content</param>
/// <param name="I18nJson">Combined i18n translations as JSON</param>
/// <param name="BundleBase64">Base64-encoded ZIP bundle containing all plugin files</param>
namespace Klacks.Marketplace.Api.DTOs;

public class PluginUploadDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public string Description { get; set; } = string.Empty;
    public string MinKlacksVersion { get; set; } = "1.0.0";
    public string ManifestJson { get; set; } = string.Empty;
    public string? I18nJson { get; set; }
    public string? BundleBase64 { get; set; }
    public string? ChangeLog { get; set; }
}
