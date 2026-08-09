namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public static class SetupperAccessExtensions
{
    public static async Task<TestAuthenticatedAccess> SetupAccessAsync(
        this ISetupper setupper,
        string userId,
        int auditUserId = 101,
        int accessProfileId = 501,
        params string[] privileges)
    {
        ArgumentNullException.ThrowIfNull(setupper);

        var user = new TestAuthenticatedEntity
        {
            Id = auditUserId,
            ExternalUserId = userId
        };
        var accessProfile = new TestAuthenticatedEntity
        {
            Id = accessProfileId,
            Privileges = privileges
        };

        await setupper.SetAuthenticatedUserDataAsync(user, accessProfile);

        return new TestAuthenticatedAccess(userId, auditUserId, accessProfileId, privileges);
    }
}
