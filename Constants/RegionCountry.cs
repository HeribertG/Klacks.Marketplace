// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Maps the 30 supported region package country codes (ISO 3166-1 alpha-2, lowercase) to their English country names.
/// </summary>
namespace Klacks.Marketplace.Constants;

public static class RegionCountry
{
    public const int CountryCodeLength = 2;

    public static bool IsValidCountryCode(string countryCode)
    {
        return countryCode.Length == CountryCodeLength && countryCode.All(c => c is >= 'a' and <= 'z');
    }

    public static readonly IReadOnlyDictionary<string, string> CountryNames = new Dictionary<string, string>
    {
        ["ae"] = "United Arab Emirates",
        ["at"] = "Austria",
        ["ch"] = "Switzerland",
        ["cn"] = "China",
        ["cz"] = "Czech Republic",
        ["de"] = "Germany",
        ["dk"] = "Denmark",
        ["es"] = "Spain",
        ["fi"] = "Finland",
        ["fr"] = "France",
        ["gb"] = "United Kingdom",
        ["gr"] = "Greece",
        ["id"] = "Indonesia",
        ["il"] = "Israel",
        ["it"] = "Italy",
        ["jp"] = "Japan",
        ["kr"] = "South Korea",
        ["li"] = "Liechtenstein",
        ["my"] = "Malaysia",
        ["nl"] = "Netherlands",
        ["no"] = "Norway",
        ["pl"] = "Poland",
        ["pt"] = "Portugal",
        ["ro"] = "Romania",
        ["sa"] = "Saudi Arabia",
        ["se"] = "Sweden",
        ["th"] = "Thailand",
        ["tw"] = "Taiwan",
        ["us"] = "United States",
        ["vn"] = "Vietnam"
    };
}
