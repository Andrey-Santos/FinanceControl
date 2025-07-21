using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Infrastructure.Repositories;

public class BancoRepository : BaseRepository<Banco>, IBancoRepository
{
    public BancoRepository(FinanceDbContext context) : base(context)
    {
    }
}