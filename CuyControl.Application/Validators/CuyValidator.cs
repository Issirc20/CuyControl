using CuyControl.Application.DTOs;

namespace CuyControl.Application.Validators;

/// <summary>
/// Validador para CuyDto.
/// </summary>
public class CuyValidator
{
    /// <summary>
    /// Valida que un CuyDto tenga datos válidos para creación.
    /// </summary>
    /// <param name="cuyDto">DTO a validar.</param>
    /// <returns>Lista de mensajes de error, vacía si es válido.</returns>
    public IEnumerable<string> ValidarCreacion(CuyDto cuyDto)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(cuyDto.Codigo))
            errores.Add("El código del cuy es obligatorio.");

        if (cuyDto.Codigo?.Length > 50)
            errores.Add("El código del cuy no debe exceder 50 caracteres.");

        if (cuyDto.FechaNacimiento == default)
            errores.Add("La fecha de nacimiento es obligatoria.");

        if (cuyDto.FechaNacimiento > DateTime.Now)
            errores.Add("La fecha de nacimiento no puede ser posterior a hoy.");

        if (cuyDto.SexoId <= 0)
            errores.Add("El sexo del cuy es obligatorio.");

        if (cuyDto.JaulaId <= 0)
            errores.Add("La jaula es obligatoria.");

        if (cuyDto.PesoActual <= 0)
            errores.Add("El peso actual debe ser mayor a 0.");

        if (cuyDto.PesoActual > 10000) // 10 kg máximo
            errores.Add("El peso actual no puede ser mayor a 10000 gramos.");

        return errores;
    }

    /// <summary>
    /// Valida que un CuyDto tenga datos válidos para actualización.
    /// </summary>
    /// <param name="cuyDto">DTO a validar.</param>
    /// <returns>Lista de mensajes de error, vacía si es válido.</returns>
    public IEnumerable<string> ValidarActualizacion(CuyDto cuyDto)
    {
        var errores = ValidarCreacion(cuyDto).ToList();

        if (cuyDto.Id <= 0)
            errores.Add("El identificador del cuy es requerido.");

        return errores;
    }

    /// <summary>
    /// Valida el cambio de estado del cuy.
    /// </summary>
    /// <param name="nuevoEstadoId">Nuevo estado.</param>
    /// <returns>Lista de mensajes de error, vacía si es válido.</returns>
    public IEnumerable<string> ValidarCambioEstado(int nuevoEstadoId)
    {
        var errores = new List<string>();

        if (nuevoEstadoId <= 0 || nuevoEstadoId > 5)
            errores.Add("El estado del cuy es inválido.");

        return errores;
    }

    /// <summary>
    /// Valida el registro de peso.
    /// </summary>
    /// <param name="peso">Peso en gramos.</param>
    /// <returns>Lista de mensajes de error, vacía si es válido.</returns>
    public IEnumerable<string> ValidarPeso(decimal peso)
    {
        var errores = new List<string>();

        if (peso <= 0)
            errores.Add("El peso debe ser mayor a 0.");

        if (peso > 10000)
            errores.Add("El peso no puede ser mayor a 10000 gramos.");

        return errores;
    }
}
