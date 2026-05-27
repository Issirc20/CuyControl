using CuyControl.Domain.Interfaces;
using CuyControl.Domain.Enums;

namespace CuyControl.Domain.Entities;

public class MovimientoAlimento : IBaseEntity, IAuditable
{
    public int Id { get; set; }
    public int InventarioAlimentoId { get; set; }
    public TipoMovimientoAlimentoEnum TipoMovimiento { get; set; }
    public decimal Cantidad { get; set; }
    public DateTime FechaMovimiento { get; set; } = DateTime.Now;
    public string? Observaciones { get; set; }

    public int UsuarioId { get; set; }

    // Auditoría
    public DateTime FechaCreacion { get; set; }
    public int UsuarioCreacionId { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public int? UsuarioModificacionId { get; set; }

    // Relaciones
    public virtual InventarioAlimento? InventarioAlimento { get; set; }
    public virtual Usuario? Usuario { get; set; }
}
