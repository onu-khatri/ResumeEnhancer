using Shouldly;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Persistence;

public sealed class IncludablePathTests
{
    [Fact]
    public void Constructor_SegmentsContainNullOrWhitespace_FiltersInvalidSegments()
    {
        var path = new IncludablePath(["PersonalInformation", "", "  ", "Address"]);

        path.Segments.ShouldBe(["PersonalInformation", "Address"]);
        path.Path.ShouldBe("PersonalInformation.Address");
        path.ToString().ShouldBe("PersonalInformation.Address");
    }

    [Fact]
    public void Constructor_SegmentsAreNull_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => new IncludablePath(null!));
    }

    [Fact]
    public void FromExpression_MemberPath_ReturnsPath()
    {
        var path = IncludablePath.FromExpression<Resume, string?>(resume => resume.PersonalInformation!.Email);

        path.Path.ShouldBe("PersonalInformation.Email");
    }

    [Fact]
    public void FromExpression_ValueTypeMember_HandlesConversionExpression()
    {
        var path = IncludablePath.FromExpression<Resume, object>(resume => resume.Id);

        path.Path.ShouldBe("Id");
    }

    [Fact]
    public void FromExpression_NotMemberExpression_ThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(
            () => IncludablePath.FromExpression<Resume, string>(resume => resume.Title.ToLowerInvariant()));
    }

    [Fact]
    public void Equals_PathsMatch_ReturnsTrueAndSameHashCode()
    {
        var first = IncludablePath.FromSegments(["Education"]);
        var second = IncludablePath.FromSegments(["Education"]);

        first.Equals(second).ShouldBeTrue();
        first.Equals((object)second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    public void Equals_OtherIsNull_ReturnsFalse()
    {
        var path = IncludablePath.FromSegments(["Education"]);

        path.Equals(null).ShouldBeFalse();
        path.Equals(new object()).ShouldBeFalse();
    }
}


