using Antifraud.Worker.Models;

using Microsoft.EntityFrameworkCore;

namespace Antifraud.Worker.Persistence;
public class AntifraudDbContext(DbContextOptions<AntifraudDbContext> options) : DbContext(options)
{
    public DbSet<DailyTransactionLimit> DailyLimits => Set<DailyTransactionLimit>();
}

