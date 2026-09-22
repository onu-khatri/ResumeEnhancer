using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.Web.Outbox;
using ResumeEnhancer.BillingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.SL.Integrations;

using System.Security.Claims;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthRegistrationServiceTests
{
    private static readonly DateTimeOffset TestNow = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static DateTime TestNowUtc => TestNow.UtcDateTime;

    [Theory]
    [InlineData(" pricing ", "/app/billing")]
    [InlineData("template-selection", "/app/templates?selected=starter%20one")]
    [InlineData("unsupported", "/app/templates")]
    public void Bootstrap_uses_allowlisted_route_and_safe_fallback(
        string source,
        string expectedRoute
    )
    {
        var service = CreateService();
        var result = service.CalculateBootstrap(
            source,
            source == "template-selection" ? "starter one" : null
        );
        Assert.Equal(expectedRoute, result.WorkspaceRoute);
        Assert.Equal(
            "v1",
            ResumeEnhancer.AuthModule.AM.Responses.BootstrapResponse.ContractVersion
        );
    }

    [Fact]
    public async Task Register_normalizes_email_and_persists_two_side_effects()
    {
        var repository = new FakeAuthRepository();
        var service = CreateService(repository);
        var result = await service.RegisterAsync(
            new RegisterCommand(
                new RegisterRequest
                {
                    FirstName = " Ada ",
                    LastName = " Lovelace ",
                    Email = " ADA@Example.COM ",
                    Password = "Password!1234",
                    TermsConsent = true,
                    PrivacyConsent = true,
                    Source = "homepage",
                },
                "127.0.0.1",
                "test-agent"
            ),
            CancellationToken.None
        );

        Assert.Equal("ada@example.com", repository.Identity!.NormalizedEmail);
        Assert.Equal(2, repository.Outbox.Count);
        var verificationPayload = repository.Outbox.Single(x => x.Type == "verification-email").PayloadJson;
        Assert.Contains("\"Challenge\"", verificationPayload);
        Assert.DoesNotContain("raw-challenge", verificationPayload);
        using (var payload = JsonDocument.Parse(verificationPayload))
        {
            var envelope = payload.RootElement.GetProperty("Challenge");
            Assert.Equal(1, envelope.GetProperty("Version").GetInt32());
            Assert.Equal("email-verification", envelope.GetProperty("Purpose").GetString());
            Assert.Equal(repository.Identity.UserId, envelope.GetProperty("UserId").GetInt32());
            Assert.Equal("ada@example.com", envelope.GetProperty("Email").GetString());
            Assert.NotEqual("raw-challenge", envelope.GetProperty("ProtectedValue").GetString());
        }
        Assert.Equal(["terms", "privacy", "marketing"], repository.Consents.Select(x => x.Type));
        Assert.Equal([true, true, false], repository.Consents.Select(x => x.Accepted));
        Assert.Equal("AUTH_VERIFICATION_PENDING", result.Bootstrap.StateCodes.Single());
        Assert.NotNull(repository.RegisteredSession?.CreatedAtUtc);
        Assert.InRange(
            repository.RegisteredSession!.CreatedAtUtc!.Value,
            TestNowUtc.AddMinutes(-1),
            TestNowUtc.AddMinutes(1));
    }

    [Fact]
    public async Task Register_replays_the_persisted_result_for_the_same_idempotency_key()
    {
        var repository = new FakeAuthRepository();
        var service = CreateService(repository);
        var command = new RegisterCommand(
            new RegisterRequest
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Email = "ada@example.com",
                Password = "Password!1234",
                TermsConsent = true,
                PrivacyConsent = true,
                Source = "homepage",
                IdempotencyKey = "registration-1",
            },
            "127.0.0.1",
            "test-agent"
        );

        var first = await service.RegisterAsync(command, CancellationToken.None);
        var replay = await service.RegisterAsync(command, CancellationToken.None);

        Assert.Equal(first.UserId, replay.UserId);
        Assert.Equal(first.Tokens, replay.Tokens);
        Assert.Equal(first.Bootstrap.SessionState, replay.Bootstrap.SessionState);
        Assert.Equal(first.Bootstrap.WorkspaceRoute, replay.Bootstrap.WorkspaceRoute);
        Assert.Equal(
            first.Bootstrap.FeatureEntitlements.OrderBy(x => x.Key),
            replay.Bootstrap.FeatureEntitlements.OrderBy(x => x.Key)
        );
        Assert.Equal(first.Bootstrap.StateCodes, replay.Bootstrap.StateCodes);
        Assert.Equal(2, repository.Outbox.Count);
    }

    [Fact]
    public async Task Register_rejects_reuse_of_an_idempotency_key_with_different_data()
    {
        var repository = new FakeAuthRepository();
        var service = CreateService(repository);
        var first = new RegisterRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            Password = "Password!1234",
            TermsConsent = true,
            PrivacyConsent = true,
            Source = "homepage",
            IdempotencyKey = "registration-1",
        };
        await service.RegisterAsync(new RegisterCommand(first, null, null), CancellationToken.None);
        first.LastName = "Byron";

        var exception = await Assert.ThrowsAsync<AuthException>(() =>
            service.RegisterAsync(new RegisterCommand(first, null, null), CancellationToken.None)
        );

        Assert.Equal("AUTH_IDEMPOTENCY_CONFLICT", exception.Code);
        Assert.Equal(409, exception.StatusCode);
    }

    [Fact]
    public async Task Outbox_failure_is_retryable_and_audited_without_exception_text()
    {
        var repository = new FakeAuthRepository
        {
            Due =
            [
                new AuthOutboxMessage
                {
                    Id = 11,
                    Type = "welcome-email",
                    PayloadJson = JsonSerializer.Serialize(
                        new { UserId = 42, Email = "ada@example.com" }
                    ),
                },
            ],
        };
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthAuditRecorder>(_ => new FakeAuditRecorder(repository))
            .AddScoped<IAuthSideEffectHandler, FailingHandler>()
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(
            services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AuthOutboxDispatcher>.Instance,
            new FakeTimeProvider(TestNow)
        );

        Assert.Equal(0, await dispatcher.ProcessOnceAsync());
        Assert.Equal(1, repository.FailedAttempts);
        Assert.Equal("side_effect_delivery_failed", repository.FailedError);
        Assert.NotNull(repository.FailedAvailableAtUtc);
        Assert.Equal("side_effect_failed", Assert.Single(repository.Audits).EventType);
        Assert.Contains("outbox:11", Assert.Single(repository.Audits).MetadataJson);
        Assert.DoesNotContain("provider secret", repository.FailedError);
    }

    [Fact]
    public async Task Outbox_failure_at_retry_ceiling_becomes_degraded_and_stops_retrying()
    {
        var repository = new FakeAuthRepository
        {
            Due =
            [
                new AuthOutboxMessage
                {
                    Id = 12,
                    Attempts = 4,
                    Type = "welcome-email",
                    PayloadJson = JsonSerializer.Serialize(
                        new { UserId = 42, Email = "ada@example.com" }
                    ),
                },
            ],
        };
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthAuditRecorder>(_ => new FakeAuditRecorder(repository))
            .AddScoped<IAuthSideEffectHandler, FailingHandler>()
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(
            services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AuthOutboxDispatcher>.Instance,
            new FakeTimeProvider(TestNow)
        );

        Assert.Equal(0, await dispatcher.ProcessOnceAsync());
        Assert.Equal(5, repository.FailedAttempts);
        Assert.Equal("side_effect_delivery_exhausted", repository.FailedError);
        Assert.Equal(DateTime.MaxValue, repository.FailedAvailableAtUtc);
        Assert.Contains("retryable", Assert.Single(repository.Audits).MetadataJson);
        Assert.Contains("false", Assert.Single(repository.Audits).MetadataJson);
        Assert.Contains("outbox:12", Assert.Single(repository.Audits).MetadataJson);
    }

    [Fact]
    public async Task Audit_recorder_failure_does_not_change_outbox_retry_state()
    {
        var repository = new FakeAuthRepository
        {
            Due =
            [
                new AuthOutboxMessage
                {
                    Id = 15,
                    Type = "welcome-email",
                    PayloadJson = JsonSerializer.Serialize(new { UserId = 42, Email = "ada@example.com" }),
                },
            ],
        };
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthAuditRecorder, FailingAuditRecorder>()
            .AddScoped<IAuthSideEffectHandler, FailingHandler>()
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(
            services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AuthOutboxDispatcher>.Instance,
            new FakeTimeProvider(TestNow));

        var processed = await dispatcher.ProcessOnceAsync();

        Assert.Equal(0, processed);
        Assert.Equal(1, repository.FailedAttempts);
        Assert.Equal("side_effect_delivery_failed", repository.FailedError);
        Assert.Equal(TestNow.UtcDateTime.AddMinutes(2), repository.FailedAvailableAtUtc);
        Assert.Empty(repository.Audits);
    }

    [Fact]
    public async Task Outbox_dispatcher_processes_valid_payloads()
    {
        var repository = new FakeAuthRepository
        {
            Due =
            [
                new AuthOutboxMessage
                {
                    Id = 13,
                    Type = "welcome-email",
                    PayloadJson = JsonSerializer.Serialize(
                        new { UserId = 42, Email = "ada@example.com" }
                    ),
                },
            ],
        };
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthAuditRecorder>(_ => new FakeAuditRecorder(repository))
            .AddScoped<IAuthSideEffectHandler, SuccessfulHandler>()
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(
            services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AuthOutboxDispatcher>.Instance,
            new FakeTimeProvider(TestNow)
        );

        Assert.Equal(1, await dispatcher.ProcessOnceAsync());
        Assert.Empty(repository.Audits);
    }

    [Theory]
    [InlineData("verification-email", "email-verification")]
    [InlineData("password-reset-email", "password-reset")]
    public async Task Outbox_dispatcher_accepts_the_typed_registration_and_reset_payload_shape(
        string messageType,
        string purpose)
    {
        var repository = new FakeAuthRepository
        {
            Due =
            [
                new AuthOutboxMessage
                {
                    Id = 16,
                    Type = messageType,
                    PayloadJson = JsonSerializer.Serialize(
                        new AuthSideEffectPayload(
                            42,
                            "ada@example.com",
                            new AuthChallengeDeliveryEnvelope(
                                1,
                                purpose,
                                42,
                                "ada@example.com",
                                "protected-value"))),
                },
            ],
        };
        var handler = new CapturingHandler();
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthAuditRecorder>(_ => new FakeAuditRecorder(repository))
            .AddScoped<IAuthSideEffectHandler>(_ => handler)
            .AddScoped<IAuthChallengeDeliveryProtector, FakeChallengeProtector>()
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(
            services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AuthOutboxDispatcher>.Instance,
            new FakeTimeProvider(TestNow));

        Assert.Equal(1, await dispatcher.ProcessOnceAsync());
        Assert.Equal(messageType, handler.Type);
        Assert.Equal("raw-challenge", handler.Challenge);
        Assert.Empty(repository.Audits);
    }

    [Fact]
    public async Task Outbox_dispatcher_records_invalid_payload_without_exposing_details()
    {
        var repository = new FakeAuthRepository
        {
            Due =
            [
                new AuthOutboxMessage
                {
                    Id = 14,
                    Type = "welcome-email",
                    PayloadJson = "not-json",
                },
            ],
        };
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthAuditRecorder>(_ => new FakeAuditRecorder(repository))
            .AddScoped<IAuthSideEffectHandler, SuccessfulHandler>()
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(
            services.GetRequiredService<IServiceScopeFactory>(),
            NullLogger<AuthOutboxDispatcher>.Instance,
            new FakeTimeProvider(TestNow)
        );

        Assert.Equal(0, await dispatcher.ProcessOnceAsync());
        Assert.Equal("side_effect_payload_invalid", repository.FailedError);
        Assert.Null(Assert.Single(repository.Audits).UserId);
    }

    [Fact]
    public async Task Register_duplicate_normalized_email_returns_stable_conflict()
    {
        var repository = new FakeAuthRepository
        {
            Identity = new AuthenticationIdentity { NormalizedEmail = "ada@example.com" },
        };
        var service = CreateService(repository);

        var exception = await Assert.ThrowsAsync<AuthException>(() =>
            service.RegisterAsync(
                new RegisterCommand(
                    new RegisterRequest
                    {
                        FirstName = "Ada",
                        LastName = "Lovelace",
                        Email = " ADA@EXAMPLE.COM ",
                        Password = "Password!1234",
                        TermsConsent = true,
                        PrivacyConsent = true,
                        Source = "homepage",
                    },
                    "127.0.0.1",
                    "test-agent"
                ),
                CancellationToken.None
            )
        );

        Assert.Equal("AUTH_EMAIL_IN_USE", exception.Code);
        Assert.Equal(409, exception.StatusCode);
    }

    [Fact]
    public async Task Refresh_rotates_session_and_replay_revokes_family()
    {
        var repository = new FakeAuthRepository
        {
            Session = new RefreshSession
            {
                UserId = 42,
                FamilyId = Guid.NewGuid(),
                TokenHash = "refresh",
                CreatedAtUtc = new DateTime(2026, 9, 1, 7, 0, 0, DateTimeKind.Utc),
                ExpiresAtUtc = TestNowUtc.AddDays(1),
            },
        };
        var service = CreateService(repository);

        var result = await service.RefreshAsync(
            new RefreshCommand(new RefreshRequest { RefreshToken = "refresh" }),
            CancellationToken.None
        );

        Assert.Equal("access", result.AccessToken);
        Assert.NotNull(repository.Session!.RotatedAtUtc);
        Assert.NotNull(repository.Replacement);
        Assert.Equal(repository.Session.CreatedAtUtc, repository.Replacement!.CreatedAtUtc);

        var replay = await Assert.ThrowsAsync<AuthException>(() =>
            service.RefreshAsync(
                new RefreshCommand(new RefreshRequest { RefreshToken = "refresh" }),
                CancellationToken.None
            )
        );
        Assert.Equal("AUTH_REFRESH_REPLAYED", replay.Code);
        Assert.True(repository.FamilyRevoked);
    }

    [Fact]
    public async Task Refresh_maps_state_dependency_failure_safely_without_issuing_credentials()
    {
        var repository = new FakeAuthRepository
        {
            Session = new RefreshSession
            {
                UserId = 42,
                FamilyId = Guid.NewGuid(),
                TokenHash = "refresh",
                CreatedAtUtc = TestNowUtc,
                ExpiresAtUtc = TestNowUtc.AddDays(1),
            },
        };
        var tokens = new FakeTokens();
        repository.CurrentStateException = new InvalidOperationException("state dependency unavailable");
        var service = CreateService(repository, tokens: tokens);

        var exception = await Assert.ThrowsAsync<AuthException>(() => service.RefreshAsync(
            new RefreshCommand(new RefreshRequest { RefreshToken = "refresh" }),
            CancellationToken.None));

        Assert.Equal("AUTH_CONFIGURATION_UNAVAILABLE", exception.Code);
        Assert.Equal(503, exception.StatusCode);
        Assert.Null(repository.Replacement);
        Assert.Null(repository.Session!.RotatedAtUtc);
        Assert.Equal(0, tokens.AccessTokenIssuances);
    }

    [Fact]
    public async Task Refresh_legacy_null_creation_timestamp_uses_current_time_fallback()
    {
        var now = new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var timeProvider = new FakeTimeProvider(now);
        var repository = new FakeAuthRepository
        {
            Session = new RefreshSession
            {
                UserId = 42,
                FamilyId = Guid.NewGuid(),
                TokenHash = "legacy-refresh",
                CreatedAtUtc = null,
                ExpiresAtUtc = now.UtcDateTime.AddDays(1),
            },
        };
        var service = CreateService(repository, timeProvider);

        await service.RefreshAsync(
            new RefreshCommand(new RefreshRequest { RefreshToken = "legacy-refresh" }),
            CancellationToken.None);

        Assert.NotNull(repository.Replacement?.CreatedAtUtc);
        Assert.Equal(now.UtcDateTime, repository.Replacement!.CreatedAtUtc);
    }

    [Fact]
    public async Task Refresh_conditional_claim_failure_revokes_family_as_replay()
    {
        var repository = new FakeAuthRepository
        {
            RotateSucceeds = false,
            Session = new RefreshSession
            {
                UserId = 42,
                FamilyId = Guid.NewGuid(),
                TokenHash = "refresh",
                ExpiresAtUtc = TestNowUtc.AddDays(1),
            },
        };
        var service = CreateService(repository);

        var replay = await Assert.ThrowsAsync<AuthException>(() =>
            service.RefreshAsync(
                new RefreshCommand(new RefreshRequest { RefreshToken = "refresh" }),
                CancellationToken.None
            )
        );

        Assert.Equal("AUTH_REFRESH_REPLAYED", replay.Code);
        Assert.True(repository.FamilyRevoked);
        Assert.Null(repository.Replacement);
    }

    [Fact]
    public async Task Refresh_rejects_missing_and_expired_sessions()
    {
        var repository = new FakeAuthRepository
        {
            Session = new RefreshSession
            {
                UserId = 42,
                TokenHash = "expired",
                ExpiresAtUtc = TestNowUtc.AddMinutes(-1),
            },
        };
        var service = CreateService(repository);

        var missing = await Assert.ThrowsAsync<AuthException>(() =>
            service.RefreshAsync(
                new RefreshCommand(new RefreshRequest { RefreshToken = "missing" }),
                CancellationToken.None
            )
        );
        var expired = await Assert.ThrowsAsync<AuthException>(() =>
            service.RefreshAsync(
                new RefreshCommand(new RefreshRequest { RefreshToken = "expired" }),
                CancellationToken.None
            )
        );

        Assert.Equal("AUTH_REFRESH_INVALID", missing.Code);
        Assert.Equal("AUTH_REFRESH_INVALID", expired.Code);
    }

    [Fact]
    public async Task Logout_revokes_known_family_and_is_idempotent_for_unknown_token()
    {
        var repository = new FakeAuthRepository
        {
            Session = new RefreshSession
            {
                UserId = 42,
                FamilyId = Guid.NewGuid(),
                TokenHash = "refresh",
                ExpiresAtUtc = TestNowUtc.AddDays(1),
            },
        };
        var service = CreateService(repository);

        Assert.True(
            await service.LogoutAsync(new LogoutCommand("missing"), CancellationToken.None)
        );
        Assert.False(repository.FamilyRevoked);
        Assert.True(
            await service.LogoutAsync(new LogoutCommand("refresh"), CancellationToken.None)
        );
        Assert.True(repository.FamilyRevoked);
    }

    [Fact]
    public async Task Resend_verification_is_safe_for_unknown_and_verified_identities()
    {
        var repository = new FakeAuthRepository
        {
            Identity = new AuthenticationIdentity
            {
                UserId = 42,
                NormalizedEmail = "verified@example.com",
                EmailVerified = true,
            },
        };
        var service = CreateService(repository);

        Assert.True(
            await service.ResendVerificationAsync(
                new ResendVerificationCommand(new() { Email = "unknown@example.com" }),
                CancellationToken.None
            )
        );
        Assert.True(
            await service.ResendVerificationAsync(
                new ResendVerificationCommand(new() { Email = "verified@example.com" }),
                CancellationToken.None
            )
        );
        Assert.Empty(repository.Outbox);
    }

    [Fact]
    public async Task Resend_verification_adds_outbox_message_for_unverified_identity()
    {
        var repository = new FakeAuthRepository
        {
            Identity = new AuthenticationIdentity
            {
                UserId = 42,
                NormalizedEmail = "ada@example.com",
            },
        };
        var service = CreateService(repository);

        Assert.True(
            await service.ResendVerificationAsync(
                new ResendVerificationCommand(new() { Email = " ADA@EXAMPLE.COM " }),
                CancellationToken.None
            )
        );

        var message = Assert.Single(repository.Outbox);
        Assert.Equal("verification-email", message.Type);
        Assert.Contains("ada@example.com", message.PayloadJson);
        Assert.Contains("\"Challenge\"", message.PayloadJson);
        Assert.DoesNotContain("raw-challenge", message.PayloadJson);
    }

    private static RegistrationService CreateService(
        FakeAuthRepository? repository = null,
        TimeProvider? timeProvider = null,
        FakeTokens? tokens = null
        )
    {
        var targetRepository = repository ?? new FakeAuthRepository();
        return new(
            targetRepository,
            new FakeProfilingService(),
            new FakeBillingService(),
            new FakeHasher(),
            tokens ?? new FakeTokens(),
            new AllowThrottle(),
            new FixedEntitlements(),
            timeProvider ?? new FakeTimeProvider(TestNow),
            new FakeAuditRecorder(targetRepository),
            new FakeChallengeFactory(),
            new FakeChallengeProtector());
    }

    private sealed class FakeAuditRecorder(FakeAuthRepository repository) : IAuthAuditRecorder
    {
        public Task RecordAsync(
            string eventType,
            int? userId,
            string? ipAddress,
            string metadataJson = "{}",
            CancellationToken cancellationToken = default,
            string? correlationId = null)
        {
            var redactedMetadata = metadataJson;
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                var metadata = System.Text.Json.Nodes.JsonNode.Parse(metadataJson) as System.Text.Json.Nodes.JsonObject
                    ?? new System.Text.Json.Nodes.JsonObject();
                metadata["correlationId"] = correlationId;
                redactedMetadata = metadata.ToJsonString();
            }

            repository.Audits.Add(new AuthAuditEvent
            {
                EventType = eventType,
                UserId = userId,
                IpAddress = ipAddress,
                MetadataJson = redactedMetadata,
            });
            return Task.CompletedTask;
        }
    }

    private sealed class AllowThrottle : IRegistrationThrottle
    {
        public Task<bool> IsAllowedAsync(string e, string? i, CancellationToken c = default) =>
            Task.FromResult(true);

        public Task<LimiterDecision> TryConsumeAsync(
            LimiterOperation operation,
            string? normalizedEmail,
            string? subject,
            string? ipAddress,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new LimiterDecision(true, 1, 1, DateTime.UnixEpoch));
    }

    private sealed class FakeHasher : IPasswordHasher
    {
        public string Hash(string password) => "hash";

        public bool Verify(string hash, string password) => true;
    }

    private sealed class FakeTokens : ITokenService
    {
        public int AccessTokenIssuances { get; private set; }

        public string CreateRefreshToken() => "refresh";

        public string HashRefreshToken(string token) => token;

        public Task<(string AccessToken, DateTime ExpiresAtUtc)> CreateAccessTokenAsync(
            int userId,
            Guid sessionId,
            CancellationToken c = default
        )
        {
            AccessTokenIssuances++;
            return Task.FromResult<(string AccessToken, DateTime ExpiresAtUtc)>(
                ("access", TestNowUtc.AddMinutes(15)));
        }

        public Task<ClaimsPrincipal?> ValidateAccessTokenAsync(string token, CancellationToken c = default) =>
            Task.FromResult<ClaimsPrincipal?>(null);

        public Task InvalidateKeyAsync(string keyIdentifier, CancellationToken c = default) => Task.CompletedTask;
    }

    private sealed class FixedEntitlements : IEntitlementResolver
    {
        public Task<IReadOnlySet<string>> ResolveAsync(int userId, CancellationToken c = default) =>
            Task.FromResult<IReadOnlySet<string>>(new HashSet<string> { "ResumeCreate" });
    }

    private sealed class FakeProfilingService : IProfilingRegistrationService
    {
        public Task<ProfileRegistrationSnapshot> CreateRegistrationUserAsync(
            ProfileRegistrationInput i,
            CancellationToken c = default
        ) => Task.FromResult(new ProfileRegistrationSnapshot(42));

        public Task AddRegistrationBaselineAsync(
            RegistrationBaselineInput i,
            CancellationToken c = default
        ) => Task.CompletedTask;

        public Task<AccessShapeSnapshot> GetAccessShapeAsync(
            int i,
            CancellationToken c = default
        ) => Task.FromResult(new AccessShapeSnapshot(new HashSet<string>()));

        public Task<AccessShapeSnapshot> GetStarterAccessShapeAsync(
            CancellationToken c = default
        ) => Task.FromResult(new AccessShapeSnapshot(new HashSet<string>()));

        public Task<StarterAccessProfileSnapshot?> GetStarterAccessProfileAsync(
            CancellationToken c = default
        ) => Task.FromResult<StarterAccessProfileSnapshot?>(new(7));

        public Task ReviseAccessProfilesAsync(
            IReadOnlyCollection<AccessProfileRevisionInput> inputs,
            CancellationToken c = default
        ) => Task.CompletedTask;

        public Task ProcessPendingAccessProfileRevisionsAsync(
            CancellationToken c = default
        ) => Task.CompletedTask;
    }

    private sealed class FakeBillingService : IBillingRegistrationService
    {
        public Task<BillingRegistrationSnapshot?> AddStarterRegistrationBillingAsync(
            int u,
            CancellationToken c = default
        ) => Task.FromResult<BillingRegistrationSnapshot?>(new(9, 1, 7, "FREE"));

        public Task<
            IReadOnlyList<BillingSubscriptionSnapshot>
        > ListActiveSubscriptionSnapshotsAsync(int u, CancellationToken c = default) =>
            Task.FromResult<IReadOnlyList<BillingSubscriptionSnapshot>>([]);
    }

    private sealed class FakeAuthRepository : IAuthRepository
    {
        public AuthenticationIdentity? Identity { get; set; }
        public AuthRegistrationIdempotency? Idempotency { get; set; }
        public List<AuthOutboxMessage> Outbox { get; } = [];
        public List<ConsentRecord> Consents { get; } = [];
        public IReadOnlyList<AuthOutboxMessage> Due { get; init; } = [];
        public List<AuthAuditEvent> Audits { get; } = [];
        public RefreshSession? Session { get; set; }
        public RefreshSession? RegisteredSession { get; private set; }
        public RefreshSession? Replacement { get; private set; }
        public bool RotateSucceeds { get; set; } = true;
        public Exception? CurrentStateException { get; set; }
        public bool FamilyRevoked { get; private set; }

        public Task<IReadOnlyList<AuthSigningKeyMetadata>> GetSigningKeyMetadataAsync(CancellationToken c = default) =>
            Task.FromResult<IReadOnlyList<AuthSigningKeyMetadata>>([]);

        public Task InvalidateSigningKeyAsync(string keyIdentifier, DateTime invalidatedAtUtc, CancellationToken c = default) =>
            Task.CompletedTask;

        public Task<AuthenticationIdentity?> FindIdentityByIdAsync(int id, CancellationToken c = default) =>
            Task.FromResult(Identity?.Id == id ? Identity : null);

        public Task<IReadOnlyList<PasswordHistoryEntry>> GetPasswordHistoryAsync(int id, int take = 2, CancellationToken c = default) =>
            Task.FromResult<IReadOnlyList<PasswordHistoryEntry>>([]);

        public Task UpdatePasswordAndRevokeSessionsAsync(int id, int userId, string hash, DateTime changedAtUtc, string reason, CancellationToken c = default) =>
            Task.CompletedTask;

        public Task AddChallengeAsync(AuthChallenge challenge, CancellationToken c = default) => Task.CompletedTask;

        public Task<int?> FindActiveChallengePurposeIdAsync(string purposeCode, CancellationToken c = default) =>
            Task.FromResult<int?>(1);

        public Task<ChallengeConsumeResult> TryConsumeChallengeAsync(int id, string purpose, string hash, DateTime nowUtc, CancellationToken c = default) =>
            Task.FromResult(ChallengeConsumeResult.InvalidOrExpired);

        public Task<bool> ConsumeChallengeAndUpdatePasswordAsync(int id, int userId, string purpose, string tokenHash, string newHash, DateTime changedAtUtc, string reason, CancellationToken c = default) =>
            Task.FromResult(false);

        public Task<FailedLoginResult> RecordFailedLoginAsync(int id, DateTime nowUtc, int threshold, TimeSpan window, CancellationToken c = default) =>
            Task.FromResult(new FailedLoginResult(0, null, null));

        public Task ClearLockoutAsync(int id, CancellationToken c = default) => Task.CompletedTask;

        public Task<AuthRegistrationIdempotency?> FindRegistrationIdempotencyAsync(
            string email,
            string key,
            CancellationToken c = default
        ) =>
            Task.FromResult(
                Idempotency?.NormalizedEmail == email && Idempotency.IdempotencyKey == key
                    ? Idempotency
                    : null
            );

        public Task AddRegistrationIdempotencyAsync(
            AuthRegistrationIdempotency record,
            CancellationToken c = default
        )
        {
            Idempotency = record;
            return Task.CompletedTask;
        }

        public int FailedAttempts { get; private set; }
        public string? FailedError { get; private set; }
        public DateTime? FailedAvailableAtUtc { get; private set; }

        public Task<AuthenticationIdentity?> FindIdentityAsync(
            string e,
            CancellationToken c = default
        ) => Task.FromResult(Identity?.NormalizedEmail == e ? Identity : null);

        public Task AddAsync(
            AuthenticationIdentity i,
            RefreshSession s,
            IReadOnlyCollection<ConsentRecord> c,
            IReadOnlyCollection<AuthAuditEvent> a,
            IReadOnlyCollection<AuthOutboxMessage> o,
            CancellationToken ct = default
        )
        {
            Identity = i;
            RegisteredSession = s;
            Consents.AddRange(c);
            Outbox.AddRange(o);
            return Task.CompletedTask;
        }

        public Task<RefreshSession?> FindSessionAsync(string h, CancellationToken c = default) =>
            Task.FromResult(Session?.TokenHash == h ? Session : null);

        public Task<RefreshSession?> FindSessionBySessionKeyAsync(Guid sessionKey, CancellationToken c = default) =>
            Task.FromResult(Session?.SessionKey == sessionKey ? Session : null);

        public Task AddSessionAsync(RefreshSession s, CancellationToken c = default)
        {
            Replacement = s;
            return Task.CompletedTask;
        }

        public Task<bool> TryRotateSessionAsync(
            int id,
            DateTime rotatedAtUtc,
            RefreshSession replacement,
            CancellationToken c = default
        )
        {
            if (!RotateSucceeds)
                return Task.FromResult(false);

            if (Session is not null)
                Session.RotatedAtUtc = rotatedAtUtc;
            Replacement = replacement;
            return Task.FromResult(true);
        }

        public Task RevokeFamilyAsync(Guid f, DateTime d, CancellationToken c = default)
        {
            FamilyRevoked = true;
            if (Session?.FamilyId == f)
                Session.RevokedAtUtc = d;
            return Task.CompletedTask;
        }

        public Task AddOutboxAsync(AuthOutboxMessage m, CancellationToken c = default)
        {
            Outbox.Add(m);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<AuthOutboxMessage>> ClaimDueOutboxAsync(
            DateTime n,
            DateTime leaseExpiresAtUtc,
            int t,
            CancellationToken c = default
        )
        {
            foreach (var message in Due.Take(t))
            {
                message.LeaseId ??= Guid.NewGuid();
                message.LeaseExpiresAtUtc = leaseExpiresAtUtc;
            }

            return Task.FromResult<IReadOnlyList<AuthOutboxMessage>>(Due.Take(t).ToList());
        }

        public Task<bool> MarkOutboxProcessedAsync(
            int i,
            Guid leaseId,
            DateTime d,
            CancellationToken c = default
        ) => Task.FromResult(true);

        public Task MarkOutboxFailedAsync(
            int i,
            Guid leaseId,
            int a,
            DateTime d,
            string e,
            CancellationToken c = default
        )
        {
            FailedAttempts = a;
            FailedAvailableAtUtc = d;
            FailedError = e;
            return Task.CompletedTask;
        }

        public Task AddAuditAsync(AuthAuditEvent a, CancellationToken c = default)
        {
            Audits.Add(a);
            return Task.CompletedTask;
        }

        public Task<bool> TryRecordAuditRetryAsync(
            AuthAuditRetryPayload payload,
            CancellationToken c = default)
        {
            if (Audits.Any(audit =>
                    audit.EventType == payload.EventType
                    && audit.UserId == payload.UserId
                    && audit.IpAddress == payload.IpAddress
                    && audit.MetadataJson == payload.MetadataJson))
                return Task.FromResult(false);

            Audits.Add(new AuthAuditEvent
            {
                EventType = payload.EventType,
                UserId = payload.UserId,
                IpAddress = payload.IpAddress,
                MetadataJson = payload.MetadataJson,
            });
            return Task.FromResult(true);
        }

        public async Task<RefreshRotationResult> TryRotateSessionIfCurrentAsync(
            int id,
            int userId,
            Guid sessionKey,
            DateTime rotatedAtUtc,
            RefreshSession replacement,
            CancellationToken c = default)
        {
            if (CurrentStateException is not null)
                throw CurrentStateException;

            return await TryRotateSessionAsync(id, rotatedAtUtc, replacement, c)
                ? RefreshRotationResult.Rotated
                : RefreshRotationResult.Rejected;
        }

        public Task SaveAsync(CancellationToken c = default) => Task.CompletedTask;
    }

    private sealed class FailingHandler : IAuthSideEffectHandler
    {
        public Task HandleAsync(
            string type,
            int userId,
            string email,
            string? challenge,
            CancellationToken cancellationToken = default
        ) => throw new InvalidOperationException("provider secret must not be persisted");
    }

    private sealed class SuccessfulHandler : IAuthSideEffectHandler
    {
        public Task HandleAsync(
            string type,
            int userId,
            string email,
            string? challenge,
            CancellationToken cancellationToken = default
        ) => Task.CompletedTask;
    }

    private sealed class CapturingHandler : IAuthSideEffectHandler
    {
        public string? Type { get; private set; }
        public string? Challenge { get; private set; }

        public Task HandleAsync(
            string type,
            int userId,
            string email,
            string? challenge,
            CancellationToken cancellationToken = default)
        {
            Type = type;
            Challenge = challenge;
            return Task.CompletedTask;
        }
    }

    private sealed class FakeChallengeFactory : IAuthChallengeFactory
    {
        public Task<(AuthChallenge Challenge, string RawChallenge)> CreateAsync(AuthenticationIdentity identity, string purposeCode, DateTime issuedAtUtc, DateTime expiresAtUtc, string? ipAddress, string? userAgent, CancellationToken cancellationToken = default) =>
            Task.FromResult<(AuthChallenge, string)>((new AuthChallenge { AuthenticationIdentity = identity, AuthenticationIdentityId = identity.Id, AuthChallengePurposeId = 1, TokenHash = "hash", IssuedAtUtc = issuedAtUtc, ExpiresAtUtc = expiresAtUtc }, "raw-challenge"));
    }

    private sealed class FakeChallengeProtector : IAuthChallengeDeliveryProtector
    {
        public string Protect(string rawChallenge, string purposeCode, int userId, string email) => $"protected:{purposeCode}:{userId}:{email}";
        public string Unprotect(string protectedChallenge, string purposeCode, int userId, string email) => "raw-challenge";
    }

    private sealed class FailingAuditRecorder : IAuthAuditRecorder
    {
        public Task RecordAsync(
            string eventType,
            int? userId,
            string? ipAddress,
            string metadataJson = "{}",
            CancellationToken cancellationToken = default,
            string? correlationId = null) =>
            throw new InvalidOperationException("audit provider secret must not escape");
    }
}
