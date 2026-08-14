using AuthModuleAM.RequestsDTO;
using FluentValidation;
namespace AuthModuleWeb.Validation;


public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
{
    public RegisterDTOValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required").MinimumLength(3).WithMessage("First Name must be at least 3 characters");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is not valid");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required").Length(8, 25).WithMessage("Password must be between 8 and 25 characters").Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,25}$").WithMessage("Password must contain at least one letter, one number, and one special character");
        RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password is required").Equal(x => x.Password).WithMessage("Passwords do not match");
    }
}
