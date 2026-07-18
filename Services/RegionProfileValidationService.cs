// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Validates region setup profile JSON: size limit, JSON shape, schema version and canonical industry slugs.
/// </summary>
/// <param name="json">Region setup profile JSON string to validate</param>
using System.Text;
using System.Text.Json;
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public class RegionProfileValidationService : IRegionProfileValidationService
{
    private const string VersionPropertyName = "version";
    private const string IndustryProfilesPropertyName = "industryProfiles";

    public RegionProfileValidationResult ValidateProfileJson(string json)
    {
        var result = new RegionProfileValidationResult();

        if (string.IsNullOrWhiteSpace(json))
        {
            result.Errors.Add("Profile JSON must not be empty");
            return result;
        }

        if (Encoding.UTF8.GetByteCount(json) > AppConstants.RegionProfileMaxUploadSizeBytes)
        {
            result.Errors.Add($"Profile exceeds maximum size of {AppConstants.RegionProfileMaxUploadSizeBytes / (1024 * 1024)} MB");
            return result;
        }

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                result.Errors.Add("Profile must be a JSON object");
                return result;
            }

            ValidateSchemaVersion(root, result);
            ValidateIndustryProfiles(root, result);
        }
        catch (JsonException)
        {
            result.Errors.Add("Invalid JSON format");
        }

        return result;
    }

    private static void ValidateSchemaVersion(JsonElement root, RegionProfileValidationResult result)
    {
        if (!root.TryGetProperty(VersionPropertyName, out var versionProp))
        {
            result.Errors.Add($"Missing required top-level field: {VersionPropertyName}");
            return;
        }

        if (versionProp.ValueKind != JsonValueKind.Number || !versionProp.TryGetInt32(out var version))
        {
            result.Errors.Add($"Top-level field '{VersionPropertyName}' must be a number");
            return;
        }

        if (version != AppConstants.RegionProfileSchemaVersion)
        {
            result.Errors.Add($"Unsupported schema version {version}. Expected: {AppConstants.RegionProfileSchemaVersion}");
        }
    }

    private static void ValidateIndustryProfiles(JsonElement root, RegionProfileValidationResult result)
    {
        if (!root.TryGetProperty(IndustryProfilesPropertyName, out var industryProfiles))
        {
            result.Warnings.Add($"Profile has no '{IndustryProfilesPropertyName}' section");
            return;
        }

        if (industryProfiles.ValueKind != JsonValueKind.Object)
        {
            result.Errors.Add($"'{IndustryProfilesPropertyName}' must be a JSON object");
            return;
        }

        foreach (var block in industryProfiles.EnumerateObject())
        {
            if (!RegionIndustry.CanonicalSlugs.Contains(block.Name))
            {
                result.Warnings.Add($"Industry profile key '{block.Name}' is not a canonical slug. Canonical: {string.Join(", ", RegionIndustry.CanonicalSlugs)}");
            }
        }
    }
}
