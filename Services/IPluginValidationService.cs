// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

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
