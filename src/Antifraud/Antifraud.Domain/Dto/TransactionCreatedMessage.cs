namespace Antifraud.Domain.Dto;

public record TransactionCreatedMessage(
    Guid TransactionExternalId,
    Guid SourceAccountId,
    decimal Value
);
