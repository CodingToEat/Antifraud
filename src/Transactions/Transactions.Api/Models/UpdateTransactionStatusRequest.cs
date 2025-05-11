namespace Transactions.Api.Models;

public class UpdateTransactionStatusRequest
{
    private static readonly HashSet<string> AllowedStatuses = ["Approved", "Rejected"];

    private string _status = default!;

    public string Status
    {
        get => _status;
        set
        {
            if (!AllowedStatuses.Contains(value))
                throw new ArgumentException($"Invalid status '{value}'. Allowed: {string.Join(", ", AllowedStatuses)}");

            _status = value;
        }
    }
}
