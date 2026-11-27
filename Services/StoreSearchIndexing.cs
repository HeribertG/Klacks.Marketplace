// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Decides how a store page presents itself to search engines. Only the catalogue and the public package, plugin and
/// region pages are indexable; they get a canonical URL on the public store address without query string, so the
/// "?culture=" variants and the legacy port-only host collapse onto one URL. Every other page (login, register,
/// admin, uploads, tickets, own packages, not found) is marked noindex.
/// </summary>
/// <param name="path">Request path below the store's path base, e.g. "/package/de-CH"</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public static class StoreSearchIndexing
{
    public const string NoIndexRobotsContent = "noindex, nofollow";

    private const string CatalogPath = "/";

    private static readonly string[] IndexablePathPrefixes = ["/package/", "/plugin/", "/region/"];

    public static bool IsIndexable(string? path)
    {
        if (string.IsNullOrEmpty(path) || path == CatalogPath)
        {
            return true;
        }

        return IndexablePathPrefixes.Any(prefix =>
            path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && path.Length > prefix.Length);
    }

    public static string CanonicalUrl(string? path)
        => $"{ExternalLinks.PublicStoreUrl}{(string.IsNullOrEmpty(path) ? CatalogPath : path)}";
}
