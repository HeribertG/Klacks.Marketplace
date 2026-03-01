// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using System.Text.Json;

namespace Klacks.Marketplace.Services;

public class FileValidationService : IFileValidationService
{
    private static readonly string[] RequiredManifestFields = ["code", "name", "displayName", "speechLocale", "version", "author"];

    public (bool IsValid, string ErrorMessage) ValidateManifest(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return (false, "Manifest must be a JSON object");
            }

            foreach (var field in RequiredManifestFields)
            {
                if (!root.TryGetProperty(field, out var prop) || prop.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(prop.GetString()))
                {
                    return (false, $"Missing or empty required field: {field}");
                }
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }

    public (bool IsValid, string ErrorMessage) ValidateTranslations(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return (false, "Translations must be a JSON object");
            }

            var count = 0;
            foreach (var prop in root.EnumerateObject())
            {
                if (prop.Value.ValueKind != JsonValueKind.String)
                {
                    return (false, $"Translation value for key '{prop.Name}' must be a string");
                }
                count++;
            }

            if (count == 0)
            {
                return (false, "Translations file must contain at least one entry");
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }
}
