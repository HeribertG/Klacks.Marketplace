// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Result of a region profile validation containing hard errors and non-blocking warnings.
/// </summary>
/// <param name="Errors">Validation failures that reject the upload</param>
/// <param name="Warnings">Non-blocking findings such as unknown industry profile keys</param>
namespace Klacks.Marketplace.Services;

public class RegionProfileValidationResult
{
    public List<string> Errors { get; } = [];
    public List<string> Warnings { get; } = [];
    public bool IsValid => Errors.Count == 0;
}
