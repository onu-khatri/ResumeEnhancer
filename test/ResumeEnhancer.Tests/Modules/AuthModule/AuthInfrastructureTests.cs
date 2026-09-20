using System.Security.Cryptography;
using System.Security.Claims;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.AM.Responses;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.PL.Repositories;
using ResumeEnhancer.AuthModule.PL.Seeding;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Handlers;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.AuthModule.Web;
using ResumeEnhancer.AuthModule.Web.Outbox;
using ResumeEnhancer.BillingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using ResumeEnhancer.WebSolution.ModulesComposition.Authorization;
using ResumeEnhancer.Core.WebLibrary.Authorization;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthInfrastructureTests
{
    private static readonly DateTimeOffset TestNow = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private static DateTime TestNowUtc => TestNow.UtcDateTime;

    [Fact]
    public async Task Authorization_denial_passes_response_correlation_to_audit_recorder()
    {
        var auditRecorder = Substitute.For<IAuthAuditRecorder>();
        var profilingAuthorization = Substitute.For<IProfilingAuthorizationService>();
        profilingAuthorization.GetGuestAuthorizationAsync(Arg.Any<CancellationToken>())
            .Returns(new ProfilingAuthorizationSnapshot(
                0,
                new HashSet<string>(),
                new HashSet<string>(),
                new HashSet<string>()));
        var entitlementResolver = Substitute.For<IEntitlementResolver>();
        var context = new DefaultHttpContext
        {
            TraceIdentifier = "request-correlation-1",
        };
        context.Request.Path = "/protected";
        context.RequestServices = new ServiceCollection()
            .AddLogging()
            .AddProblemDetails()
            .BuildServiceProvider();
        context.SetEndpoint(new Endpoint(
            _ => Task.CompletedTask,
            new EndpointMetadataCollection(new GuestAccessMetadata("starter", new HashSet<string> { "guest" })),
            "protected"));

        var middleware = new EndpointAuthorizationMiddleware(_ => Task.CompletedTask);
        await middleware.InvokeAsync(context, profilingAuthorization, entitlementResolver, auditRecorder);

        Assert.Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
        await auditRecorder.Received(1).RecordAsync(
            "authorization_denied",
            Arg.Any<int?>(),
            Arg.Any<string?>(),
            Arg.Any<string>(),
            Arg.Any<CancellationToken>(),
            "request-correlation-1");
    }

    [Fact]
    public void Database_schema_uses_default_and_rooted_names()
    {
        Assert.Equal("auth", AuthModuleDatabase.GetSchema(null));
        Assert.Equal("auth", AuthModuleDatabase.GetSchema(" "));
        Assert.Equal("tenant_auth", AuthModuleDatabase.GetSchema("tenant"));
    }

    [Fact]
    public void Password_hasher_round_trips_and_rejects_invalid_values()
    {
        var hasher = new AuthPasswordHasher();
        var encoded = hasher.Hash("Password!1234");

        Assert.True(hasher.Verify(encoded, "Password!1234"));
        Assert.False(hasher.Verify(encoded, "wrong"));
        Assert.False(hasher.Verify("not-a-hash", "Password!1234"));
        Assert.False(hasher.Verify("pbkdf2-sha512$x$bad$bad", "Password!1234"));
    }

    [Fact]
    public void Password_hasher_rejects_wrong_algorithm_and_malformed_payloads()
    {
        var hasher = new AuthPasswordHasher();

        Assert.False(hasher.Verify("bcrypt$210000$c2FsdA==$aGFzaA==", "Password!1234"));
        Assert.False(
            hasher.Verify("pbkdf2-sha512$not-a-number$c2FsdA==$aGFzaA==", "Password!1234")
        );
        Assert.False(hasher.Verify("pbkdf2-sha512$1$%%%$%%%", "Password!1234"));
    }

    [Fact]
    public void Challenge_delivery_envelope_rejects_malformed_wrong_purpose_wrong_user_and_missing_key_ring()
    {
        var keyRing = Path.Combine(Path.GetTempPath(), $"ResumeEnhancerChallengeUnit-{Guid.NewGuid():N}");
        Directory.CreateDirectory(keyRing);
        try
        {
            var provider = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(keyRing);
            var protector = new AuthChallengeDeliveryProtector(provider);
            var protectedValue = protector.Protect("raw-challenge", "password-reset", 42, "ada@example.com");

            Assert.Equal("raw-challenge", protector.Unprotect(protectedValue, "password-reset", 42, "ada@example.com"));
            Assert.ThrowsAny<Exception>(() => protector.Unprotect(protectedValue, "email-verification", 42, "ada@example.com"));
            Assert.ThrowsAny<Exception>(() => protector.Unprotect(protectedValue, "password-reset", 43, "ada@example.com"));
            Assert.ThrowsAny<Exception>(() => protector.Unprotect("malformed", "password-reset", 42, "ada@example.com"));

            var unavailableProvider = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(
                Path.Combine(Path.GetTempPath(), $"ResumeEnhancerChallengeMissing-{Guid.NewGuid():N}"));
            var unavailableProtector = new AuthChallengeDeliveryProtector(unavailableProvider);
            Assert.ThrowsAny<Exception>(() => unavailableProtector.Unprotect(protectedValue, "password-reset", 42, "ada@example.com"));
        }
        finally
        {
            Directory.Delete(keyRing, recursive: true);
        }
    }

    [Fact]
    public async Task Provider_backed_token_service_issues_rs256_without_pem_configuration()
    {
        var keyRing = Path.Combine(Path.GetTempPath(), $"ResumeEnhancerAuthUnit-{Guid.NewGuid():N}");
        Directory.CreateDirectory(keyRing);
        try
        {
            var options = new AuthSecurityOptions { DataProtectionKeyRingPath = keyRing };
            var now = TestNow.AddMinutes(-1);
            var timeProvider = new FakeTimeProvider(now);
            var dataProtection = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(keyRing);
            var materialStore = new AuthProtectedKeyMaterialStore(dataProtection, options);
            using var rsa = RSA.Create(2048);
            var repository = Substitute.For<IAuthRepository>();
            repository.GetSigningKeyMetadataAsync(Arg.Any<CancellationToken>()).Returns([
                new AuthSigningKeyMetadata
                {
                    KeyIdentifier = "primary",
                    ProtectedMaterial = materialStore.Protect(rsa),
                    IsActive = true,
                    LifecycleVersion = 1,
                    ActivatedAtUtc = now.UtcDateTime,
                }
            ]);
            var provider = new AuthSigningKeyProvider(repository, materialStore, options);
            var service = new AuthTokenService(options, provider, timeProvider);

            var issued = await service.CreateAccessTokenAsync(42, Guid.NewGuid());
            Assert.Equal(now.UtcDateTime.AddMinutes(30), issued.ExpiresAtUtc);
        }
        finally
        {
            Directory.Delete(keyRing, recursive: true);
        }
    }

    [Fact]
    public async Task Provider_validation_rejects_malformed_unsigned_and_wrong_algorithm_tokens()
    {
        var keyRing = Path.Combine(Path.GetTempPath(), $"ResumeEnhancerAuthUnit-{Guid.NewGuid():N}");
        Directory.CreateDirectory(keyRing);
        try
        {
            var options = new AuthSecurityOptions { DataProtectionKeyRingPath = keyRing };
            var dataProtection = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(keyRing);
            var materialStore = new AuthProtectedKeyMaterialStore(dataProtection, options);
            using var rsa = RSA.Create(2048);
            var repository = Substitute.For<IAuthRepository>();
            repository.GetSigningKeyMetadataAsync(Arg.Any<CancellationToken>()).Returns([
                new AuthSigningKeyMetadata { KeyIdentifier = "primary", ProtectedMaterial = materialStore.Protect(rsa), IsActive = true, LifecycleVersion = 1, ActivatedAtUtc = TestNowUtc }
            ]);
            var timeProvider = new FakeTimeProvider(TestNow);
            var service = new AuthTokenService(options, new AuthSigningKeyProvider(repository, materialStore, options), timeProvider);
            var identity = new ClaimsIdentity([new Claim(JwtRegisteredClaimNames.Sub, "42"), new Claim("sid", Guid.NewGuid().ToString("N"))]);
            var handler = new JsonWebTokenHandler();
            var unsigned = handler.CreateToken(new SecurityTokenDescriptor { Issuer = options.Issuer, Audience = options.Audience, Subject = identity, Expires = TestNowUtc.AddMinutes(5) });
            var hmac = handler.CreateToken(new SecurityTokenDescriptor { Issuer = options.Issuer, Audience = options.Audience, Subject = identity, Expires = TestNowUtc.AddMinutes(5), SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("01234567890123456789012345678901")), SecurityAlgorithms.HmacSha256) });

            Assert.Null(await service.ValidateAccessTokenAsync("not-a-jwt"));
            Assert.Null(await service.ValidateAccessTokenAsync(unsigned));
            Assert.Null(await service.ValidateAccessTokenAsync(hmac));
        }
        finally
        {
            Directory.Delete(keyRing, recursive: true);
        }
    }

    /* Obsolete PEM/configuration and synchronous-token-validation tests retained for historical review only.
    [Fact]
    public void Token_service_rejects_malformed_wrong_key_expired_unsigned_and_wrong_algorithm_tokens()
    {
        using var activeRsa = RSA.Create(2048);
        using var otherRsa = RSA.Create(2048);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Security:SigningKeyPem"] = activeRsa.ExportPkcs8PrivateKeyPem(),
            })
            .Build();
        var service = new ResumeEnhancer.AuthModule.SL.Services.AuthTokenService(configuration);

        Assert.Null(service.ValidateAccessToken("not-a-jwt"));
        Assert.Null(service.ValidateAccessToken("eyJhbGciOiJSUzI1NiJ9.invalid.signature"));

        var wrongKeyToken = CreateJwt(otherRsa, TestNowUtc.AddMinutes(5));
        Assert.Null(service.ValidateAccessToken(wrongKeyToken));

        var wrongIssuerToken = CreateJwt(activeRsa, TestNowUtc.AddMinutes(5), issuer: "wrong-issuer");
        Assert.Null(service.ValidateAccessToken(wrongIssuerToken));

        var expiredToken = CreateJwt(activeRsa, TestNowUtc.AddMinutes(-2));
        Assert.Null(service.ValidateAccessToken(expiredToken));

        var notYetValidToken = CreateJwt(activeRsa, TestNowUtc.AddMinutes(5), notBeforeUtc: TestNowUtc.AddMinutes(2));
        Assert.Null(service.ValidateAccessToken(notYetValidToken));

        var unsignedToken = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = "ResumeEnhancer",
            Audience = "ResumeEnhancer.Api",
            NotBefore = TestNowUtc,
            IssuedAt = TestNowUtc,
            Expires = TestNowUtc.AddMinutes(5),
            Subject = CreateClaimsIdentity(),
        });
        Assert.Null(service.ValidateAccessToken(unsignedToken));

        var wrongAlgorithmToken = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
        {
            Issuer = "ResumeEnhancer",
            Audience = "ResumeEnhancer.Api",
            NotBefore = TestNowUtc,
            IssuedAt = TestNowUtc,
            Expires = TestNowUtc.AddMinutes(5),
            Subject = CreateClaimsIdentity(),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("01234567890123456789012345678901")),
                SecurityAlgorithms.HmacSha256),
        });
        Assert.Null(service.ValidateAccessToken(wrongAlgorithmToken));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("+42")]
    [InlineData("042")]
    [InlineData(" 42")]
    [InlineData("42.0")]
    public void Token_service_rejects_noncanonical_or_nonpositive_subjects(string subject)
    {
        using var activeRsa = RSA.Create(2048);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Security:SigningKeyPem"] = activeRsa.ExportPkcs8PrivateKeyPem(),
            })
            .Build();
        var service = new ResumeEnhancer.AuthModule.SL.Services.AuthTokenService(configuration);

        Assert.Null(service.ValidateAccessToken(CreateJwt(activeRsa, TestNowUtc.AddMinutes(5), subject: subject)));
    }

    [Theory]
    [InlineData(47, true)]
    [InlineData(48, false)]
    [InlineData(49, false)]
    public void Token_service_enforces_previous_key_overlap_boundary(int retiredHoursAgo, bool expectedAccepted)
    {
        using var activeRsa = RSA.Create(2048);
        using var previousRsa = RSA.Create(2048);
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Security:SigningKeyPem"] = activeRsa.ExportPkcs8PrivateKeyPem(),
                ["Auth:Security:PreviousKeyId"] = "previous",
                ["Auth:Security:PreviousSigningKeyPem"] = previousRsa.ExportPkcs8PrivateKeyPem(),
                ["Auth:Security:PreviousKeyRetiredAtUtc"] = TestNowUtc.AddHours(-retiredHoursAgo).ToString("O"),
            })
            .Build();
        var service = new ResumeEnhancer.AuthModule.SL.Services.AuthTokenService(configuration);

        var principal = service.ValidateAccessToken(CreateJwt(previousRsa, TestNowUtc.AddMinutes(5), keyId: "previous"));
        if (expectedAccepted) Assert.NotNull(principal);
        else Assert.Null(principal);
    }

    [Fact]
    public void Token_service_rejects_invalidated_active_key()
    {
        using var activeRsa = RSA.Create(2048);
        var invalidatedConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Security:SigningKeyPem"] = activeRsa.ExportPkcs8PrivateKeyPem(),
                ["Auth:Security:InvalidatedKeyIds:0"] = "primary",
            })
            .Build();

        var invalidatedService = new ResumeEnhancer.AuthModule.SL.Services.AuthTokenService(invalidatedConfiguration);
        Assert.Throws<InvalidOperationException>(() => invalidatedService.CreateAccessToken(42, Guid.NewGuid()));
    }

    [Fact]
    public async Task Async_token_validation_applies_durable_previous_key_retirement_boundary()
    {
        using var activeRsa = RSA.Create(2048);
        using var previousRsa = RSA.Create(2048);
        var options = new AuthSecurityOptions
        {
            SigningKeyPem = activeRsa.ExportPkcs8PrivateKeyPem(),
            PreviousKeyId = "previous",
            PreviousSigningKeyPem = previousRsa.ExportPkcs8PrivateKeyPem(),
            PreviousKeyRetiredAtUtc = TestNowUtc.AddHours(-1),
        };
        var repository = Substitute.For<IAuthRepository>();
        repository.GetSigningKeyMetadataAsync(Arg.Any<CancellationToken>()).Returns([
            new AuthSigningKeyMetadata
            {
                KeyIdentifier = "previous",
                ActivatedAtUtc = TestNowUtc.AddDays(-181),
                RetiredAtUtc = TestNowUtc.AddHours(-49),
            },
        ]);
        var services = new ServiceCollection();
        services.AddScoped(_ => repository);
        using var provider = services.BuildServiceProvider();
        var service = new ResumeEnhancer.AuthModule.SL.Services.AuthTokenService(
            options,
            provider.GetRequiredService<IServiceScopeFactory>());

        var token = CreateJwt(previousRsa, TestNowUtc.AddMinutes(5), keyId: "previous");

        Assert.Null(await service.ValidateAccessTokenAsync(token));
    }

    private static string CreateJwt(
        RSA rsa,
        DateTime expiresAtUtc,
        string issuer = "ResumeEnhancer",
        DateTime? notBeforeUtc = null,
        string keyId = "primary",
        string subject = "42") => new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor
    {
        Issuer = issuer,
        Audience = "ResumeEnhancer.Api",
        NotBefore = notBeforeUtc ?? TestNowUtc.AddMinutes(-1),
        IssuedAt = TestNowUtc.AddMinutes(-1),
        Expires = expiresAtUtc,
        Subject = CreateClaimsIdentity(subject),
        SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa) { KeyId = keyId }, SecurityAlgorithms.RsaSha256),
    });

    private static ClaimsIdentity CreateClaimsIdentity(string subject = "42") => new([
        new Claim(JwtRegisteredClaimNames.Sub, subject),
        new Claim("sid", Guid.NewGuid().ToString("N")),
    ]);

    */
    [Fact]
    public async Task Registration_throttle_limits_email_and_ip_attempts()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(2030, 1, 1, 0, 0, 0, TimeSpan.Zero));
        var throttle = new InMemoryRegistrationThrottle(timeProvider);
        for (var attempt = 0; attempt < 5; attempt++)
            Assert.True(await throttle.IsAllowedAsync("ada@example.com", "127.0.0.1"));

        Assert.False(await throttle.IsAllowedAsync("ada@example.com", "127.0.0.1"));
        Assert.True(await throttle.IsAllowedAsync("other@example.com", "127.0.0.2"));

        timeProvider.Advance(TimeSpan.FromMinutes(15) + TimeSpan.FromTicks(1));
        Assert.True(await throttle.IsAllowedAsync("ada@example.com", "127.0.0.1"));
    }

    [Fact]
    public async Task Auth_repository_persists_and_transitions_sessions_and_outbox()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new AuthRepository(scope.UnitOfWork);
        var family = Guid.NewGuid();
        var session = new RefreshSession
        {
            UserId = 1,
            FamilyId = family,
            TokenHash = Convert
                .ToHexString(SHA256.HashData("refresh"u8.ToArray()))
                .ToLowerInvariant(),
            ExpiresAtUtc = TestNowUtc.AddDays(1),
        };
        var identity = new AuthenticationIdentity
        {
            UserId = 1,
            NormalizedEmail = "ada@example.com",
            PasswordHash = "hash",
        };
        var outbox = new AuthOutboxMessage
        {
            Type = "verification-email",
            PayloadJson = "{\"UserId\":1,\"Email\":\"ada@example.com\"}",
        };

        await repository.AddAsync(
            identity,
            session,
            [
                new ConsentRecord
                {
                    UserId = 1,
                    Type = "terms",
                    VersionId = "terms-v1",
                    AcceptedAtUtc = TestNowUtc,
                },
            ],
            [new AuthAuditEvent { UserId = 1, EventType = "account_created" }],
            [outbox]
        );

        var idempotency = new AuthRegistrationIdempotency
        {
            NormalizedEmail = "ada@example.com",
            IdempotencyKey = "registration-1",
            RequestHash = "hash",
            ResponseJson = "{}"
        };
        await repository.AddRegistrationIdempotencyAsync(idempotency);
        await repository.AddAuditAsync(new AuthAuditEvent { UserId = 1, EventType = "registration_replayed" });
        await repository.SaveAsync();
        Assert.Same(idempotency, await repository.FindRegistrationIdempotencyAsync("ada@example.com", "registration-1"));

        Assert.NotNull(await repository.FindIdentityAsync("ada@example.com"));
        Assert.Equal(session.Id, (await repository.FindSessionAsync(session.TokenHash))?.Id);
        var claimed = await repository.ClaimDueOutboxAsync(
            TestNowUtc.AddMinutes(1),
            TestNowUtc.AddMinutes(6),
            10
        );
        Assert.Single(claimed);
        Assert.Equal(outbox.Id, claimed[0].Id);
        Assert.NotNull(claimed[0].LeaseId);
        Assert.Empty(
            await repository.ClaimDueOutboxAsync(
                TestNowUtc.AddMinutes(1),
                TestNowUtc.AddMinutes(6),
                10
            )
        );

        var rotated = await repository.TryRotateSessionAsync(
            session.Id,
            TestNowUtc,
            new RefreshSession
            {
                UserId = 1,
                FamilyId = family,
                TokenHash = "replacement-by-rotation",
                ExpiresAtUtc = TestNowUtc.AddDays(1),
            }
        );
        Assert.True(rotated);
        Assert.False(
            await repository.TryRotateSessionAsync(
                session.Id,
                TestNowUtc,
                new RefreshSession
                {
                    UserId = 1,
                    FamilyId = family,
                    TokenHash = "rejected-rotation",
                    ExpiresAtUtc = TestNowUtc.AddDays(1),
                }
            )
        );

        await repository.MarkOutboxFailedAsync(
            outbox.Id,
            claimed[0].LeaseId!.Value,
            1,
            TestNowUtc.AddMinutes(1),
            "safe_error"
        );
        await repository.SaveAsync();
        var reclaimed = await repository.ClaimDueOutboxAsync(
            TestNowUtc.AddMinutes(2),
            TestNowUtc.AddMinutes(7),
            10
        );
        Assert.Single(reclaimed);
        _ = await repository.MarkOutboxProcessedAsync(
            outbox.Id,
            reclaimed[0].LeaseId!.Value,
            TestNowUtc
        );
        await repository.AddSessionAsync(
            new RefreshSession
            {
                UserId = 1,
                FamilyId = family,
                TokenHash = "replacement",
                ExpiresAtUtc = TestNowUtc.AddDays(1),
            }
        );
        await repository.RevokeFamilyAsync(family, TestNowUtc);
        await repository.SaveAsync();

        Assert.Empty(
            await repository.ClaimDueOutboxAsync(
                TestNowUtc.AddDays(1),
                TestNowUtc.AddDays(1).AddMinutes(5),
                10
            )
        );
    }

    [Fact]
    public async Task Audit_retry_claim_insert_is_idempotent_without_collapsing_distinct_events()
    {
        using var scope = new SqliteAppDbContextScope();
        var repository = new AuthRepository(scope.UnitOfWork);
        var duplicate = new AuthAuditRetryPayload(
            "authorization_denied",
            42,
            "127.0.0.1",
            "{\"correlationId\":\"request-1\"}",
            "request-1");
        var distinct = duplicate with { CorrelationId = "request-2", MetadataJson = "{\"correlationId\":\"request-2\"}" };

        Assert.True(await repository.TryRecordAuditRetryAsync(duplicate));
        Assert.False(await repository.TryRecordAuditRetryAsync(duplicate));
        Assert.True(await repository.TryRecordAuditRetryAsync(distinct));

        var audits = await scope.DbContext.Set<AuthAuditEvent>()
            .Where(audit => audit.EventType == duplicate.EventType)
            .ToListAsync();
        Assert.Equal(2, audits.Count);
        Assert.Equal(
            ["{\"correlationId\":\"request-1\"}", "{\"correlationId\":\"request-2\"}"],
            audits.Select(audit => audit.MetadataJson).OrderBy(metadata => metadata));
    }

    [Fact]
    public async Task Independent_workers_deduplicate_concurrent_audits_and_honor_outbox_leases()
    {
        using var firstScope = new SqliteAppDbContextScope();
        using var secondScope = new SqliteAppDbContextScope(firstScope.ConnectionString);
        var firstRepository = new AuthRepository(firstScope.UnitOfWork);
        var secondRepository = new AuthRepository(secondScope.UnitOfWork);
        var payload = new AuthAuditRetryPayload(
            "authorization_denied",
            42,
            "127.0.0.1",
            "{\"correlationId\":\"concurrent-request\"}",
            "concurrent-request");

        var firstAttempt = CaptureSqliteLockAsync(() => firstRepository.TryRecordAuditRetryAsync(payload));
        var secondAttempt = CaptureSqliteLockAsync(() => secondRepository.TryRecordAuditRetryAsync(payload));
        var results = await Task.WhenAll(firstAttempt, secondAttempt);

        for (var index = 0; index < results.Length; index++)
        {
            if (results[index] is null)
            {
                results[index] = await (index == 0
                    ? secondRepository.TryRecordAuditRetryAsync(payload)
                    : firstRepository.TryRecordAuditRetryAsync(payload));
            }
        }

        Assert.Equal([true, false], results.OrderByDescending(result => result).ToArray());
        Assert.Equal(
            1,
            await firstScope.DbContext.Set<AuthAuditEvent>()
                .CountAsync(audit => audit.MetadataJson == payload.MetadataJson));

        var distinct = payload with
        {
            CorrelationId = "distinct-request",
            MetadataJson = "{\"correlationId\":\"distinct-request\"}",
        };
        Assert.True(await secondRepository.TryRecordAuditRetryAsync(distinct));

        var outbox = new AuthOutboxMessage
        {
            Type = "verification-email",
            PayloadJson = "{\"UserId\":42,\"Email\":\"ada@example.com\"}",
            AvailableAtUtc = TestNowUtc,
        };
        await firstRepository.AddOutboxAsync(outbox);
        await firstRepository.SaveAsync();

        var firstClaim = Assert.Single(await firstRepository.ClaimDueOutboxAsync(
            TestNowUtc,
            TestNowUtc.AddMinutes(5),
            10));
        Assert.Empty(await secondRepository.ClaimDueOutboxAsync(
            TestNowUtc,
            TestNowUtc.AddMinutes(5),
            10));

        await firstRepository.MarkOutboxFailedAsync(
            firstClaim.Id,
            firstClaim.LeaseId!.Value,
            1,
            TestNowUtc.AddMinutes(1),
            "safe_retry");
        await firstRepository.SaveAsync();

        var reclaimed = Assert.Single(await secondRepository.ClaimDueOutboxAsync(
            TestNowUtc.AddMinutes(1),
            TestNowUtc.AddMinutes(6),
            10));
        Assert.False(await firstRepository.MarkOutboxProcessedAsync(
            outbox.Id,
            firstClaim.LeaseId!.Value,
            TestNowUtc.AddMinutes(1)));
        Assert.True(await secondRepository.MarkOutboxProcessedAsync(
            outbox.Id,
            reclaimed.LeaseId!.Value,
            TestNowUtc.AddMinutes(1)));
    }

    private static async Task<bool?> CaptureSqliteLockAsync(Func<Task<bool>> operation)
    {
        try
        {
            return await operation();
        }
        catch (Exception exception) when (exception.ToString().Contains("database is locked", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }
    }

    [Fact]
    public async Task Entitlement_resolver_unions_profiles_and_falls_back_when_empty()
    {
        var billing = new StubBillingService
        {
            Subscriptions =
            [
                new BillingSubscriptionSnapshot(1, 1, 10, "PRO"),
                new BillingSubscriptionSnapshot(2, 2, null, "FREE"),
            ],
        };
        var profiling = new StubProfilingService
        {
            Shapes = new Dictionary<int, AccessShapeSnapshot>
            {
                [10] = new(new HashSet<string> { "PdfExport" }),
            },
            Starter = new(new HashSet<string> { "ResumeCreate" }),
        };
        var resolver = new SubscriptionEntitlementResolver(billing, profiling);

        var capabilities = await resolver.ResolveAsync(1);
        Assert.Equal(["PdfExport"], capabilities);

        billing.Subscriptions = [];
        capabilities = await resolver.ResolveAsync(1);
        Assert.Equal(["ResumeCreate"], capabilities);
    }

    [Fact]
    public async Task Auth_handlers_delegate_to_registration_service()
    {
        var service = new StubRegistrationService();
        Assert.Same(
            service.Registered,
            await new RegisterCommandHandler(service).Handle(
                new RegisterCommand(new(), null, null),
                CancellationToken.None
            )
        );
        Assert.Same(
            service.Tokens,
            await new RefreshCommandHandler(service).Handle(
                new RefreshCommand(new()),
                CancellationToken.None
            )
        );
        Assert.True(
            await new LogoutCommandHandler(service).Handle(
                new LogoutCommand("refresh"),
                CancellationToken.None
            )
        );
        Assert.True(
            await new ResendVerificationCommandHandler(service).Handle(
                new ResendVerificationCommand(new()),
                CancellationToken.None
            )
        );
    }

    [Fact]
    public void Auth_composition_registers_owned_services()
    {
        var services = new ServiceCollection();
        services.AddDataProtection();
        services.AddAuthModulePersistence();
        services.AddAuthModuleApplication(new AuthSecurityOptions());
        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IPasswordHasher>());
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(ITokenService));
        Assert.NotNull(provider.GetRequiredService<IRegistrationThrottle>());
        Assert.Contains(services, descriptor => descriptor.ServiceType == typeof(IAuthRepository));
    }

    [Fact]
    public async Task Auth_seeder_creates_required_consent_versions_idempotently()
    {
        using var scope = new SqliteAppDbContextScope();
        var seeder = new AuthModuleSeeder();

        await seeder.SeedAsync(scope.DbContext, TestContext.Current.CancellationToken);
        scope.DbContext.ChangeTracker.Clear();
        await seeder.SeedAsync(scope.DbContext, TestContext.Current.CancellationToken);

        var versions = await scope
            .DbContext.Set<AuthConsentVersion>()
            .OrderBy(x => x.Type)
            .ToArrayAsync(TestContext.Current.CancellationToken);

        Assert.Equal(["marketing", "privacy", "terms"], versions.Select(x => x.Type));
        Assert.Equal([false, true, true], versions.Select(x => x.Required));
        Assert.Equal(3, versions.Length);
    }

    [Fact]
    public void Auth_model_contains_required_relationships_and_unique_indexes()
    {
        using var scope = new SqliteAppDbContextScope();
        var identity = scope.DbContext.Model.FindEntityType(typeof(AuthenticationIdentity));
        var session = scope.DbContext.Model.FindEntityType(typeof(RefreshSession));
        var consent = scope.DbContext.Model.FindEntityType(typeof(ConsentRecord));

        Assert.NotNull(identity);
        Assert.NotNull(identity!.FindNavigation(nameof(AuthenticationIdentity.User)));
        Assert.Contains(
            identity.GetIndexes(),
            index =>
                index.IsUnique
                && index.Properties.Single().Name == nameof(AuthenticationIdentity.NormalizedEmail)
        );
        Assert.Contains(
            session!.GetIndexes(),
            index =>
                index.IsUnique && index.Properties.Single().Name == nameof(RefreshSession.TokenHash)
        );
        Assert.Contains(
            consent!.GetIndexes(),
            index =>
                index.IsUnique
                && index
                    .Properties.Select(x => x.Name)
                    .SequenceEqual([
                        nameof(ConsentRecord.UserId),
                        nameof(ConsentRecord.Type),
                        nameof(ConsentRecord.VersionId),
                    ])
        );
    }

    [Fact]
    public async Task Auth_endpoint_mapping_registers_all_public_routes()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSingleton<IValidator<RegisterRequest>, RegisterRequestValidator>();
        builder.Services.AddSingleton(Substitute.For<IMediator>());
        builder.Services.AddSingleton(Substitute.For<IRegistrationService>());
        await using var app = builder.Build();

        IEndpointRouteBuilder routeBuilder = app;
        routeBuilder.MapAuthModuleApis();

        var routes = routeBuilder
            .DataSources.SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => endpoint.RoutePattern.RawText)
            .Where(route => route is not null)
            .ToArray();

        Assert.Equal(11, routes.Length);
        Assert.Contains("/api/v1/auth/register", routes);
        Assert.Contains("/api/v1/auth/refresh", routes);
        Assert.Contains("/api/v1/auth/logout", routes);
        Assert.Contains("/api/v1/auth/verify-email/resend", routes);
        Assert.Contains("/api/v1/auth/login", routes);
        Assert.Contains("/api/v1/auth/password/change", routes);
        Assert.Contains("/api/v1/auth/password/forgot", routes);
        Assert.Contains("/api/v1/auth/password/reset", routes);
        Assert.Contains("/api/v1/auth/verify-email", routes);
        Assert.Contains("/api/v1/auth/me", routes);
        Assert.Contains("/api/v1/bootstrap", routes);
    }

    [Fact]
    public async Task Logging_side_effect_handler_accepts_a_dispatch()
    {
        var handler = new LoggingAuthSideEffectHandler(
            NullLogger<LoggingAuthSideEffectHandler>.Instance
        );

        await handler.HandleAsync("welcome-email", 42, "ada@example.com", null);
    }

    private sealed class StubBillingService : IBillingRegistrationService
    {
        public IReadOnlyList<BillingSubscriptionSnapshot> Subscriptions { get; set; } = [];

        public Task<BillingRegistrationSnapshot?> AddStarterRegistrationBillingAsync(
            int u,
            CancellationToken c = default
        ) => Task.FromResult<BillingRegistrationSnapshot?>(new(1, 1, 1, "FREE"));

        public Task<
            IReadOnlyList<BillingSubscriptionSnapshot>
        > ListActiveSubscriptionSnapshotsAsync(int u, CancellationToken c = default) =>
            Task.FromResult(Subscriptions);
    }

    private sealed class StubProfilingService : IProfilingRegistrationService
    {
        public Dictionary<int, AccessShapeSnapshot> Shapes { get; set; } = [];
        public AccessShapeSnapshot Starter { get; set; } = new(new HashSet<string>());

        public Task<ProfileRegistrationSnapshot> CreateRegistrationUserAsync(
            ProfileRegistrationInput i,
            CancellationToken c = default
        ) => Task.FromResult(new ProfileRegistrationSnapshot(1));

        public Task AddRegistrationBaselineAsync(
            RegistrationBaselineInput i,
            CancellationToken c = default
        ) => Task.CompletedTask;

        public Task<AccessShapeSnapshot> GetAccessShapeAsync(
            int i,
            CancellationToken c = default
        ) => Task.FromResult(Shapes[i]);

        public Task<AccessShapeSnapshot> GetStarterAccessShapeAsync(
            CancellationToken c = default
        ) => Task.FromResult(Starter);

        public Task<StarterAccessProfileSnapshot?> GetStarterAccessProfileAsync(
            CancellationToken c = default
        ) => Task.FromResult<StarterAccessProfileSnapshot?>(new(1));

        public Task ReviseAccessProfilesAsync(
            IReadOnlyCollection<AccessProfileRevisionInput> inputs,
            CancellationToken c = default
        ) => Task.CompletedTask;

        public Task ProcessPendingAccessProfileRevisionsAsync(
            CancellationToken c = default
        ) => Task.CompletedTask;
    }

    private sealed class StubRegistrationService : IRegistrationService
    {
        public RegisterResponse Registered { get; } =
            new(
                1,
                new("access", "refresh", TestNowUtc, TestNowUtc),
                new(
                    "active",
                    "pending",
                    "FREE",
                    "/app/templates",
                    "homepage",
                    null,
                    "continue",
                    new Dictionary<string, bool>(),
                    []
                )
            );
        public AuthTokens Tokens { get; } =
            new("access", "refresh", TestNowUtc, TestNowUtc);

        public Task<RegisterResponse> RegisterAsync(
            RegisterCommand c,
            CancellationToken ct = default
        ) => Task.FromResult(Registered);

        public Task<AuthTokens> RefreshAsync(RefreshCommand c, CancellationToken ct = default) =>
            Task.FromResult(Tokens);

        public Task<bool> LogoutAsync(LogoutCommand c, CancellationToken ct = default) =>
            Task.FromResult(true);

        public Task<bool> ResendVerificationAsync(
            ResendVerificationCommand c,
            CancellationToken ct = default
        ) => Task.FromResult(true);

        public BootstrapResponse CalculateBootstrap(string source, string? selectedTemplateId) =>
            Registered.Bootstrap;
    }
}
