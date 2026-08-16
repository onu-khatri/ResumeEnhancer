using Mediator;
using ResumeEnhancer.BillingModule.AM.Requests;
using ResumeEnhancer.BillingModule.AM.Responses;

namespace ResumeEnhancer.BillingModule.SL.Contracts;

public sealed record CreateBillingAccountCommand(CreateBillingAccountRequest Request, int? AuditUserId) : ICommand<BillingAccountDetailResponse>;
public sealed record UpdateBillingAccountCommand(int BillingAccountId, UpdateBillingAccountRequest Request, int? AuditUserId) : ICommand<BillingAccountDetailResponse?>;
public sealed record DeleteBillingAccountCommand(int BillingAccountId, int? AuditUserId) : ICommand<bool>;
public sealed record GetBillingAccountQuery(int BillingAccountId) : IQuery<BillingAccountDetailResponse?>;
public sealed record ListBillingAccountsQuery() : IQuery<IReadOnlyList<BillingAccountListItemResponse>>;
