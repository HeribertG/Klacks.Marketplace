// Copyright (c) Heribert Gasparoli Private. All rights reserved.

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
