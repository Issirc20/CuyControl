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
public class VentaController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<VentaController> _logger;

    public VentaController(ApplicationDbContext context, ILogger<VentaController> logger)
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
            var ventas = await _context.Ventas
                .Include(v => v.Cuy)
                .OrderByDescending(v => v.FechaVenta)
                .Select(v => new VentaViewModel
                {
                    Id = v.Id,
                    CuyId = v.CuyId,
                    CuyCodigo = v.Cuy.Codigo,
                    FechaVenta = v.FechaVenta,
                    PrecioUnitario = v.PrecioUnitario,
                    Cantidad = v.Cantidad,
                    PrecioTotal = v.PrecioTotal,
                    NombreComprador = v.NombreComprador,
                    ContactoComprador = v.ContactoComprador,
                    MetodoPago = v.MetodoPago,
                    Observaciones = v.Observaciones
                })
                .ToListAsync();

            return View(ventas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener listado de ventas");
            TempData["Error"] = "Error al cargar las ventas";
            return RedirectToAction("Error", "Home");
        }
    }

    public async Task<IActionResult> Create()
    {
        try
        {
            // Solo mostrar cuyes activos que no sean vendidos ni muertos
            ViewBag.Cuyes = new SelectList(
                await _context.Cuyes
                    .Where(c => c.Activo 
                        && c.EstadoId != (int)EstadoCuy.Vendido 
                        && c.EstadoId != (int)EstadoCuy.Muerto)
                    .ToListAsync(),
                "Id",
                "Codigo");

            return View(new VentaViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar formulario de venta");
            TempData["Error"] = "Error al preparar el formulario";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VentaViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cuyes = new SelectList(
                    await _context.Cuyes
                        .Where(c => c.Activo 
                            && c.EstadoId != (int)EstadoCuy.Vendido 
                            && c.EstadoId != (int)EstadoCuy.Muerto)
                        .ToListAsync(),
                    "Id",
                    "Codigo");

                return View(model);
            }

            // Validar que el cuy existe
            var cuy = await _context.Cuyes.FindAsync(model.CuyId);
            if (cuy == null)
            {
                ModelState.AddModelError("CuyId", "Cuy no encontrado");
                ViewBag.Cuyes = new SelectList(
                    await _context.Cuyes
                        .Where(c => c.Activo 
                            && c.EstadoId != (int)EstadoCuy.Vendido 
                            && c.EstadoId != (int)EstadoCuy.Muerto)
                        .ToListAsync(),
                    "Id",
                    "Codigo");
                return View(model);
            }

            // Validar que el cuy está activo
            if (!cuy.Activo)
            {
                ModelState.AddModelError("CuyId", "El cuy debe estar activo para vender");
                ViewBag.Cuyes = new SelectList(
                    await _context.Cuyes
                        .Where(c => c.Activo 
                            && c.EstadoId != (int)EstadoCuy.Vendido 
                            && c.EstadoId != (int)EstadoCuy.Muerto)
                        .ToListAsync(),
                    "Id",
                    "Codigo");
                return View(model);
            }

            // Validar que el cuy no fue vendido
            if (cuy.EstadoId == (int)EstadoCuy.Vendido)
            {
                ModelState.AddModelError("CuyId", "El cuy ya fue vendido");
                ViewBag.Cuyes = new SelectList(
                    await _context.Cuyes
                        .Where(c => c.Activo 
                            && c.EstadoId != (int)EstadoCuy.Vendido 
                            && c.EstadoId != (int)EstadoCuy.Muerto)
                        .ToListAsync(),
                    "Id",
                    "Codigo");
                return View(model);
            }

            // Validar que el cuy no está muerto
            if (cuy.EstadoId == (int)EstadoCuy.Muerto)
            {
                ModelState.AddModelError("CuyId", "No se puede vender un cuy muerto");
                ViewBag.Cuyes = new SelectList(
                    await _context.Cuyes
                        .Where(c => c.Activo 
                            && c.EstadoId != (int)EstadoCuy.Vendido 
                            && c.EstadoId != (int)EstadoCuy.Muerto)
                        .ToListAsync(),
                    "Id",
                    "Codigo");
                return View(model);
            }

            var venta = new Venta
            {
                CuyId = model.CuyId,
                FechaVenta = model.FechaVenta,
                PrecioUnitario = model.PrecioUnitario,
                Cantidad = model.Cantidad,
                PrecioTotal = model.PrecioUnitario * model.Cantidad,
                NombreComprador = model.NombreComprador,
                ContactoComprador = model.ContactoComprador,
                MetodoPago = model.MetodoPago,
                Observaciones = model.Observaciones,
                FechaCreacion = DateTime.Now,
                UsuarioCreacionId = GetCurrentUserId()
            };

            _context.Ventas.Add(venta);

            // Cambiar estado del cuy a vendido
            cuy.Activo = false;
            cuy.EstadoId = (int)EstadoCuy.Vendido;
            _context.Cuyes.Update(cuy);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Venta registrada exitosamente";
            _logger.LogInformation($"Venta creada - CuyId: {model.CuyId}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear venta");
            ModelState.AddModelError("", "Error al crear la venta: " + ex.Message);
            ViewBag.Cuyes = new SelectList(
                await _context.Cuyes
                    .Where(c => c.Activo 
                        && c.EstadoId != (int)EstadoCuy.Vendido 
                        && c.EstadoId != (int)EstadoCuy.Muerto)
                    .ToListAsync(),
                "Id",
                "Codigo");
            return View(model);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var venta = await _context.Ventas
                .Include(v => v.Cuy)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (venta == null)
                return NotFound();

            var model = new VentaViewModel
            {
                Id = venta.Id,
                CuyId = venta.CuyId,
                CuyCodigo = venta.Cuy.Codigo,
                FechaVenta = venta.FechaVenta,
                PrecioUnitario = venta.PrecioUnitario,
                Cantidad = venta.Cantidad,
                PrecioTotal = venta.PrecioTotal,
                NombreComprador = venta.NombreComprador,
                ContactoComprador = venta.ContactoComprador,
                MetodoPago = venta.MetodoPago,
                Observaciones = venta.Observaciones
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener detalles de venta");
            return RedirectToAction("Error", "Home");
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var venta = await _context.Ventas.FindAsync(id);

            if (venta == null)
                return NotFound();

            ViewBag.Cuyes = new SelectList(
                await _context.Cuyes.ToListAsync(),
                "Id",
                "Codigo",
                venta.CuyId);

            var model = new VentaViewModel
            {
                Id = venta.Id,
                CuyId = venta.CuyId,
                FechaVenta = venta.FechaVenta,
                PrecioUnitario = venta.PrecioUnitario,
                Cantidad = venta.Cantidad,
                PrecioTotal = venta.PrecioTotal,
                NombreComprador = venta.NombreComprador,
                ContactoComprador = venta.ContactoComprador,
                MetodoPago = venta.MetodoPago,
                Observaciones = venta.Observaciones
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al preparar edición de venta");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VentaViewModel model)
    {
        try
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

            var venta = await _context.Ventas.FindAsync(id);

            if (venta == null)
                return NotFound();

            venta.CuyId = model.CuyId;
            venta.FechaVenta = model.FechaVenta;
            venta.PrecioUnitario = model.PrecioUnitario;
            venta.Cantidad = model.Cantidad;
            venta.PrecioTotal = model.PrecioUnitario * model.Cantidad;
            venta.NombreComprador = model.NombreComprador;
            venta.ContactoComprador = model.ContactoComprador;
            venta.MetodoPago = model.MetodoPago;
            venta.Observaciones = model.Observaciones;
            venta.FechaModificacion = DateTime.Now;
            venta.UsuarioModificacionId = GetCurrentUserId();

            await _context.SaveChangesAsync();

            TempData["Success"] = "Venta actualizada exitosamente";
            _logger.LogInformation($"Venta actualizada - VentaId: {id}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar venta");
            ModelState.AddModelError("", "Error al actualizar la venta: " + ex.Message);
            ViewBag.Cuyes = new SelectList(
                await _context.Cuyes.ToListAsync(),
                "Id",
                "Codigo",
                model.CuyId);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var venta = await _context.Ventas.FindAsync(id);

            if (venta == null)
                return NotFound();

            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Venta eliminada exitosamente";
            _logger.LogInformation($"Venta eliminada - VentaId: {id}, Usuario: {GetCurrentUserId()}");

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar venta");
            TempData["Error"] = "Error al eliminar la venta: " + ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}