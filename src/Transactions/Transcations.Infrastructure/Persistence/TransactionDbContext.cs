using Microsoft.EntityFrameworkCore;

using Transactions.Domain.Entity;

namespace Transcations.Infrastructure.Persistence;

public class TransactionDbContext(DbContextOptions<TransactionDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
}