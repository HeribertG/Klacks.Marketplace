// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// In-memory implementation of ILoginTokenService: tokens are cryptographically random, live for 60 seconds, and are removed on first read so a captured URL cannot be replayed.
/// </summary>
/// <param name="cache">Process-wide memory cache backing the token store</param>
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace Klacks.Marketplace.Services;

public class LoginTokenService : ILoginTokenService
{
    private const int TokenByteLength = 32;
    private static readonly TimeSpan TokenLifetime = TimeSpan.FromSeconds(60);

    private readonly IMemoryCache _cache;

    public LoginTokenService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string IssueToken(int userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(TokenByteLength))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        _cache.Set(CacheKey(token), userId, TokenLifetime);
        return token;
    }

    public int? ConsumeToken(string token)
    {
        var key = CacheKey(token);
        if (_cache.TryGetValue(key, out int userId))
        {
            _cache.Remove(key);
            return userId;
        }

        return null;
    }

    private static string CacheKey(string token) => $"login-token:{token}";
}
