using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;

namespace FinanceControl.Infrastructure.Repositories;

public class CategoriaTransacaoRepository : BaseRepository<CategoriaTransacao>, ICategoriaTransacaoRepository
{
    public CategoriaTransacaoRepository(FinanceDbContext context) : base(context) { }
}
