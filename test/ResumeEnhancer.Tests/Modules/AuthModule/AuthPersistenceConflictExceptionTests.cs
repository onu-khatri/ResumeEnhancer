using ResumeEnhancer.AuthModule.SL.Abstractions;
using Shouldly;

namespace ResumeEnhancer.Tests.Unit.Modules.AuthModule;

public sealed class AuthPersistenceConflictExceptionTests
{
    [Fact]
    public void Preserves_the_database_failure_as_inner_exception()
    {
        var inner = new InvalidOperationException("duplicate");
        var exception = new AuthPersistenceConflictException(inner);

        exception.Message.ShouldBe("The authentication record conflicts with an existing record.");
        exception.InnerException.ShouldBeSameAs(inner);
    }
}
