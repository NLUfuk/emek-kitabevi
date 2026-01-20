using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmekKitabevi.Application.DTOs.Transactions;
using EmekKitabevi.Application.Services;

namespace EmekKitabevi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionsController> _logger;

    public TransactionsController(ITransactionService transactionService, ILogger<TransactionsController> logger)
    {
        _transactionService = transactionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TransactionSearchRequest request)
    {
        try
        {
            var result = await _transactionService.GetAllAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İşlemler listelenirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var transaction = await _transactionService.GetByIdAsync(id);
            if (transaction == null)
                return NotFound(new { message = "İşlem bulunamadı" });

            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İşlem getirilirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpPost("sale")]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var transaction = await _transactionService.CreateSaleAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Satış işlemi oluşturulurken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> CreatePurchase([FromBody] CreatePurchaseRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var transaction = await _transactionService.CreatePurchaseAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Alış işlemi oluşturulurken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpPost("return")]
    public async Task<IActionResult> CreateReturn([FromBody] CreateReturnRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var transaction = await _transactionService.CreateReturnAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İade işlemi oluşturulurken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Geçersiz kullanıcı");
        }
        return userId;
    }
}
