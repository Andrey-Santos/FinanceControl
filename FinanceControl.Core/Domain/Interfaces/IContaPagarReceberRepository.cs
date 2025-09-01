using FinanceControl.Core.Domain.Entities;

namespace FinanceControl.Core.Domain.Interfaces;

public interface IContaPagarReceberRepository : IBaseRepository<ContaPagarReceber>
{
    IQueryable<ContaPagarReceber> GetAll();
}
