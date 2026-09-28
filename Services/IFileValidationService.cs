// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Services;

public interface IFileValidationService
{
    (bool IsValid, string ErrorMessage) ValidateManifest(string json);
    (bool IsValid, string ErrorMessage) ValidateTranslations(string json);
    (bool IsValid, string ErrorMessage) ValidateDocs(string json);
    (bool IsValid, string ErrorMessage) ValidateCountriesJson(string json);
    (bool IsValid, string ErrorMessage) ValidateStatesJson(string json);
    (bool IsValid, string ErrorMessage) ValidateCalendarRulesJson(string json);
}
