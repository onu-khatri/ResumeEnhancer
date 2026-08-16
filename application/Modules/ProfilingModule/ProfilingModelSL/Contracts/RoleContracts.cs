using Mediator;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.AM.Responses;

namespace ResumeEnhancer.ProfilingModule.SL.Contracts;

public sealed record CreateRoleCommand(CreateRoleRequest Request, int? AuditUserId) : ICommand<RoleDetailResponse>;
public sealed record UpdateRoleCommand(int RoleId, UpdateRoleRequest Request, int? AuditUserId) : ICommand<RoleDetailResponse?>;
public sealed record DeleteRoleCommand(int RoleId, int? AuditUserId) : ICommand<bool>;
public sealed record GetRoleQuery(int RoleId) : IQuery<RoleDetailResponse?>;
public sealed record ListRolesQuery() : IQuery<IReadOnlyList<RoleListItemResponse>>;
