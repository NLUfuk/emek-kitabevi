namespace EmekKitabevi.Application.DTOs.Books;

public class BookDto
{
    public Guid Id { get; set; }
    public string? ISBN { get; set; }
    public string? Barcode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinStockLevel { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public bool IsLowStock { get; set; }
}
