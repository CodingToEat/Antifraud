namespace Antifraud.Worker.Models;
public class DailyTransactionLimit
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public decimal TotalAmount { get; set; }
}
