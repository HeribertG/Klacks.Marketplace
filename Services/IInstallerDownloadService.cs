// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Services;

public interface IInstallerDownloadService
{
    string? FindWindowsSetup(string version);
}
