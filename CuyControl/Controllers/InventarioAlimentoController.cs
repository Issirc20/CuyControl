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

    public InventarioAlimentoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
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
            .ToListAsync();

        return View(alimentos);
    }

    public IActionResult Create()
    {
        return View(new InventarioAlimentoViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventarioAlimentoViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var alimento = new InventarioAlimento
        {
            TipoAlimento = model.TipoAlimento,
            CantidadActual = model.CantidadActual,
            CantidadMinima = model.CantidadMinima,
            FechaUltimaReposicion = model.FechaUltimaReposicion,
            CostoUnitario = model.CostoUnitario,
            Observaciones = model.Observaciones,
            FechaCreacion = DateTime.Now,
            UsuarioCreacionId = 1
        };

        _context.InventariosAlimento.Add(alimento);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}