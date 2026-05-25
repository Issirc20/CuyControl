using System.ComponentModel.DataAnnotations;

namespace CuyControl.Domain.Entities;

public class Galpon
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Ubicacion { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Jaula> Jaulas { get; set; } = new List<Jaula>();
}