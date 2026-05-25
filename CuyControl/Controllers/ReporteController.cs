using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Controllers;

[Authorize(Roles = "Administrador")]
public class ReporteController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReporteController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

        var reporte = new DashboardViewModel
        {
            TotalCuyes = await _context.Cuyes.CountAsync(),
            CuyesActivos = await _context.Cuyes.CountAsync(c => c.Activo),
            CuyesVendidos = await _context.Cuyes.CountAsync(c => c.EstadoId == 6),
            CuyesMuertos = await _context.Cuyes.CountAsync(c => c.EstadoId == 7),
            TotalGalpones = await _context.Galpones.CountAsync(),
            TotalVentas = await _context.Ventas.SumAsync(v => (decimal?)v.PrecioTotal) ?? 0,
            VentasEsteMes = await _context.Ventas
                .Where(v => v.FechaVenta >= inicioMes)
                .SumAsync(v => (decimal?)v.PrecioTotal) ?? 0,
            RegistrosEsteMes = await _context.Cuyes.CountAsync(c => c.FechaCreacion >= inicioMes),
            PesoPromedioCuyes = await _context.Cuyes.AnyAsync()
                ? await _context.Cuyes.AverageAsync(c => c.PesoActual)
                : 0,
            UltimaActualizacion = DateTime.Now
        };

        return View(reporte);
    }
}
