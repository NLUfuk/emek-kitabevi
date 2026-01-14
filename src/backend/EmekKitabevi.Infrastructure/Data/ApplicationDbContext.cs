using Microsoft.EntityFrameworkCore;
using EmekKitabevi.Domain.Entities;

namespace EmekKitabevi.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<PriceHistory> PriceHistories { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.FullName).HasMaxLength(200);
            entity.Property(e => e.Role).HasConversion<int>();
        });

        // Book configuration
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ISBN).IsUnique().HasFilter("[ISBN] IS NOT NULL");
            entity.HasIndex(e => e.Barcode).IsUnique().HasFilter("[Barcode] IS NOT NULL");
            entity.HasIndex(e => e.Title);
            entity.Property(e => e.ISBN).HasMaxLength(20);
            entity.Property(e => e.Barcode).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Author).HasMaxLength(300);
            entity.Property(e => e.Publisher).HasMaxLength(200);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CurrentPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);
            entity.Property(e => e.MinStockLevel).HasDefaultValue(5);

            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.CreatedBooks)
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // PriceHistory configuration
        modelBuilder.Entity<PriceHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OldPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NewPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ChangeReason).HasMaxLength(500);
            entity.HasIndex(e => e.BookId);
            entity.HasIndex(e => e.ChangedAt);

            entity.HasOne(e => e.Book)
                  .WithMany(b => b.PriceHistory)
                  .HasForeignKey(e => e.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ChangedByUser)
                  .WithMany(u => u.PriceChanges)
                  .HasForeignKey(e => e.ChangedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionType).HasConversion<int>();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.BookId);

            entity.HasOne(e => e.Book)
                  .WithMany(b => b.Transactions)
                  .HasForeignKey(e => e.BookId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.Transactions)
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // StockMovement configuration
        modelBuilder.Entity<StockMovement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MovementType).HasConversion<int>();
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.HasIndex(e => e.BookId);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasOne(e => e.Book)
                  .WithMany(b => b.StockMovements)
                  .HasForeignKey(e => e.BookId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Creator)
                  .WithMany(u => u.StockMovements)
                  .HasForeignKey(e => e.CreatedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EntityType).HasMaxLength(100);
            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.ChangedAt);

            entity.HasOne(e => e.ChangedByUser)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(e => e.ChangedBy)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
