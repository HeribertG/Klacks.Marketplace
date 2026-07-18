// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service interface for assembling downloadable region deployment bundles (on-prem and compose ZIPs) in memory.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionArtifactService
{
    byte[] BuildOnPremBundle(string countryCode, string patchedProfileJson);

    byte[] BuildComposeBundle(string countryCode, string patchedProfileJson);
}
