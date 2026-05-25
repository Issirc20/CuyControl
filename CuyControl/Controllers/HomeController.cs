using CuyControl.Infrastructure.Data;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuyControl.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

        var dashboard = new DashboardViewModel
        {
            TotalCuyes = await _context.Cuyes.CountAsync(),
            CuyesActivos = await _context.Cuyes.CountAsync(c => c.Activo),
            CuyesMuertos = await _context.Cuyes.CountAsync(c => c.EstadoId == 7),
            CuyesVendidos = await _context.Cuyes.CountAsync(c => c.EstadoId == 6),

            TotalMachos = await _context.Cuyes.CountAsync(c => c.SexoId == 1),
            TotalHembras = await _context.Cuyes.CountAsync(c => c.SexoId == 2),

            TotalGalpones = await _context.Galpones.CountAsync(),
            RegistrosEsteMes = await _context.Cuyes.CountAsync(c => c.FechaCreacion >= inicioMes),

            TotalVentas = await _context.Ventas.SumAsync(v => (decimal?)v.PrecioTotal) ?? 0,
            VentasEsteMes = await _context.Ventas
                .Where(v => v.FechaVenta >= inicioMes)
                .SumAsync(v => (decimal?)v.PrecioTotal) ?? 0,

            PesoPromedioCuyes = await _context.Cuyes.AnyAsync()
                ? await _context.Cuyes.AverageAsync(c => c.PesoActual)
                : 0,

            UltimaActualizacion = DateTime.Now
        };

        var ventasPorMes = await _context.Ventas
            .GroupBy(v => v.FechaVenta.Month)
            .Select(g => new
            {
                Mes = g.Key,
                Total = g.Sum(v => v.PrecioTotal)
            })
            .OrderBy(x => x.Mes)
            .ToListAsync();

        dashboard.MesesVentas = ventasPorMes
            .Select(x => $"Mes {x.Mes}")
            .ToList();

        dashboard.MontosVentas = ventasPorMes
            .Select(x => x.Total)
            .ToList();

        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}