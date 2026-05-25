using CuyControl.Domain.Entities;

namespace CuyControl.Application.Interfaces.Repositories;

public interface IVentaRepository
{
    Task<IEnumerable<Venta>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<decimal> GetTotalByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<Venta>> GetByCuyAsync(int cuyId);
    Task<int> CountByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
    Task<IEnumerable<Venta>> GetAllAsync();
    Task<Venta?> GetByIdAsync(int id);
    Task<Venta> AddAsync(Venta entity);
    Task<Venta> UpdateAsync(Venta entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> SaveChangesAsync();
}