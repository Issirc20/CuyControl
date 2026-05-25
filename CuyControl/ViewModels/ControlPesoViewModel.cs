using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class ControlPesoViewModel
{
    public int Id { get; set; }

    [Required]
    public int CuyId { get; set; }

    public string? CuyCodigo { get; set; }

    [Required]
    [Range(1, 10000)]
    public decimal Peso { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaPesaje { get; set; } = DateTime.Today;

    [StringLength(500)]
    public string? Observaciones { get; set; }
}