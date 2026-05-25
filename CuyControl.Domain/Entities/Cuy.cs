using System.ComponentModel.DataAnnotations;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad principal para la gestión de cuyes.
/// </summary>
public class Cuy
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    public DateTime FechaNacimiento { get; set; }

    /// <summary>
    /// 1 = Macho | 2 = Hembra
    /// </summary>
    [Required]
    public int SexoId { get; set; }

    /// <summary>
    /// Estado actual del cuy.
    /// </summary>
    [Required]
    public int EstadoId { get; set; }

    public int? JaulaId { get; set; }

    [Required]
    [Range(0, 9999.99)]
    public decimal PesoActual { get; set; }

    [MaxLength(100)]
    public string? Raza { get; set; }

    [MaxLength(500)]
    public string? Observaciones { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // RELACIONES

    public Jaula? Jaula { get; set; }

    public ICollection<ControlPeso> ControlesPeso { get; set; } = new List<ControlPeso>();

    public ICollection<Enfermedad> Enfermedades { get; set; } = new List<Enfermedad>();

    public ICollection<Tratamiento> Tratamientos { get; set; } = new List<Tratamiento>();

    public ICollection<Reproduccion> Reproducciones { get; set; } = new List<Reproduccion>();

    public ICollection<Mortalidad> Mortalidades { get; set; } = new List<Mortalidad>();

    public ICollection<Venta> Ventas { get; set; } = new List<Venta>();
}