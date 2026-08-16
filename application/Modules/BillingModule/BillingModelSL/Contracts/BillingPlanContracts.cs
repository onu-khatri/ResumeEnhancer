using Mediator;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.AM.Responses;

namespace ResumeEnhancer.BillingModule.SL.Contracts;

public sealed record CreateBillingPlanCommand(CreateBillingPlanRequest Request, int? AuditUserId) : ICommand<BillingPlanDetailResponse>;
public sealed record UpdateBillingPlanCommand(int BillingPlanId, UpdateBillingPlanRequest Request, int? AuditUserId) : ICommand<BillingPlanDetailResponse?>;
public sealed record DeleteBillingPlanCommand(int BillingPlanId, int? AuditUserId) : ICommand<bool>;
public sealed record GetBillingPlanQuery(int BillingPlanId) : IQuery<BillingPlanDetailResponse?>;
public sealed record ListBillingPlansQuery() : IQuery<IReadOnlyList<BillingPlanListItemResponse>>;
