// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Allowed category values for feature plugins in the marketplace.
/// </summary>
namespace Klacks.Marketplace.Constants;

public static class PluginCategory
{
    public const string Communication = "communication";
    public const string Erp = "erp";
    public const string Accounting = "accounting";
    public const string Reporting = "reporting";
    public const string Integration = "integration";
    public const string Other = "other";

    public static readonly string[] AllCategories =
    [
        Communication,
        Erp,
        Accounting,
        Reporting,
        Integration,
        Other
    ];
}
