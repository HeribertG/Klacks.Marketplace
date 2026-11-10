// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// The installer download only ever resolves RootPath/windows/X.Y.Z/Klacks-Setup-win-x64.exe; anything else is not found.
/// </summary>
[TestFixture]
public class InstallerDownloadServiceTests
{
    private string _root = string.Empty;
    private InstallerDownloadService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _root = Path.Combine(Path.GetTempPath(), "klacks-downloads-" + Guid.NewGuid().ToString("N"));
        var versionFolder = Path.Combine(_root, InstallerDownload.WindowsFolder, "1.0.66");
        Directory.CreateDirectory(versionFolder);
        File.WriteAllText(Path.Combine(versionFolder, InstallerDownload.WindowsSetupFileName), "setup");
        File.WriteAllText(Path.Combine(_root, "secret.txt"), "secret");

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { [InstallerDownload.RootPathKey] = _root })
            .Build();
        _service = new InstallerDownloadService(configuration);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(_root, recursive: true);
    }

    [Test]
    public void Finds_the_setup_of_an_existing_version()
    {
        _service.FindWindowsSetup("1.0.66")
            .ShouldBe(Path.Combine(_root, InstallerDownload.WindowsFolder, "1.0.66", InstallerDownload.WindowsSetupFileName));
    }

    [Test]
    public void Unknown_version_is_not_found()
    {
        _service.FindWindowsSetup("1.0.67").ShouldBeNull();
    }

    [TestCase("")]
    [TestCase("latest")]
    [TestCase("1.0")]
    [TestCase("1.0.66.1")]
    [TestCase("1.0.66 ")]
    [TestCase("1.0.66\n")]
    [TestCase("..")]
    [TestCase("../..")]
    [TestCase("1.0.66/../..")]
    [TestCase("12345.0.0")]
    [TestCase("١.٠.٦٦")]
    public void Malformed_version_is_not_found(string version)
    {
        _service.FindWindowsSetup(version).ShouldBeNull();
    }
}
