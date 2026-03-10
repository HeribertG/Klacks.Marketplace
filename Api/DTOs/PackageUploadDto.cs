// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// DTO für den Upload eines Sprachpakets via REST-API.
/// </summary>
/// <param name="Code">Sprachcode des Pakets (z.B. "es")</param>
/// <param name="Name">Englischer Name der Sprache</param>
/// <param name="DisplayName">Anzeigename in der Sprache selbst</param>
/// <param name="SpeechLocale">Locale für Spracherkennung (z.B. "es-ES")</param>
/// <param name="Version">Versionsnummer des Pakets</param>
/// <param name="Coverage">Übersetzungsabdeckung in Prozent</param>
/// <param name="Description">Beschreibung des Sprachpakets</param>
/// <param name="MinKlacksVersion">Mindestversion von Klacks</param>
/// <param name="ManifestJson">JSON-String des Manifests</param>
/// <param name="TranslationsJson">JSON-String der Übersetzungen</param>
/// <param name="DocsJson">Optionaler JSON-String der Dokumentation</param>
/// <param name="CountriesJson">Optionaler JSON-String der Länderdaten</param>
/// <param name="StatesJson">Optionaler JSON-String der Kantonsdaten</param>
/// <param name="CalendarRulesJson">Optionaler JSON-String der Kalenderregeln</param>
namespace Klacks.Marketplace.Api.DTOs;

public class PackageUploadDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string SpeechLocale { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0.0";
    public double Coverage { get; set; }
    public string Description { get; set; } = string.Empty;
    public string MinKlacksVersion { get; set; } = "1.0.0";
    public string ManifestJson { get; set; } = string.Empty;
    public string TranslationsJson { get; set; } = string.Empty;
    public string? DocsJson { get; set; }
    public string? CountriesJson { get; set; }
    public string? StatesJson { get; set; }
    public string? CalendarRulesJson { get; set; }
}
