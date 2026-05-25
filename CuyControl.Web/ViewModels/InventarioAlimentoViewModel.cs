namespace CuyControl.Web.ViewModels;

public class InventarioAlimentoViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Stock { get; set; }
    public string Unidad { get; set; } = string.Empty;
}
