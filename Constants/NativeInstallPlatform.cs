// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Platform route values of the counted "office PC without Docker" download redirect and the artifact type each one is
/// logged with in the region download log. Kept apart from RegionArtifactType, whose list whitelists the profile download.
/// </summary>
/// <param name="ArtifactTypeByPlatform">Maps a lower-case platform route value to its download log artifact type</param>
namespace Klacks.Marketplace.Constants;

public static class NativeInstallPlatform
{
    public const string Windows = "windows";
    public const string Mac = "mac";
    public const string WindowsArtifactType = "officePcWindows";
    public const string MacArtifactType = "officePcMac";

    public static readonly IReadOnlyDictionary<string, string> ArtifactTypeByPlatform = new Dictionary<string, string>
    {
        [Windows] = WindowsArtifactType,
        [Mac] = MacArtifactType
    };
}
