using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class GalponController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<GalponController> _logger;

    public GalponController(ApplicationDbContext context, ILogger<GalponController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim?.Value, out int userId) ? userId : 0;
    }

    public async Task<IActionResult> Index()
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener galpones");
            TempData["Error"] = "Error al cargar los galpones";
            return RedirectToAction("Error", "Home");
        }
    }

    public IActionResult Create()
    {
        return View(new GalponViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GalponViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var existe = await _context.Galpones
                .AnyAsync(g => g.Nombre.ToLower() == model.Nombre.ToLower());

            if (existe)
            {
                ModelState.AddModelError(nameof(model.Nombre), "Ya existe un galpon con este nombre");
                return View(model);
            }

            var galpon = new Galpon
            {
                Nombre = model.Nombre.Trim(),
                Ubicacion = model.Ubicacion?.Trim() ?? "",
                Activo = model.Activo
            };

            _context.Galpones.Add(galpon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Galpon creado exitosamente";
            _logger.LogInformation($"Galpon creado - Nombre: {model.Nombre}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear galpon");
            ModelState.AddModelError("", "Error al crear el galpon: " + ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar edición de galpon");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GalponViewModel model)
    {
        try
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            var galpon = await _context.Galpones.FindAsync(id);

            if (galpon == null)
                return NotFound();

            galpon.Nombre = model.Nombre.Trim();
            galpon.Ubicacion = model.Ubicacion?.Trim() ?? "";
            galpon.Activo = model.Activo;

            _context.Galpones.Update(galpon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Galpon actualizado exitosamente";
            _logger.LogInformation($"Galpon actualizado - Nombre: {galpon.Nombre}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar galpon");
            ModelState.AddModelError("", "Error al actualizar el galpon: " + ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var galpon = await _context.Galpones.FindAsync(id);

            if (galpon == null)
                return NotFound();

            var tieneJaulas = await _context.Jaulas
                .AnyAsync(j => j.GalponId == id);

            if (tieneJaulas)
            {
                TempData["Error"] = "No se puede eliminar un galpon que contiene jaulas";
                return RedirectToAction(nameof(Index));
            }

            _context.Galpones.Remove(galpon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Galpon eliminado exitosamente";
            _logger.LogInformation($"Galpon eliminado - Nombre: {galpon.Nombre}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar galpon");
            TempData["Error"] = "Error al eliminar el galpon: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de galpon");
            return RedirectToAction("Error", "Home");
        }
    }
}
