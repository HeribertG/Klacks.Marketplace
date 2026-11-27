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

    [TestCase("/", "https://klacks-software.ch/store/")]
    [TestCase(null, "https://klacks-software.ch/store/")]
    [TestCase("/package/de-CH", "https://klacks-software.ch/store/package/de-CH")]
    public void Canonical_points_to_the_public_store_address(string? path, string expected)
    {
        StoreSearchIndexing.CanonicalUrl(path).ShouldBe(expected);
    }
}
