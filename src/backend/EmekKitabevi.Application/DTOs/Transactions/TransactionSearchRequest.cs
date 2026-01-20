namespace EmekKitabevi.Application.DTOs.Transactions;

public class TransactionSearchRequest
{
    public Guid? BookId { get; set; }
    public string? TransactionType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
