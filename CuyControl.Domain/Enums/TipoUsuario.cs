namespace CuyControl.Domain.Enums;

/// <summary>
/// Enumeración para el tipo de usuario en el sistema.
/// </summary>
public enum TipoUsuario
{
    /// <summary>
    /// Administrador del sistema
    /// </summary>
    Administrador = 1,

    /// <summary>
    /// Usuario operador/empleado
    /// </summary>
    Operador = 2,

    /// <summary>
    /// Usuario con acceso de lectura solamente
    /// </summary>
    Visualizador = 3
}
