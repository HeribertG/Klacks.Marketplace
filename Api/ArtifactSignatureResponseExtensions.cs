// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

/// <summary>
/// Attaches the vendor signature of a download artifact to the response (header X-Klacks-Signature). The
/// one place all download endpoints use, so the header name and the unsigned fallback cannot drift apart.
/// Region packages are signed over the exact served bytes (earlier scheme, kept for installed clients);
/// language packs and plugins are signed over the identity-bound statement of
/// <see cref="ArtifactSignatureStatement"/>.
/// </summary>
/// <param name="response">Response the signed artifact is written to</param>
/// <param name="signingService">Vendor signing service; returns null in unsigned mode</param>
/// <param name="payload">Exact bytes served as the response body</param>
/// <param name="artifactType">Artifact type bound into the statement (plugin or language pack)</param>
/// <param name="name">Plugin name or language code bound into the statement</param>
using Klacks.Marketplace.Constants;
using Klacks.Marketplace.Services;

namespace Klacks.Marketplace.Api;

public static class ArtifactSignatureResponseExtensions
{
    public static void AppendArtifactSignature(this HttpResponse response, IArtifactSigningService signingService, byte[] payload)
    {
        AppendSignature(response, signingService.SignPayload(payload));
    }

    public static void AppendBoundArtifactSignature(
        this HttpResponse response,
        IArtifactSigningService signingService,
        string artifactType,
        string name,
        byte[] payload)
    {
        AppendSignature(response, signingService.SignPayload(ArtifactSignatureStatement.Build(artifactType, name, payload)));
    }

    private static void AppendSignature(HttpResponse response, string? signature)
    {
        if (signature is not null)
        {
            response.Headers.Append(AppConstants.SignatureHeaderName, signature);
        }
    }
}
