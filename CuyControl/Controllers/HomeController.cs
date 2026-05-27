using CuyControl.Domain.Enums;
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
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var hoy = DateTime.Today;
            var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

            // Contar cuyes enfermos (único, no registros de enfermedad)
            var cuyesEnfermos = await _context.Enfermedades
                .Select(e => e.CuyId)
                .Distinct()
                .CountAsync();

            // Cuyes disponibles: activos, NO vendidos, NO muertos, NO gestantes, NO lactantes
            var cuyesDisponibles = await _context.Cuyes
                .CountAsync(c => c.Activo
                    && c.EstadoId != (int)EstadoCuy.Vendido
                    && c.EstadoId != (int)EstadoCuy.Muerto
                    && c.EstadoId != (int)EstadoCuy.Gestante
                    && c.EstadoId != (int)EstadoCuy.Lactante);

            // Cuyes nacidos este mes
            var cuyesNacidosEsteMes = await _context.Partos
                .Where(p => p.FechaParto >= inicioMes && p.FechaParto <= hoy)
                .SumAsync(p => (int?)p.NumeroDeCreasVivas) ?? 0;

            var dashboard = new DashboardViewModel
            {
                TotalCuyes = await _context.Cuyes.CountAsync(),
                CuyesActivos = await _context.Cuyes.CountAsync(c => c.Activo),
                CuyesMuertos = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Muerto),
                CuyesVendidos = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Vendido),
                CuyesEnfermos = cuyesEnfermos,

                TotalMachos = await _context.Cuyes.CountAsync(c => c.SexoId == 1),
                TotalHembras = await _context.Cuyes.CountAsync(c => c.SexoId == 2),

                TotalGalpones = await _context.Galpones.CountAsync(),
                TotalJaulas = await _context.Jaulas.CountAsync(),
                TotalEnfermedades = await _context.Enfermedades.CountAsync(),

                RegistrosEsteMes = await _context.Cuyes.CountAsync(c => c.FechaCreacion >= inicioMes),

                TotalVentas = await _context.Ventas.SumAsync(v => (decimal?)v.PrecioTotal) ?? 0,
                VentasEsteMes = await _context.Ventas
                    .Where(v => v.FechaVenta >= inicioMes)
                    .SumAsync(v => (decimal?)v.PrecioTotal) ?? 0,

                InventarioAlimento = await _context.InventariosAlimento
                    .SumAsync(i => (decimal?)i.CantidadActual) ?? 0,

                StockBajo = await _context.InventariosAlimento
                    .CountAsync(i => i.CantidadActual <= i.CantidadMinima),

                PesoPromedioCuyes = await _context.Cuyes.AnyAsync()
                    ? await _context.Cuyes.AverageAsync(c => c.PesoActual)
                    : 0,

                UltimaActualizacion = DateTime.Now,

                CuyesDisponibles = cuyesDisponibles,

                Crias = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Cria && c.Activo),
                Recrias = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Recria && c.Activo),
                Reproductores = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Reproductor && c.Activo),
                Gestantes = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Gestante && c.Activo),
                Lactantes = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Lactante && c.Activo),

                ParaVenta = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Reproductor && c.Activo),

                CuyesNacidosEsteMes = cuyesNacidosEsteMes,
            };

            dashboard.PorcentajeOcupacionJaulas =
                await _context.Jaulas.AnyAsync()
                    ? Math.Round(
                        (decimal)(await _context.Jaulas.SumAsync(j => j.CantidadActual)) /
                        (decimal)(await _context.Jaulas.SumAsync(j => j.Capacidad)) * 100, 2)
                    : 0;

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

            dashboard.EstadosCuyesLabels = new List<string>
            {
                "Activos",
                "Vendidos",
                "Muertos",
                "Enfermos"
            };

            dashboard.EstadosCuyesData = new List<int>
            {
                dashboard.CuyesActivos,
                dashboard.CuyesVendidos,
                dashboard.CuyesMuertos,
                dashboard.CuyesEnfermos
            };

            var alimentos = await _context.InventariosAlimento
                .OrderByDescending(a => a.CantidadActual)
                .Take(10)
                .ToListAsync();

            dashboard.AlimentosLabels = alimentos
                .Select(a => a.TipoAlimento)
                .ToList();

            dashboard.AlimentosData = alimentos
                .Select(a => a.CantidadActual)
                .ToList();

            return View(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cargar dashboard");
            return RedirectToAction("Error", "Home");
        }
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