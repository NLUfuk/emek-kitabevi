namespace EmekKitabevi.Domain.Entities;

public class Book : BaseEntity
{
    public string? ISBN { get; set; }
    public string? Barcode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; } = 5;
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }

    // Navigation properties
    public User Creator { get; set; } = null!;
    public ICollection<PriceHistory> PriceHistory { get; set; } = new List<PriceHistory>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
