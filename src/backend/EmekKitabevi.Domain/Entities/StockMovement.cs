using EmekKitabevi.Domain.Enums;

namespace EmekKitabevi.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid BookId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string Reason { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Book Book { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
