using Antifraud.Domain.Entity;

using Microsoft.EntityFrameworkCore;

namespace Antifraud.Infrastructure.Persistence;
public class AntifraudDbContext(DbContextOptions<AntifraudDbContext> options) : DbContext(options)
{
    public DbSet<DailyTransactionLimit> DailyLimits => Set<DailyTransactionLimit>();
}

