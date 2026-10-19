namespace Core.Auth.Options;

public sealed class OAuthOptions
{
    public const string SectionName = "OAuth";

    public string Authority { get; init; } = string.Empty;

    /// <summary>
    /// Optional discovery document URL used by the API to fetch metadata/JWKS when <see cref="Authority"/>
    /// is not reachable from the API itself (e.g. API in a container, Authority is the browser-facing host).
    /// Leave empty to derive it from <see cref="Authority"/>.
    /// </summary>
    public string MetadataAddress { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public bool RequireHttpsMetadata { get; init; } = true;

    /// <summary>
    /// Public (secret-less) client used by Swagger UI for the authorization_code + PKCE login flow.
    /// Leave empty to disable the interactive login in Swagger UI (manual bearer token paste still works).
    /// </summary>
    public string SwaggerClientId { get; init; } = string.Empty;
}
