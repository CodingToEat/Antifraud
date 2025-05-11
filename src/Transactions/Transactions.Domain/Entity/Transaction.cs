using System.ComponentModel.DataAnnotations;

namespace Transactions.Domain.Entity;

public class Transaction
{
    public Guid TransactionId { get; set; } = Guid.NewGuid();
    public Guid TransactionExternalId { get; set; } = Guid.NewGuid();
    public Guid SourceAccountId { get; set; }
    public Guid TargetAccountId { get; set; }
    public int TransferTypeId { get; set; }
    public decimal Value { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public TransactionStatus Status { get; private set; } = TransactionStatus.Pending;

    public void SetStatus(TransactionStatus newStatus)
    {
        if (!Enum.IsDefined(typeof(TransactionStatus), newStatus))
            throw new ArgumentException("Invalid transaction status.");

        Status = newStatus;
    }
}

public enum TransactionStatus
{
    Pending,
    Approved,
    Rejected
}