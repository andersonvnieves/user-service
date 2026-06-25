using br.com.fiap.cloudgames.Domain.Aggregates;

namespace br.com.fiap.cloudgames.Application.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user); 
}