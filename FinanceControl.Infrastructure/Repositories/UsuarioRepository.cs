using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(FinanceDbContext context) : base(context) { }
}
