// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using System.Security.Cryptography;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Models;
using Microsoft.EntityFrameworkCore;

namespace Klacks.Marketplace.Services;

public class AuthService : IAuthService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    private readonly MarketplaceDbContext _db;

    public AuthService(MarketplaceDbContext db)
    {
        _db = db;
    }

    public async Task<User> RegisterAsync(string email, string password, string displayName)
    {
        if (await _db.Users.AnyAsync(u => u.Email == email))
        {
            throw new InvalidOperationException("Email already registered");
        }

        if (password.Length < AppConstants.MinPasswordLength)
        {
            throw new InvalidOperationException($"Password must be at least {AppConstants.MinPasswordLength} characters");
        }

        var user = new User
        {
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = HashPassword(password),
            DisplayName = displayName.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant().Trim());
        if (user is null || !VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }
        return user;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _db.Users.FindAsync(userId);
    }

    public async Task<User> GetOrCreateSystemUserAsync()
    {
        const string systemEmail = "system@klacks.app";
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == systemEmail);
        if (user is not null)
        {
            return user;
        }

        user = new User
        {
            Email = systemEmail,
            PasswordHash = HashPassword(Guid.NewGuid().ToString()),
            DisplayName = "System",
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        var parts = passwordHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);
        var testHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        return CryptographicOperations.FixedTimeEquals(hash, testHash);
    }
}
