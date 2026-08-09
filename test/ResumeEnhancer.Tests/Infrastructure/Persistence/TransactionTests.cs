using Microsoft.EntityFrameworkCore.Storage;
using NSubstitute;
using Shouldly;
using Persistence;

namespace ResumeEnhancer.Tests.Infrastructure.Persistence;

public sealed class TransactionTests
{
    [Fact]
    public async Task NonRelationalDbTransaction_CommitMarksCompleted()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var transaction = new NonRelationalDbTransaction();

        await transaction.CommitAsync(cancellationToken);
        await transaction.RollbackAsync(cancellationToken);

        transaction.IsCompleted.ShouldBeTrue();
        transaction.Dispose();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NonRelationalDbTransaction_CancellationRequested_ThrowsOperationCanceledException()
    {
        var transaction = new NonRelationalDbTransaction();
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(
            () => transaction.CommitAsync(cancellationTokenSource.Token));
    }

    [Fact]
    public async Task NestedDbTransaction_CommitDoesNotCommitOuterTransaction()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var currentTransaction = Substitute.For<IDbContextTransaction>();
        var transaction = new NestedDbTransaction(currentTransaction);

        await transaction.CommitAsync(cancellationToken);
        await transaction.RollbackAsync(cancellationToken);

        transaction.IsCompleted.ShouldBeTrue();
        await currentTransaction.DidNotReceive().CommitAsync(cancellationToken);
        await currentTransaction.DidNotReceive().RollbackAsync(cancellationToken);
    }

    [Fact]
    public async Task NestedDbTransaction_RollbackRollsBackCurrentTransactionOnce()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var currentTransaction = Substitute.For<IDbContextTransaction>();
        var transaction = new NestedDbTransaction(currentTransaction);

        await transaction.RollbackAsync(cancellationToken);
        await transaction.RollbackAsync(cancellationToken);

        transaction.IsCompleted.ShouldBeTrue();
        await currentTransaction.Received(1).RollbackAsync(cancellationToken);
    }

    [Fact]
    public async Task RelationalDbTransaction_CommitIsIdempotent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var dbTransaction = Substitute.For<IDbContextTransaction>();
        var transaction = new RelationalDbTransaction(dbTransaction);

        await transaction.CommitAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        transaction.IsCompleted.ShouldBeTrue();
        await dbTransaction.Received(1).CommitAsync(cancellationToken);
    }

    [Fact]
    public async Task RelationalDbTransaction_RollbackIsIdempotent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var dbTransaction = Substitute.For<IDbContextTransaction>();
        var transaction = new RelationalDbTransaction(dbTransaction);

        await transaction.RollbackAsync(cancellationToken);
        await transaction.RollbackAsync(cancellationToken);

        transaction.IsCompleted.ShouldBeTrue();
        await dbTransaction.Received(1).RollbackAsync(cancellationToken);
    }

    [Fact]
    public async Task RelationalDbTransaction_DisposeDelegatesToInnerTransaction()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var dbTransaction = Substitute.For<IDbContextTransaction>();
        var transaction = new RelationalDbTransaction(dbTransaction);

        transaction.Dispose();
        await transaction.DisposeAsync();

        dbTransaction.Received(1).Dispose();
        await dbTransaction.Received(1).DisposeAsync();
        cancellationToken.ThrowIfCancellationRequested();
    }
}
