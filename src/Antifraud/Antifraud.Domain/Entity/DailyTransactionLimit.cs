namespace Antifraud.Domain.Entity;
public class DailyTransactionLimit
{
    private const decimal DAILY_LIMIT = 20000;
    private const decimal SINGLE_LIMIT = 2000;

    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public decimal TotalAmount { get; set; }

    public bool CanAccept(decimal amount)
    {
        return TotalAmount + amount <= DAILY_LIMIT && amount <= SINGLE_LIMIT;
    }

    public void ApplyTransaction(decimal value)
    {
        if (!CanAccept(value))
            throw new InvalidOperationException("Transaction exceeds limit.");

        TotalAmount += value;
    }
}
