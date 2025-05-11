using Antifraud.Domain.Entity;

namespace Antifraud.Application.Ports;
public interface IDailyLimitRepository
{
    Task<DailyTransactionLimit?> GetByUserAndDateAsync(Guid userId, DateOnly date);
    Task AddOrUpdateAsync(Guid userId, decimal amount);
}
