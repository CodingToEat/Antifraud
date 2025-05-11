using Transactions.Domain.Dto;

namespace Transactions.Application.Ports;

public interface ITransactionEventProducer
{
    Task PublishTransactionCreatedAsync(TransactionCreatedMessage transaction);
}
