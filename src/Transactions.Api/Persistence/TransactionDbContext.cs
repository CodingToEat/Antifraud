using Microsoft.EntityFrameworkCore;

using Transactions.Api.Models;

namespace Transactions.Api.Persistence;

public class TransactionDbContext(DbContextOptions<TransactionDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
}