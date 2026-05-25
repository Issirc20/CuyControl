using CuyControl.Domain.Entities;

namespace CuyControl.Domain.Interfaces.Repositories;

public interface IInventarioAlimentoRepository
{
    Task<InventarioAlimento?> GetByIdAsync(int id);
    Task<IEnumerable<InventarioAlimento>> GetAllAsync();
    Task AddAsync(InventarioAlimento entity);
    Task UpdateAsync(InventarioAlimento entity);
    Task DeleteAsync(int id);
}
