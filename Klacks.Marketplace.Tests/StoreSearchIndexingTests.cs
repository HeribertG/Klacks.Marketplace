// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Services;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// Only the catalogue and the public detail pages are indexable, with a canonical on the public store address;
/// account, admin, upload and ticket pages are noindex.
/// </summary>
[TestFixture]
public class StoreSearchIndexingTests
{
    [TestCase("/")]
    [TestCase("")]
    [TestCase(null)]
    [TestCase("/package/de-CH")]
    [TestCase("/plugin/messaging")]
    [TestCase("/region/CH")]
    public void Public_pages_are_indexable(string? path)
    {
        StoreSearchIndexing.IsIndexable(path).ShouldBeTrue();
    }

    [TestCase("/login")]
    [TestCase("/register")]
    [TestCase("/admin")]
    [TestCase("/my-packages")]
    [TestCase("/upload")]
    [TestCase("/upload-plugin")]
    [TestCase("/upload-region")]
    [TestCase("/tickets")]
    [TestCase("/tickets/new")]
    [TestCase("/tickets/7")]
    [TestCase("/package/")]
    [TestCase("/unknown")]
    public void Other_pages_are_noindex(string path)
    {
        StoreSearchIndexing.IsIndexable(path).ShouldBeFalse();
    }

    [Test]
    public void Missing_detail_page_answering_404_is_noindex()
    {
        StoreSearchIndexing.IsIndexable("/region/xx", 404).ShouldBeFalse();
    }

    [Test]
    public void Mark_not_found_sets_404_before_the_response_starts()
    {
        var context = new Microsoft.AspNetCore.Http.DefaultHttpContext();

        StoreSearchIndexing.MarkNotFound(context);

        context.Response.StatusCode.ShouldBe(404);
    }

    [Test]
    public void Mark_not_found_ignores_a_missing_context()
    {
        Should.NotThrow(() => StoreSearchIndexing.MarkNotFound(null));
    }

    [TestCase("/", "https://klacks-software.ch/store/")]
    [TestCase(null, "https://klacks-software.ch/store/")]
    [TestCase("/package/de-CH", "https://klacks-software.ch/store/package/de-CH")]
    public void Canonical_points_to_the_public_store_address(string? path, string expected)
    {
        StoreSearchIndexing.CanonicalUrl(path).ShouldBe(expected);
    }
}
