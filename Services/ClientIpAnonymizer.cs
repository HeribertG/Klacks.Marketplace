// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

using System.Net;

namespace Klacks.Marketplace.Services;

/// <summary>
/// Truncates a client IP address before it is stored, so download logs keep only a coarse network prefix:
/// IPv4 loses the last octet (/24), IPv6 keeps the first 48 bits (/48). Values that are not an IP address
/// (for example "unknown") are returned unchanged. Applying it twice gives the same result.
/// </summary>
public static class ClientIpAnonymizer
{
    private const int Ipv4AddressLength = 4;
    private const int Ipv6KeptBytes = 6;

    public static string Anonymize(string ipAddress)
    {
        if (!IPAddress.TryParse(ipAddress, out var parsed))
        {
            return ipAddress;
        }

        if (parsed.IsIPv4MappedToIPv6)
        {
            parsed = parsed.MapToIPv4();
        }

        var bytes = parsed.GetAddressBytes();
        if (bytes.Length == Ipv4AddressLength)
        {
            bytes[^1] = 0;
        }
        else
        {
            Array.Clear(bytes, Ipv6KeptBytes, bytes.Length - Ipv6KeptBytes);
        }

        return new IPAddress(bytes).ToString();
    }
}
