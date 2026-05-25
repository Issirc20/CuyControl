using CuyControl.Application.DTOs;

namespace CuyControl.Application.Interfaces;

public interface IAlimentacionService
{
    Task<IEnumerable<AlimentacionDto>> ObtenerTodasAlimentacionesAsync();
    Task<AlimentacionDto?> ObtenerAlimentacionPorIdAsync(int id);
    Task<bool> CrearAlimentacionAsync(AlimentacionDto dto);
    Task<bool> ActualizarAlimentacionAsync(int id, AlimentacionDto dto);
    Task<bool> EliminarAlimentacionAsync(int id);
}