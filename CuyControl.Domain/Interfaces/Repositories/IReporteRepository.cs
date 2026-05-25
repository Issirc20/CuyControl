using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface IReporteRepository
{
    Task<Reporte?> GetByIdAsync(int id);
    Task<IEnumerable<Reporte>> GetAllAsync();
    Task AddAsync(Reporte entity);
    Task UpdateAsync(Reporte entity);
    Task DeleteAsync(int id);
}
