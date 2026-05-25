namespace CuyControl.Application.DTOs;

public class MovimientoAlimentoDto
{
    public int Id { get; set; }
    public int InventarioAlimentoId { get; set; }
    public int TipoMovimiento { get; set; }
    public decimal Cantidad { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public string? Observaciones { get; set; }
}
