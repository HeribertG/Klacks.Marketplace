// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// REST API controller for region package operations: search, detail, artifact download (profile JSON, on-prem ZIP, compose ZIP), upload.
/// </summary>
/// <param name="regionPackageService">Service for region package access and download tracking</param>
/// <param name="validationService">Service for region profile JSON validation</param>
/// <param name="patchService">Service for patching profile JSON with package identity and industry selection</param>
/// <param name="artifactService">Service for assembling on-prem and compose ZIP bundles</param>
/// <param name="signingService">Service for signing download artifacts with the vendor private key</param>
/// <param name="authService">Service for system user management during API uploads</param>
/// <param name="configuration">Configuration for API key validation</param>
using System.Text;
using Klacks.Marketplace.Api.DTOs;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Models;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Klacks.Marketplace.Api;

[ApiController]
[Route("api/regions")]
public class RegionPackagesApiController(
    IRegionPackageService regionPackageService,
    IRegionProfileValidationService validationService,
    IRegionProfilePatchService patchService,
    IRegionArtifactService artifactService,
    IRegionArtifactSigningService signingService,
    IAuthService authService,
    IConfiguration configuration) : ControllerBase
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string ApiKeyConfigPath = "ApiSettings:UploadApiKey";
    private const string ProfileContentType = "application/json";
    private const string ZipContentType = "application/zip";
    private const string ProfileFileNameFormat = "{0}.json";
    private const string OnPremZipFileNameFormat = "klacks-onprem-{0}-{1}.zip";
    private const string ComposeZipFileNameFormat = "klacks-compose-{0}-{1}.zip";
    private const string InvalidCountryCodeMessage = "Invalid country code. Expected a two-letter ISO code.";
    private const string PendingReviewStatusValue = "pendingReview";

    [HttpGet]
    public async Task<ActionResult<RegionPackageSearchResultDto>> Search(
        [FromQuery] string? search,
        [FromQuery] string? countryCode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12)
    {
        page = Math.Max(page, AppConstants.MinSearchPage);
        pageSize = Math.Clamp(pageSize, AppConstants.MinSearchPageSize, AppConstants.MaxSearchPageSize);

        var (items, totalCount) = await regionPackageService.SearchRegionPackagesAsync(
            search, countryCode, PackageStatus.Published, page, pageSize);

        var result = new RegionPackageSearchResultDto
        {
            Items = items.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return Ok(result);
    }

    [HttpGet("{countryCode}")]
    public async Task<ActionResult<RegionPackageListItemDto>> GetByCountry(string countryCode)
    {
        var normalizedCountry = countryCode.ToLowerInvariant();
        if (!RegionCountry.IsValidCountryCode(normalizedCountry))
        {
            return BadRequest(InvalidCountryCodeMessage);
        }

        var package = await regionPackageService.GetRegionPackageByCountryAsync(normalizedCountry);

        if (package is null || package.Status != PackageStatus.Published)
        {
            return NotFound();
        }

        var latestPublished = await regionPackageService.GetLatestPublishedVersionAsync(normalizedCountry);
        if (latestPublished is null)
        {
            return NotFound();
        }

        var dto = MapToDto(package);
        dto.Version = latestPublished.Version;
        return Ok(dto);
    }

    [HttpGet("{countryCode}/download")]
    [EnableRateLimiting(AppConstants.DownloadRateLimitPolicyName)]
    public async Task<IActionResult> Download(string countryCode, [FromQuery] string? industry, [FromQuery] string? artifact)
    {
        var normalizedCountry = countryCode.ToLowerInvariant();
        if (!RegionCountry.IsValidCountryCode(normalizedCountry))
        {
            return BadRequest(InvalidCountryCodeMessage);
        }

        var selectedIndustry = string.IsNullOrWhiteSpace(industry)
            ? RegionIndustry.All
            : industry.ToLowerInvariant();

        if (!RegionIndustry.AllOptions.Contains(selectedIndustry))
        {
            return BadRequest($"Invalid industry: '{industry}'. Allowed: {string.Join(", ", RegionIndustry.AllOptions)}");
        }

        var selectedArtifact = string.IsNullOrWhiteSpace(artifact)
            ? RegionArtifactType.ProfileJson
            : RegionArtifactType.AllArtifactTypes.FirstOrDefault(a => a.Equals(artifact, StringComparison.OrdinalIgnoreCase));

        if (selectedArtifact is null)
        {
            return BadRequest($"Invalid artifact: '{artifact}'. Allowed: {string.Join(", ", RegionArtifactType.AllArtifactTypes)}");
        }

        var package = await regionPackageService.GetRegionPackageByCountryAsync(normalizedCountry);
        if (package is null || package.Status != PackageStatus.Published)
        {
            return NotFound();
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var (profileJson, version) = await regionPackageService.DownloadRegionProfileAsync(
            normalizedCountry, selectedIndustry, selectedArtifact, ipAddress);
        var patchedProfileJson = patchService.PatchProfileJson(profileJson, package.CountryCode, version, selectedIndustry);

        var (fileBytes, contentType, fileName) = selectedArtifact switch
        {
            RegionArtifactType.OnPremZip => (
                artifactService.BuildOnPremBundle(package.CountryCode, patchedProfileJson),
                ZipContentType,
                string.Format(OnPremZipFileNameFormat, package.CountryCode, selectedIndustry)),
            RegionArtifactType.ComposeZip => (
                artifactService.BuildComposeBundle(package.CountryCode, patchedProfileJson),
                ZipContentType,
                string.Format(ComposeZipFileNameFormat, package.CountryCode, selectedIndustry)),
            _ => (
                Encoding.UTF8.GetBytes(patchedProfileJson),
                ProfileContentType,
                string.Format(ProfileFileNameFormat, package.CountryCode))
        };

        var signature = signingService.SignPayload(fileBytes);
        if (signature is not null)
        {
            Response.Headers.Append(AppConstants.SignatureHeaderName, signature);
        }

        return File(fileBytes, contentType, fileName);
    }

    [HttpPost]
    public async Task<ActionResult> Upload([FromBody] RegionPackageUploadDto dto)
    {
        var configuredKey = configuration.GetValue<string>(ApiKeyConfigPath);
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedKey) ||
            !ApiKeyValidator.IsValid(providedKey.ToString(), configuredKey))
        {
            return Unauthorized("Invalid or missing API key");
        }

        if (string.IsNullOrWhiteSpace(dto.CountryCode) ||
            string.IsNullOrWhiteSpace(dto.ProfileJson))
        {
            return BadRequest("CountryCode and ProfileJson are required");
        }

        var countryCode = dto.CountryCode.ToLowerInvariant();
        if (!RegionCountry.IsValidCountryCode(countryCode))
        {
            return BadRequest(InvalidCountryCodeMessage);
        }

        var validation = validationService.ValidateProfileJson(dto.ProfileJson);
        if (!validation.IsValid)
        {
            return BadRequest($"Profile validation failed: {string.Join("; ", validation.Errors)}");
        }

        var existing = await regionPackageService.GetRegionPackageByCountryAsync(countryCode);

        if (existing is not null)
        {
            var updated = new RegionPackage
            {
                Version = dto.Version,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? existing.Description : dto.Description,
                MinKlacksVersion = dto.MinKlacksVersion
            };

            await regionPackageService.UpdateRegionPackageAsync(
                countryCode, updated, dto.ProfileJson, dto.ChangeLog, PackageStatus.PendingReview);

            return Ok(new { message = "Region package updated, awaiting admin review", status = PendingReviewStatusValue, countryCode, warnings = validation.Warnings });
        }

        var systemUser = await authService.GetOrCreateSystemUserAsync();

        var package = new RegionPackage
        {
            CountryCode = countryCode,
            CountryName = RegionCountry.CountryNames.GetValueOrDefault(countryCode, countryCode.ToUpperInvariant()),
            Version = dto.Version,
            Description = dto.Description,
            MinKlacksVersion = dto.MinKlacksVersion,
            AuthorId = systemUser.Id,
            Status = PackageStatus.PendingReview
        };

        await regionPackageService.CreateRegionPackageAsync(package, dto.ProfileJson, dto.ChangeLog);

        return Created($"/api/regions/{countryCode}", new { message = "Region package created, awaiting admin review", status = PendingReviewStatusValue, countryCode, warnings = validation.Warnings });
    }

    private static RegionPackageListItemDto MapToDto(RegionPackage p)
    {
        return new RegionPackageListItemDto
        {
            CountryCode = p.CountryCode,
            CountryName = p.CountryName,
            Version = p.Version,
            Description = p.Description,
            MinKlacksVersion = p.MinKlacksVersion,
            AuthorName = p.Author?.DisplayName ?? string.Empty,
            Downloads = p.Downloads,
            UpdatedAt = p.UpdatedAt
        };
    }
}
