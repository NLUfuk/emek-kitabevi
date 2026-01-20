using EmekKitabevi.Application.DTOs.Books;
using EmekKitabevi.Domain.Entities;

namespace EmekKitabevi.Application.Services;

public interface IBookService
{
    Task<PagedResult<BookDto>> GetAllAsync(BookSearchRequest request);
    Task<BookDto?> GetByIdAsync(Guid id);
    Task<BookDto> CreateAsync(CreateBookRequest request, Guid userId);
    Task<BookDto> UpdateAsync(Guid id, UpdateBookRequest request);
    Task DeleteAsync(Guid id);
    Task<PagedResult<BookDto>> SearchAsync(BookSearchRequest request);
    Task<BookDto> UpdatePriceAsync(Guid id, UpdatePriceRequest request, Guid userId);
    Task<BookDto> UpdateStockAsync(Guid id, UpdateStockRequest request, Guid userId);
    Task<List<PriceHistoryDto>> GetPriceHistoryAsync(Guid bookId);
    Task<List<BookDto>> GetLowStockBooksAsync();
}
