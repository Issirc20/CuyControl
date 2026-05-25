using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class GalponController : Controller
{
    private readonly ApplicationDbContext _context;

    public GalponController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var galpones = await _context.Galpones
            .AsNoTracking()
            .Select(g => new GalponViewModel
            {
                Id = g.Id,
                Nombre = g.Nombre,
                Ubicacion = g.Ubicacion,
                Activo = g.Activo
            })
            .ToListAsync();

        return View(galpones);
    }

    public IActionResult Create()
    {
        return View(new GalponViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GalponViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var galpon = new Galpon
        {
            Nombre = model.Nombre,
            Ubicacion = model.Ubicacion,
            Activo = model.Activo
        };

        _context.Galpones.Add(galpon);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var galpon = await _context.Galpones.FindAsync(id);

        if (galpon == null)
            return NotFound();

        var model = new GalponViewModel
        {
            Id = galpon.Id,
            Nombre = galpon.Nombre,
            Ubicacion = galpon.Ubicacion,
            Activo = galpon.Activo
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GalponViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(model);

        var galpon = await _context.Galpones.FindAsync(id);

        if (galpon == null)
            return NotFound();

        galpon.Nombre = model.Nombre;
        galpon.Ubicacion = model.Ubicacion;
        galpon.Activo = model.Activo;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var galpon = await _context.Galpones.FindAsync(id);

        if (galpon == null)
            return NotFound();

        _context.Galpones.Remove(galpon);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var galpon = await _context.Galpones
            .FirstOrDefaultAsync(g => g.Id == id);

        if (galpon == null)
            return NotFound();

        var model = new GalponViewModel
        {
            Id = galpon.Id,
            Nombre = galpon.Nombre,
            Ubicacion = galpon.Ubicacion,
            Activo = galpon.Activo
        };

        return View(model);
    }
}