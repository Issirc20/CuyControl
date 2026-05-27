using CuyControl.Domain.Entities;
using CuyControl.Domain.Enums;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class MovimientoAlimentoController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<MovimientoAlimentoController> _logger;

    public MovimientoAlimentoController(ApplicationDbContext context, ILogger<MovimientoAlimentoController> logger)
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
            var movimientos = await _context.MovimientosAlimento
                .Include(m => m.InventarioAlimento)
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            return View(movimientos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener movimientos de alimento");
            TempData["Error"] = "Error al cargar los movimientos";
            return RedirectToAction("Error", "Home");
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            ViewBag.Inventarios = new SelectList(
                await _context.InventariosAlimento.ToListAsync(),
                "Id",
                "TipoAlimento");

            return View(new MovimientoAlimentoViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar formulario de movimiento");
            TempData["Error"] = "Error al preparar el formulario";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MovimientoAlimentoViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Inventarios = new SelectList(
                    await _context.InventariosAlimento.ToListAsync(),
                    "Id",
                    "TipoAlimento");

                return View(model);
            }

            // Validar que la cantidad sea positiva
            if (model.Cantidad <= 0)
            {
                ModelState.AddModelError(nameof(model.Cantidad), "La cantidad debe ser mayor a cero");
                ViewBag.Inventarios = new SelectList(
                    await _context.InventariosAlimento.ToListAsync(),
                    "Id",
                    "TipoAlimento");
                return View(model);
            }

            // Validar que el inventario existe
            var inventario = await _context.InventariosAlimento
                .FirstOrDefaultAsync(i => i.Id == model.InventarioAlimentoId);

            if (inventario == null)
            {
                ModelState.AddModelError("", "Inventario no encontrado.");
                ViewBag.Inventarios = new SelectList(
                    await _context.InventariosAlimento.ToListAsync(),
                    "Id",
                    "TipoAlimento");
                return View(model);
            }

            // Usar transacción para garantizar consistencia
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Validar si hay suficiente stock para salida
                    if (model.TipoMovimiento == TipoMovimientoAlimentoEnum.Salida)
                    {
                        if (inventario.CantidadActual < model.Cantidad)
                        {
                            ModelState.AddModelError("", 
                                $"No hay suficiente stock. Stock disponible: {inventario.CantidadActual}");
                            await transaction.RollbackAsync();
                            ViewBag.Inventarios = new SelectList(
                                await _context.InventariosAlimento.ToListAsync(),
                                "Id",
                                "TipoAlimento");
                            return View(model);
                        }

                        inventario.CantidadActual -= model.Cantidad;
                    }
                    else if (model.TipoMovimiento == TipoMovimientoAlimentoEnum.Entrada)
                    {
                        inventario.CantidadActual += model.Cantidad;
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tipo de movimiento inválido");
                        await transaction.RollbackAsync();
                        ViewBag.Inventarios = new SelectList(
                            await _context.InventariosAlimento.ToListAsync(),
                            "Id",
                            "TipoAlimento");
                        return View(model);
                    }

                    // Asegurarse que no haya cantidad negativa (safeguard adicional)
                    if (inventario.CantidadActual < 0)
                    {
                        ModelState.AddModelError("", "Error: Stock no puede ser negativo");
                        await transaction.RollbackAsync();
                        ViewBag.Inventarios = new SelectList(
                            await _context.InventariosAlimento.ToListAsync(),
                            "Id",
                            "TipoAlimento");
                        return View(model);
                    }

                    inventario.FechaUltimaReposicion = DateTime.Now;

                    var movimiento = new MovimientoAlimento
                    {
                        InventarioAlimentoId = model.InventarioAlimentoId,
                        TipoMovimiento = model.TipoMovimiento,
                        Cantidad = model.Cantidad,
                        FechaMovimiento = model.FechaMovimiento,
                        Observaciones = model.Observaciones?.Trim() ?? "",
                        UsuarioId = GetCurrentUserId(),
                        FechaCreacion = DateTime.Now,
                        UsuarioCreacionId = GetCurrentUserId()
                    };

                    _context.MovimientosAlimento.Add(movimiento);
                    _context.InventariosAlimento.Update(inventario);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Movimiento registrado correctamente.";
                    _logger.LogInformation(
                        $"Movimiento de alimento creado - Tipo: {model.TipoMovimiento}, " +
                        $"Cantidad: {model.Cantidad}, Usuario: {GetCurrentUserId()}");

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception txEx)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(txEx, "Error en transacción de movimiento");
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear movimiento de alimento");
            ModelState.AddModelError("", "Error al crear el movimiento: " + ex.Message);
            ViewBag.Inventarios = new SelectList(
                await _context.InventariosAlimento.ToListAsync(),
                "Id",
                "TipoAlimento");
            return View(model);
        }
    }
}