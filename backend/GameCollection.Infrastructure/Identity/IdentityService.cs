using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GameCollection.Application.Common.Interfaces;
using GameCollection.Application.DTOs.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GameCollection.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;

    public IdentityService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<(bool Succeeded, AuthResponse? Response, string[] Errors)> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByNameAsync(request.Username) ?? await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return (false, null, new[] { "Username or Email is already registered." });
        }

        var user = new IdentityUser
        {
            UserName = request.Username,
            Email = request.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return (false, null, result.Errors.Select(e => e.Description).ToArray());
        }

        // Check if role "User" exists, if not create it
        if (!await _roleManager.RoleExistsAsync("User"))
        {
            await _roleManager.CreateAsync(new IdentityRole("User"));
        }

        // Assign default role "User"
        await _userManager.AddToRoleAsync(user, "User");

        // Generate Token
        var token = await GenerateJwtTokenAsync(user);

        return (true, new AuthResponse
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = new List<string> { "User" }
        }, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, AuthResponse? Response, string[] Errors)> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UsernameOrEmail) ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);
        if (user == null)
        {
            return (false, null, new[] { "Invalid username/email or password." });
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return (false, null, new[] { "Invalid username/email or password." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtTokenAsync(user);

        return (true, new AuthResponse
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles.ToList()
        }, Array.Empty<string>());
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileDto
        {
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles.ToList()
        };
    }

    public async Task<(bool Succeeded, string[] Errors)> UpdateUserProfileAsync(string userId, UpdateProfileDto request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return (false, new[] { "User not found." });
        }

        // Email update
        if (user.Email != request.Email)
        {
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            if (emailExists != null && emailExists.Id != userId)
            {
                return (false, new[] { "Email is already taken." });
            }
            user.Email = request.Email;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return (false, updateResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        // Password update if requested
        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
            {
                return (false, new[] { "Current password is required to change password." });
            }

            var passwordResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!passwordResult.Succeeded)
            {
                return (false, passwordResult.Errors.Select(e => e.Description).ToArray());
            }
        }

        return (true, Array.Empty<string>());
    }

    public async Task<(bool Succeeded, string[] Errors)> ResetPasswordAsync(PasswordResetRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UsernameOrEmail) ?? await _userManager.FindByEmailAsync(request.UsernameOrEmail);
        if (user == null)
        {
            return (false, new[] { "User not found." });
        }

        // In a real application, password reset involves token validation sent via email.
        // For a local development/personal project, we will reset it directly for the user's convenience.
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded)
        {
            return (false, result.Errors.Select(e => e.Description).ToArray());
        }

        return (true, Array.Empty<string>());
    }

    private async Task<string> GenerateJwtTokenAsync(IdentityUser user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"] ?? "SuperSecretKeyForGameCollectionManagementAppKeyHere_1234567890!");
        var issuer = _configuration["JwtSettings:Issuer"] ?? "GameCollectionAPI";
        var audience = _configuration["JwtSettings:Audience"] ?? "GameCollectionApp";
        var durationMinutes = Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"] ?? "1440");

        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(durationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
