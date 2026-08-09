using System.Net;
using System.Net.Http.Json;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;
using Shouldly;

namespace ResumeEnhancer.IntegrationTests.Modules.ResumeModule;

public sealed partial class ResumeQueryIntegrationTests
{
    public static IEnumerable<object[]> GetResumeSetups()
    {
        yield return
        [
            new ResumeEndpointSetup(
                "owner can get own resume graph",
                HttpMethod.Get,
                "/api/resumes/0",
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 71,
                        accessProfileId: 901,
                        "resume.read");
                    var seeded = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Readable Resume",
                        auditUserId: 31,
                        cancellationToken: cancellationToken);

                    setup.Route = $"/api/resumes/{seeded.Id}";
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeDetailResponse>(
                        HttpStatusCode.OK,
                        cancellationToken);

                    response.Title.ShouldBe("Readable Resume");
                    response.PersonalInformation.ShouldNotBeNull();
                    response.Skills.Count.ShouldBe(2);
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(1);
                })
        ];

        yield return
        [
            new ResumeEndpointSetup(
                "different active user receives not found for owned resume",
                HttpMethod.Get,
                "/api/resumes/0",
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.IntruderUserId,
                        auditUserId: 72,
                        accessProfileId: 902,
                        "resume.read");
                    var seeded = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Hidden Resume",
                        auditUserId: 32,
                        cancellationToken: cancellationToken);

                    setup.Route = $"/api/resumes/{seeded.Id}";
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    responseMessage.StatusCode.ShouldBe(HttpStatusCode.NotFound);
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(1);
                })
        ];
    }

    public static IEnumerable<object[]> ResumeExistsSetups()
    {
        yield return
        [
            new ResumeEndpointSetup(
                "owner existence check returns true",
                HttpMethod.Get,
                "/api/resumes/0/exists",
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 73,
                        accessProfileId: 903,
                        "resume.read");
                    var seeded = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Exists Resume",
                        auditUserId: 33,
                        cancellationToken: cancellationToken);

                    setup.Route = $"/api/resumes/{seeded.Id}/exists";
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
                    var exists = await responseMessage.Content.ReadFromJsonAsync<bool>(cancellationToken);

                    exists.ShouldBeTrue();
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(1);
                })
        ];

        yield return
        [
            new ResumeEndpointSetup(
                "different active user existence check returns false",
                HttpMethod.Get,
                "/api/resumes/0/exists",
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.IntruderUserId,
                        auditUserId: 74,
                        accessProfileId: 904,
                        "resume.read");
                    var seeded = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Exists Hidden Resume",
                        auditUserId: 34,
                        cancellationToken: cancellationToken);

                    setup.Route = $"/api/resumes/{seeded.Id}/exists";
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
                    var exists = await responseMessage.Content.ReadFromJsonAsync<bool>(cancellationToken);

                    exists.ShouldBeFalse();
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(1);
                })
        ];
    }

    public static IEnumerable<object[]> SearchResumeSetups()
    {
        var platformSearch = ResumeApiTestData.SearchRequest(
            searchText: "Platform",
            template: "Modern",
            hasPhoto: true);

        yield return
        [
            new ResumeEndpointSetup<ResumeSearchRequest>(
                "search filters by user search text template and photo",
                HttpMethod.Post,
                "/api/resumes/search",
                platformSearch,
                ArrangeSearchGraphAsync,
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeSearchResponse>(
                        HttpStatusCode.OK,
                        cancellationToken);

                    response.TotalCount.ShouldBe(1);
                    response.Items.ShouldHaveSingleItem().Title.ShouldBe("Platform Engineer");
                    response.Items.Single().SkillCount.ShouldBe(2);
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(3);
                })
        ];

        var noPhotoSearch = ResumeApiTestData.SearchRequest(
            searchText: "No Photo",
            template: "Classic",
            hasPhoto: false);

        yield return
        [
            new ResumeEndpointSetup<ResumeSearchRequest>(
                "search can return owner resumes without photo",
                HttpMethod.Post,
                "/api/resumes/search",
                noPhotoSearch,
                ArrangeSearchGraphAsync,
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeSearchResponse>(
                        HttpStatusCode.OK,
                        cancellationToken);

                    response.TotalCount.ShouldBe(1);
                    response.Items.ShouldHaveSingleItem().Title.ShouldBe("No Photo Resume");
                    response.Items.Single().Photo.ShouldBeNull();
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(3);
                })
        ];
    }

    private static async Task ArrangeSearchGraphAsync(
        ISetupper setupper,
        ResumeEndpointSetup<ResumeSearchRequest> _,
        CancellationToken cancellationToken)
    {
        await setupper.SetupAccessAsync(
            ResumeApiTestData.OwnerUserId,
            auditUserId: 75,
            accessProfileId: 905,
            "resume.search");
        await setupper.GenerateResumeAsync(
            ResumeApiTestData.OwnerUserId,
            "Platform Engineer",
            "Modern",
            "https://example.com/platform.png",
            auditUserId: 35,
            cancellationToken: cancellationToken);
        await setupper.GenerateResumeAsync(
            ResumeApiTestData.OwnerUserId,
            "No Photo Resume",
            "Classic",
            photo: null,
            auditUserId: 36,
            cancellationToken: cancellationToken);
        await setupper.GenerateResumeAsync(
            ResumeApiTestData.OtherUserId,
            "Platform Engineer Other",
            "Modern",
            "https://example.com/other.png",
            auditUserId: 37,
            cancellationToken: cancellationToken);
    }
}
