using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class JaulaViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La capacidad es requerida")]
    public int Capacidad { get; set; }

    /// <summary>
    /// Cantidad actual de cuyes en la jaula
    /// </summary>
    public int CantidadActual { get; set; }

    /// <summary>
    /// Porcentaje de ocupación de la jaula
    /// </summary>
    public decimal PorcentajeOcupacion { get; set; }

    [Required(ErrorMessage = "Seleccione un galpón")]
    public int GalponId { get; set; }

    public string? GalponNombre { get; set; }

    public bool Activo { get; set; } = true;
}