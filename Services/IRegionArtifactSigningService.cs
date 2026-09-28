// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Service interface for signing region-package download artifacts with the vendor private key.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IRegionArtifactSigningService
{
    string? SignPayload(byte[] payload);
}
