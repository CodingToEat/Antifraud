namespace Antifraud.Application.Ports;

public interface ITransactionStatusUpdater
{
    Task UpdateTransactionStatusAsync(Guid transactionId, string status);
}
