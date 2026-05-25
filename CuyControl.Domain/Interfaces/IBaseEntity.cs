namespace CuyControl.Domain.Interfaces;

/// <summary>
/// Interface base para todas las entidades del dominio.
/// </summary>
public interface IBaseEntity
{
    /// <summary>
    /// Identificador único de la entidad.
    /// </summary>
    int Id { get; set; }
}
