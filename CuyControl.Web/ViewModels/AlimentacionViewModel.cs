using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class AlimentacionViewModel
{
    public int Id { get; set; }
    [Required]
    public int JaulaId { get; set; }
    public string JaulaCodigo { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public DateTime FechaAlimentacion { get; set; }
    public decimal CantidadAlimento { get; set; }
    public string TipoAlimento { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}
