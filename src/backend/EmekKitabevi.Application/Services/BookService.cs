using Microsoft.EntityFrameworkCore;
using EmekKitabevi.Application.DTOs.Books;
using EmekKitabevi.Domain.Entities;
using EmekKitabevi.Domain.Enums;
using EmekKitabevi.Domain.Interfaces;
using EmekKitabevi.Infrastructure.Data;

namespace EmekKitabevi.Application.Services;

public class BookService : IBookService
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<PriceHistory> _priceHistoryRepository;
    private readonly IRepository<StockMovement> _stockMovementRepository;
    private readonly ApplicationDbContext _context;

    public BookService(
        IRepository<Book> bookRepository,
        IRepository<User> userRepository,
        IRepository<PriceHistory> priceHistoryRepository,
        IRepository<StockMovement> stockMovementRepository,
        ApplicationDbContext context)
    {
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _priceHistoryRepository = priceHistoryRepository;
        _stockMovementRepository = stockMovementRepository;
        _context = context;
    }

    public async Task<PagedResult<BookDto>> GetAllAsync(BookSearchRequest request)
    {
        return await SearchAsync(request);
    }

    public async Task<BookDto?> GetByIdAsync(Guid id)
    {
        var book = await _context.Books
            .Include(b => b.Creator)
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);

        if (book == null) return null;

        return MapToDto(book);
    }

    public async Task<BookDto> CreateAsync(CreateBookRequest request, Guid userId)
    {
        // ISBN ve Barcode unique kontrolü
        if (!string.IsNullOrEmpty(request.ISBN))
        {
            var existingISBN = await _bookRepository.ExistsAsync(b => b.ISBN == request.ISBN && b.IsActive);
            if (existingISBN)
                throw new InvalidOperationException("Bu ISBN numarası zaten kullanılıyor");
        }

        if (!string.IsNullOrEmpty(request.Barcode))
        {
            var existingBarcode = await _bookRepository.ExistsAsync(b => b.Barcode == request.Barcode && b.IsActive);
            if (existingBarcode)
                throw new InvalidOperationException("Bu barkod numarası zaten kullanılıyor");
        }

        var book = new Book
        {
            ISBN = request.ISBN,
            Barcode = request.Barcode,
            Title = request.Title,
            Author = request.Author,
            Publisher = request.Publisher,
            Category = request.Category,
            CurrentPrice = request.CurrentPrice,
            StockQuantity = request.StockQuantity,
            MinStockLevel = request.MinStockLevel,
            Description = request.Description,
            CreatedBy = userId,
            IsActive = true
        };

        await _bookRepository.AddAsync(book);

        // İlk fiyat geçmişi kaydı
        var priceHistory = new PriceHistory
        {
            BookId = book.Id,
            OldPrice = 0,
            NewPrice = book.CurrentPrice,
            ChangedBy = userId,
            ChangeReason = "İlk kayıt",
            ChangedAt = DateTime.UtcNow
        };
        await _priceHistoryRepository.AddAsync(priceHistory);

        return MapToDto(book);
    }

    public async Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null || !book.IsActive)
            throw new KeyNotFoundException("Kitap bulunamadı");

        // ISBN ve Barcode unique kontrolü (kendisi hariç)
        if (!string.IsNullOrEmpty(request.ISBN) && request.ISBN != book.ISBN)
        {
            var existingISBN = await _bookRepository.ExistsAsync(b => b.ISBN == request.ISBN && b.Id != id && b.IsActive);
            if (existingISBN)
                throw new InvalidOperationException("Bu ISBN numarası zaten kullanılıyor");
        }

        if (!string.IsNullOrEmpty(request.Barcode) && request.Barcode != book.Barcode)
        {
            var existingBarcode = await _bookRepository.ExistsAsync(b => b.Barcode == request.Barcode && b.Id != id && b.IsActive);
            if (existingBarcode)
                throw new InvalidOperationException("Bu barkod numarası zaten kullanılıyor");
        }

        book.ISBN = request.ISBN;
        book.Barcode = request.Barcode;
        book.Title = request.Title;
        book.Author = request.Author;
        book.Publisher = request.Publisher;
        book.Category = request.Category;
        book.Description = request.Description;
        book.MinStockLevel = request.MinStockLevel;
        book.UpdatedAt = DateTime.UtcNow;

        // Fiyat değiştiyse logla
        if (book.CurrentPrice != request.CurrentPrice)
        {
            var priceHistory = new PriceHistory
            {
                BookId = book.Id,
                OldPrice = book.CurrentPrice,
                NewPrice = request.CurrentPrice,
                ChangedBy = book.CreatedBy, // Güncelleme yapan kullanıcı bilgisi eklenebilir
                ChangeReason = "Güncelleme",
                ChangedAt = DateTime.UtcNow
            };
            await _priceHistoryRepository.AddAsync(priceHistory);
            book.CurrentPrice = request.CurrentPrice;
        }

        // Stok değiştiyse logla
        if (book.StockQuantity != request.StockQuantity)
        {
            var stockMovement = new StockMovement
            {
                BookId = book.Id,
                MovementType = StockMovementType.Adjustment,
                Quantity = Math.Abs(request.StockQuantity - book.StockQuantity),
                PreviousStock = book.StockQuantity,
                NewStock = request.StockQuantity,
                Reason = "Stok güncelleme",
                CreatedBy = book.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };
            await _stockMovementRepository.AddAsync(stockMovement);
            book.StockQuantity = request.StockQuantity;
        }

        await _bookRepository.UpdateAsync(book);
        return MapToDto(book);
    }

    public async Task DeleteAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null || !book.IsActive)
            throw new KeyNotFoundException("Kitap bulunamadı");

        book.IsActive = false;
        book.UpdatedAt = DateTime.UtcNow;
        await _bookRepository.UpdateAsync(book);
    }

    public async Task<PagedResult<BookDto>> SearchAsync(BookSearchRequest request)
    {
        var query = _context.Books
            .Include(b => b.Creator)
            .Where(b => b.IsActive)
            .AsQueryable();

        // Arama terimi (ISBN, Barcode veya Title)
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim();
            query = query.Where(b =>
                b.ISBN != null && b.ISBN.Contains(searchTerm) ||
                b.Barcode != null && b.Barcode.Contains(searchTerm) ||
                b.Title.Contains(searchTerm));
        }

        // Filtreler
        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            query = query.Where(b => b.Category == request.Category);
        }

        if (!string.IsNullOrWhiteSpace(request.Author))
        {
            query = query.Where(b => b.Author.Contains(request.Author));
        }

        if (request.LowStockOnly == true)
        {
            query = query.Where(b => b.StockQuantity <= b.MinStockLevel);
        }

        var totalCount = await query.CountAsync();

        var books = await query
            .OrderBy(b => b.Title)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResult<BookDto>
        {
            Items = books.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<BookDto> UpdatePriceAsync(Guid id, UpdatePriceRequest request, Guid userId)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null || !book.IsActive)
            throw new KeyNotFoundException("Kitap bulunamadı");

        if (book.CurrentPrice == request.NewPrice)
            throw new InvalidOperationException("Yeni fiyat mevcut fiyatla aynı");

        var oldPrice = book.CurrentPrice;
        book.CurrentPrice = request.NewPrice;
        book.UpdatedAt = DateTime.UtcNow;

        // Fiyat geçmişi kaydı
        var priceHistory = new PriceHistory
        {
            BookId = book.Id,
            OldPrice = oldPrice,
            NewPrice = request.NewPrice,
            ChangedBy = userId,
            ChangeReason = request.ChangeReason ?? "Fiyat güncelleme",
            ChangedAt = DateTime.UtcNow
        };
        await _priceHistoryRepository.AddAsync(priceHistory);
        await _bookRepository.UpdateAsync(book);

        return MapToDto(book);
    }

    public async Task<BookDto> UpdateStockAsync(Guid id, UpdateStockRequest request, Guid userId)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null || !book.IsActive)
            throw new KeyNotFoundException("Kitap bulunamadı");

        var previousStock = book.StockQuantity;
        var movementType = Enum.Parse<StockMovementType>(request.MovementType);

        int newStock;
        switch (movementType)
        {
            case StockMovementType.In:
                newStock = previousStock + request.Quantity;
                break;
            case StockMovementType.Out:
                newStock = previousStock - request.Quantity;
                if (newStock < 0)
                    throw new InvalidOperationException("Stok miktarı negatif olamaz");
                break;
            case StockMovementType.Adjustment:
                newStock = request.Quantity;
                break;
            default:
                throw new ArgumentException("Geçersiz hareket tipi");
        }

        book.StockQuantity = newStock;
        book.UpdatedAt = DateTime.UtcNow;

        // Stok hareketi kaydı
        var stockMovement = new StockMovement
        {
            BookId = book.Id,
            MovementType = movementType,
            Quantity = request.Quantity,
            PreviousStock = previousStock,
            NewStock = newStock,
            Reason = request.Reason,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _stockMovementRepository.AddAsync(stockMovement);
        await _bookRepository.UpdateAsync(book);

        return MapToDto(book);
    }

    public async Task<List<PriceHistoryDto>> GetPriceHistoryAsync(Guid bookId)
    {
        var history = await _context.PriceHistories
            .Include(ph => ph.Book)
            .Include(ph => ph.ChangedByUser)
            .Where(ph => ph.BookId == bookId)
            .OrderByDescending(ph => ph.ChangedAt)
            .ToListAsync();

        return history.Select(h => new PriceHistoryDto
        {
            Id = h.Id,
            BookId = h.BookId,
            BookTitle = h.Book.Title,
            OldPrice = h.OldPrice,
            NewPrice = h.NewPrice,
            ChangedBy = h.ChangedByUser.FullName,
            ChangeReason = h.ChangeReason,
            ChangedAt = h.ChangedAt
        }).ToList();
    }

    public async Task<List<BookDto>> GetLowStockBooksAsync()
    {
        var books = await _context.Books
            .Include(b => b.Creator)
            .Where(b => b.IsActive && b.StockQuantity <= b.MinStockLevel)
            .OrderBy(b => b.StockQuantity)
            .ToListAsync();

        return books.Select(MapToDto).ToList();
    }

    private BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            ISBN = book.ISBN,
            Barcode = book.Barcode,
            Title = book.Title,
            Author = book.Author,
            Publisher = book.Publisher,
            Category = book.Category,
            CurrentPrice = book.CurrentPrice,
            StockQuantity = book.StockQuantity,
            MinStockLevel = book.MinStockLevel,
            Description = book.Description,
            CreatedAt = book.CreatedAt,
            UpdatedAt = book.UpdatedAt,
            CreatedBy = book.Creator?.FullName ?? "Bilinmiyor",
            IsLowStock = book.StockQuantity <= book.MinStockLevel
        };
    }
}
