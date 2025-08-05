namespace FinanceControl.Core.Domain.Entities;

public class EntityBase
{
    public long Id { get; set; }

    public DateTime DataCadastro { get; set; }

    public DateTime DataAlteracao { get; set; }
}