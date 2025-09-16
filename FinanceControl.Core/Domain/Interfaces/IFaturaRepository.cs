using FinanceControl.Core.Domain.Entities;

namespace FinanceControl.Core.Domain.Interfaces;

public interface IFaturaRepository : IBaseRepository<Fatura>
{
    Task<Fatura?> GetByCartaoEFaturaAsync(long cartaoId, int mes, int ano);
}