using FinanceControl.Core.Domain.Entities;

namespace FinanceControl.Core.Domain.Interfaces;

public interface ITransacaoRepository : IBaseRepository<Transacao>
{
    IQueryable<Transacao> GetAll();
}