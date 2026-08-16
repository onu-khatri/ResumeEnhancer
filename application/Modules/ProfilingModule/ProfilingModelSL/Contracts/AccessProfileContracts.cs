using Mediator;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.AM.Responses;

namespace ResumeEnhancer.ProfilingModule.SL.Contracts;

public sealed record CreateAccessProfileCommand(CreateAccessProfileRequest Request, int? AuditUserId) : ICommand<AccessProfileDetailResponse>;
public sealed record UpdateAccessProfileCommand(int AccessProfileId, UpdateAccessProfileRequest Request, int? AuditUserId) : ICommand<AccessProfileDetailResponse?>;
public sealed record DeleteAccessProfileCommand(int AccessProfileId, int? AuditUserId) : ICommand<bool>;
public sealed record GetAccessProfileQuery(int AccessProfileId) : IQuery<AccessProfileDetailResponse?>;
public sealed record ListAccessProfilesQuery() : IQuery<IReadOnlyList<AccessProfileListItemResponse>>;
