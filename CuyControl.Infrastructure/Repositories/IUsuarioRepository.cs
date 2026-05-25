using CuyControl.Domain.Entities;

namespace CuyControl.Infrastructure.Repositories;

/// <summary>
/// Repositorio para operaciones con entidad Usuario.
/// </summary>
public interface IUsuarioRepository : IRepository<Usuario>
{
    /// <summary>
    /// Obtiene un usuario por nombre de usuario.
    /// </summary>
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);

    /// <summary>
    /// Obtiene un usuario por correo.
    /// </summary>
    Task<Usuario?> GetByCorreoAsync(string correo);

    /// <summary>
    /// Obtiene usuarios activos.
    /// </summary>
    Task<IEnumerable<Usuario>> GetActivosAsync();

    /// <summary>
    /// Obtiene usuarios por tipo.
    /// </summary>
    Task<IEnumerable<Usuario>> GetByTipoAsync(int tipoUsuarioId);

    /// <summary>
    /// Verifica si existe un usuario por nombre.
    /// </summary>
    Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario);
}
