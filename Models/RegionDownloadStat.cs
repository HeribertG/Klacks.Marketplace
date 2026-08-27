// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// One aggregated row of the admin download dashboard: how many downloads a country/artifact combination had on a given day.
/// </summary>
/// <param name="Date">Calendar day the downloads happened on (UTC)</param>
/// <param name="ArtifactType">Downloaded artifact kind from RegionArtifactType constants</param>
/// <param name="Count">Number of downloads for this day/country/artifact combination</param>
namespace Klacks.Marketplace.Models;

public class RegionDownloadStat
{
    public DateTime Date { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string ArtifactType { get; set; } = string.Empty;
    public int Count { get; set; }
}
