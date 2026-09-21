using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using ResumeEnhancer.AuthModule.AM.Requests;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.PL.Repositories;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Contracts;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.Web.Outbox;
using ResumeEnhancer.Infrastructure.Persistence.Limiting;
using ResumeEnhancer.Core.CommonLibrary.Resilience;
using ResumeEnhancer.ProfilingModule.SL.Integrations;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthCoverageGapTests
{
    private static readonly DateTimeOffset Now = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task Repository_rotates_only_when_identity_and_session_are_current()
    {
        using var scope = new SqliteAppDbContextScope();
        var identity = new AuthenticationIdentity
        {
            UserId = 1, NormalizedEmail = "current@example.com", PasswordHash = "hash", EmailVerified = true,
        };
        var session = new RefreshSession
        {
            UserId = 1, FamilyId = Guid.NewGuid(), TokenHash = "current-token", ExpiresAtUtc = Now.UtcDateTime.AddHours(1),
        };
        scope.DbContext.AddRange(identity, session);
        await scope.DbContext.SaveChangesAsync();
        var repository = new AuthRepository(scope.UnitOfWork);

        var result = await repository.TryRotateSessionIfCurrentAsync(
            session.Id, 1, session.SessionKey, Now.UtcDateTime,
            new RefreshSession { UserId = 1, FamilyId = session.FamilyId, TokenHash = "replacement", ExpiresAtUtc = Now.UtcDateTime.AddDays(1) });

        Assert.Equal(RefreshRotationResult.Rotated, result);
        Assert.Equal(RefreshRotationResult.Rejected, await repository.TryRotateSessionIfCurrentAsync(
            session.Id, 1, session.SessionKey, Now.UtcDateTime,
            new RefreshSession { UserId = 1, FamilyId = session.FamilyId, TokenHash = "replay", ExpiresAtUtc = Now.UtcDateTime.AddDays(1) }));
    }

    [Fact]
    public Task Repository_rejects_rotation_for_unverified_identity() =>
        AssertRotationRejectedAsync((identity, _) => identity.EmailVerified = false);

    [Fact]
    public Task Repository_rejects_rotation_for_locked_identity() =>
        AssertRotationRejectedAsync((identity, _) => identity.LockedUntilUtc = Now.UtcDateTime.AddMinutes(5));

    [Fact]
    public Task Repository_rejects_rotation_for_deactivated_user() =>
        AssertRotationRejectedAsync((_, user) => user.IsDeactivated = true);

    private static async Task AssertRotationRejectedAsync(Action<AuthenticationIdentity, User> mutate)
    {
        using var scope = new SqliteAppDbContextScope();
        var user = new User { FirstName = "Ada", LastName = "Lovelace", Email = "rotation@example.com" };
        var identity = new AuthenticationIdentity
        {
            User = user, NormalizedEmail = "rotation@example.com", PasswordHash = "hash", EmailVerified = true,
        };
        var session = new RefreshSession
        {
            UserId = 0,
            FamilyId = Guid.NewGuid(),
            TokenHash = "rotation-token",
            ExpiresAtUtc = Now.UtcDateTime.AddHours(1),
        };
        scope.DbContext.AddRange(user, identity, session);
        await scope.DbContext.SaveChangesAsync();
        mutate(identity, user);
        await scope.DbContext.SaveChangesAsync();
        var repository = new AuthRepository(scope.UnitOfWork);
        Assert.Equal(RefreshRotationResult.CurrentStateInvalid, await repository.TryRotateSessionIfCurrentAsync(
            session.Id, user.Id, session.SessionKey, Now.UtcDateTime,
            new RefreshSession { UserId = user.Id, FamilyId = session.FamilyId, TokenHash = "replacement", ExpiresAtUtc = Now.UtcDateTime.AddDays(1) }));
    }

    [Fact]
    public async Task Db_throttle_covers_fail_open_degraded_provider_and_cancellation()
    {
        var time = new FakeTimeProvider(Now);
        var noStore = new DbCacheRegistrationThrottle(timeProvider: time);
        var degraded = await noStore.TryConsumeAsync(LimiterOperation.Login, null, null, null);
        Assert.True(degraded.Allowed);
        Assert.True(degraded.Degraded);
        Assert.Equal(10, degraded.Limit);

        var store = Substitute.For<IAtomicLimiterStore>();
        store.TryConsumeAsync(Arg.Any<string>(), 5, Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(new AtomicLimiterDecision(false, 5, 5, Now.UtcDateTime.AddMinutes(15), true));
        var throttle = new DbCacheRegistrationThrottle(store, NullLogger<DbCacheRegistrationThrottle>.Instance, time);
        var denied = await throttle.TryConsumeAsync(LimiterOperation.Registration, "ada@example.com", null, "127.0.0.1");
        Assert.False(denied.Allowed);
        Assert.True(denied.Degraded);

        store.TryConsumeAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException<AtomicLimiterDecision>(new InvalidOperationException("provider secret")));
        var failedOpen = await throttle.TryConsumeAsync(LimiterOperation.Logout, null, "42", null);
        Assert.True(failedOpen.Allowed);
        Assert.True(failedOpen.Degraded);

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        store.TryConsumeAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<TimeSpan>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromCanceled<AtomicLimiterDecision>(cancellation.Token));
        await Assert.ThrowsAsync<TaskCanceledException>(() => throttle.TryConsumeAsync(
            LimiterOperation.Refresh, "ada@example.com", null, null, cancellation.Token));
    }

    [Fact]
    public void Security_options_validator_covers_persistence_cookie_and_origin_boundaries()
    {
        Assert.Throws<ArgumentNullException>(() => AuthSecurityOptionsValidator.Validate(null!));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { AccessTokenLifetime = TimeSpan.Zero }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { ClockSkew = TimeSpan.FromMinutes(-1) }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { DataProtectionKeyRingPath = "missing" }, requirePersistedKeyRing: true));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { TrustedOrigins = ["resume.example.com"] }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { RefreshCookiePath = "/auth" }));
    }

    [Fact]
    public async Task In_memory_throttle_expires_email_and_ip_windows_independently()
    {
        var time = new FakeTimeProvider(Now);
        var throttle = new InMemoryRegistrationThrottle(time);
        Assert.True(await throttle.IsAllowedAsync("a@example.com", "1.1.1.1"));
        for (var i = 0; i < 4; i++) Assert.True(await throttle.IsAllowedAsync($"other{i}@example.com", "1.1.1.1"));
        Assert.False(await throttle.IsAllowedAsync("new@example.com", "1.1.1.1"));
        time.Advance(TimeSpan.FromMinutes(15) + TimeSpan.FromTicks(1));
        Assert.True(await throttle.IsAllowedAsync("new@example.com", "1.1.1.1"));
    }

    [Fact]
    public async Task Outbox_dispatcher_handles_invalid_payload_and_cancellation_without_leaking_details()
    {
        var repository = Substitute.For<IAuthRepository>();
        repository.ClaimDueOutboxAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), 25, Arg.Any<CancellationToken>())
            .Returns([new AuthOutboxMessage { Id = 7, LeaseId = Guid.NewGuid(), Type = "welcome-email", PayloadJson = "not-json", Attempts = 4 }]);
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthSideEffectHandler>(_ => Substitute.For<IAuthSideEffectHandler>())
            .AddScoped<IAuthAuditRecorder>(_ => Substitute.For<IAuthAuditRecorder>())
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(services.GetRequiredService<IServiceScopeFactory>(), NullLogger<AuthOutboxDispatcher>.Instance, new FakeTimeProvider(Now));
        Assert.Equal(0, await dispatcher.ProcessOnceAsync());
        await repository.Received(1).MarkOutboxFailedAsync(7, Arg.Any<Guid>(), 5, DateTime.MaxValue, "side_effect_payload_invalid", Arg.Any<CancellationToken>());

        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        repository.ClaimDueOutboxAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), 25, cancellation.Token)
            .Returns(_ => Task.FromCanceled<IReadOnlyList<AuthOutboxMessage>>(cancellation.Token));
        await Assert.ThrowsAsync<TaskCanceledException>(() => dispatcher.ProcessOnceAsync(cancellation.Token));
    }

    [Fact]
    public async Task Outbox_dispatcher_processes_audit_retry_and_stops_cleanly_when_cancelled()
    {
        var repository = Substitute.For<IAuthRepository>();
        var retry = new AuthAuditRetryPayload("login_failed", 42, "127.0.0.1", "{\"correlationId\":\"c1\"}", "c1");
        repository.ClaimDueOutboxAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), 25, Arg.Any<CancellationToken>())
            .Returns([new AuthOutboxMessage
            {
                Id = 8,
                LeaseId = Guid.NewGuid(),
                Type = "auth-audit-retry",
                PayloadJson = System.Text.Json.JsonSerializer.Serialize(retry),
            }]);
        repository.MarkOutboxProcessedAsync(8, Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(true);
        var services = new ServiceCollection()
            .AddScoped<IAuthRepository>(_ => repository)
            .AddScoped<IAuthSideEffectHandler>(_ => Substitute.For<IAuthSideEffectHandler>())
            .AddScoped<IAuthAuditRecorder>(_ => Substitute.For<IAuthAuditRecorder>())
            .BuildServiceProvider();
        var dispatcher = new AuthOutboxDispatcher(services.GetRequiredService<IServiceScopeFactory>(), NullLogger<AuthOutboxDispatcher>.Instance, new FakeTimeProvider(Now));

        Assert.Equal(1, await dispatcher.ProcessOnceAsync());
        await repository.Received(1).TryRecordAuditRetryAsync(Arg.Is<AuthAuditRetryPayload>(x => x.EventType == "login_failed"), Arg.Any<CancellationToken>());
        await repository.Received(1).SaveAsync(Arg.Any<CancellationToken>());

        using var cancellation = new CancellationTokenSource();
        await dispatcher.StartAsync(cancellation.Token);
        cancellation.Cancel();
        await dispatcher.StopAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Lifecycle_service_completes_password_recovery_verification_and_me_paths()
    {
        var repository = Substitute.For<IAuthRepository>();
        var users = Substitute.For<IUserLookupService>();
        var hasher = Substitute.For<IPasswordHasher>();
        var tokens = Substitute.For<ITokenService>();
        var security = Substitute.For<IAuthSecurityStateService>();
        var limiter = Substitute.For<IRegistrationThrottle>();
        var profiling = Substitute.For<IProfilingAuthorizationService>();
        var audit = Substitute.For<IAuthAuditRecorder>();
        var challengeFactory = Substitute.For<IAuthChallengeFactory>();
        var protector = Substitute.For<IAuthChallengeDeliveryProtector>();
        limiter.TryConsumeAsync(Arg.Any<LimiterOperation>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new LimiterDecision(true, 1, 5, Now.UtcDateTime.AddMinutes(1)));
        var identity = new AuthenticationIdentity
        {
            Id = 7, UserId = 42, NormalizedEmail = "ada@example.com", PasswordHash = "old", EmailVerified = true,
        };
        repository.FindIdentityAsync("ada@example.com", Arg.Any<CancellationToken>()).Returns(identity);
        repository.FindIdentityForUserIdAsync(42, Arg.Any<CancellationToken>()).Returns(identity);
        users.GetUserStateAsync(42, Arg.Any<CancellationToken>()).Returns(new ProfilingUserStateSnapshot(42, false, false));
        profiling.GetUserAuthorizationAsync(42, Arg.Any<CancellationToken>()).Returns(ProfilingAuthorizationSnapshot.Empty(42));
        hasher.Verify("old", "current").Returns(true);
        hasher.Hash("new").Returns("new-hash");
        security.IsPasswordReusedAsync(7, "new", Arg.Any<CancellationToken>()).Returns(false);
        var challenge = new AuthChallenge { AuthenticationIdentityId = 7, AuthChallengePurposeId = 1, TokenHash = "hashed", IssuedAtUtc = Now.UtcDateTime, ExpiresAtUtc = Now.UtcDateTime.AddMinutes(30) };
        challengeFactory.CreateAsync(identity, "password-reset", Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns((challenge, "raw-reset"));
        protector.Protect("raw-reset", "password-reset", 42, "ada@example.com").Returns("protected-reset");
        repository.FindActiveChallengePurposeIdAsync("password-reset", Arg.Any<CancellationToken>()).Returns(1);
        repository.ConsumeChallengeAndUpdatePasswordAsync(7, 42, "password-reset", Arg.Any<string>(), "new-hash", Arg.Any<DateTime>(), "password_reset", Arg.Any<CancellationToken>()).Returns(true);
        repository.TryConsumeChallengeAsync(7, "email-verification", Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>()).Returns(ChallengeConsumeResult.Consumed);

        var service = new AuthLifecycleService(repository, users, hasher, tokens, security, limiter, profiling, audit,
            new FakeTimeProvider(Now), challengeFactory, protector);

        Assert.True(await service.ChangePasswordAsync(new ChangePasswordCommand(new ChangePasswordRequest { CurrentPassword = "current", NewPassword = "new" }, 42), default));
        Assert.True(await service.ForgotPasswordAsync(new ForgotPasswordCommand(new ForgotPasswordRequest { Email = "ada@example.com" }, "ip", "ua"), default));
        Assert.True(await service.ResetPasswordAsync(new ResetPasswordCommand(new ResetPasswordRequest { Email = "ada@example.com", Challenge = "reset", NewPassword = "new" }, "ip"), default));
        Assert.True(await service.VerifyEmailAsync(new VerifyEmailCommand(new VerifyEmailRequest { Email = "ada@example.com", Challenge = "verify" }, "ip"), default));
        Assert.NotNull(await service.GetMeAsync(42, default));
        await repository.Received(1).UpdatePasswordAndRevokeSessionsAsync(7, 42, "new-hash", Arg.Any<DateTime>(), "password_changed", Arg.Any<CancellationToken>());
        await repository.Received(1).MarkEmailVerifiedAsync(7, Arg.Any<DateTime>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Audit_recorder_redacts_invalid_metadata_and_tolerates_retry_signal_failures()
    {
        var repository = Substitute.For<IAuthRepository>();
        var signal = Substitute.For<IAuthAuditFailureSignal>();
        signal.SignalAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromException(new InvalidOperationException("signal unavailable")));
        var recorder = new AuthAuditRecorder(repository, signal, new ThrowingResilienceExecutor());

        await recorder.RecordAsync("login_failed", 42, "127.0.0.1", "not-json", correlationId: "corr-1");

        await repository.Received(1).QueueAuditRetryAsync(Arg.Is<AuthAuditRetryPayload>(x =>
            x.EventType == "login_failed" && x.CorrelationId == "corr-1" && x.MetadataJson.Contains("not-json")), Arg.Any<CancellationToken>());
        await signal.Received(1).SignalAsync("login_failed", true, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Lifecycle_service_covers_throttle_denial_degraded_recovery_and_verification_paths()
    {
        var repository = Substitute.For<IAuthRepository>();
        var users = Substitute.For<IUserLookupService>();
        var security = Substitute.For<IAuthSecurityStateService>();
        var limiter = Substitute.For<IRegistrationThrottle>();
        var audit = Substitute.For<IAuthAuditRecorder>();
        var identity = new AuthenticationIdentity { Id = 3, UserId = 42, NormalizedEmail = "ada@example.com", PasswordHash = "stored" };
        repository.FindIdentityAsync("ada@example.com", Arg.Any<CancellationToken>()).Returns(identity);
        limiter.TryConsumeAsync(Arg.Any<LimiterOperation>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new LimiterDecision(false, 5, 5, Now.UtcDateTime.AddMinutes(1)));
        var service = CreateLifecycle(repository, users, security, limiter, audit);
        await Assert.ThrowsAsync<AuthException>(() => service.ForgotPasswordAsync(new(new() { Email = "ada@example.com" }, "ip", null), default));

        limiter.TryConsumeAsync(Arg.Any<LimiterOperation>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(new LimiterDecision(true, 1, 5, Now.UtcDateTime.AddMinutes(1), true));
        repository.FindActiveChallengePurposeIdAsync("password-reset", Arg.Any<CancellationToken>()).Returns((int?)null);
        Assert.True(await service.ForgotPasswordAsync(new(new() { Email = "ada@example.com" }, "ip", null), default));
        repository.TryConsumeChallengeAsync(3, "email-verification", Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(ChallengeConsumeResult.Consumed);
        Assert.True(await service.VerifyEmailAsync(new(new() { Email = "ada@example.com", Challenge = "challenge" }, "ip"), default));
        await audit.Received().RecordAsync("limiter_degraded", null, "ip", Arg.Any<string>(), Arg.Any<CancellationToken>(), Arg.Any<string?>());
    }

    private static AuthLifecycleService CreateLifecycle(
        IAuthRepository repository, IUserLookupService users, IAuthSecurityStateService security,
        IRegistrationThrottle limiter, IAuthAuditRecorder audit)
    {
        return new AuthLifecycleService(repository, users, Substitute.For<IPasswordHasher>(), Substitute.For<ITokenService>(), security,
            limiter, Substitute.For<IProfilingAuthorizationService>(), audit, new FakeTimeProvider(Now),
            Substitute.For<IAuthChallengeFactory>(), Substitute.For<IAuthChallengeDeliveryProtector>());
    }

    private sealed class ThrowingResilienceExecutor : IResilienceExecutor
    {
        public Task<T> ExecuteAsync<T>(string profileName, ResilienceOperation operation, Func<CancellationToken, Task<T>> callback,
            ResilienceRetryPredicate? retryPredicate = null, CancellationToken cancellationToken = default) =>
            Task.FromException<T>(new InvalidOperationException("audit unavailable"));

        public Task ExecuteAsync(string profileName, ResilienceOperation operation, Func<CancellationToken, Task> callback,
            ResilienceRetryPredicate? retryPredicate = null, CancellationToken cancellationToken = default) =>
            Task.FromException(new InvalidOperationException("audit unavailable"));
    }
}
