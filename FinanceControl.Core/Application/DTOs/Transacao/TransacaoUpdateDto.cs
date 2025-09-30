namespace FinanceControl.Core.Application.DTOs.Transacao;

public class TransacaoUpdateDto : TransacaoCreateDto
{
    public long Id { get; set; }

    public TransacaoUpdateDto(Domain.Entities.Transacao model) : base(model)
    {
        Id = model.Id;
    }

    public TransacaoUpdateDto(TransacaoResponseDto model) : base(model)
    {
        Id = model.Id;
    }
}