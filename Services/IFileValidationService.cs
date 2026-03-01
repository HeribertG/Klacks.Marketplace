// Copyright (c) Heribert Gasparoli Private. All rights reserved.

namespace Klacks.Marketplace.Services;

public interface IFileValidationService
{
    (bool IsValid, string ErrorMessage) ValidateManifest(string json);
    (bool IsValid, string ErrorMessage) ValidateTranslations(string json);
}
