// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using System.Xml.Linq;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// Every store language carries the same resource keys as German, and the region page's Microsoft Store link has its
/// label in all 25 languages, so no visitor sees a missing text or a raw resource key.
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
