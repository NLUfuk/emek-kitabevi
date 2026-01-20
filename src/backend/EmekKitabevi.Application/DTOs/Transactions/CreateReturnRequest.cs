using System.ComponentModel.DataAnnotations;

namespace EmekKitabevi.Application.DTOs.Transactions;

public class CreateReturnRequest
{
    [Required(ErrorMessage = "Kitap ID gereklidir")]
    public Guid BookId { get; set; }

    [Required(ErrorMessage = "Miktar gereklidir")]
    [Range(1, int.MaxValue, ErrorMessage = "Miktar 1'den büyük olmalıdır")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Birim fiyat gereklidir")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
    public decimal UnitPrice { get; set; }

    public string? Notes { get; set; }
}
