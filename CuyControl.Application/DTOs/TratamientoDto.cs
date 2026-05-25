namespace CuyControl.Application.DTOs;

public class TratamientoDto
{
    public int Id { get; set; }
    public int EnfermedadId { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int EstadoId { get; set; }
}
