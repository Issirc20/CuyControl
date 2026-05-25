using CuyControl.Application.DTOs;

namespace CuyControl.Application.Interfaces;

public interface ICuyService
{
    Task<IEnumerable<CuyDto>> ObtenerTodosCuyesAsync();

    Task<CuyDto?> ObtenerCuyPorIdAsync(int id);

    Task<bool> CrearCuyAsync(CuyDto cuyDto);

    Task<bool> ActualizarCuyAsync(int id, CuyDto cuyDto);

    Task<bool> EliminarCuyAsync(int id);
}