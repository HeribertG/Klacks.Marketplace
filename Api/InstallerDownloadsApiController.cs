// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Serves versioned installer files directly from disk with range support and without a redirect. This is the package URL of
/// the Microsoft Store listing: Partner Center rejects package URLs that redirect (GitHub release downloads always do).
/// </summary>
/// <param name="installerDownloadService">Resolves the file of a version; unknown or malformed versions give 404</param>
/// <param name="logger">Logs each served download (version only, no client data)</param>
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Klacks.Marketplace.Api;

[ApiController]
[Route("api/downloads")]
public class InstallerDownloadsApiController(
    IInstallerDownloadService installerDownloadService,
    ILogger<InstallerDownloadsApiController> logger) : ControllerBase
{
    [HttpGet(InstallerDownload.WindowsFolder + "/{version}/" + InstallerDownload.WindowsSetupFileName)]
    [HttpHead(InstallerDownload.WindowsFolder + "/{version}/" + InstallerDownload.WindowsSetupFileName)]
    [EnableRateLimiting(AppConstants.DownloadRateLimitPolicyName)]
    public IActionResult WindowsSetup(string version)
    {
        var path = installerDownloadService.FindWindowsSetup(version);
        if (path is null)
        {
            return NotFound();
        }

        logger.LogInformation("Serving Windows setup {Version}.", version);
        return PhysicalFile(path, InstallerDownload.WindowsSetupContentType, InstallerDownload.WindowsSetupFileName,
            enableRangeProcessing: true);
    }
}
