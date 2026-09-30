// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Builds culture-aware links to the Klacks website. The website serves German without a language
/// prefix, every other culture under its lower-case code ("/en", "/zh-cn"), and redirects the
/// country-less company pages ("/en/impressum") to the culture's default country. The docs site uses
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

    public static string CompanyPage(string cultureCode, string pageSlug)
        => $"{ExternalLinks.WebsiteUrl}{CulturePrefix(cultureCode)}/{pageSlug}";

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
