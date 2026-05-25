using CuyControl.Domain.Entities;

namespace CuyControl.Infrastructure.Repositories;

/// <summary>
/// Repositorio para operaciones con entidad Venta.
/// </summary>
public interface IVentaRepository : IRepository<Venta>
{
    /// <summary>
    /// Obtiene ventas en un rango de fechas.
    /// </summary>
    Task<IEnumerable<Venta>> GetByFechaAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene total de ventas en un período.
    /// </summary>
    Task<decimal> GetTotalByFechaAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene ventas por cuy.
    /// </summary>
    Task<IEnumerable<Venta>> GetByCuyAsync(int cuyId);

    /// <summary>
    /// Cuenta ventas en un período.
    /// </summary>
    Task<int> CountByFechaAsync(DateTime fechaInicio, DateTime fechaFin);
}
