using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmekKitabevi.Application.DTOs.Auth;
using EmekKitabevi.Domain.Entities;
using EmekKitabevi.Domain.Enums;
using EmekKitabevi.Domain.Interfaces;
using BCrypt.Net;

namespace EmekKitabevi.Application.Services;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IRepository<User> userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var users = await _userRepository.FindAsync(u => u.Username == request.Username || u.Email == request.Username);
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Kullanıcı adı veya şifre hatalı");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Hesabınız aktif değil");
        }

        return await GenerateTokenResponseAsync(user);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        // Kullanıcı adı kontrolü
        var existingUser = await _userRepository.FindAsync(u => u.Username == request.Username || u.Email == request.Email);
        if (existingUser.Any())
        {
            throw new InvalidOperationException("Bu kullanıcı adı veya e-posta zaten kullanılıyor");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Role = UserRole.User, // Varsayılan olarak User, ilk kullanıcı Admin olacak
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        return await GenerateTokenResponseAsync(user);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        // Basit refresh token implementasyonu - Production'da daha güvenli bir yöntem kullanılmalı
        // Şimdilik sadece token'ı validate edip yeni token üretiyoruz
        throw new NotImplementedException("Refresh token implementasyonu Faz 2'de tamamlanacak");
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("Kullanıcı bulunamadı");
        }

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString()
        };
    }

    private async Task<LoginResponse> GenerateTokenResponseAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey yapılandırılmamış"));

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "15")),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        // Basit refresh token (Production'da daha güvenli olmalı)
        var refreshToken = GenerateRefreshToken();

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = tokenDescriptor.Expires.Value,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            }
        };
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
