namespace EmekKitabevi.Application.DTOs.Books;

public class BookSearchRequest
{
    public string? SearchTerm { get; set; } // ISBN, Barcode veya Title için
    public string? Category { get; set; }
    public string? Author { get; set; }
    public bool? LowStockOnly { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
