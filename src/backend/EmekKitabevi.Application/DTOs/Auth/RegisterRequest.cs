using System.ComponentModel.DataAnnotations;

namespace EmekKitabevi.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    [MinLength(3, ErrorMessage = "Kullanıcı adı en az 3 karakter olmalıdır")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta gereklidir")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre gereklidir")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ad Soyad gereklidir")]
    public string FullName { get; set; } = string.Empty;
}
