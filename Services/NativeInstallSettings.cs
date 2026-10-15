// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Switch and download targets of the "office PC without Docker" install option on the region page.
/// The option is public once NativeInstall:Enabled is true; before that only signed-in marketplace admins see it as a
/// preview. Windows defaults to the office-PC package in the rolling Klacks.Api GitHub release tagged "office-pc", which CI
/// replaces after every green native build (never "latest", which is empty while a release is still building). The Mac
/// button only appears when NativeInstall:MacUrl is configured (no macOS package exists yet; until then the card says "coming soon"). The region page links to the
/// counted redirect in RegionPackagesApiController, which takes its target exclusively from these settings.
/// Klacks.Marketing/Localization/NativeInstallSettings.cs still points at the planned installer names.
/// </summary>
/// <param name="Enabled">Whether the option is shown to everybody</param>
/// <param name="WindowsUrl">Download URL of the Windows x64 office-PC package; NativeInstall:WindowsUrl overrides the rolling release URL</param>
/// <param name="MacUrl">Download URL of the macOS package for Apple silicon; null hides the button</param>
using System.Security.Claims;
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public sealed record NativeInstallSettings(bool Enabled, string WindowsUrl, string? MacUrl)
{
    public const string EnabledKey = "NativeInstall:Enabled";
    public const string WindowsUrlKey = "NativeInstall:WindowsUrl";
    public const string MacUrlKey = "NativeInstall:MacUrl";
    public const string AdminRole = "Admin";

    private const string RollingReleaseDownloadBaseUrl = "https://github.com/HeribertG/Klacks.Api/releases/download/office-pc/";
    private const string WindowsPackageFileName = "klacks-office-pc-win-x64.zip";

    public static NativeInstallSettings From(IConfiguration configuration)
        => new(
            configuration.GetValue(EnabledKey, false),
            NonEmpty(configuration[WindowsUrlKey]) ?? RollingReleaseDownloadBaseUrl + WindowsPackageFileName,
            NonEmpty(configuration[MacUrlKey]));

    public bool IsVisibleFor(ClaimsPrincipal? user) => Enabled || IsAdminPreview(user);

    public bool IsAdminPreview(ClaimsPrincipal? user) => !Enabled && (user?.IsInRole(AdminRole) ?? false);

    public string? DownloadUrlFor(string platform) => platform switch
    {
        NativeInstallPlatform.Windows => WindowsUrl,
        NativeInstallPlatform.Mac => MacUrl,
        _ => null
    };

    private static string? NonEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
