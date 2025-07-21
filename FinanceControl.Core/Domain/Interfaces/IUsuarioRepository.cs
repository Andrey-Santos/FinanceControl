using FinanceControl.Core.Domain.Entities;

namespace FinanceControl.Core.Domain.Interfaces;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{
    Task<Usuario?> GetByUsernameAsync(string nome);
}