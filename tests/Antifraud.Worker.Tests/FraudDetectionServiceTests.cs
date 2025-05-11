using System.Net;

using Antifraud.Application.Ports;
using Antifraud.Application.Services;
using Antifraud.Domain.Dto;
using Antifraud.Domain.Entity;

using Microsoft.Extensions.Logging;

using Moq;

namespace Antifraud.Worker.Tests;

public class FraudDetectionServiceTests
{
    private readonly Mock<IDailyLimitRepository> _repoMock = new();
    private readonly Mock<ITransactionStatusUpdater> _statusUpdaterMock = new();
    private readonly Mock<ILogger<FraudDetectionService>> _loggerMock = new();

    private FraudDetectionService CreateService(HttpResponseMessage response, decimal existingTotal)
    {
        _repoMock.Setup(r => r.GetByUserAndDateAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                 .ReturnsAsync(new DailyTransactionLimit { TotalAmount = existingTotal });

        return new FraudDetectionService(_repoMock.Object, _statusUpdaterMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Rejects_When_OverSingleLimit()
    {
        var transaction = new TransactionCreatedMessage(Guid.NewGuid(), Guid.NewGuid(), 5000);
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), 0);

        await service.EvaluateAsync(transaction);

        _repoMock.Verify(r => r.AddOrUpdateAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task Rejects_When_OverDailyLimit()
    {
        var transaction = new TransactionCreatedMessage(Guid.NewGuid(), Guid.NewGuid(), 1500);
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), 19000);

        await service.EvaluateAsync(transaction);

        _repoMock.Verify(r => r.AddOrUpdateAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task Approves_And_Updates_When_UnderLimits()
    {
        var transaction = new TransactionCreatedMessage(Guid.NewGuid(), Guid.NewGuid(), 1000);
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), 1000);

        await service.EvaluateAsync(transaction);

        _repoMock.Verify(r => r.AddOrUpdateAsync(transaction.SourceAccountId, transaction.Value), Times.Once);
    }
}
