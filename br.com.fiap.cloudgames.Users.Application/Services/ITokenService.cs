using br.com.fiap.cloudgames.Users.Domain.Aggregates;

namespace br.com.fiap.cloudgames.Users.Application.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user); 
}