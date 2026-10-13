using Microsoft.AspNetCore.Authorization;

namespace Core.Auth.Policies;

public static class ScopeAuthorizationPolicyBuilderExtensions
{
    private const string ScopeClaimType = "scope";

    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, string scope)
        => builder.RequireAssertion(context => context.User.Claims
            .Where(x => x.Type == ScopeClaimType)
            .SelectMany(x => x.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Contains(scope));
}
