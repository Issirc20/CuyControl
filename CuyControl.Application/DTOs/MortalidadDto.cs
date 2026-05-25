namespace CuyControl.Application.DTOs;

public class MortalidadDto
{
    public int Id { get; set; }
    public int CuyId { get; set; }
    public DateTime Fecha { get; set; }
    public string Causa { get; set; } = string.Empty;
}
