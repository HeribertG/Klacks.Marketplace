// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Validation interface for region setup profile JSON content.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionProfileValidationService
{
    RegionProfileValidationResult ValidateProfileJson(string json);
}
