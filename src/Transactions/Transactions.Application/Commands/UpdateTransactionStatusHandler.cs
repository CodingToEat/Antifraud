using Transactions.Application.Ports;
using Transactions.Domain.Dto;
using Transactions.Domain.Entity;

namespace Transactions.Application.Commands;
public class UpdateTransactionStatusHandler(ITransactionRepository repository)
{
    public async Task HandleAsync(Guid transactionId, UpdateTransactionStatusRequest request)
    {
        var transaction = await repository.GetByExternalIdAsync(transactionId) ?? throw new KeyNotFoundException("Transaction not found.");

        if (!Enum.TryParse<TransactionStatus>(request.Status, true, out var parsedStatus))
            throw new ArgumentException("Invalid status.");

        transaction.SetStatus(parsedStatus);

        await repository.SaveChangesAsync();
    }
}
