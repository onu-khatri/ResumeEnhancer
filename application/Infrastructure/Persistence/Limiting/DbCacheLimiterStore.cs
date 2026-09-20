using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.Core.CommonLibrary.Resilience;

namespace ResumeEnhancer.Infrastructure.Persistence.Limiting;

public sealed class DbCacheLimiterStore(
    AppDbContext dbContext,
    IResilienceExecutor resilienceExecutor,
    TimeProvider timeProvider) : IAtomicLimiterStore
{
    public async Task<AtomicLimiterDecision> TryConsumeAsync(
        string keyHash,
        int limit,
        TimeSpan window,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(keyHash);
        if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
        if (window <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(window));

        return await resilienceExecutor.ExecuteAsync(
            "DbCache",
            ResilienceOperation.DatabaseConflict,
            token => ExecuteAttemptAsync(keyHash, limit, window, token),
            IsExpectedConcurrencyConflict,
            cancellationToken);
    }

    private async Task<AtomicLimiterDecision> ExecuteAttemptAsync(
        string keyHash,
        int limit,
        TimeSpan window,
        CancellationToken cancellationToken)
    {
        try
        {
            var now = timeProvider.GetUtcNow().UtcDateTime;
            var expires = now.Add(window);
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, cancellationToken);
            var entry = await dbContext.Set<DbCacheLimiterEntry>()
                .SingleOrDefaultAsync(x => x.KeyHash == keyHash, cancellationToken);

            if (entry is null)
            {
                entry = new DbCacheLimiterEntry { KeyHash = keyHash, Count = 1, WindowStartedUtc = now, ExpiresAtUtc = expires };
                dbContext.Add(entry);
            }
            else if (entry.ExpiresAtUtc <= now)
            {
                entry.Count = 1;
                entry.WindowStartedUtc = now;
                entry.ExpiresAtUtc = expires;
            }
            else if (entry.Count < limit)
            {
                entry.Count++;
            }
            else
            {
                await transaction.CommitAsync(cancellationToken);
                return new AtomicLimiterDecision(false, entry.Count, limit, entry.ExpiresAtUtc);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new AtomicLimiterDecision(true, entry.Count, limit, entry.ExpiresAtUtc);
        }
        catch (Exception exception) when (IsExpectedConcurrencyConflict(exception))
        {
            dbContext.ChangeTracker.Clear();
            throw;
        }
    }

    private static bool IsExpectedConcurrencyConflict(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is DbUpdateConcurrencyException)
                return true;

            var typeName = current.GetType().Name;
            var message = current.Message;
            if (typeName.Contains("SqlException", StringComparison.Ordinal)
                && (message.Contains("2601", StringComparison.Ordinal) || message.Contains("2627", StringComparison.Ordinal)
                    || message.Contains("1205", StringComparison.Ordinal) || message.Contains("3960", StringComparison.Ordinal)))
                return true;
            if (typeName.Contains("SqliteException", StringComparison.Ordinal)
                && (message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase)
                    || message.Contains("database is locked", StringComparison.OrdinalIgnoreCase)))
                return true;
        }

        return false;
    }
}
