using Antifraud.Worker.Persistence;

using Confluent.Kafka;
using Confluent.Kafka.Admin;

using Microsoft.EntityFrameworkCore;

namespace Antifraud.Worker;

public static class InfrastructureSetup
{
    public static async Task InitializeAsync(IHost host)
    {
        await ApplyMigrationsAsync(host);
        await EnsureKafkaTopicAsync(host);
    }

    private static async Task ApplyMigrationsAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AntifraudDbContext>();
        await db.Database.MigrateAsync();
    }

    private static async Task EnsureKafkaTopicAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("KafkaTopic");

        var configSection = host.Services.GetRequiredService<IConfiguration>();
        var bootstrapServers = configSection.GetSection("Kafka")["BootstrapServers"] ?? "localhost:9092";

        var config = new AdminClientConfig { BootstrapServers = bootstrapServers };

        using var adminClient = new AdminClientBuilder(config).Build();

        try
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            if (!metadata.Topics.Any(t => t.Topic == "transactions-created"))
            {
                await adminClient.CreateTopicsAsync(new[]
                {
                new TopicSpecification
                {
                    Name = "transactions-created",
                    NumPartitions = 1,
                    ReplicationFactor = 1
                }
            });
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create Kafka topic.");
        }
    }
}
