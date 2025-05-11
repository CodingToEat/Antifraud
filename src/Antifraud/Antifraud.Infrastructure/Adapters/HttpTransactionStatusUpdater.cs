using System.Net.Http.Json;

using Antifraud.Application.Ports;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Antifraud.Infrastructure.Adapters;

public class HttpTransactionStatusUpdater(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<HttpTransactionStatusUpdater> logger) : ITransactionStatusUpdater
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<HttpTransactionStatusUpdater> _logger = logger;

    public async Task UpdateTransactionStatusAsync(Guid transactionId, string status)
    {
        var baseUrl = _configuration["TransactionsApi:BaseUrl"] ?? throw new InvalidOperationException("Base URL not configured.");
        var client = _httpClientFactory.CreateClient();

        var response = await client.PutAsJsonAsync(
            $"{baseUrl}/transactions/{transactionId}/status",
            new { status });

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("API responded with {StatusCode} for transaction {Id}", response.StatusCode, transactionId);
            response.EnsureSuccessStatusCode();
        }
    }
}

