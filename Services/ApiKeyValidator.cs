// Copyright (c) Heribert Gasparoli Private. All rights reserved.

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
