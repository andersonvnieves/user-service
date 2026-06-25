using br.com.fiap.cloudgames.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace br.com.fiap.cloudgames.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _context.SaveChangesAsync();
        if (_transaction != null)

            await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
            await _transaction.RollbackAsync();
    }
}