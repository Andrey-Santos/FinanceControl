using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.ContaBancaria;
using FinanceControl.Core.Application.DTOs.ContaBancaria;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Financecontrol.WebApi.Controllers.Web;

public class ContaBancariaController : Controller
{
    private readonly ContaBancariaUseCase _useCase;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;
    private readonly IBancoRepository _bancoRepository;

    public ContaBancariaController(ContaBancariaUseCase useCase, IBancoRepository bancoRepository, ICategoriaTransacaoRepository categoriaTransacaoRepository)
    {
        _useCase = useCase;
        _bancoRepository = bancoRepository;
        _categoriaTransacaoRepository = categoriaTransacaoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _useCase.GetAllAsync());
    }

    private async Task LoadLists()
    {
        ViewBag.Bancos = new SelectList(await _bancoRepository.GetAllAsync(), "Id", "Nome");
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContaBancariaCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var bancos = await _bancoRepository.GetAllAsync();
            ViewBag.Bancos = new SelectList(bancos, "Id", "Nome");
            return View(dto);
        }

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        dto.UsuarioId = long.Parse(userIdClaim.Value);

        await _useCase.AddAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var entity = await _useCase.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var dto = new ContaBancariaUpdateDto
        {
            Numero = entity.Numero,
            BancoId = entity.BancoId,
        };

        await LoadLists();

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContaBancariaUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var bancos = await _bancoRepository.GetAllAsync();
            ViewBag.Bancos = new SelectList(bancos, "Id", "Nome");
            return View(dto);
        }

        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        dto.UsuarioId = long.Parse(userIdClaim.Value);

        await _useCase.UpdateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _useCase.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
