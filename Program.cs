// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using System.Security.Claims;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<MarketplaceDbContext>(options =>
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "data", AppConstants.DatabaseFileName);
    options.UseSqlite($"Data Source={dbPath}");
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = AppConstants.AuthCookieName;
        options.LoginPath = "/login";
        options.LogoutPath = "/auth/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IFileValidationService, FileValidationService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
    var dataDir = Path.Combine(app.Environment.ContentRootPath, "data");
    Directory.CreateDirectory(dataDir);
    await db.Database.MigrateAsync();

    if (!await db.Users.AnyAsync(u => u.IsAdmin))
    {
        var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
        var adminEmail = app.Configuration["AdminSettings:SeedAdminEmail"] ?? "admin@klacks.app";
        var adminPassword = app.Configuration["AdminSettings:SeedAdminPassword"] ?? "Admin123!";
        var adminDisplayName = app.Configuration["AdminSettings:SeedAdminDisplayName"] ?? "Admin";

        var admin = await authService.RegisterAsync(adminEmail, adminPassword, adminDisplayName);
        admin.IsAdmin = true;
        await db.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
