using System.Threading.Tasks;
using GameCollection.Application.DTOs.Auth;

namespace GameCollection.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, AuthResponse? Response, string[] Errors)> RegisterAsync(RegisterRequest request);
    Task<(bool Succeeded, AuthResponse? Response, string[] Errors)> LoginAsync(LoginRequest request);
    Task<UserProfileDto?> GetUserProfileAsync(string userId);
    Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(string userId, UpdateProfileDto request);
    Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(PasswordResetRequest request);
}
