// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

namespace Klacks.Marketplace.Tests;

using System.Security.Cryptography;
using System.Text;
using Klacks.Marketplace.Api;
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Shouldly;

/// <summary>
/// The marketplace signs region, language-pack and plugin downloads with one service: the signature
/// verifies against the public half of the configured key over the exact bytes, a key given as a
/// single line with escaped newlines works, and without a key the download stays unsigned (no header).
/// </summary>
[TestFixture]
public class ArtifactSigningServiceTests
{
    private const int RsaKeySizeBits = 2048;
    private const string EscapedNewline = "\\n";

    private static readonly byte[] Payload = Encoding.UTF8.GetBytes("""{ "code": "xx", "translations": {} }""");

    private RSA _rsa = null!;

    [SetUp]
    public void SetUp()
    {
        _rsa = RSA.Create(RsaKeySizeBits);
    }

    [TearDown]
    public void TearDown()
    {
        _rsa.Dispose();
    }

    [Test]
    public void SignPayload_with_configured_key_produces_a_signature_the_public_key_verifies()
    {
        var service = CreateService(_rsa.ExportPkcs8PrivateKeyPem());

        var signature = service.SignPayload(Payload);

        signature.ShouldNotBeNull();
        _rsa.VerifyData(Payload, Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            .ShouldBeTrue();
    }

    [Test]
    public void SignPayload_signature_does_not_verify_for_other_bytes()
    {
        var service = CreateService(_rsa.ExportPkcs8PrivateKeyPem());

        var signature = service.SignPayload(Payload)!;
        var tampered = Payload.Concat(new byte[] { 0x20 }).ToArray();

        _rsa.VerifyData(tampered, Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            .ShouldBeFalse();
    }

    [Test]
    public void SignPayload_accepts_a_single_line_key_with_escaped_newlines()
    {
        var singleLine = _rsa.ExportPkcs8PrivateKeyPem().Replace("\r\n", "\n").Replace("\n", EscapedNewline);
        singleLine.Contains('\n').ShouldBeFalse();
        singleLine.ShouldContain(EscapedNewline);
        var service = CreateService(singleLine);

        var signature = service.SignPayload(Payload);

        signature.ShouldNotBeNull();
        _rsa.VerifyData(Payload, Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            .ShouldBeTrue();
    }

    [Test]
    public void SignPayload_without_configured_key_returns_null()
    {
        var service = CreateService(null);

        service.SignPayload(Payload).ShouldBeNull();
    }

    [Test]
    public void AppendArtifactSignature_with_key_sets_the_signature_header()
    {
        var service = CreateService(_rsa.ExportPkcs8PrivateKeyPem());
        var context = new DefaultHttpContext();

        context.Response.AppendArtifactSignature(service, Payload);

        var header = context.Response.Headers[AppConstants.SignatureHeaderName].ToString();
        header.ShouldNotBeNullOrWhiteSpace();
        _rsa.VerifyData(Payload, Convert.FromBase64String(header), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            .ShouldBeTrue();
    }

    [Test]
    public void Statement_has_the_format_klacks_api_verifies()
    {
        var statement = ArtifactSignatureStatement.Build("plugin", "Demo-Plugin", Encoding.UTF8.GetBytes("abc"));

        Encoding.UTF8.GetString(statement).ShouldBe(
            "plugin\ndemo-plugin\nba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");
    }

    [Test]
    public void AppendBoundArtifactSignature_signs_the_statement_not_the_raw_bytes()
    {
        var service = CreateService(_rsa.ExportPkcs8PrivateKeyPem());
        var context = new DefaultHttpContext();

        context.Response.AppendBoundArtifactSignature(service, ArtifactSignatureStatement.PluginArtifactType, "demo", Payload);

        var signature = Convert.FromBase64String(context.Response.Headers[AppConstants.SignatureHeaderName].ToString());
        var statement = ArtifactSignatureStatement.Build(ArtifactSignatureStatement.PluginArtifactType, "demo", Payload);
        var otherName = ArtifactSignatureStatement.Build(ArtifactSignatureStatement.PluginArtifactType, "other", Payload);
        _rsa.VerifyData(statement, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).ShouldBeTrue();
        _rsa.VerifyData(Payload, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).ShouldBeFalse();
        _rsa.VerifyData(otherName, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).ShouldBeFalse();
    }

    [Test]
    public void AppendArtifactSignature_without_key_sets_no_header()
    {
        var service = CreateService(null);
        var context = new DefaultHttpContext();

        context.Response.AppendArtifactSignature(service, Payload);

        context.Response.Headers.ContainsKey(AppConstants.SignatureHeaderName).ShouldBeFalse();
    }

    private static ArtifactSigningService CreateService(string? privateKeyPem)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [ArtifactSigningService.PrivateKeyConfigPath] = privateKeyPem
            })
            .Build();

        return new ArtifactSigningService(configuration, NullLogger<ArtifactSigningService>.Instance);
    }
}
