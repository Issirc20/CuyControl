using CuyControl.Application.DTOs;

namespace CuyControl.Application.Validators;

/// <summary>
/// Validador para VentaDto.
/// </summary>
public class VentaValidator
{
    /// <summary>
    /// Valida que un VentaDto tenga datos válidos para creación.
    /// </summary>
    /// <param name="ventaDto">DTO a validar.</param>
    /// <returns>Lista de mensajes de error, vacía si es válido.</returns>
    public IEnumerable<string> ValidarCreacion(VentaDto ventaDto)
    {
        var errores = new List<string>();

        if (ventaDto.CuyId <= 0)
            errores.Add("El cuy es obligatorio.");

        if (ventaDto.FechaVenta == default)
            errores.Add("La fecha de venta es obligatoria.");

        if (ventaDto.FechaVenta > DateTime.Now)
            errores.Add("La fecha de venta no puede ser posterior a hoy.");

        if (ventaDto.PrecioUnitario <= 0)
            errores.Add("El precio unitario debe ser mayor a 0.");

        if (ventaDto.Cantidad <= 0)
            errores.Add("La cantidad debe ser mayor a 0.");

        if (ventaDto.PrecioTotal <= 0)
            errores.Add("El precio total debe ser mayor a 0.");

        if (string.IsNullOrWhiteSpace(ventaDto.NombreComprador))
            errores.Add("El nombre del comprador es obligatorio.");

        if (ventaDto.NombreComprador?.Length > 100)
            errores.Add("El nombre del comprador no debe exceder 100 caracteres.");

        // Validar que el precio total sea consistente
        var precioTotalCalculado = ventaDto.PrecioUnitario * ventaDto.Cantidad;
        if (Math.Abs(ventaDto.PrecioTotal - precioTotalCalculado) > 0.01m)
            errores.Add("El precio total no corresponde con el precio unitario y la cantidad.");

        return errores;
    }

    /// <summary>
    /// Valida que un VentaDto tenga datos válidos para actualización.
    /// </summary>
    /// <param name="ventaDto">DTO a validar.</param>
    /// <returns>Lista de mensajes de error, vacía si es válido.</returns>
    public IEnumerable<string> ValidarActualizacion(VentaDto ventaDto)
    {
        var errores = ValidarCreacion(ventaDto).ToList();

        if (ventaDto.Id <= 0)
            errores.Add("El identificador de la venta es requerido.");

        return errores;
    }

    /// <summary>
    /// Valida el rango de fechas para reportes.
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio.</param>
    /// <param name="fechaFin">Fecha de fin.</param>
    /// <returns>Lista de mensajes de error, vacía si es válido.</returns>
    public IEnumerable<string> ValidarRangoFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        var errores = new List<string>();

        if (fechaInicio > fechaFin)
            errores.Add("La fecha de inicio no puede ser posterior a la fecha de fin.");

        if (fechaFin > DateTime.Now)
            errores.Add("La fecha de fin no puede ser posterior a hoy.");

        return errores;
    }
}
