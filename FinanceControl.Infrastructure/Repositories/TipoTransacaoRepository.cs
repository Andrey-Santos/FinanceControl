using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class TipoTransacaoRepository : BaseRepository<TipoTransacao>, ITipoTransacaoRepository
{
    public TipoTransacaoRepository(FinanceDbContext context) : base(context) { }
}
