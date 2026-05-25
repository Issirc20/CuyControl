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

    [Required(ErrorMessage = "Seleccione un galpón")]
    public int GalponId { get; set; }

    public string? GalponNombre { get; set; }

    public bool Activo { get; set; } = true;
}