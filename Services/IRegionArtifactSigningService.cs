// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Service interface for signing region-package download artifacts with the vendor private key.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionArtifactSigningService
{
    string? SignPayload(byte[] payload);
}
