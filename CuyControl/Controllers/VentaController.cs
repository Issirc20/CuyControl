using CuyControl.Domain.Entities;
using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador,Operador")]
public class VentaController : Controller
{
    private readonly ApplicationDbContext _context;

    public VentaController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
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

    public async Task<IActionResult> Create()
    {
        ViewBag.Cuyes = new SelectList(
            await _context.Cuyes.Where(c => c.Activo).ToListAsync(),
            "Id",
            "Codigo");

        return View(new VentaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VentaViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Cuyes = new SelectList(
                await _context.Cuyes.Where(c => c.Activo).ToListAsync(),
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
            UsuarioCreacionId = 1
        };

        _context.Ventas.Add(venta);

        var cuy = await _context.Cuyes.FindAsync(model.CuyId);
        if (cuy != null)
        {
            cuy.Activo = false;
            cuy.EstadoId = 6;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
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

    public async Task<IActionResult> Edit(int id)
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VentaViewModel model)
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

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var venta = await _context.Ventas.FindAsync(id);

        if (venta == null)
            return NotFound();

        _context.Ventas.Remove(venta);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}