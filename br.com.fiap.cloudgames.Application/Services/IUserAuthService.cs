namespace br.com.fiap.cloudgames.Application.Services;

public interface IUserAuthService
{
    Task<string> CreateUserAsync(string email, string password, string role);
    Task<string> AuthenticateUserAsync(string email, string password);
    bool IsUserAuthenticatedAsync();
    IEnumerable<string> GetAuthenticatedUserRolesAsync();
    
    Task ReplaceUserRoleAsync(string identityUserId, string role);
}