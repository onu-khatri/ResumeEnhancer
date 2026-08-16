using FluentValidation;
using ResumeEnhancer.TemplateModule.AM.Requests;

namespace ResumeEnhancer.TemplateModule.Web.Validation;

public sealed class CreateTemplateCategoryRequestValidator : AbstractValidator<CreateTemplateCategoryRequest>
{
    public CreateTemplateCategoryRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateTemplateCategoryRequestValidator : AbstractValidator<UpdateTemplateCategoryRequest>
{
    public UpdateTemplateCategoryRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
    }
}

public sealed class CreateTemplateRequestValidator : AbstractValidator<CreateTemplateRequest>
{
    public CreateTemplateRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.TemplateCategoryId).GreaterThan(0);
        RuleFor(request => request.Body).NotEmpty().MaximumLength(20000);
        RuleFor(request => request.PreviewImageUrl).MaximumLength(500);
        RuleFor(request => request.RenderTypeCode).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateTemplateRequestValidator : AbstractValidator<UpdateTemplateRequest>
{
    public UpdateTemplateRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
        RuleFor(request => request.TemplateCategoryId).GreaterThan(0);
        RuleFor(request => request.Body).NotEmpty().MaximumLength(20000);
        RuleFor(request => request.PreviewImageUrl).MaximumLength(500);
        RuleFor(request => request.RenderTypeCode).NotEmpty().MaximumLength(100);
    }
}
