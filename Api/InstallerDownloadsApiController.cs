// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Serves versioned installer files directly from disk with range support and without a redirect. This is the package URL of
/// the Microsoft Store listing: Partner Center rejects package URLs that redirect (GitHub release downloads always do).
/// </summary>
/// <param name="installerDownloadService">Resolves the file of a version; unknown or malformed versions and file names outside
/// InstallerDownload.WindowsSetupFileNames give 404</param>
/// <param name="logger">Logs each served download (version and file name only, no client data)</param>
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
    [HttpGet(InstallerDownload.WindowsFolder + "/{version}/{fileName}")]
    [HttpHead(InstallerDownload.WindowsFolder + "/{version}/{fileName}")]
    [EnableRateLimiting(AppConstants.DownloadRateLimitPolicyName)]
    public IActionResult WindowsSetup(string version, string fileName)
    {
        var path = installerDownloadService.FindWindowsSetup(version, fileName);
        if (path is null)
        {
            return NotFound();
        }

        var servedFileName = Path.GetFileName(path);
        logger.LogInformation("Serving Windows setup {Version} {FileName}.", version, servedFileName);
        return PhysicalFile(path, InstallerDownload.WindowsSetupContentType, servedFileName, enableRangeProcessing: true);
    }
}
