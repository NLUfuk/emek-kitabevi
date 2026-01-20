namespace EmekKitabevi.Application.DTOs.Books;

public class PriceHistoryDto
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public string ChangedBy { get; set; } = string.Empty;
    public string? ChangeReason { get; set; }
    public DateTime ChangedAt { get; set; }
}
