using Microsoft.AspNetCore.Http;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.Core.WebLibrary.Authorization;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.Core.CommonLibrary.Exceptions;

namespace ResumeEnhancer.WebSolution.ModulesComposition.Authorization;

public sealed class EndpointAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IProfilingAuthorizationService profilingAuthorization,
        IEntitlementResolver entitlementResolver,
        IAuthAuditRecorder auditRecorder)
    {
        var endpoint = context.GetEndpoint();
        var correlationId = context.TraceIdentifier;
        var guestMetadata = endpoint?.Metadata.GetMetadata<GuestAccessMetadata>();
        var protectedMetadata = endpoint?.Metadata.GetMetadata<ProtectedAccessMetadata>();

        if (guestMetadata is not null && !await IsGuestAllowedAsync(
                guestMetadata,
                profilingAuthorization,
                context.RequestAborted))
        {
            await auditRecorder.RecordAsync("authorization_denied", context.User.GetSubjectId(), context.Connection.RemoteIpAddress?.ToString(),
                System.Text.Json.JsonSerializer.Serialize(new { outcome = "denied", route = context.Request.Path.ToString() }), context.RequestAborted, correlationId);
            await ApiProblemDetails.WriteAsync(context, "AUTH_FORBIDDEN", StatusCodes.Status403Forbidden);
            return;
        }

        if (protectedMetadata is not null && context.User.Identity?.IsAuthenticated == true)
        {
            var subjectId = context.User.GetSubjectId();
            if (subjectId is null || !await IsProtectedAccessAllowedAsync(
                    protectedMetadata,
                    subjectId.Value,
                    profilingAuthorization,
                    entitlementResolver,
                    context.RequestAborted))
            {
                await auditRecorder.RecordAsync("authorization_denied", context.User.GetSubjectId(), context.Connection.RemoteIpAddress?.ToString(),
                    System.Text.Json.JsonSerializer.Serialize(new { outcome = "denied", route = context.Request.Path.ToString() }), context.RequestAborted, correlationId);
                await ApiProblemDetails.WriteAsync(context, "AUTH_FORBIDDEN", StatusCodes.Status403Forbidden);
                return;
            }
        }

        await next(context);
    }

    private static async Task<bool> IsGuestAllowedAsync(
        GuestAccessMetadata metadata,
        IProfilingAuthorizationService profilingAuthorization,
        CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = await profilingAuthorization.GetGuestAuthorizationAsync(cancellationToken);
            return snapshot is not null
                && snapshot.AccessProfileCodes.Contains(metadata.AccessProfileCode)
                && metadata.AllowedRoles.Any(snapshot.RoleCodes.Contains);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<bool> IsProtectedAccessAllowedAsync(
        ProtectedAccessMetadata metadata,
        int subjectId,
        IProfilingAuthorizationService profilingAuthorization,
        IEntitlementResolver entitlementResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = await profilingAuthorization.GetUserAuthorizationAsync(subjectId, cancellationToken);
            if (snapshot is null || snapshot.IsDeactivated || snapshot.IsDeleted)
            {
                return false;
            }

            if (!metadata.RequiredRoles.All(snapshot.RoleCodes.Contains)
                || !metadata.RequiredCapabilities.All(snapshot.Capabilities.Contains))
            {
                return false;
            }

            if (metadata.RequiredEntitlements.Count == 0)
            {
                return true;
            }

            var entitlements = await entitlementResolver.ResolveAsync(subjectId, cancellationToken);
            return metadata.RequiredEntitlements.All(entitlements.Contains);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return false;
        }
    }
}
