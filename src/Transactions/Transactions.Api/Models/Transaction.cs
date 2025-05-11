using System.ComponentModel.DataAnnotations;

namespace Transactions.Api.Models;

public class Transaction
{
    [Key]
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public Guid TransactionExternalId { get; set; } = Guid.NewGuid();
    public Guid SourceAccountId { get; set; }
    public Guid TargetAccountId { get; set; }
    public int TransferTypeId { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "pending";
}
