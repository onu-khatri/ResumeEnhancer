using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.AuthModule.PL;
using ResumeEnhancer.AuthModule.PL.Repositories;
using ResumeEnhancer.AuthModule.SL.Abstractions;
using ResumeEnhancer.AuthModule.SL.Services;
using ResumeEnhancer.AuthModule.SL.Options;
using ResumeEnhancer.AuthModule.Web;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthDurableStateTests
{
    private static readonly DateTime TestNowUtc = new(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Auth_security_options_validate_defaults_and_test_overrides()
    {
        var options = new AuthSecurityOptions
        {
            LockoutThreshold = 3,
            LockoutWindow = TimeSpan.FromMinutes(5),
            ProgressiveLoginDelays = [TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8), TimeSpan.FromSeconds(16)]
        };
        AuthSecurityOptionsValidator.Validate(options);
        AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions
        {
            TrustedOrigins =
            [
                "https://app.dev.resumeenhancer.local",
                "https://www.dev.resumeenhancer.local",
            ],
        });
        var policy = new ProgressiveLoginDelayPolicy(options);
        Assert.Equal(TimeSpan.FromSeconds(2), policy.Calculate(2));
        Assert.Equal(TimeSpan.FromSeconds(16), policy.Calculate(99));
    }

    [Fact]
    public void Auth_security_options_reject_invalid_values()
    {
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { LockoutThreshold = 0 }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { LockoutWindow = TimeSpan.Zero }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { ProgressiveLoginDelays = [TimeSpan.Zero, TimeSpan.FromSeconds(2)] }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { ProgressiveLoginDelays = [TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15)] }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { RefreshCookieSameSite = "None" }));
        Assert.Throws<InvalidOperationException>(() => AuthSecurityOptionsValidator.Validate(new AuthSecurityOptions { TrustedOrigins = ["https://*.resumeenhancer.com"] }));
    }

    [Fact]
    public void Durable_auth_model_has_hash_only_entities_and_expected_indexes()
    {
        using var scope = new SqliteAppDbContextScope();
        var model = scope.DbContext.Model;

        var identity = model.FindEntityType(typeof(AuthenticationIdentity));
        var history = model.FindEntityType(typeof(PasswordHistoryEntry));
        var challenge = model.FindEntityType(typeof(AuthChallenge));
        var purpose = model.FindEntityType(typeof(AuthChallengePurpose));
        var keyMetadata = model.FindEntityType(typeof(AuthSigningKeyMetadata));

        Assert.NotNull(identity);
        Assert.NotNull(history);
        Assert.NotNull(challenge);
        Assert.NotNull(purpose);
        Assert.NotNull(keyMetadata);
        Assert.Contains(identity!.GetIndexes(), x => x.Properties.Any(p => p.Name == nameof(AuthenticationIdentity.LockedUntilUtc)));
        Assert.Contains(history!.GetIndexes(), x => x.Properties.Select(p => p.Name)
            .SequenceEqual([nameof(PasswordHistoryEntry.AuthenticationIdentityId), nameof(PasswordHistoryEntry.CreatedAtUtc)]));
        Assert.Contains(challenge!.GetIndexes(), x => x.IsUnique && x.Properties.Select(p => p.Name)
            .SequenceEqual([nameof(AuthChallenge.AuthenticationIdentityId), nameof(AuthChallenge.AuthChallengePurposeId), nameof(AuthChallenge.TokenHash)]));
        Assert.Contains(challenge.GetForeignKeys(), x => x.Properties.Select(p => p.Name)
            .SequenceEqual([nameof(AuthChallenge.AuthChallengePurposeId)]));
        Assert.DoesNotContain(typeof(AuthChallenge).GetProperties(), x => x.Name == "PurposeCode");
        Assert.DoesNotContain(typeof(AuthChallenge).GetProperties(), x => x.Name.Contains("Token", StringComparison.OrdinalIgnoreCase) && x.Name != nameof(AuthChallenge.TokenHash));
        Assert.Contains(typeof(AuthSigningKeyMetadata).GetProperties(), x => x.Name == nameof(AuthSigningKeyMetadata.ProtectedMaterial));
    }

    [Fact]
    public async Task Password_history_detects_current_and_previous_two_only()
    {
        using var scope = new SqliteAppDbContextScope();
        var hasher = new AuthPasswordHasher();
        var identity = new AuthenticationIdentity
        {
            UserId = 1,
            NormalizedEmail = "durable@example.com",
            PasswordHash = hasher.Hash("current-password"),
        };
        scope.DbContext.Add(identity);
        await scope.DbContext.SaveChangesAsync();
        scope.DbContext.AddRange(
            new PasswordHistoryEntry { AuthenticationIdentityId = identity.Id, PasswordHash = hasher.Hash("previous-one"), CreatedAtUtc = TestNowUtc.AddMinutes(-1) },
            new PasswordHistoryEntry { AuthenticationIdentityId = identity.Id, PasswordHash = hasher.Hash("previous-two"), CreatedAtUtc = TestNowUtc.AddMinutes(-2) },
            new PasswordHistoryEntry { AuthenticationIdentityId = identity.Id, PasswordHash = hasher.Hash("older-password"), CreatedAtUtc = TestNowUtc.AddMinutes(-3) });
        await scope.DbContext.SaveChangesAsync();
        scope.DbContext.ChangeTracker.Clear();

        var options = new AuthSecurityOptions();
        var service = new AuthSecurityStateService(new AuthRepository(scope.UnitOfWork), hasher, new ProgressiveLoginDelayPolicy(options), options);
        Assert.True(await service.IsPasswordReusedAsync(identity.Id, "current-password"));
        Assert.True(await service.IsPasswordReusedAsync(identity.Id, "previous-one"));
        Assert.True(await service.IsPasswordReusedAsync(identity.Id, "previous-two"));
        Assert.False(await service.IsPasswordReusedAsync(identity.Id, "older-password"));
        Assert.False(await service.IsPasswordReusedAsync(identity.Id, "new-password"));
    }

    [Fact]
    public async Task Fifth_failed_login_locks_for_fifteen_minutes_and_locked_attempt_does_not_extend_it()
    {
        using var scope = new SqliteAppDbContextScope();
        var identity = new AuthenticationIdentity { UserId = 1, NormalizedEmail = "lock@example.com", PasswordHash = "protected-hash" };
        scope.DbContext.Add(identity);
        await scope.DbContext.SaveChangesAsync();
        var options = new AuthSecurityOptions();
        var service = new AuthSecurityStateService(new AuthRepository(scope.UnitOfWork), new AuthPasswordHasher(), new ProgressiveLoginDelayPolicy(options), options);
        var now = new DateTime(2026, 9, 15, 7, 0, 0, DateTimeKind.Utc);

        for (var attempt = 1; attempt <= 4; attempt++)
        {
            var result = await service.RecordFailedLoginAsync(identity.Id, now.AddSeconds(attempt));
            Assert.Equal(attempt, result.FailedAttempts);
            Assert.Null(result.LockedUntilUtc);
        }

        var locked = await service.RecordFailedLoginAsync(identity.Id, now.AddSeconds(5));
        Assert.Equal(5, locked.FailedAttempts);
        Assert.Equal(now.AddSeconds(5).AddMinutes(15), locked.LockedUntilUtc);
        Assert.Equal(TimeSpan.FromSeconds(15), locked.Delay);

        var stillLocked = await service.RecordFailedLoginAsync(identity.Id, now.AddMinutes(1));
        Assert.Equal(locked.FailedAttempts, stillLocked.FailedAttempts);
        Assert.Equal(locked.LockedUntilUtc, stillLocked.LockedUntilUtc);

        var afterWindow = await service.RecordFailedLoginAsync(identity.Id, now.AddMinutes(16));
        Assert.Equal(1, afterWindow.FailedAttempts);
        Assert.Null(afterWindow.LockedUntilUtc);
    }

    [Fact]
    public async Task Overlapping_failed_logins_do_not_lose_increments_or_bypass_threshold()
    {
        using var scope = new SqliteAppDbContextScope();
        var identity = new AuthenticationIdentity { UserId = 1, NormalizedEmail = "concurrent-lock@example.com", PasswordHash = "protected-hash" };
        scope.DbContext.Add(identity);
        await scope.DbContext.SaveChangesAsync();
        var now = new DateTime(2026, 9, 15, 7, 0, 0, DateTimeKind.Utc);
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var workers = Enumerable.Range(0, 5)
            .Select(_ => new SqliteAppDbContextScope(scope.ConnectionString))
            .ToArray();

        try
        {
            var operations = workers.Select(worker => Task.Run(async () =>
            {
                var options = new AuthSecurityOptions();
                var service = new AuthSecurityStateService(
                    new AuthRepository(worker.UnitOfWork),
                    new AuthPasswordHasher(),
                    new ProgressiveLoginDelayPolicy(options),
                    options);
                await start.Task;
                return await service.RecordFailedLoginAsync(identity.Id, now);
            })).ToArray();
            start.SetResult();
            await Task.WhenAll(operations);

            scope.DbContext.ChangeTracker.Clear();
            var saved = await scope.DbContext.Set<AuthenticationIdentity>().SingleAsync(x => x.Id == identity.Id);
            Assert.Equal(5, saved.FailedLoginAttempts);
            Assert.Equal(now.AddMinutes(15), saved.LockedUntilUtc);
        }
        finally
        {
            foreach (var worker in workers)
                worker.Dispose();
        }
    }

    [Fact]
    public async Task Challenge_is_hash_only_single_use_and_expiry_aware()
    {
        using var scope = new SqliteAppDbContextScope();
        var identity = new AuthenticationIdentity { UserId = 1, NormalizedEmail = "challenge@example.com", PasswordHash = "protected-hash" };
        scope.DbContext.Add(identity);
        await scope.DbContext.SaveChangesAsync();
        var repository = new AuthRepository(scope.UnitOfWork);
        var now = new DateTime(2026, 9, 15, 7, 0, 0, DateTimeKind.Utc);
        var purposeId = await repository.FindActiveChallengePurposeIdAsync("password-reset");
        Assert.NotNull(purposeId);
        await repository.AddChallengeAsync(new AuthChallenge
        {
            AuthenticationIdentityId = identity.Id,
            AuthChallengePurposeId = purposeId.Value,
            TokenHash = "hashed-challenge",
            IssuedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(30),
        });
        await scope.DbContext.SaveChangesAsync();

        Assert.Equal(ChallengeConsumeResult.Consumed, await repository.TryConsumeChallengeAsync(identity.Id, "password-reset", "hashed-challenge", now.AddMinutes(1)));
        Assert.Equal(ChallengeConsumeResult.InvalidOrExpired, await repository.TryConsumeChallengeAsync(identity.Id, "password-reset", "hashed-challenge", now.AddMinutes(2)));
        Assert.Equal(ChallengeConsumeResult.InvalidOrExpired, await repository.TryConsumeChallengeAsync(identity.Id, "password-reset", "missing", now.AddMinutes(2)));

        await repository.AddChallengeAsync(new AuthChallenge
        {
            AuthenticationIdentityId = identity.Id,
            AuthChallengePurposeId = purposeId.Value,
            TokenHash = "hashed-challenge-2",
            IssuedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(30),
        });
        await scope.DbContext.SaveChangesAsync();
        Assert.True(await repository.ConsumeChallengeAndUpdatePasswordAsync(
            identity.Id, 1, "password-reset", "hashed-challenge-2", "replacement-hash", now.AddMinutes(3), "password_reset"));
        Assert.Equal(ChallengeConsumeResult.InvalidOrExpired, await repository.TryConsumeChallengeAsync(identity.Id, "password-reset", "hashed-challenge-2", now.AddMinutes(4)));
    }

    [Fact]
    public async Task Deactivated_purpose_cannot_be_consumed_and_failed_password_update_rolls_back_consumption()
    {
        using var scope = new SqliteAppDbContextScope();
        var now = new DateTime(2026, 9, 15, 7, 0, 0, DateTimeKind.Utc);
        var identity = new AuthenticationIdentity { UserId = 1, NormalizedEmail = "challenge-race@example.com", PasswordHash = "old-hash" };
        var session = new RefreshSession
        {
            UserId = 1,
            FamilyId = Guid.NewGuid(),
            TokenHash = "rollback-session-hash",
            ExpiresAtUtc = now.AddDays(1),
        };
        scope.DbContext.AddRange(identity, session);
        await scope.DbContext.SaveChangesAsync();
        var repository = new AuthRepository(scope.UnitOfWork);
        var purposeId = (await repository.FindActiveChallengePurposeIdAsync("password-reset"))!.Value;
        await repository.AddChallengeAsync(new AuthChallenge
        {
            AuthenticationIdentityId = identity.Id,
            AuthChallengePurposeId = purposeId,
            TokenHash = "deactivation-race-hash",
            IssuedAtUtc = now,
            ExpiresAtUtc = now.AddMinutes(30),
        });
        await scope.DbContext.SaveChangesAsync();

        await scope.DbContext.Set<AuthChallengePurpose>()
            .Where(x => x.Id == purposeId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ObsoleteFlag, true));
        Assert.Equal(ChallengeConsumeResult.InvalidOrExpired,
            await repository.TryConsumeChallengeAsync(identity.Id, "password-reset", "deactivation-race-hash", now.AddMinutes(1)));

        await scope.DbContext.Set<AuthChallengePurpose>()
            .Where(x => x.Id == purposeId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ObsoleteFlag, false));
        Assert.False(await repository.ConsumeChallengeAndUpdatePasswordAsync(
            identity.Id, 999, "password-reset", "deactivation-race-hash", "replacement-hash", now.AddMinutes(2), "password_reset"));
        scope.DbContext.ChangeTracker.Clear();
        var challenge = await scope.DbContext.Set<AuthChallenge>().SingleAsync(x => x.TokenHash == "deactivation-race-hash");
        Assert.Null(challenge.ConsumedAtUtc);
        Assert.Equal("old-hash", (await scope.DbContext.Set<AuthenticationIdentity>().SingleAsync(x => x.Id == identity.Id)).PasswordHash);
        Assert.Empty(await scope.DbContext.Set<PasswordHistoryEntry>().Where(x => x.AuthenticationIdentityId == identity.Id).ToListAsync());
        var savedSession = await scope.DbContext.Set<RefreshSession>().SingleAsync(x => x.Id == session.Id);
        Assert.Null(savedSession.RevokedAtUtc);
        Assert.Null(savedSession.RevocationReason);

        Assert.Equal(
            ChallengeConsumeResult.Consumed,
            await repository.TryConsumeChallengeAsync(
                identity.Id,
                "password-reset",
                "deactivation-race-hash",
                now.AddMinutes(3)));
    }

    // Evidence boundary: this test deliberately orders the decisive operations while using
    // independent contexts over the shared-cache in-memory Microsoft.Data.Sqlite fixture
    // (SqliteAppDbContextScope, Default Timeout=30). It proves the active-purpose conditional
    // consumption invariant and its result mapping for the two supported orders: consume-first
    // commits consumption before purpose deactivation, while deactivate-first commits
    // invalidation before consumption; the committed rows are then read from the original
    // context. It does not prove SQL Server lock acquisition, isolation, deadlock or lock-wait
    // behavior, snapshot/RCSI or row-version behavior, or arbitrary scheduling of concurrent
    // production requests. SQL Server acceptance requires a separately authorized SQL Server
    // fixture/run with independent contexts and provider-observed commit/lock evidence.
    [Fact]
    public async Task Challenge_purpose_ordering_records_committed_winner_with_independent_contexts()
    {
        var consumeFirst = await RunOrderedChallengeCaseAsync(consumeFirst: true);
        Assert.Equal(["consume-committed", "deactivation-committed"], consumeFirst.CommitOrder);
        Assert.Equal(ChallengeConsumeResult.Consumed, consumeFirst.ConsumeResult);
        Assert.Equal(1, consumeFirst.DeactivationCount);
        Assert.True(consumeFirst.PurposeObsolete);
        Assert.NotNull(consumeFirst.ConsumedAtUtc);

        var deactivateFirst = await RunOrderedChallengeCaseAsync(consumeFirst: false);
        Assert.Equal(["deactivation-committed", "consume-committed"], deactivateFirst.CommitOrder);
        Assert.Equal(ChallengeConsumeResult.InvalidOrExpired, deactivateFirst.ConsumeResult);
        Assert.Equal(1, deactivateFirst.DeactivationCount);
        Assert.True(deactivateFirst.PurposeObsolete);
        Assert.Null(deactivateFirst.ConsumedAtUtc);

        async Task<ChallengeOrderingEvidence> RunOrderedChallengeCaseAsync(bool consumeFirst)
        {
            using var scope = new SqliteAppDbContextScope();
            var identity = new AuthenticationIdentity
            {
                UserId = 1,
                NormalizedEmail = $"ordered-{Guid.NewGuid():N}@example.com",
                PasswordHash = "old-hash",
            };
            scope.DbContext.Add(identity);
            await scope.DbContext.SaveChangesAsync();

            var seedRepository = new AuthRepository(scope.UnitOfWork);
            var purposeId = (await seedRepository.FindActiveChallengePurposeIdAsync("password-reset"))!.Value;
            var now = new DateTime(2026, 9, 15, 7, 0, 0, DateTimeKind.Utc);
            await seedRepository.AddChallengeAsync(new AuthChallenge
            {
                AuthenticationIdentityId = identity.Id,
                AuthChallengePurposeId = purposeId,
                TokenHash = "ordered-purpose-hash",
                IssuedAtUtc = now,
                ExpiresAtUtc = now.AddMinutes(30),
            });
            await scope.DbContext.SaveChangesAsync();

            using var consumeScope = new SqliteAppDbContextScope(scope.ConnectionString);
            using var deactivateScope = new SqliteAppDbContextScope(scope.ConnectionString);
            var consumeRepository = new AuthRepository(consumeScope.UnitOfWork);
            var commitOrder = new List<string>(2);
            var consumeReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var deactivateReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var consumeGo = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var deactivateGo = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var consumeTask = Task.Run(async () =>
            {
                consumeReady.SetResult();
                await consumeGo.Task.WaitAsync(TimeSpan.FromSeconds(5));
                var result = await consumeRepository.TryConsumeChallengeAsync(
                    identity.Id, "password-reset", "ordered-purpose-hash", now.AddMinutes(1));
                commitOrder.Add("consume-committed");
                return result;
            });
            var deactivateTask = Task.Run(async () =>
            {
                deactivateReady.SetResult();
                await deactivateGo.Task.WaitAsync(TimeSpan.FromSeconds(5));
                var count = await deactivateScope.DbContext.Set<AuthChallengePurpose>()
                    .Where(x => x.Id == purposeId)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ObsoleteFlag, true));
                commitOrder.Add("deactivation-committed");
                return count;
            });

            try
            {
                await Task.WhenAll(consumeReady.Task, deactivateReady.Task).WaitAsync(TimeSpan.FromSeconds(5));
                if (consumeFirst)
                {
                    consumeGo.SetResult();
                    var consumeResult = await consumeTask.WaitAsync(TimeSpan.FromSeconds(5));
                    deactivateGo.SetResult();
                    var deactivationCount = await deactivateTask.WaitAsync(TimeSpan.FromSeconds(5));
                    return await ReadEvidenceAsync(consumeResult, deactivationCount);
                }

                deactivateGo.SetResult();
                var firstDeactivationCount = await deactivateTask.WaitAsync(TimeSpan.FromSeconds(5));
                consumeGo.SetResult();
                var firstConsumeResult = await consumeTask.WaitAsync(TimeSpan.FromSeconds(5));
                return await ReadEvidenceAsync(firstConsumeResult, firstDeactivationCount);
            }
            finally
            {
                consumeGo.TrySetResult();
                deactivateGo.TrySetResult();
                await Task.WhenAll(consumeTask, deactivateTask).WaitAsync(TimeSpan.FromSeconds(5));
            }

            async Task<ChallengeOrderingEvidence> ReadEvidenceAsync(
                ChallengeConsumeResult consumeResult,
                int deactivationCount)
            {
                scope.DbContext.ChangeTracker.Clear();
                var savedPurpose = await scope.DbContext.Set<AuthChallengePurpose>().SingleAsync(x => x.Id == purposeId);
                var savedChallenge = await scope.DbContext.Set<AuthChallenge>()
                    .SingleAsync(x => x.TokenHash == "ordered-purpose-hash");
                return new ChallengeOrderingEvidence(
                    consumeResult,
                    deactivationCount,
                    commitOrder.ToArray(),
                    savedPurpose.ObsoleteFlag,
                    savedChallenge.ConsumedAtUtc);
            }
        }
    }

    private sealed record ChallengeOrderingEvidence(
        ChallengeConsumeResult ConsumeResult,
        int DeactivationCount,
        string[] CommitOrder,
        bool PurposeObsolete,
        DateTime? ConsumedAtUtc);

    [Fact]
    public async Task Password_update_appends_history_clears_lockout_and_revokes_active_sessions()
    {
        using var scope = new SqliteAppDbContextScope();
        var identity = new AuthenticationIdentity { UserId = 1, NormalizedEmail = "change@example.com", PasswordHash = "old-protected-hash", FailedLoginAttempts = 5, LockedUntilUtc = TestNowUtc.AddMinutes(10) };
        var family = Guid.NewGuid();
        var session = new RefreshSession { UserId = 1, FamilyId = family, TokenHash = "session-hash", ExpiresAtUtc = TestNowUtc.AddDays(1) };
        scope.DbContext.AddRange(identity, session);
        await scope.DbContext.SaveChangesAsync();
        scope.DbContext.ChangeTracker.Clear();

        var repository = new AuthRepository(scope.UnitOfWork);
        var changedAt = new DateTime(2026, 9, 15, 7, 0, 0, DateTimeKind.Utc);
        await repository.UpdatePasswordAndRevokeSessionsAsync(identity.Id, 1, "new-protected-hash", changedAt, "password_changed");
        scope.DbContext.ChangeTracker.Clear();

        var savedIdentity = await scope.DbContext.Set<AuthenticationIdentity>().SingleAsync(x => x.Id == identity.Id);
        var savedSession = await scope.DbContext.Set<RefreshSession>().SingleAsync(x => x.Id == session.Id);
        var history = await scope.DbContext.Set<PasswordHistoryEntry>().SingleAsync();
        Assert.Equal("new-protected-hash", savedIdentity.PasswordHash);
        Assert.Equal(0, savedIdentity.FailedLoginAttempts);
        Assert.Null(savedIdentity.LockedUntilUtc);
        Assert.Equal("old-protected-hash", history.PasswordHash);
        Assert.Equal(changedAt, history.CreatedAtUtc);
        Assert.Equal(changedAt, savedSession.RevokedAtUtc);
        Assert.Equal("password_changed", savedSession.RevocationReason);
    }

    [Fact]
    public async Task Refresh_session_creation_and_rotation_preserve_explicit_creation_timestamp()
    {
        using var scope = new SqliteAppDbContextScope();
        var createdAt = new DateTime(2026, 9, 15, 7, 0, 0, DateTimeKind.Utc);
        var session = new RefreshSession
        {
            UserId = 1,
            FamilyId = Guid.NewGuid(),
            TokenHash = "timestamp-session",
            CreatedAtUtc = createdAt,
            ExpiresAtUtc = createdAt.AddDays(30),
        };
        await new AuthRepository(scope.UnitOfWork).AddSessionAsync(session);
        await new AuthRepository(scope.UnitOfWork).SaveAsync();
        Assert.Equal(createdAt, (await scope.DbContext.Set<RefreshSession>().SingleAsync(x => x.TokenHash == "timestamp-session")).CreatedAtUtc);
    }

    [Fact]
    public async Task Independent_providers_prove_persisted_invalidation_retirement_and_one_winner_rotation()
    {
        var keyRing = Path.Combine(Path.GetTempPath(), $"ResumeEnhancerAuthDurable-{Guid.NewGuid():N}");
        Directory.CreateDirectory(keyRing);
        try
        {
            var options = new AuthSecurityOptions
            {
                DataProtectionKeyRingPath = keyRing,
                PreviousKeyOverlap = TimeSpan.FromHours(48),
                KeyRotationPeriod = TimeSpan.FromDays(180),
            };
            var dataProtectionOne = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(
                new DirectoryInfo(keyRing),
                builder => builder.SetApplicationName("ResumeEnhancer.Auth.Tests"));
            var dataProtectionTwo = Microsoft.AspNetCore.DataProtection.DataProtectionProvider.Create(
                new DirectoryInfo(keyRing),
                builder => builder.SetApplicationName("ResumeEnhancer.Auth.Tests"));
            var materialStoreOne = new AuthProtectedKeyMaterialStore(dataProtectionOne, options);
            var materialStoreTwo = new AuthProtectedKeyMaterialStore(dataProtectionTwo, options);
            using var activeRsa = RSA.Create(2048);
            using var previousRsa = RSA.Create(2048);
            var now = new DateTime(2026, 9, 18, 12, 0, 0, DateTimeKind.Utc);

            using var firstScope = new SqliteAppDbContextScope();
            firstScope.DbContext.AddRange(
                new AuthSigningKeyMetadata
                {
                    KeyIdentifier = "primary",
                    ProtectedMaterial = materialStoreOne.Protect(activeRsa),
                    IsActive = true,
                    LifecycleVersion = 1,
                    ActivatedAtUtc = now.AddDays(-181),
                },
                new AuthSigningKeyMetadata
                {
                    KeyIdentifier = "previous",
                    ProtectedMaterial = materialStoreOne.Protect(previousRsa),
                    RetiredAtUtc = now.AddHours(-1),
                    ActivatedAtUtc = now.AddDays(-182),
                    LifecycleVersion = 0,
                });
            await firstScope.DbContext.SaveChangesAsync();

            // Two independent repositories, DbContexts, Data Protection providers, and signing-key providers.
            // SQLite proves committed relational state and conditional-update behavior here; it does not prove
            // SQL Server locking/isolation or distributed serialization semantics.
            using var secondScope = new SqliteAppDbContextScope(firstScope.ConnectionString);
            var firstRepository = new AuthRepository(firstScope.UnitOfWork);
            var secondRepository = new AuthRepository(secondScope.UnitOfWork);
            var firstProvider = new AuthSigningKeyProvider(firstRepository, materialStoreOne, options);
            var secondProvider = new AuthSigningKeyProvider(secondRepository, materialStoreTwo, options);

            using (var beforeRetirement = await firstProvider.GetValidationKeySetAsync(now))
            {
                Assert.NotNull(beforeRetirement);
                Assert.Contains(beforeRetirement!.ValidationKeys, x => x.KeyIdentifier == "previous");
            }

            using var replacementRsa = RSA.Create(2048);
            var replacement = new AuthSigningKeyMetadata
            {
                KeyIdentifier = "replacement",
                ProtectedMaterial = materialStoreOne.Protect(replacementRsa),
                ActivatedAtUtc = now,
            };
            Assert.True(await firstRepository.TryRotateSigningKeyAsync("primary", 1, replacement, now));

            using (var duringOverlap = await secondProvider.GetValidationKeySetAsync(now.AddMinutes(1)))
            {
                Assert.NotNull(duringOverlap);
                Assert.Contains(duringOverlap!.ValidationKeys, x => x.KeyIdentifier == "primary");
            }

            await secondScope.DbContext.Set<AuthSigningKeyMetadata>()
                .Where(x => x.KeyIdentifier == "primary")
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.RetiredAtUtc, now.AddHours(-49)));
            using (var afterRetirement = await secondProvider.GetValidationKeySetAsync(now.AddMinutes(2)))
            {
                Assert.NotNull(afterRetirement);
                Assert.DoesNotContain(afterRetirement!.ValidationKeys, x => x.KeyIdentifier == "primary");
            }

            using var conflictRsaOne = RSA.Create(2048);
            using var conflictRsaTwo = RSA.Create(2048);
            var conflictOne = new AuthSigningKeyMetadata
            {
                KeyIdentifier = "conflict-one",
                ProtectedMaterial = materialStoreOne.Protect(conflictRsaOne),
                ActivatedAtUtc = now.AddMinutes(4),
            };
            var conflictTwo = new AuthSigningKeyMetadata
            {
                KeyIdentifier = "conflict-two",
                ProtectedMaterial = materialStoreTwo.Protect(conflictRsaTwo),
                ActivatedAtUtc = now.AddMinutes(4),
            };
            Assert.True(await firstRepository.TryRotateSigningKeyAsync("replacement", 2, conflictOne, now.AddMinutes(4)));
            Assert.False(await secondRepository.TryRotateSigningKeyAsync("replacement", 2, conflictTwo, now.AddMinutes(4)));

            var committed = await firstRepository.GetSigningKeyMetadataAsync();
            Assert.Single(committed.Where(x => x.IsActive && x.RetiredAtUtc is null && x.InvalidatedAtUtc is null));
            Assert.Contains(committed, x => x.KeyIdentifier == "conflict-one" && x.IsActive);
            Assert.DoesNotContain(committed, x => x.KeyIdentifier == "conflict-two" && x.IsActive);

            var service = new AuthTokenService(options, firstProvider);
            var issued = await service.CreateAccessTokenAsync(42, Guid.NewGuid());
            await secondProvider.InvalidateAsync("conflict-one", now.AddMinutes(5));
            Assert.Null(await service.ValidateAccessTokenAsync(issued.AccessToken));
        }
        finally
        {
            Directory.Delete(keyRing, recursive: true);
        }
    }
}
