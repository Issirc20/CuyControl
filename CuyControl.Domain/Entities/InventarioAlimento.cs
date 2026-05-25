using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra el inventario de alimento disponible.
/// </summary>
public class InventarioAlimento : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de inventario.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Tipo de alimento.
    /// </summary>
    public string TipoAlimento { get; set; } = string.Empty;

    /// <summary>
    /// Cantidad actual en kilogramos.
    /// </summary>
    public decimal CantidadActual { get; set; }

    /// <summary>
    /// Cantidad mínima recomendada en kilogramos.
    /// </summary>
    public decimal CantidadMinima { get; set; }

    /// <summary>
    /// Fecha de última reposición.
    /// </summary>
    public DateTime? FechaUltimaReposicion { get; set; }

    /// <summary>
    /// Costo unitario del alimento.
    /// </summary>
    public decimal CostoUnitario { get; set; }

    /// <summary>
    /// Observaciones sobre el alimento.
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
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
}
