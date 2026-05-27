# GUÍA PARA FASE 2: COMPLETAR ARQUITECTURA

## Objetivo
Completar la migración de lógica de negocio del DbContext directo a servicios y repositorios, implementando Clean Architecture completa.

---

## PASO 1: Crear Servicios de Dominio Faltantes

### 1.1 IJaulaService
```csharp
public interface IJaulaService
{
	Task<JaulaDto> CrearJaulaAsync(JaulaDto dto);
	Task<JaulaDto> ActualizarJaulaAsync(int id, JaulaDto dto);
	Task<bool> ValidarCapacidadAsync(int jaulaId, int cuyesAAsignar);
	Task<bool> AsignarCuyesAsync(int jaulaId, int cantidad);
	Task<bool> DesasignarCuyesAsync(int jaulaId, int cantidad);
}
```

### 1.2 IGalponService
```csharp
public interface IGalponService
{
	Task<GalponDto> CrearGalponAsync(GalponDto dto);
	Task<GalponDto> ActualizarGalponAsync(int id, GalponDto dto);
	Task<bool> ValidarNoTieneJaulasAsync(int galponId);
}
```

### 1.3 IAlimentacionService (Mejorado)
Extender con validaciones de stock mínimo y alertas.

---

## PASO 2: Implementar Todos los Repositorios

### 2.1 Faltantes a Crear
- EnfermedadRepository
- MortalidadRepository
- PartoRepository
- ReproduccionRepository
- TratamientoRepository
- CuyRepository (completar si existe parcial)

### 2.2 Patrón de Implementación
```csharp
public class JaulaRepository : GenericRepository<Jaula>, IJaulaRepository
{
	public JaulaRepository(ApplicationDbContext context) : base(context) { }

	public async Task<IEnumerable<Jaula>> GetJaulasByGalponAsync(int galponId)
	{
		return await _dbSet.Where(j => j.GalponId == galponId).ToListAsync();
	}
}
```

---

## PASO 3: Registrar en Program.cs

```csharp
// Servicios de Dominio
builder.Services.AddScoped<IJaulaService, JaulaService>();
builder.Services.AddScoped<IGalponService, GalponService>();

// Repositorios
builder.Services.AddScoped<IJaulaRepository, JaulaRepository>();
builder.Services.AddScoped<IGalponRepository, GalponRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

---

## PASO 4: Refactorizar Controladores Restantes

### Prioridad Alta:
1. ReproduccionController
2. PartoController
3. EnfermedadController
4. TratamientoController
5. AccountController

### Patrón a Aplicar a Cada Uno:
```csharp
[Authorize]
public class MiController : Controller
{
	private readonly IMiService _service;
	private readonly ILogger<MiController> _logger;

	// Inyección en constructor
	public MiController(IMiService service, ILogger<MiController> logger)
	{
		_service = service ?? throw new ArgumentNullException(nameof(service));
		_logger = logger;
	}

	private int GetCurrentUserId() { /* ... */ }

	public async Task<IActionResult> Index()
	{
		try
		{
			var datos = await _service.ObtenerTodosAsync();
			return View(datos);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error en acción");
			return RedirectToAction("Error", "Home");
		}
	}
}
```

---

## PASO 5: Implementar DTOs Completos

Crear DTOs para todos los servicios:
- JaulaDto
- GalponDto
- ReproduccionDto
- PartoDto
- EnfermedadDto
- TratamientoDto

Patrón:
```csharp
public class JaulaDto
{
	public int Id { get; set; }
	public string Codigo { get; set; }
	public int Capacidad { get; set; }
	public int CantidadActual { get; set; }
	public int GalponId { get; set; }
	public bool Disponible { get; set; }
}
```

---

## PASO 6: Validaciones de Negocio Complejas

### 6.1 Reproducción
- Validar macho y hembra != null
- Validar sexo correcto
- Validar edad mínima
- Validar no sean parientes
- Validar no estén en otro evento reproductivo

### 6.2 Partos
- Validar hembra existe
- Validar hembra estaba gestante
- Actualizar estado a Lactante
- Registrar crías

### 6.3 Mortalidad
- Validar cuy existe
- Actualizar EstadoId = 7 (Muerto)
- Registrar causa si aplica
- Quitar de disponibles

---

## PASO 7: Transacciones para Operaciones Complejas

Ejemplo:
```csharp
public async Task<VentaDto> CrearVentaAsync(VentaDto dto)
{
	using (var transaction = await _context.Database.BeginTransactionAsync())
	{
		try
		{
			// 1. Validar cuy
			var cuy = await _cuyRepository.GetByIdAsync(dto.CuyId);
			if (cuy == null) throw new InvalidOperationException("Cuy no encontrado");

			// 2. Crear venta
			var venta = new Venta { /* ... */ };
			await _ventaRepository.AddAsync(venta);

			// 3. Actualizar cuy
			cuy.EstadoId = (int)EstadoCuy.Vendido;
			cuy.Activo = false;
			await _cuyRepository.UpdateAsync(cuy);

			// 4. Guardar
			await _cuyRepository.SaveChangesAsync();
			await transaction.CommitAsync();

			return _mapper.Map<VentaDto>(venta);
		}
		catch
		{
			await transaction.RollbackAsync();
			throw;
		}
	}
}
```

---

## PASO 8: Migraciones de Base de Datos

Si necesario:
```bash
dotnet ef migrations add AddAuditableEntity
dotnet ef migrations add UpdateEstadoCuy
dotnet ef database update
```

---

## PASO 9: Pruebas

### Tests Unitarios
```csharp
[Fact]
public async Task CrearVenta_DebeActualizarEstadoCuy()
{
	// Arrange
	var cuy = new Cuy { Id = 1, EstadoId = (int)EstadoCuy.Reproductor };
	var venta = new Venta { CuyId = 1 };

	// Act
	await _ventaService.CrearVentaAsync(ventaDto);

	// Assert
	var cuyActualizado = await _cuyRepository.GetByIdAsync(1);
	Assert.Equal((int)EstadoCuy.Vendido, cuyActualizado.EstadoId);
}
```

---

## Checklist de Implementación

- [ ] Crear todas las interfaces de servicios
- [ ] Implementar todos los repositorios
- [ ] Crear DTOs completos
- [ ] Registrar servicios y repositorios en DI
- [ ] Refactorizar controladores faltantes
- [ ] Implementar validaciones complejas
- [ ] Agregar transacciones donde sea necesario
- [ ] Crear migraciones si aplica
- [ ] Escribir tests
- [ ] Verificar compilación
- [ ] Prueba manual de flujos

---

## Estimado de Tiempo

- Paso 1-2: 2 horas
- Paso 3-4: 2 horas  
- Paso 5-6: 1 hora
- Paso 7-8: 1 hora
- Paso 9: 1-2 horas

**Total**: 7-9 horas

