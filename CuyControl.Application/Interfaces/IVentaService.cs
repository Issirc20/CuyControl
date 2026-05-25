using CuyControl.Application.DTOs;

namespace CuyControl.Application.Interfaces;

/// <summary>
/// Interface de servicio para operaciones con ventas.
/// </summary>
public interface IVentaService
{
    /// <summary>
    /// Obtiene todas las ventas.
    /// </summary>
    /// <returns>Lista de DTOs de ventas.</returns>
    Task<IEnumerable<VentaDto>> ObtenerTodasVentasAsync();

    /// <summary>
    /// Obtiene una venta por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la venta.</param>
    /// <returns>DTO de la venta o null si no existe.</returns>
    Task<VentaDto?> ObtenerVentaPorIdAsync(int id);

    /// <summary>
    /// Obtiene ventas en un rango de fechas.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Lista de DTOs de ventas en el rango.</returns>
    Task<IEnumerable<VentaDto>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene el total de ventas en un periodo.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Total de ventas en el periodo.</returns>
    Task<decimal> ObtenerTotalVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Crea una nueva venta.
    /// </summary>
    /// <param name="ventaDto">DTO de la venta a crear.</param>
    /// <returns>DTO de la venta creada.</returns>
    Task<VentaDto> CrearVentaAsync(VentaDto ventaDto);

    /// <summary>
    /// Actualiza una venta existente.
    /// </summary>
    /// <param name="id">Identificador de la venta.</param>
    /// <param name="ventaDto">DTO con datos actualizados.</param>
    /// <returns>DTO de la venta actualizada.</returns>
    Task<VentaDto> ActualizarVentaAsync(int id, VentaDto ventaDto);

    /// <summary>
    /// Elimina una venta.
    /// </summary>
    /// <param name="id">Identificador de la venta a eliminar.</param>
    /// <returns>True si se eliminó exitosamente.</returns>
    Task<bool> EliminarVentaAsync(int id);

    /// <summary>
    /// Obtiene estadísticas de ventas por método de pago.
    /// </summary>
    /// <returns>Diccionario con método de pago y total.</returns>
    Task<Dictionary<string, decimal>> ObtenerEstadisticasVentasPorMetodoPagoAsync();
}
