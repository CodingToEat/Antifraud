using Antifraud.Application.Ports;
using Antifraud.Domain.Dto;
using Antifraud.Domain.Entity;
using Antifraud.Domain.Enums;

using Microsoft.Extensions.Logging;

namespace Antifraud.Application.Services;

public interface IFraudDetectionService
{
    Task EvaluateAsync(TransactionCreatedMessage transaction);
}

public class FraudDetectionService(IDailyLimitRepository limitRepository, ITransactionStatusUpdater statusUpdater, ILogger<FraudDetectionService> logger) : IFraudDetectionService
{
    private readonly IDailyLimitRepository _limitRepository = limitRepository;
    private readonly ITransactionStatusUpdater _statusUpdater = statusUpdater;
    private readonly ILogger<FraudDetectionService> _logger = logger;

    public async Task EvaluateAsync(TransactionCreatedMessage transaction)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var current = await _limitRepository.GetByUserAndDateAsync(transaction.SourceAccountId, today)
                      ?? new DailyTransactionLimit { UserId = transaction.SourceAccountId, Date = today };

        var status = current.CanAccept(transaction.Value) ? TransactionStatus.Approved : TransactionStatus.Rejected;

        if (status == TransactionStatus.Approved)
        {
            await _limitRepository.AddOrUpdateAsync(transaction.SourceAccountId, transaction.Value);
        }

        try
        {
            await _statusUpdater.UpdateTransactionStatusAsync(transaction.TransactionExternalId, status.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update transaction status for {TransactionId}", transaction.TransactionExternalId);
        }
    }
}
