using Mediator;
using ResumeEnhancer.TemplateModule.AM.Requests;
using ResumeEnhancer.TemplateModule.AM.Responses;

namespace ResumeEnhancer.TemplateModule.SL.Contracts;

public sealed record CreateTemplateCategoryCommand(CreateTemplateCategoryRequest Request, int? AuditUserId) : ICommand<TemplateCategoryDetailResponse>;
public sealed record UpdateTemplateCategoryCommand(int TemplateCategoryId, UpdateTemplateCategoryRequest Request, int? AuditUserId) : ICommand<TemplateCategoryDetailResponse?>;
public sealed record DeleteTemplateCategoryCommand(int TemplateCategoryId, int? AuditUserId) : ICommand<bool>;
public sealed record GetTemplateCategoryQuery(int TemplateCategoryId) : IQuery<TemplateCategoryDetailResponse?>;
public sealed record ListTemplateCategoriesQuery() : IQuery<IReadOnlyList<TemplateCategoryListItemResponse>>;

public sealed record CreateTemplateCommand(CreateTemplateRequest Request, int? AuditUserId) : ICommand<TemplateDetailResponse?>;
public sealed record UpdateTemplateCommand(int TemplateId, UpdateTemplateRequest Request, int? AuditUserId) : ICommand<TemplateDetailResponse?>;
public sealed record DeleteTemplateCommand(int TemplateId, int? AuditUserId) : ICommand<bool>;
public sealed record GetTemplateQuery(int TemplateId) : IQuery<TemplateDetailResponse?>;
public sealed record ListTemplatesQuery() : IQuery<IReadOnlyList<TemplateListItemResponse>>;
