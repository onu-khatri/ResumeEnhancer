using ResumeEnhancer.Core.DomainLibrary.DomainModel;
using Shouldly;
using ResumeEnhancer.Tests.Unit.TestInfrastructure;
using ResumeEnhancer.ResumeModule.AM.Requests;
using ResumeEnhancer.ResumeModule.DM.Entities;
using ResumeEnhancer.ResumeModule.SL.Abstractions.Persistence;
using ResumeEnhancer.ResumeModule.SL.Handlers;

namespace ResumeEnhancer.Tests.Unit.Modules.ResumeModule.Application;

public sealed class ResumeModelMapperTests
{
    [Fact]
    public void CreateResume_RequestIsNull_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ResumeModelMapper.CreateResume(null!));
    }

    [Fact]
    public void CreateResume_FullRequest_NormalizesScalarsAndCreatesGraph()
    {
        var request = ResumeTestData.CreateResumeRequest();

        var resume = ResumeModelMapper.CreateResume(request);

        resume.Title.ShouldBe("Senior Engineer");
        resume.Summary.ShouldBe("Builds things");
        resume.Photo.ShouldBeNull();
        resume.ResumeTemplate.ShouldBe("Modern");
        resume.UserId.ShouldBe(ResumeTestData.UserId);
        resume.PersonalInformation.ShouldNotBeNull();
        resume.PersonalInformation!.Resume.ShouldBeSameAs(resume);
        resume.PersonalInformation.Email.ShouldBe("person@example.com");
        resume.PersonalInformation.Address!.PersonalInformation.ShouldBeSameAs(resume.PersonalInformation);
        resume.Education.Single().Resume.ShouldBeSameAs(resume);
        resume.Certifications.Single().CertificationName.ShouldBe("Azure");
        resume.Skills.Single().SkillName.ShouldBe("C#");
        resume.WorkExperiences.Single().CompanyName.ShouldBe("Contoso");
        resume.Projects.Single().TechnologiesUsed.ShouldBe(".NET");
        resume.PersonalInformation.SocialMediaLinks.Single().Url.ShouldBe("https://example.com/profile");
    }

    [Fact]
    public void ApplyResumeUpdate_RemovePersonalInformation_RemovesOwnedEntityAndKeepsSectionsUnchanged()
    {
        var resume = ResumeTestData.ResumeGraph();
        var removed = new List<AuditEntity>();
        var request = new UpdateResumeRequest
        {
            Title = " Updated ",
            RemovePersonalInformation = true,
            Education = null
        };

        ResumeModelMapper.ApplyResumeUpdate(resume, request, removed.Add);

        resume.Title.ShouldBe("Updated");
        resume.PersonalInformation.ShouldBeNull();
        resume.Education.Count.ShouldBe(1);
        removed.ShouldHaveSingleItem().ShouldBeOfType<PersonalInformation>();
    }

    [Fact]
    public void ApplyResumeUpdate_ExistingNewAndRemovedCollectionItems_SyncsCollection()
    {
        var resume = ResumeTestData.ResumeGraph();
        var removed = new List<AuditEntity>();
        var existingEducationId = resume.Education.Single().Id;
        var request = new UpdateResumeRequest
        {
            Title = "Updated",
            UserId = " updated-user ",
            Education =
            [
                new EducationRequest
                {
                    Id = existingEducationId,
                    Degree = " MS ",
                    Institution = " New University "
                },
                new EducationRequest
                {
                    Degree = " PhD "
                }
            ],
            Skills = []
        };

        ResumeModelMapper.ApplyResumeUpdate(resume, request, removed.Add);

        resume.UserId.ShouldBe("updated-user");
        resume.Education.Count.ShouldBe(2);
        resume.Education.Single(education => education.Id == existingEducationId).Degree.ShouldBe("MS");
        resume.Education.Single(education => education.Id == 0).Resume.ShouldBeSameAs(resume);
        resume.Skills.ShouldBeEmpty();
        removed.ShouldHaveSingleItem().ShouldBeOfType<Skill>();
    }

    [Fact]
    public void ApplyResumeUpdate_UnknownCollectionId_ThrowsInvalidOperationException()
    {
        var resume = ResumeTestData.ResumeGraph();
        var request = new UpdateResumeRequest
        {
            Title = "Updated",
            Education = [new EducationRequest { Id = 999, Degree = "Unknown" }]
        };

        var exception = Should.Throw<InvalidOperationException>(
            () => ResumeModelMapper.ApplyResumeUpdate(resume, request, _ => { }));

        exception.Message.ShouldContain("does not belong");
    }

    [Fact]
    public void ApplyResumeUpdate_PersonalInformationIdDoesNotBelong_ThrowsInvalidOperationException()
    {
        var resume = ResumeTestData.ResumeGraph();
        var request = new UpdateResumeRequest
        {
            Title = "Updated",
            PersonalInformation = new PersonalInformationRequest { Id = 999 }
        };

        var exception = Should.Throw<InvalidOperationException>(
            () => ResumeModelMapper.ApplyResumeUpdate(resume, request, _ => { }));

        exception.Message.ShouldContain("Personal information '999'");
    }

    [Fact]
    public void ApplyResumeUpdate_AddressIdDoesNotBelong_ThrowsInvalidOperationException()
    {
        var resume = ResumeTestData.ResumeGraph();
        var request = new UpdateResumeRequest
        {
            Title = "Updated",
            PersonalInformation = new PersonalInformationRequest
            {
                Id = resume.PersonalInformation!.Id,
                Address = new AddressRequest { Id = 999 }
            }
        };

        var exception = Should.Throw<InvalidOperationException>(
            () => ResumeModelMapper.ApplyResumeUpdate(resume, request, _ => { }));

        exception.Message.ShouldContain("Address '999'");
    }

    [Fact]
    public void ApplyResumeUpdate_NewPersonalInformationWithExistingId_ThrowsInvalidOperationException()
    {
        var resume = new Resume { Id = 5, Title = "Title", UserId = ResumeTestData.UserId };
        var request = new UpdateResumeRequest
        {
            Title = "Updated",
            PersonalInformation = new PersonalInformationRequest { Id = 12 }
        };

        var exception = Should.Throw<InvalidOperationException>(
            () => ResumeModelMapper.ApplyResumeUpdate(resume, request, _ => { }));

        exception.Message.ShouldContain("Personal information '12'");
    }

    [Fact]
    public void ApplyResumeUpdate_RemoveAddress_RemovesOnlyAddress()
    {
        var resume = ResumeTestData.ResumeGraph();
        var removed = new List<AuditEntity>();
        var request = new UpdateResumeRequest
        {
            Title = "Updated",
            PersonalInformation = new PersonalInformationRequest
            {
                Id = resume.PersonalInformation!.Id,
                RemoveAddress = true
            }
        };

        ResumeModelMapper.ApplyResumeUpdate(resume, request, removed.Add);

        resume.PersonalInformation!.Address.ShouldBeNull();
        removed.ShouldHaveSingleItem().ShouldBeOfType<Address>();
    }

    [Fact]
    public void ToCriteria_RequestValues_MapAndNormalize()
    {
        var request = new ResumeSearchRequest
        {
            UserId = " user ",
            SearchText = " engineer ",
            ResumeTemplate = " modern ",
            SortBy = ResumeSearchSortBy.Title,
            SortDirection = ResumeEnhancer.ResumeModule.AM.Requests.SortDirection.Ascending
        };

        var criteria = ResumeModelMapper.ToCriteria(request);

        criteria.UserId.ShouldBe("user");
        criteria.SearchText.ShouldBe("engineer");
        criteria.ResumeTemplate.ShouldBe("modern");
        criteria.SortBy.ShouldBe(ResumeSortBy.Title);
        criteria.SortDirection.ShouldBe(ResumeSortDirection.Ascending);
    }

    [Fact]
    public void ToCriteria_InvalidSortValues_UseDefaults()
    {
        var request = new ResumeSearchRequest
        {
            SortBy = (ResumeSearchSortBy)999,
            SortDirection = (ResumeEnhancer.ResumeModule.AM.Requests.SortDirection)999
        };

        var criteria = ResumeModelMapper.ToCriteria(request);

        criteria.SortBy.ShouldBe(ResumeSortBy.UpdatedDate);
        criteria.SortDirection.ShouldBe(ResumeSortDirection.Descending);
    }

    [Fact]
    public void MapSearch_ResultWithItems_MapsCountsAndPaging()
    {
        var resume = ResumeTestData.ResumeGraph();
        var result = new ResumeSearchResult([resume], pageNumber: 2, pageSize: 10, totalCount: 11);

        var response = ResumeModelMapper.MapSearch(result);

        response.Items.Single().EducationCount.ShouldBe(1);
        response.Items.Single().SkillCount.ShouldBe(1);
        response.TotalPages.ShouldBe(2);
        response.HasPreviousPage.ShouldBeTrue();
        response.HasNextPage.ShouldBeFalse();
    }

    [Fact]
    public void MapDelete_Result_MapsCountsAndFailures()
    {
        var result = new ResumeDeleteResult([1, 2], [1], [2], []);

        var response = ResumeModelMapper.MapDelete(result);

        response.DeletedCount.ShouldBe(1);
        response.HasFailures.ShouldBeTrue();
    }

    [Fact]
    public void MapDetail_ResumeIsNull_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ResumeModelMapper.MapDetail(null!));
    }

    [Fact]
    public void EnsureUserAccess_UserMissingOrMatches_DoesNotThrow()
    {
        var resume = ResumeTestData.ResumeGraph(userId: "user");

        Should.NotThrow(() => ResumeModelMapper.EnsureUserAccess(resume, null));
        Should.NotThrow(() => ResumeModelMapper.EnsureUserAccess(resume, " "));
        Should.NotThrow(() => ResumeModelMapper.EnsureUserAccess(resume, " user "));
    }

    [Fact]
    public void EnsureUserAccess_UserMismatch_ThrowsUnauthorizedAccessException()
    {
        var resume = ResumeTestData.ResumeGraph(userId: "owner");

        var exception = Should.Throw<UnauthorizedAccessException>(
            () => ResumeModelMapper.EnsureUserAccess(resume, "other"));

        exception.Message.ShouldContain("does not belong");
    }
}



