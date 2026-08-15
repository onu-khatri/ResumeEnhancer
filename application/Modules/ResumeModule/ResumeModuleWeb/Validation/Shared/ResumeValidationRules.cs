using FluentValidation;

namespace ResumeEnhancer.ResumeModule.Web.Validation.Shared;

internal static class ResumeValidationRules
{
    public static void RuleForRequestId<TRequest>(
        this AbstractValidator<TRequest> validator,
        System.Linq.Expressions.Expression<Func<TRequest, int>> expression,
        bool isCreate)
    {
        validator.RuleFor(expression)
            .GreaterThanOrEqualTo(0)
            .WithMessage("{PropertyName} must be zero or greater.");

        if (isCreate)
        {
            validator.RuleFor(expression)
                .Equal(0)
                .WithMessage("{PropertyName} must be 0 when creating.");
        }
    }

    public static bool IsValidHttpUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }

    public static bool HasNoDuplicateExistingIds<TRequest>(
        IEnumerable<TRequest>? requests,
        Func<TRequest, int> idSelector) =>
        FindDuplicateExistingId(requests, idSelector) is null;

    public static string DuplicateExistingIdMessage<TRequest>(
        string collectionName,
        IEnumerable<TRequest>? requests,
        Func<TRequest, int> idSelector) =>
        $"{collectionName} contains duplicate item id '{FindDuplicateExistingId(requests, idSelector)}'.";

    private static int? FindDuplicateExistingId<TRequest>(
        IEnumerable<TRequest>? requests,
        Func<TRequest, int> idSelector) =>
        requests?
            .Select(idSelector)
            .Where(id => id > 0)
            .GroupBy(id => id)
            .FirstOrDefault(group => group.Count() > 1)
            ?.Key;
}

