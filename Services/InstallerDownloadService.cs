// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Locates a versioned installer file below the configured download folder. The path is built only from constants and a
/// version that matched InstallerDownload.VersionPattern, so no request value can reach a file outside that folder.
/// </summary>
/// <param name="configuration">Supplies Downloads:RootPath; defaults to /app/downloads</param>
using System.Text.RegularExpressions;
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public sealed partial class InstallerDownloadService(IConfiguration configuration) : IInstallerDownloadService
{
    public string? FindWindowsSetup(string version)
    {
        if (string.IsNullOrEmpty(version) || !VersionRegex().IsMatch(version))
        {
            return null;
        }

        var rootPath = configuration[InstallerDownload.RootPathKey];
        if (string.IsNullOrWhiteSpace(rootPath))
        {
            rootPath = InstallerDownload.DefaultRootPath;
        }

        var path = Path.Combine(rootPath, InstallerDownload.WindowsFolder, version, InstallerDownload.WindowsSetupFileName);
        return File.Exists(path) ? path : null;
    }

    [GeneratedRegex(InstallerDownload.VersionPattern, RegexOptions.CultureInvariant)]
    private static partial Regex VersionRegex();
}
