// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Builds the statement a language-pack or plugin signature covers: "{artifactType}\n{name}\n{sha256hex}"
/// (UTF-8, name lower-case, SHA-256 of the exact response bytes as lower-case hex). Binding type and name
/// into the signed bytes stops a signed plugin A from being served as plugin B, or a language pack from
/// being served as another language. Region packages keep the earlier raw-bytes signature for backward
/// compatibility. Klacks.Api rebuilds the identical statement in PackageSignatureStatement; both sides
/// pin the format with the same literal test.
/// </summary>
using System.Security.Cryptography;
using System.Text;

namespace Klacks.Marketplace.Services;

public static class ArtifactSignatureStatement
{
    public const string PluginArtifactType = "plugin";

    public const string LanguagePackArtifactType = "language-pack";

    private const char Separator = '\n';

    public static byte[] Build(string artifactType, string name, byte[] payload)
    {
        var hash = Convert.ToHexStringLower(SHA256.HashData(payload));
        var statement = string.Concat(artifactType, Separator, name.ToLowerInvariant(), Separator, hash);
        return Encoding.UTF8.GetBytes(statement);
    }
}
