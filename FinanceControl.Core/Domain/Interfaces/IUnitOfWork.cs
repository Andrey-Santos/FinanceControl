namespace FinanceControl.Core.Domain.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync();
}
