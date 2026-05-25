using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class VentaViewModel
{
    public int Id { get; set; }

    [Required]
    public int CuyId { get; set; }

    public string? CuyCodigo { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime FechaVenta { get; set; } = DateTime.Today;

    [Required]
    [Range(1, 99999)]
    public decimal PrecioUnitario { get; set; }

    [Required]
    [Range(1, 100)]
    public int Cantidad { get; set; } = 1;

    public decimal PrecioTotal { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreComprador { get; set; } = string.Empty;

    [StringLength(20)]
    public string? ContactoComprador { get; set; }

    [StringLength(50)]
    public string? MetodoPago { get; set; }

    public string? Observaciones { get; set; }
}