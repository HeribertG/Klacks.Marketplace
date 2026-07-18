// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Allowed artifact type values for region package downloads: patched profile JSON, on-prem installation ZIP and compose ZIP.
/// </summary>
namespace Klacks.Marketplace.Constants;

public static class RegionArtifactType
{
    public const string ProfileJson = "profileJson";
    public const string OnPremZip = "onpremZip";
    public const string ComposeZip = "composeZip";

    public static readonly string[] AllArtifactTypes =
    [
        ProfileJson,
        OnPremZip,
        ComposeZip
    ];
}
