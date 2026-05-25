using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra los casos de mortalidad en los cuyes.
/// </summary>
public class Mortalidad : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de mortalidad.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cuy que murió.
    /// </summary>
    public int CuyId { get; set; }

    /// <summary>
    /// Fecha de la muerte.
    /// </summary>
    public DateTime FechaMuerte { get; set; }

    /// <summary>
    /// Causa probable de la muerte.
    /// </summary>
    public string CausaProblable { get; set; } = string.Empty;

    /// <summary>
    /// Descripción detallada de las circunstancias de la muerte.
    /// </summary>
    public string? Descripcion { get; set; }

    /// <summary>
    /// Observaciones adicionales.
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
