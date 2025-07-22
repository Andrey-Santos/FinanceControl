
using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FinanceControl.Core.Application.UseCases;


public abstract class BaseUseCase
{
    protected async Task ValidarEntidadeExistenteAsync<TEntity>(IBaseRepository<TEntity> repository, long id, string nomeEntidade) where TEntity : EntityBase
    {
        if (id == 0 || await repository.GetByIdAsync(id) == null)
            throw new InvalidOperationException($"{nomeEntidade} não encontrada.");
    }
}