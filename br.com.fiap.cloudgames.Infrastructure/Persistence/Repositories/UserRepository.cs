using br.com.fiap.cloudgames.Domain.Aggregates;
using br.com.fiap.cloudgames.Domain.Repositories;
using br.com.fiap.cloudgames.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace br.com.fiap.cloudgames.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<User>  _users;

    public UserRepository(AppDbContext context)
    {
        _context = context;
        _users = _context.Set<User>();
    }
    
    public async Task AddAsync(User user)
    {
        await _users.AddAsync(user);
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        return await _users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public void Update(User user)
    {
        _users.Update(user);
    }

    public async Task<User> GetByIdentityIdAsync(string identityId)
    {
        return await _users.FirstOrDefaultAsync(u => u.IdentityId == identityId);
    }
}