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

    private readonly IBancoRepository _bancoRepository;

    public ContaBancariaController(ContaBancariaUseCase useCase, IBancoRepository bancoRepository)
    {
        _useCase = useCase;
        _bancoRepository = bancoRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _useCase.GetAllAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var bancos = await _bancoRepository.GetAllAsync();
        ViewBag.Bancos = new SelectList(bancos, "Id", "Nome");
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

        var bancos = await _bancoRepository.GetAllAsync();
        ViewBag.Bancos = new SelectList(bancos, "Id", "Nome");

        var dto = new ContaBancariaUpdateDto
        {
            Numero = entity.Numero,
            BancoId = entity.BancoId,
        };

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
