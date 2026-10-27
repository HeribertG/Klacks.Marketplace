// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Services;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// Stored client IP addresses keep only a coarse prefix: IPv4 /24, IPv6 /48, anything that is not an address stays as it is.
/// </summary>
[TestFixture]
public class ClientIpAnonymizerTests
{
    [TestCase("203.0.113.57", "203.0.113.0")]
    [TestCase("203.0.113.0", "203.0.113.0")]
    [TestCase("2001:db8:abcd:1234:5678:9abc:def0:1234", "2001:db8:abcd::")]
    [TestCase("2001:db8:abcd::", "2001:db8:abcd::")]
    [TestCase("::ffff:198.51.100.77", "198.51.100.0")]
    [TestCase("::1", "::")]
    [TestCase("unknown", "unknown")]
    [TestCase("", "")]
    public void Anonymize_keeps_only_the_network_prefix(string input, string expected)
    {
        ClientIpAnonymizer.Anonymize(input).ShouldBe(expected);
    }

    [TestCase("203.0.113.57")]
    [TestCase("2001:db8:abcd:1234:5678:9abc:def0:1234")]
    public void Anonymize_is_idempotent(string input)
    {
        var once = ClientIpAnonymizer.Anonymize(input);

        ClientIpAnonymizer.Anonymize(once).ShouldBe(once);
    }
}
