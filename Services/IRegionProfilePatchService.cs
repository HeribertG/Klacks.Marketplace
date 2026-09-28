// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Service interface for patching region profile JSON with package identity and industry selection before delivery.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionProfilePatchService
{
    string PatchProfileJson(string profileJson, string countryCode, string packageVersion, string industry);
}
