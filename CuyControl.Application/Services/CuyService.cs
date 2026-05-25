using CuyControl.Application.DTOs;
using CuyControl.Application.Interfaces;
using CuyControl.Application.Mappings;
using CuyControl.Application.Validators;
using CuyControl.Application.Interfaces.Repositories;

namespace CuyControl.Application.Services;

public class CuyService : ICuyService
{
    private readonly ICuyRepository _repository;
    private readonly CuyValidator _validator;

    public CuyService(ICuyRepository repository, CuyValidator validator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<IEnumerable<CuyDto>> ObtenerTodosCuyesAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MappingProfile.MapToCuyDto).ToList();
    }

    public async Task<CuyDto?> ObtenerCuyPorIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MappingProfile.MapToCuyDto(entity);
    }

    public async Task<bool> CrearCuyAsync(CuyDto cuyDto)
    {
        var errores = _validator.ValidarCreacion(cuyDto).ToList();
        if (errores.Any())
            throw new ArgumentException(string.Join("; ", errores));

        var entity = MappingProfile.MapToCuy(cuyDto);
        await _repository.AddAsync(entity);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> ActualizarCuyAsync(int id, CuyDto cuyDto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return false;

        var errores = _validator.ValidarActualizacion(cuyDto).ToList();
        if (errores.Any())
            throw new ArgumentException(string.Join("; ", errores));

        // Actualizar campos permitidos
        existing.Codigo = cuyDto.Codigo;
        existing.FechaNacimiento = cuyDto.FechaNacimiento;
        existing.SexoId = cuyDto.SexoId;
        existing.EstadoId = cuyDto.EstadoId;
        existing.JaulaId = cuyDto.JaulaId;
        existing.PesoActual = cuyDto.PesoActual;
        existing.Raza = cuyDto.Raza;
        existing.Observaciones = cuyDto.Observaciones;

        await _repository.UpdateAsync(existing);
        return await _repository.SaveChangesAsync();
    }

    public async Task<bool> EliminarCuyAsync(int id)
    {
        var exists = await _repository.GetByIdAsync(id);
        if (exists == null)
            return false;

        var removed = await _repository.DeleteAsync(id);
        if (!removed)
            return false;

        return await _repository.SaveChangesAsync();
    }
}
