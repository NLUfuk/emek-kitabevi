namespace EmekKitabevi.Application.DTOs.Transactions;

public class TransactionDto
{
    public Guid Id { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
