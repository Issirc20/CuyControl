using CuyControl.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CuyControl.Controllers;

/// <summary>
/// Controlador para operaciones con usuarios.
/// </summary>
[Authorize(Roles = "Administrador")]
public class UsuarioController : Controller
{
    private readonly IUsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;

    /// <summary>
    /// Constructor del controlador.
    /// </summary>
    public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
    {
        _usuarioService = usuarioService ?? throw new ArgumentNullException(nameof(usuarioService));
        _logger = logger;
    }

    /// <summary>
    /// Lista todos los usuarios.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            var usuarios = await _usuarioService.ObtenerTodosUsuariosAsync();
            return View(usuarios);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener usuarios: {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    /// <summary>
    /// Muestra los detalles de un usuario.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener detalles del usuario: {ex.Message}");
            return RedirectToAction("Error", "Home");
        }
    }

    /// <summary>
    /// Desactiva un usuario.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(int id)
    {
        try
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Activo = false;
            await _usuarioService.ActualizarUsuarioAsync(id, usuario);
            TempData["Mensaje"] = "Usuario desactivado exitosamente";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al desactivar usuario: {ex.Message}");
            TempData["Error"] = "Error al desactivar el usuario";
            return RedirectToAction(nameof(Index));
        }
    }
}






