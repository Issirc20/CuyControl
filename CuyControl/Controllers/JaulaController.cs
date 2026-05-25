using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class JaulaController : Controller
{
    private readonly ApplicationDbContext _context;

    public JaulaController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var jaulas = await _context.Jaulas
            .Include(j => j.Galpon)
            .Select(j => new JaulaViewModel
            {
                Id = j.Id,
                Codigo = j.Codigo,
                Capacidad = j.Capacidad,
                GalponId = j.GalponId,
                GalponNombre = j.Galpon.Nombre,
                Activo = j.Disponible
            })
            .ToListAsync();

        return View(jaulas);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Galpones = new SelectList(
            await _context.Galpones.ToListAsync(),
            "Id",
            "Nombre");

        return View(new JaulaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JaulaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Galpones = new SelectList(
                await _context.Galpones.ToListAsync(),
                "Id",
                "Nombre");

            return View(model);
        }

        var jaula = new Jaula
        {
            Codigo = model.Codigo,
            Capacidad = model.Capacidad,
            GalponId = model.GalponId,
            Disponible = model.Activo,
            CantidadActual = 0,
            FechaCreacion = DateTime.Now,
            UsuarioCreacionId = 1
        };

        _context.Jaulas.Add(jaula);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var jaula = await _context.Jaulas.FindAsync(id);

        if (jaula == null)
            return NotFound();

        ViewBag.Galpones = new SelectList(
            await _context.Galpones.ToListAsync(),
            "Id",
            "Nombre",
            jaula.GalponId);

        var model = new JaulaViewModel
        {
            Id = jaula.Id,
            Codigo = jaula.Codigo,
            Capacidad = jaula.Capacidad,
            GalponId = jaula.GalponId,
            Activo = jaula.Disponible
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JaulaViewModel model)
    {
        if (id != model.Id)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            ViewBag.Galpones = new SelectList(
                await _context.Galpones.ToListAsync(),
                "Id",
                "Nombre",
                model.GalponId);

            return View(model);
        }

        var jaula = await _context.Jaulas.FindAsync(id);

        if (jaula == null)
            return NotFound();

        jaula.Codigo = model.Codigo;
        jaula.Capacidad = model.Capacidad;
        jaula.GalponId = model.GalponId;
        jaula.Disponible = model.Activo;
        jaula.FechaModificacion = DateTime.Now;
        jaula.UsuarioModificacionId = 1;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var jaula = await _context.Jaulas.FindAsync(id);

        if (jaula == null)
            return NotFound();

        _context.Jaulas.Remove(jaula);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}