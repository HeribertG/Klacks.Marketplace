// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Sets the culture cookie and redirects back. Required because Blazor Server
/// cannot set HTTP cookies directly from SignalR circuit.
/// </summary>
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Klacks.Marketplace.Api;

[Route("[controller]")]
public class CultureController : Controller
{
    [HttpGet("Set")]
    public IActionResult Set(string culture, string redirectUri)
    {
        if (!string.IsNullOrWhiteSpace(culture))
        {
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                });
        }

        if (string.IsNullOrWhiteSpace(redirectUri) || !Url.IsLocalUrl(redirectUri))
            redirectUri = "/";

        return LocalRedirect(redirectUri);
    }
}
