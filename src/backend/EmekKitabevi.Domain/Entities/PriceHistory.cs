namespace EmekKitabevi.Domain.Entities;

public class PriceHistory : BaseEntity
{
    public Guid BookId { get; set; }
    public decimal OldPrice { get; set; }
    public decimal NewPrice { get; set; }
    public Guid ChangedBy { get; set; }
    public string? ChangeReason { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Book Book { get; set; } = null!;
    public User ChangedByUser { get; set; } = null!;
}
