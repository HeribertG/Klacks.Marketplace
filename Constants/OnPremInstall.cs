// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// File names, commands and paths of the on-prem (Docker) installation, shared by the bundle builder and the Linux
/// instructions on the region page so the shown command always matches the script inside the ZIP.
/// </summary>
namespace Klacks.Marketplace.Constants;

public static class OnPremInstall
{
    public const string ShellWrapperNameFormat = "install-{0}.sh";
    public const string LinuxInstallCommandFormat = "sudo bash ./" + ShellWrapperNameFormat;
    public const string LinuxInstallFolderExample = "~/klacks";
    public const string DockerInstallConfirmAnswer = "y";
    public const string LocalBrowserUrl = "https://localhost";
}
