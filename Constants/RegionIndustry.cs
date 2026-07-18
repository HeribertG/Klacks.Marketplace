// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Canonical industry slugs used by region setup profiles plus the "all" selector for downloads.
/// The five slugs mirror the industryProfiles keys shipped in every Klacks region profile.
/// </summary>
namespace Klacks.Marketplace.Constants;

public static class RegionIndustry
{
    public const string Homecare = "homecare";
    public const string Healthcare = "healthcare";
    public const string Security = "security";
    public const string Facility = "facility";
    public const string Logistics = "logistics";
    public const string All = "all";

    public static readonly string[] CanonicalSlugs =
    [
        Homecare,
        Healthcare,
        Security,
        Facility,
        Logistics
    ];

    public static readonly string[] AllOptions =
    [
        Homecare,
        Healthcare,
        Security,
        Facility,
        Logistics,
        All
    ];
}
