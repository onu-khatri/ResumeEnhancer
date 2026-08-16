using FluentValidation;
using ResumeEnhancer.ProfilingModule.AM.Requests;

namespace ResumeEnhancer.ProfilingModule.Web.Validation;

public sealed class CreateAccessProfileRequestValidator : AbstractValidator<CreateAccessProfileRequest>
{
    public CreateAccessProfileRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
        RuleForEach(request => request.RoleIds).GreaterThan(0);
    }
}

public sealed class UpdateAccessProfileRequestValidator : AbstractValidator<UpdateAccessProfileRequest>
{
    public UpdateAccessProfileRequestValidator()
    {
        RuleFor(request => request.Code).NotEmpty().MaximumLength(100);
        RuleFor(request => request.Description).NotEmpty().MaximumLength(1000);
        RuleFor(request => request.DisplayName).NotEmpty().MaximumLength(200);
        RuleForEach(request => request.RoleIds).GreaterThan(0);
    }
}
