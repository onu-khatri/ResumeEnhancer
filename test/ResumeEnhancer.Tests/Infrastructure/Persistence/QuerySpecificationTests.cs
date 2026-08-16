using Shouldly;
using ResumeEnhancer.Infrastructure.Persistence;
using ResumeEnhancer.ResumeModule.DM.Entities;

namespace ResumeEnhancer.Tests.Unit.Infrastructure.Persistence;

public sealed class QuerySpecificationTests
{
    [Fact]
    public void GetQuery_CriteriaOrderAndSelectApplied_ReturnsProjectedItems()
    {
        var resumes = new[]
        {
            new Resume { Id = 1, Title = "B", UserId = 1 },
            new Resume { Id = 2, Title = "A", UserId = 1 },
            new Resume { Id = 3, Title = "C", UserId = 2 }
        }.AsQueryable();
        var specification = new TestResumeSpecification();
        specification.Criteria = resume => resume.UserId == 1;
        specification.Select = resume => new Resume { Id = resume.Id, Title = resume.Title };
        specification.OrderByTitle();

        var result = specification.GetQuery(resumes).ToArray();

        result.Select(resume => resume.Title).ShouldBe(["A", "B"]);
    }

    [Fact]
    public void GetQuery_DescendingOrderApplied_ReturnsDescendingItems()
    {
        var resumes = new[]
        {
            new Resume { Id = 1, Title = "A" },
            new Resume { Id = 2, Title = "B" }
        }.AsQueryable();
        var specification = new TestResumeSpecification();
        specification.OrderByTitleDescending();

        var result = specification.GetQuery(resumes).ToArray();

        result.Select(resume => resume.Title).ShouldBe(["B", "A"]);
    }

    [Fact]
    public void GetQuery_InputQueryIsNull_ThrowsArgumentNullException()
    {
        var specification = new TestResumeSpecification();

        Should.Throw<ArgumentNullException>(() => specification.GetQuery(null!));
    }

    [Fact]
    public void ApplyOrderBy_ExpressionIsNull_ThrowsArgumentNullException()
    {
        var specification = new TestResumeSpecification();

        Should.Throw<ArgumentNullException>(() => specification.ApplyOrdering(null!));
    }

    private sealed class TestResumeSpecification : QuerySpecification<Resume>
    {
        public void OrderByTitle() => ApplyOrderBy(resume => resume.Title);

        public void OrderByTitleDescending() => ApplyOrderByDescending(resume => resume.Title);

        public void ApplyOrdering(System.Linq.Expressions.Expression<Func<Resume, object>> expression) =>
            ApplyOrderBy(expression);
    }
}


