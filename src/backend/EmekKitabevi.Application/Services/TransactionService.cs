using Microsoft.EntityFrameworkCore;
using EmekKitabevi.Application.DTOs.Books;
using EmekKitabevi.Application.DTOs.Transactions;
using EmekKitabevi.Domain.Entities;
using EmekKitabevi.Domain.Enums;
using EmekKitabevi.Domain.Interfaces;
using EmekKitabevi.Infrastructure.Data;

namespace EmekKitabevi.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly IRepository<Transaction> _transactionRepository;
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<StockMovement> _stockMovementRepository;
    private readonly ApplicationDbContext _context;

    public TransactionService(
        IRepository<Transaction> transactionRepository,
        IRepository<Book> bookRepository,
        IRepository<StockMovement> stockMovementRepository,
        ApplicationDbContext context)
    {
        _transactionRepository = transactionRepository;
        _bookRepository = bookRepository;
        _stockMovementRepository = stockMovementRepository;
        _context = context;
    }

    public async Task<PagedResult<TransactionDto>> GetAllAsync(TransactionSearchRequest request)
    {
        var query = _context.Transactions
            .Include(t => t.Book)
            .Include(t => t.Creator)
            .AsQueryable();

        if (request.BookId.HasValue)
        {
            query = query.Where(t => t.BookId == request.BookId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.TransactionType))
        {
            var transactionType = Enum.Parse<TransactionType>(request.TransactionType);
            query = query.Where(t => t.TransactionType == transactionType);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(t => t.TransactionDate <= request.EndDate.Value);
        }

        var totalCount = await query.CountAsync();

        var transactions = await query
            .OrderByDescending(t => t.TransactionDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResult<TransactionDto>
        {
            Items = transactions.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<TransactionDto?> GetByIdAsync(Guid id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Book)
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction == null) return null;

        return MapToDto(transaction);
    }

    public async Task<TransactionDto> CreateSaleAsync(CreateSaleRequest request, Guid userId)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if (book == null || !book.IsActive)
            throw new KeyNotFoundException("Kitap bulunamadı");

        if (book.StockQuantity < request.Quantity)
            throw new InvalidOperationException($"Yetersiz stok. Mevcut stok: {book.StockQuantity}");

        var totalAmount = request.UnitPrice * request.Quantity;

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Sale,
            BookId = request.BookId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = totalAmount,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = userId,
            Notes = request.Notes,
            IsActive = true
        };

        await _transactionRepository.AddAsync(transaction);

        // Stok güncelleme
        var previousStock = book.StockQuantity;
        book.StockQuantity -= request.Quantity;
        book.UpdatedAt = DateTime.UtcNow;
        await _bookRepository.UpdateAsync(book);

        // Stok hareketi kaydı
        var stockMovement = new StockMovement
        {
            BookId = book.Id,
            MovementType = StockMovementType.Out,
            Quantity = request.Quantity,
            PreviousStock = previousStock,
            NewStock = book.StockQuantity,
            Reason = $"Satış işlemi - İşlem No: {transaction.Id}",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _stockMovementRepository.AddAsync(stockMovement);

        return MapToDto(transaction);
    }

    public async Task<TransactionDto> CreatePurchaseAsync(CreatePurchaseRequest request, Guid userId)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if (book == null || !book.IsActive)
            throw new KeyNotFoundException("Kitap bulunamadı");

        var totalAmount = request.UnitPrice * request.Quantity;

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Purchase,
            BookId = request.BookId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = totalAmount,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = userId,
            Notes = request.Notes,
            IsActive = true
        };

        await _transactionRepository.AddAsync(transaction);

        // Stok güncelleme
        var previousStock = book.StockQuantity;
        book.StockQuantity += request.Quantity;
        book.UpdatedAt = DateTime.UtcNow;
        await _bookRepository.UpdateAsync(book);

        // Stok hareketi kaydı
        var stockMovement = new StockMovement
        {
            BookId = book.Id,
            MovementType = StockMovementType.In,
            Quantity = request.Quantity,
            PreviousStock = previousStock,
            NewStock = book.StockQuantity,
            Reason = $"Alış işlemi - İşlem No: {transaction.Id}",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _stockMovementRepository.AddAsync(stockMovement);

        return MapToDto(transaction);
    }

    public async Task<TransactionDto> CreateReturnAsync(CreateReturnRequest request, Guid userId)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if (book == null || !book.IsActive)
            throw new KeyNotFoundException("Kitap bulunamadı");

        var totalAmount = request.UnitPrice * request.Quantity;

        var transaction = new Transaction
        {
            TransactionType = TransactionType.Return,
            BookId = request.BookId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice,
            TotalAmount = totalAmount,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = userId,
            Notes = request.Notes,
            IsActive = true
        };

        await _transactionRepository.AddAsync(transaction);

        // Stok güncelleme (iade = stok artışı)
        var previousStock = book.StockQuantity;
        book.StockQuantity += request.Quantity;
        book.UpdatedAt = DateTime.UtcNow;
        await _bookRepository.UpdateAsync(book);

        // Stok hareketi kaydı
        var stockMovement = new StockMovement
        {
            BookId = book.Id,
            MovementType = StockMovementType.In,
            Quantity = request.Quantity,
            PreviousStock = previousStock,
            NewStock = book.StockQuantity,
            Reason = $"İade işlemi - İşlem No: {transaction.Id}",
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _stockMovementRepository.AddAsync(stockMovement);

        return MapToDto(transaction);
    }

    private TransactionDto MapToDto(Transaction transaction)
    {
        return new TransactionDto
        {
            Id = transaction.Id,
            TransactionType = transaction.TransactionType.ToString(),
            BookId = transaction.BookId,
            BookTitle = transaction.Book.Title,
            Quantity = transaction.Quantity,
            UnitPrice = transaction.UnitPrice,
            TotalAmount = transaction.TotalAmount,
            TransactionDate = transaction.TransactionDate,
            CreatedBy = transaction.Creator?.FullName ?? "Bilinmiyor",
            Notes = transaction.Notes
        };
    }
}
