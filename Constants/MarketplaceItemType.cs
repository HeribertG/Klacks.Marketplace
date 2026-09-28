// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Distinguishes between language packages, feature plugins and region packages in the marketplace.
/// </summary>
namespace Klacks.Marketplace.Constants;

public enum MarketplaceItemType
{
    LanguagePackage,
    FeaturePlugin,
    RegionPackage
}
