// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Switch and download targets of the "office PC without Docker" install option on the region page.
/// The option stays hidden until NativeInstall:Enabled is true, i.e. until signed native installers are
/// published as Klacks.Api release assets; the URLs default to those assets. Mirrors
/// Klacks.Marketing/Localization/NativeInstallSettings.cs.
/// </summary>
/// <param name="Enabled">Whether the option is shown at all</param>
/// <param name="WindowsUrl">Download URL of the Windows x64 installer</param>
/// <param name="MacUrl">Download URL of the macOS installer for Apple silicon</param>
namespace Klacks.Marketplace.Services;

public sealed record NativeInstallSettings(bool Enabled, string WindowsUrl, string MacUrl)
{
    public const string EnabledKey = "NativeInstall:Enabled";
    public const string WindowsUrlKey = "NativeInstall:WindowsUrl";
    public const string MacUrlKey = "NativeInstall:MacUrl";

    private const string ReleaseDownloadBaseUrl = "https://github.com/HeribertG/Klacks.Api/releases/latest/download/";
    private const string WindowsInstallerFileName = "klacks-setup-win-x64.exe";
    private const string MacInstallerFileName = "klacks-setup-osx-arm64.pkg";

    public static NativeInstallSettings From(IConfiguration configuration)
        => new(
            configuration.GetValue(EnabledKey, false),
            NonEmpty(configuration[WindowsUrlKey]) ?? ReleaseDownloadBaseUrl + WindowsInstallerFileName,
            NonEmpty(configuration[MacUrlKey]) ?? ReleaseDownloadBaseUrl + MacInstallerFileName);

    private static string? NonEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}
