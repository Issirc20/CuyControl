namespace CuyControl.Web.ViewModels;

/// <summary>
/// ViewModel para mostrar datos del dashboard.
/// </summary>
public class DashboardViewModel
{
    /// <summary>
    /// Cantidad total de cuyes.
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
    /// Total de galpones.
    /// </summary>
    public int TotalGalpones { get; set; }

    /// <summary>
    /// Total de jaulas.
    /// </summary>
    public int TotalJaulas { get; set; }

    /// <summary>
    /// Total de enfermedades.
    /// </summary>
    public int TotalEnfermedades { get; set; }

    /// <summary>
    /// Total de ventas.
    /// </summary>
    public decimal TotalVentas { get; set; }

    /// <summary>
    /// Ventas en el mes actual.
    /// </summary>
    public decimal VentasEsteMes { get; set; }

    /// <summary>
    /// Registros en el mes actual.
    /// </summary>
    public int RegistrosEsteMes { get; set; }

    /// <summary>
    /// Inventario de alimento.
    /// </summary>
    public decimal InventarioAlimento { get; set; }

    /// <summary>
    /// Stock bajo.
    /// </summary>
    public int StockBajo { get; set; }

    /// <summary>
    /// Porcentaje de ocupación de jaulas.
    /// </summary>
    public decimal PorcentajeOcupacionJaulas { get; set; }

    /// <summary>
    /// Cuyes nacidos este mes.
    /// </summary>
    public int CuyesNacidosEsteMes { get; set; }

    /// <summary>
    /// Peso promedio de los cuyes.
    /// </summary>
    public decimal PesoPromedioCuyes { get; set; }

    /// <summary>
    /// Última actualización del dashboard.
    /// </summary>
    public DateTime UltimaActualizacion { get; set; }

    /// <summary>
    /// Total de machos.
    /// </summary>
    public int TotalMachos { get; set; }

    /// <summary>
    /// Total de hembras.
    /// </summary>
    public int TotalHembras { get; set; }

    /// <summary>
    /// Meses de ventas.
    /// </summary>
    public List<string> MesesVentas { get; set; } = new();

    /// <summary>
    /// Montos de ventas.
    /// </summary>
    public List<decimal> MontosVentas { get; set; } = new();

    /// <summary>
    /// Labels para estados de cuyes.
    /// </summary>
    public List<string> EstadosCuyesLabels { get; set; } = new();

    /// <summary>
    /// Datos para estados de cuyes.
    /// </summary>
    public List<int> EstadosCuyesData { get; set; } = new();

    /// <summary>
    /// Labels de alimentos.
    /// </summary>
    public List<string> AlimentosLabels { get; set; } = new();

    /// <summary>
    /// Datos de alimentos.
    /// </summary>
    public List<decimal> AlimentosData { get; set; } = new();

    public int CuyesDisponibles { get; set; }
    public int Crias { get; set; }
    public int Recrias { get; set; }
    public int Reproductores { get; set; }
    public int Gestantes { get; set; }
    public int Lactantes { get; set; }
    public int ParaVenta { get; set; }
}