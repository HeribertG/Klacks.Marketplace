// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Decides how a store page presents itself to search engines. Only the catalogue and the public package, plugin and
/// region pages are indexable; they get a canonical URL on the public store address without query string, so the
/// "?culture=" variants and the legacy port-only host collapse onto one URL. Every other page (login, register,
/// admin, uploads, tickets, own packages, not found) is marked noindex. A detail page whose package, plugin or region
/// does not exist answers 404 during prerendering, so it is noindex as well instead of a soft 404.
/// </summary>
/// <param name="path">Request path below the store's path base, e.g. "/package/de-CH"</param>
/// <param name="statusCode">Response status of the prerendered page</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public static class StoreSearchIndexing
{
    public const string NoIndexRobotsContent = "noindex, nofollow";

    private const string CatalogPath = "/";

    private static readonly string[] IndexablePathPrefixes = ["/package/", "/plugin/", "/region/"];

    public static void MarkNotFound(HttpContext? context)
    {
        if (context is { Response.HasStarted: false })
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }
    }

    public static bool IsIndexable(string? path, int statusCode = StatusCodes.Status200OK)
    {
        if (statusCode == StatusCodes.Status404NotFound)
        {
            return false;
        }

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
