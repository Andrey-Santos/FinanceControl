using Microsoft.EntityFrameworkCore;
using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class FaturaRepository : BaseRepository<Fatura>, IFaturaRepository
{
    public FaturaRepository(FinanceDbContext context) : base(context)
    {
        
    }

    public Task<Fatura?> GetByCartaoEFaturaAsync(long cartaoId, int mes, int ano)
    {
        return _dbSet.Where(f => f.CartaoId == cartaoId && f.Mes == mes && f.Ano == ano)
            .FirstOrDefaultAsync();
    }

}
