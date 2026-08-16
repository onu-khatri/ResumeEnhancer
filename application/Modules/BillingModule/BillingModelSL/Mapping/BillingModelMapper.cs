using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.AM.Responses;
using ResumeEnhancer.BillingModule.DM.Entities;

namespace ResumeEnhancer.BillingModule.SL.Mapping;

internal static class BillingModelMapper
{
    public static BillingAccount CreateBillingAccount(CreateBillingAccountRequest request) => new()
    {
        UserId = request.UserId,
        AccountNumber = request.AccountNumber.Trim(),
        Status = request.Status.Trim(),
        ExternalReference = TrimOrNull(request.ExternalReference)
    };

    public static void Apply(UpdateBillingAccountRequest request, BillingAccount entity)
    {
        entity.UserId = request.UserId;
        entity.AccountNumber = request.AccountNumber.Trim();
        entity.Status = request.Status.Trim();
        entity.ExternalReference = TrimOrNull(request.ExternalReference);
    }

    public static BillingPlan CreateBillingPlan(CreateBillingPlanRequest request) => new()
    {
        Code = request.Code.Trim(),
        Description = request.Description.Trim(),
        DisplayName = request.DisplayName.Trim(),
        Price = request.Price,
        Currency = request.Currency.Trim(),
        BillingInterval = request.BillingInterval.Trim(),
        IsDeactivated = request.IsDeactivated
    };

    public static void Apply(UpdateBillingPlanRequest request, BillingPlan entity)
    {
        entity.Code = request.Code.Trim();
        entity.Description = request.Description.Trim();
        entity.DisplayName = request.DisplayName.Trim();
        entity.Price = request.Price;
        entity.Currency = request.Currency.Trim();
        entity.BillingInterval = request.BillingInterval.Trim();
        entity.IsDeactivated = request.IsDeactivated;
        entity.ObsoleteFlag = request.ObsoleteFlag;
    }

    public static BillingSubscription CreateBillingSubscription(CreateBillingSubscriptionRequest request) => new()
    {
        BillingAccountId = request.BillingAccountId,
        BillingPlanId = request.BillingPlanId,
        ResumeId = request.ResumeId,
        Status = request.Status.Trim(),
        StartDateUtc = request.StartDateUtc,
        EndDateUtc = request.EndDateUtc
    };

    public static void Apply(UpdateBillingSubscriptionRequest request, BillingSubscription entity)
    {
        entity.BillingAccountId = request.BillingAccountId;
        entity.BillingPlanId = request.BillingPlanId;
        entity.ResumeId = request.ResumeId;
        entity.Status = request.Status.Trim();
        entity.StartDateUtc = request.StartDateUtc;
        entity.EndDateUtc = request.EndDateUtc;
    }

    public static BillingAccountDetailResponse MapBillingAccountDetail(BillingAccount entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        AccountNumber = entity.AccountNumber,
        Status = entity.Status,
        ExternalReference = entity.ExternalReference,
        App_CreateDate = entity.App_CreateDate,
        App_UpdateDate = entity.App_UpdateDate,
        App_Version = entity.App_Version
    };

    public static BillingAccountListItemResponse MapBillingAccountListItem(BillingAccount entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        AccountNumber = entity.AccountNumber,
        Status = entity.Status
    };

    public static BillingPlanDetailResponse MapBillingPlanDetail(BillingPlan entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Description = entity.Description,
        DisplayName = entity.DisplayName,
        Price = entity.Price,
        Currency = entity.Currency,
        BillingInterval = entity.BillingInterval,
        IsDeactivated = entity.IsDeactivated,
        ObsoleteFlag = entity.ObsoleteFlag,
        App_CreateDate = entity.App_CreateDate,
        App_UpdateDate = entity.App_UpdateDate,
        App_Version = entity.App_Version
    };

    public static BillingPlanListItemResponse MapBillingPlanListItem(BillingPlan entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        DisplayName = entity.DisplayName,
        Price = entity.Price,
        Currency = entity.Currency,
        IsDeactivated = entity.IsDeactivated
    };

    public static BillingSubscriptionDetailResponse MapBillingSubscriptionDetail(BillingSubscription entity) => new()
    {
        Id = entity.Id,
        BillingAccountId = entity.BillingAccountId,
        BillingPlanId = entity.BillingPlanId,
        ResumeId = entity.ResumeId,
        Status = entity.Status,
        StartDateUtc = entity.StartDateUtc,
        EndDateUtc = entity.EndDateUtc,
        App_CreateDate = entity.App_CreateDate,
        App_UpdateDate = entity.App_UpdateDate,
        App_Version = entity.App_Version
    };

    public static BillingSubscriptionListItemResponse MapBillingSubscriptionListItem(BillingSubscription entity) => new()
    {
        Id = entity.Id,
        BillingAccountId = entity.BillingAccountId,
        BillingPlanId = entity.BillingPlanId,
        ResumeId = entity.ResumeId,
        Status = entity.Status
    };

    private static string? TrimOrNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
