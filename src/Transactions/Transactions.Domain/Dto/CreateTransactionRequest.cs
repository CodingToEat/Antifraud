namespace Transactions.Domain.Dto;

public record CreateTransactionRequest(
    Guid SourceAccountId,
    Guid TargetAccountId,
    int TransferTypeId,
    decimal Value
);