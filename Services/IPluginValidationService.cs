// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Validation interface for feature plugin manifest and i18n file content.
/// </summary>
namespace Klacks.Marketplace.Services;

public interface IPluginValidationService
{
    (bool IsValid, string ErrorMessage) ValidatePluginManifest(string json);
    (bool IsValid, string ErrorMessage) ValidateI18nFile(string json);
    (bool IsValid, string ErrorMessage) ValidatePluginBundle(byte[] zipData);
}
