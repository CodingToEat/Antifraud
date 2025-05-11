using Transactions.Domain.Entity;

namespace Transactions.Application.Ports;

public interface ITransactionRepository
{
    Task CreateAsync(Transaction transaction);
    Task<Transaction?> GetByExternalIdAsync(Guid externalId);
    Task SaveChangesAsync();
}
