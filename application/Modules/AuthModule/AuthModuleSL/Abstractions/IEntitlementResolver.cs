namespace ResumeEnhancer.AuthModule.SL.Abstractions;

public interface IEntitlementResolver
{
    public Task<IReadOnlySet<string>> ResolveAsync(
        int userId,
        CancellationToken cancellationToken = default
    );
}
