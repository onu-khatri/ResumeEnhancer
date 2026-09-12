using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.AM.Responses;
using ResumeEnhancer.BillingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.SL.Mapping;

internal static class BillingModelMapper
{
    public static BillingAccount CreateBillingAccount(CreateBillingAccountRequest request)
    {
        return new()
        {
            UserId = request.UserId,
            AccountNumber = request.AccountNumber.Trim(),
            StatusId = request.StatusId,
            ExternalReference = TrimOrNull(request.ExternalReference),
        };
    }

    public static void Apply(UpdateBillingAccountRequest request, BillingAccount entity)
    {
        entity.UserId = request.UserId;
        entity.AccountNumber = request.AccountNumber.Trim();
        entity.StatusId = request.StatusId;
        entity.ExternalReference = TrimOrNull(request.ExternalReference);
    }

    public static BillingPlan CreateBillingPlan(CreateBillingPlanRequest request)
    {
        return new()
        {
            Code = request.Code.Trim(),
            Description = request.Description.Trim(),
            DisplayName = request.DisplayName.Trim(),
            Price = request.Price,
            CurrencyId = request.CurrencyId,
            BillingIntervalId = request.BillingIntervalId,
            AccessProfileId = request.AccessProfileId,
            IsDeactivated = request.IsDeactivated,
        };
    }

    public static void Apply(UpdateBillingPlanRequest request, BillingPlan entity)
    {
        entity.Code = request.Code.Trim();
        entity.Description = request.Description.Trim();
        entity.DisplayName = request.DisplayName.Trim();
        entity.Price = request.Price;
        entity.CurrencyId = request.CurrencyId;
        entity.BillingIntervalId = request.BillingIntervalId;
        entity.AccessProfileId = request.AccessProfileId;
        entity.IsDeactivated = request.IsDeactivated;
        entity.ObsoleteFlag = request.ObsoleteFlag;
    }

    public static BillingSubscription CreateBillingSubscription(
        CreateBillingSubscriptionRequest request
    )
    {
        return new()
        {
            BillingAccountId = request.BillingAccountId,
            UserId = request.UserId,
            BillingPlanId = request.BillingPlanId,
            StatusId = request.StatusId,
            StartDateUtc = request.StartDateUtc,
            EndDateUtc = request.EndDateUtc,
        };
    }

    public static void Apply(UpdateBillingSubscriptionRequest request, BillingSubscription entity)
    {
        entity.BillingAccountId = request.BillingAccountId;
        entity.UserId = request.UserId;
        entity.BillingPlanId = request.BillingPlanId;
        entity.StatusId = request.StatusId;
        entity.StartDateUtc = request.StartDateUtc;
        entity.EndDateUtc = request.EndDateUtc;
    }

    public static BillingAccountDetailResponse MapBillingAccountDetail(BillingAccount entity)
    {
        return new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccountNumber = entity.AccountNumber,
            Status = entity.Status?.Code ?? string.Empty,
            ExternalReference = entity.ExternalReference,
            App_CreateDate = entity.App_CreateDate,
            App_UpdateDate = entity.App_UpdateDate,
            App_Version = entity.App_Version,
        };
    }

    public static BillingAccountListItemResponse MapBillingAccountListItem(BillingAccount entity)
    {
        return new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            AccountNumber = entity.AccountNumber,
            Status = entity.Status?.Code ?? string.Empty,
        };
    }

    public static BillingPlanDetailResponse MapBillingPlanDetail(BillingPlan entity)
    {
        return new()
        {
            Id = entity.Id,
            Code = entity.Code,
            Description = entity.Description,
            DisplayName = entity.DisplayName,
            Price = entity.Price,
            Currency = entity.Currency?.Code ?? string.Empty,
            BillingInterval = entity.BillingInterval?.Code ?? string.Empty,
            IsDeactivated = entity.IsDeactivated,
            ObsoleteFlag = entity.ObsoleteFlag,
            App_CreateDate = entity.App_CreateDate,
            App_UpdateDate = entity.App_UpdateDate,
            App_Version = entity.App_Version,
        };
    }

    public static BillingPlanListItemResponse MapBillingPlanListItem(BillingPlan entity)
    {
        return new()
        {
            Id = entity.Id,
            Code = entity.Code,
            DisplayName = entity.DisplayName,
            Price = entity.Price,
            Currency = entity.Currency?.Code ?? string.Empty,
            IsDeactivated = entity.IsDeactivated,
        };
    }

    public static BillingSubscriptionDetailResponse MapBillingSubscriptionDetail(
        BillingSubscription entity
    )
    {
        return new()
        {
            Id = entity.Id,
            BillingAccountId = entity.BillingAccountId,
            UserId = entity.UserId,
            BillingPlanId = entity.BillingPlanId,
            Status = entity.Status?.Code ?? string.Empty,
            StartDateUtc = entity.StartDateUtc,
            EndDateUtc = entity.EndDateUtc,
            App_CreateDate = entity.App_CreateDate,
            App_UpdateDate = entity.App_UpdateDate,
            App_Version = entity.App_Version,
        };
    }

    public static BillingSubscriptionListItemResponse MapBillingSubscriptionListItem(
        BillingSubscription entity
    )
    {
        return new()
        {
            Id = entity.Id,
            BillingAccountId = entity.BillingAccountId,
            UserId = entity.UserId,
            BillingPlanId = entity.BillingPlanId,
            Status = entity.Status?.Code ?? string.Empty,
        };
    }

    private static string? TrimOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
