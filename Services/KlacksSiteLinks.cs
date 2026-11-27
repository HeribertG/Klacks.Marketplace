// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Builds culture-aware links to the Klacks website. The website serves German without a language
/// prefix, every other culture under its lower-case code ("/en", "/zh-cn"), and serves the company pages below the
/// culture's default country ("/en/land-gb/impressum"); the country-less form only redirects there, so the store links
/// the target directly. The default countries mirror LanguageCountries.DefaultCountryFor in Klacks.Marketing. The docs site uses
/// Docusaurus locale codes, which differ from the culture code only for Chinese.
/// </summary>
/// <param name="cultureCode">UI culture of the current request, e.g. "de", "en" or "zh-CN"</param>
/// <param name="pageSlug">Company-wide website page, e.g. "impressum"</param>
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public static class KlacksSiteLinks
{
    private static readonly Dictionary<string, string> DocsLocaleOverrides = new(StringComparer.OrdinalIgnoreCase)
    {
        ["zh-CN"] = "zh-Hans",
        ["zh-TW"] = "zh-Hant",
    };

    private static readonly Dictionary<string, string> DefaultCountryPages = new(StringComparer.OrdinalIgnoreCase)
    {
        ["de"] = "land-ch",
        ["en"] = "land-gb",
        ["fr"] = "land-fr",
        ["it"] = "land-it",
        ["ar"] = "land-sa",
        ["cs"] = "land-cz",
        ["da"] = "land-dk",
        ["el"] = "land-gr",
        ["es"] = "land-es",
        ["fi"] = "land-fi",
        ["he"] = "land-il",
        ["id"] = "land-id",
        ["ja"] = "land-jp",
        ["ko"] = "land-kr",
        ["ms"] = "land-my",
        ["nb"] = "land-no",
        ["nl"] = "land-nl",
        ["pl"] = "land-pl",
        ["pt"] = "land-pt",
        ["ro"] = "land-ro",
        ["sv"] = "land-se",
        ["th"] = "land-th",
        ["vi"] = "land-vn",
        ["zh-CN"] = "land-cn",
        ["zh-TW"] = "land-tw",
    };

    public static string CompanyPage(string cultureCode, string pageSlug)
    {
        var countryPage = DefaultCountryPages.TryGetValue(cultureCode ?? string.Empty, out var country)
            ? $"/{country}"
            : string.Empty;

        return $"{ExternalLinks.WebsiteUrl}{CulturePrefix(cultureCode)}{countryPage}/{pageSlug}";
    }

    public static string Docs(string cultureCode)
    {
        if (IsUnprefixed(cultureCode))
        {
            return $"{ExternalLinks.WebsiteUrl}{ExternalLinks.DocsPath}";
        }

        var docsLocale = DocsLocaleOverrides.GetValueOrDefault(cultureCode, cultureCode);

        return $"{ExternalLinks.WebsiteUrl}{ExternalLinks.DocsPath}{docsLocale}/";
    }

    private static string CulturePrefix(string cultureCode)
        => IsUnprefixed(cultureCode) ? string.Empty : $"/{cultureCode.ToLowerInvariant()}";

    private static bool IsUnprefixed(string cultureCode)
        => string.IsNullOrWhiteSpace(cultureCode)
           || cultureCode.Equals(ExternalLinks.UnprefixedWebsiteCulture, StringComparison.OrdinalIgnoreCase);
}
