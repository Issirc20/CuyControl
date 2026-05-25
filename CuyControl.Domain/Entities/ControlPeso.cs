using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra el control de peso de los cuyes.
/// </summary>
public class ControlPeso : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de control de peso.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cuy al que se le registra el peso.
    /// </summary>
    public int CuyId { get; set; }

    /// <summary>
    /// Peso registrado en gramos.
    /// </summary>
    public decimal Peso { get; set; }

    /// <summary>
    /// Fecha del pesaje.
    /// </summary>
    public DateTime FechaPesaje { get; set; }

    /// <summary>
    /// Observaciones sobre el estado del cuy durante el pesaje.
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
    public virtual Cuy? Cuy { get; set; }
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
}
