namespace FinanceControl.Core.Domain.Entities;

public class ContaBancaria : EntityBase
{
    public long UsuarioId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public long BancoId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public Banco Banco { get; set; } = null!;
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    public ICollection<Cartao> Cartoes { get; set; } = new List<Cartao>();
}
