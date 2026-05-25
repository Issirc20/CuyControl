using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class ControlPesoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ControlPesoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var controles = await _context.ControlesPeso
            .Include(c => c.Cuy)
            .OrderByDescending(c => c.FechaPesaje)
            .Select(c => new ControlPesoViewModel
            {
                Id = c.Id,
                CuyId = c.CuyId,
                CuyCodigo = c.Cuy.Codigo,
                Peso = c.Peso,
                FechaPesaje = c.FechaPesaje,
                Observaciones = c.Observaciones
            })
            .ToListAsync();

        return View(controles);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Cuyes = new SelectList(
            await _context.Cuyes.ToListAsync(),
            "Id",
            "Codigo");

        return View(new ControlPesoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ControlPesoViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cuyes = new SelectList(
                await _context.Cuyes.ToListAsync(),
                "Id",
                "Codigo");

            return View(model);
        }

        var control = new ControlPeso
        {
            CuyId = model.CuyId,
            Peso = model.Peso,
            FechaPesaje = model.FechaPesaje,
            Observaciones = model.Observaciones,
            FechaCreacion = DateTime.Now,
            UsuarioCreacionId = 1
        };

        _context.ControlesPeso.Add(control);

        var cuy = await _context.Cuyes.FindAsync(model.CuyId);
        if (cuy != null)
            cuy.PesoActual = model.Peso;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var control = await _context.ControlesPeso.FindAsync(id);

        if (control == null)
            return NotFound();

        ViewBag.Cuyes = new SelectList(
            await _context.Cuyes.ToListAsync(),
            "Id",
            "Codigo",
            control.CuyId);

        var model = new ControlPesoViewModel
        {
            Id = control.Id,
            CuyId = control.CuyId,
            Peso = control.Peso,
            FechaPesaje = control.FechaPesaje,
            Observaciones = control.Observaciones
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ControlPesoViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Cuyes = new SelectList(
                await _context.Cuyes.ToListAsync(),
                "Id",
                "Codigo",
                model.CuyId);

            return View(model);
        }

        var control = await _context.ControlesPeso.FindAsync(id);

        if (control == null)
            return NotFound();

        control.CuyId = model.CuyId;
        control.Peso = model.Peso;
        control.FechaPesaje = model.FechaPesaje;
        control.Observaciones = model.Observaciones;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var control = await _context.ControlesPeso
            .Include(c => c.Cuy)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (control == null)
            return NotFound();

        var model = new ControlPesoViewModel
        {
            Id = control.Id,
            CuyId = control.CuyId,
            CuyCodigo = control.Cuy.Codigo,
            Peso = control.Peso,
            FechaPesaje = control.FechaPesaje,
            Observaciones = control.Observaciones
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var control = await _context.ControlesPeso.FindAsync(id);

        if (control == null)
            return NotFound();

        _context.ControlesPeso.Remove(control);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}