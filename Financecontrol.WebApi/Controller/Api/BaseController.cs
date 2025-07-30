using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BaseController<TEntity, TCreateDto, TResponseDto, TUseCase> : ControllerBase
                                                                            where TEntity : EntityBase
                                                                            where TUseCase : IBaseUseCase<TEntity, TCreateDto, TResponseDto>
{
    protected readonly TUseCase _useCase;

    public BaseController(TUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TResponseDto>>> GetAllAsync()
    {
        var result = await _useCase.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<TResponseDto>> GetByIdAsync(long id)
    {
        var result = await _useCase.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] TCreateDto dto)
    {
        await _useCase.AddAsync(dto);
        return Ok();
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAsync(long id, [FromBody] TCreateDto dto)
    {
        await _useCase.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        await _useCase.DeleteAsync(id);
        return NoContent();
    }
}
