using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class ContaBancariaRepository : BaseRepository<ContaBancaria>, IContaBancariaRepository
{
    public ContaBancariaRepository(FinanceDbContext context) : base(context)
    {

    }
}