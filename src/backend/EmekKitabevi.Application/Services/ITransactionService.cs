using EmekKitabevi.Application.DTOs.Books;
using EmekKitabevi.Application.DTOs.Transactions;

namespace EmekKitabevi.Application.Services;

public interface ITransactionService
{
    Task<PagedResult<TransactionDto>> GetAllAsync(TransactionSearchRequest request);
    Task<TransactionDto?> GetByIdAsync(Guid id);
    Task<TransactionDto> CreateSaleAsync(CreateSaleRequest request, Guid userId);
    Task<TransactionDto> CreatePurchaseAsync(CreatePurchaseRequest request, Guid userId);
    Task<TransactionDto> CreateReturnAsync(CreateReturnRequest request, Guid userId);
}
