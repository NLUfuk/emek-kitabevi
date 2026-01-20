using System.ComponentModel.DataAnnotations;

namespace EmekKitabevi.Application.DTOs.Books;

public class UpdateStockRequest
{
    [Required(ErrorMessage = "Miktar gereklidir")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Hareket tipi gereklidir")]
    public string MovementType { get; set; } = string.Empty; // "In", "Out", "Adjustment"

    [Required(ErrorMessage = "Neden gereklidir")]
    [MaxLength(500, ErrorMessage = "Neden en fazla 500 karakter olabilir")]
    public string Reason { get; set; } = string.Empty;
}
