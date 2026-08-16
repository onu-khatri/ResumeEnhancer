using Mediator;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.AM.Responses;

namespace ResumeEnhancer.BillingModule.SL.Contracts;

public sealed record CreateBillingSubscriptionCommand(CreateBillingSubscriptionRequest Request, int? AuditUserId) : ICommand<BillingSubscriptionDetailResponse?>;
public sealed record UpdateBillingSubscriptionCommand(int BillingSubscriptionId, UpdateBillingSubscriptionRequest Request, int? AuditUserId) : ICommand<BillingSubscriptionDetailResponse?>;
public sealed record DeleteBillingSubscriptionCommand(int BillingSubscriptionId, int? AuditUserId) : ICommand<bool>;
public sealed record GetBillingSubscriptionQuery(int BillingSubscriptionId) : IQuery<BillingSubscriptionDetailResponse?>;
public sealed record ListBillingSubscriptionsQuery() : IQuery<IReadOnlyList<BillingSubscriptionListItemResponse>>;
