using Transactions.Application.Ports;
using Transactions.Domain.Dto;
using Transactions.Domain.Entity;

namespace Transactions.Application.Commands;

public class CreateTransactionHandler(ITransactionRepository repository, ITransactionEventProducer kafka)
{
    private readonly ITransactionRepository _repository = repository;
    private readonly ITransactionEventProducer _kafka = kafka;

    public async Task<CreateTransactionResponse> HandleAsync(CreateTransactionRequest request)
    {
        var transaction = new Transaction
        {
            SourceAccountId = request.SourceAccountId,
            TargetAccountId = request.TargetAccountId,
            TransferTypeId = request.TransferTypeId,
            Value = request.Value
        };

        var transactionCreatedMessage = new TransactionCreatedMessage(
            transaction.TransactionExternalId, 
            transaction.SourceAccountId,
            transaction.Value);
        
        await _repository.CreateAsync(transaction);
        await _kafka.PublishTransactionCreatedAsync(transactionCreatedMessage);
        
        return new CreateTransactionResponse(transaction.TransactionExternalId, transaction.CreatedAt);
    }
}
