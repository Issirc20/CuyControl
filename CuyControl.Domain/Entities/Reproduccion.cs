using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra la información de reproducción de los cuyes.
/// </summary>
public class Reproduccion : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de reproducción.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cuy macho que participa en la reproducción.
    /// </summary>
    public int CuyMachoId { get; set; }

    /// <summary>
    /// Identificador del cuy hembra que participa en la reproducción.
    /// </summary>
    public int CuyHembraId { get; set; }

    /// <summary>
    /// Fecha del cruzamiento.
    /// </summary>
    public DateTime FechaCruzamiento { get; set; }

    /// <summary>
    /// Observaciones sobre el cruzamiento.
    /// </summary>
    public string? Observaciones { get; set; }

    /// <summary>
    /// Indica si la reproducción fue exitosa.
    /// </summary>
    public bool Exitosa { get; set; }

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
    public virtual Cuy? CuyMacho { get; set; }
    public virtual Cuy? CuyHembra { get; set; }
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
    public virtual ICollection<Parto> Partos { get; set; } = new List<Parto>();
}
