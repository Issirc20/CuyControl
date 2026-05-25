using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra los partos de las hembras.
/// </summary>
public class Parto : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de parto.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador de la reproducción que resultó en este parto.
    /// </summary>
    public int ReproduccionId { get; set; }

    /// <summary>
    /// Identificador de la hembra que parió.
    /// </summary>
    public int CuyHembraId { get; set; }

    /// <summary>
    /// Fecha del parto.
    /// </summary>
    public DateTime FechaParto { get; set; }

    /// <summary>
    /// Número de crías nacidas.
    /// </summary>
    public int NumeroDeCreasNacidas { get; set; }

    /// <summary>
    /// Número de crías vivas al nacer.
    /// </summary>
    public int NumeroDeCreasVivas { get; set; }

    /// <summary>
    /// Número de crías muertas al nacer.
    /// </summary>
    public int NumeroDeCreasNacidosMuertos { get; set; }

    /// <summary>
    /// Peso total de las crías nacidas en gramos.
    /// </summary>
    public decimal PesoTotalCreasGramos { get; set; }

    /// <summary>
    /// Estado de salud de la madre después del parto.
    /// </summary>
    public string? EstadoMadre { get; set; }

    /// <summary>
    /// Observaciones sobre el parto.
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
    public virtual Reproduccion? Reproduccion { get; set; }
    public virtual Cuy? CuyHembra { get; set; }
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
}
