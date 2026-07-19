// Copyright (c) Heribert Gasparoli Private. All rights reserved.

/// <summary>
/// Signs region-package download artifacts with RSA-SHA256 (PKCS#1 v1.5) using the vendor private
/// key from configuration (RegionSigning:PrivateKeyPem, PEM; a single-line value may escape
/// newlines as \n) — the same key pair whose public half Klacks installations hold as
/// Update:SignaturePublicKey. When no key is configured the marketplace serves unsigned downloads
/// (logged once as a warning) so existing installations keep working; a signing failure yields an
/// unsigned response instead of a failed download.
/// </summary>
/// <param name="configuration">App configuration providing the vendor private key PEM</param>
/// <param name="logger">Logger for diagnostic output</param>
using System.Security.Cryptography;

namespace Klacks.Marketplace.Services;

public class RegionArtifactSigningService : IRegionArtifactSigningService
{
    public const string PrivateKeyConfigPath = "RegionSigning:PrivateKeyPem";

    private const string EscapedNewline = "\\n";
    private const string Newline = "\n";

    private readonly string? _privateKeyPem;
    private readonly ILogger<RegionArtifactSigningService> _logger;

    public RegionArtifactSigningService(IConfiguration configuration, ILogger<RegionArtifactSigningService> logger)
    {
        _logger = logger;

        var rawKey = configuration.GetValue<string>(PrivateKeyConfigPath);
        _privateKeyPem = string.IsNullOrWhiteSpace(rawKey) ? null : rawKey.Replace(EscapedNewline, Newline);

        if (_privateKeyPem == null)
        {
            _logger.LogWarning("No region signing private key configured — region package downloads are served unsigned.");
        }
    }

    public string? SignPayload(byte[] payload)
    {
        if (_privateKeyPem == null)
        {
            return null;
        }

        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(_privateKeyPem);

            var signature = rsa.SignData(payload, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signature);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sign region package artifact — serving the download unsigned.");
            return null;
        }
    }
}
