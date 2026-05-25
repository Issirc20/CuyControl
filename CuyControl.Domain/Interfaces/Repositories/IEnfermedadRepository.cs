using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface IEnfermedadRepository
{
    Task<Enfermedad?> GetByIdAsync(int id);
    Task<IEnumerable<Enfermedad>> GetAllAsync();
    Task AddAsync(Enfermedad entity);
    Task UpdateAsync(Enfermedad entity);
    Task DeleteAsync(int id);
}
