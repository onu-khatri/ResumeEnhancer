using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.WebSolution.ModulesComposition.Authorization;

public sealed class EndpointAuthorizationStartupValidator
{
    public void Validate(IEnumerable<EndpointDataSource> dataSources)
    {
        var apiEndpoints = dataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Where(endpoint => endpoint.RoutePattern.RawText?.StartsWith("/api/", StringComparison.OrdinalIgnoreCase) == true)
            .ToArray();

        foreach (var endpoint in apiEndpoints)
        {
            var guest = endpoint.Metadata.GetMetadata<GuestAccessMetadata>();
            var protectedAccess = endpoint.Metadata.GetMetadata<ProtectedAccessMetadata>();
            var allowsAnonymous = endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null;

            if (guest is null && protectedAccess is null)
            {
                throw new InvalidOperationException($"API endpoint '{endpoint.DisplayName}' has no access classification.");
            }

            if (guest is not null && protectedAccess is not null)
            {
                throw new InvalidOperationException($"API endpoint '{endpoint.DisplayName}' has conflicting access metadata.");
            }

            if (protectedAccess is not null && allowsAnonymous)
            {
                throw new InvalidOperationException($"Protected API endpoint '{endpoint.DisplayName}' cannot allow anonymous access.");
            }

            if (guest is not null && guest.AllowedRoles.Count == 0)
            {
                throw new InvalidOperationException($"Guest endpoint '{endpoint.DisplayName}' has no allowed role.");
            }
        }
    }
}
