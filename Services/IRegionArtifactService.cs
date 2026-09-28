// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Service interface for assembling downloadable region deployment bundles (on-prem and compose ZIPs) in memory.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionArtifactService
{
    byte[] BuildOnPremBundle(string countryCode, string patchedProfileJson);

    byte[] BuildComposeBundle(string countryCode, string patchedProfileJson);
}
