// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Reads "latestVersion" from the signed stable.json manifest of the latest Klacks.Api release, the same manifest the
/// on-prem installer pins its image tags from. The result is cached; a failed fetch yields null so the page simply
/// omits the version instead of showing a wrong one.
/// </summary>
/// <param name="httpClientFactory">Creates the client for the GitHub release download</param>
/// <param name="cache">Holds the last fetched version (or the failure) for a short time</param>
/// <param name="logger">Logs failed fetches</param>
using System.Text.Json;
using Klacks.Marketplace.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace Klacks.Marketplace.Services;

public sealed class KlacksReleaseService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    ILogger<KlacksReleaseService> logger) : IKlacksReleaseService
{
    public const string HttpClientName = "klacks-release";
    public static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(5);
    private const string CacheKey = "klacks-release:latest-stable-version";
    private const string LatestVersionProperty = "latestVersion";
    private static readonly TimeSpan SuccessCacheDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan FailureCacheDuration = TimeSpan.FromMinutes(2);

    public async Task<string?> GetLatestStableVersionAsync(CancellationToken cancellationToken = default)
    {
        if (cache.TryGetValue(CacheKey, out string? cached))
        {
            return cached;
        }

        var version = await FetchAsync(cancellationToken);
        cache.Set(CacheKey, version, version is null ? FailureCacheDuration : SuccessCacheDuration);
        return version;
    }

    private async Task<string?> FetchAsync(CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HttpClientName);
            await using var stream = await client.GetStreamAsync(ExternalLinks.StableManifestUrl, cancellationToken);
            using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            if (document.RootElement.TryGetProperty(LatestVersionProperty, out var element)
                && element.ValueKind == JsonValueKind.String
                && Version.TryParse(element.GetString(), out var parsed))
            {
                return parsed.ToString();
            }

            logger.LogWarning("stable.json has no valid {Property}", LatestVersionProperty);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or JsonException)
        {
            logger.LogWarning(ex, "Could not read the latest Klacks version from {Url}", ExternalLinks.StableManifestUrl);
        }

        return null;
    }
}
