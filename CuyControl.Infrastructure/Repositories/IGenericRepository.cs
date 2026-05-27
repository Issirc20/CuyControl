namespace CuyControl.Infrastructure.Repositories;

/// <summary>
/// Interfaz genérica para operaciones CRUD
/// </summary>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todas las entidades
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene una entidad por ID
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Cuenta todas las entidades
    /// </summary>
    Task<int> CountAsync();

    /// <summary>
    /// Cuenta entidades que cumplen un predicado
    /// </summary>
    Task<int> CountAsync(Func<T, bool> predicate);

    /// <summary>
    /// Verifica si una entidad existe
    /// </summary>
    Task<bool> ExistsAsync(int id);

    /// <summary>
    /// Agrega una nueva entidad
    /// </summary>
    Task<bool> AddAsync(T entity);

    /// <summary>
    /// Agrega múltiples entidades
    /// </summary>
    Task<bool> AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// Elimina una entidad por ID
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Elimina una entidad
    /// </summary>
    Task<bool> DeleteAsync(T entity);

    /// <summary>
    /// Guarda los cambios en la base de datos
    /// </summary>
    Task<bool> SaveChangesAsync();

    /// <summary>
    /// Obtiene entidades con paginación
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(int skip, int take);
}
