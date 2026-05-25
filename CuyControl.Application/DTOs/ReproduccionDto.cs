namespace CuyControl.Application.DTOs;

public class ReproduccionDto
{
    public int Id { get; set; }
    public int MadreId { get; set; }
    public int PadreId { get; set; }
    public DateTime Fecha { get; set; }
    public bool Exitosa { get; set; }
}
