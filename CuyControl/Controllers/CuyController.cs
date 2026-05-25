using CuyControl.Application.Interfaces;
using CuyControl.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuyControl.Controllers;

/// <summary>
/// Controlador para operaciones con cuyes.
/// </summary>
[Authorize(Roles = "Administrador,Operador")]
public class CuyController : Controller
{
    private readonly ICuyService _cuyService;
    private readonly ILogger<CuyController> _logger;

    /// <summary>
    /// Constructor del controlador.
    /// </summary>
    public CuyController(ICuyService cuyService, ILogger<CuyController> logger)
    {
        _cuyService = cuyService ?? throw new ArgumentNullException(nameof(cuyService));
        _logger = logger;
    }

    /// <summary>
    /// Lista todos los cuyes.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var cuyes = await _cuyService.ObtenerTodosCuyesAsync();
            var viewModels = cuyes.Select(c => new CuyViewModel
            {
                Id = c.Id,
                Codigo = c.Codigo,
                FechaNacimiento = c.FechaNacimiento,
                SexoId = c.SexoId,
                SexoNombre = c.SexoId == 1 ? "Macho" : "Hembra",
                EstadoId = c.EstadoId,
                JaulaId = c.JaulaId,
                PesoActual = c.PesoActual,
                Raza = c.Raza,
                Observaciones = c.Observaciones,
                EdadDias = c.EdadDias,
                FechaCreacion = c.Id > 0 ? DateTime.Now : DateTime.Now
            }).ToList();
            return View(viewModels);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener cuyes: {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    /// <summary>
    /// Muestra el formulario para crear un nuevo cuy.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CuyViewModel());
    }

    /// <summary>
    /// Crea un nuevo cuy.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CuyViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            var cuyDto = new Application.DTOs.CuyDto
            {
                Codigo = viewModel.Codigo,
                FechaNacimiento = viewModel.FechaNacimiento,
                SexoId = viewModel.SexoId,
                EstadoId = viewModel.EstadoId,
                JaulaId = viewModel.JaulaId,
                PesoActual = viewModel.PesoActual,
                Raza = viewModel.Raza,
                Observaciones = viewModel.Observaciones
            };

            var resultado = await _cuyService.CrearCuyAsync(cuyDto);
            TempData["Mensaje"] = "Cuy creado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al crear cuy: {ex.Message}");
            ModelState.AddModelError("", "Error al crear el cuy");
            return View(viewModel);
        }
    }

    /// <summary>
    /// Muestra los detalles de un cuy.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var cuy = await _cuyService.ObtenerCuyPorIdAsync(id);
            if (cuy == null)
                return NotFound();

            var viewModel = new CuyViewModel
            {
                Id = cuy.Id,
                Codigo = cuy.Codigo,
                FechaNacimiento = cuy.FechaNacimiento,
                SexoId = cuy.SexoId,
                SexoNombre = cuy.SexoId == 1 ? "Macho" : "Hembra",
                EstadoId = cuy.EstadoId,
                JaulaId = cuy.JaulaId,
                PesoActual = cuy.PesoActual,
                Raza = cuy.Raza,
                Observaciones = cuy.Observaciones,
                EdadDias = cuy.EdadDias
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener detalles del cuy: {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    /// <summary>
    /// Muestra el formulario para editar un cuy.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var cuy = await _cuyService.ObtenerCuyPorIdAsync(id);
            if (cuy == null)
                return NotFound();

            var viewModel = new CuyViewModel
            {
                Id = cuy.Id,
                Codigo = cuy.Codigo,
                FechaNacimiento = cuy.FechaNacimiento,
                SexoId = cuy.SexoId,
                EstadoId = cuy.EstadoId,
                JaulaId = cuy.JaulaId,
                PesoActual = cuy.PesoActual,
                Raza = cuy.Raza,
                Observaciones = cuy.Observaciones
            };
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener cuy para editar: {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    /// <summary>
    /// Actualiza un cuy existente.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CuyViewModel viewModel)
    {
        if (id != viewModel.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(viewModel);

        try
        {
            var cuyDto = new Application.DTOs.CuyDto
            {
                Id = viewModel.Id,
                Codigo = viewModel.Codigo,
                FechaNacimiento = viewModel.FechaNacimiento,
                SexoId = viewModel.SexoId,
                EstadoId = viewModel.EstadoId,
                JaulaId = viewModel.JaulaId,
                PesoActual = viewModel.PesoActual,
                Raza = viewModel.Raza,
                Observaciones = viewModel.Observaciones
            };

            await _cuyService.ActualizarCuyAsync(id, cuyDto);
            TempData["Mensaje"] = "Cuy actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al actualizar cuy: {ex.Message}");
            ModelState.AddModelError("", "Error al actualizar el cuy");
            return View(viewModel);
        }
    }

    /// <summary>
    /// Elimina un cuy.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var resultado = await _cuyService.EliminarCuyAsync(id);
            if (resultado)
                TempData["Mensaje"] = "Cuy eliminado exitosamente";
            else
                TempData["Error"] = "No se pudo eliminar el cuy";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al eliminar cuy: {ex.Message}");
            TempData["Error"] = "Error al eliminar el cuy";
            return RedirectToAction(nameof(Index));
        }
    }
}
