using Mediator;
using ResumeEnhancer.ProfilingModule.AM.Requests;
using ResumeEnhancer.ProfilingModule.AM.Responses;

namespace ResumeEnhancer.ProfilingModule.SL.Contracts;

public sealed record CreateUserCommand(CreateUserRequest Request, int? AuditUserId) : ICommand<UserDetailResponse>;
public sealed record UpdateUserCommand(int UserId, UpdateUserRequest Request, int? AuditUserId) : ICommand<UserDetailResponse?>;
public sealed record DeleteUserCommand(int UserId, int? AuditUserId) : ICommand<bool>;
public sealed record GetUserQuery(int UserId) : IQuery<UserDetailResponse?>;
public sealed record ListUsersQuery() : IQuery<IReadOnlyList<UserListItemResponse>>;
