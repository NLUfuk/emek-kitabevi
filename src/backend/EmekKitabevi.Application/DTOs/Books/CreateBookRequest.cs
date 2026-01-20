using System.ComponentModel.DataAnnotations;

namespace EmekKitabevi.Application.DTOs.Books;

public class CreateBookRequest
{
    public string? ISBN { get; set; }
    public string? Barcode { get; set; }

    [Required(ErrorMessage = "Kitap adı gereklidir")]
    [MaxLength(500, ErrorMessage = "Kitap adı en fazla 500 karakter olabilir")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(300, ErrorMessage = "Yazar adı en fazla 300 karakter olabilir")]
    public string Author { get; set; } = string.Empty;

    [MaxLength(200, ErrorMessage = "Yayınevi adı en fazla 200 karakter olabilir")]
    public string Publisher { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Kategori adı en fazla 100 karakter olabilir")]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Fiyat gereklidir")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
    public decimal CurrentPrice { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stok miktarı negatif olamaz")]
    public int StockQuantity { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Minimum stok seviyesi negatif olamaz")]
    public int MinStockLevel { get; set; } = 5;

    public string? Description { get; set; }
}
