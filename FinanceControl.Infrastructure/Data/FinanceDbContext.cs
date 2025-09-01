using Microsoft.EntityFrameworkCore;
using FinanceControl.Core.Domain.Entities;

namespace FinanceControl.Infrastructure.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<ContaBancaria> ContasBancarias => Set<ContaBancaria>();
    public DbSet<Banco> Bancos => Set<Banco>();
    public DbSet<Cartao> Cartoes => Set<Cartao>();
    public DbSet<Fatura> Faturas => Set<Fatura>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();
    public DbSet<CategoriaTransacao> CategoriasTransacao => Set<CategoriaTransacao>();
    public DbSet<ContaPagarReceber> ContaPagarReceber => Set<ContaPagarReceber>();

}
