using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class AlimentacionViewModel
{
    public int Id { get; set; }

    [Required]
    public int JaulaId { get; set; }

    public string? JaulaCodigo { get; set; }

    public int UsuarioId { get; set; }

    [Required]
    public DateTime FechaAlimentacion { get; set; }

    [Required]
    [Range(0.01, 1000)]
    public decimal CantidadAlimento { get; set; }

    [Required]
    [StringLength(150)]
    public string TipoAlimento { get; set; } = string.Empty;

    public string? Observaciones { get; set; }
}