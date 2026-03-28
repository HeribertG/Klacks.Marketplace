// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Razor Page endpoint to set the culture cookie and redirect back.
/// Blazor Server cannot set HTTP cookies directly from SignalR.
/// </summary>
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Klacks.Marketplace.Pages.Culture;

public class SetCultureModel : PageModel
{
    public IActionResult OnGet(string culture, string redirectUri)
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

        return LocalRedirect(redirectUri ?? "/");
    }
}
