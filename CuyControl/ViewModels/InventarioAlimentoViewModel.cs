using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class InventarioAlimentoViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El tipo de alimento es requerido")]
    [StringLength(100)]
    public string TipoAlimento { get; set; } = string.Empty;

    [Required]
    [Range(0, 999999)]
    public decimal CantidadActual { get; set; }

    [Required]
    [Range(0, 999999)]
    public decimal CantidadMinima { get; set; }

    public DateTime? FechaUltimaReposicion { get; set; }

    [Range(0, 999999)]
    public decimal CostoUnitario { get; set; }

    public string? Observaciones { get; set; }

    public bool StockBajo => CantidadActual <= CantidadMinima;
}