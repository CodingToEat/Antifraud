using Moq;

using Transactions.Application.Commands;
using Transactions.Application.Ports;
using Transactions.Domain.Dto;
using Transactions.Domain.Entity;

namespace Transactions.Api.Tests;

public class CreateTransactionHandlerTests
{
    private readonly Mock<ITransactionRepository> _repoMock = new();
    private readonly Mock<ITransactionEventProducer> _producerMock = new();

    [Fact]
    public async Task CreatesTransaction_AndPublishesEvent_ReturnsResponse()
    {
        // Arrange
        var sourceId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        var request = new CreateTransactionRequest(sourceId, targetId, 1, 1000);

        var handler = new CreateTransactionHandler(_repoMock.Object, _producerMock.Object);

        // Act
        var result = await handler.HandleAsync(request);

        // Assert
        _repoMock.Verify(r => r.CreateAsync(It.IsAny<Transaction>()), Times.Once);
        _producerMock.Verify(p => p.PublishTransactionCreatedAsync(It.IsAny<TransactionCreatedMessage>()), Times.Once);

        Assert.NotEqual(Guid.Empty, result.TransactionExternalId);
        Assert.True(result.CreatedAt <= DateTime.UtcNow);
    }
}


