# CORRECCIONES - INVENTARIO DE ALIMENTOS Y REPRODUCCIÓN

## 📋 RESUMEN EJECUTIVO

Este documento detalla las correcciones específicas requeridas para los módulos de **Inventario de Alimentos** y **Reproducción** en CuyControl.

**Problemas Críticos Identificados**: 6
**Problemas Altos**: 4
**Problemas Medios**: 2

---

## 🎯 INVENTARIO DE ALIMENTOS

### Problema 1: Falta de Servicio para Inventario (PC-07) ⚠️ CRÍTICO

**Estado Actual**:
- ❌ No existe `IInventarioAlimentoService`
- ❌ Controlador accede directamente a `ApplicationDbContext`
- ❌ Sin validaciones en la capa de aplicación

**Solución Requerida**:
```bash
CREAR: CuyControl.Application/Interfaces/Services/IInventarioAlimentoService.cs
CREAR: CuyControl.Application/Services/InventarioAlimentoService.cs
MODIFICAR: Program.cs (registrar servicio)
```

**Validaciones a Implementar**:
- ✅ Tipo de alimento requerido y no vacío
- ✅ Cantidad no negativa
- ✅ Validar unicidad de tipo de alimento
- ✅ Logging de operaciones

**Métodos Principales**:
- `CrearInventarioAsync()` - Crear nuevo inventario
- `ActualizarInventarioAsync()` - Actualizar existente
- `ObtenerTodosAsync()` - Listar todos
- `ObtenerPorIdAsync()` - Obtener específico
- `EliminarAsync()` - Eliminar inventario

---

### Problema 2: Movimiento de Alimento Sin Validación (PC-09) 🔴 CRÍTICO

**Estado Actual**:
- ❌ Acceso directo a BD desde controlador
- ❌ Sin lock para operaciones concurrentes
- ❌ Posible stock negativo en ambiente multi-usuario
- ❌ Sin manejo de excepciones consistente

**Riesgo de Concurrencia**:
```
Thread 1: Lee stock = 10
Thread 2: Lee stock = 10
Thread 1: Resta 8 → Stock = 2
Thread 2: Resta 7 → Stock = 3  ❌ INCORRECTO! Debería ser -5
```

**Solución Requerida**:
```bash
CREAR: CuyControl.Application/Interfaces/Services/IMovimientoAlimentoService.cs
CREAR: CuyControl.Application/Services/MovimientoAlimentoService.cs
MODIFICAR: Program.cs (registrar servicio)
MODIFICAR: MovimientoAlimentoController.cs (usar servicio)
```

**Validaciones a Implementar**:
- ✅ Cantidad > 0 (positiva)
- ✅ Para SALIDA: Validar stock suficiente
- ✅ Para ENTRADA: No hay restricción
- ✅ Lock de escritura en BD
- ✅ Transacción atómica
- ✅ Logging detallado

**Métodos Principales**:
- `RegistrarMovimientoAsync()` - Registrar entrada/salida
- `ObtenerMovimientosAsync()` - Histórico
- `ObtenerStockActualAsync()` - Consultar stock

---

## 🔄 REPRODUCCIÓN

### Problema 3: Falta Relación CuyHembra en Reproduccion (BD-03) 🟠 ALTO

**Estado Actual**:
- ❌ Tabla `Reproduccion` solo tiene `CuyMachoId` y `CuyMacho`
- ❌ No hay `CuyHembraId` ni navegación a hembra
- ❌ Imposible consultar historial de hembras
- ❌ Lógica de validación incompleta

**Diagrama Actual**:
```
Reproduccion
├── CuyMachoId (FK) → Cuy (Macho) ✅
└── ❌ FALTA: CuyHembraId (FK) → Cuy (Hembra)
```

**Solución Requerida**:
```bash
MODIFICAR: CuyControl.Domain/Entities/Reproduccion.cs (agregar CuyHembraId)
MODIFICAR: CuyControl.Domain/Entities/Cuy.cs (agregar navegables)
MODIFICAR: ApplicationDbContext.cs (configurar relaciones)
CREAR: Migración EF Core
```

**Cambios en Entidades**:

```csharp
// Reproduccion.cs
public int CuyMachoId { get; set; }
public int CuyHembraId { get; set; }  // ✅ NUEVO

public virtual Cuy? CuyMacho { get; set; }
public virtual Cuy? CuyHembra { get; set; }  // ✅ NUEVO

// Cuy.cs
public virtual ICollection<Reproduccion> ReproduccionesComoMacho { get; set; }
public virtual ICollection<Reproduccion> ReproduccionesComoHembra { get; set; }
```

**Configuración en DbContext**:
```csharp
modelBuilder.Entity<Reproduccion>()
	.HasOne(r => r.CuyHembra)
	.WithMany(c => c.ReproduccionesComoHembra)
	.HasForeignKey(r => r.CuyHembraId)
	.OnDelete(DeleteBehavior.Restrict);
```

---

### Problema 4: Reproducción Exitosa Sin Lógica (LN-02) 🔴 CRÍTICO

**Estado Actual**:
- ❌ Campo `Exitosa` existe pero nunca se valida
- ❌ Permite marcar como no exitosa si ya hay partos
- ❌ Sin servicio de reproducción
- ❌ Falta validación de género

**Ejemplo de Inconsistencia**:
```
Reproducción ID=1:
  - CuyMacho: ID=5, Sexo=Macho ✓
  - CuyHembra: FALTA (no hay validación)
  - Exitosa: false
  - Partos: 3 registrados ❌ INCONSISTENCIA!
```

**Solución Requerida**:
```bash
CREAR: CuyControl.Application/Interfaces/Services/IReproduccionService.cs
CREAR: CuyControl.Application/Services/ReproduccionService.cs
MODIFICAR: Program.cs (registrar servicio)
MODIFICAR: Controllers/ReproduccionController.cs (usar servicio)
```

**Validaciones a Implementar en CrearReproduccionAsync()**:
- ✅ Ambos cuyes existen
- ✅ Sexo opuesto (macho ≠ hembra)
- ✅ CuyMacho es macho (SexoId = 1)
- ✅ CuyHembra es hembra (SexoId = 2)
- ✅ No son el mismo cuy
- ✅ No están vendidos/muertos
- ✅ Hembra no tiene reproducción activa
- ✅ Cambiar estado hembra a GESTANTE

**Validaciones en ActualizarReproduccionAsync()**:
- ✅ CRÍTICO: Si hay partos → DEBE ser exitosa
- ✅ No permitir cambiar a no exitosa si hay partos

**Métodos Principales**:
- `CrearReproduccionAsync()` - Crear reproducción
- `ActualizarReproduccionAsync()` - Actualizar y validar
- `ObtenerHistorialHembraAsync()` - Historial de hembra
- `ObtenerReproduccionesActivasAsync()` - Reproducciones pendientes

---

### Problema 5: Estados No se Actualizan Automáticamente (LN-03) 🔴 CRÍTICO

**Estado Actual**:
- ❌ No existe `IPartoService`
- ❌ Cuando se registra parto, hembra sigue como gestante
- ❌ Estados desactualizados en reportes
- ❌ Sin actualizar reproducción a exitosa

**Ciclo de Estados Esperado**:
```
CRÍA → RECRÍA → REPRODUCTOR
					↓ (emparejamiento)
				GESTANTE → LACTANTE → REPRODUCTOR
```

**Problema Actual**:
```
HEMBRA se empareja
Estado = GESTANTE ✓
Se registra parto
Estado = GESTANTE ❌ SIGUE IGUAL!
Debería ser = LACTANTE
```

**Solución Requerida**:
```bash
CREAR: CuyControl.Application/Interfaces/Services/IPartoService.cs
CREAR: CuyControl.Application/Services/PartoService.cs
MODIFICAR: Program.cs (registrar servicio)
MODIFICAR: Controllers/PartoController.cs (usar servicio)
```

**Lógica en CrearPartoAsync()**:
```csharp
// 1. Validar que reproducción existe
// 2. Validar que hembra esté en GESTANTE
// 3. Registrar parto
// 4. ACTUALIZAR: Hembra → LACTANTE  ✅ NUEVO
// 5. ACTUALIZAR: Reproducción → Exitosa = true  ✅ NUEVO
// 6. Log de operación
```

**Métodos Principales**:
- `CrearPartoAsync()` - Registrar parto con actualizaciones automáticas
- `ObtenerPartosReproduccionAsync()` - Listar partos
- `ContarCuyesNacidosAsync()` - Para dashboard

---

### Problema 6: Mortalidad No se Registra Automáticamente (LN-04) 🟠 ALTO

**Estado Actual**:
- ❌ No hay servicio de cuy
- ❌ Cuando se marca cuy como muerto, no se crea registro en Mortalidad
- ❌ Sin trazabilidad de causa de muerte
- ❌ Imposible hacer análisis de mortalidad

**Información Perdida**:
```
Cuy marcado como muerto:
- EstadoId = 7 ✓
- Pero: ¿Fecha? ¿Causa? ¿Observaciones? ❌ NO REGISTRADO
```

**Solución Requerida**:
```bash
CREAR: CuyControl.Application/Interfaces/Services/IMortalidadService.cs
CREAR: CuyControl.Application/Services/MortalidadService.cs
MODIFICAR: CuyService (o crear CuyService si no existe)
MODIFICAR: Program.cs (registrar servicio)
```

**Lógica Requerida**:
```csharp
// Cuando se actualiza un cuy de estado:
if (estadoAnterior != MUERTO && estadoNuevo == MUERTO)
{
	// Crear automáticamente registro en Mortalidad
	var mortalidad = new Mortalidad
	{
		CuyId = cuyId,
		FechaDefuncion = DateTime.Now,
		Causa = observaciones,
		UsuarioRegistroId = usuarioActual
	};
	await _mortalidadRepository.AddAsync(mortalidad);
}
```

---

## 📊 MAPA DE DEPENDENCIAS DE SERVICIOS

```
InventarioAlimentoController
	↓ (usa)
IInventarioAlimentoService
	↓ (usa)
IInventarioAlimentoRepository

MovimientoAlimentoController
	↓ (usa)
IMovimientoAlimentoService
	├─ IMovimientoAlimentoRepository
	└─ IInventarioAlimentoRepository

ReproduccionController
	↓ (usa)
IReproduccionService
	├─ IReproduccionRepository
	├─ ICuyRepository
	└─ IPartoRepository

PartoController
	↓ (usa)
IPartoService
	├─ IPartoRepository
	├─ IReproduccionRepository
	└─ ICuyRepository

CuyController
	↓ (usa)
ICuyService (NEW o MODIFY)
	├─ ICuyRepository
	├─ IMortalidadRepository
	└─ IJaulaRepository
```

---

## ✅ ORDEN DE IMPLEMENTACIÓN

### FASE 1: Base de Datos (Día 1)
1. ✅ Agregar `CuyHembraId` a tabla `Reproduccion`
2. ✅ Crear migración EF Core
3. ✅ Ejecutar migración

### FASE 2: Servicios de Aplicación (Días 2-3)
1. ✅ Crear `IInventarioAlimentoService` + implementación
2. ✅ Crear `IMovimientoAlimentoService` + implementación
3. ✅ Crear `IReproduccionService` + implementación
4. ✅ Crear `IPartoService` + implementación
5. ✅ Crear/Modificar `IMortalidadService` + implementación

### FASE 3: Controladores (Días 4-5)
1. ✅ Refactorizar `InventarioAlimentoController`
2. ✅ Refactorizar `MovimientoAlimentoController`
3. ✅ Refactorizar `ReproduccionController`
4. ✅ Refactorizar `PartoController`
5. ✅ Crear o refactorizar `MortalidadController`

### FASE 4: Registro de Servicios (Día 5)
1. ✅ Registrar en `Program.cs`:
   - `IInventarioAlimentoService`
   - `IMovimientoAlimentoService`
   - `IReproduccionService`
   - `IPartoService`
   - `IMortalidadService`

### FASE 5: Testing (Día 6)
1. ✅ Pruebas unitarias de servicios
2. ✅ Pruebas de integración
3. ✅ Pruebas de casos concurrentes (Movimiento)

---

## 🧪 CASOS DE PRUEBA CRÍTICOS

### Inventario de Alimentos
```gherkin
Scenario: Crear inventario válido
  Given: Sistema está conectado
  When: Se crea inventario con tipo="Forraje", cantidad=100
  Then: Inventario se crea exitosamente

Scenario: No permitir cantidad negativa
  Given: Formulario de crear inventario
  When: Se intenta crear con cantidad=-50
  Then: Sistema rechaza y muestra error

Scenario: No permitir tipo duplicado
  Given: Ya existe inventario tipo="Grano"
  When: Se intenta crear otro con tipo="Grano"
  Then: Sistema rechaza con error de duplicidad
```

### Movimiento de Alimento (Concurrencia)
```gherkin
Scenario: Salida de alimento válida
  Given: Inventario tiene 50 kg
  When: Se registra salida de 20 kg
  Then: Stock actualiza a 30 kg

Scenario: Rechazar salida mayor que stock
  Given: Inventario tiene 30 kg
  When: Se intenta salida de 50 kg
  Then: Sistema rechaza con "Stock insuficiente"

Scenario: Operaciones concurrentes
  Given: Inventario inicial = 100 kg
  When: Thread 1 resta 60 AND Thread 2 resta 50
  Then: Sistema rechaza uno (stock insuficiente)
```

### Reproducción
```gherkin
Scenario: Validar sexos opuestos
  Given: Macho (ID=1, SexoId=1) y Hembra (ID=2, SexoId=2)
  When: Se crea reproducción
  Then: Éxitosa, hembra pasa a GESTANTE

Scenario: No permitir mismo sexo
  Given: Macho (ID=1) y Macho (ID=3)
  When: Se intenta crear reproducción
  Then: Sistema rechaza "Sexo opuesto requerido"

Scenario: No permitir hembra ya gestante
  Given: Hembra (ID=2) con reproducción activa
  When: Se intenta crear otra reproducción
  Then: Sistema rechaza "Reproducción activa"

Scenario: Validar que no sean el mismo
  Given: Intento de reproducción del mismo ID
  When: CuyMachoId = CuyHembraId
  Then: Sistema rechaza "No consigo mismo"
```

### Parto y Actualización de Estados
```gherkin
Scenario: Parto actualiza estado automáticamente
  Given: Hembra en estado GESTANTE
  When: Se registra parto con 3 crías vivas
  Then: 
	- Parto se registra
	- Hembra pasa a LACTANTE
	- Reproducción marcada como Exitosa

Scenario: No permitir parto de hembra no gestante
  Given: Hembra en estado REPRODUCTOR
  When: Se intenta registrar parto
  Then: Sistema rechaza "Debe estar gestante"

Scenario: No permitir cambiar a no exitosa si hay partos
  Given: Reproducción con 2 partos registrados
  When: Se intenta marcar como no exitosa
  Then: Sistema rechaza "Hay partos registrados"
```

---

## 📝 RESUMEN DE ARCHIVOS A CREAR/MODIFICAR

| Archivo | Acción | Prioridad |
|---------|--------|-----------|
| `IInventarioAlimentoService.cs` | CREAR | 🔴 CRÍTICA |
| `InventarioAlimentoService.cs` | CREAR | 🔴 CRÍTICA |
| `IMovimientoAlimentoService.cs` | CREAR | 🔴 CRÍTICA |
| `MovimientoAlimentoService.cs` | CREAR | 🔴 CRÍTICA |
| `IReproduccionService.cs` | CREAR | 🔴 CRÍTICA |
| `ReproduccionService.cs` | CREAR | 🔴 CRÍTICA |
| `IPartoService.cs` | CREAR | 🔴 CRÍTICA |
| `PartoService.cs` | CREAR | 🔴 CRÍTICA |
| `IMortalidadService.cs` | CREAR/MODIFY | 🟠 ALTA |
| `MortalidadService.cs` | CREAR/MODIFY | 🟠 ALTA |
| `Reproduccion.cs` | MODIFICAR | 🔴 CRÍTICA |
| `Cuy.cs` | MODIFICAR | 🔴 CRÍTICA |
| `ApplicationDbContext.cs` | MODIFICAR | 🔴 CRÍTICA |
| `MovimientoAlimentoController.cs` | REFACTORIZAR | 🔴 CRÍTICA |
| `ReproduccionController.cs` | REFACTORIZAR | 🟠 ALTA |
| `PartoController.cs` | REFACTORIZAR | 🟠 ALTA |
| `Program.cs` | MODIFICAR | 🔴 CRÍTICA |
| `AUDIT_TECNICO_INTEGRAL.md` | ACTUALIZAR | 🟢 BAJA |

---

## ⏱️ TIEMPO ESTIMADO

- **FASE 1 (BD)**: 1 hora
- **FASE 2 (Servicios)**: 8-10 horas
- **FASE 3 (Controladores)**: 4-5 horas
- **FASE 4 (Registro)**: 1 hora
- **FASE 5 (Testing)**: 6-8 horas

**Total**: 20-25 horas

---

## ✨ BENEFICIOS ESPERADOS

✅ **Arquitectura**: Clean Architecture correctamente implementada
✅ **Testabilidad**: Código testeable con inyección de dependencias
✅ **Mantenibilidad**: Código limpio y organizado por capas
✅ **Confiabilidad**: Transacciones atómicas, validaciones completas
✅ **Trazabilidad**: Logging de operaciones críticas
✅ **Escalabilidad**: Soporta múltiples usuarios concurrentes
✅ **Reportes**: Datos consistentes para análisis

---

**Documento generado**: 2024
**Versión**: 1.0
