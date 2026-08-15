using Shouldly;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Persistence;

public sealed class ModelLoaderTests
{
    [Fact]
    public void Build_LoadsDirectAndNestedPaths_DeduplicatesPaths()
    {
        var loader = new ModelLoader<Resume>();

        loader.Build(navigator =>
        {
            navigator.Load(resume => resume.Education);
            navigator.Load(resume => resume.Education);
            navigator.Navigate(resume => resume.PersonalInformation!)
                .Load(personalInformation => personalInformation.Address);
            navigator.Navigate(resume => resume.PersonalInformation!)
                .NavigateCollection(personalInformation => personalInformation.Awards)
                .LoadRelated();
        });

        loader.GetIncludablePaths()
            .Select(path => path.Path)
            .ShouldBe(["Education", "PersonalInformation.Address", "PersonalInformation.Awards"]);
    }

    [Fact]
    public void Build_ActionIsNull_ThrowsArgumentNullException()
    {
        var loader = new ModelLoader<Resume>();

        Should.Throw<ArgumentNullException>(() => loader.Build(null!));
    }

    [Fact]
    public void Navigator_LoadExpressionIsNull_ThrowsArgumentNullException()
    {
        var navigator = new ModelLoaderNavigator<Resume>();

        Should.Throw<ArgumentNullException>(() => navigator.Load<object>(null!));
    }

    [Fact]
    public void Navigator_NavigateExpressionIsNull_ThrowsArgumentNullException()
    {
        var navigator = new ModelLoaderNavigator<Resume>();

        Should.Throw<ArgumentNullException>(() => navigator.Navigate<object>(null!));
    }
}


