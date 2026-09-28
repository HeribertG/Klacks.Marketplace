// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Service interface for the idempotent startup seeding of bundled region profiles.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionPackageSeedService
{
    Task SeedAsync();
}
