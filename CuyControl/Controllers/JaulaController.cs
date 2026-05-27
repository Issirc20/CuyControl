using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class JaulaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<JaulaController> _logger;

    public JaulaController(ApplicationDbContext context, ILogger<JaulaController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el ID del usuario actual desde el contexto
    /// </summary>
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim?.Value, out int userId) ? userId : 0;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var jaulas = await _context.Jaulas
                .Include(j => j.Galpon)
                .Select(j => new JaulaViewModel
                {
                    Id = j.Id,
                    Codigo = j.Codigo,
                    Capacidad = j.Capacidad,
                    CantidadActual = j.CantidadActual,
                    GalponId = j.GalponId,
                    GalponNombre = j.Galpon.Nombre,
                    Activo = j.Disponible,
                    PorcentajeOcupacion = j.Capacidad > 0 ? Math.Round((decimal)j.CantidadActual / j.Capacidad * 100, 2) : 0
                })
                .ToListAsync();

            return View(jaulas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener jaulas");
            TempData["Error"] = "Error al cargar las jaulas";
            return RedirectToAction("Error", "Home");
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            ViewBag.Galpones = new SelectList(
                await _context.Galpones.ToListAsync(),
                "Id",
                "Nombre");

            return View(new JaulaViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar formulario de jaula");
            TempData["Error"] = "Error al preparar el formulario";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(JaulaViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Galpones = new SelectList(
                    await _context.Galpones.ToListAsync(),
                    "Id",
                    "Nombre");

                return View(model);
            }

            // Validar que capacidad sea positiva
            if (model.Capacidad <= 0)
            {
                ModelState.AddModelError(nameof(model.Capacidad), "La capacidad debe ser mayor a cero");
                ViewBag.Galpones = new SelectList(
                    await _context.Galpones.ToListAsync(),
                    "Id",
                    "Nombre");
                return View(model);
            }

            // Validar que el galpon existe
            var galpon = await _context.Galpones.FindAsync(model.GalponId);
            if (galpon == null)
            {
                ModelState.AddModelError(nameof(model.GalponId), "Galpon no encontrado");
                ViewBag.Galpones = new SelectList(
                    await _context.Galpones.ToListAsync(),
                    "Id",
                    "Nombre");
                return View(model);
            }

            // Validar que no existe una jaula con el mismo código
            var existe = await _context.Jaulas
                .AnyAsync(j => j.Codigo.ToLower() == model.Codigo.ToLower());

            if (existe)
            {
                ModelState.AddModelError(nameof(model.Codigo), "Ya existe una jaula con este código");
                ViewBag.Galpones = new SelectList(
                    await _context.Galpones.ToListAsync(),
                    "Id",
                    "Nombre");
                return View(model);
            }

            var jaula = new Jaula
            {
                Codigo = model.Codigo.Trim(),
                Capacidad = model.Capacidad,
                GalponId = model.GalponId,
                Disponible = model.Activo,
                CantidadActual = 0,
                FechaCreacion = DateTime.Now,
                UsuarioCreacionId = GetCurrentUserId()
            };

            _context.Jaulas.Add(jaula);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Jaula creada exitosamente";
            _logger.LogInformation($"Jaula creada - Código: {model.Codigo}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear jaula");
            ModelState.AddModelError("", "Error al crear la jaula: " + ex.Message);
            ViewBag.Galpones = new SelectList(
                await _context.Galpones.ToListAsync(),
                "Id",
                "Nombre");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
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
                CantidadActual = jaula.CantidadActual,
                GalponId = jaula.GalponId,
                Activo = jaula.Disponible
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar edición de jaula");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, JaulaViewModel model)
    {
        try
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

            // Validar que capacidad sea positiva
            if (model.Capacidad <= 0)
            {
                ModelState.AddModelError(nameof(model.Capacidad), "La capacidad debe ser mayor a cero");
                ViewBag.Galpones = new SelectList(
                    await _context.Galpones.ToListAsync(),
                    "Id",
                    "Nombre");
                return View(model);
            }

            var jaula = await _context.Jaulas.FindAsync(id);

            if (jaula == null)
                return NotFound();

            // Validar que no intenten asignar más cuyes de la capacidad
            if (model.Capacidad < jaula.CantidadActual)
            {
                ModelState.AddModelError(nameof(model.Capacidad),
                    $"La capacidad no puede ser menor que los cuyes actualmente asignados ({jaula.CantidadActual})");
                ViewBag.Galpones = new SelectList(
                    await _context.Galpones.ToListAsync(),
                    "Id",
                    "Nombre");
                return View(model);
            }

            jaula.Codigo = model.Codigo.Trim();
            jaula.Capacidad = model.Capacidad;
            jaula.GalponId = model.GalponId;
            jaula.Disponible = model.Activo;
            jaula.FechaModificacion = DateTime.Now;
            jaula.UsuarioModificacionId = GetCurrentUserId();

            _context.Jaulas.Update(jaula);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Jaula actualizada exitosamente";
            _logger.LogInformation($"Jaula actualizada - Código: {jaula.Codigo}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar jaula");
            ModelState.AddModelError("", "Error al actualizar la jaula: " + ex.Message);
            ViewBag.Galpones = new SelectList(
                await _context.Galpones.ToListAsync(),
                "Id",
                "Nombre");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var jaula = await _context.Jaulas.FindAsync(id);

            if (jaula == null)
                return NotFound();

            // No permitir eliminar jaula con cuyes
            if (jaula.CantidadActual > 0)
            {
                TempData["Error"] = "No se puede eliminar una jaula que contiene cuyes";
                return RedirectToAction(nameof(Index));
            }

            _context.Jaulas.Remove(jaula);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Jaula eliminada exitosamente";
            _logger.LogInformation($"Jaula eliminada - Código: {jaula.Codigo}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar jaula");
            TempData["Error"] = "Error al eliminar la jaula: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
