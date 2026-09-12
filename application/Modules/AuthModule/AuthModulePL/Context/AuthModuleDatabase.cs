namespace ResumeEnhancer.AuthModule.PL;

public static class AuthModuleDatabase
{
    public static string GetSchema(string? root)
    {
        return string.IsNullOrWhiteSpace(root) ? "auth" : $"{root}_auth";
    }
}
