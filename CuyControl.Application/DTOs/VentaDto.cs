namespace CuyControl.Application.DTOs;

/// <summary>
/// DTO para transferencia de datos de Venta.
/// </summary>
public class VentaDto
{
    /// <summary>
    /// Identificador único de la venta.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cuy vendido.
    /// </summary>
    public int CuyId { get; set; }

    /// <summary>
    /// Código del cuy vendido.
    /// </summary>
    public string? Cuycodigo { get; set; }

    /// <summary>
    /// Fecha de la venta.
    /// </summary>
    public DateTime FechaVenta { get; set; }

    /// <summary>
    /// Precio unitario de venta.
    /// </summary>
    public decimal PrecioUnitario { get; set; }

    /// <summary>
    /// Cantidad de cuyes vendidos.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Precio total de la venta.
    /// </summary>
    public decimal PrecioTotal { get; set; }

    /// <summary>
    /// Nombre del comprador.
    /// </summary>
    public string NombreComprador { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono o contacto del comprador.
    /// </summary>
    public string? ContactoComprador { get; set; }

    /// <summary>
    /// Método de pago utilizado.
    /// </summary>
    public string? MetodoPago { get; set; }

    /// <summary>
    /// Observaciones sobre la venta.
    /// </summary>
    public string? Observaciones { get; set; }
}
