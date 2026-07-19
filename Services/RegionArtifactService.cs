// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Assembles the on-prem and compose deployment ZIP bundles in memory from the bundled on-prem template and a patched region profile.
/// The template file list and contents are loaded from disk once per process and cached thread-safely.
/// </summary>
/// <param name="environment">Host environment used to resolve the bundled on-prem template directory</param>
/// <param name="countryCode">Validated lowercase two-letter country code of the region package</param>
/// <param name="patchedProfileJson">Region profile JSON already patched with package identity and industry selection</param>
using System.IO.Compression;
using System.Text;
using Klacks.Marketplace.Constants;

namespace Klacks.Marketplace.Services;

public class RegionArtifactService : IRegionArtifactService
{
    private const string RegionsDirectoryName = "regions";
    private const string NginxDirectoryName = "nginx";
    private const string CertsDirectoryName = "certs";
    private const string InitScriptsDirectoryName = "init-scripts";
    private const string ComposeFileName = "docker-compose.yml";
    private const string EnvExampleFileName = ".env.example";
    private const string ComposeReadmeFileName = "README-compose.md";
    private const string ShellScriptExtension = ".sh";
    private const string ProfileFileExtension = ".json";
    private const string AllFilesSearchPattern = "*";
    private const string ShellWrapperNameFormat = "install-{0}.sh";
    private const string PowerShellWrapperNameFormat = "install-{0}.ps1";
    private const string EntrySeparator = "/";
    private const string UnixLineEnding = "\n";
    private const string WindowsLineEnding = "\r\n";
    private const int UnixExecutableFileMode = 0x81ED;
    private const int UnixRegularFileMode = 0x81A4;
    private const int UnixDirectoryMode = 0x41ED;
    private const int ExternalAttributesShift = 16;

    private readonly IWebHostEnvironment _environment;
    private readonly Lazy<IReadOnlyList<RegionTemplateFile>> _templateFiles;

    public RegionArtifactService(IWebHostEnvironment environment)
    {
        _environment = environment;
        _templateFiles = new Lazy<IReadOnlyList<RegionTemplateFile>>(LoadTemplateFiles, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public byte[] BuildOnPremBundle(string countryCode, string patchedProfileJson)
    {
        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            AddCachedEntries(archive, _templateFiles.Value);
            AddTextEntry(archive, BuildRegionProfileEntryName(countryCode), patchedProfileJson, isExecutable: false);
            AddTextEntry(archive, string.Format(ShellWrapperNameFormat, countryCode), BuildShellWrapper(countryCode), isExecutable: true);
            AddTextEntry(archive, string.Format(PowerShellWrapperNameFormat, countryCode), BuildPowerShellWrapper(countryCode), isExecutable: false);
        }

        return stream.ToArray();
    }

    public byte[] BuildComposeBundle(string countryCode, string patchedProfileJson)
    {
        var templateFiles = _templateFiles.Value;
        var nginxPrefix = NginxDirectoryName + EntrySeparator;
        var initScriptsPrefix = InitScriptsDirectoryName + EntrySeparator;

        using var stream = new MemoryStream();
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            AddCachedEntries(archive, templateFiles.Where(f => f.RelativePath == ComposeFileName));
            AddCachedEntries(archive, templateFiles.Where(f => f.RelativePath.StartsWith(nginxPrefix, StringComparison.Ordinal)));
            AddCachedEntries(archive, templateFiles.Where(f => f.RelativePath.StartsWith(initScriptsPrefix, StringComparison.Ordinal)));
            AddDirectoryEntry(archive, nginxPrefix + CertsDirectoryName + EntrySeparator);
            AddTextEntry(archive, BuildRegionProfileEntryName(countryCode), patchedProfileJson, isExecutable: false);
            AddTextEntry(archive, EnvExampleFileName, BuildEnvExample(countryCode), isExecutable: false);
            AddTextEntry(archive, ComposeReadmeFileName, BuildComposeReadme(countryCode), isExecutable: false);
        }

        return stream.ToArray();
    }

    private IReadOnlyList<RegionTemplateFile> LoadTemplateFiles()
    {
        var templateDirectory = Path.Combine(_environment.ContentRootPath, AppConstants.OnPremTemplateDirectory);
        if (!Directory.Exists(templateDirectory))
        {
            throw new InvalidOperationException($"On-prem template directory not found: {templateDirectory}");
        }

        return Directory.EnumerateFiles(templateDirectory, AllFilesSearchPattern, SearchOption.AllDirectories)
            .Select(filePath => new RegionTemplateFile(
                Path.GetRelativePath(templateDirectory, filePath).Replace(Path.DirectorySeparatorChar, '/'),
                File.ReadAllBytes(filePath)))
            .OrderBy(f => f.RelativePath, StringComparer.Ordinal)
            .ToList();
    }

    private static string BuildRegionProfileEntryName(string countryCode)
    {
        return RegionsDirectoryName + EntrySeparator + countryCode + ProfileFileExtension;
    }

    private static void AddCachedEntries(ZipArchive archive, IEnumerable<RegionTemplateFile> templateFiles)
    {
        foreach (var templateFile in templateFiles)
        {
            var entry = archive.CreateEntry(templateFile.RelativePath, CompressionLevel.Optimal);
            entry.ExternalAttributes = GetFileMode(templateFile.RelativePath) << ExternalAttributesShift;
            using var entryStream = entry.Open();
            entryStream.Write(templateFile.Content, 0, templateFile.Content.Length);
        }
    }

    private static void AddTextEntry(ZipArchive archive, string entryName, string content, bool isExecutable)
    {
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        var mode = isExecutable ? UnixExecutableFileMode : GetFileMode(entryName);
        entry.ExternalAttributes = mode << ExternalAttributesShift;
        using var entryStream = entry.Open();
        var bytes = Encoding.UTF8.GetBytes(content);
        entryStream.Write(bytes, 0, bytes.Length);
    }

    private static void AddDirectoryEntry(ZipArchive archive, string entryName)
    {
        var entry = archive.CreateEntry(entryName);
        entry.ExternalAttributes = UnixDirectoryMode << ExternalAttributesShift;
    }

    private static int GetFileMode(string entryName)
    {
        return entryName.EndsWith(ShellScriptExtension, StringComparison.Ordinal)
            ? UnixExecutableFileMode
            : UnixRegularFileMode;
    }

    private static string BuildShellWrapper(string countryCode)
    {
        return "#!/bin/sh" + UnixLineEnding +
               $"REGION={countryCode} exec ./install.sh \"$@\"" + UnixLineEnding;
    }

    private static string BuildPowerShellWrapper(string countryCode)
    {
        return $"& \"$PSScriptRoot\\install.ps1\" -Region {countryCode} @args" + WindowsLineEnding;
    }

    private static string BuildEnvExample(string countryCode)
    {
        return $"""
            # Klacks Docker Compose configuration.
            # Copy this file to .env, fill in the secrets, then run: docker compose up -d

            COMPOSE_PROJECT_NAME=klacks

            SERVER_NAME=localhost
            HTTP_PORT=80
            HTTPS_PORT=443

            REGION_SETUP_FILE=/app/regions/{countryCode}.json

            POSTGRES_PASSWORD=
            JWT_SECRET=

            KLACKS_API_TAG=latest
            KLACKS_UI_TAG=latest
            KLACKS_UPDATER_TAG=latest

            UPDATE_MANIFEST_BASE_URL=https://github.com/HeribertG/Klacks.Api/releases/latest/download
            UPDATE_SIGNATURE_PUBLIC_KEY=
            UPDATE_REQUIRE_SIGNED_REGION_PACKAGES=false
            KLACKS_MARKETPLACE_URL=https://klacks-software.ch/store
            """.ReplaceLineEndings(UnixLineEnding) + UnixLineEnding;
    }

    private static string BuildComposeReadme(string countryCode)
    {
        return $"""
            # Klacks Docker Compose bundle ({countryCode})

            Minimal Klacks stack for Docker-experienced operators. The region profile
            `regions/{countryCode}.json` is applied once on first boot via `REGION_SETUP_FILE`.

            ## Setup

            1. Copy `.env.example` to `.env`.
            2. Set the secrets in `.env`: `POSTGRES_PASSWORD`, `JWT_SECRET` and the vendor
               `UPDATE_SIGNATURE_PUBLIC_KEY` (single line, newlines escaped as `\n`).
            3. Adjust `SERVER_NAME`, `HTTP_PORT` and `HTTPS_PORT` if needed.
            4. Place a TLS certificate (`server.crt` and `server.key`) into `nginx/certs/`.
            5. Start the stack: `docker compose up -d`.

            The first boot creates, migrates and seeds the database and applies the region
            profile. For an installer-guided setup use the On-Prem bundle instead.
            """.ReplaceLineEndings(UnixLineEnding) + UnixLineEnding;
    }
}
