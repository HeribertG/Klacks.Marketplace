// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Validates upload API keys using a constant-time comparison to prevent timing attacks.
/// </summary>
/// <param name="providedKey">API key value taken from the request header</param>
/// <param name="configuredKey">API key value configured for the marketplace</param>
using System.Security.Cryptography;
using System.Text;

namespace Klacks.Marketplace.Services;

public static class ApiKeyValidator
{
    public static bool IsValid(string? providedKey, string? configuredKey)
    {
        if (string.IsNullOrEmpty(providedKey) || string.IsNullOrEmpty(configuredKey))
        {
            return false;
        }

        var providedBytes = Encoding.UTF8.GetBytes(providedKey);
        var configuredBytes = Encoding.UTF8.GetBytes(configuredKey);
        return CryptographicOperations.FixedTimeEquals(providedBytes, configuredBytes);
    }
}
