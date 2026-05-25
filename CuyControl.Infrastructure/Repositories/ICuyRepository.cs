using CuyControl.Domain.Entities;

namespace CuyControl.Infrastructure.Repositories;

/// <summary>
/// Repositorio para operaciones con entidad Cuy.
/// </summary>
public interface ICuyRepository : IRepository<Cuy>
{
    /// <summary>
    /// Obtiene un cuy por su código único.
    /// </summary>
    Task<Cuy?> GetByCodigoAsync(string codigo);

    /// <summary>
    /// Obtiene cuyes por jaula.
    /// </summary>
    Task<IEnumerable<Cuy>> GetByJaulaAsync(int jaulaId);

    /// <summary>
    /// Obtiene cuyes activos.
    /// </summary>
    Task<IEnumerable<Cuy>> GetActivosAsync();

    /// <summary>
    /// Obtiene cuyes por estado.
    /// </summary>
    Task<IEnumerable<Cuy>> GetByEstadoAsync(int estadoId);

    /// <summary>
    /// Cuenta cuyes activos.
    /// </summary>
    Task<int> CountActivosAsync();
}
