namespace FinanceControl.Core.Domain.Entities;

public class Usuario : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public ICollection<ContaBancaria> ContasBancarias { get; set; } = new List<ContaBancaria>();
}
