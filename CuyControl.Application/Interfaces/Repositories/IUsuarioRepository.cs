using CuyControl.Domain.Entities;

namespace CuyControl.Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
    Task<Usuario?> GetByCorreoAsync(string correo);
    Task<IEnumerable<Usuario>> GetActivosAsync();
    Task<IEnumerable<Usuario>> GetByTipoAsync(int tipoUsuarioId);
    Task<bool> ExistsByNombreUsuarioAsync(string nombreUsuario);
    Task<IEnumerable<Usuario>> GetAllAsync();
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario> AddAsync(Usuario entity);
    Task<Usuario> UpdateAsync(Usuario entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> SaveChangesAsync();
}