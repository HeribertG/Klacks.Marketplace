// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Validation interface for region setup profile JSON content.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionProfileValidationService
{
    RegionProfileValidationResult ValidateProfileJson(string json);
}
