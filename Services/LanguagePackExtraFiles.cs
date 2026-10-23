// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Validates the optional further files of a language pack upload: a JSON object whose property names are plain
/// lower-case JSON file names ("assistant-texts.json") and whose values are the file contents. Klacks writes only
/// the names it knows, but the store already refuses anything that could be a path.
/// </summary>
/// <param name="extraFilesJson">The uploaded JSON object, or null/empty when the upload has no further files</param>
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Klacks.Marketplace.Services;

public static partial class LanguagePackExtraFiles
{
    public const string InvalidMessage = "ExtraFilesJson must be a JSON object keyed by plain lower-case .json file names";

    public static bool IsValid(string? extraFilesJson)
    {
        if (string.IsNullOrWhiteSpace(extraFilesJson))
        {
            return true;
        }

        try
        {
            using var document = JsonDocument.Parse(extraFilesJson);
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            return document.RootElement.EnumerateObject().All(file => FileNamePattern().IsMatch(file.Name));
        }
        catch (JsonException)
        {
            return false;
        }
    }

    [GeneratedRegex(@"^[a-z0-9-]+\.json$")]
    private static partial Regex FileNamePattern();
}
