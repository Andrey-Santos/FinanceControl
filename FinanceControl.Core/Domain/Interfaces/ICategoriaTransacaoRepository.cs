using FinanceControl.Core.Domain.Entities;

namespace FinanceControl.Core.Domain.Interfaces;

public interface ICategoriaTransacaoRepository : IBaseRepository<CategoriaTransacao>
{
    Task<CategoriaTransacao?> GetByNomeAsync(string Nome);
}
