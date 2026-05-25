using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface IMortalidadRepository
{
    Task<Mortalidad?> GetByIdAsync(int id);
    Task<IEnumerable<Mortalidad>> GetAllAsync();
    Task AddAsync(Mortalidad entity);
    Task UpdateAsync(Mortalidad entity);
    Task DeleteAsync(int id);
}
