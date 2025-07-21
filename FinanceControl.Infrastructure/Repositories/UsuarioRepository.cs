using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(FinanceDbContext context) : base(context) { }

    public async Task<Usuario?> GetByUsernameAsync(string nome)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Nome == nome);
    }
}
