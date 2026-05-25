namespace CuyControl.Domain.Entities;

using CuyControl.Domain.Interfaces;

public class Reporte : IBaseEntity
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public int UsuarioCreacionId { get; set; }
}
