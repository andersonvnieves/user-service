using br.com.fiap.cloudgames.Domain.Aggregates;

namespace br.com.fiap.cloudgames.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User> GetByIdentityIdAsync(string identityId);
    Task<User> GetUserByIdAsync(Guid userId);
    void Update(User user);
}