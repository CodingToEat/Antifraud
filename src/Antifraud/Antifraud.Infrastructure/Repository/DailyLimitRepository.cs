using Antifraud.Application.Ports;
using Antifraud.Domain.Entity;
using Antifraud.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Antifraud.Infrastructure.Repository;

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
