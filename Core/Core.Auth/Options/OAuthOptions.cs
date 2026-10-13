namespace Core.Auth.Options;

public sealed class OAuthOptions
{
    public const string SectionName = "OAuth";

    public string Authority { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public bool RequireHttpsMetadata { get; init; } = true;

    /// <summary>
    /// Public (secret-less) client used by Swagger UI for the authorization_code + PKCE login flow.
    /// Leave empty to disable the interactive login in Swagger UI (manual bearer token paste still works).
    /// </summary>
    public string SwaggerClientId { get; init; } = string.Empty;
}
