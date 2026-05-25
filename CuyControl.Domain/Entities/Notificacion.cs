namespace CuyControl.Domain.Entities;

using CuyControl.Domain.Interfaces;

public class Notificacion : IBaseEntity
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public bool Leida { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
