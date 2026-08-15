using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace ResumeEnhancer.Infrastructure.Persistence;

public static class SetupDataSeedingExtensions
{
    public static async Task SeedSetupDataAsync<TEntity>(
        this DbSet<TEntity> setupDataSet,
        IEnumerable<TEntity> seeds,
        Func<TEntity, TEntity, bool> updateFunction,
        CancellationToken cancellationToken = default)
        where TEntity : class, ISetupData
    {
        ArgumentNullException.ThrowIfNull(setupDataSet);
        ArgumentNullException.ThrowIfNull(seeds);
        ArgumentNullException.ThrowIfNull(updateFunction);

        var seedList = seeds.ToArray();
        ValidateSeeds(seedList);

        var existingRows = await setupDataSet.ToListAsync(cancellationToken);
        var incomingGuids = seedList.Select(seed => seed.Guid!.Value).ToHashSet();

        foreach (var seed in seedList)
        {
            var existingByGuid = existingRows.SingleOrDefault(existing => existing.Guid == seed.Guid);
            var existingByCode = existingRows.SingleOrDefault(
                existing => string.Equals(existing.Code, seed.Code, StringComparison.OrdinalIgnoreCase));

            if (existingByGuid is not null
                && existingByCode is not null
                && !ReferenceEquals(existingByGuid, existingByCode))
            {
                throw new InvalidOperationException(
                    $"Seed '{typeof(TEntity).Name}:{seed.Code}' matches different existing rows by Guid and Code.");
            }

            var existingRow = existingByGuid ?? existingByCode;

            if (existingRow is null)
            {
                StampCreate(seed);
                setupDataSet.Add(seed);
                continue;
            }

            var hasChanges = ApplySetupData(existingRow, seed);
            hasChanges |= updateFunction(existingRow, seed);

            if (hasChanges)
            {
                StampUpdate(existingRow);
            }
        }

        foreach (var existingRow in existingRows)
        {
            if (existingRow.Guid is null
                || incomingGuids.Contains(existingRow.Guid.Value)
                || existingRow.ObsoleteFlag
                || !WasManagedBySeeder(existingRow))
            {
                continue;
            }

            existingRow.ObsoleteFlag = true;
            StampUpdate(existingRow);
        }
    }

    private static void ValidateSeeds<TEntity>(IEnumerable<TEntity> seeds)
        where TEntity : ISetupData
    {
        var seedList = seeds.ToArray();

        var missingIdentitySeed = seedList.FirstOrDefault(
            seed => seed.Guid is null || string.IsNullOrWhiteSpace(seed.Code));

        if (missingIdentitySeed is not null)
        {
            throw new InvalidOperationException(
                $"{typeof(TEntity).Name} setup seeds must define both Code and Guid.");
        }

        var duplicateGuid = seedList
            .GroupBy(seed => seed.Guid!.Value)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateGuid is not null)
        {
            throw new InvalidOperationException(
                $"{typeof(TEntity).Name} setup seeds contain duplicate Guid '{duplicateGuid.Key}'.");
        }

        var duplicateCode = seedList
            .GroupBy(seed => seed.Code, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateCode is not null)
        {
            throw new InvalidOperationException(
                $"{typeof(TEntity).Name} setup seeds contain duplicate Code '{duplicateCode.Key}'.");
        }
    }

    private static bool ApplySetupData(ISetupData existingRow, ISetupData seed)
    {
        var hasChanges = false;

        if (!string.Equals(existingRow.Code, seed.Code, StringComparison.Ordinal))
        {
            existingRow.Code = seed.Code;
            hasChanges = true;
        }

        if (!string.Equals(existingRow.Description, seed.Description, StringComparison.Ordinal))
        {
            existingRow.Description = seed.Description;
            hasChanges = true;
        }

        if (existingRow.Guid != seed.Guid)
        {
            existingRow.Guid = seed.Guid;
            hasChanges = true;
        }

        if (existingRow.ObsoleteFlag != seed.ObsoleteFlag)
        {
            existingRow.ObsoleteFlag = seed.ObsoleteFlag;
            hasChanges = true;
        }

        return hasChanges;
    }

    private static void StampCreate(ISetupData setupData)
    {
        var utcNow = DateTime.UtcNow;

        setupData.App_CreateUserId ??= SeedingUser.UserId;
        setupData.App_UpdateUserId = SeedingUser.UserId;
        setupData.App_CreateDate = setupData.App_CreateDate == default
            ? utcNow
            : setupData.App_CreateDate;
        setupData.App_UpdateDate = utcNow;
    }

    private static void StampUpdate(ISetupData setupData)
    {
        setupData.App_UpdateUserId = SeedingUser.UserId;
        setupData.App_UpdateDate = DateTime.UtcNow;
    }

    private static bool WasManagedBySeeder(ISetupData setupData) =>
        setupData.App_UpdateUserId is null or SeedingUser.UserId;
}

