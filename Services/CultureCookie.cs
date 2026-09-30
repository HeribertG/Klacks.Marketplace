// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Writes the culture cookie that the Blazor circuit reads its UI culture from. A "?culture=xx" link
/// (e.g. from the Klacks website) only reaches the prerender request; the circuit connects without the
/// query string, so a supported query culture is persisted into the cookie to keep the language.
/// </summary>
/// <param name="culture">Supported culture code, e.g. "de" or "zh-CN"</param>
/// <param name="supportedCultures">Culture codes the store is localized into</param>
using Microsoft.AspNetCore.Localization;

namespace Klacks.Marketplace.Services;

public static class CultureCookie
{
    public const string QueryParameterName = "culture";
    private const int LifetimeYears = 1;

    public static void Append(HttpResponse response, string culture)
    {
        response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture, culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(LifetimeYears),
                IsEssential = true,
                SameSite = SameSiteMode.Lax
            });
    }

    public static IApplicationBuilder UseQueryCulturePersistence(this IApplicationBuilder app, IReadOnlyCollection<string> supportedCultures)
    {
        return app.Use(async (context, next) =>
        {
            var requested = context.Request.Query[QueryParameterName].ToString();
            var supported = supportedCultures.FirstOrDefault(c => c.Equals(requested, StringComparison.OrdinalIgnoreCase));
            if (supported is not null)
            {
                Append(context.Response, supported);
            }

            await next();
        });
    }
}
