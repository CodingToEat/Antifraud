using Moq;
using Transactions.Application.Commands;
using Transactions.Application.Ports;
using Transactions.Domain.Dto;
using Transactions.Domain.Entity;

namespace Transactions.Api.Tests;

public class UpdateTransactionStatusHandlerTests
{
    private readonly Mock<ITransactionRepository> _repoMock = new();

    [Fact]
    public async Task UpdatesTransactionStatusSuccessfully()
    {
        var tx = new Transaction { TransactionExternalId = Guid.NewGuid() };
        _repoMock.Setup(r => r.GetByExternalIdAsync(tx.TransactionExternalId)).ReturnsAsync(tx);

        var handler = new UpdateTransactionStatusHandler(_repoMock.Object);
        var dto = new UpdateTransactionStatusRequest("Approved");

        await handler.HandleAsync(tx.TransactionExternalId, dto);

        Assert.Equal(TransactionStatus.Approved, tx.Status);
        _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Throws_When_TransactionNotFound()
    {
        var handler = new UpdateTransactionStatusHandler(_repoMock.Object);
        var id = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.HandleAsync(id, new UpdateTransactionStatusRequest("Approved")));
    }

    [Fact]
    public async Task Throws_When_InvalidStatus()
    {
        var tx = new Transaction { TransactionExternalId = Guid.NewGuid() };
        _repoMock.Setup(r => r.GetByExternalIdAsync(tx.TransactionExternalId)).ReturnsAsync(tx);

        var handler = new UpdateTransactionStatusHandler(_repoMock.Object);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.HandleAsync(tx.TransactionExternalId, new UpdateTransactionStatusRequest("Invalid")));
    }
}

