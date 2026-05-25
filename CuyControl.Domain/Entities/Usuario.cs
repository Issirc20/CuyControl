using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que representa un usuario en el sistema.
/// </summary>
public class Usuario : IBaseEntity
{
    public int Id { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string NombreUsuario { get; set; } = string.Empty;

    public string Correo { get; set; } = string.Empty;

    public string ContraseñaHash { get; set; } = string.Empty;

    public int TipoUsuarioId { get; set; }

    public bool Activo { get; set; } = true;

    public string? Telefono { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public DateTime? FechaModificacion { get; set; }
}