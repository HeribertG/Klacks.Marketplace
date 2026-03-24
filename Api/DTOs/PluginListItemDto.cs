// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// DTO for displaying a feature plugin in the marketplace list view.
/// </summary>
namespace Klacks.Marketplace.Api.DTOs;

public class PluginListItemDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MinKlacksVersion { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public int Downloads { get; set; }
    public string[] RequiredPermissions { get; set; } = [];
    public string[] ProvidedSkills { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
}
