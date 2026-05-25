namespace CuyControl.Application.DTOs;

public class AlimentacionDto
{
    public int Id { get; set; }
    public int JaulaId { get; set; }
    public int UsuarioId { get; set; }
    public DateTime FechaAlimentacion { get; set; }
    public decimal CantidadAlimento { get; set; }
    public string TipoAlimento { get; set; } = string.Empty;
    public string? Observaciones { get; set; }
}