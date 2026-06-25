namespace br.com.fiap.cloudgames.Application.UnitsOfWork;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();
}