using System.Net.Http.Json;

using Antifraud.Worker.Models;
using Antifraud.Worker.Persistence;

namespace Antifraud.Worker.Services;

public interface IFraudDetectionService
{
    Task EvaluateAsync(Transaction transaction);
}

public class FraudDetectionService(IDailyLimitRepository limitRepository, IHttpClientFactory httpClientFactory, ILogger<FraudDetectionService> logger, IConfiguration config) : IFraudDetectionService
{
    private readonly IDailyLimitRepository _limitRepository = limitRepository;
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<FraudDetectionService> _logger = logger;
    private readonly string _transactionsApiBaseUrl = config["TransactionsApi:BaseUrl"]!;

    private const decimal DAILY_LIMIT = 20000;
    private const decimal SINGLE_LIMIT = 2000;

    public async Task EvaluateAsync(Transaction transaction)
    {
        var client = _httpClientFactory.CreateClient();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var current = await _limitRepository.GetByUserAndDateAsync(transaction.SourceAccountId, today);
        var currentTotal = current?.TotalAmount ?? 0m;

        string status;

        if (transaction.Value > SINGLE_LIMIT)
        {
            status = "Rejected";
        }
        else if ((currentTotal + transaction.Value) > DAILY_LIMIT)
        {
            status = "Rejected";
        }
        else
        {
            status = "Approved";
            await _limitRepository.AddOrUpdateAsync(transaction.SourceAccountId, transaction.Value);
        }

        try
        {
            var result = await client.PutAsJsonAsync(
                $"{_transactionsApiBaseUrl}/transactions/{transaction.TransactionExternalId}/status",
                new { status });

            result.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update transaction status for {TransactionId}", transaction.TransactionExternalId);
        }
    }
}

