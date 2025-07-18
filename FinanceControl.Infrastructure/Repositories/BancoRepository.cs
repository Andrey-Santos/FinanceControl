using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class BancoRepository : BaseRepository<Banco>, IBancoRepository
{
    public BancoRepository(DbContext context) : base(context)
    {
    }
}