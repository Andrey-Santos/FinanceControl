using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class FaturaRepository : BaseRepository<Fatura>, IFaturaRepository
{
    public FaturaRepository(FinanceDbContext context) : base(context)
    {
    }
}
