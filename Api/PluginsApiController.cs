// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// REST API controller for feature plugin operations: search, detail, download, upload.
/// </summary>
/// <param name="pluginService">Service for feature plugin access and download tracking</param>
/// <param name="validationService">Service for manifest and bundle validation</param>
/// <param name="authService">Service for system user management during API uploads</param>
/// <param name="configuration">Configuration for API key validation</param>
using System.Text.Json;
using Klacks.Marketplace.Api.DTOs;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Mvc;

namespace Klacks.Marketplace.Api;

[ApiController]
[Route("api/plugins")]
public class PluginsApiController(
    IFeaturePluginMarketplaceService pluginService,
    IPluginValidationService validationService,
    IAuthService authService,
    IConfiguration configuration) : ControllerBase
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string ApiKeyConfigPath = "ApiSettings:UploadApiKey";

    [HttpGet]
    public async Task<ActionResult<PluginSearchResultDto>> Search(
        [FromQuery] string? search,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var (items, totalCount) = await pluginService.SearchPluginsAsync(search, category, PackageStatus.Published, page, pageSize);

        var result = new PluginSearchResultDto
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<PluginListItemDto>> GetByName(string name)
    {
        var plugin = await pluginService.GetPluginByNameAsync(name);

        if (plugin is null || plugin.Status != PackageStatus.Published)
        {
            return NotFound();
        }

        return Ok(MapToDto(plugin));
    }

    [HttpGet("{name}/download")]
    public async Task<IActionResult> Download(string name)
    {
        var plugin = await pluginService.GetPluginByNameAsync(name);

        if (plugin is null || plugin.Status != PackageStatus.Published)
        {
            return NotFound();
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var bytes = await pluginService.DownloadPluginAsync(name, ipAddress);

        if (bytes.Length == 0)
        {
            return NotFound("No bundle available");
        }

        return File(bytes, "application/zip", $"{name}.zip");
    }

    [HttpPost]
    public async Task<ActionResult> Upload([FromBody] PluginUploadDto dto)
    {
        var configuredKey = configuration.GetValue<string>(ApiKeyConfigPath) ?? string.Empty;
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedKey) ||
            providedKey.ToString() != configuredKey)
        {
            return Unauthorized("Invalid or missing API key");
        }

        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.ManifestJson))
        {
            return BadRequest("Name and ManifestJson are required");
        }

        var (isValid, error) = validationService.ValidatePluginManifest(dto.ManifestJson);
        if (!isValid)
        {
            return BadRequest($"Manifest validation failed: {error}");
        }

        byte[]? bundleData = null;
        if (!string.IsNullOrWhiteSpace(dto.BundleBase64))
        {
            bundleData = Convert.FromBase64String(dto.BundleBase64);
            var (bundleValid, bundleError) = validationService.ValidatePluginBundle(bundleData);
            if (!bundleValid)
            {
                return BadRequest($"Bundle validation failed: {bundleError}");
            }
        }

        var permissionsJson = "[]";
        var skillsJson = "[]";
        try
        {
            using var manifestDoc = JsonDocument.Parse(dto.ManifestJson);
            var root = manifestDoc.RootElement;

            if (root.TryGetProperty("requiredPermissions", out var perms))
            {
                permissionsJson = perms.GetRawText();
            }
            if (root.TryGetProperty("providedSkills", out var skills))
            {
                skillsJson = skills.GetRawText();
            }
        }
        catch (JsonException)
        {
            return BadRequest("Failed to parse manifest JSON");
        }

        var existing = await pluginService.GetPluginByNameAsync(dto.Name);

        if (existing is not null)
        {
            var updated = new FeaturePlugin
            {
                DisplayName = dto.DisplayName,
                Category = dto.Category,
                Version = dto.Version,
                Description = dto.Description,
                MinKlacksVersion = dto.MinKlacksVersion,
                RequiredPermissionsJson = permissionsJson,
                ProvidedSkillsJson = skillsJson
            };

            await pluginService.UpdatePluginAsync(dto.Name, updated, dto.ManifestJson, dto.I18nJson, bundleData, dto.ChangeLog);
            return Ok(new { message = "Plugin updated", name = dto.Name });
        }

        var systemUser = await authService.GetOrCreateSystemUserAsync();

        var plugin = new FeaturePlugin
        {
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Category = dto.Category,
            Version = dto.Version,
            Description = dto.Description,
            MinKlacksVersion = dto.MinKlacksVersion,
            RequiredPermissionsJson = permissionsJson,
            ProvidedSkillsJson = skillsJson,
            AuthorId = systemUser.Id,
            Status = PackageStatus.Published
        };

        await pluginService.CreatePluginAsync(plugin, dto.ManifestJson, dto.I18nJson, bundleData, dto.ChangeLog);
        return Created($"/api/plugins/{dto.Name}", new { message = "Plugin created", name = dto.Name });
    }

    private static PluginListItemDto MapToDto(FeaturePlugin p)
    {
        string[] permissions = [];
        string[] skills = [];

        try
        {
            if (!string.IsNullOrWhiteSpace(p.RequiredPermissionsJson))
            {
                permissions = JsonSerializer.Deserialize<string[]>(p.RequiredPermissionsJson) ?? [];
            }
        }
        catch (JsonException) { }

        try
        {
            if (!string.IsNullOrWhiteSpace(p.ProvidedSkillsJson))
            {
                skills = JsonSerializer.Deserialize<string[]>(p.ProvidedSkillsJson) ?? [];
            }
        }
        catch (JsonException) { }

        return new PluginListItemDto
        {
            Name = p.Name,
            DisplayName = p.DisplayName,
            Category = p.Category,
            Version = p.Version,
            Description = p.Description,
            MinKlacksVersion = p.MinKlacksVersion,
            AuthorName = p.Author?.DisplayName ?? string.Empty,
            Downloads = p.Downloads,
            RequiredPermissions = permissions,
            ProvidedSkills = skills,
            UpdatedAt = p.UpdatedAt
        };
    }
}
