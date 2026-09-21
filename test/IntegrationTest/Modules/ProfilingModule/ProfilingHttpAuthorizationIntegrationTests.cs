using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using ResumeEnhancer.AuthModule.DM.Entities;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ProfilingModule.DM.Entities;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeEnhancer.Tests.Integration.Modules.AuthModule.TestSupport;
using ResumeEnhancer.Tests.Integration.TestSupport;
using Shouldly;

namespace ResumeEnhancer.Tests.Integration.Modules.ProfilingModule;

[Collection("Sequential_Integration")]
public sealed class ProfilingHttpAuthorizationIntegrationTests(IntegrationTestAssemblyFixture fixture)
{
    [Fact]
    public async Task Profiling_routes_deny_anonymous_and_authenticated_principals_without_capability()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);

        using var anonymousClient = fixture.RealUtilities.CreateClient();
        using var anonymous = await anonymousClient.GetAsync("/api/profiling/users/", cancellationToken);
        anonymous.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        var accessToken = await CreateVerifiedAccessTokenAsync("profiling-denied@example.com", cancellationToken);
        using var authenticatedClient = fixture.RealUtilities.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var denied = await authenticatedClient.GetAsync("/api/profiling/users/", cancellationToken);
        denied.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Profiling_routes_use_real_authorization_and_preserve_http_validation_and_not_found_results()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await fixture.ResetAndSeedAsync(cancellationToken);
        var accessToken = await CreateVerifiedAccessTokenAsync(
            "profiling-allowed@example.com",
            cancellationToken,
            grantAdminCapability: true);

        using var client = fixture.RealUtilities.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using var roles = await client.GetAsync("/api/profiling/roles/", cancellationToken);
        roles.StatusCode.ShouldBe(HttpStatusCode.OK);
        using var missingRole = await client.GetAsync("/api/profiling/roles/999999", cancellationToken);
        missingRole.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        using var invalidRole = await client.PostAsJsonAsync(
            "/api/profiling/roles/",
            new { Code = "", Description = "", DisplayName = "" },
            cancellationToken);
        invalidRole.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        using var missingDelete = await client.DeleteAsync("/api/profiling/roles/999999", cancellationToken);
        missingDelete.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private async Task<string> CreateVerifiedAccessTokenAsync(
        string email,
        CancellationToken cancellationToken,
        bool grantAdminCapability = false)
    {
        using var client = fixture.RealUtilities.CreateClient();
        using var register = await client.PostAsJsonAsync(
            "/api/v1/auth/register",
            AuthApiTestData.ValidRegistration(email),
            cancellationToken);
        using var registration = await register.ReadJsonAsync(HttpStatusCode.Created, cancellationToken);
        var userId = registration.RootElement.GetProperty("userId").GetInt32();

        using (var setupper = fixture.RealUtilities.CreateSetupper())
        {
            var dbContext = (AppDbContext)setupper.GetFreshDbContext();
            var identity = await dbContext.Set<AuthenticationIdentity>()
                .SingleAsync(item => item.NormalizedEmail == email, cancellationToken);
            identity.EmailVerified = true;
            identity.EmailVerifiedAtUtc = fixture.TimeProvider.GetUtcNow().UtcDateTime;

            if (grantAdminCapability)
            {
                var user = await dbContext.Set<User>()
                    .Include(item => item.UserAccessProfiles)
                    .SingleAsync(item => item.Id == userId, cancellationToken);
                var administrator = await dbContext.Set<AccessProfile>()
                    .SingleAsync(item => item.Code == "Administrator", cancellationToken);
                var adminSource = await dbContext.Set<AccessProfileSource>()
                    .SingleAsync(item => item.Code == "admin", cancellationToken);
                var adminRole = await dbContext.Set<Role>()
                    .SingleAsync(item => item.Code == "ViewAdminPortal", cancellationToken);
                adminRole.Capability = "ViewAdminPortal";
                if (!await dbContext.Set<AccessProfileRole>().AnyAsync(
                        item => item.AccessProfileId == administrator.Id && item.RoleId == adminRole.Id,
                        cancellationToken))
                {
                    dbContext.Add(new AccessProfileRole
                    {
                        Guid = Guid.NewGuid(),
                        Code = $"{administrator.Code}:{adminRole.Id}",
                        AccessProfileId = administrator.Id,
                        RoleId = adminRole.Id,
                    });
                }

                var assignment = user.UserAccessProfiles.Single();
                assignment.AccessProfileId = administrator.Id;
                assignment.AccessProfileSourceId = adminSource.Id;
                assignment.Enabled = true;
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        using var login = await client.PostAsJsonAsync(
            "/api/v1/auth/login",
            new { Email = email, Password = "Password!1234" },
            cancellationToken);
        using var loginJson = await login.ReadJsonAsync(HttpStatusCode.OK, cancellationToken);
        var accessToken = loginJson.RootElement.GetProperty("accessToken").GetString();
        accessToken.ShouldNotBeNullOrWhiteSpace();
        return accessToken!;
    }
}
