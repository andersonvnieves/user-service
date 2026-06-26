namespace br.com.fiap.cloudgames.Users.Application.UnitsOfWork;

public interface IUnitOfWork
{
    Task BeginTransactionAsync();

    Task CommitAsync();

    Task RollbackAsync();
}