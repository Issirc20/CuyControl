using CuyControl.Domain.Entities;

namespace CuyControl.Application.Interfaces.Repositories;

public interface IAlimentacionRepository
{
    Task<IEnumerable<Alimentacion>> GetAllAsync();
    Task<Alimentacion?> GetByIdAsync(int id);
    Task<IEnumerable<Alimentacion>> GetByJaulaAsync(int jaulaId);
    Task<Alimentacion> AddAsync(Alimentacion entity);
    Task<Alimentacion> UpdateAsync(Alimentacion entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> SaveChangesAsync();
}