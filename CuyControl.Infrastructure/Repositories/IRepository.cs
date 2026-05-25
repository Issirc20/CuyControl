using CuyControl.Domain.Entities;

namespace CuyControl.Infrastructure.Repositories;

/// <summary>
/// Repositorio genérico base para acceso a datos.
/// </summary>
/// <typeparam name="TEntity">Tipo de entidad.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Obtiene todas las entidades.
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad por id.
    /// </summary>
    Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// Agrega una nueva entidad.
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Actualiza una entidad existente.
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity);

    /// <summary>
    /// Elimina una entidad.
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Guarda los cambios en la base de datos.
    /// </summary>
    Task<bool> SaveChangesAsync();
}
