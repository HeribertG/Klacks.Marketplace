// Copyright (c) Heribert Gasparoli Private. All rights reserved.

using System.Security.Claims;
using System.Threading.RateLimiting;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Data;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization();

builder.Services.AddDbContext<MarketplaceDbContext>(options =>
{
    var dbPath = Path.Combine(builder.Environment.ContentRootPath, "data", AppConstants.DatabaseFileName);
    options.UseSqlite($"Data Source={dbPath}");
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
    ForwardedHeadersSetup.Configure(options, builder.Configuration));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(AppConstants.DownloadRateLimitPolicyName, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? AppConstants.UnknownClientIp,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = AppConstants.DownloadRateLimitPermitLimit,
                Window = TimeSpan.FromSeconds(AppConstants.DownloadRateLimitWindowSeconds)
            }));
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
builder.Services.AddScoped<IFeaturePluginMarketplaceService, FeaturePluginMarketplaceService>();
builder.Services.AddScoped<IPluginValidationService, PluginValidationService>();
builder.Services.AddScoped<IRegionPackageService, RegionPackageService>();
builder.Services.AddScoped<IRegionProfileValidationService, RegionProfileValidationService>();
builder.Services.AddScoped<IRegionPackageSeedService, RegionPackageSeedService>();
builder.Services.AddScoped<IRegionProfilePatchService, RegionProfilePatchService>();
builder.Services.AddSingleton<IRegionArtifactService, RegionArtifactService>();
builder.Services.AddSingleton<IRegionArtifactSigningService, RegionArtifactSigningService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
    var dataDir = Path.Combine(app.Environment.ContentRootPath, "data");
    Directory.CreateDirectory(dataDir);
    await db.Database.MigrateAsync();

    if (!await db.Users.AnyAsync(u => u.IsAdmin))
    {
        var adminEmail = app.Configuration["AdminSettings:SeedAdminEmail"];
        var adminPassword = app.Configuration["AdminSettings:SeedAdminPassword"];
        var adminDisplayName = app.Configuration["AdminSettings:SeedAdminDisplayName"] ?? "Admin";

        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            var admin = await authService.RegisterAsync(adminEmail, adminPassword, adminDisplayName);
            admin.IsAdmin = true;
            await db.SaveChangesAsync();
        }
    }

    var regionSeedService = scope.ServiceProvider.GetRequiredService<IRegionPackageSeedService>();
    await regionSeedService.SeedAsync();
}

app.UseForwardedHeaders();

var pathBase = app.Configuration["PathBase"];
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

var supportedCultures = new[]
{
    "en", "de", "fr", "it",
    "ar", "cs", "da", "el", "es", "fi", "he", "id", "ja", "ko", "ms", "nb", "nl",
    "pl", "pt", "ro", "sv", "th", "vi", "zh-CN", "zh-TW"
};
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.RequestCultureProviders = new List<IRequestCultureProvider>
{
    new CookieRequestCultureProvider { CookieName = ".AspNetCore.Culture" },
    new AcceptLanguageHeaderRequestCultureProvider()
};

app.UseRequestLocalization(localizationOptions);
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
