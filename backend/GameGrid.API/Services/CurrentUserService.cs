using System.Security.Claims;
using GameGrid.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GameGrid.API.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Maps sub claim from JWT which contains the Identity UserId
    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
