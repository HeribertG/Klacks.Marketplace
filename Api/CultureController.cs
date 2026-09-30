// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Sets the culture cookie and redirects back. Required because Blazor Server
/// cannot set HTTP cookies directly from SignalR circuit.
/// The return path is relative to the app base and resolved with "~/", so the redirect keeps the
/// PathBase ("/store" in production) instead of landing on the website root.
/// </summary>
/// <param name="culture">Culture code to store in the culture cookie, e.g. "de"</param>
/// <param name="redirectUri">Page to return to, relative to the app base, e.g. "package/de" or ""</param>
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Mvc;

namespace Klacks.Marketplace.Api;

[Route("[controller]")]
public class CultureController : Controller
{
    private const string AppRootPath = "~/";

    [HttpGet("Set")]
    public IActionResult Set(string culture, string redirectUri)
    {
        if (!string.IsNullOrWhiteSpace(culture))
        {
            CultureCookie.Append(HttpContext.Response, culture);
        }

        var target = AppRootPath + (redirectUri ?? string.Empty).TrimStart('/');
        if (!Url.IsLocalUrl(target))
            target = AppRootPath;

        return LocalRedirect(target);
    }
}
