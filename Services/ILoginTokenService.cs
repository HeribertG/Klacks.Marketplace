// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Issues and consumes short-lived, single-use tokens that carry a validated user id across the redirect needed to turn a Blazor Server login into a real cookie sign-in.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface ILoginTokenService
{
    string IssueToken(int userId);
    int? ConsumeToken(string token);
}
