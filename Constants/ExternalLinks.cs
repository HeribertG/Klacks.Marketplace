// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// External links shown in the store footer: the Klacks website (legal pages, docs), the store's own
/// source code (offered to every user as required by section 13 of the AGPL-3.0) and the community.
/// Mirrors Klacks.Marketing/Localization/ExternalLinks.cs and the Klacks.Ui footer.
/// </summary>
namespace Klacks.Marketplace.Constants;

public static class ExternalLinks
{
    public const string WebsiteUrl = "https://klacks-software.ch";
    public const string WebsiteLabel = "klacks-software.ch";
    public const string DocsPath = "/docs/";
    public const string SourceCodeUrl = "https://github.com/HeribertG/Klacks.Marketplace";
    public const string DiscordUrl = "https://discord.gg/YRP8p2abVC";
    public const string DiscordLabel = "Discord";
    public const string ImprintPageSlug = "impressum";
    public const string PrivacyPageSlug = "datenschutz";
    public const string LicensePageSlug = "lizenz";
    public const string UnprefixedWebsiteCulture = "de";
}
