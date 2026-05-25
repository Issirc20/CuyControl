using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface IPartoRepository
{
    Task<Parto?> GetByIdAsync(int id);
    Task<IEnumerable<Parto>> GetAllAsync();
    Task AddAsync(Parto entity);
    Task UpdateAsync(Parto entity);
    Task DeleteAsync(int id);
}
