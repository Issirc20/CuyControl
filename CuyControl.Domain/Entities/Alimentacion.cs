using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra la alimentación de los cuyes.
/// </summary>
public class Alimentacion : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de alimentación.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la jaula alimentada.
    /// </summary>
    public int JaulaId { get; set; }

    /// <summary>
    /// Identificador del usuario que registra la alimentación.
    /// </summary>
    public int UsuarioId { get; set; }

    /// <summary>
    /// Fecha y hora de la alimentación.
    /// </summary>
    public DateTime FechaAlimentacion { get; set; }

    /// <summary>
    /// Cantidad de alimento suministrado en kilogramos.
    /// </summary>
    public decimal CantidadAlimento { get; set; }

    /// <summary>
    /// Tipo de alimento suministrado.
    /// </summary>
    public string TipoAlimento { get; set; } = string.Empty;

    /// <summary>
    /// Observaciones sobre la alimentación.
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Usuario que creó el registro.
    /// </summary>
    public int UsuarioCreacionId { get; set; }

    /// <summary>
    /// Fecha de última modificación del registro.
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Usuario que realizó la última modificación.
    /// </summary>
    public int? UsuarioModificacionId { get; set; }

    // Relaciones
    public virtual Jaula? Jaula { get; set; }
    public virtual Usuario? Usuario { get; set; }
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
}
