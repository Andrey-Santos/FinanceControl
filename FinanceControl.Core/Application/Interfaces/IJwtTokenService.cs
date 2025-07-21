using FinanceControl.Core.Domain.Entities;

namespace FinanceControl.Core.Domain.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(Usuario usuario);
}
