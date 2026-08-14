using AuthModuleAM.RequestsDTO;
using FluentValidation;

namespace AuthModuleWeb.Validation;

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    public LoginDTOValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is not valid");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").Length(8, 25).WithMessage("Password must be between 8 and 25 characters").Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,25}$").WithMessage("Password must contain at least one letter, one number, and one special character");
    }
}
