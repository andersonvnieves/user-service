using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using br.com.fiap.cloudgames.Application.Services;
using br.com.fiap.cloudgames.Domain.Aggregates;
using br.com.fiap.cloudgames.Infrastructure.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace br.com.fiap.cloudgames.Infrastructure.Service;

public class JwtTokenService : ITokenService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IOptions<JwtTokenSettings> _settings;

    public JwtTokenService(UserManager<IdentityUser> userManager, IOptions<JwtTokenSettings> settings)
    {
        _userManager = userManager;
        _settings = settings;
    }
    
    public async Task<string> GenerateTokenAsync(User user)
    {
        var identityUser = await _userManager.FindByIdAsync(user.IdentityId);
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, identityUser.Email)
        };
        
        var roles = await _userManager.GetRolesAsync(identityUser);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _settings.Value.Issuer,
            audience: _settings.Value.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_settings.Value.TokenTtlInMinutes),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}