using System.Collections.Generic;

namespace GameCollection.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class AuthResponse
{
    public string Token { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsSuperAdmin { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

public class UserProfileDto
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsSuperAdmin { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}

public class UpdateProfileDto
{
    public string Email { get; set; } = null!;
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}

public class PasswordResetRequest
{
    public string UsernameOrEmail { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
