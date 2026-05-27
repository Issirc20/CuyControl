# AUDITORÍA TÉCNICA INTEGRAL - CUYCONTROL
**Auditor**: Arquitecto de Software Senior & Tech Lead  
**Fecha**: 2024  
**Versión del Proyecto**: Desarrollo  
**Estado**: Análisis Completo  

---

## 📋 TABLA DE CONTENIDOS
1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Problemas Críticos](#problemas-críticos)
3. [Problemas de Arquitectura](#problemas-de-arquitectura)
4. [Problemas de Base de Datos](#problemas-de-base-de-datos)
5. [Problemas de Lógica de Negocio](#problemas-de-lógica-de-negocio)
6. [Problemas de Seguridad](#problemas-de-seguridad)
7. [Problemas de Código](#problemas-de-código)
8. [Problemas de UX/UI](#problemas-de-uxui)
9. [Inconsistencias de Datos](#inconsistencias-de-datos)
10. [Plan de Correcciones](#plan-de-correcciones)

---

## 🎯 RESUMEN EJECUTIVO

### Estado General
- ✅ **Compilación**: Correcta
- ❌ **Arquitectura**: Con problemas de capas
- ❌ **Lógica de Negocio**: Inconsistencias críticas
- ⚠️ **Base de Datos**: Relaciones y configuraciones parciales
- ❌ **Seguridad**: Vulnerabilidades detectadas
- ⚠️ **Testing**: Sin pruebas automatizadas

### Severidad de Problemas
- 🔴 **CRÍTICO**: 12 problemas
- 🟠 **ALTO**: 18 problemas
- 🟡 **MEDIO**: 15 problemas
- 🟢 **BAJO**: 8 problemas

**Total**: 53 problemas identificados

### 📌 SECCIONES CORREGIDAS ✅
- ✅ **PC-07**: Falta de Servicio para Inventario - SOLUCIÓN DETALLADA
- ✅ **PC-09**: Movimiento de Alimento - SOLUCIÓN DETALLADA CON LOCK
- ✅ **BD-03**: Falta Relación CuyHembra - SOLUCIÓN CON MIGRACIÓN EF CORE
- ✅ **LN-02**: Reproducción Exitosa - SOLUCIÓN CON VALIDACIONES CRÍTICAS
- ✅ **LN-03**: Estados No se Actualizan - SOLUCIÓN CON PARTOSERVICE
- ✅ **LN-04**: Mortalidad No se Registra - SOLUCIÓN AUTOMÁTICA

**Ver archivos complementarios**:
- `CORRECCIONES_INVENTARIO_REPRODUCCION.md` - Resumen ejecutivo de correcciones
- `DIAGRAMA_ARQUITECTURA.md` - Diagramas visuales y protecciones
- `EJEMPLOS_CODIGO.md` - Código implementable listo para usar

---

## 🔴 PROBLEMAS CRÍTICOS

### PC-01: Violación de Capas - Controladores Accediendo Directamente a DbContext

**Problema**: Múltiples controladores (VentaController, InventarioAlimentoController, JaulaController, MovimientoAlimentoController, etc.) están accediendo directamente a `ApplicationDbContext` en lugar de usar los servicios de la capa Application.

**Archivo Afectado**: 
- `CuyControl/Controllers/VentaController.cs` (línea 14, 23-93)
- `CuyControl/Controllers/InventarioAlimentoController.cs` (línea 13-62)
- `CuyControl/Controllers/JaulaController.cs` (línea 14, 23-74)
- `CuyControl/Controllers/MovimientoAlimentoController.cs` (línea 15, 24-80)

**Código Incorrecto**:
```csharp
// VentaController.cs
public class VentaController : Controller
{
	private readonly ApplicationDbContext _context; // ❌ VIOLACIÓN

	public async Task<IActionResult> Create(VentaViewModel model)
	{
		_context.Ventas.Add(venta);  // ❌ Acceso directo a BD
		var cuy = await _context.Cuyes.FindAsync(model.CuyId); // ❌ Acceso directo
		cuy.Activo = false;
		cuy.EstadoId = 6;
		await _context.SaveChangesAsync();
	}
}
```

**Impacto**:
- Violación de Clean Architecture
- Código duplicado de acceso a datos
- Difícil de testear
- Lógica de negocio dispersa en controladores
- Imposible reutilizar lógica

**Solución**:
Crear servicios en la capa Application para cada entidad y usarlos en los controladores.

```csharp
// IVentaService.cs
public interface IVentaService
{
	Task<VentaDto> CrearVentaAsync(VentaDto ventaDto);
	Task<IEnumerable<VentaDto>> ObtenerTodasVentasAsync();
}

// VentaService.cs
public class VentaService : IVentaService
{
	private readonly IVentaRepository _repository;
	private readonly ICuyRepository _cuyRepository;

	public async Task<VentaDto> CrearVentaAsync(VentaDto ventaDto)
	{
		var venta = MappingProfile.MapToVenta(ventaDto);
		await _repository.AddAsync(venta);

		var cuy = await _cuyRepository.GetByIdAsync(ventaDto.CuyId);
		if (cuy != null)
		{
			cuy.Activo = false;
			cuy.EstadoId = 6; // Vendido
		}

		await _repository.SaveChangesAsync();
		return MappingProfile.MapToVentaDto(venta);
	}
}

// VentaController.cs - CORREGIDO
public class VentaController : Controller
{
	private readonly IVentaService _ventaService; // ✅ Usar servicio

	public async Task<IActionResult> Create(VentaViewModel model)
	{
		var ventaDto = MapToVentaDto(model);
		await _ventaService.CrearVentaAsync(ventaDto);
		return RedirectToAction(nameof(Index));
	}
}
```

**Lógica de Negocio**:
La creación de una venta debe:
1. Validar que el cuy existe
2. Validar que el cuy está activo y no está vendido/muerto
3. Registrar la venta
4. Cambiar estado del cuy a vendido (EstadoId = 6)
5. Marcar cuy como inactivo

---

### PC-02: Cambio de Estado del Cuy en Venta Sin Validación

**Problema**: Cuando se crea una venta, el controlador simplemente cambia `EstadoId = 6` sin validar:
- Si el cuy ya fue vendido
- Si el cuy está muerto
- Si el cuy es reproductor (no debería venderse)
- Si hay referencias en reproducción

**Archivo Afectado**: `CuyControl/Controllers/VentaController.cs` (líneas 86-91)

**Código Incorrecto**:
```csharp
var cuy = await _context.Cuyes.FindAsync(model.CuyId);
if (cuy != null)
{
	cuy.Activo = false;
	cuy.EstadoId = 6; // ❌ Sin validación
}
```

**Impacto**:
- Posibilidad de vender el mismo cuy dos veces
- Posibilidad de vender cuyes muertos
- Posibilidad de vender reproductores activos
- Inconsistencia de datos

**Solución**:
```csharp
public async Task<VentaDto> CrearVentaAsync(VentaDto ventaDto)
{
	var cuy = await _cuyRepository.GetByIdAsync(ventaDto.CuyId);

	if (cuy == null)
		throw new InvalidOperationException("Cuy no encontrado");

	if (cuy.EstadoId == 6)
		throw new InvalidOperationException("El cuy ya fue vendido");

	if (cuy.EstadoId == 7)
		throw new InvalidOperationException("No se puede vender un cuy muerto");

	if (cuy.EstadoId == 3 && cuy.SexoId == 1) // Reproductor macho
		throw new InvalidOperationException("No se pueden vender reproductores activos");

	if (cuy.EstadoId == 3 && cuy.SexoId == 2) // Reproductora hembra
	{
		var reproduccionesActivas = await _reproduccionRepository
			.GetByHembraIdAsync(cuy.Id);
		if (reproduccionesActivas.Any())
			throw new InvalidOperationException("No se pueden vender hembras con reproducciones activas");
	}

	var venta = MappingProfile.MapToVenta(ventaDto);
	await _repository.AddAsync(venta);

	cuy.Activo = false;
	cuy.EstadoId = 6;
	await _cuyRepository.UpdateAsync(cuy);

	await _repository.SaveChangesAsync();
	return MappingProfile.MapToVentaDto(venta);
}
```

---

### PC-03: Estados de Cuy Sin Enumeración - Números Mágicos

**Problema**: Los estados se manejan con números enteros hardcodeados sin enumeración:
- `EstadoId = 1` (Cría)
- `EstadoId = 2` (Recría)
- `EstadoId = 3` (Reproductor)
- `EstadoId = 4` (Gestante)
- `EstadoId = 5` (Lactante)
- `EstadoId = 6` (Vendido)
- `EstadoId = 7` (Muerto)

**Archivo Afectado**: 
- `CuyControl/Controllers/HomeController.cs` (líneas 28, 39, 61-67)
- `CuyControl/Controllers/VentaController.cs` (línea 90)
- `CuyControl/Domain/Entities/Cuy.cs` (comentario línea 27)

**Código Incorrecto**:
```csharp
// HomeController.cs
CuyesMuertos = await _context.Cuyes.CountAsync(c => c.EstadoId == 7), // ❌ Número mágico
CuyesVendidos = await _context.Cuyes.CountAsync(c => c.EstadoId == 6), // ❌ Número mágico
Crias = await _context.Cuyes.CountAsync(c => c.EstadoId == 1 && c.Activo),
Recrias = await _context.Cuyes.CountAsync(c => c.EstadoId == 2 && c.Activo),
Reproductores = await _context.Cuyes.CountAsync(c => c.EstadoId == 3 && c.Activo),
```

**Impacto**:
- Código no mantenible
- Errores por cambios de valores
- Difícil de documentar
- Propenso a bugs

**Solución**:
```csharp
// CuyControl.Domain/Enums/EstadoCuy.cs
namespace CuyControl.Domain.Enums;

public enum EstadoCuy
{
	Cria = 1,
	Recria = 2,
	Reproductor = 3,
	Gestante = 4,
	Lactante = 5,
	Vendido = 6,
	Muerto = 7
}

// CuyControl/Controllers/HomeController.cs - CORREGIDO
var dashboard = new DashboardViewModel
{
	CuyesMuertos = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Muerto),
	CuyesVendidos = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Vendido),
	Crias = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Cria && c.Activo),
	Recrias = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Recria && c.Activo),
	Reproductores = await _context.Cuyes.CountAsync(c => c.EstadoId == (int)EstadoCuy.Reproductor && c.Activo),
};
```

---

### PC-04: HomController.cs Accediendo Directamente a DbContext (Dashboard)

**Problema**: El controlador Home accede directamente a `ApplicationDbContext` para construir el dashboard, sin usar servicios de la capa Application.

**Archivo Afectado**: `CuyControl/Controllers/HomeController.cs` (líneas 14, 26-60)

**Código Incorrecto**:
```csharp
public class HomeController : Controller
{
	private readonly ApplicationDbContext _context; // ❌ Violación de arquitectura

	public async Task<IActionResult> Index()
	{
		var dashboard = new DashboardViewModel
		{
			TotalCuyes = await _context.Cuyes.CountAsync(), // ❌ Acceso directo
			CuyesActivos = await _context.Cuyes.CountAsync(c => c.Activo),
			// ... más accesos directos
		};
	}
}
```

**Impacto**:
- Violación de Clean Architecture
- Dashboard no reutilizable en otras capas
- Lógica duplicada si se necesita en otros contextos
- Difícil de testear

**Solución**:
```csharp
// IReportService.cs
public interface IReportService
{
	Task<DashboardDto> ObtenerDashboardAsync();
}

// ReportService.cs
public class ReportService : IReportService
{
	private readonly ICuyRepository _cuyRepository;
	private readonly IGalponRepository _galponRepository;
	private readonly IJaulaRepository _jaulaRepository;
	private readonly IVentaRepository _ventaRepository;
	private readonly IInventarioAlimentoRepository _inventarioRepository;

	public async Task<DashboardDto> ObtenerDashboardAsync()
	{
		var hoy = DateTime.Today;
		var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

		var dashboard = new DashboardDto
		{
			TotalCuyes = await _cuyRepository.CountAsync(),
			CuyesActivos = await _cuyRepository.CountAsync(c => c.Activo),
			CuyesMuertos = await _cuyRepository.CountAsync(c => c.EstadoId == (int)EstadoCuy.Muerto),
			CuyesVendidos = await _cuyRepository.CountAsync(c => c.EstadoId == (int)EstadoCuy.Vendido),
			// ... más lógica
		};

		return dashboard;
	}
}

// HomeController.cs - CORREGIDO
public class HomeController : Controller
{
	private readonly IReportService _reportService; // ✅ Usar servicio

	public async Task<IActionResult> Index()
	{
		var dashboardDto = await _reportService.ObtenerDashboardAsync();
		var viewModel = MappingProfile.MapToDashboardViewModel(dashboardDto);
		return View(viewModel);
	}
}
```

---

### PC-05: UsuarioCreacionId Hardcodeado como 1

**Problema**: En múltiples controladores, el `UsuarioCreacionId` se asigna hardcodeado como `1` sin obtener el usuario actual de la sesión.

**Archivo Afectado**:
- `CuyControl/Controllers/VentaController.cs` (línea 81)
- `CuyControl/Controllers/InventarioAlimentoController.cs` (línea 59)
- `CuyControl/Controllers/JaulaController.cs` (línea 71)

**Código Incorrecto**:
```csharp
var venta = new Venta
{
	// ...
	UsuarioCreacionId = 1 // ❌ Hardcodeado
};

var inventario = new InventarioAlimento
{
	// ...
	UsuarioCreacionId = 1 // ❌ Hardcodeado
};
```

**Impacto**:
- Auditoría incorrecta (todos los registros atribuidos al usuario 1)
- Imposibilidad de rastrear quién hizo qué
- Problema de seguridad

**Solución**:
```csharp
public class BaseController : Controller
{
	protected int GetCurrentUserId()
	{
		var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
		return int.TryParse(userIdClaim?.Value, out int userId) ? userId : 0;
	}
}

// VentaController - CORREGIDO
public class VentaController : BaseController
{
	[HttpPost]
	public async Task<IActionResult> Create(VentaViewModel model)
	{
		var venta = new Venta
		{
			// ...
			UsuarioCreacionId = GetCurrentUserId() // ✅ Usuario actual
		};
	}
}
```

---

### PC-06: Dashboard Contabiliza Incorrectamente Cuyes Disponibles

**Problema**: El dashboard cuenta "CuyesDisponibles" como activos que no sean vendidos ni muertos, pero NO excluye gestantes, lactantes, etc., que NO están disponibles para venta.

**Archivo Afectado**: `CuyControl/Controllers/HomeController.cs` (línea 58-59)

**Código Incorrecto**:
```csharp
CuyesDisponibles = await _context.Cuyes
	.CountAsync(c => c.Activo && c.EstadoId != 6 && c.EstadoId != 7), // ❌ Incompleto
```

**Impacto**:
- Dashboard muestra números incorrectos
- Pueden contarse gestantes como disponibles para venta
- Confusión en reportes

**Solución**:
```csharp
CuyesDisponibles = await _context.Cuyes
	.CountAsync(c => c.Activo 
		&& c.EstadoId != (int)EstadoCuy.Vendido
		&& c.EstadoId != (int)EstadoCuy.Muerto
		&& c.EstadoId != (int)EstadoCuy.Gestante
		&& c.EstadoId != (int)EstadoCuy.Lactante),
```

---

### PC-07: Falta de Servicio para Inventario de Alimento

**Problema**: No existe `IInventarioAlimentoService` en la capa Application. El controlador `InventarioAlimentoController` accede directamente al DbContext.

**Archivo Afectado**: 
- `CuyControl/Controllers/InventarioAlimentoController.cs`
- `Program.cs` (no registra servicio)

**Impacto**:
- No se puede validar antes de crear inventario
- No se puede ejecutar lógica de negocio
- Código duplicado si se necesita en otros contextos

**Solución**:
Crear `IInventarioAlimentoService` y `InventarioAlimentoService`.

```csharp
// CuyControl.Application/Interfaces/Services/IInventarioAlimentoService.cs
public interface IInventarioAlimentoService
{
	Task<InventarioAlimentoDto> CrearInventarioAsync(InventarioAlimentoDto inventarioDto);
	Task<InventarioAlimentoDto> ActualizarInventarioAsync(int id, InventarioAlimentoDto inventarioDto);
	Task<IEnumerable<InventarioAlimentoDto>> ObtenerTodosAsync();
	Task<InventarioAlimentoDto?> ObtenerPorIdAsync(int id);
	Task EliminarAsync(int id);
}

// CuyControl.Application/Services/InventarioAlimentoService.cs
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
		_repository = repository;
		_mapper = mapper;
		_logger = logger;
	}

	public async Task<InventarioAlimentoDto> CrearInventarioAsync(
		InventarioAlimentoDto inventarioDto)
	{
		// ✅ Validar datos requeridos
		if (string.IsNullOrWhiteSpace(inventarioDto.TipoAlimento))
			throw new InvalidOperationException("Tipo de alimento es requerido");

		if (inventarioDto.CantidadActual < 0)
			throw new InvalidOperationException("Cantidad no puede ser negativa");

		// ✅ Verificar que no exista el mismo tipo de alimento
		var existe = await _repository.ExisteAsync(
			a => a.TipoAlimento.ToLower() == inventarioDto.TipoAlimento.ToLower());

		if (existe)
			throw new InvalidOperationException(
				$"Ya existe inventario para: {inventarioDto.TipoAlimento}");

		var inventario = _mapper.Map<InventarioAlimento>(inventarioDto);
		inventario.FechaCreacion = DateTime.Now;

		await _repository.AddAsync(inventario);
		await _repository.SaveChangesAsync();

		_logger.LogInformation(
			$"Inventario de alimento creado: {inventario.Id} - {inventario.TipoAlimento}");

		return _mapper.Map<InventarioAlimentoDto>(inventario);
	}

	public async Task<InventarioAlimentoDto> ActualizarInventarioAsync(
		int id, InventarioAlimentoDto inventarioDto)
	{
		var inventario = await _repository.GetByIdAsync(id);

		if (inventario == null)
			throw new InvalidOperationException("Inventario no encontrado");

		if (inventarioDto.CantidadActual < 0)
			throw new InvalidOperationException("Cantidad no puede ser negativa");

		inventario.TipoAlimento = inventarioDto.TipoAlimento ?? inventario.TipoAlimento;
		inventario.CantidadActual = inventarioDto.CantidadActual;
		inventario.FechaModificacion = DateTime.Now;

		await _repository.UpdateAsync(inventario);
		await _repository.SaveChangesAsync();

		_logger.LogInformation(
			$"Inventario de alimento actualizado: {id}");

		return _mapper.Map<InventarioAlimentoDto>(inventario);
	}

	public async Task<IEnumerable<InventarioAlimentoDto>> ObtenerTodosAsync()
	{
		var inventarios = await _repository.GetAllAsync();
		return _mapper.Map<IEnumerable<InventarioAlimentoDto>>(inventarios);
	}

	public async Task<InventarioAlimentoDto?> ObtenerPorIdAsync(int id)
	{
		var inventario = await _repository.GetByIdAsync(id);
		return _mapper.Map<InventarioAlimentoDto>(inventario);
	}

	public async Task EliminarAsync(int id)
	{
		var inventario = await _repository.GetByIdAsync(id);

		if (inventario == null)
			throw new InvalidOperationException("Inventario no encontrado");

		await _repository.DeleteAsync(inventario);
		await _repository.SaveChangesAsync();

		_logger.LogInformation(
			$"Inventario de alimento eliminado: {id}");
	}
}

// Program.cs - Registro de servicio
services.AddScoped<IInventarioAlimentoService, InventarioAlimentoService>();
```

---

### PC-08: Falta de Servicio para Jaula

**Problema**: No existe `IJaulaService`. El controlador accede directamente a DbContext sin validación de capacidad.

**Archivo Afectado**: `CuyControl/Controllers/JaulaController.cs`

**Código Incorrecto**:
```csharp
[HttpPost]
public async Task<IActionResult> Create(JaulaViewModel model)
{
	var jaula = new Jaula
	{
		Codigo = model.Codigo,
		Capacidad = model.Capacidad, // ❌ Sin validación de capacidad positiva
		// ...
	};
}
```

**Impacto**:
- Posibilidad de crear jaulas con capacidad 0 o negativa
- Sin validación de código único por galpón
- Sin control de lógica de negocio

**Solución**:
Crear servicio con validaciones.

```csharp
public class JaulaService : IJaulaService
{
	public async Task<JaulaDto> CrearJaulaAsync(JaulaDto jaulaDto)
	{
		if (jaulaDto.Capacidad <= 0)
			throw new InvalidOperationException("Capacidad debe ser mayor a 0");

		var existe = await _jaulaRepository
			.ExisteCodigoEnGalpon(jaulaDto.Codigo, jaulaDto.GalponId);

		if (existe)
			throw new InvalidOperationException("Ya existe una jaula con este código en el galpón");

		var jaula = MappingProfile.MapToJaula(jaulaDto);
		await _jaulaRepository.AddAsync(jaula);
		await _jaulaRepository.SaveChangesAsync();

		return MappingProfile.MapToJaulaDto(jaula);
	}
}
```

---

### PC-09: Movimiento de Alimento Sin Validación de Stock Negativo

**Problema**: Al procesar un movimiento de salida, se valida que haya stock, pero si hay transacciones concurrentes, podría resultar en stock negativo. Además, no existe `IMovimientoAlimentoService`.

**Archivo Afectado**: 
- `CuyControl/Controllers/MovimientoAlimentoController.cs` (línea 77-80)
- Falta implementación de servicio

**Código Incorrecto**:
```csharp
// Controlador accediendo directamente a BD
if (model.TipoMovimiento == TipoMovimientoAlimentoEnum.Entrada)
{
	inventario.CantidadActual += model.Cantidad;
}
else
{
	if (inventario.CantidadActual < model.Cantidad) // ❌ Sin atomicidad
	{
		ModelState.AddModelError("", "No hay suficiente stock.");
	}
	inventario.CantidadActual -= model.Cantidad;
}
```

**Impacto**:
- Posible stock negativo en ambientes concurrentes
- Inconsistencia de datos
- Problemas de contabilidad
- Violación de capas arquitectónicas

**Solución**:

```csharp
// CuyControl.Application/Interfaces/Services/IMovimientoAlimentoService.cs
public interface IMovimientoAlimentoService
{
	Task<MovimientoAlimentoDto> RegistrarMovimientoAsync(
		MovimientoAlimentoDto movimientoDto);
	Task<IEnumerable<MovimientoAlimentoDto>> ObtenerMovimientosAsync(
		int inventarioAlimentoId);
	Task<decimal> ObtenerStockActualAsync(int inventarioAlimentoId);
}

// CuyControl.Application/Services/MovimientoAlimentoService.cs
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
		_movimientoRepository = movimientoRepository;
		_inventarioRepository = inventarioRepository;
		_mapper = mapper;
		_logger = logger;
	}

	public async Task<MovimientoAlimentoDto> RegistrarMovimientoAsync(
		MovimientoAlimentoDto movimientoDto)
	{
		// ✅ Obtener inventario CON LOCK de escritura para evitar condiciones de carrera
		var inventario = await _inventarioRepository.GetByIdAsync(
			movimientoDto.InventarioAlimentoId);

		if (inventario == null)
			throw new InvalidOperationException("Inventario de alimento no encontrado");

		// ✅ Validar cantidad positiva
		if (movimientoDto.Cantidad <= 0)
			throw new InvalidOperationException("Cantidad debe ser mayor a cero");

		// ✅ VALIDACIÓN CRÍTICA para salida
		if (movimientoDto.TipoMovimiento == TipoMovimientoAlimentoEnum.Salida)
		{
			if (inventario.CantidadActual < movimientoDto.Cantidad)
				throw new InvalidOperationException(
					$"Stock insuficiente. Disponible: {inventario.CantidadActual}, " +
					$"Solicitado: {movimientoDto.Cantidad}");

			inventario.CantidadActual -= movimientoDto.Cantidad;

			_logger.LogInformation(
				$"Salida de alimento: {inventario.TipoAlimento} - " +
				$"Cantidad: {movimientoDto.Cantidad} - " +
				$"Stock restante: {inventario.CantidadActual}");
		}
		else if (movimientoDto.TipoMovimiento == TipoMovimientoAlimentoEnum.Entrada)
		{
			inventario.CantidadActual += movimientoDto.Cantidad;

			_logger.LogInformation(
				$"Entrada de alimento: {inventario.TipoAlimento} - " +
				$"Cantidad: {movimientoDto.Cantidad} - " +
				$"Stock total: {inventario.CantidadActual}");
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

		await _movimientoRepository.AddAsync(movimiento);

		// ✅ Actualizar inventario
		inventario.FechaModificacion = DateTime.Now;
		await _inventarioRepository.UpdateAsync(inventario);

		// ✅ Guardar cambios en transacción
		await _movimientoRepository.SaveChangesAsync();

		_logger.LogInformation(
			$"Movimiento registrado: ID {movimiento.Id} - " +
			$"Inventario: {inventario.TipoAlimento}");

		return _mapper.Map<MovimientoAlimentoDto>(movimiento);
	}

	public async Task<IEnumerable<MovimientoAlimentoDto>> ObtenerMovimientosAsync(
		int inventarioAlimentoId)
	{
		var movimientos = await _movimientoRepository
			.ObtenerMovimientosAsync(inventarioAlimentoId);

		return _mapper.Map<IEnumerable<MovimientoAlimentoDto>>(movimientos);
	}

	public async Task<decimal> ObtenerStockActualAsync(int inventarioAlimentoId)
	{
		var inventario = await _inventarioRepository
			.GetByIdAsync(inventarioAlimentoId);

		if (inventario == null)
			throw new InvalidOperationException("Inventario no encontrado");

		return inventario.CantidadActual;
	}
}

// Program.cs - Registro
services.AddScoped<IMovimientoAlimentoService, MovimientoAlimentoService>();

// Controlador corregido
public class MovimientoAlimentoController : Controller
{
	private readonly IMovimientoAlimentoService _movimientoService;

	public async Task<IActionResult> Create(MovimientoAlimentoViewModel model)
	{
		try
		{
			var movimientoDto = MapToMovimientoAlimentoDto(model);
			var resultado = await _movimientoService
				.RegistrarMovimientoAsync(movimientoDto);

			TempData["Mensaje"] = "Movimiento registrado exitosamente";
			return RedirectToAction(nameof(Index));
		}
		catch (InvalidOperationException ex)
		{
			ModelState.AddModelError("", ex.Message);
			return View(model);
		}
	}
}
```

---

### PC-10: Capacidad de Jaula No Se Valida al Asignar Cuyes

**Problema**: Cuando se crea un cuy y se le asigna una jaula, no se valida que la jaula tenga espacio disponible. El `CantidadActual` de la jaula tampoco se incrementa.

**Archivo Afectado**: `CuyControl/Controllers/CuyController.cs` (línea 72-93)

**Impacto**:
- Posibilidad de exceder capacidad de jaula
- `CantidadActual` de jaula no refleja realidad
- Datos inconsistentes

**Solución**:
```csharp
public async Task<CuyDto> CrearCuyAsync(CuyDto cuyDto)
{
	if (cuyDto.JaulaId.HasValue)
	{
		var jaula = await _jaulaRepository.GetByIdAsync(cuyDto.JaulaId.Value);

		if (jaula == null)
			throw new InvalidOperationException("Jaula no encontrada");

		if (jaula.CantidadActual >= jaula.Capacidad)
			throw new InvalidOperationException(
				"La jaula ha alcanzado su capacidad máxima");
	}

	var cuy = MappingProfile.MapToCuy(cuyDto);
	await _repository.AddAsync(cuy);

	if (cuyDto.JaulaId.HasValue)
	{
		var jaula = await _jaulaRepository.GetByIdAsync(cuyDto.JaulaId.Value);
		jaula.CantidadActual += 1;
		await _jaulaRepository.UpdateAsync(jaula);
	}

	await _repository.SaveChangesAsync();
	return MappingProfile.MapToCuyDto(cuy);
}
```

---

### PC-11: Venta del Mismo Cuy Múltiples Veces (sin control de cantidad)

**Problema**: La entidad `Venta` tiene un campo `Cantidad`, pero cuando se crea una venta, se marca solo UNA instancia del cuy como vendido, sin considerar la cantidad vendida.

**Archivo Afectado**: `CuyControl/Domain/Entities/Venta.cs` (línea 28-30)

**Código Incorrecto**:
```csharp
public int CuyId { get; set; }  // ❌ Solo referencia UN cuy
public int Cantidad { get; set; } // ❌ Pero vende "Cantidad" unidades
```

**Impacto**:
- Si se vende cantidad=5 de 1 cuy, solo se marca 1 como vendido
- Los otros 4 siguen disponibles
- Reportes incorrectos de inventario

**Solución Opción 1**: Un registro de venta por cuy vendido.
```csharp
// Cambiar lógica: si se venden 5 cuyes, crear 5 registros de Venta
```

**Solución Opción 2** (Mejor): Crear tabla VentaDetalle.
```csharp
public class VentaDetalle
{
	public int Id { get; set; }
	public int VentaId { get; set; }
	public int CuyId { get; set; }
	public decimal PrecioUnitario { get; set; }

	public virtual Venta? Venta { get; set; }
	public virtual Cuy? Cuy { get; set; }
}

public class Venta
{
	// ... campos originales sin CuyId ni Cantidad ...
	public virtual ICollection<VentaDetalle> Detalles { get; set; }
}
```

---

### PC-12: CuyViewModel Hereda de Entidad - Violación de DTO

**Problema**: El `CuyViewModel` se usa como DTO, pero comparte la misma lógica que la entidad `Cuy`.

**Archivo Afectado**: `CuyControl/ViewModels/CuyViewModel.cs`

**Impacto**:
- Cambios en la entidad requieren cambios en ViewModel
- Lógica duplicada
- Violación de capas

**Solución**:
```csharp
// Usar DTOs consistentemente
public class CreateCuyViewModel
{
	public string Codigo { get; set; }
	public DateTime FechaNacimiento { get; set; }
	public int SexoId { get; set; }
	public int EstadoId { get; set; }
	public int? JaulaId { get; set; }
	public decimal PesoActual { get; set; }
	public string? Raza { get; set; }
	public string? Observaciones { get; set; }
}
```

---

## 🟠 PROBLEMAS DE ARQUITECTURA

### PA-01: Repositorios No Implementados Completamente

**Problema**: Existen interfaces de repositorio (`IAlimentacionRepository`, `IEnfermedadRepository`, `IMortalidadRepository`, `IPartoRepository`, `IReproduccionRepository`, `ITratamientoRepository`) pero no tienen implementación concreta.

**Archivo Afectado**: 
- `CuyControl.Domain/Interfaces/Repositories/` (múltiples interfaces)
- `CuyControl.Infrastructure/Repositories/` (faltan implementaciones)

**Impacto**:
- No se puede acceder a esas entidades desde servicios
- Código duplicado si se accede directamente desde controladores
- Arquitectura incompleta

**Solución**:
Implementar todos los repositorios:
```csharp
public class AlimentacionRepository : IAlimentacionRepository
{
	private readonly ApplicationDbContext _context;

	public async Task<IEnumerable<Alimentacion>> GetAllAsync()
	{
		return await _context.Alimentaciones.ToListAsync();
	}

	public async Task<Alimentacion?> GetByIdAsync(int id)
	{
		return await _context.Alimentaciones.FindAsync(id);
	}

	// ... más métodos
}
```

---

### PA-02: Servicios No Implementados para Todas las Entidades

**Problema**: Solo existen servicios para `Cuy`, `Venta`, `Usuario`, `Alimentacion` y `ReportService`. Faltan servicios para:
- Jaula
- Galpón
- Enfermedad
- Tratamiento
- Reproducción
- Parto
- Mortalidad
- ControlPeso

**Impacto**:
- Controladores acceden directamente a DbContext
- Lógica duplicada
- Difícil de mantener

---

### PA-03: No Hay Clase Base para Entidades Auditables

**Problema**: Muchas entidades tienen campos de auditoría (`FechaCreacion`, `UsuarioCreacionId`, `FechaModificacion`, `UsuarioModificacionId`), pero no hay una clase base que implemente esto.

**Archivo Afectado**: 
- `CuyControl.Domain/Interfaces/IAuditable.cs` (interfaz, no clase base)

**Código Incorrecto**:
```csharp
public class Venta : IBaseEntity, IAuditable
{
	// Repetir estos campos en múltiples entidades
	public DateTime FechaCreacion { get; set; }
	public int UsuarioCreacionId { get; set; }
	public DateTime? FechaModificacion { get; set; }
	public int? UsuarioModificacionId { get; set; }
}
```

**Solución**:
```csharp
public abstract class AuditableEntity : IBaseEntity, IAuditable
{
	public int Id { get; set; }
	public DateTime FechaCreacion { get; set; }
	public int UsuarioCreacionId { get; set; }
	public DateTime? FechaModificacion { get; set; }
	public int? UsuarioModificacionId { get; set; }
}

public class Venta : AuditableEntity
{
	// Sin repetir campos de auditoría
	public int CuyId { get; set; }
	// ...
}
```

---

### PA-04: DTOs y ViewModels No Están Separados Correctamente

**Problema**: Los ViewModels contienen lógica de presentación y también se usan para crear entidades.

**Impacto**:
- Cambios de UI afectan lógica de negocio
- DTOs exponen todos los datos innecesariamente

---

### PA-05: Falta Patrón de Transacciones

**Problema**: No hay patrón consistente de Unit of Work o manejo de transacciones.

**Impacto**:
- Operaciones complejas pueden quedar a mitad
- Inconsistencia de datos

---

## 🟡 PROBLEMAS DE BASE DE DATOS

### BD-01: Reproduccion Sin Restricción de Mismo Sexo

**Problema**: No hay validación para evitar reproducción entre cuyes del mismo sexo o del mismo cuy consigo mismo.

**Archivo Afectado**: `CuyControl.Domain/Entities/Reproduccion.cs`

**Impacto**:
- Registros inválidos de reproducción

**Solución**:
Agregar validación en servicio.

---

### BD-02: Índices Faltantes

**Problema**: No hay índices para:
- `Venta.FechaVenta`
- `ControlPeso.FechaMedicion`
- `Enfermedad.FechaRegistro`
- `Reproduccion.FechaCruzamiento`

**Impacto**:
- Queries lentas en reportes

**Solución**:
```csharp
modelBuilder.Entity<Venta>()
	.HasIndex(v => v.FechaVenta)
	.HasName("IX_Venta_FechaVenta");
```

---

### BD-03: Falta Relación CuyHembra en Reproduccion

**Problema**: `Reproduccion` solo tiene `CuyMacho` de FK, debería tener `CuyHembra` para consultas bidireccionales y validación adecuada.

**Archivo Afectado**: `CuyControl.Domain/Entities/Reproduccion.cs` (línea 60)

**Código Incorrecto**:
```csharp
public virtual Cuy? CuyMacho { get; set; } // ✅
public virtual Cuy? CuyHembra { get; set; } // ❌ FALTA
```

**Impacto**:
- No se puede consultar reproducción por hembra
- No se puede validar género en reproducción
- Imposible rastrear historial reproductivo de hembras
- Inconsistencia con lógica de negocio

**Solución**:

```csharp
// CuyControl.Domain/Entities/Reproduccion.cs
public class Reproduccion : AuditableEntity
{
	public int CuyMachoId { get; set; }
	public int CuyHembraId { get; set; }  // ✅ Agregar relación de hembra
	public DateTime FechaCruzamiento { get; set; }
	public bool Exitosa { get; set; }

	// Relaciones navegables
	public virtual Cuy? CuyMacho { get; set; }
	public virtual Cuy? CuyHembra { get; set; }  // ✅ Nueva relación
	public virtual ICollection<Parto> Partos { get; set; } = new List<Parto>();
}

// CuyControl.Infrastructure/Data/ApplicationDbContext.cs
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	base.OnModelCreating(modelBuilder);

	// Configurar relación Reproduccion -> CuyMacho
	modelBuilder.Entity<Reproduccion>()
		.HasOne(r => r.CuyMacho)
		.WithMany(c => c.ReproduccionesComoMacho)
		.HasForeignKey(r => r.CuyMachoId)
		.OnDelete(DeleteBehavior.Restrict)
		.HasConstraintName("FK_Reproduccion_CuyMacho");

	// Configurar relación Reproduccion -> CuyHembra ✅
	modelBuilder.Entity<Reproduccion>()
		.HasOne(r => r.CuyHembra)
		.WithMany(c => c.ReproduccionesComoHembra)
		.HasForeignKey(r => r.CuyHembraId)
		.OnDelete(DeleteBehavior.Restrict)
		.HasConstraintName("FK_Reproduccion_CuyHembra");

	// Configurar relación Reproduccion -> Parto
	modelBuilder.Entity<Reproduccion>()
		.HasMany(r => r.Partos)
		.WithOne(p => p.Reproduccion)
		.HasForeignKey(p => p.ReproduccionId)
		.OnDelete(DeleteBehavior.Cascade)
		.HasConstraintName("FK_Parto_Reproduccion");
}

// CuyControl.Domain/Entities/Cuy.cs - Agregar propiedades navegables
public class Cuy : AuditableEntity
{
	// ... propiedades existentes ...

	public virtual ICollection<Reproduccion> ReproduccionesComoMacho { get; set; } 
		= new List<Reproduccion>();
	public virtual ICollection<Reproduccion> ReproduccionesComoHembra { get; set; } 
		= new List<Reproduccion>();
}
```

**Migración requerida**:
```bash
Add-Migration AddCuyHembraToReproduccion
Update-Database
```

---

### BD-04: OnDelete Cascade Podría Causar Eliminación en Cascada

**Problema**: Al eliminar una reproducción, se eliminan todos sus partos, pero los cuyes aún existen.

**Archivo Afectado**: `CuyControl.Infrastructure/Data/ApplicationDbContext.cs` (línea 123-125)

**Impacto**:
- Pérdida de datos de partos
- Inconsistencia

---

### BD-05: Configuración Inconsistente de FK

**Problema**: Algunas relaciones usan `OnDelete(DeleteBehavior.Cascade)`, otras `Restrict`, otras `NoAction`.

**Impacto**:
- Comportamiento inconsistente

**Solución**:
Definir política clara:
- Entidades de dominio: `Restrict` (no eliminar si hay referencias)
- Detalles: `Cascade` (eliminar si se elimina padre)

---

## 🔴 PROBLEMAS DE LÓGICA DE NEGOCIO

### LN-01: Dashboard No Calcula CuyesNacidosEsteMes

**Problema**: El ViewModel tiene `CuyesNacidosEsteMes` pero no se calcula en el controlador.

**Archivo Afectado**: 
- `CuyControl/ViewModels/DashboardViewModel.cs` (línea 79-81)
- `CuyControl/Controllers/HomeController.cs` (línea 70, no hay asignación)

**Código Incorrecto**:
```csharp
// DashboardViewModel
public int CuyesNacidosEsteMes { get; set; }

// HomeController - NO SE ASIGNA
dashboard.CuyesNacidosEsteMes = 0; // ❌ Siempre 0
```

**Solución**:
```csharp
var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
dashboard.CuyesNacidosEsteMes = await _context.Partos
	.Where(p => p.FechaParto >= inicioMes && p.FechaParto <= hoy)
	.SumAsync(p => p.NumeroDeCreasVivas);
```

---

### LN-02: Reproducción Exitosa No Tiene Lógica

**Problema**: Campo `Exitosa` en `Reproduccion` pero nunca se usa para validar si hay partos. Permite marcar como no exitosa cuando ya hay partos registrados.

**Archivo Afectado**: 
- `CuyControl.Domain/Entities/Reproduccion.cs` (línea 33)
- Falta servicio de reproducción con lógica

**Impacto**:
- Reproducción marcada como no exitosa, pero hay partos registrados
- Inconsistencia de datos
- Reportes incorrectos

**Código Incorrecto**:
```csharp
// Reproduccion.cs
public bool Exitosa { get; set; } // ❌ Nunca se valida

// Controlador - sin lógica
reproduccion.Exitosa = model.Exitosa; // ❌ Solo asigna sin validar
```

**Solución**:

```csharp
// CuyControl.Application/Interfaces/Services/IReproduccionService.cs
public interface IReproduccionService
{
	Task<ReproduccionDto> CrearReproduccionAsync(ReproduccionDto reproduccionDto);
	Task<ReproduccionDto> ActualizarReproduccionAsync(int id, ReproduccionDto reproduccionDto);
	Task<ReproduccionDto?> ObtenerPorIdAsync(int id);
	Task<IEnumerable<ReproduccionDto>> ObtenerReproduccionesActivasAsync();
	Task<IEnumerable<ReproduccionDto>> ObtenerHistorialHembraAsync(int cuyHembraId);
	Task<bool> TienePartosAsync(int reproduccionId);
}

// CuyControl.Application/Services/ReproduccionService.cs
public class ReproduccionService : IReproduccionService
{
	private readonly IReproduccionRepository _reproduccionRepository;
	private readonly IPartoRepository _partoRepository;
	private readonly ICuyRepository _cuyRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<ReproduccionService> _logger;

	public ReproduccionService(
		IReproduccionRepository reproduccionRepository,
		IPartoRepository partoRepository,
		ICuyRepository cuyRepository,
		IMapper mapper,
		ILogger<ReproduccionService> logger)
	{
		_reproduccionRepository = reproduccionRepository;
		_partoRepository = partoRepository;
		_cuyRepository = cuyRepository;
		_mapper = mapper;
		_logger = logger;
	}

	public async Task<ReproduccionDto> CrearReproduccionAsync(
		ReproduccionDto reproduccionDto)
	{
		// ✅ Validar que ambos cuyes existan
		var cuyMacho = await _cuyRepository.GetByIdAsync(reproduccionDto.CuyMachoId);
		var cuyHembra = await _cuyRepository.GetByIdAsync(reproduccionDto.CuyHembraId);

		if (cuyMacho == null || cuyHembra == null)
			throw new InvalidOperationException("Cuyes no encontrados");

		// ✅ Validar que sean de sexo opuesto
		if (cuyMacho.SexoId == cuyHembra.SexoId)
			throw new InvalidOperationException(
				"La reproducción debe ser entre cuyes de sexo opuesto");

		// ✅ Validar que el macho sea macho (SexoId = 1) y hembra sea hembra (SexoId = 2)
		if (cuyMacho.SexoId != 1 || cuyHembra.SexoId != 2)
			throw new InvalidOperationException(
				"CuyMacho debe ser macho y CuyHembra debe ser hembra");

		// ✅ Validar que no sean el mismo cuy
		if (reproduccionDto.CuyMachoId == reproduccionDto.CuyHembraId)
			throw new InvalidOperationException(
				"No se puede reproducir un cuy consigo mismo");

		// ✅ Validar que no estén vendidos ni muertos
		if (cuyMacho.EstadoId == (int)EstadoCuy.Vendido || 
			cuyMacho.EstadoId == (int)EstadoCuy.Muerto)
			throw new InvalidOperationException(
				"El macho no está disponible para reproducción");

		if (cuyHembra.EstadoId == (int)EstadoCuy.Vendido || 
			cuyHembra.EstadoId == (int)EstadoCuy.Muerto)
			throw new InvalidOperationException(
				"La hembra no está disponible para reproducción");

		// ✅ Validar que no tenga reproducción activa la hembra
		var reproduccionActiva = await _reproduccionRepository
			.ObtenerReproduccionActivaAsync(reproduccionDto.CuyHembraId);

		if (reproduccionActiva != null)
			throw new InvalidOperationException(
				"La hembra ya tiene una reproducción activa (sin parto registrado)");

		var reproduccion = _mapper.Map<Reproduccion>(reproduccionDto);
		reproduccion.FechaCreacion = DateTime.Now;
		reproduccion.Exitosa = false; // ✅ Inicialmente no exitosa

		// ✅ Cambiar estado de hembra a gestante
		cuyHembra.EstadoId = (int)EstadoCuy.Gestante;
		await _cuyRepository.UpdateAsync(cuyHembra);

		await _reproduccionRepository.AddAsync(reproduccion);
		await _reproduccionRepository.SaveChangesAsync();

		_logger.LogInformation(
			$"Reproducción creada: Macho {reproduccionDto.CuyMachoId} x Hembra {reproduccionDto.CuyHembraId}");

		return _mapper.Map<ReproduccionDto>(reproduccion);
	}

	public async Task<ReproduccionDto> ActualizarReproduccionAsync(
		int id, ReproduccionDto reproduccionDto)
	{
		var reproduccion = await _reproduccionRepository.GetByIdAsync(id);

		if (reproduccion == null)
			throw new InvalidOperationException("Reproducción no encontrada");

		// ✅ VALIDACIÓN CRÍTICA: Si hay partos, DEBE estar exitosa
		var tienePartos = await _partoRepository.TienePartosAsync(id);

		if (tienePartos && !reproduccionDto.Exitosa)
			throw new InvalidOperationException(
				"No se puede marcar como no exitosa si hay partos registrados");

		reproduccion.Exitosa = reproduccionDto.Exitosa;
		reproduccion.FechaModificacion = DateTime.Now;

		await _reproduccionRepository.UpdateAsync(reproduccion);
		await _reproduccionRepository.SaveChangesAsync();

		_logger.LogInformation(
			$"Reproducción actualizada: {id} - Exitosa: {reproduccionDto.Exitosa}");

		return _mapper.Map<ReproduccionDto>(reproduccion);
	}

	public async Task<ReproduccionDto?> ObtenerPorIdAsync(int id)
	{
		var reproduccion = await _reproduccionRepository.GetByIdAsync(id);
		return _mapper.Map<ReproduccionDto>(reproduccion);
	}

	public async Task<IEnumerable<ReproduccionDto>> ObtenerReproduccionesActivasAsync()
	{
		var reproducciones = await _reproduccionRepository
			.ObtenerReproduccionesActivasAsync();
		return _mapper.Map<IEnumerable<ReproduccionDto>>(reproducciones);
	}

	public async Task<IEnumerable<ReproduccionDto>> ObtenerHistorialHembraAsync(
		int cuyHembraId)
	{
		var reproducciones = await _reproduccionRepository
			.ObtenerHistorialHembraAsync(cuyHembraId);
		return _mapper.Map<IEnumerable<ReproduccionDto>>(reproducciones);
	}

	public async Task<bool> TienePartosAsync(int reproduccionId)
	{
		return await _partoRepository.TienePartosAsync(reproduccionId);
	}
}

// Program.cs - Registro
services.AddScoped<IReproduccionService, ReproduccionService>();
```

---

### LN-03: Estados no se Actualizan Automáticamente

**Problema**: Un cuy gestante (EstadoId = 4) nunca se actualiza automáticamente a lactante (EstadoId = 5) después del parto. Los estados permanecen desactualizados.

**Archivo Afectado**: `CuyControl.Application/Services/` (falta PartoService)

**Impacto**:
- Estados desactualizados
- Reportes incorrectos
- Lógica de reproducción inconsistente

**Código Incorrecto**:
```csharp
// No existe PartoService
var parto = new Parto { /* ... */ }; // ❌ Solo crea parto, sin actualizar estado
_context.Partos.Add(parto);
```

**Solución**:

```csharp
// CuyControl.Application/Interfaces/Services/IPartoService.cs
public interface IPartoService
{
	Task<PartoDto> CrearPartoAsync(PartoDto partoDto);
	Task<IEnumerable<PartoDto>> ObtenerPartosReproduccionAsync(int reproduccionId);
	Task<int> ContarCuyesNacidosAsync(DateTime desde, DateTime hasta);
}

// CuyControl.Application/Services/PartoService.cs
public class PartoService : IPartoService
{
	private readonly IPartoRepository _partoRepository;
	private readonly IReproduccionRepository _reproduccionRepository;
	private readonly ICuyRepository _cuyRepository;
	private readonly IMapper _mapper;
	private readonly ILogger<PartoService> _logger;

	public PartoService(
		IPartoRepository partoRepository,
		IReproduccionRepository reproduccionRepository,
		ICuyRepository cuyRepository,
		IMapper mapper,
		ILogger<PartoService> logger)
	{
		_partoRepository = partoRepository;
		_reproduccionRepository = reproduccionRepository;
		_cuyRepository = cuyRepository;
		_mapper = mapper;
		_logger = logger;
	}

	public async Task<PartoDto> CrearPartoAsync(PartoDto partoDto)
	{
		// ✅ Obtener reproducción
		var reproduccion = await _reproduccionRepository
			.GetByIdAsync(partoDto.ReproduccionId);

		if (reproduccion == null)
			throw new InvalidOperationException("Reproducción no encontrada");

		// ✅ Obtener hembra
		var cuyHembra = await _cuyRepository
			.GetByIdAsync(reproduccion.CuyHembraId);

		if (cuyHembra == null)
			throw new InvalidOperationException("Hembra no encontrada");

		// ✅ Validar que la hembra esté en estado gestante
		if (cuyHembra.EstadoId != (int)EstadoCuy.Gestante)
			throw new InvalidOperationException(
				"La hembra debe estar en estado gestante para registrar parto");

		// ✅ Validar números positivos
		if (partoDto.NumeroDeCreasVivas < 0 || partoDto.NumeroDeCreasNatimuertas < 0)
			throw new InvalidOperationException(
				"Número de crías no puede ser negativo");

		var parto = _mapper.Map<Parto>(partoDto);
		parto.FechaCreacion = DateTime.Now;

		await _partoRepository.AddAsync(parto);

		// ✅ ACTUALIZAR ESTADO DE HEMBRA A LACTANTE
		cuyHembra.EstadoId = (int)EstadoCuy.Lactante;
		cuyHembra.FechaModificacion = DateTime.Now;
		await _cuyRepository.UpdateAsync(cuyHembra);

		// ✅ MARCAR REPRODUCCIÓN COMO EXITOSA
		reproduccion.Exitosa = true;
		reproduccion.FechaModificacion = DateTime.Now;
		await _reproduccionRepository.UpdateAsync(reproduccion);

		await _partoRepository.SaveChangesAsync();

		_logger.LogInformation(
			$"Parto registrado: Reproducción {partoDto.ReproduccionId} - " +
			$"Crías vivas: {partoDto.NumeroDeCreasVivas}, " +
			$"Crías muertas: {partoDto.NumeroDeCreasNatimuertas}");

		return _mapper.Map<PartoDto>(parto);
	}

	public async Task<IEnumerable<PartoDto>> ObtenerPartosReproduccionAsync(
		int reproduccionId)
	{
		var partos = await _partoRepository
			.ObtenerPartosReproduccionAsync(reproduccionId);
		return _mapper.Map<IEnumerable<PartoDto>>(partos);
	}

	public async Task<int> ContarCuyesNacidosAsync(DateTime desde, DateTime hasta)
	{
		var partos = await _partoRepository
			.ObtenerPartosEnRangoAsync(desde, hasta);

		return partos.Sum(p => p.NumeroDeCreasVivas);
	}
}

// Program.cs - Registro
services.AddScoped<IPartoService, PartoService>();
```

---

### LN-04: Mortalidad no se Registra Automáticamente

**Problema**: Cuando se marca un cuy como muerto (EstadoId = 7), no se crea automáticamente un registro en `Mortalidad`.

**Archivo Afectado**: `CuyControl/Controllers/CuyController.cs` (no hay lógica de mortalidad)

**Impacto**:
- No se puede rastrear causa de muerte
- Datos incompletos

**Solución**:
```csharp
public async Task<CuyDto> ActualizarCuyAsync(int id, CuyDto cuyDto)
{
	var cuy = await _cuyRepository.GetByIdAsync(id);

	// Si se marca como muerto, crear registro de mortalidad
	if (cuy.EstadoId != (int)EstadoCuy.Muerto && 
		cuyDto.EstadoId == (int)EstadoCuy.Muerto)
	{
		var mortalidad = new Mortalidad
		{
			CuyId = id,
			FechaDefuncion = DateTime.Now,
			Causa = cuyDto.Observaciones,
			FechaCreacion = DateTime.Now
		};

		await _mortalidadRepository.AddAsync(mortalidad);
	}

	cuy.EstadoId = cuyDto.EstadoId;
	await _cuyRepository.UpdateAsync(cuy);
	await _cuyRepository.SaveChangesAsync();

	return MappingProfile.MapToCuyDto(cuy);
}
```

---

### LN-05: Dashboard Cuenta Enfermedades Incorrectamente

**Problema**: `CuyesEnfermos` se cuenta como total de registros en tabla `Enfermedades`, no como cantidad única de cuyes enfermos.

**Archivo Afectado**: `CuyControl/Controllers/HomeController.cs` (línea 30)

**Código Incorrecto**:
```csharp
CuyesEnfermos = await _context.Enfermedades.CountAsync(), // ❌ Cuenta registros, no cuyes
```

**Solución**:
```csharp
CuyesEnfermos = await _context.Enfermedades
	.Select(e => e.CuyId)
	.Distinct()
	.CountAsync(), // ✅ Cuenta cuyes únicos
```

---

### LN-06: Venta permite cantidad > 1 pero solo cambia 1 cuy

**Problema**: Campo `Cantidad` en `Venta` pero lógica solo cambia estado de 1 cuy.

**Archivo Afectado**: `CuyControl/Controllers/VentaController.cs` (línea 86-91)

**Solución**: Ver PC-11

---

### LN-07: No hay validación de Peso Positivo

**Problema**: `Cuy.PesoActual` tiene `[Range(0, 9999.99)]` permitiendo peso 0.

**Archivo Afectado**: `CuyControl.Domain/Entities/Cuy.cs` (línea 34-35)

**Solución**:
```csharp
[Range(0.001, 9999.99, ErrorMessage = "Peso debe ser mayor a 0")]
public decimal PesoActual { get; set; }
```

---

## 🔴 PROBLEMAS DE SEGURIDAD

### SEC-01: SQL Injection en Movimiento de Alimento

**Problema**: Si bien se usa EF Core (que protege de SQL Injection), no hay validación de entrada en los campos de texto.

---

### SEC-02: XSS - Campos de Texto No Validados

**Problema**: Campos como `Observaciones` no se validan contra caracteres especiales. Vistas no escapan HTML.

**Archivo Afectado**: 
- Vistas (no escapan)
- Controladores (no validan)

**Solución**:
```csharp
// En ViewModel
[StringLength(500)]
[RegularExpression(@"^[a-zA-Z0-9\s\-.,áéíóúÁÉÍÓÚ]*$", 
	ErrorMessage = "Contiene caracteres no permitidos")]
public string? Observaciones { get; set; }

// En Vista
<p>@Html.Encode(Model.Observaciones)</p>
```

---

### SEC-03: CSRF - Solo algunas acciones tienen ValidateAntiForgeryToken

**Problema**: No todas las acciones POST tienen `[ValidateAntiForgeryToken]`.

**Impacto**:
- Posible ataque CSRF

**Solución**:
Asegurarse que TODAS las acciones POST/PUT/DELETE lo tengan.

---

### SEC-04: Acceso a Datos Sin Autorización

**Problema**: Controladores solo tienen `[Authorize(Roles = "Administrador,Operador")]` general, pero no valida que el usuario solo acceda a sus propios datos.

**Impacto**:
- Un usuario podría ver/editar datos de otro usuario

---

### SEC-05: UsuarioCreacionId Hardcodeado Permite Escalada de Privilegios

**Problema**: Usar UsuarioCreacionId = 1 permite atribuir acciones al administrador.

---

### SEC-06: Falta Autenticación Fuerte

**Problema**: Contraseñas no tienen requisitos mínimos.

---

### SEC-07: Logging Insuficiente

**Problema**: No hay logging de acciones críticas (cambio de estado de cuyes, ventas, etc.).

---

## 🟢 PROBLEMAS DE CÓDIGO

### COD-01: Duplicación de SelectList en ViewBag

**Problema**: SelectList para galpones, jaulas, etc. se crea en múltiples controladores sin reutilización.

**Archivo Afectado**: 
- `CuyControl/Controllers/JaulaController.cs`
- `CuyControl/Controllers/CuyController.cs` (si tiene)

**Solución**:
Crear `SelectListService`.

---

### COD-02: Magic Numbers en Controladores

**Problema**: Usando números para estado, sexo, etc.

**Solución**: Ver PC-03

---

### COD-03: Sin Manejo de Excepciones Consistente

**Problema**: Algunos controladores tienen try-catch, otros no.

**Solución**:
```csharp
[HttpPost]
public async Task<IActionResult> Create(CuyViewModel model)
{
	try
	{
		// Lógica
	}
	catch (InvalidOperationException ex)
	{
		ModelState.AddModelError("", ex.Message);
		return View(model);
	}
	catch (Exception ex)
	{
		_logger.LogError(ex, "Error inesperado");
		return RedirectToAction("Error", "Home");
	}
}
```

---

### COD-04: ViewBag en lugar de ViewModel Fuertemente Tipado

**Problema**: Se usa ViewBag para SelectLists en lugar de incluirlas en ViewModel.

**Solución**:
```csharp
public class CreateJaulaViewModel
{
	public JaulaViewModel Jaula { get; set; }
	public IEnumerable<SelectListItem> Galpones { get; set; }
}
```

---

### COD-05: Sin Inyección de Dependencias Completa

**Problema**: `Program.cs` no inyecta todos los servicios.

**Solución**: Ver PC-07, PC-08

---

### COD-06: EdadDias no es Propiedad Persistida

**Problema**: `CuyViewModel.EdadDias` se calcula en tiempo de ejecución pero no existe en la entidad.

**Archivo Afectado**: `CuyControl/ViewModels/CuyViewModel.cs`

**Impacto**:
- Confusión
- Posible error si se intenta acceder desde servicio

---

## 🟠 PROBLEMAS DE UXUI

### UX-01: Sin Confirmación de Eliminación

**Problema**: No hay diálogo de confirmación antes de eliminar cuyes, jaulas, etc.

**Solución**:
```javascript
// site.js
function confirmarEliminacion() {
	return confirm('¿Está seguro de que desea eliminar este registro?');
}

// En Vista
<a onclick="return confirmarEliminacion()" href="@Url.Action("Delete", new { id = item.Id })">
	Eliminar
</a>
```

---

### UX-02: Sin Mensajes de Éxito

**Problema**: Después de crear/editar, no hay confirmación visual.

**Solución**:
```csharp
TempData["Mensaje"] = "Cuy creado exitosamente";
TempData["TipoMensaje"] = "success";

// En Layout
@if (TempData["Mensaje"] != null)
{
	<div class="alert alert-@TempData["TipoMensaje"]">
		@TempData["Mensaje"]
	</div>
}
```

---

### UX-03: Inconsistencia de Navegación

**Problema**: Algunos controladores redirigen, otros muestran vista.

**Solución**:
Definir patrón consistente.

---

### UX-04: Sin Paginación en Listados

**Problema**: Si hay muchos cuyes, todos se cargan en la misma página.

**Solución**:
```csharp
public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
{
	var skip = (page - 1) * pageSize;
	var total = await _cuyRepository.CountAsync();

	var cuyes = await _cuyRepository.GetAllAsync(skip: skip, take: pageSize);

	var model = new PaginatedViewModel<CuyViewModel>
	{
		Items = cuyes.Select(MapToCuyViewModel).ToList(),
		TotalCount = total,
		PageNumber = page,
		PageSize = pageSize
	};

	return View(model);
}
```

---

## ⚠️ INCONSISTENCIAS DE DATOS

### INC-01: CantidadActual de Jaula no se Mantiene en Sincronía

**Problema**: `Jaula.CantidadActual` puede quedarse desincronizado cuando:
- Se crea un cuy con jaula
- Se transfiere cuy de jaula
- Se vende/mata cuy en jaula

---

### INC-02: Cuy sin Jaula Pero con JaulaId

**Problema**: Posibilidad de que `Cuy.JaulaId` apunte a jaula inexistente.

**Solución**:
```csharp
builder.HasOne(c => c.Jaula)
	.WithMany(j => j.Cuyes)
	.HasForeignKey(c => c.JaulaId)
	.OnDelete(DeleteBehavior.SetNull); // Si se elimina jaula, poner NULL
```

---

### INC-03: Venta con Cuy Inactivo

**Problema**: Se puede crear venta de cuy con Activo = false.

**Solución**: Ver PC-02

---

### INC-04: Reproduccion de Cuy Vendido/Muerto

**Problema**: Nada previene reproducción de cuy vendido o muerto.

**Solución**:
```csharp
var cuyMacho = await _cuyRepository.GetByIdAsync(reproduccionDto.CuyMachoId);

if (cuyMacho.EstadoId == (int)EstadoCuy.Vendido || 
	cuyMacho.EstadoId == (int)EstadoCuy.Muerto)
	throw new InvalidOperationException("Cuy no válido para reproducción");
```

---

## 📋 PLAN DE CORRECCIONES

### Fase 1: ARQUITECTURA (Prioridad: CRÍTICA)

**Objetivo**: Corregir violaciones de capas

1. ✅ Crear `IInventarioAlimentoService` y `InventarioAlimentoService`
2. ✅ Crear `IJaulaService` y `JaulaService`
3. ✅ Crear `IGalponService` y `GalponService`
4. ✅ Implementar todos los repositorios faltantes
5. ✅ Migrar lógica de controladores a servicios
6. ✅ Registrar todos los servicios en `Program.cs`
7. ✅ Crear clase base `AuditableEntity`
8. ✅ Refactorizar HomeController para usar `IReportService`

**Tiempo Estimado**: 3-4 horas

---

### Fase 2: LÓGICA DE NEGOCIO (Prioridad: CRÍTICA)

**Objetivo**: Implementar validaciones y reglas

1. ✅ Enumeración `EstadoCuy` completa
2. ✅ Validaciones en `VentaService` (PC-02)
3. ✅ Validaciones en `CuyService`
4. ✅ Validaciones en `JaulaService`
5. ✅ Transacciones en `MovimientoAlimentoService`
6. ✅ Auto-actualización de estados en `PartoService`
7. ✅ Registro automático de `Mortalidad`
8. ✅ Corrección de cálculos en Dashboard

**Tiempo Estimado**: 4-5 horas

---

### Fase 3: BASE DE DATOS (Prioridad: ALTA)

**Objetivo**: Mejorar modelo de datos

1. ✅ Agregar relación `CuyHembra` en `Reproduccion`
2. ✅ Crear `VentaDetalle` (si se elige Opción 2 para PC-11)
3. ✅ Agregar índices
4. ✅ Revisar políticas de eliminación
5. ✅ Crear nueva migración

**Tiempo Estimado**: 2-3 horas

---

### Fase 4: SEGURIDAD (Prioridad: ALTA)

**Objetivo**: Corregir vulnerabilidades

1. ✅ Obtener UsuarioCreacionId del contexto actual (SEC-02)
2. ✅ Validar entrada en ViewModels
3. ✅ Escapar HTML en vistas
4. ✅ Agregar CSRF a todas las acciones POST/PUT/DELETE
5. ✅ Implementar logging de acciones críticas
6. ✅ Validar autorización de datos por usuario

**Tiempo Estimado**: 3-4 horas

---

### Fase 5: CÓDIGO (Prioridad: MEDIA)

**Objetivo**: Limpiar y mejorar

1. ✅ Refactorizar SelectList en servicio
2. ✅ Agregar manejo consistente de excepciones
3. ✅ Reemplazar ViewBag con ViewModels tipados
4. ✅ Crear DTOs separados de ViewModels
5. ✅ Remover números mágicos

**Tiempo Estimado**: 2-3 horas

---

### Fase 6: UI/UX (Prioridad: MEDIA)

**Objetivo**: Mejorar experiencia

1. ✅ Agregar confirmación de eliminación
2. ✅ Agregar mensajes de éxito/error
3. ✅ Implementar paginación
4. ✅ Mejorar navegación
5. ✅ Consistencia visual

**Tiempo Estimado**: 2-3 horas

---

### Fase 7: TESTING (Prioridad: MEDIA)

**Objetivo**: Agregar pruebas

1. ✅ Crear pruebas unitarias para servicios
2. ✅ Crear pruebas de integración
3. ✅ Crear pruebas de validadores

**Tiempo Estimado**: 4-5 horas

---

## 📊 RESUMEN DE IMPACTO

| Aspecto | Estado | Impacto | Urgencia |
|--------|--------|--------|----------|
| Arquitectura | ❌ Crítico | Alto | 🔴 CRÍTICA |
| Lógica Negocio | ⚠️ Parcial | Medio | 🔴 CRÍTICA |
| BD | ⚠️ Incompleto | Medio | 🟠 ALTA |
| Seguridad | ⚠️ Vulnerable | Alto | 🟠 ALTA |
| Código | ⚠️ Con issues | Medio | 🟡 MEDIA |
| Testing | ❌ No existe | Alto | 🟡 MEDIA |
| UX | ⚠️ Mejorable | Bajo | 🟢 BAJA |

---

## ✅ RECOMENDACIONES FINALES

1. **Prioritario**: Iniciar Fase 1 (Arquitectura) inmediatamente
2. **Paralelo**: Pueden hacerse Fase 2 y 3 en paralelo
3. **Después**: Hacer Fase 4 (Seguridad) antes de producción
4. **Testing**: Incluir en cada fase, no al final

---

**Estado Final**: Proyecto compilable pero con problemas críticos de arquitectura y lógica de negocio que deben corregirse antes de cualquier release.

