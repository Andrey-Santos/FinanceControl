using System.ComponentModel.DataAnnotations;

namespace FinanceControl.Core.Domain.Enums;

public enum TipoCartao
{
    [Display(Name = "Débito")]
    Debito = 1,

    [Display(Name = "Crédito")]
    Credito = 2
}