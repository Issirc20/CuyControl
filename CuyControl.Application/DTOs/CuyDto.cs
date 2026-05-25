namespace CuyControl.Application.DTOs;

public class CuyDto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }
    public int SexoId { get; set; }
    public int EstadoId { get; set; }
    public int? JaulaId { get; set; }
    public decimal PesoActual { get; set; }
    public string? Raza { get; set; }
    public string? Observaciones { get; set; }
    public int EdadDias { get; set; }
}