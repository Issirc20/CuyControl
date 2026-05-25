using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

/// <summary>
/// ViewModel para el login de usuarios.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Nombre de usuario.
    /// </summary>
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50)]
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Contraseña.
    /// </summary>
    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;

    /// <summary>
    /// Mantener sesión iniciada.
    /// </summary>
    public bool RecordarMe { get; set; }

    /// <summary>
    /// URL de retorno después de login.
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Mensaje de error en caso de fallar.
    /// </summary>
    public string? ErrorMessage { get; set; }
}
