using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class CategoriaTransacaoRepository : BaseRepository<CategoriaTransacao>, ICategoriaTransacaoRepository
{
    public CategoriaTransacaoRepository(FinanceDbContext context) : base(context) { }

    public async Task<CategoriaTransacao?> GetByNomeAsync(string Nome)
    {
        return await _context.CategoriasTransacao.FirstOrDefaultAsync(c => c.Nome.ToLower() == Nome.ToLower());
    }
}
