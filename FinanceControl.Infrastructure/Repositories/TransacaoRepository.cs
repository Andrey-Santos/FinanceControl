using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class TransacaoRepository : BaseRepository<Transacao>, ITransacaoRepository
{
    public TransacaoRepository(FinanceDbContext context) : base(context) { }

    public IQueryable<Transacao> GetAll()
    {
        return _context.Transacoes.AsQueryable();
    }
}
