using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface IReproduccionRepository
{
    Task<Reproduccion?> GetByIdAsync(int id);
    Task<IEnumerable<Reproduccion>> GetAllAsync();
    Task AddAsync(Reproduccion entity);
    Task UpdateAsync(Reproduccion entity);
    Task DeleteAsync(int id);
}
