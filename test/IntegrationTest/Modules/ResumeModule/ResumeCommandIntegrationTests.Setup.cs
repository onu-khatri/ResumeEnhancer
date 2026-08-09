using System.Net;
using MoreLinq;
using ResumeEnhancer.TestUtilities.IntegrationSupport;
using ResumeModuleAM.Requests;
using ResumeModuleAM.Responses;
using Shouldly;

namespace ResumeEnhancer.IntegrationTests.Modules.ResumeModule;

public sealed partial class ResumeCommandIntegrationTests
{
    public static IEnumerable<object[]> CreateResumeSetups()
    {
        var fullGraphRequest = ResumeApiTestData.CreateResumeRequest(seed: 101);
        yield return
        [
            new ResumeEndpointSetup<CreateResumeRequest>(
                "authenticated owner creates a full resume graph",
                HttpMethod.Post,
                "/api/resumes/",
                fullGraphRequest,
                async (setupper, _, _) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 41,
                        accessProfileId: 601,
                        "resume.create");
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeDetailResponse>(
                        HttpStatusCode.Created,
                        cancellationToken);
                    var saved = await setupper.FindResumeGraphAsync(response.Id, cancellationToken);

                    responseMessage.Headers.Location!.ToString().ShouldBe($"/api/resumes/{response.Id}");
                    response.Title.ShouldBe(fullGraphRequest.Title.Trim());
                    response.UserId.ShouldBe(ResumeApiTestData.OwnerUserId);
                    response.PersonalInformation.ShouldNotBeNull();
                    response.Education.Count.ShouldBe(1);
                    response.Certifications.Count.ShouldBe(1);
                    response.Skills.Count.ShouldBe(1);
                    response.WorkExperiences.Count.ShouldBe(1);
                    response.Projects.Count.ShouldBe(1);

                    saved.ShouldNotBeNull();
                    saved!.Title.ShouldBe(fullGraphRequest.Title.Trim());
                    saved.App_CreateUserId.ShouldBe(41);
                    saved.App_UpdateUserId.ShouldBe(41);
                    saved.PersonalInformation.ShouldNotBeNull();
                    saved.PersonalInformation!.Address.ShouldNotBeNull();
                    saved.PersonalInformation.Awards.Count.ShouldBe(1);
                    saved.PersonalInformation.Languages.Count.ShouldBe(1);
                    saved.PersonalInformation.Hobbies.Count.ShouldBe(1);
                    saved.PersonalInformation.SocialMediaLinks.Count.ShouldBe(1);
                    saved.Education.Count.ShouldBe(1);
                    saved.Certifications.Count.ShouldBe(1);
                    saved.Skills.Count.ShouldBe(1);
                    saved.WorkExperiences.Count.ShouldBe(1);
                    saved.Projects.Count.ShouldBe(1);
                })
        ];

        var minimalRequest = ResumeApiTestData.CreateResumeRequest(includeFullGraph: false, seed: 102);
        yield return
        [
            new ResumeEndpointSetup<CreateResumeRequest>(
                "authenticated owner creates a minimal resume",
                HttpMethod.Post,
                "/api/resumes/",
                minimalRequest,
                async (setupper, _, _) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 42,
                        accessProfileId: 602,
                        "resume.create");
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeDetailResponse>(
                        HttpStatusCode.Created,
                        cancellationToken);
                    var saved = await setupper.FindResumeGraphAsync(response.Id, cancellationToken);

                    response.PersonalInformation.ShouldBeNull();
                    response.Education.ShouldBeEmpty();
                    response.Certifications.ShouldBeEmpty();
                    response.Skills.ShouldBeEmpty();
                    response.WorkExperiences.ShouldBeEmpty();
                    response.Projects.ShouldBeEmpty();

                    saved.ShouldNotBeNull();
                    saved!.PersonalInformation.ShouldBeNull();
                    saved.Education.ShouldBeEmpty();
                    saved.Certifications.ShouldBeEmpty();
                    saved.Skills.ShouldBeEmpty();
                    saved.WorkExperiences.ShouldBeEmpty();
                    saved.Projects.ShouldBeEmpty();
                    saved.App_CreateUserId.ShouldBe(42);
                })
        ];

        var invalidRequest = ResumeApiTestData.CreateResumeRequest(seed: 103);
        invalidRequest.Title = string.Empty;
        yield return
        [
            new ResumeEndpointSetup<CreateResumeRequest>(
                "invalid request returns validation problem without persisting",
                HttpMethod.Post,
                "/api/resumes/",
                invalidRequest,
                async (setupper, _, _) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 43,
                        accessProfileId: 603,
                        "resume.create");
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    responseMessage.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
                    var payload = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
                    payload.ShouldContain("Title");
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(0);
                })
        ];
    }

    public static IEnumerable<object[]> UpdateResumeSetups()
    {
        var updateRequest = ResumeApiTestData.UpdateResumeRequest(seed: 201);
        yield return
        [
            new ResumeEndpointSetup<UpdateResumeRequest>(
                "owner updates scalar fields and synchronizes owned graph",
                HttpMethod.Put,
                "/api/resumes/0",
                updateRequest,
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 51,
                        accessProfileId: 701,
                        "resume.update");
                    var seeded = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Original Integration Resume",
                        auditUserId: 11,
                        cancellationToken: cancellationToken);

                    updateRequest.PersonalInformation!.Id = seeded.PersonalInformation!.Id;
                    updateRequest.Skills![0].Id = seeded.Skills.Single(skill => skill.SkillName == "C#").Id;
                    setup.Route = $"/api/resumes/{seeded.Id}";
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeDetailResponse>(
                        HttpStatusCode.OK,
                        cancellationToken);
                    var saved = await setupper.FindResumeGraphAsync(response.Id, cancellationToken);

                    response.Title.ShouldBe("Updated Integration Resume");
                    response.PersonalInformation.ShouldNotBeNull();
                    response.PersonalInformation!.Address.ShouldBeNull();
                    response.Skills.Select(skill => skill.SkillName)
                        .ShouldBe(["Updated C#", "Distributed Systems"], ignoreOrder: true);
                    response.Education.ShouldBeEmpty();

                    saved.ShouldNotBeNull();
                    saved!.Title.ShouldBe("Updated Integration Resume");
                    saved.Summary.ShouldBe("Updated summary from API");
                    saved.ResumeTemplate.ShouldBe("Focused");
                    saved.App_CreateUserId.ShouldBe(11);
                    saved.App_UpdateUserId.ShouldBe(51);
                    saved.PersonalInformation.ShouldNotBeNull();
                    saved.PersonalInformation!.Email.ShouldBe("updated-201@example.com");
                    saved.PersonalInformation.Address.ShouldBeNull();
                    saved.Education.ShouldBeEmpty();
                    saved.Skills.Select(skill => skill.SkillName)
                        .ShouldBe(["Updated C#", "Distributed Systems"], ignoreOrder: true);
                    saved.Skills.ShouldNotContain(skill => skill.SkillName == "Legacy Skill");
                })
        ];

        var forbiddenRequest = new UpdateResumeRequest { Title = "Forbidden Update" };
        yield return
        [
            new ResumeEndpointSetup<UpdateResumeRequest>(
                "different active user receives forbidden and cannot update",
                HttpMethod.Put,
                "/api/resumes/0",
                forbiddenRequest,
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.IntruderUserId,
                        auditUserId: 52,
                        accessProfileId: 702,
                        "resume.update");
                    var seeded = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Owned Resume",
                        auditUserId: 12,
                        cancellationToken: cancellationToken);

                    setup.Route = $"/api/resumes/{seeded.Id}";
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    responseMessage.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
                    var saved = await SingleResumeAsync(setupper, cancellationToken);

                    saved.Title.ShouldBe("Owned Resume");
                    saved.App_UpdateUserId.ShouldBe(12);
                })
        ];
    }

    public static IEnumerable<object[]> DeleteResumeSetups()
    {
        yield return
        [
            new ResumeEndpointSetup(
                "owner deletes a single resume through DELETE route",
                HttpMethod.Delete,
                "/api/resumes/0",
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 61,
                        accessProfileId: 801,
                        "resume.delete");
                    var seeded = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Delete Me",
                        auditUserId: 21,
                        cancellationToken: cancellationToken);

                    setup.Route = $"/api/resumes/{seeded.Id}";
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeDeleteResponse>(
                        HttpStatusCode.OK,
                        cancellationToken);

                    response.DeletedCount.ShouldBe(1);
                    response.HasFailures.ShouldBeFalse();
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(0);
                })
        ];

        yield return
        [
            new ResumeEndpointSetup(
                "missing single delete returns not-found ids without side effects",
                HttpMethod.Delete,
                "/api/resumes/999999",
                async (setupper, _, _) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 62,
                        accessProfileId: 802,
                        "resume.delete");
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeDeleteResponse>(
                        HttpStatusCode.OK,
                        cancellationToken);

                    response.DeletedCount.ShouldBe(0);
                    response.NotFoundIds.ShouldBe([999999]);
                    response.HasFailures.ShouldBeTrue();
                    (await CountResumesAsync(setupper, cancellationToken)).ShouldBe(0);
                })
        ];
    }

    public static IEnumerable<object[]> BulkDeleteResumeSetups()
    {
        var request = new DeleteResumesRequest();

        yield return
        [
            new ResumeEndpointSetup<DeleteResumesRequest>(
                "bulk delete reports deleted forbidden and missing ids",
                HttpMethod.Post,
                "/api/resumes/delete",
                request,
                async (setupper, setup, cancellationToken) =>
                {
                    await setupper.SetupAccessAsync(
                        ResumeApiTestData.OwnerUserId,
                        auditUserId: 63,
                        accessProfileId: 803,
                        "resume.delete");
                    var ownerResume = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OwnerUserId,
                        "Owner Delete",
                        auditUserId: 22,
                        cancellationToken: cancellationToken);
                    var otherResume = await setupper.GenerateResumeAsync(
                        ResumeApiTestData.OtherUserId,
                        "Other Keep",
                        auditUserId: 23,
                        cancellationToken: cancellationToken);

                    setup.Input.ResumeIds = [ownerResume.Id, otherResume.Id, 999999, ownerResume.Id];
                },
                async (setupper, responseMessage, cancellationToken) =>
                {
                    var response = await responseMessage.ReadSuccessJsonAsync<ResumeDeleteResponse>(
                        HttpStatusCode.OK,
                        cancellationToken);
                    var remainingIds = await ResumeIdsAsync(setupper, cancellationToken);

                    response.RequestedIds.ShouldBe([response.DeletedIds.Single(), response.ForbiddenIds.Single(), 999999]);
                    response.DeletedCount.ShouldBe(1);
                    response.NotFoundIds.ShouldBe([999999]);
                    response.ForbiddenIds.Count.ShouldBe(1);
                    response.HasFailures.ShouldBeTrue();
                    MoreEnumerable.ForEach(response.DeletedIds, deletedId => remainingIds.ShouldNotContain(deletedId));
                    remainingIds.ShouldContain(response.ForbiddenIds.Single());
                })
        ];
    }
}
