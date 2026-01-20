using System.ComponentModel.DataAnnotations;

namespace EmekKitabevi.Application.DTOs.Books;

public class UpdatePriceRequest
{
    [Required(ErrorMessage = "Yeni fiyat gereklidir")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
    public decimal NewPrice { get; set; }

    [MaxLength(500, ErrorMessage = "Değişiklik nedeni en fazla 500 karakter olabilir")]
    public string? ChangeReason { get; set; }
}
