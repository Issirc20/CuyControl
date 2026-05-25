using System.ComponentModel.DataAnnotations;

namespace CuyControl.Web.ViewModels;

public class GalponViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del galpón es requerido")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Ubicacion { get; set; }

    public bool Activo { get; set; } = true;
}