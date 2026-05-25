using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra los tratamientos aplicados a los cuyes.
/// </summary>
public class Tratamiento : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de tratamiento.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cuy tratado.
    /// </summary>
    public int CuyId { get; set; }

    /// <summary>
    /// Identificador de la enfermedad que se trata.
    /// </summary>
    public int EnfermedadId { get; set; }

    /// <summary>
    /// Nombre del medicamento aplicado.
    /// </summary>
    public string Medicamento { get; set; } = string.Empty;

    /// <summary>
    /// Dosis del medicamento.
    /// </summary>
    public string Dosis { get; set; } = string.Empty;

    /// <summary>
    /// Vía de administración del medicamento.
    /// </summary>
    public string ViaAdministracion { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de inicio del tratamiento.
    /// </summary>
    public DateTime FechaInicio { get; set; }

    /// <summary>
    /// Fecha de finalización del tratamiento.
    /// </summary>
    public DateTime? FechaFin { get; set; }

    /// <summary>
    /// Resultados del tratamiento.
    /// </summary>
    public string? Resultado { get; set; }

    /// <summary>
    /// Observaciones sobre el tratamiento.
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
    public virtual Enfermedad? Enfermedad { get; set; }
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
}
