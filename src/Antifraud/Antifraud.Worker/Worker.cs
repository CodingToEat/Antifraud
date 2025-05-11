using Antifraud.Worker.Models;
using Antifraud.Worker.Services;
using Confluent.Kafka;
using System.Text.Json;

namespace Antifraud.Worker;

public class Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration) : BackgroundService
{
    private readonly ILogger<Worker> _logger = logger;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly IConfiguration _configuration = configuration;
    private const string Topic = "transactions-created";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            GroupId = "antifraud-worker",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                if (cr is null) continue;

                var transaction = JsonSerializer.Deserialize<Transaction>(cr.Message.Value);
                if (transaction is null) continue;

                using var scope = _scopeFactory.CreateScope();
                var fraudService = scope.ServiceProvider.GetRequiredService<IFraudDetectionService>();

                await fraudService.EvaluateAsync(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Kafka message");
            }
        }
    }
}
