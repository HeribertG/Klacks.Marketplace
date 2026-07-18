// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service interface for the idempotent startup seeding of bundled region profiles.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionPackageSeedService
{
    Task SeedAsync();
}
