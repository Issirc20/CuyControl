using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class InventarioAlimentoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InventarioAlimentoController> _logger;

    public InventarioAlimentoController(
        ApplicationDbContext context,
        ILogger<InventarioAlimentoController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var alimentos = await _context.InventariosAlimento
                .Select(a => new InventarioAlimentoViewModel
                {
                    Id = a.Id,
                    TipoAlimento = a.TipoAlimento,
                    CantidadActual = a.CantidadActual,
                    CantidadMinima = a.CantidadMinima,
                    FechaUltimaReposicion = a.FechaUltimaReposicion,
                    CostoUnitario = a.CostoUnitario,
                    Observaciones = a.Observaciones
                })
                .OrderBy(a => a.CantidadActual <= a.CantidadMinima ? 0 : 1)
                .ThenBy(a => a.TipoAlimento)
                .ToListAsync();

            return View(alimentos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar inventario de alimentos");
            TempData["Error"] = "Error al cargar el inventario de alimentos.";
            return RedirectToAction("Index", "Home");
        }
    }

    public IActionResult Create()
    {
        return View(new InventarioAlimentoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventarioAlimentoViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.CantidadActual <= 0)
            {
                ModelState.AddModelError(nameof(model.CantidadActual), "La cantidad actual debe ser mayor a cero.");
                return View(model);
            }

            if (model.CantidadMinima < 0)
            {
                ModelState.AddModelError(nameof(model.CantidadMinima), "La cantidad mínima no puede ser negativa.");
                return View(model);
            }

            if (model.CostoUnitario < 0)
            {
                ModelState.AddModelError(nameof(model.CostoUnitario), "El costo unitario no puede ser negativo.");
                return View(model);
            }

            var existe = await _context.InventariosAlimento
                .AnyAsync(a => a.TipoAlimento.ToLower() == model.TipoAlimento.Trim().ToLower());

            if (existe)
            {
                ModelState.AddModelError(nameof(model.TipoAlimento), "Ya existe un alimento con ese nombre.");
                return View(model);
            }

            var alimento = new InventarioAlimento
            {
                TipoAlimento = model.TipoAlimento.Trim(),
                CantidadActual = model.CantidadActual,
                CantidadMinima = model.CantidadMinima,
                FechaUltimaReposicion = model.FechaUltimaReposicion ?? DateTime.Now,
                CostoUnitario = model.CostoUnitario,
                Observaciones = model.Observaciones?.Trim(),
                FechaCreacion = DateTime.Now,

                // IMPORTANTE:
                // Este campo apunta a la tabla Usuarios, no a ApplicationUsers.
                // Por eso usamos 1 mientras no exista integración completa de auditoría.
                UsuarioCreacionId = 1
            };

            _context.InventariosAlimento.Add(alimento);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Alimento registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear alimento");
            ModelState.AddModelError(string.Empty, "Error al guardar el alimento.");
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var alimento = await _context.InventariosAlimento.FindAsync(id);

        if (alimento == null)
            return NotFound();

        var model = new InventarioAlimentoViewModel
        {
            Id = alimento.Id,
            TipoAlimento = alimento.TipoAlimento,
            CantidadActual = alimento.CantidadActual,
            CantidadMinima = alimento.CantidadMinima,
            FechaUltimaReposicion = alimento.FechaUltimaReposicion,
            CostoUnitario = alimento.CostoUnitario,
            Observaciones = alimento.Observaciones
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventarioAlimentoViewModel model)
    {
        try
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            if (model.CantidadActual < 0)
            {
                ModelState.AddModelError(nameof(model.CantidadActual), "La cantidad actual no puede ser negativa.");
                return View(model);
            }

            if (model.CantidadMinima < 0)
            {
                ModelState.AddModelError(nameof(model.CantidadMinima), "La cantidad mínima no puede ser negativa.");
                return View(model);
            }

            if (model.CostoUnitario < 0)
            {
                ModelState.AddModelError(nameof(model.CostoUnitario), "El costo unitario no puede ser negativo.");
                return View(model);
            }

            var alimento = await _context.InventariosAlimento.FindAsync(id);

            if (alimento == null)
                return NotFound();

            alimento.TipoAlimento = model.TipoAlimento.Trim();
            alimento.CantidadActual = model.CantidadActual;
            alimento.CantidadMinima = model.CantidadMinima;
            alimento.FechaUltimaReposicion = model.FechaUltimaReposicion;
            alimento.CostoUnitario = model.CostoUnitario;
            alimento.Observaciones = model.Observaciones?.Trim();
            alimento.FechaModificacion = DateTime.Now;

            // Igual que arriba: apunta a Usuarios, no a ApplicationUsers.
            alimento.UsuarioModificacionId = 1;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Alimento actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar alimento");
            ModelState.AddModelError(string.Empty, "Error al actualizar el alimento.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var alimento = await _context.InventariosAlimento.FindAsync(id);

            if (alimento == null)
                return NotFound();

            _context.InventariosAlimento.Remove(alimento);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Alimento eliminado correctamente.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar alimento");
            TempData["Error"] = "No se pudo eliminar el alimento.";
            return RedirectToAction(nameof(Index));
        }
    }
}