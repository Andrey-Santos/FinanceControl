using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class CartaoRepository : BaseRepository<Cartao>, ICartaoRepository
{
    public CartaoRepository(FinanceDbContext context) : base(context)
    {
    }
}