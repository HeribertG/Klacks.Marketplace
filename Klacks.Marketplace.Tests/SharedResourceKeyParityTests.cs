// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using System.Xml.Linq;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// Every store language carries the same resource keys as German, and the region page's Microsoft Store link has its
/// label in all 25 languages, so no visitor sees a missing text or a raw resource key. The Linux install card texts are
/// translated everywhere (no English fallback) and keep the placeholder for the folder, answer or address they show.
/// </summary>
[TestFixture]
public class SharedResourceKeyParityTests
{
    private const string ProjectFileName = "Klacks.Marketplace.csproj";
    private const string ResourcesFolder = "Resources";
    private const string ResourceFilePattern = "SharedResource.*.resx";
    private const string GermanResourceFile = "SharedResource.de.resx";
    private const string DataElement = "data";
    private const string NameAttribute = "name";
    private const string ValueElement = "value";
    private const string MicrosoftStoreKey = "RegionDetail_NativeMicrosoftStore";
    private const string MicrosoftStoreName = "Microsoft Store";
    private const int ExpectedLanguageCount = 25;
    private const string EnglishResourceFile = "SharedResource.en.resx";
    private const string LinuxKeyPrefix = "RegionDetail_Linux";
    private const string FirstPlaceholder = "{0}";
    private const int ExpectedLinuxKeyCount = 9;

    [Test]
    public void Every_language_has_exactly_the_german_keys()
    {
        var germanKeys = LoadTexts(Path.Combine(ResourcesDirectory(), GermanResourceFile)).Keys.ToHashSet();

        foreach (var file in ResourceFiles())
        {
            var keys = LoadTexts(file).Keys.ToHashSet();

            germanKeys.Except(keys).ShouldBeEmpty($"{Path.GetFileName(file)} misses keys");
            keys.Except(germanKeys).ShouldBeEmpty($"{Path.GetFileName(file)} has extra keys");
        }
    }

    [Test]
    public void Microsoft_store_label_exists_in_all_languages_and_names_the_store()
    {
        var files = ResourceFiles();

        files.Length.ShouldBe(ExpectedLanguageCount);

        foreach (var file in files)
        {
            var texts = LoadTexts(file);

            texts.ShouldContainKey(MicrosoftStoreKey, Path.GetFileName(file));
            texts[MicrosoftStoreKey].ShouldContain(MicrosoftStoreName, Case.Sensitive, Path.GetFileName(file));
        }
    }

    [Test]
    public void Linux_install_texts_are_translated_in_all_languages_and_keep_their_placeholders()
    {
        var files = ResourceFiles();
        var german = LoadTexts(Path.Combine(ResourcesDirectory(), GermanResourceFile));
        var english = LoadTexts(Path.Combine(ResourcesDirectory(), EnglishResourceFile));
        var linuxKeys = german.Keys.Where(key => key.StartsWith(LinuxKeyPrefix, StringComparison.Ordinal)).ToList();

        files.Length.ShouldBe(ExpectedLanguageCount);
        linuxKeys.Count.ShouldBe(ExpectedLinuxKeyCount);

        foreach (var file in files)
        {
            var fileName = Path.GetFileName(file);
            var texts = LoadTexts(file);

            foreach (var key in linuxKeys)
            {
                texts[key].ShouldNotBeNullOrWhiteSpace($"{fileName} {key}");
                PlaceholderCount(texts[key]).ShouldBe(PlaceholderCount(german[key]), $"{fileName} {key}");

                if (fileName != EnglishResourceFile)
                {
                    texts[key].ShouldNotBe(english[key], $"{fileName} {key} falls back to English");
                }
            }
        }
    }

    private static int PlaceholderCount(string text)
        => text.Split(FirstPlaceholder).Length - 1;

    private static string[] ResourceFiles()
        => Directory.GetFiles(ResourcesDirectory(), ResourceFilePattern);

    private static Dictionary<string, string> LoadTexts(string file)
        => XDocument.Load(file).Root!
            .Elements(DataElement)
            .ToDictionary(
                element => element.Attribute(NameAttribute)!.Value,
                element => element.Element(ValueElement)?.Value ?? string.Empty);

    private static string ResourcesDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, ProjectFileName)))
        {
            directory = directory.Parent;
        }

        directory.ShouldNotBeNull($"{ProjectFileName} not found above {AppContext.BaseDirectory}");

        return Path.Combine(directory.FullName, ResourcesFolder);
    }
}
