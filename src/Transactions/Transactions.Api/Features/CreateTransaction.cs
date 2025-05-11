using Transactions.Api.Persistence;
using Transactions.Api.Services;

namespace Transactions.Api.Features;

public record CreateTransactionRequest
(
    Guid SourceAccountId,
    Guid TargetAccountId,
    int TransferTypeId,
    decimal Value
);

public class CreateTransactionHandler(ITransactionRepository repository, IKafkaProducerService kafka)
{
    private readonly ITransactionRepository _repository = repository;
    private readonly IKafkaProducerService _kafka = kafka;

    public async Task<IResult> HandleAsync(CreateTransactionRequest request)
    {
        var transaction = await _repository.CreateAsync(request);
        await _kafka.PublishTransactionCreatedAsync(transaction);
        return Results.Ok(new { transaction.TransactionExternalId, transaction.CreatedAt });
    }
}
