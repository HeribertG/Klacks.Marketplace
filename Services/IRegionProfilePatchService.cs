// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service interface for patching region profile JSON with package identity and industry selection before delivery.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionProfilePatchService
{
    string PatchProfileJson(string profileJson, string countryCode, string packageVersion, string industry);
}
