using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra las enfermedades diagnosticadas en los cuyes.
/// </summary>
public class Enfermedad : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de enfermedad.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cuy afectado.
    /// </summary>
    public int CuyId { get; set; }

    /// <summary>
    /// Nombre de la enfermedad.
    /// </summary>
    public string Nombre { get; set; } = string.Empty;

    /// <summary>
    /// Descripción o síntomas de la enfermedad.
    /// </summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de diagnóstico.
    /// </summary>
    public DateTime FechaDiagnostico { get; set; }

    /// <summary>
    /// Estado de la enfermedad.
    /// </summary>
    public string? Estado { get; set; }

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
    public virtual ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();
}
