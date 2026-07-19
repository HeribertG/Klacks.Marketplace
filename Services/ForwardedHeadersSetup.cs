// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Configures forwarded-header processing for the reverse-proxy (nginx) deployment so middleware
/// such as the download rate limiter partitions on the real client IP instead of the proxy IP.
/// Trusts loopback (framework default) and the private RFC1918 ranges (covers a local or
/// containerized nginx) and additionally any proxies/networks listed in configuration; requests
/// arriving directly from public addresses stay untrusted, so their X-Forwarded-For headers are
/// ignored and cannot spoof rate-limit partitions.
/// </summary>
/// <param name="options">Forwarded-headers options to configure</param>
/// <param name="configuration">App configuration providing optional extra proxies and networks</param>
using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using IPNetwork = System.Net.IPNetwork;

namespace Klacks.Marketplace.Services;

public static class ForwardedHeadersSetup
{
    public const string KnownProxiesConfigPath = "ForwardedHeaders:KnownProxies";
    public const string KnownNetworksConfigPath = "ForwardedHeaders:KnownNetworks";

    private static readonly string[] PrivateNetworkRanges =
    [
        "10.0.0.0/8",
        "172.16.0.0/12",
        "192.168.0.0/16"
    ];

    public static void Configure(ForwardedHeadersOptions options, IConfiguration configuration)
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

        foreach (var range in PrivateNetworkRanges)
        {
            options.KnownIPNetworks.Add(IPNetwork.Parse(range));
        }

        foreach (var proxy in configuration.GetSection(KnownProxiesConfigPath).Get<string[]>() ?? [])
        {
            if (IPAddress.TryParse(proxy, out var address))
            {
                options.KnownProxies.Add(address);
            }
        }

        foreach (var network in configuration.GetSection(KnownNetworksConfigPath).Get<string[]>() ?? [])
        {
            if (IPNetwork.TryParse(network, out var parsed))
            {
                options.KnownIPNetworks.Add(parsed);
            }
        }
    }
}
