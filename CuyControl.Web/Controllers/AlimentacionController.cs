using CuyControl.Application.Interfaces;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class AlimentacionController : Controller
{
    private readonly IAlimentacionService _service;
    private readonly ILogger<AlimentacionController> _logger;
    private readonly ApplicationDbContext _context;

    public AlimentacionController(IAlimentacionService service, ILogger<AlimentacionController> logger, ApplicationDbContext context)
    {
        _service = service;
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _service.ObtenerTodasAlimentacionesAsync();
        var vm = items.Select(a => new AlimentacionViewModel
        {
            Id = a.Id,
            JaulaId = a.JaulaId,
            JaulaCodigo = a.JaulaId > 0 ? _context.Jaulas.Find(a.JaulaId)?.Codigo : "",
            UsuarioId = a.UsuarioId,
            FechaAlimentacion = a.FechaAlimentacion,
            CantidadAlimento = a.CantidadAlimento,
            TipoAlimento = a.TipoAlimento,
            Observaciones = a.Observaciones
        }).ToList();

        return View(vm);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Jaulas = new SelectList(await _context.Jaulas.ToListAsync(), "Id", "Codigo");
        return View(new AlimentacionViewModel { FechaAlimentacion = DateTime.Now });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AlimentacionViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Jaulas = new SelectList(await _context.Jaulas.ToListAsync(), "Id", "Codigo", vm.JaulaId);
            return View(vm);
        }

        var dto = new CuyControl.Application.DTOs.AlimentacionDto
        {
            JaulaId = vm.JaulaId,
            UsuarioId = vm.UsuarioId,
            FechaAlimentacion = vm.FechaAlimentacion,
            CantidadAlimento = vm.CantidadAlimento,
            TipoAlimento = vm.TipoAlimento,
            Observaciones = vm.Observaciones
        };

        await _service.CrearAlimentacionAsync(dto);
        TempData["Mensaje"] = "Alimentación registrada";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.ObtenerAlimentacionPorIdAsync(id);
        if (item == null) return NotFound();

        ViewBag.Jaulas = new SelectList(await _context.Jaulas.ToListAsync(), "Id", "Codigo", item.JaulaId);

        var vm = new AlimentacionViewModel
        {
            Id = item.Id,
            JaulaId = item.JaulaId,
            UsuarioId = item.UsuarioId,
            FechaAlimentacion = item.FechaAlimentacion,
            CantidadAlimento = item.CantidadAlimento,
            TipoAlimento = item.TipoAlimento,
            Observaciones = item.Observaciones
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AlimentacionViewModel vm)
    {
        if (id != vm.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Jaulas = new SelectList(await _context.Jaulas.ToListAsync(), "Id", "Codigo", vm.JaulaId);
            return View(vm);
        }

        var dto = new CuyControl.Application.DTOs.AlimentacionDto
        {
            Id = vm.Id,
            JaulaId = vm.JaulaId,
            UsuarioId = vm.UsuarioId,
            FechaAlimentacion = vm.FechaAlimentacion,
            CantidadAlimento = vm.CantidadAlimento,
            TipoAlimento = vm.TipoAlimento,
            Observaciones = vm.Observaciones
        };

        await _service.ActualizarAlimentacionAsync(id, dto);
        TempData["Mensaje"] = "Alimentación actualizada";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.EliminarAlimentacionAsync(id);
        TempData["Mensaje"] = "Registro eliminado";
        return RedirectToAction(nameof(Index));
    }
}