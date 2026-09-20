using System.Collections.Frozen;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace ResumeEnhancer.Core.WebLibrary.Authorization;

public sealed record GuestAccessMetadata(
    string AccessProfileCode,
    IReadOnlySet<string> AllowedRoles);

public sealed record ProtectedAccessMetadata(
    IReadOnlySet<string> RequiredRoles,
    IReadOnlySet<string> RequiredCapabilities,
    IReadOnlySet<string> RequiredEntitlements);

public static class EndpointAccessConventionExtensions
{
    public static TBuilder AllowGuest<TBuilder>(
        this TBuilder builder,
        string accessProfileCode,
        params string[] allowedRoles)
        where TBuilder : IEndpointConventionBuilder
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessProfileCode);
        ArgumentNullException.ThrowIfNull(allowedRoles);

        var roles = allowedRoles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .ToFrozenSet(StringComparer.Ordinal);

        if (roles.Count == 0)
        {
            throw new ArgumentException("At least one guest role is required.", nameof(allowedRoles));
        }

        builder.AllowAnonymous();
        builder.WithMetadata(new GuestAccessMetadata(accessProfileCode, roles));
        return builder;
    }

    public static TBuilder RequireProtectedAccess<TBuilder>(
        this TBuilder builder,
        IEnumerable<string>? requiredRoles = null,
        IEnumerable<string>? requiredCapabilities = null,
        IEnumerable<string>? requiredEntitlements = null)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.WithMetadata(new ProtectedAccessMetadata(
            ToSet(requiredRoles),
            ToSet(requiredCapabilities),
            ToSet(requiredEntitlements)));
        return builder;
    }

    private static IReadOnlySet<string> ToSet(IEnumerable<string>? values) =>
        (values ?? []).Where(value => !string.IsNullOrWhiteSpace(value))
            .ToFrozenSet(StringComparer.Ordinal);
}
