using EmekKitabevi.Domain.Enums;

namespace EmekKitabevi.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    // Navigation properties
    public ICollection<Book> CreatedBooks { get; set; } = new List<Book>();
    public ICollection<PriceHistory> PriceChanges { get; set; } = new List<PriceHistory>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
