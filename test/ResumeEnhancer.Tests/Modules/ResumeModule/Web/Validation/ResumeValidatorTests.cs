using Shouldly;
using ResumeEnhancer.Tests.TestInfrastructure;
using ResumeModuleAM.Requests;
using ResumeModuleWeb.Validation.PersonalInformation;
using ResumeModuleWeb.Validation.Resumes;
using RequestSortDirection = ResumeModuleAM.Requests.SortDirection;

namespace ResumeEnhancer.Tests.Modules.ResumeModule.Web.Validation;

public sealed class ResumeValidatorTests
{
    [Fact]
    public void CreateResumeRequestValidator_ValidFullRequest_Passes()
    {
        var validator = new CreateResumeRequestValidator();
        var request = ResumeTestData.CreateResumeRequest();

        var result = validator.Validate(request);

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void CreateResumeRequestValidator_NullCollections_ReturnsErrors()
    {
        var validator = new CreateResumeRequestValidator();
        var request = new CreateResumeRequest
        {
            Title = "Title",
            UserId = "user",
            Education = null!,
            Certifications = null!,
            Skills = null!,
            WorkExperiences = null!,
            Projects = null!
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(CreateResumeRequest.Education));
        result.ShouldHaveErrorFor(nameof(CreateResumeRequest.Certifications));
        result.ShouldHaveErrorFor(nameof(CreateResumeRequest.Skills));
        result.ShouldHaveErrorFor(nameof(CreateResumeRequest.WorkExperiences));
        result.ShouldHaveErrorFor(nameof(CreateResumeRequest.Projects));
    }

    [Fact]
    public void UpdateResumeRequestValidator_RemoveAndUpdatePersonalInformation_ReturnsError()
    {
        var validator = new UpdateResumeRequestValidator();
        var request = new UpdateResumeRequest
        {
            Title = "Title",
            RemovePersonalInformation = true,
            PersonalInformation = new PersonalInformationRequest()
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(UpdateResumeRequest.PersonalInformation), "Cannot remove and update");
    }

    [Fact]
    public void UpdateResumeRequestValidator_DuplicateExistingIds_ReturnsCollectionError()
    {
        var validator = new UpdateResumeRequestValidator();
        var request = new UpdateResumeRequest
        {
            Title = "Title",
            Skills =
            [
                new SkillRequest { Id = 10, SkillName = "C#" },
                new SkillRequest { Id = 10, SkillName = "SQL" }
            ]
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(UpdateResumeRequest.Skills), "duplicate item id '10'");
    }

    [Fact]
    public void PersonalInformationRequestValidator_CreateWithRemoveAddress_ReturnsError()
    {
        var validator = new PersonalInformationRequestValidator(isCreate: true);
        var request = new PersonalInformationRequest { RemoveAddress = true };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(PersonalInformationRequest.RemoveAddress), "Cannot remove an address");
    }

    [Fact]
    public void PersonalInformationRequestValidator_RemoveAndUpdateAddress_ReturnsError()
    {
        var validator = new PersonalInformationRequestValidator();
        var request = new PersonalInformationRequest
        {
            RemoveAddress = true,
            Address = new AddressRequest()
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(PersonalInformationRequest.Address), "Cannot remove and update");
    }

    [Fact]
    public void PersonalInformationRequestValidator_DuplicateNestedExistingIds_ReturnsCollectionError()
    {
        var validator = new PersonalInformationRequestValidator();
        var request = new PersonalInformationRequest
        {
            Awards =
            [
                new AwardRequest { Id = 8, AwardName = "A" },
                new AwardRequest { Id = 8, AwardName = "B" }
            ]
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor(nameof(PersonalInformationRequest.Awards), "duplicate item id '8'");
    }

    [Fact]
    public void ResumeSearchRequestValidator_InvalidDateRangesAndPaging_ReturnsErrors()
    {
        var validator = new ResumeSearchRequestValidator();
        var request = new ResumeSearchRequest
        {
            Ids = [1, 0],
            CreatedFromUtc = new DateTime(2025, 1, 1),
            CreatedToUtc = new DateTime(2024, 1, 1),
            UpdatedFromUtc = new DateTime(2025, 1, 1),
            UpdatedToUtc = new DateTime(2024, 1, 1),
            PageNumber = 0,
            PageSize = 101,
            SortBy = (ResumeSearchSortBy)999,
            SortDirection = (RequestSortDirection)999
        };

        var result = validator.Validate(request);

        result.ShouldHaveErrorFor("Ids[1]");
        result.ShouldHaveErrorFor(nameof(ResumeSearchRequest.CreatedToUtc));
        result.ShouldHaveErrorFor(nameof(ResumeSearchRequest.UpdatedToUtc));
        result.ShouldHaveErrorFor(nameof(ResumeSearchRequest.PageNumber));
        result.ShouldHaveErrorFor(nameof(ResumeSearchRequest.PageSize));
        result.ShouldHaveErrorFor(nameof(ResumeSearchRequest.SortBy));
        result.ShouldHaveErrorFor(nameof(ResumeSearchRequest.SortDirection));
    }

    [Fact]
    public void DeleteResumesRequestValidator_NullOrInvalidIds_ReturnsErrors()
    {
        var validator = new DeleteResumesRequestValidator();

        var nullResult = validator.Validate(new DeleteResumesRequest { ResumeIds = null! });
        var invalidResult = validator.Validate(new DeleteResumesRequest { ResumeIds = [1, -1] });

        nullResult.ShouldHaveErrorFor(nameof(DeleteResumesRequest.ResumeIds));
        invalidResult.ShouldHaveErrorFor("ResumeIds[1]");
    }
}
