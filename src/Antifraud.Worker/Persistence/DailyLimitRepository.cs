using Antifraud.Worker.Models;

using Microsoft.EntityFrameworkCore;

namespace Antifraud.Worker.Persistence;

public interface IDailyLimitRepository
{
    Task<DailyTransactionLimit?> GetByUserAndDateAsync(Guid userId, DateOnly date);
    Task AddOrUpdateAsync(Guid userId, decimal amount);
}

public class DailyLimitRepository(AntifraudDbContext context) : IDailyLimitRepository
{
    private readonly AntifraudDbContext _context = context;

    public async Task<DailyTransactionLimit?> GetByUserAndDateAsync(Guid userId, DateOnly date)
    {
        return await _context.DailyLimits
            .FirstOrDefaultAsync(l => l.UserId == userId && l.Date == date);
    }

    public async Task AddOrUpdateAsync(Guid userId, decimal amount)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var existing = await GetByUserAndDateAsync(userId, today);

        if (existing is null)
        {
            _context.DailyLimits.Add(new DailyTransactionLimit
            {
                UserId = userId,
                Date = today,
                TotalAmount = amount
            });
        }
        else
        {
            existing.TotalAmount += amount;
        }

        await _context.SaveChangesAsync();
    }
}
