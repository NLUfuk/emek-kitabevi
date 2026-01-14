using EmekKitabevi.Domain.Enums;

namespace EmekKitabevi.Domain.Entities;

public class Transaction : BaseEntity
{
    public TransactionType TransactionType { get; set; }
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Book Book { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
