// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Models;

public class DownloadLog
{
    public int Id { get; set; }
    public int PackageId { get; set; }
    public LanguagePackage Package { get; set; } = null!;
    public DateTime DownloadedAt { get; set; } = DateTime.UtcNow;
    public string IpAddress { get; set; } = string.Empty;
}
