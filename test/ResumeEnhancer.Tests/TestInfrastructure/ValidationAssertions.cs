using FluentValidation.Results;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.TestInfrastructure;

internal static class ValidationAssertions
{
    public static void ShouldHaveErrorFor(
        this ValidationResult validationResult,
        string propertyName,
        string? messageFragment = null)
    {
        var errors = validationResult.Errors
            .Where(error => error.PropertyName == propertyName)
            .ToArray();

        errors.ShouldNotBeEmpty();

        if (messageFragment is not null)
        {
            errors.ShouldContain(error => error.ErrorMessage.Contains(messageFragment, StringComparison.Ordinal));
        }
    }

    public static void ShouldNotHaveErrorFor(
        this ValidationResult validationResult,
        string propertyName)
    {
        validationResult.Errors
            .Where(error => error.PropertyName == propertyName)
            .ShouldBeEmpty();
    }
}

