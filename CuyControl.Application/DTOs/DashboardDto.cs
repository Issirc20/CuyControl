namespace CuyControl.Application.DTOs;

/// <summary>
/// DTO para datos del dashboard.
/// </summary>
public class DashboardDto
{
    /// <summary>
    /// Cantidad total de cuyes en el sistema.
    /// </summary>
    public int TotalCuyes { get; set; }

    /// <summary>
    /// Cantidad de cuyes activos.
    /// </summary>
    public int CuyesActivos { get; set; }

    /// <summary>
    /// Cantidad de cuyes enfermos.
    /// </summary>
    public int CuyesEnfermos { get; set; }

    /// <summary>
    /// Cantidad de cuyes muertos.
    /// </summary>
    public int CuyesMuertos { get; set; }

    /// <summary>
    /// Cantidad de cuyes vendidos.
    /// </summary>
    public int CuyesVendidos { get; set; }

    /// <summary>
    /// Total de galpones registrados.
    /// </summary>
    public int TotalGalpones { get; set; }

    /// <summary>
    /// Total de ventas realizadas.
    /// </summary>
    public decimal TotalVentas { get; set; }

    /// <summary>
    /// Total de ventas en el mes actual.
    /// </summary>
    public decimal VentasEsteMes { get; set; }

    /// <summary>
    /// Cantidad de registros en el mes actual.
    /// </summary>
    public int RegistrosEsteMes { get; set; }

    /// <summary>
    /// Inventario actual de alimento en kilogramos.
    /// </summary>
    public decimal InventarioAlimento { get; set; }

    /// <summary>
    /// Porcentaje de ocupación de jaulas.
    /// </summary>
    public decimal PorcentajeOcupacionJaulas { get; set; }

    /// <summary>
    /// Cuyes nacidos en el mes actual.
    /// </summary>
    public int CuyesNacidosEsteMes { get; set; }

    /// <summary>
    /// Peso promedio de los cuyes en gramos.
    /// </summary>
    public decimal PesoPromedioCuyes { get; set; }

    /// <summary>
    /// Última actualización del dashboard.
    /// </summary>
    public DateTime UltimaActualizacion { get; set; }
}
