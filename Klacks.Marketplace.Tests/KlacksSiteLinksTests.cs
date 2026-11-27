// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Services;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// Company page links go straight to the culture's default country page instead of the country-less redirect.
/// </summary>
[TestFixture]
public class KlacksSiteLinksTests
{
    [TestCase("de", "https://klacks-software.ch/land-ch/impressum")]
    [TestCase("en", "https://klacks-software.ch/en/land-gb/impressum")]
    [TestCase("fr", "https://klacks-software.ch/fr/land-fr/impressum")]
    [TestCase("zh-CN", "https://klacks-software.ch/zh-cn/land-cn/impressum")]
    [TestCase("ja", "https://klacks-software.ch/ja/land-jp/impressum")]
    public void Company_page_links_the_default_country_directly(string culture, string expected)
    {
        KlacksSiteLinks.CompanyPage(culture, ExternalLinks.ImprintPageSlug).ShouldBe(expected);
    }

    [Test]
    public void Unknown_culture_falls_back_to_the_country_less_link()
    {
        KlacksSiteLinks.CompanyPage("xx", ExternalLinks.ImprintPageSlug).ShouldBe("https://klacks-software.ch/xx/impressum");
    }
}
