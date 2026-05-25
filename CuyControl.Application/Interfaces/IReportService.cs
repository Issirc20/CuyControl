using CuyControl.Application.DTOs;

namespace CuyControl.Application.Interfaces;

/// <summary>
/// Interface de servicio para reportes.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Obtiene los datos del dashboard.
    /// </summary>
    /// <returns>DTO con datos del dashboard.</returns>
    Task<DashboardDto> ObtenerDashboardAsync();

    /// <summary>
    /// Obtiene reporte de ventas por período.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Lista de DTOs de ventas en el período.</returns>
    Task<IEnumerable<VentaDto>> ObtenerReporteVentasAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene reporte de mortalidad.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Diccionario con causas de mortalidad y cantidad.</returns>
    Task<Dictionary<string, int>> ObtenerReporteMortalidadAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene reporte de enfermedades.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Diccionario con nombres de enfermedades y cantidad de casos.</returns>
    Task<Dictionary<string, int>> ObtenerReporteEnfermedadesAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene reporte de reproducción.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Información sobre reproducciones exitosas y fallidas.</returns>
    Task<Dictionary<string, object>> ObtenerReporteReproduccionAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene reporte de alimentación.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Información sobre consumo de alimento.</returns>
    Task<Dictionary<string, decimal>> ObtenerReporteAlimentacionAsync(DateTime fechaInicio, DateTime fechaFin);

    /// <summary>
    /// Obtiene estadísticas de crecimiento de cuyes.
    /// </summary>
    /// <returns>Información sobre pesos promedio por edad.</returns>
    Task<Dictionary<string, decimal>> ObtenerEstadisticasCrecimientoAsync();
}
