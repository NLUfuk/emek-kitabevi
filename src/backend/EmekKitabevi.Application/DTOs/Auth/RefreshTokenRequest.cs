using System.ComponentModel.DataAnnotations;

namespace EmekKitabevi.Application.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Refresh token gereklidir")]
    public string RefreshToken { get; set; } = string.Empty;
}
