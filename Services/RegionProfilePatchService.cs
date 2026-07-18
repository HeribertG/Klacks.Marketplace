// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Patches a region profile JSON with the marketplace package identity block and the selected industry before delivery.
/// </summary>
/// <param name="profileJson">Region profile JSON of the published package version</param>
/// <param name="countryCode">Lowercase two-letter country code written into the package block</param>
/// <param name="packageVersion">Version of the delivered package written into the package block</param>
/// <param name="industry">Selected industry slug or "all"</param>
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public class RegionProfilePatchService : IRegionProfilePatchService
{
    private const string PackagePropertyName = "package";
    private const string PackageCountryPropertyName = "country";
    private const string PackageVersionPropertyName = "version";
    private const string ActiveIndustriesPropertyName = "activeIndustries";
    private const string IndustryProfilesPropertyName = "industryProfiles";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string PatchProfileJson(string profileJson, string countryCode, string packageVersion, string industry)
    {
        if (JsonNode.Parse(profileJson) is not JsonObject root)
        {
            throw new InvalidOperationException("Region profile must be a JSON object");
        }

        root[PackagePropertyName] = new JsonObject
        {
            [PackageCountryPropertyName] = countryCode,
            [PackageVersionPropertyName] = packageVersion
        };

        if (industry != RegionIndustry.All && HasIndustryProfile(root, industry))
        {
            root[ActiveIndustriesPropertyName] = new JsonArray(industry);
        }
        else
        {
            root.Remove(ActiveIndustriesPropertyName);
        }

        return root.ToJsonString(SerializerOptions);
    }

    private static bool HasIndustryProfile(JsonObject root, string industry)
    {
        return root[IndustryProfilesPropertyName] is JsonObject industryProfiles &&
               industryProfiles.ContainsKey(industry);
    }
}
