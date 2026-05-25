namespace CuyControl.Application.DTOs;

/// <summary>
/// DTO para transferencia de datos de Usuario.
/// </summary>
public class UsuarioDto
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre completo del usuario.
    /// </summary>
    public string NombreCompleto { get; set; } = string.Empty;

    /// <summary>
    /// Nombre de usuario para acceso al sistema.
    /// </summary>
    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Correo { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de usuario (1=Administrador, 2=Operador, 3=Visualizador).
    /// </summary>
    public int TipoUsuarioId { get; set; }

    /// <summary>
    /// Nombre del tipo de usuario para mostrar.
    /// </summary>
    public string? TipoUsuarioNombre { get; set; }

    /// <summary>
    /// Indica si el usuario está activo.
    /// </summary>
    public bool Activo { get; set; }

    /// <summary>
    /// Teléfono de contacto del usuario.
    /// </summary>
    public string? Telefono { get; set; }

    /// <summary>
    /// Contraseña (solo para creación/actualización, no se retorna).
    /// </summary>
    public string? Contrasena { get; set; }
}
