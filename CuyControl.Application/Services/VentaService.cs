using CuyControl.Application.DTOs;
using CuyControl.Application.Interfaces;
using CuyControl.Application.Mappings;
using CuyControl.Application.Validators;
using CuyControl.Application.Interfaces.Repositories;

namespace CuyControl.Application.Services;

/// <summary>
/// Servicio de aplicación para operaciones con ventas.
/// </summary>
public class VentaService : IVentaService
{
    private readonly IVentaRepository _repository;
    private readonly VentaValidator _validator;

    public VentaService(IVentaRepository repository, VentaValidator validator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<IEnumerable<VentaDto>> ObtenerTodasVentasAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(MappingProfile.MapToVentaDto).ToList();
    }

    public async Task<VentaDto?> ObtenerVentaPorIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : MappingProfile.MapToVentaDto(entity);
    }

    public async Task<IEnumerable<VentaDto>> ObtenerVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var entities = await _repository.GetByFechaAsync(fechaInicio, fechaFin);
        return entities.Select(MappingProfile.MapToVentaDto).ToList();
    }

    public async Task<decimal> ObtenerTotalVentasPorFechaAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _repository.GetTotalByFechaAsync(fechaInicio, fechaFin);
    }

    public async Task<VentaDto> CrearVentaAsync(VentaDto ventaDto)
    {
        var errores = _validator.ValidarCreacion(ventaDto).ToList();
        if (errores.Any())
            throw new ArgumentException(string.Join("; ", errores));

        var entity = MappingProfile.MapToVenta(ventaDto);
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return MappingProfile.MapToVentaDto(entity);
    }

    public async Task<VentaDto> ActualizarVentaAsync(int id, VentaDto ventaDto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException("Venta no encontrada");

        var errores = _validator.ValidarActualizacion(ventaDto).ToList();
        if (errores.Any())
            throw new ArgumentException(string.Join("; ", errores));

        existing.FechaVenta = ventaDto.FechaVenta;
        existing.PrecioUnitario = ventaDto.PrecioUnitario;
        existing.Cantidad = ventaDto.Cantidad;
        existing.PrecioTotal = ventaDto.PrecioTotal;
        existing.NombreComprador = ventaDto.NombreComprador;
        existing.ContactoComprador = ventaDto.ContactoComprador;
        existing.MetodoPago = ventaDto.MetodoPago;
        existing.Observaciones = ventaDto.Observaciones;

        await _repository.UpdateAsync(existing);
        await _repository.SaveChangesAsync();

        return MappingProfile.MapToVentaDto(existing);
    }

    public async Task<bool> EliminarVentaAsync(int id)
    {
        var exists = await _repository.GetByIdAsync(id);
        if (exists == null)
            return false;

        var removed = await _repository.DeleteAsync(id);
        if (!removed)
            return false;

        return await _repository.SaveChangesAsync();
    }

    public async Task<Dictionary<string, decimal>> ObtenerEstadisticasVentasPorMetodoPagoAsync()
    {
        var ventas = await _repository.GetAllAsync();
        return ventas
            .GroupBy(v => v.MetodoPago ?? "Desconocido")
            .ToDictionary(g => g.Key, g => g.Sum(v => v.PrecioTotal));
    }
}
