// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Validates feature plugin manifest JSON, i18n files and ZIP bundles against required schemas.
/// </summary>
/// <param name="json">JSON string to validate</param>
/// <param name="zipData">ZIP archive bytes to inspect</param>
using System.IO.Compression;
using System.Text.Json;
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public class PluginValidationService : IPluginValidationService
{
    public (bool IsValid, string ErrorMessage) ValidatePluginManifest(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return (false, "Manifest must be a JSON object");
            }

            foreach (var field in AppConstants.RequiredManifestFields)
            {
                if (!root.TryGetProperty(field, out var prop) ||
                    prop.ValueKind != JsonValueKind.String ||
                    string.IsNullOrWhiteSpace(prop.GetString()))
                {
                    return (false, $"Missing or empty required field: {field}");
                }
            }

            if (root.TryGetProperty("category", out var categoryProp))
            {
                var category = categoryProp.GetString() ?? string.Empty;
                if (!PluginCategory.AllCategories.Contains(category))
                {
                    return (false, $"Invalid category: '{category}'. Allowed: {string.Join(", ", PluginCategory.AllCategories)}");
                }
            }

            if (root.TryGetProperty("navigation", out var nav) && nav.ValueKind == JsonValueKind.Object)
            {
                if (!nav.TryGetProperty("route", out var route) ||
                    route.ValueKind != JsonValueKind.String ||
                    string.IsNullOrWhiteSpace(route.GetString()))
                {
                    return (false, "Navigation object requires a non-empty 'route' field");
                }

                if (!nav.TryGetProperty("labelKey", out var label) ||
                    label.ValueKind != JsonValueKind.String ||
                    string.IsNullOrWhiteSpace(label.GetString()))
                {
                    return (false, "Navigation object requires a non-empty 'labelKey' field");
                }
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }

    public (bool IsValid, string ErrorMessage) ValidateI18nFile(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return (false, "i18n file must be a JSON object");
            }

            var count = 0;
            foreach (var prop in root.EnumerateObject())
            {
                if (prop.Value.ValueKind != JsonValueKind.String)
                {
                    return (false, $"i18n value for key '{prop.Name}' must be a string");
                }
                count++;
            }

            if (count == 0)
            {
                return (false, "i18n file must contain at least one entry");
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }

    public (bool IsValid, string ErrorMessage) ValidatePluginBundle(byte[] zipData)
    {
        try
        {
            if (zipData.Length > AppConstants.PluginMaxUploadSizeBytes)
            {
                return (false, $"Bundle exceeds maximum size of {AppConstants.PluginMaxUploadSizeBytes / (1024 * 1024)} MB");
            }

            using var stream = new MemoryStream(zipData);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

            var hasManifest = false;
            var hasEnglishI18n = false;

            foreach (var entry in archive.Entries)
            {
                var name = entry.FullName.Replace('\\', '/');

                if (name == AppConstants.PluginManifestFileName)
                {
                    hasManifest = true;
                }

                if (name == $"{AppConstants.PluginI18nDirectory}/en.json" ||
                    name == "i18n/en.json")
                {
                    hasEnglishI18n = true;
                }
            }

            if (!hasManifest)
            {
                return (false, "Bundle must contain a manifest.json file");
            }

            if (!hasEnglishI18n)
            {
                return (false, "Bundle must contain an i18n/en.json file");
            }

            return (true, string.Empty);
        }
        catch (InvalidDataException)
        {
            return (false, "Invalid ZIP archive");
        }
    }
}
