using System.ComponentModel.DataAnnotations;

namespace EmekKitabevi.Application.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre gereklidir")]
    public string Password { get; set; } = string.Empty;
}
