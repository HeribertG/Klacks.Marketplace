// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using System.Text.Json;
using Klacks.Marketplace.Constants;

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

    public (bool IsValid, string ErrorMessage) ValidateDocs(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Object)
            {
                return (false, "Docs must be a JSON object");
            }

            var count = 0;
            foreach (var prop in root.EnumerateObject())
            {
                if (!AppConstants.AllowedManualNames.Contains(prop.Name))
                {
                    return (false, $"Unknown manual name: '{prop.Name}'. Allowed: {string.Join(", ", AppConstants.AllowedManualNames)}");
                }

                if (prop.Value.ValueKind != JsonValueKind.String)
                {
                    return (false, $"Manual content for '{prop.Name}' must be a string");
                }

                if (string.IsNullOrWhiteSpace(prop.Value.GetString()))
                {
                    return (false, $"Manual content for '{prop.Name}' must not be empty");
                }

                count++;
            }

            if (count == 0)
            {
                return (false, "Docs file must contain at least one manual");
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }

    public (bool IsValid, string ErrorMessage) ValidateCountriesJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
            {
                return (false, "Countries must be a JSON array");
            }

            foreach (var item in root.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    return (false, "Each country must be a JSON object");
                }

                string[] requiredFields = ["id", "abbreviation", "prefix"];
                foreach (var field in requiredFields)
                {
                    if (!item.TryGetProperty(field, out _))
                    {
                        return (false, $"Country missing required field: {field}");
                    }
                }
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }

    public (bool IsValid, string ErrorMessage) ValidateStatesJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
            {
                return (false, "States must be a JSON array");
            }

            foreach (var item in root.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    return (false, "Each state must be a JSON object");
                }

                string[] requiredFields = ["id", "abbreviation", "countryPrefix"];
                foreach (var field in requiredFields)
                {
                    if (!item.TryGetProperty(field, out _))
                    {
                        return (false, $"State missing required field: {field}");
                    }
                }
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }

    public (bool IsValid, string ErrorMessage) ValidateCalendarRulesJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
            {
                return (false, "Calendar rules must be a JSON array");
            }

            foreach (var item in root.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object)
                {
                    return (false, "Each calendar rule must be a JSON object");
                }

                string[] requiredFields = ["id", "rule", "country"];
                foreach (var field in requiredFields)
                {
                    if (!item.TryGetProperty(field, out _))
                    {
                        return (false, $"Calendar rule missing required field: {field}");
                    }
                }
            }

            return (true, string.Empty);
        }
        catch (JsonException)
        {
            return (false, "Invalid JSON format");
        }
    }
}
