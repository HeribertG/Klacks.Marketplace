// Copyright (c) Heribert Gasparoli Private. All rights reserved.

namespace Klacks.Marketplace.Constants;

public static class AppConstants
{
    public const string AuthCookieName = "MarketplaceAuth";
    public const int MaxUploadSizeBytes = 5 * 1024 * 1024;
    public const int PageSize = 12;
    public const int MinPasswordLength = 8;
    public const string ManifestFileName = "manifest.json";
    public const string TranslationsFileName = "translations.json";
    public const string DocsFileName = "docs.json";
    public const string DatabaseFileName = "marketplace.db";

    public static readonly string[] AllowedManualNames =
    [
        "scheduling-rule-manual",
        "calendar-rule-manual",
        "report-manual",
        "macro-manual",
        "identity-provider-manual"
    ];
}
