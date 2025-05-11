using Microsoft.EntityFrameworkCore;

using Transactions.Domain.Entity;
using Transactions.Application.Ports;
using Transcations.Infrastructure.Persistence;

namespace Transcations.Infrastructure.Repository;


public class TransactionRepository(TransactionDbContext db) : ITransactionRepository
{
    private readonly TransactionDbContext _db = db;

    public async Task CreateAsync(Transaction transaction)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();
    }

    public async Task<Transaction?> GetByExternalIdAsync(Guid externalId) =>
        await _db.Transactions.FirstOrDefaultAsync(t => t.TransactionExternalId == externalId);

    public Task SaveChangesAsync()
    {
        return _db.SaveChangesAsync();
    }
}
