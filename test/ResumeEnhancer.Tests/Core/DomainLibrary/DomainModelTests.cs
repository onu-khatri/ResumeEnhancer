using System.ComponentModel.DataAnnotations;
using Shouldly;
using DomainLibrary.DomainModel;
using ResumeModuleDM.Entities;

namespace ResumeEnhancer.Tests.Core.DomainLibrary;

public sealed class DomainModelTests
{
    [Fact]
    public void AuditEntity_DefaultValues_InitializesCreateDateAndVersion()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var entity = new Resume();

        entity.Id.ShouldBe(0);
        entity.App_CreateDate.ShouldBeGreaterThanOrEqualTo(before);
        entity.App_UpdateDate.ShouldBeNull();
        entity.App_Version.ShouldBeEmpty();
    }

    [Fact]
    public void SetupData_DefaultValues_InitializesOptionalFields()
    {
        var setup = new TestSetupData();

        setup.Code.ShouldBe(string.Empty);
        setup.Description.ShouldBe(string.Empty);
        setup.Guid.ShouldBeNull();
        setup.ObsoleteFlag.ShouldBeFalse();
    }

    [Fact]
    public void SetupData_CodeExceedsMaxLength_FailsDataAnnotationsValidation()
    {
        var setup = new TestSetupData
        {
            Code = new string('A', 101),
            Description = "valid"
        };
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            setup,
            new ValidationContext(setup),
            validationResults,
            validateAllProperties: true);

        isValid.ShouldBeFalse();
        validationResults.ShouldContain(result => result.MemberNames.Contains(nameof(SetupData.Code)));
    }

    private sealed class TestSetupData : SetupData;
}
