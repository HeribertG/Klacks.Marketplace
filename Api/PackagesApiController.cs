// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// REST-API-Controller für öffentliche Sprachpaket-Operationen (Suche, Detail, Download, Upload).
/// </summary>
/// <param name="packageService">Service für Sprachpaket-Zugriff und Download-Tracking</param>
/// <param name="authService">Service für Benutzer-Authentifizierung und System-User-Verwaltung</param>
/// <param name="configuration">Konfiguration für API-Key-Validierung</param>
using Klacks.Marketplace.Api.DTOs;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Mvc;

namespace Klacks.Marketplace.Api;

[ApiController]
[Route("api/packages")]
public class PackagesApiController(
    IPackageService packageService,
    IAuthService authService,
    IConfiguration configuration) : ControllerBase
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string ApiKeyConfigPath = "ApiSettings:UploadApiKey";

    [HttpGet]
    public async Task<ActionResult<PackageSearchResultDto>> Search(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        var (items, totalCount) = await packageService.SearchPackagesAsync(search, PackageStatus.Published, page, pageSize);

        var result = new PackageSearchResultDto
        {
            Items = items.Select(p => new PackageListItemDto
            {
                Code = p.Code,
                Name = p.Name,
                DisplayName = p.DisplayName,
                SpeechLocale = p.SpeechLocale,
                Version = p.Version,
                Coverage = p.Coverage,
                TranslationCount = p.TranslationCount,
                Description = p.Description,
                Downloads = p.Downloads,
                MinKlacksVersion = p.MinKlacksVersion,
                AuthorName = p.Author?.DisplayName ?? string.Empty,
                UpdatedAt = p.UpdatedAt
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<ActionResult<PackageListItemDto>> GetByCode(string code)
    {
        var package = await packageService.GetPackageByCodeAsync(code);

        if (package is null || package.Status != PackageStatus.Published)
        {
            return NotFound();
        }

        var dto = new PackageListItemDto
        {
            Code = package.Code,
            Name = package.Name,
            DisplayName = package.DisplayName,
            SpeechLocale = package.SpeechLocale,
            Version = package.Version,
            Coverage = package.Coverage,
            TranslationCount = package.TranslationCount,
            Description = package.Description,
            Downloads = package.Downloads,
            MinKlacksVersion = package.MinKlacksVersion,
            AuthorName = package.Author?.DisplayName ?? string.Empty,
            UpdatedAt = package.UpdatedAt
        };

        return Ok(dto);
    }

    [HttpGet("{code}/download")]
    public async Task<IActionResult> Download(string code)
    {
        var package = await packageService.GetPackageByCodeAsync(code);

        if (package is null || package.Status != PackageStatus.Published)
        {
            return NotFound();
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var bytes = await packageService.DownloadPackageAsync(code, ipAddress);

        return File(bytes, "application/json", $"{code}.json");
    }

    [HttpPost]
    public async Task<ActionResult> Upload([FromBody] PackageUploadDto dto)
    {
        var configuredKey = configuration.GetValue<string>(ApiKeyConfigPath) ?? string.Empty;
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedKey) ||
            providedKey.ToString() != configuredKey)
        {
            return Unauthorized("Invalid or missing API key");
        }

        if (string.IsNullOrWhiteSpace(dto.Code) ||
            string.IsNullOrWhiteSpace(dto.ManifestJson) ||
            string.IsNullOrWhiteSpace(dto.TranslationsJson))
        {
            return BadRequest("Code, ManifestJson and TranslationsJson are required");
        }

        var existing = await packageService.GetPackageByCodeAsync(dto.Code);

        if (existing is not null)
        {
            var updated = new LanguagePackage
            {
                Name = dto.Name,
                DisplayName = dto.DisplayName,
                SpeechLocale = dto.SpeechLocale,
                Version = dto.Version,
                Coverage = dto.Coverage,
                Description = dto.Description,
                MinKlacksVersion = dto.MinKlacksVersion
            };

            await packageService.UpdatePackageAsync(
                dto.Code, updated, dto.ManifestJson, dto.TranslationsJson,
                dto.DocsJson, dto.CountriesJson, dto.StatesJson, dto.CalendarRulesJson);

            return Ok(new { message = "Package updated", code = dto.Code });
        }

        var systemUser = await authService.GetOrCreateSystemUserAsync();

        var package = new LanguagePackage
        {
            Code = dto.Code,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            SpeechLocale = dto.SpeechLocale,
            Version = dto.Version,
            Coverage = dto.Coverage,
            Description = dto.Description,
            MinKlacksVersion = dto.MinKlacksVersion,
            AuthorId = systemUser.Id,
            Status = PackageStatus.Published
        };

        await packageService.CreatePackageAsync(
            package, dto.ManifestJson, dto.TranslationsJson,
            dto.DocsJson, dto.CountriesJson, dto.StatesJson, dto.CalendarRulesJson);

        return Created($"/api/packages/{dto.Code}", new { message = "Package created", code = dto.Code });
    }
}
