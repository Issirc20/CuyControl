namespace CuyControl.Web.Helpers;

/// <summary>
/// Clase auxiliar para utilidades de formateo y validación.
/// </summary>
public static class FormattingHelper
{
    /// <summary>
    /// Formatea una moneda a formato de presentación.
    /// </summary>
    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("C");
    }

    /// <summary>
    /// Formatea una fecha a formato corto.
    /// </summary>
    public static string FormatDate(DateTime date)
    {
        return date.ToString("dd/MM/yyyy");
    }

    /// <summary>
    /// Formatea una fecha y hora.
    /// </summary>
    public static string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("dd/MM/yyyy HH:mm");
    }

    /// <summary>
    /// Obtiene el nombre del sexo a partir del ID.
    /// </summary>
    public static string GetSexoNombre(int sexoId)
    {
        return sexoId switch
        {
            1 => "Macho",
            2 => "Hembra",
            _ => "Desconocido"
        };
    }

    /// <summary>
    /// Obtiene el nombre del estado a partir del ID.
    /// </summary>
    public static string GetEstadoNombre(int estadoId)
    {
        return estadoId switch
        {
            1 => "Activo",
            2 => "Enfermo",
            3 => "Muerto",
            4 => "Vendido",
            5 => "En Cuarentena",
            _ => "Desconocido"
        };
    }

    /// <summary>
    /// Obtiene el color de Bootstrap para un estado.
    /// </summary>
    public static string GetEstadoBadgeClass(int estadoId)
    {
        return estadoId switch
        {
            1 => "badge bg-success",
            2 => "badge bg-warning",
            3 => "badge bg-danger",
            4 => "badge bg-info",
            5 => "badge bg-secondary",
            _ => "badge bg-dark"
        };
    }

    /// <summary>
    /// Trunca un texto a una longitud máxima.
    /// </summary>
    public static string Truncate(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
    }

    /// <summary>
    /// Calcula la edad en días desde una fecha.
    /// </summary>
    public static int CalcularEdadEnDias(DateTime fechaNacimiento)
    {
        return (int)(DateTime.Now - fechaNacimiento).TotalDays;
    }

    /// <summary>
    /// Formatea el peso en gramos a un formato legible.
    /// </summary>
    public static string FormatPeso(decimal pesoGramos)
    {
        if (pesoGramos < 1000)
            return $"{pesoGramos:F2} g";

        var pesoKg = pesoGramos / 1000;
        return $"{pesoKg:F2} kg";
    }
}
