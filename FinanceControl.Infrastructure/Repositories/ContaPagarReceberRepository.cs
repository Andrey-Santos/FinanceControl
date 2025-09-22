using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class ContaPagarReceberRepository : BaseRepository<ContaPagarReceber>, IContaPagarReceberRepository
{
    public ContaPagarReceberRepository(FinanceDbContext context) : base(context) { }
}