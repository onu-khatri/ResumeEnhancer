namespace ResumeEnhancer.TestUtilities.IntegrationSupport;

public static class SetupperAccessExtensions
{
    public static async Task<TestAuthenticatedAccess> SetupAccessAsync(
        this ISetupper setupper,
        int userId,
        int auditUserId = 101,
        int accessProfileId = 501,
        params string[] privileges)
    {
        ArgumentNullException.ThrowIfNull(setupper);

        var user = new TestAuthenticatedEntity
        {
            Id = auditUserId,
            ExternalUserId = userId.ToString()
        };
        var accessProfile = new TestAuthenticatedEntity
        {
            Id = accessProfileId,
            Privileges = privileges
        };

        await setupper.SetAuthenticatedUserDataAsync(user, accessProfile);

        return new TestAuthenticatedAccess(userId.ToString(), auditUserId, accessProfileId, privileges);
    }
}
