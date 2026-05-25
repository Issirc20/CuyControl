using CuyControl.Application.DTOs;
using CuyControl.Application.Interfaces;
using CuyControl.Application.Interfaces.Repositories;
using CuyControl.Application.Mappings;
using CuyControl.Domain.Entities;

namespace CuyControl.Application.Services;

public class AlimentacionService : IAlimentacionService
{
    private readonly IAlimentacionRepository _repository;

    public AlimentacionService(IAlimentacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AlimentacionDto>> ObtenerTodasAlimentacionesAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MappingProfile.MapToAlimentacionDto).ToList();
    }

    public async Task<AlimentacionDto?> ObtenerAlimentacionPorIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MappingProfile.MapToAlimentacionDto(entity);
    }

    public async Task<bool> CrearAlimentacionAsync(AlimentacionDto dto)
    {
        var entity = MappingProfile.MapToAlimentacion(dto);
        await _repository.AddAsync(entity);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> ActualizarAlimentacionAsync(int id, AlimentacionDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        existing.CantidadAlimento = dto.CantidadAlimento;
        existing.TipoAlimento = dto.TipoAlimento;
        existing.Observaciones = dto.Observaciones;
        existing.FechaAlimentacion = dto.FechaAlimentacion;

        await _repository.UpdateAsync(existing);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> EliminarAlimentacionAsync(int id)
    {
        var removed = await _repository.DeleteAsync(id);
        if (!removed) return false;
        return await _repository.SaveChangesAsync();
    }
}