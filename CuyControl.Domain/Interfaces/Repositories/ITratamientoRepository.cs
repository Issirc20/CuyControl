using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface ITratamientoRepository
{
    Task<Tratamiento?> GetByIdAsync(int id);
    Task<IEnumerable<Tratamiento>> GetAllAsync();
    Task AddAsync(Tratamiento entity);
    Task UpdateAsync(Tratamiento entity);
    Task DeleteAsync(int id);
}
