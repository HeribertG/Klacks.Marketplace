// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// The installer download only ever resolves RootPath/windows/X.Y.Z/ plus one of the allowlisted setup names
/// (Klacks-Setup-win-x64.exe, Klacks-Setup-win-arm64.exe); anything else is not found.
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
        File.WriteAllText(Path.Combine(versionFolder, InstallerDownload.WindowsSetupX64FileName), "setup x64");
        File.WriteAllText(Path.Combine(versionFolder, InstallerDownload.WindowsSetupArm64FileName), "setup arm64");
        File.WriteAllText(Path.Combine(versionFolder, "Klacks-Setup-win-x86.exe"), "not served");
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

    [TestCase(InstallerDownload.WindowsSetupX64FileName)]
    [TestCase(InstallerDownload.WindowsSetupArm64FileName)]
    public void Finds_each_allowlisted_setup_of_an_existing_version(string fileName)
    {
        _service.FindWindowsSetup("1.0.66", fileName)
            .ShouldBe(Path.Combine(_root, InstallerDownload.WindowsFolder, "1.0.66", fileName));
    }

    [Test]
    public void Allowlist_holds_exactly_the_x64_and_arm64_setup()
    {
        InstallerDownload.WindowsSetupFileNames.ShouldBe(["Klacks-Setup-win-x64.exe", "Klacks-Setup-win-arm64.exe"]);
    }

    [Test]
    public void Unknown_version_is_not_found()
    {
        _service.FindWindowsSetup("1.0.67", InstallerDownload.WindowsSetupX64FileName).ShouldBeNull();
    }

    [TestCase("Klacks-Setup-win-x86.exe")]
    [TestCase("klacks-setup-win-x64.exe")]
    [TestCase("KLACKS-SETUP-WIN-ARM64.EXE")]
    [TestCase("Klacks-Setup-win-x64.exe ")]
    [TestCase("Klacks-Setup-win-x64.exe.")]
    [TestCase("../../secret.txt")]
    [TestCase("..")]
    [TestCase("secret.txt")]
    [TestCase("")]
    public void File_name_outside_the_allowlist_is_not_found(string fileName)
    {
        _service.FindWindowsSetup("1.0.66", fileName).ShouldBeNull();
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
        _service.FindWindowsSetup(version, InstallerDownload.WindowsSetupX64FileName).ShouldBeNull();
    }
}
