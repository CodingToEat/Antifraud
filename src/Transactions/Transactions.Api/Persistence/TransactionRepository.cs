using Microsoft.EntityFrameworkCore;

using Transactions.Api.Features;
using Transactions.Api.Models;

namespace Transactions.Api.Persistence;

public interface ITransactionRepository
{
    Task<Transaction> CreateAsync(CreateTransactionRequest req);
    Task<Transaction?> GetByExternalIdAsync(Guid externalId);
    Task UpdateStatusAsync(Guid externalId, string newStatus);
}

public class TransactionRepository(TransactionDbContext db) : ITransactionRepository
{
    private readonly TransactionDbContext _db = db;

    public async Task<Transaction> CreateAsync(CreateTransactionRequest req)
    {
        var tx = new Transaction
        {
            SourceAccountId = req.SourceAccountId,
            TargetAccountId = req.TargetAccountId,
            TransferTypeId = req.TransferTypeId,
            Value = req.Value
        };
        _db.Transactions.Add(tx);
        await _db.SaveChangesAsync();
        return tx;
    }

    public async Task<Transaction?> GetByExternalIdAsync(Guid externalId) =>
        await _db.Transactions.FirstOrDefaultAsync(t => t.TransactionExternalId == externalId);

    public async Task UpdateStatusAsync(Guid externalId, string newStatus)
    {
        var tx = await GetByExternalIdAsync(externalId);
        if (tx != null)
        {
            tx.Status = newStatus;
            await _db.SaveChangesAsync();
        }
    }
}
