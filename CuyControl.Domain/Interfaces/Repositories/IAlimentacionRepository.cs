using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface IAlimentacionRepository
{
    Task<Alimentacion?> GetByIdAsync(int id);
    Task<IEnumerable<Alimentacion>> GetAllAsync();
    Task AddAsync(Alimentacion entity);
    Task UpdateAsync(Alimentacion entity);
    Task DeleteAsync(int id);
}
