using System.ComponentModel.DataAnnotations;
using CuyControl.Domain.Enums;

namespace CuyControl.Web.ViewModels;

public class MovimientoAlimentoViewModel
{
    public int Id { get; set; }

    [Required]
    public int InventarioAlimentoId { get; set; }

    [Required]
    public TipoMovimientoAlimentoEnum TipoMovimiento { get; set; }

    [Required]
    [Range(0.01, 999999)]
    public decimal Cantidad { get; set; }

    [Required]
    public DateTime FechaMovimiento { get; set; } = DateTime.Now;

    [StringLength(500)]
    public string? Observaciones { get; set; }
}
