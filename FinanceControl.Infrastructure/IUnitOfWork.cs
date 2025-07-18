using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly FinanceDbContext _context;

    public UnitOfWork(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync() =>
        await _context.SaveChangesAsync();
}