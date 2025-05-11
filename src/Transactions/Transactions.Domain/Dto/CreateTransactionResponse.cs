namespace Transactions.Domain.Dto;

public record CreateTransactionResponse(Guid TransactionExternalId, DateTime CreatedAt);