// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Service interface for signing marketplace download artifacts (region packages, language packs,
/// feature plugin bundles) with the vendor private key.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IArtifactSigningService
{
    string? SignPayload(byte[] payload);
}
