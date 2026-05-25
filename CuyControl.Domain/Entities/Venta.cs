using CuyControl.Domain.Interfaces;

namespace CuyControl.Domain.Entities;

/// <summary>
/// Entidad que registra las ventas de cuyes.
/// </summary>
public class Venta : IBaseEntity, IAuditable
{
    /// <summary>
    /// Identificador único del registro de venta.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identificador del cuy vendido.
    /// </summary>
    public int CuyId { get; set; }

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

    /// <summary>
    /// Fecha de creación del registro.
    /// </summary>
    public DateTime FechaCreacion { get; set; }

    /// <summary>
    /// Usuario que creó el registro.
    /// </summary>
    public int UsuarioCreacionId { get; set; }

    /// <summary>
    /// Fecha de última modificación del registro.
    /// </summary>
    public DateTime? FechaModificacion { get; set; }

    /// <summary>
    /// Usuario que realizó la última modificación.
    /// </summary>
    public int? UsuarioModificacionId { get; set; }

    // Relaciones
    public virtual Cuy? Cuy { get; set; }
    public virtual Usuario? UsuarioCreacion { get; set; }
    public virtual Usuario? UsuarioModificacion { get; set; }
}
