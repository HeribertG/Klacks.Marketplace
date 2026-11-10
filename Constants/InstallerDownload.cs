// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Fixed names of the versioned installer downloads that the store serves directly from disk (no redirect), for example as
/// the package URL of the Microsoft Store listing. CI writes each file once into RootPath/windows/X.Y.Z/ on the server and
/// never replaces it, because the Microsoft Store rejects a package URL whose binary changes after submission.
/// </summary>
/// <param name="RootPathKey">Configuration key of the folder the download files live in (mounted read-only into the container)</param>
/// <param name="VersionPattern">Accepted version route value: three dot-separated numbers, nothing else</param>
namespace Klacks.Marketplace.Constants;

public static class InstallerDownload
{
    public const string RootPathKey = "Downloads:RootPath";
    public const string DefaultRootPath = "/app/downloads";
    public const string WindowsFolder = "windows";
    public const string WindowsSetupFileName = "Klacks-Setup-win-x64.exe";
    public const string WindowsSetupContentType = "application/vnd.microsoft.portable-executable";
    public const string VersionPattern = @"^[0-9]{1,4}\.[0-9]{1,4}\.[0-9]{1,4}\z";
}
