// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using Klacks.Marketplace.Models;

namespace Klacks.Marketplace.Services;

public interface IAuthService
{
    Task<User> RegisterAsync(string email, string password, string displayName);
    Task<User?> LoginAsync(string email, string password);
    Task<User?> GetUserByIdAsync(int userId);
}
