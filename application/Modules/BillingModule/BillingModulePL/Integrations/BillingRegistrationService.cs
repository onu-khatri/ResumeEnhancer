using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.BillingModule.DM.Entities;
using ResumeEnhancer.BillingModule.SL.Integrations;
using ResumeEnhancer.Infrastructure.Persistence;

namespace ResumeEnhancer.BillingModule.PL.Integrations;

public sealed class BillingRegistrationService(IUnitOfWork<AppDbContext> unitOfWork)
    : IBillingRegistrationService
{
    public async Task<BillingRegistrationSnapshot?> AddStarterRegistrationBillingAsync(
        int userId,
        int accessProfileId,
        CancellationToken cancellationToken = default
    )
    {
        var plan = await unitOfWork
            .GetRepo<BillingPlan>()
            .Query()
            .SingleOrDefaultAsync(x => x.Code == "FREE", cancellationToken);
        if (plan is null || accessProfileId <= 0)
            return null;
        var account = new BillingAccount
        {
            UserId = userId,
            AccountNumber = $"USR-{Guid.NewGuid():N}"[..20],
        };
        await unitOfWork.GetRepo<BillingAccount>().AddAsync(account, cancellationToken);
        var subscription = new BillingSubscription
        {
            BillingAccount = account,
            UserId = userId,
            BillingPlanId = plan.Id,
            AccessProfileId = accessProfileId,
            Status = "Active",
        };
        await unitOfWork.GetRepo<BillingSubscription>().AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        return new BillingRegistrationSnapshot(
            subscription.Id,
            plan.Id,
            accessProfileId,
            plan.Code
        );
    }

    public async Task<
        IReadOnlyList<BillingSubscriptionSnapshot>
    > ListActiveSubscriptionSnapshotsAsync(
        int userId,
        CancellationToken cancellationToken = default
    )
    {
        return await unitOfWork
            .GetRepo<BillingSubscription>()
            .Query()
            .Where(x =>
                x.BillingAccount!.UserId == userId
                && x.Status == "Active"
                && (x.EndDateUtc == null || x.EndDateUtc > DateTime.UtcNow)
            )
            .Select(x => new BillingSubscriptionSnapshot(
                x.Id,
                x.BillingPlanId,
                x.AccessProfileId,
                x.BillingPlan!.Code
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
