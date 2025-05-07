using System.Net;

using Antifraud.Worker.Models;
using Antifraud.Worker.Persistence;
using Antifraud.Worker.Services;

using Microsoft.Extensions.Logging;

using Moq;

namespace Antifraud.Worker.Tests;

public class FraudDetectionServiceTests
{
    private readonly Mock<IDailyLimitRepository> _repoMock = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
    private readonly Mock<ILogger<FraudDetectionService>> _loggerMock = new();

    private FraudDetectionService CreateService(HttpResponseMessage response, decimal existingTotal)
    {
        _repoMock.Setup(r => r.GetByUserAndDateAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>()))
                 .ReturnsAsync(new DailyTransactionLimit { TotalAmount = existingTotal });

        var httpClientHandler = new MockHttpMessageHandler(response);
        var client = new HttpClient(httpClientHandler);

        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        return new FraudDetectionService(_repoMock.Object, _httpClientFactoryMock.Object, _loggerMock.Object, new ConfigurationStub());
    }

    [Fact]
    public async Task Rejects_When_OverSingleLimit()
    {
        var transaction = new Transaction { Value = 5000, TransactionExternalId = Guid.NewGuid() };
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), 0);

        await service.EvaluateAsync(transaction);

        _repoMock.Verify(r => r.AddOrUpdateAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task Rejects_When_OverDailyLimit()
    {
        var transaction = new Transaction { Value = 1500, TransactionExternalId = Guid.NewGuid(), SourceAccountId = Guid.NewGuid() };
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), 19000);

        await service.EvaluateAsync(transaction);

        _repoMock.Verify(r => r.AddOrUpdateAsync(It.IsAny<Guid>(), It.IsAny<decimal>()), Times.Never);
    }

    [Fact]
    public async Task Approves_And_Updates_When_UnderLimits()
    {
        var transaction = new Transaction { Value = 1000, TransactionExternalId = Guid.NewGuid(), SourceAccountId = Guid.NewGuid() };
        var service = CreateService(new HttpResponseMessage(HttpStatusCode.OK), 1000);

        await service.EvaluateAsync(transaction);

        _repoMock.Verify(r => r.AddOrUpdateAsync(transaction.SourceAccountId, transaction.Value), Times.Once);
    }
}
