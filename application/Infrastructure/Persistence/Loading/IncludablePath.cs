using System.Linq.Expressions;

namespace Persistence;

public sealed class IncludablePath : IEquatable<IncludablePath>
{
    public IncludablePath(IEnumerable<string> segments)
    {
        ArgumentNullException.ThrowIfNull(segments);

        Segments = segments
            .Where(segment => !string.IsNullOrWhiteSpace(segment))
            .ToArray();

        Path = string.Join(".", Segments);
    }

    public IReadOnlyList<string> Segments { get; }

    public string Path { get; }

    public static IncludablePath FromExpression<TModel, TResult>(
        Expression<Func<TModel, TResult>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return new IncludablePath(MemberPath.FromExpression(expression.Body));
    }

    public static IncludablePath FromSegments(IEnumerable<string> segments) =>
        new(segments);

    public bool Equals(IncludablePath? other) =>
        other is not null
        && string.Equals(Path, other.Path, StringComparison.Ordinal);

    public override bool Equals(object? obj) =>
        Equals(obj as IncludablePath);

    public override int GetHashCode() =>
        StringComparer.Ordinal.GetHashCode(Path);

    public override string ToString() => Path;

    internal static class MemberPath
    {
        public static IReadOnlyList<string> FromExpression(Expression expression)
        {
            var segments = new Stack<string>();
            var currentExpression = RemoveConversion(expression);

            while (currentExpression is MemberExpression memberExpression)
            {
                segments.Push(memberExpression.Member.Name);
                currentExpression = RemoveConversion(memberExpression.Expression!);
            }

            if (segments.Count == 0)
            {
                throw new ArgumentException(
                    "Expression must select a member path.",
                    nameof(expression));
            }

            return segments.ToArray();
        }

        private static Expression RemoveConversion(Expression expression)
        {
            while (expression is UnaryExpression unaryExpression
                   && (unaryExpression.NodeType == ExpressionType.Convert
                       || unaryExpression.NodeType == ExpressionType.ConvertChecked))
            {
                expression = unaryExpression.Operand;
            }

            return expression;
        }
    }
}
