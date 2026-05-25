using Microsoft.AspNetCore.Identity;

namespace CuyControl.Infrastructure.Identity;

/// <summary>
/// Clase que extiende IdentityUser para agregar propiedades adicionales de aplicación.
/// </summary>
public class ApplicationUser : IdentityUser<int>
{
    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de usuario en el sistema (1=Administrador, 2=Operador, 3=Visualizador).
    /// </summary>
    public int TipoUsuario { get; set; }

    /// <summary>
    /// Indica si el usuario está activo en el sistema.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de creación del usuario.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Fecha de última modificación del usuario.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Usuario que creó este registro.
    /// </summary>
    public int? CreatedByUserId { get; set; }

    /// <summary>
    /// Usuario que modificó este registro.
    /// </summary>
    public int? UpdatedByUserId { get; set; }
}
