using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que representa una jaula dentro de un galpón.
/// </summary>
public class Jaula : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único de la jaula.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código o nombre de la jaula.
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Identificador del galpón al que pertenece.
    /// </summary>
    public int GalponId { get; set; }

    /// <summary>
    /// Capacidad máxima de cuyes en la jaula.
    /// </summary>
    public int Capacidad { get; set; }

    /// <summary>
    /// Cantidad actual de cuyes en la jaula.
    /// </summary>
    public int CantidadActual { get; set; }

    /// <summary>
    /// Tipo de jaula (ejemplo: Cría, Engorde, Reproducción).
    /// </summary>
    public string? Tipo { get; set; }

    /// <summary>
    /// Describe si la jaula está disponible.
    /// </summary>
    public bool Disponible { get; set; }

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
    public virtual Galpon? Galpon { get; set; }
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
    public virtual ICollection<Cuy> Cuyes { get; set; } = new List<Cuy>();
}
