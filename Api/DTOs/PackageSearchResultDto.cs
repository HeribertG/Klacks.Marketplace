// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// DTO für paginierte Suchergebnisse von Sprachpaketen.
/// </summary>
namespace Klacks.Marketplace.Api.DTOs;

public class PackageSearchResultDto
{
    public List<PackageListItemDto> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
