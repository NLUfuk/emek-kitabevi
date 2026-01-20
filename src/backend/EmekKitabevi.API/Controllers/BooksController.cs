using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmekKitabevi.Application.DTOs.Books;
using EmekKitabevi.Application.Services;

namespace EmekKitabevi.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookService bookService, ILogger<BooksController> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BookSearchRequest request)
    {
        try
        {
            var result = await _bookService.GetAllAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitaplar listelenirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null)
                return NotFound(new { message = "Kitap bulunamadı" });

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap getirilirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var book = await _bookService.CreateAsync(request, userId);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap oluşturulurken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBookRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = await _bookService.UpdateAsync(id, request);
            return Ok(book);
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
            _logger.LogError(ex, "Kitap güncellenirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap silinirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] BookSearchRequest request)
    {
        try
        {
            var result = await _bookService.SearchAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kitap arama sırasında hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpPut("{id}/price")]
    public async Task<IActionResult> UpdatePrice(Guid id, [FromBody] UpdatePriceRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var book = await _bookService.UpdatePriceAsync(id, request, userId);
            return Ok(book);
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
            _logger.LogError(ex, "Fiyat güncellenirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            var book = await _bookService.UpdateStockAsync(id, request, userId);
            return Ok(book);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Stok güncellenirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpGet("{id}/price-history")]
    public async Task<IActionResult> GetPriceHistory(Guid id)
    {
        try
        {
            var history = await _bookService.GetPriceHistoryAsync(id);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fiyat geçmişi getirilirken hata oluştu");
            return StatusCode(500, new { message = "Bir hata oluştu" });
        }
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockBooks()
    {
        try
        {
            var books = await _bookService.GetLowStockBooksAsync();
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Düşük stoklu kitaplar getirilirken hata oluştu");
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
