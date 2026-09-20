using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.BillingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

namespace ResumeEnhancer.AuthModule.SL.Services;

internal sealed class RegistrationService(
    IAuthRepository repository,
    IProfilingRegistrationService profiling,
    IBillingRegistrationService billing,
    IPasswordHasher hasher,
    ITokenService tokens,
    IRegistrationThrottle throttle,
    IEntitlementResolver entitlements,
    TimeProvider timeProvider,
    IAuthAuditRecorder auditRecorder,
    IAuthChallengeFactory challengeFactory,
    IAuthChallengeDeliveryProtector deliveryProtector
) : IRegistrationService
{
    private static readonly HashSet<string> Sources =
    [
        "homepage",
        "pricing",
        "template-selection",
        "premium-lock",
    ];

    public async Task<RegisterResponse> RegisterAsync(RegisterCommand command, CancellationToken ct)
    {
        var request = command.Request;
        var email = NormalizeEmail(request.Email);
        var source = request.Source.Trim().ToLowerInvariant();
        var idempotencyKey = request.IdempotencyKey?.Trim();
        var requestHash = CreateRequestHash(request, email, source);
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            var prior = await repository.FindRegistrationIdempotencyAsync(
                email,
                idempotencyKey,
                ct
            );
            if (prior is not null)
            {
                if (!string.Equals(prior.RequestHash, requestHash, StringComparison.Ordinal))
                    throw new AuthException(
                        "AUTH_IDEMPOTENCY_CONFLICT",
                        "The idempotency key was used with different data.",
                        409
                    );
                return JsonSerializer.Deserialize<RegisterResponse>(prior.ResponseJson)
                    ?? throw new AuthException(
                        "AUTH_IDEMPOTENCY_UNAVAILABLE",
                        "The registration result is unavailable.",
                        503
                    );
            }
        }
        if (!await throttle.IsAllowedAsync(email, command.IpAddress, ct))
        {
            await auditRecorder.RecordAsync("registration_throttled", null, command.IpAddress, cancellationToken: ct);
            throw new AuthException("AUTH_RATE_LIMITED", "Too many registration attempts.", 429);
        }
        if (!request.TermsConsent || !request.PrivacyConsent)
        {
            await auditRecorder.RecordAsync("registration_validation_failed", null, command.IpAddress, cancellationToken: ct);
            throw new AuthException(
                "AUTH_REQUIRED_CONSENT",
                "Terms and privacy consent are required.",
                422
            );
        }
        if (!Sources.Contains(source))
        {
            await auditRecorder.RecordAsync("registration_validation_failed", null, command.IpAddress, cancellationToken: ct);
            throw new AuthException("AUTH_INVALID_SOURCE", "Unsupported source.", 422);
        }
        var existing = await repository.FindIdentityAsync(email, ct);
        if (existing is not null)
        {
            await auditRecorder.RecordAsync("registration_conflict", null, command.IpAddress, cancellationToken: ct);
            throw new AuthException("AUTH_EMAIL_IN_USE", "The email is already registered.", 409);
        }
        var user = await profiling.CreateRegistrationUserAsync(
            new ProfileRegistrationInput(request.FirstName.Trim(), request.LastName.Trim(), email),
            ct
        );
        var billingBaseline =
            await billing.AddStarterRegistrationBillingAsync(user.UserId, ct)
            ?? throw new AuthException(
                "AUTH_CONFIGURATION_UNAVAILABLE",
                "Registration setup is unavailable.",
                503
            );
        await profiling.AddRegistrationBaselineAsync(
            new RegistrationBaselineInput(
                user.UserId,
                billingBaseline.BillingSubscriptionId,
                billingBaseline.AccessProfileId,
                billingBaseline.PlanCode
            ),
            ct
        );
        var refresh = tokens.CreateRefreshToken();
        var sessionCreatedAtUtc = timeProvider.GetUtcNow().UtcDateTime;
        var session = new RefreshSession
        {
            UserId = user.UserId,
            TokenHash = tokens.HashRefreshToken(refresh),
            FamilyId = Guid.NewGuid(),
            ExpiresAtUtc = sessionCreatedAtUtc.AddDays(30),
            CreatedAtUtc = sessionCreatedAtUtc,
            IpAddress = command.IpAddress,
            UserAgent = command.UserAgent,
        };
        var identity = new AuthenticationIdentity
        {
            UserId = user.UserId,
            NormalizedEmail = email,
            PasswordHash = hasher.Hash(request.Password),
        };
        var now = sessionCreatedAtUtc;
        var (verificationChallenge, rawVerificationChallenge) = await challengeFactory.CreateAsync(
            identity, "email-verification", now, now.AddHours(24), command.IpAddress, command.UserAgent, ct);
        identity.Challenges.Add(verificationChallenge);
        var verificationEnvelope = new AuthChallengeDeliveryEnvelope(
            1,
            "email-verification",
            user.UserId,
            email,
            deliveryProtector.Protect(rawVerificationChallenge, "email-verification", user.UserId, email));
        var audits = Array.Empty<AuthAuditEvent>();
        var consents = new[]
        {
            new ConsentRecord
            {
                UserId = user.UserId,
                Type = "terms",
                VersionId = "terms-v1",
                Accepted = true,
                AcceptedAtUtc = now,
                IpAddress = command.IpAddress,
            },
            new ConsentRecord
            {
                UserId = user.UserId,
                Type = "privacy",
                VersionId = "privacy-v1",
                Accepted = true,
                AcceptedAtUtc = now,
                IpAddress = command.IpAddress,
            },
            new ConsentRecord
            {
                UserId = user.UserId,
                Type = "marketing",
                VersionId = "marketing-v1",
                Accepted = request.MarketingConsent,
                AcceptedAtUtc = now,
                IpAddress = command.IpAddress,
            },
        };
        var outbox = new[]
        {
            new AuthOutboxMessage
            {
                Type = "verification-email",
                PayloadJson = JsonSerializer.Serialize(
                    new AuthSideEffectPayload(user.UserId, email, verificationEnvelope)),
                AvailableAtUtc = now,
            },
            new AuthOutboxMessage
            {
                Type = "welcome-email",
                PayloadJson = JsonSerializer.Serialize(new AuthSideEffectPayload(user.UserId, email)),
                AvailableAtUtc = now,
            },
        };
        try
        {
            await repository.AddAsync(identity, session, consents, audits, outbox, ct);
        }
        catch (AuthPersistenceConflictException)
        {
            await auditRecorder.RecordAsync("registration_conflict", null, command.IpAddress, cancellationToken: ct);
            throw new AuthException("AUTH_EMAIL_IN_USE", "The email is already registered.", 409);
        }
        await auditRecorder.RecordAsync("account_created", user.UserId, command.IpAddress,
            JsonSerializer.Serialize(new { source }), ct);
        await auditRecorder.RecordAsync("session_created", user.UserId, command.IpAddress, cancellationToken: ct);
        var (AccessToken, ExpiresAtUtc) = await tokens.CreateAccessTokenAsync(user.UserId, session.SessionKey, ct);
        var bootstrap = CalculateBootstrap(
            source,
            request.SelectedTemplateId,
            await entitlements.ResolveAsync(user.UserId, ct)
        );
        var response = new RegisterResponse(
            user.UserId,
            new AuthTokens(AccessToken, refresh, ExpiresAtUtc, session.ExpiresAtUtc),
            bootstrap
        );
        if (!string.IsNullOrWhiteSpace(idempotencyKey))
        {
            try
            {
                await repository.AddRegistrationIdempotencyAsync(
                    new AuthRegistrationIdempotency
                    {
                        NormalizedEmail = email,
                        IdempotencyKey = idempotencyKey,
                        RequestHash = requestHash,
                        ResponseJson = JsonSerializer.Serialize(response),
                    },
                    ct
                );
                await repository.SaveAsync(ct);
            }
            catch (AuthPersistenceConflictException)
            {
                var prior = await repository.FindRegistrationIdempotencyAsync(
                    email,
                    idempotencyKey,
                    ct
                );
                if (
                    prior is null
                    || !string.Equals(prior.RequestHash, requestHash, StringComparison.Ordinal)
                )
                    throw new AuthException(
                        "AUTH_IDEMPOTENCY_CONFLICT",
                        "The idempotency key was used with different data.",
                        409
                    );
                return JsonSerializer.Deserialize<RegisterResponse>(prior.ResponseJson)
                    ?? throw new AuthException(
                        "AUTH_IDEMPOTENCY_UNAVAILABLE",
                        "The registration result is unavailable.",
                        503
                    );
            }
        }
        return response;
    }

    public async Task<AuthTokens> RefreshAsync(RefreshCommand command, CancellationToken ct)
    {
        var current = await repository.FindSessionAsync(
            tokens.HashRefreshToken(command.Request.RefreshToken),
            ct
        );
        var now = timeProvider.GetUtcNow().UtcDateTime;
        if (current is null || current.ExpiresAtUtc <= now)
            throw new AuthException(
                "AUTH_REFRESH_INVALID",
                "The refresh session is invalid or expired.",
                401
            );
        if (current.RevokedAtUtc is not null || current.RotatedAtUtc is not null)
        {
            await repository.RevokeFamilyAsync(current.FamilyId, now, ct);
            await repository.SaveAsync(ct);
            throw new AuthException(
                "AUTH_REFRESH_REPLAYED",
                "The refresh session was already used.",
                401
            );
        }
        var refresh = tokens.CreateRefreshToken();
        var sessionCreatedAtUtc = current.CreatedAtUtc ?? now;
        var replacement = new RefreshSession
        {
            UserId = current.UserId,
            FamilyId = current.FamilyId,
            TokenHash = tokens.HashRefreshToken(refresh),
            ExpiresAtUtc = now.AddDays(30),
            CreatedAtUtc = sessionCreatedAtUtc,
            IpAddress = current.IpAddress,
            UserAgent = current.UserAgent,
        };
        RefreshRotationResult rotation;
        try
        {
            rotation = await repository.TryRotateSessionIfCurrentAsync(
                current.Id,
                current.UserId,
                current.SessionKey,
                now,
                replacement,
                ct);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            throw new AuthException(
                "AUTH_CONFIGURATION_UNAVAILABLE",
                "Authentication state is unavailable.",
                503);
        }
        if (rotation == RefreshRotationResult.CurrentStateInvalid)
            throw new AuthException("AUTH_REFRESH_INVALID", "The refresh session is invalid or expired.", 401);
        if (rotation != RefreshRotationResult.Rotated)
        {
            await repository.RevokeFamilyAsync(current.FamilyId, now, ct);
            await repository.SaveAsync(ct);
            throw new AuthException(
                "AUTH_REFRESH_REPLAYED",
                "The refresh session was already used.",
                401
            );
        }
        // The repository operation evaluates current Auth/Profiling state in the
        // same serializable boundary as conditional rotation. A second read here
        // would reintroduce the race after rotation.
        if (rotation == RefreshRotationResult.CurrentStateInvalid)
            throw new AuthException("AUTH_REFRESH_INVALID", "The refresh session is invalid or expired.", 401);
        var (AccessToken, ExpiresAtUtc) = await tokens.CreateAccessTokenAsync(
            current.UserId,
            replacement.SessionKey,
            ct
        );
        return new AuthTokens(AccessToken, refresh, ExpiresAtUtc, replacement.ExpiresAtUtc);
    }

    public async Task<bool> LogoutAsync(LogoutCommand command, CancellationToken ct)
    {
        var session = await repository.FindSessionAsync(
            tokens.HashRefreshToken(command.RefreshToken),
            ct
        );
        if (session is null)
            return true;
        await repository.RevokeFamilyAsync(session.FamilyId, timeProvider.GetUtcNow().UtcDateTime, ct);
        await repository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> ResendVerificationAsync(
        ResendVerificationCommand command,
        CancellationToken ct
    )
    {
        var email = NormalizeEmail(command.Request.Email);
        var identity = await repository.FindIdentityAsync(email, ct);
        if (identity is null || identity.EmailVerified)
            return true;
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var (verificationChallenge, rawVerificationChallenge) = await challengeFactory.CreateAsync(
            identity, "email-verification", now, now.AddHours(24), null, null, ct);
        await repository.AddChallengeAsync(verificationChallenge, ct);
        var verificationEnvelope = new AuthChallengeDeliveryEnvelope(
            1,
            "email-verification",
            identity.UserId,
            email,
            deliveryProtector.Protect(rawVerificationChallenge, "email-verification", identity.UserId, email));
        await repository.AddOutboxAsync(
            new AuthOutboxMessage
            {
                Type = "verification-email",
                PayloadJson = JsonSerializer.Serialize(
                    new AuthSideEffectPayload(identity.UserId, email, verificationEnvelope)),
                AvailableAtUtc = now,
            },
            ct
        );
        await repository.SaveAsync(ct);
        return true;
    }

    public BootstrapResponse CalculateBootstrap(string source, string? selectedTemplateId)
    {
        return CalculateBootstrap(source, selectedTemplateId, null);
    }

    public static BootstrapResponse CalculateBootstrap(
        string source,
        string? selectedTemplateId,
        IReadOnlySet<string>? effectiveEntitlements
    )
    {
        var normalizedSource = source.Trim().ToLowerInvariant();
        var normalized = Sources.Contains(normalizedSource) ? normalizedSource : "homepage";
        var route =
            normalized == "template-selection" && !string.IsNullOrWhiteSpace(selectedTemplateId)
                ? $"/app/templates?selected={Uri.EscapeDataString(selectedTemplateId)}"
            : normalized == "pricing" ? "/app/billing"
            : "/app/templates";
        var capabilities = effectiveEntitlements ?? new HashSet<string>(StringComparer.Ordinal);
        var featureEntitlements = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase)
        {
            ["resumeCreate"] = capabilities.Contains("ResumeCreate"),
            ["premiumTemplates"] = capabilities.Contains("PremiumTemplates"),
            ["pdfExport"] = capabilities.Contains("PdfExport"),
            ["templateView"] = capabilities.Contains("TemplateView"),
            ["resumePublish"] = capabilities.Contains("ResumePublish"),
        };
        return new BootstrapResponse(
            "active",
            "pending",
            "FREE",
            route,
            normalized,
            selectedTemplateId,
            "continue",
            featureEntitlements,
            ["AUTH_VERIFICATION_PENDING"]
        );
    }

    internal static string NormalizeEmail(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static string CreateRequestHash(RegisterRequest request, string email, string source)
    {
        var canonical = string.Join(
            "|",
            email,
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Password,
            request.TermsConsent,
            request.PrivacyConsent,
            request.MarketingConsent,
            source,
            request.SelectedTemplateId?.Trim() ?? string.Empty
        );
        return Convert
            .ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)))
            .ToLowerInvariant();
    }

}

public interface IRegistrationService
{
    public Task<RegisterResponse> RegisterAsync(
        RegisterCommand command,
        CancellationToken cancellationToken = default
    );
    public Task<AuthTokens> RefreshAsync(
        RefreshCommand command,
        CancellationToken cancellationToken = default
    );
    public Task<bool> LogoutAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default
    );
    public Task<bool> ResendVerificationAsync(
        ResendVerificationCommand command,
        CancellationToken cancellationToken = default
    );
    public BootstrapResponse CalculateBootstrap(string source, string? selectedTemplateId);
}

public sealed class AuthException(string code, string message, int statusCode) : Exception(message)
{
    public string Code { get; } = code;
    public int StatusCode { get; } = statusCode;
}
