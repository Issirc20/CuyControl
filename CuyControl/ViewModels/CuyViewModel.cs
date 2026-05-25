using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

/// <summary>
/// ViewModel para mostrar y editar información de un cuy.
/// </summary>
public class CuyViewModel
{
    /// <summary>
    /// Identificador único del cuy.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Código único del cuy.
    /// </summary>
    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Fecha de nacimiento del cuy.
    /// </summary>
    [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
    [DataType(DataType.Date)]
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// Sexo del cuy.
    /// </summary>
    [Required(ErrorMessage = "El sexo es requerido")]
    public int SexoId { get; set; }

    /// <summary>
    /// Nombre del sexo para mostrar.
    /// </summary>
    public string? SexoNombre { get; set; }

    /// <summary>
    /// Estado del cuy.
    /// </summary>
    [Required(ErrorMessage = "El estado es requerido")]
    public int EstadoId { get; set; }

    /// <summary>
    /// Nombre del estado para mostrar.
    /// </summary>
    public string? EstadoNombre { get; set; }

    /// <summary>
    /// Identificador de la jaula.
    /// </summary>
    public int? JaulaId { get; set; }

    /// <summary>
    /// Información de la jaula.
    /// </summary>
    public string? JaulaInfo { get; set; }

    /// <summary>
    /// Peso actual en gramos.
    /// </summary>
    [Required(ErrorMessage = "El peso es requerido")]
    [Range(1, 10000, ErrorMessage = "El peso debe estar entre 1 y 10000 gramos")]
    public decimal PesoActual { get; set; }

    /// <summary>
    /// Raza del cuy.
    /// </summary>
    [StringLength(100)]
    public string? Raza { get; set; }

    /// <summary>
    /// Observaciones adicionales.
    /// </summary>
    [StringLength(500)]
    public string? Observaciones { get; set; }

    /// <summary>
    /// Edad calculada en días.
    /// </summary>
    public int? EdadDias { get; set; }

    /// <summary>
    /// Fecha de creación.
    /// </summary>
    public DateTime FechaCreacion { get; set; }
}