// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// DTO for paginated search results of region packages.
/// </summary>
namespace Klacks.Marketplace.Api.DTOs;

public class RegionPackageSearchResultDto
{
    public List<RegionPackageListItemDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
