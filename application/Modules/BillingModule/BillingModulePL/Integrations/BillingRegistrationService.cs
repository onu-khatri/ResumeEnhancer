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
        CancellationToken cancellationToken = default
    )
    {
        var plan = await unitOfWork
            .GetRepo<BillingPlan>()
            .Query()
            .SingleOrDefaultAsync(x => x.Code == "FREE", cancellationToken);
        if (plan is null || plan.AccessProfileId <= 0)
            return null;
        var accountStatusId = await unitOfWork
            .GetRepo<BillingAccountStatus>()
            .Query()
            .Where(x => x.Code == "Active")
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);
        var subscriptionStatusId = await unitOfWork
            .GetRepo<BillingSubscriptionStatus>()
            .Query()
            .Where(x => x.Code == "Active")
            .Select(x => (int?)x.Id)
            .SingleOrDefaultAsync(cancellationToken);
        if (accountStatusId is null || subscriptionStatusId is null)
            return null;
        var account = new BillingAccount
        {
            UserId = userId,
            AccountNumber = $"USR-{Guid.NewGuid():N}"[..20],
            StatusId = accountStatusId.Value,
        };
        await unitOfWork.GetRepo<BillingAccount>().AddAsync(account, cancellationToken);
        var subscription = new BillingSubscription
        {
            BillingAccount = account,
            UserId = userId,
            BillingPlanId = plan.Id,
            StatusId = subscriptionStatusId.Value,
        };
        await unitOfWork.GetRepo<BillingSubscription>().AddAsync(subscription, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        return new BillingRegistrationSnapshot(
            subscription.Id,
            plan.Id,
            plan.AccessProfileId,
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
                && x.Status!.Code == "Active"
                && (x.EndDateUtc == null || x.EndDateUtc > DateTime.UtcNow)
            )
            .Select(x => new BillingSubscriptionSnapshot(
                x.Id,
                x.BillingPlanId,
                x.BillingPlan!.AccessProfileId,
                x.BillingPlan!.Code
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
