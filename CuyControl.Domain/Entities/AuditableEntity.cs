namespace CuyControl.Domain.Entities;

/// <summary>
/// Clase base para entidades que requieren auditoría
/// </summary>
public abstract class AuditableEntity
{
    /// <summary>
    /// Identificador único
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    /// <summary>
    /// ID del usuario que creó el registro
    /// </summary>
    public int UsuarioCreacionId { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// ID del usuario que realizó la última modificación
    /// </summary>
    public int? UsuarioModificacionId { get; set; }
}
