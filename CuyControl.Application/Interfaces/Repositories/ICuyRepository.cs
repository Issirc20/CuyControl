using CuyControl.Domain.Entities;

namespace CuyControl.Application.Interfaces.Repositories;

public interface ICuyRepository
{
    Task<IEnumerable<Cuy>> GetAllAsync();
    Task<Cuy?> GetByIdAsync(int id);
    Task<Cuy?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Cuy>> GetByJaulaAsync(int jaulaId);
    Task<IEnumerable<Cuy>> GetActivosAsync();
    Task<IEnumerable<Cuy>> GetByEstadoAsync(int estadoId);
    Task<int> CountActivosAsync();
    Task<Cuy> AddAsync(Cuy entity);
    Task<Cuy> UpdateAsync(Cuy entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> SaveChangesAsync();
}