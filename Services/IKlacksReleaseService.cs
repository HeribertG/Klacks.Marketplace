// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Provides the current stable Klacks version, i.e. the version the store's installers set up.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IKlacksReleaseService
{
    Task<string?> GetLatestStableVersionAsync(CancellationToken cancellationToken = default);
}
