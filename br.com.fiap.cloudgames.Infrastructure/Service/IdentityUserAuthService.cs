using System.Security.Claims;
using br.com.fiap.cloudgames.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace br.com.fiap.cloudgames.Infrastructure.Service;

public class IdentityUserAuthService : IUserAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string LOG_IN_ERROR_MESSAGE = "Email or Password invalid";

    public IdentityUserAuthService(UserManager<IdentityUser> userManager,  IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<string> CreateUserAsync(string email, string password, String role)
    {
        var identityUser = new IdentityUser()
        {
            UserName = email,
            Email = email,
        };
        
        var result = await _userManager.CreateAsync(identityUser, password);
        if(!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ApplicationException($"Error creating user: {errors}");
        }
        
        return identityUser.Id;
    }

    public async Task<string> AuthenticateUserAsync(string email, string password)
    {
        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser == null)
            throw new ApplicationException(LOG_IN_ERROR_MESSAGE);

        var isPasswordValid = await _userManager.CheckPasswordAsync(identityUser, password);
        if (!isPasswordValid)
            throw new ApplicationException(LOG_IN_ERROR_MESSAGE);
        
        return identityUser.Id;
    }

    public bool IsUserAuthenticatedAsync()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public IEnumerable<string> GetAuthenticatedUserRolesAsync()
    {
        return _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(x => x.Value) ?? [];
    }

    public async Task ReplaceUserRoleAsync(string identityUserId, string role)
    {
        var user = await _userManager.FindByIdAsync(identityUserId);
        if (user == null)
            throw new ApplicationException("User not found");
        
        var currentRole = await _userManager.GetRolesAsync(user);
        if (currentRole.Any())
        { 
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRole);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new ApplicationException($"Error removing roles: {errors}");
            }
        }
        
        var addResult = await _userManager.AddToRoleAsync(user, role);
        if (!addResult.Succeeded)
        {
            var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
            throw new ApplicationException($"Error creating roles: {errors}");
        }
    }
}