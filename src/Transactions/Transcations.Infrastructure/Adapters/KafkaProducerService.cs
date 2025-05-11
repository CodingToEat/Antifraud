using System.Text.Json;

using Confluent.Kafka;

using Microsoft.Extensions.Configuration;

using Transactions.Application.Ports;
using Transactions.Domain.Dto;

namespace Transcations.Infrastructure.Adapters;

public class KafkaProducerService : ITransactionEventProducer
{
    private readonly IProducer<Null, string> _producer;
    private const string Topic = "transactions-created";

    public KafkaProducerService(IConfiguration configuration)
    {
        var config = new ProducerConfig { BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092" };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishTransactionCreatedAsync(TransactionCreatedMessage transaction)
    {
        var json = JsonSerializer.Serialize(transaction);
        await _producer.ProduceAsync(Topic, new Message<Null, string> { Value = json });
    }
}
