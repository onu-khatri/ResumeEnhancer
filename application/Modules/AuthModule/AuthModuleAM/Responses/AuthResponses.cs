namespace ResumeEnhancer.AuthModule.AM.Responses;

public sealed record AuthTokens(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc,
    DateTime RefreshTokenExpiresAtUtc
);

public sealed record BootstrapResponse(
    string SessionState,
    string EmailVerificationState,
    string DefaultPlanCode,
    string WorkspaceRoute,
    string Source,
    string? SelectedTemplateId,
    string NextAction,
    IReadOnlyDictionary<string, bool> FeatureEntitlements,
    IReadOnlyList<string> StateCodes
)
{
    public static string ContractVersion => "v1";
}

public sealed record RegisterResponse(int UserId, AuthTokens Tokens, BootstrapResponse Bootstrap);

public sealed record AuthError(
    string Code,
    string Message,
    IReadOnlyDictionary<string, string[]>? FieldErrors = null
);
