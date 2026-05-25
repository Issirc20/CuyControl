namespace CuyControl.Domain.Interfaces;

/// <summary>
/// Interface para entidades que requieren información de auditoría.
/// </summary>
public interface IAuditable
{
    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Usuario que creó el registro.
    /// </summary>
    int UsuarioCreacionId { get; set; }

    /// <summary>
    /// Fecha de última modificación del registro.
    /// </summary>
    DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Usuario que realizó la última modificación.
    /// </summary>
    int? UsuarioModificacionId { get; set; }
}
