# EJEMPLOS DE CÓDIGO - IMPLEMENTACIÓN DE SOLUCIONES

## 📝 Tabla de Contenidos
1. [InventarioAlimentoService](#inventarioalimentoservice)
2. [MovimientoAlimentoService](#movimientoalimentoservice)
3. [ReproduccionService](#reproduccionservice)
4. [PartoService](#partoservice)
5. [Refactorización de Controladores](#refactorización-de-controladores)
6. [Program.cs - Registro de Servicios](#programcs---registro-de-servicios)

---

## InventarioAlimentoService

### Archivo: `CuyControl.Application/Interfaces/Services/IInventarioAlimentoService.cs`

```csharp
namespace CuyControl.Application.Interfaces.Services;

public interface IInventarioAlimentoService
{
	/// <summary>
	/// Crea un nuevo inventario de alimento con validaciones
	/// </summary>
	/// <param name="inventarioDto">Datos del inventario a crear</param>
	/// <returns>InventarioAlimentoDto creado</returns>
	/// <exception cref="InvalidOperationException">Si los datos son inválidos</exception>
	Task<InventarioAlimentoDto> CrearInventarioAsync(InventarioAlimentoDto inventarioDto);

	/// <summary>
	/// Actualiza un inventario existente
	/// </summary>
	/// <param name="id">ID del inventario</param>
	/// <param name="inventarioDto">Nuevos datos</param>
	/// <returns>InventarioAlimentoDto actualizado</returns>
	Task<InventarioAlimentoDto> ActualizarInventarioAsync(int id, InventarioAlimentoDto inventarioDto);

	/// <summary>
	/// Obtiene todos los inventarios de alimento
	/// </summary>
	/// <returns>Lista de InventarioAlimentoDto</returns>
	Task<IEnumerable<InventarioAlimentoDto>> ObtenerTodosAsync();

	/// <summary>
	/// Obtiene un inventario específico por ID
	/// </summary>
	/// <param name="id">ID del inventario</param>
	/// <returns>InventarioAlimentoDto o null</returns>
	Task<InventarioAlimentoDto?> ObtenerPorIdAsync(int id);

	/// <summary>
	/// Elimina un inventario
	/// </summary>
	/// <param name="id">ID del inventario a eliminar</param>
	/// <exception cref="InvalidOperationException">Si no existe o hay movimientos activos</exception>
	Task EliminarAsync(int id);
}
```

### Archivo: `CuyControl.Application/Services/InventarioAlimentoService.cs`

```csharp
namespace CuyControl.Application.Services;

using CuyControl.Application.Interfaces.Services;
using CuyControl.Domain.Entities;
using CuyControl.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

public class InventarioAlimentoService : IInventarioAlimentoService
{
	private readonly IInventarioAlimentoRepository _repository;
	private readonly IMapper _mapper;
	private readonly ILogger<InventarioAlimentoService> _logger;

	public InventarioAlimentoService(
		IInventarioAlimentoRepository repository,
		IMapper mapper,
		ILogger<InventarioAlimentoService> logger)
	{
		_repository = repository ?? throw new ArgumentNullException(nameof(repository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<InventarioAlimentoDto> CrearInventarioAsync(
		InventarioAlimentoDto inventarioDto)
	{
		try
		{
			// ✅ Validación 1: Datos requeridos
			if (inventarioDto == null)
				throw new InvalidOperationException("Los datos del inventario son requeridos");

			if (string.IsNullOrWhiteSpace(inventarioDto.TipoAlimento))
				throw new InvalidOperationException("Tipo de alimento es requerido");

			// ✅ Validación 2: Cantidad no negativa
			if (inventarioDto.CantidadActual < 0)
				throw new InvalidOperationException(
					"Cantidad no puede ser negativa");

			// ✅ Validación 3: Verificar duplicidad de tipo
			var existe = await _repository.ExisteAsync(
				a => a.TipoAlimento.ToLower() == inventarioDto.TipoAlimento.ToLower());

			if (existe)
				throw new InvalidOperationException(
					$"Ya existe inventario para el tipo: {inventarioDto.TipoAlimento}");

			// Mapear DTO a entidad
			var inventario = _mapper.Map<InventarioAlimento>(inventarioDto);
			inventario.FechaCreacion = DateTime.Now;

			// Guardar en BD
			await _repository.AddAsync(inventario);
			await _repository.SaveChangesAsync();

			_logger.LogInformation(
				"Inventario de alimento creado exitosamente. " +
				"ID: {InventarioId}, Tipo: {TipoAlimento}, Cantidad: {Cantidad}",
				inventario.Id, inventario.TipoAlimento, inventario.CantidadActual);

			// Mapear de vuelta a DTO
			return _mapper.Map<InventarioAlimentoDto>(inventario);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning(
				"Error al crear inventario de alimento. Tipo: {Tipo}, Error: {Error}",
				inventarioDto?.TipoAlimento, ex.Message);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Error inesperado al crear inventario de alimento");
			throw new InvalidOperationException(
				"Error al crear el inventario de alimento", ex);
		}
	}

	public async Task<InventarioAlimentoDto> ActualizarInventarioAsync(
		int id, InventarioAlimentoDto inventarioDto)
	{
		try
		{
			// Obtener inventario existente
			var inventario = await _repository.GetByIdAsync(id);

			if (inventario == null)
				throw new InvalidOperationException(
					$"Inventario con ID {id} no encontrado");

			// ✅ Validar cantidad
			if (inventarioDto.CantidadActual < 0)
				throw new InvalidOperationException(
					"Cantidad no puede ser negativa");

			// Actualizar campos
			inventario.TipoAlimento = inventarioDto.TipoAlimento ?? inventario.TipoAlimento;
			inventario.CantidadActual = inventarioDto.CantidadActual;
			inventario.FechaModificacion = DateTime.Now;

			// Guardar cambios
			await _repository.UpdateAsync(inventario);
			await _repository.SaveChangesAsync();

			_logger.LogInformation(
				"Inventario actualizado. ID: {Id}, Cantidad nueva: {Cantidad}",
				id, inventarioDto.CantidadActual);

			return _mapper.Map<InventarioAlimentoDto>(inventario);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning(
				"Error al actualizar inventario ID {Id}. Error: {Error}",
				id, ex.Message);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error inesperado al actualizar inventario ID {Id}", id);
			throw new InvalidOperationException(
				"Error al actualizar el inventario", ex);
		}
	}

	public async Task<IEnumerable<InventarioAlimentoDto>> ObtenerTodosAsync()
	{
		try
		{
			var inventarios = await _repository.GetAllAsync();
			return _mapper.Map<IEnumerable<InventarioAlimentoDto>>(inventarios);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al obtener todos los inventarios");
			throw new InvalidOperationException(
				"Error al obtener los inventarios", ex);
		}
	}

	public async Task<InventarioAlimentoDto?> ObtenerPorIdAsync(int id)
	{
		try
		{
			var inventario = await _repository.GetByIdAsync(id);

			if (inventario == null)
			{
				_logger.LogWarning("Inventario ID {Id} no encontrado", id);
				return null;
			}

			return _mapper.Map<InventarioAlimentoDto>(inventario);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al obtener inventario ID {Id}", id);
			throw new InvalidOperationException(
				"Error al obtener el inventario", ex);
		}
	}

	public async Task EliminarAsync(int id)
	{
		try
		{
			var inventario = await _repository.GetByIdAsync(id);

			if (inventario == null)
				throw new InvalidOperationException(
					$"Inventario con ID {id} no encontrado");

			// Podría añadirse validación: no eliminar si hay movimientos activos
			// var tieneMovimientos = await _movimientoRepository.ExisteAsync(
			//     m => m.InventarioAlimentoId == id);
			// if (tieneMovimientos)
			//     throw new InvalidOperationException(
			//         "No se puede eliminar inventario que tiene movimientos");

			await _repository.DeleteAsync(inventario);
			await _repository.SaveChangesAsync();

			_logger.LogInformation(
				"Inventario eliminado. ID: {Id}, Tipo: {Tipo}",
				id, inventario.TipoAlimento);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning("Error al eliminar inventario ID {Id}. Error: {Error}",
				id, ex.Message);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error inesperado al eliminar inventario ID {Id}", id);
			throw new InvalidOperationException(
				"Error al eliminar el inventario", ex);
		}
	}
}
```

---

## MovimientoAlimentoService

### Archivo: `CuyControl.Application/Interfaces/Services/IMovimientoAlimentoService.cs`

```csharp
namespace CuyControl.Application.Interfaces.Services;

public interface IMovimientoAlimentoService
{
	/// <summary>
	/// Registra un movimiento de alimento (entrada/salida) con validación de stock
	/// </summary>
	/// <param name="movimientoDto">Datos del movimiento</param>
	/// <returns>MovimientoAlimentoDto registrado</returns>
	/// <exception cref="InvalidOperationException">Si stock es insuficiente o datos inválidos</exception>
	Task<MovimientoAlimentoDto> RegistrarMovimientoAsync(
		MovimientoAlimentoDto movimientoDto);

	/// <summary>
	/// Obtiene el histórico de movimientos de un inventario
	/// </summary>
	/// <param name="inventarioAlimentoId">ID del inventario</param>
	/// <returns>Lista de movimientos</returns>
	Task<IEnumerable<MovimientoAlimentoDto>> ObtenerMovimientosAsync(
		int inventarioAlimentoId);

	/// <summary>
	/// Obtiene el stock actual de un inventario
	/// </summary>
	/// <param name="inventarioAlimentoId">ID del inventario</param>
	/// <returns>Cantidad actual en stock</returns>
	Task<decimal> ObtenerStockActualAsync(int inventarioAlimentoId);

	/// <summary>
	/// Obtiene movimientos en un rango de fechas
	/// </summary>
	/// <param name="desde">Fecha inicio</param>
	/// <param name="hasta">Fecha fin</param>
	/// <returns>Movimientos en el período</returns>
	Task<IEnumerable<MovimientoAlimentoDto>> ObtenerMovimientosEnRangoAsync(
		DateTime desde, DateTime hasta);
}
```

### Archivo: `CuyControl.Application/Services/MovimientoAlimentoService.cs`

```csharp
namespace CuyControl.Application.Services;

using CuyControl.Application.Interfaces.Services;
using CuyControl.Domain.Entities;
using CuyControl.Domain.Enums;
using CuyControl.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

public class MovimientoAlimentoService : IMovimientoAlimentoService
{
	private readonly IMovimientoAlimentoRepository _movimientoRepository;
	private readonly IInventarioAlimentoRepository _inventarioRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<MovimientoAlimentoService> _logger;

	public MovimientoAlimentoService(
		IMovimientoAlimentoRepository movimientoRepository,
		IInventarioAlimentoRepository inventarioRepository,
		IMapper mapper,
		ILogger<MovimientoAlimentoService> logger)
	{
		_movimientoRepository = movimientoRepository 
			?? throw new ArgumentNullException(nameof(movimientoRepository));
		_inventarioRepository = inventarioRepository 
			?? throw new ArgumentNullException(nameof(inventarioRepository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<MovimientoAlimentoDto> RegistrarMovimientoAsync(
		MovimientoAlimentoDto movimientoDto)
	{
		try
		{
			// ✅ Validación 1: Datos requeridos
			if (movimientoDto == null)
				throw new InvalidOperationException(
					"Los datos del movimiento son requeridos");

			// ✅ Validación 2: Cantidad debe ser positiva
			if (movimientoDto.Cantidad <= 0)
				throw new InvalidOperationException(
					"Cantidad debe ser mayor a cero");

			// ✅ Validación 3: Obtener inventario
			var inventario = await _inventarioRepository.GetByIdAsync(
				movimientoDto.InventarioAlimentoId);

			if (inventario == null)
				throw new InvalidOperationException(
					$"Inventario con ID {movimientoDto.InventarioAlimentoId} no encontrado");

			// ✅ Validación 4: Para salida, validar stock disponible
			if (movimientoDto.TipoMovimiento == TipoMovimientoAlimentoEnum.Salida)
			{
				if (inventario.CantidadActual < movimientoDto.Cantidad)
				{
					_logger.LogWarning(
						"Intento de salida con stock insuficiente. " +
						"Disponible: {Disponible}, Solicitado: {Solicitado}",
						inventario.CantidadActual, movimientoDto.Cantidad);

					throw new InvalidOperationException(
						$"Stock insuficiente. Disponible: {inventario.CantidadActual}, " +
						$"Solicitado: {movimientoDto.Cantidad}");
				}

				// ✅ Restar del stock
				inventario.CantidadActual -= movimientoDto.Cantidad;

				_logger.LogInformation(
					"Salida de alimento registrada. " +
					"Tipo: {TipoAlimento}, Cantidad: {Cantidad}, " +
					"Stock restante: {StockRestante}",
					inventario.TipoAlimento, movimientoDto.Cantidad,
					inventario.CantidadActual);
			}
			else if (movimientoDto.TipoMovimiento == TipoMovimientoAlimentoEnum.Entrada)
			{
				// ✅ Sumar al stock
				inventario.CantidadActual += movimientoDto.Cantidad;

				_logger.LogInformation(
					"Entrada de alimento registrada. " +
					"Tipo: {TipoAlimento}, Cantidad: {Cantidad}, " +
					"Stock total: {StockTotal}",
					inventario.TipoAlimento, movimientoDto.Cantidad,
					inventario.CantidadActual);
			}
			else
			{
				throw new InvalidOperationException(
					"Tipo de movimiento no válido");
			}

			// ✅ Crear registro del movimiento
			var movimiento = _mapper.Map<MovimientoAlimento>(movimientoDto);
			movimiento.FechaMovimiento = DateTime.Now;
			movimiento.FechaCreacion = DateTime.Now;

			// ✅ Actualizar timestamp del inventario
			inventario.FechaModificacion = DateTime.Now;

			// ✅ Guardar ambos cambios en transacción (EF Core maneja la transacción)
			await _movimientoRepository.AddAsync(movimiento);
			await _inventarioRepository.UpdateAsync(inventario);
			await _movimientoRepository.SaveChangesAsync();

			_logger.LogInformation(
				"Movimiento registrado exitosamente. " +
				"ID: {MovimientoId}, Inventario: {InventarioId}",
				movimiento.Id, inventario.Id);

			return _mapper.Map<MovimientoAlimentoDto>(movimiento);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning(
				"Error al registrar movimiento de alimento. Error: {Error}",
				ex.Message);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Error inesperado al registrar movimiento de alimento");
			throw new InvalidOperationException(
				"Error al registrar el movimiento de alimento", ex);
		}
	}

	public async Task<IEnumerable<MovimientoAlimentoDto>> ObtenerMovimientosAsync(
		int inventarioAlimentoId)
	{
		try
		{
			var movimientos = await _movimientoRepository
				.ObtenerMovimientosAsync(inventarioAlimentoId);

			return _mapper.Map<IEnumerable<MovimientoAlimentoDto>>(movimientos);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Error al obtener movimientos del inventario ID {Id}",
				inventarioAlimentoId);
			throw new InvalidOperationException(
				"Error al obtener los movimientos", ex);
		}
	}

	public async Task<decimal> ObtenerStockActualAsync(int inventarioAlimentoId)
	{
		try
		{
			var inventario = await _inventarioRepository
				.GetByIdAsync(inventarioAlimentoId);

			if (inventario == null)
				throw new InvalidOperationException(
					$"Inventario con ID {inventarioAlimentoId} no encontrado");

			return inventario.CantidadActual;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Error al obtener stock actual del inventario ID {Id}",
				inventarioAlimentoId);
			throw new InvalidOperationException(
				"Error al obtener el stock", ex);
		}
	}

	public async Task<IEnumerable<MovimientoAlimentoDto>> ObtenerMovimientosEnRangoAsync(
		DateTime desde, DateTime hasta)
	{
		try
		{
			if (desde > hasta)
				throw new InvalidOperationException(
					"Fecha 'desde' no puede ser mayor que 'hasta'");

			var movimientos = await _movimientoRepository
				.ObtenerMovimientosEnRangoAsync(desde, hasta);

			return _mapper.Map<IEnumerable<MovimientoAlimentoDto>>(movimientos);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Error al obtener movimientos en rango. Desde: {Desde}, Hasta: {Hasta}",
				desde, hasta);
			throw new InvalidOperationException(
				"Error al obtener los movimientos en el rango", ex);
		}
	}
}
```

---

## ReproduccionService

### Archivo: `CuyControl.Application/Services/ReproduccionService.cs`

```csharp
namespace CuyControl.Application.Services;

using CuyControl.Application.Interfaces.Services;
using CuyControl.Domain.Entities;
using CuyControl.Domain.Enums;
using CuyControl.Domain.Interfaces.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;

public class ReproduccionService : IReproduccionService
{
	private readonly IReproduccionRepository _reproduccionRepository;
	private readonly ICuyRepository _cuyRepository;
	private readonly IPartoRepository _partoRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<ReproduccionService> _logger;

	public ReproduccionService(
		IReproduccionRepository reproduccionRepository,
		ICuyRepository cuyRepository,
		IPartoRepository partoRepository,
		IMapper mapper,
		ILogger<ReproduccionService> logger)
	{
		_reproduccionRepository = reproduccionRepository 
			?? throw new ArgumentNullException(nameof(reproduccionRepository));
		_cuyRepository = cuyRepository 
			?? throw new ArgumentNullException(nameof(cuyRepository));
		_partoRepository = partoRepository 
			?? throw new ArgumentNullException(nameof(partoRepository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	public async Task<ReproduccionDto> CrearReproduccionAsync(
		ReproduccionDto reproduccionDto)
	{
		try
		{
			// ✅ Validación 1: Datos requeridos
			if (reproduccionDto == null)
				throw new InvalidOperationException(
					"Los datos de reproducción son requeridos");

			// ✅ Validación 2: Obtener cuyes
			var cuyMacho = await _cuyRepository.GetByIdAsync(
				reproduccionDto.CuyMachoId);
			var cuyHembra = await _cuyRepository.GetByIdAsync(
				reproduccionDto.CuyHembraId);

			if (cuyMacho == null)
				throw new InvalidOperationException(
					$"Macho con ID {reproduccionDto.CuyMachoId} no encontrado");

			if (cuyHembra == null)
				throw new InvalidOperationException(
					$"Hembra con ID {reproduccionDto.CuyHembraId} no encontrado");

			// ✅ Validación 3: No pueden ser el mismo cuy
			if (reproduccionDto.CuyMachoId == reproduccionDto.CuyHembraId)
				throw new InvalidOperationException(
					"No se puede reproducir un cuy consigo mismo");

			// ✅ Validación 4: Validar sexo del macho (SexoId = 1 = Macho)
			if (cuyMacho.SexoId != 1)
				throw new InvalidOperationException(
					"El CuyMacho debe ser de sexo masculino (SexoId=1)");

			// ✅ Validación 5: Validar sexo de la hembra (SexoId = 2 = Hembra)
			if (cuyHembra.SexoId != 2)
				throw new InvalidOperationException(
					"El CuyHembra debe ser de sexo femenino (SexoId=2)");

			// ✅ Validación 6: Macho no debe estar vendido ni muerto
			if (cuyMacho.EstadoId == (int)EstadoCuy.Vendido)
				throw new InvalidOperationException(
					"El macho no puede estar vendido");

			if (cuyMacho.EstadoId == (int)EstadoCuy.Muerto)
				throw new InvalidOperationException(
					"El macho no puede estar muerto");

			// ✅ Validación 7: Hembra no debe estar vendida ni muerta
			if (cuyHembra.EstadoId == (int)EstadoCuy.Vendido)
				throw new InvalidOperationException(
					"La hembra no puede estar vendida");

			if (cuyHembra.EstadoId == (int)EstadoCuy.Muerto)
				throw new InvalidOperationException(
					"La hembra no puede estar muerta");

			// ✅ Validación 8: Hembra no debe tener reproducción activa
			var reproduccionActiva = await _reproduccionRepository
				.ObtenerReproduccionActivaAsync(reproduccionDto.CuyHembraId);

			if (reproduccionActiva != null)
				throw new InvalidOperationException(
					"La hembra ya tiene una reproducción activa (pendiente de parto)");

			// ✅ Crear reproducción
			var reproduccion = _mapper.Map<Reproduccion>(reproduccionDto);
			reproduccion.FechaCreacion = DateTime.Now;
			reproduccion.Exitosa = false; // Inicialmente no exitosa

			// ✅ ACTUALIZAR: Hembra pasa a estado GESTANTE
			cuyHembra.EstadoId = (int)EstadoCuy.Gestante;
			cuyHembra.FechaModificacion = DateTime.Now;

			// Guardar cambios
			await _reproduccionRepository.AddAsync(reproduccion);
			await _cuyRepository.UpdateAsync(cuyHembra);
			await _reproduccionRepository.SaveChangesAsync();

			_logger.LogInformation(
				"Reproducción creada exitosamente. " +
				"Macho ID: {MachoId}, Hembra ID: {HembraId}, " +
				"Hembra actualizada a estado GESTANTE",
				reproduccionDto.CuyMachoId, reproduccionDto.CuyHembraId);

			return _mapper.Map<ReproduccionDto>(reproduccion);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning(
				"Error al crear reproducción. Error: {Error}",
				ex.Message);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error inesperado al crear reproducción");
			throw new InvalidOperationException(
				"Error al crear la reproducción", ex);
		}
	}

	public async Task<ReproduccionDto> ActualizarReproduccionAsync(
		int id, ReproduccionDto reproduccionDto)
	{
		try
		{
			var reproduccion = await _reproduccionRepository.GetByIdAsync(id);

			if (reproduccion == null)
				throw new InvalidOperationException(
					$"Reproducción con ID {id} no encontrada");

			// ✅ VALIDACIÓN CRÍTICA: Si hay partos, DEBE estar exitosa
			var tienePartos = await _partoRepository.TienePartosAsync(id);

			if (tienePartos && !reproduccionDto.Exitosa)
				throw new InvalidOperationException(
					"No se puede marcar como no exitosa si hay partos registrados. " +
					"Una reproducción con partos es automáticamente exitosa.");

			reproduccion.Exitosa = reproduccionDto.Exitosa;
			reproduccion.FechaModificacion = DateTime.Now;

			await _reproduccionRepository.UpdateAsync(reproduccion);
			await _reproduccionRepository.SaveChangesAsync();

			_logger.LogInformation(
				"Reproducción actualizada. ID: {ReproduccionId}, Exitosa: {Exitosa}",
				id, reproduccionDto.Exitosa);

			return _mapper.Map<ReproduccionDto>(reproduccion);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogWarning(
				"Error al actualizar reproducción ID {Id}. Error: {Error}",
				id, ex.Message);
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error inesperado al actualizar reproducción ID {Id}", id);
			throw new InvalidOperationException(
				"Error al actualizar la reproducción", ex);
		}
	}

	public async Task<ReproduccionDto?> ObtenerPorIdAsync(int id)
	{
		try
		{
			var reproduccion = await _reproduccionRepository.GetByIdAsync(id);
			return _mapper.Map<ReproduccionDto>(reproduccion);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al obtener reproducción ID {Id}", id);
			throw new InvalidOperationException("Error al obtener la reproducción", ex);
		}
	}

	public async Task<IEnumerable<ReproduccionDto>> ObtenerReproduccionesActivasAsync()
	{
		try
		{
			var reproducciones = await _reproduccionRepository
				.ObtenerReproduccionesActivasAsync();
			return _mapper.Map<IEnumerable<ReproduccionDto>>(reproducciones);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al obtener reproducciones activas");
			throw new InvalidOperationException(
				"Error al obtener reproducciones activas", ex);
		}
	}

	public async Task<IEnumerable<ReproduccionDto>> ObtenerHistorialHembraAsync(
		int cuyHembraId)
	{
		try
		{
			var reproducciones = await _reproduccionRepository
				.ObtenerHistorialHembraAsync(cuyHembraId);
			return _mapper.Map<IEnumerable<ReproduccionDto>>(reproducciones);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Error al obtener historial de reproducción de hembra ID {Id}",
				cuyHembraId);
			throw new InvalidOperationException(
				"Error al obtener historial de reproducción", ex);
		}
	}

	public async Task<bool> TienePartosAsync(int reproduccionId)
	{
		try
		{
			return await _partoRepository.TienePartosAsync(reproduccionId);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex,
				"Error al verificar si hay partos en reproducción ID {Id}",
				reproduccionId);
			throw new InvalidOperationException(
				"Error al verificar partos", ex);
		}
	}
}
```

---

## PartoService

### Archivo: `CuyControl.Application/Services/PartoService.cs` (Fragmento clave)

```csharp
public async Task<PartoDto> CrearPartoAsync(PartoDto partoDto)
{
	try
	{
		// ✅ Obtener reproducción
		var reproduccion = await _reproduccionRepository
			.GetByIdAsync(partoDto.ReproduccionId);

		if (reproduccion == null)
			throw new InvalidOperationException(
				$"Reproducción con ID {partoDto.ReproduccionId} no encontrada");

		// ✅ Obtener hembra
		var cuyHembra = await _cuyRepository
			.GetByIdAsync(reproduccion.CuyHembraId);

		if (cuyHembra == null)
			throw new InvalidOperationException(
				"Hembra no encontrada en la reproducción");

		// ✅ VALIDACIÓN: Hembra debe estar en estado GESTANTE
		if (cuyHembra.EstadoId != (int)EstadoCuy.Gestante)
			throw new InvalidOperationException(
				$"La hembra debe estar en estado GESTANTE (actual: {cuyHembra.EstadoId}). " +
				"No se puede registrar parto si la hembra no está gestante.");

		// ✅ Validar números positivos
		if (partoDto.NumeroDeCreasVivas < 0)
			throw new InvalidOperationException(
				"Número de crías vivas no puede ser negativo");

		if (partoDto.NumeroDeCreasNatimuertas < 0)
			throw new InvalidOperationException(
				"Número de crías nacimuertas no puede ser negativo");

		// ✅ Crear parto
		var parto = _mapper.Map<Parto>(partoDto);
		parto.FechaCreacion = DateTime.Now;

		await _partoRepository.AddAsync(parto);

		// ✅ ACTUALIZACIÓN AUTOMÁTICA 1: Hembra → LACTANTE
		cuyHembra.EstadoId = (int)EstadoCuy.Lactante;
		cuyHembra.FechaModificacion = DateTime.Now;

		// ✅ ACTUALIZACIÓN AUTOMÁTICA 2: Reproducción → Exitosa = true
		reproduccion.Exitosa = true;
		reproduccion.FechaModificacion = DateTime.Now;

		// Guardar cambios
		await _cuyRepository.UpdateAsync(cuyHembra);
		await _reproduccionRepository.UpdateAsync(reproduccion);
		await _partoRepository.SaveChangesAsync();

		_logger.LogInformation(
			"Parto registrado exitosamente. " +
			"Reproducción: {ReproduccionId}, " +
			"Crías vivas: {VivasCreated: {MuertsNumber}}, " +
			"Hembra {HembraId} actualizada a LACTANTE, " +
			"Reproducción marcada como Exitosa",
			partoDto.ReproduccionId,
			partoDto.NumeroDeCreasVivas,
			partoDto.NumeroDeCreasNatimuertas,
			cuyHembra.Id);

		return _mapper.Map<PartoDto>(parto);
	}
	catch (InvalidOperationException ex)
	{
		_logger.LogWarning(
			"Error al registrar parto. Reproducción: {Id}, Error: {Error}",
			partoDto.ReproduccionId, ex.Message);
		throw;
	}
	catch (Exception ex)
	{
		_logger.LogError(ex,
			"Error inesperado al registrar parto. Reproducción: {Id}",
			partoDto.ReproduccionId);
		throw new InvalidOperationException("Error al registrar el parto", ex);
	}
}
```

---

## Refactorización de Controladores

### Archivo: `CuyControl/Controllers/MovimientoAlimentoController.cs` (Refactorizado)

```csharp
namespace CuyControl.Controllers;

using CuyControl.Application.Interfaces.Services;
using CuyControl.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Administrador,Operador")]
public class MovimientoAlimentoController : Controller
{
	private readonly IMovimientoAlimentoService _movimientoService;
	private readonly IInventarioAlimentoService _inventarioService;
	private readonly ILogger<MovimientoAlimentoController> _logger;

	public MovimientoAlimentoController(
		IMovimientoAlimentoService movimientoService,
		IInventarioAlimentoService inventarioService,
		ILogger<MovimientoAlimentoController> logger)
	{
		_movimientoService = movimientoService 
			?? throw new ArgumentNullException(nameof(movimientoService));
		_inventarioService = inventarioService 
			?? throw new ArgumentNullException(nameof(inventarioService));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		try
		{
			var inventarios = await _inventarioService.ObtenerTodosAsync();
			return View(inventarios);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error al obtener inventarios");
			TempData["Error"] = "Error al cargar los inventarios";
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpPost]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> Create(MovimientoAlimentoViewModel model)
	{
		if (!ModelState.IsValid)
		{
			TempData["Error"] = "Por favor, revise los datos ingresados";
			return RedirectToAction(nameof(Index));
		}

		try
		{
			// Mapear ViewModel a DTO
			var movimientoDto = new MovimientoAlimentoDto
			{
				InventarioAlimentoId = model.InventarioAlimentoId,
				TipoMovimiento = model.TipoMovimiento,
				Cantidad = model.Cantidad
			};

			// Llamar al servicio (que maneja validaciones y transacciones)
			var resultado = await _movimientoService
				.RegistrarMovimientoAsync(movimientoDto);

			TempData["Exito"] = "Movimiento registrado exitosamente";
			_logger.LogInformation(
				"Movimiento registrado exitosamente. Inventario: {Id}",
				model.InventarioAlimentoId);

			return RedirectToAction(nameof(Index));
		}
		catch (InvalidOperationException ex)
		{
			// Errores esperados (stock insuficiente, etc.)
			TempData["Error"] = ex.Message;
			_logger.LogWarning(
				"Error de validación en movimiento. Error: {Error}",
				ex.Message);
			return RedirectToAction(nameof(Index));
		}
		catch (Exception ex)
		{
			// Errores inesperados
			TempData["Error"] = "Ocurrió un error al registrar el movimiento";
			_logger.LogError(ex, "Error inesperado al registrar movimiento");
			return RedirectToAction(nameof(Index));
		}
	}
}
```

---

## Program.cs - Registro de Servicios

### Archivo: `CuyControl/Program.cs` (Sección de registros)

```csharp
// En el método ConfigureServices o en Program.cs (Startup pattern)

// ✅ Agregar estos registros de servicios
services.AddScoped<IInventarioAlimentoService, InventarioAlimentoService>();
services.AddScoped<IMovimientoAlimentoService, MovimientoAlimentoService>();
services.AddScoped<IReproduccionService, ReproduccionService>();
services.AddScoped<IPartoService, PartoService>();
services.AddScoped<IMortalidadService, MortalidadService>();

// Verifica que también esténregistrados los repositorios:
services.AddScoped<IInventarioAlimentoRepository, InventarioAlimentoRepository>();
services.AddScoped<IMovimientoAlimentoRepository, MovimientoAlimentoRepository>();
services.AddScoped<IReproduccionRepository, ReproduccionRepository>();
services.AddScoped<IPartoRepository, PartoRepository>();
services.AddScoped<IMortalidadRepository, MortalidadRepository>();
services.AddScoped<ICuyRepository, CuyRepository>();
services.AddScoped<IJaulaRepository, JaulaRepository>();
```

---

**Documentación generada**: 2024
**Versión**: 1.0
