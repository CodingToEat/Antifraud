using System.Text.Json;

using Confluent.Kafka;

using Transactions.Api.Models;

namespace Transactions.Api.Services;

public interface IKafkaProducerService
{
    Task PublishTransactionCreatedAsync(Transaction transaction);
}

public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducer<Null, string> _producer;
    private const string Topic = "transactions-created";

    public KafkaProducerService(IConfiguration configuration)
    {
        var config = new ProducerConfig { BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092" };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishTransactionCreatedAsync(Transaction transaction)
    {
        var json = JsonSerializer.Serialize(transaction);
        await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = json });
    }
}
