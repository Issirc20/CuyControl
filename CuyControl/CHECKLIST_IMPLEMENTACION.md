# CHECKLIST DE IMPLEMENTACIÓN - INVENTARIO Y REPRODUCCIÓN

## 📋 FASE 1: PREPARACIÓN (Día 0 - 1 hora)

- [ ] Crear rama de feature: `feature/inventory-reproduction-refactor`
- [ ] Crear carpeta: `CuyControl.Application/Interfaces/Services`
- [ ] Crear carpeta: `CuyControl.Application/Services`
- [ ] Revisar archivos existentes:
  - [ ] `Reproduccion.cs` - Verificar estructura actual
  - [ ] `Cuy.cs` - Revisar entidades navegables
  - [ ] `ApplicationDbContext.cs` - Revisar configuraciones

---

## 🗄️ FASE 2: CAMBIOS EN BASE DE DATOS (Día 1 - 1 hora)

### 2.1 Entidad Reproduccion

- [ ] Abrir: `CuyControl.Domain/Entities/Reproduccion.cs`
- [ ] **AGREGAR** después de `CuyMachoId`:
```csharp
public int CuyHembraId { get; set; }  // ✅ NUEVO
```
- [ ] **AGREGAR** después de `public virtual Cuy? CuyMacho`:
```csharp
public virtual Cuy? CuyHembra { get; set; }  // ✅ NUEVO
```

### 2.2 Entidad Cuy

- [ ] Abrir: `CuyControl.Domain/Entities/Cuy.cs`
- [ ] **AGREGAR** nuevas propiedades navegables:
```csharp
public virtual ICollection<Reproduccion> ReproduccionesComoMacho { get; set; } 
	= new List<Reproduccion>();
public virtual ICollection<Reproduccion> ReproduccionesComoHembra { get; set; } 
	= new List<Reproduccion>();
```

### 2.3 ApplicationDbContext

- [ ] Abrir: `CuyControl.Infrastructure/Data/ApplicationDbContext.cs`
- [ ] **AGREGAR** configuración de relación en `OnModelCreating`:
```csharp
// Relación Reproduccion -> CuyHembra
modelBuilder.Entity<Reproduccion>()
	.HasOne(r => r.CuyHembra)
	.WithMany(c => c.ReproduccionesComoHembra)
	.HasForeignKey(r => r.CuyHembraId)
	.OnDelete(DeleteBehavior.Restrict)
	.HasConstraintName("FK_Reproduccion_CuyHembra");
```

### 2.4 Crear Migración EF Core

- [ ] En Package Manager Console:
```powershell
Add-Migration AddCuyHembraToReproduccion
```
- [ ] Verificar que la migración se generó correctamente
- [ ] Aplicar migración:
```powershell
Update-Database
```
- [ ] ✅ Confirmación: BD actualizada con nueva columna `CuyHembraId`

---

## 📚 FASE 3: CREAR SERVICIOS (Día 2-3 - 8 horas)

### 3.1 IInventarioAlimentoService

- [ ] **CREAR** archivo: `CuyControl.Application/Interfaces/Services/IInventarioAlimentoService.cs`
  - [ ] Usar código de ejemplo en `EJEMPLOS_CODIGO.md`
  - [ ] Verificar métodos:
	- [ ] `CrearInventarioAsync()`
	- [ ] `ActualizarInventarioAsync()`
	- [ ] `ObtenerTodosAsync()`
	- [ ] `ObtenerPorIdAsync()`
	- [ ] `EliminarAsync()`

### 3.2 InventarioAlimentoService (Implementación)

- [ ] **CREAR** archivo: `CuyControl.Application/Services/InventarioAlimentoService.cs`
  - [ ] Implementar validaciones:
	- [ ] Tipo no vacío
	- [ ] Cantidad >= 0
	- [ ] No duplicar tipo
  - [ ] Implementar logging
  - [ ] Implementar manejo de excepciones
  - [ ] Compilar y verificar sin errores

### 3.3 IMovimientoAlimentoService

- [ ] **CREAR** archivo: `CuyControl.Application/Interfaces/Services/IMovimientoAlimentoService.cs`
  - [ ] Métodos:
	- [ ] `RegistrarMovimientoAsync()`
	- [ ] `ObtenerMovimientosAsync()`
	- [ ] `ObtenerStockActualAsync()`
	- [ ] `ObtenerMovimientosEnRangoAsync()`

### 3.4 MovimientoAlimentoService (Implementación)

- [ ] **CREAR** archivo: `CuyControl.Application/Services/MovimientoAlimentoService.cs`
  - [ ] Implementar validaciones:
	- [ ] ✅ Cantidad > 0
	- [ ] ✅ Para SALIDA: validar stock
	- [ ] ✅ Lock de escritura (en repositorio)
	- [ ] ✅ Transacción atómica
  - [ ] Implementar logging detallado
  - [ ] Compilar sin errores

### 3.5 IReproduccionService

- [ ] **CREAR** archivo: `CuyControl.Application/Interfaces/Services/IReproduccionService.cs`
  - [ ] Métodos:
	- [ ] `CrearReproduccionAsync()`
	- [ ] `ActualizarReproduccionAsync()`
	- [ ] `ObtenerPorIdAsync()`
	- [ ] `ObtenerReproduccionesActivasAsync()`
	- [ ] `ObtenerHistorialHembraAsync()`
	- [ ] `TienePartosAsync()`

### 3.6 ReproduccionService (Implementación)

- [ ] **CREAR** archivo: `CuyControl.Application/Services/ReproduccionService.cs`
  - [ ] Implementar validaciones en `CrearReproduccionAsync()`:
	- [ ] ✅ Ambos cuyes existen
	- [ ] ✅ Sexo opuesto
	- [ ] ✅ Macho es macho (SexoId=1)
	- [ ] ✅ Hembra es hembra (SexoId=2)
	- [ ] ✅ No mismo ID
	- [ ] ✅ No vendidos/muertos
	- [ ] ✅ Hembra sin reproducción activa
	- [ ] ✅ Cambiar estado hembra a GESTANTE
  - [ ] Implementar validación en `ActualizarReproduccionAsync()`:
	- [ ] ✅ CRÍTICO: Si hay partos → DEBE ser exitosa
  - [ ] Compilar sin errores

### 3.7 IPartoService

- [ ] **CREAR** archivo: `CuyControl.Application/Interfaces/Services/IPartoService.cs`
  - [ ] Métodos:
	- [ ] `CrearPartoAsync()`
	- [ ] `ObtenerPartosReproduccionAsync()`
	- [ ] `ContarCuyesNacidosAsync()`

### 3.8 PartoService (Implementación)

- [ ] **CREAR** archivo: `CuyControl.Application/Services/PartoService.cs`
  - [ ] En `CrearPartoAsync()`:
	- [ ] ✅ Validar hembra está en GESTANTE
	- [ ] ✅ Registrar parto
	- [ ] ✅ AUTO-ACTUALIZAR: Hembra → LACTANTE
	- [ ] ✅ AUTO-ACTUALIZAR: Reproducción.Exitosa = true
	- [ ] ✅ Logging detallado
  - [ ] Compilar sin errores

### 3.9 IMortalidadService (si no existe)

- [ ] **CREAR o REVISAR** archivo: `CuyControl.Application/Interfaces/Services/IMortalidadService.cs`
  - [ ] Métodos:
	- [ ] `CrearMortalidadAsync()`
	- [ ] `ObtenerMortalidadCuyAsync()`

### 3.10 MortalidadService (Implementación)

- [ ] **CREAR o REVISAR** archivo: `CuyControl.Application/Services/MortalidadService.cs`
  - [ ] Implementar creación automática cuando cuy se marca como muerto
  - [ ] Compilar sin errores

---

## 🔗 FASE 4: MODIFICAR CONTROLADORES (Día 4-5 - 4-5 horas)

### 4.1 InventarioAlimentoController

- [ ] Abrir: `CuyControl/Controllers/InventarioAlimentoController.cs`
- [ ] **ELIMINAR**: Inyección de `ApplicationDbContext`
- [ ] **AGREGAR**: Inyección de `IInventarioAlimentoService`
- [ ] Refactorizar métodos:
  - [ ] `Index()` - usar servicio
  - [ ] `Create()` - usar servicio con try-catch
  - [ ] `Edit()` - usar servicio
  - [ ] `Delete()` - usar servicio
- [ ] Reemplazar accesos directos a BD con llamadas a servicio
- [ ] Añadir manejo de excepciones consistente
- [ ] Compilar sin errores

### 4.2 MovimientoAlimentoController

- [ ] Abrir: `CuyControl/Controllers/MovimientoAlimentoController.cs`
- [ ] **ELIMINAR**: Acceso directo a `ApplicationDbContext`
- [ ] **AGREGAR**: Inyección de `IMovimientoAlimentoService`
- [ ] Refactorizar métodos:
  - [ ] `Create()` - **CRÍTICO**: Usar servicio que valida stock
  - [ ] `Index()` - usar servicio
- [ ] Asegurar que el formulario mapea correctamente a DTO
- [ ] Compilar sin errores

### 4.3 ReproduccionController

- [ ] Abrir: `CuyControl/Controllers/ReproduccionController.cs` (crear si no existe)
- [ ] **ELIMINAR**: Acceso directo a BD
- [ ] **AGREGAR**: Inyección de `IReproduccionService`
- [ ] Refactorizar:
  - [ ] `Create()` - usar servicio con validaciones
  - [ ] `Edit()` - usar servicio (validar reproducción exitosa)
  - [ ] `Index()` - usar servicio
- [ ] Compilar sin errores

### 4.4 PartoController

- [ ] Abrir: `CuyControl/Controllers/PartoController.cs` (crear si no existe)
- [ ] **ELIMINAR**: Acceso directo a BD
- [ ] **AGREGAR**: Inyección de `IPartoService`
- [ ] Refactorizar:
  - [ ] `Create()` - usar servicio (auto-actualiza estados)
  - [ ] `Index()` - usar servicio
- [ ] Compilar sin errores

### 4.5 MortalidadController

- [ ] Abrir: `CuyControl/Controllers/MortalidadController.cs`
- [ ] **AGREGAR**: Inyección de `IMortalidadService`
- [ ] Refactorizar si es necesario
- [ ] Compilar sin errores

---

## 🔧 FASE 5: REGISTRAR SERVICIOS EN PROGRAM.CS (Día 5 - 1 hora)

- [ ] Abrir: `CuyControl/Program.cs`
- [ ] **BUSCAR**: Sección `services.AddScoped` u otra inyección de dependencias

- [ ] **AGREGAR** (antes de `app.Run()`):
```csharp
// Servicios de aplicación - Inventario y Reproducción
services.AddScoped<IInventarioAlimentoService, InventarioAlimentoService>();
services.AddScoped<IMovimientoAlimentoService, MovimientoAlimentoService>();
services.AddScoped<IReproduccionService, ReproduccionService>();
services.AddScoped<IPartoService, PartoService>();
services.AddScoped<IMortalidadService, MortalidadService>();
```

- [ ] **VERIFICAR** que ya están registrados los repositorios:
  - [ ] `IInventarioAlimentoRepository`
  - [ ] `IMovimientoAlimentoRepository`
  - [ ] `IReproduccionRepository`
  - [ ] `IPartoRepository`
  - [ ] `IMortalidadRepository`
  - [ ] `ICuyRepository`

- [ ] Si faltan repositorios, AGREGAR:
```csharp
services.AddScoped<IInventarioAlimentoRepository, InventarioAlimentoRepository>();
services.AddScoped<IMovimientoAlimentoRepository, MovimientoAlimentoRepository>();
services.AddScoped<IReproduccionRepository, ReproduccionRepository>();
services.AddScoped<IPartoRepository, PartoRepository>();
services.AddScoped<IMortalidadRepository, MortalidadRepository>();
```

- [ ] Compilar proyecto completo
- [ ] **Verificar**: Sin errores de compilación

---

## ✅ FASE 6: TESTING (Día 6 - 6-8 horas)

### 6.1 Build y Compilación

- [ ] Limpiar solución: `Clean Solution`
- [ ] Compilar: `Build Solution`
- [ ] ✅ **Resultado esperado**: Build exitosa, cero errores

### 6.2 Pruebas Manuales - Inventario

- [ ] Ejecutar aplicación: `F5` o `Ctrl+F5`
- [ ] Navegar a: Inventario de Alimento
- [ ] **Test 1**: Crear inventario válido
  - [ ] Tipo: "Forraje"
  - [ ] Cantidad: 100
  - [ ] ✅ Esperado: Se crea exitosamente
- [ ] **Test 2**: Intenta crear inventario con cantidad negativa
  - [ ] Cantidad: -50
  - [ ] ✅ Esperado: Rechaza con error
- [ ] **Test 3**: Intenta crear inventario con tipo duplicado
  - [ ] Tipo: "Forraje" (ya existe)
  - [ ] ✅ Esperado: Rechaza con error

### 6.3 Pruebas Manuales - Movimiento

- [ ] Navegar a: Movimiento de Alimento
- [ ] **Test 4**: Entrada de alimento
  - [ ] Inventario: Forraje
  - [ ] Tipo: Entrada
  - [ ] Cantidad: 50
  - [ ] ✅ Esperado: Se registra, stock aumenta
- [ ] **Test 5**: Salida de alimento (stock suficiente)
  - [ ] Inventario: Forraje
  - [ ] Tipo: Salida
  - [ ] Cantidad: 30
  - [ ] ✅ Esperado: Se registra, stock disminuye
- [ ] **Test 6**: Salida de alimento (stock insuficiente)
  - [ ] Stock actual: 20
  - [ ] Cantidad: 50
  - [ ] ✅ Esperado: Rechaza con "Stock insuficiente"

### 6.4 Pruebas Manuales - Reproducción

- [ ] Navegar a: Reproducción (crear si no existe)
- [ ] Crear 2 cuyes (1 macho, 1 hembra) si no existen
- [ ] **Test 7**: Crear reproducción válida
  - [ ] Macho: ID válido
  - [ ] Hembra: ID válido
  - [ ] ✅ Esperado: Se crea, hembra pasa a GESTANTE
- [ ] **Test 8**: Intenta crear reproducción con mismo sexo
  - [ ] Macho: ID1 (Sexo Macho)
  - [ ] Hembra: ID3 (Sexo Macho)
  - [ ] ✅ Esperado: Rechaza con "Sexo opuesto requerido"
- [ ] **Test 9**: Intenta reproducir hembra ya gestante
  - [ ] Hembra ID: ya con reproducción activa
  - [ ] ✅ Esperado: Rechaza con "Reproducción activa"

### 6.5 Pruebas Manuales - Parto

- [ ] Navegar a: Partos
- [ ] **Test 10**: Registrar parto
  - [ ] Reproducción: la creada en Test 7
  - [ ] Crías vivas: 3
  - [ ] Crías muertas: 1
  - [ ] ✅ Esperado:
	- [ ] Se registra parto
	- [ ] Hembra pasa a LACTANTE (verificar en detalle de cuy)
	- [ ] Reproducción marcada como Exitosa
- [ ] **Test 11**: Intenta cambiar reproducción a no exitosa
  - [ ] Reproducción con partos registrados
  - [ ] Marcar: Exitosa = false
  - [ ] ✅ Esperado: Rechaza con "Hay partos registrados"

### 6.6 Pruebas de Logs

- [ ] Abrir Output Window: `View → Output`
- [ ] Ejecutar Test 4 (Entrada de alimento)
- [ ] ✅ Esperado: Mensaje de log:
  ```
  "Entrada de alimento registrada. Tipo: Forraje, Cantidad: 50, Stock total: 150"
  ```
- [ ] Ejecutar Test 10 (Registrar parto)
- [ ] ✅ Esperado: Mensaje de log:
  ```
  "Parto registrado exitosamente. Reproducción X, Crías vivas 3, Hembra Y actualizada a LACTANTE"
  ```

### 6.7 Revisión de BD

- [ ] Abrir SQL Server Management Studio
- [ ] Conectar a base de datos
- [ ] **Verificar tabla `Reproducciones`**:
  - [ ] ✅ Existe columna `CuyHembraId`
  - [ ] ✅ FK correctamente configurado
  - [ ] ✅ Datos de prueba presentes
- [ ] **Verificar tabla `Partos`**:
  - [ ] ✅ Registro creado
  - [ ] ✅ FK a Reproduccion correcta

### 6.8 Pruebas Unitarias (Opcional pero recomendado)

- [ ] **CREAR** proyecto de tests si no existe:
  ```bash
  dotnet new xunit -n CuyControl.Tests
  dotnet add reference CuyControl.Application
  ```

- [ ] **CREAR** archivo de tests:
  ```
  CuyControl.Tests/Services/MovimientoAlimentoServiceTests.cs
  ```

- [ ] **ESCRIBIR** tests para validación de stock:
  ```csharp
  [Fact]
  public async Task RegistrarMovimiento_ConStockInsuficiente_ThrowsException()
  {
	  // Arrange
	  var movimientoDto = new MovimientoAlimentoDto
	  {
		  InventarioAlimentoId = 1,
		  Cantidad = 100,
		  TipoMovimiento = TipoMovimientoAlimentoEnum.Salida
	  };

	  // Act & Assert
	  await Assert.ThrowsAsync<InvalidOperationException>(
		  () => _service.RegistrarMovimientoAsync(movimientoDto));
  }
  ```

- [ ] Ejecutar tests: `Test → Run All Tests` o `Ctrl+R, Ctrl+A`
- [ ] ✅ Todos los tests deben pasar

---

## 📊 FASE 7: VALIDACIÓN Y DOCUMENTACIÓN (Día 7 - 2 horas)

- [ ] Revisar código:
  - [ ] ✅ Sin código duplicado
  - [ ] ✅ Sin números mágicos
  - [ ] ✅ Logging consistente
  - [ ] ✅ Manejo de excepciones uniforme

- [ ] Documentación:
  - [ ] ✅ Comentarios en métodos críticos
  - [ ] ✅ XML docs en interfaces públicas
  - [ ] ✅ README actualizado si aplica

- [ ] Performance:
  - [ ] ✅ Sin N+1 queries
  - [ ] ✅ Índices presentes en BD
  - [ ] ✅ Transacciones eficientes

- [ ] Seguridad:
  - [ ] ✅ Validación de entrada en todos los servicios
  - [ ] ✅ Controladores con `[Authorize]`
  - [ ] ✅ Logging de operaciones críticas

---

## 🎯 FASE 8: MERGE Y DEPLOY (Día 7-8)

- [ ] Crear Pull Request con descripción detallada
- [ ] Incluir archivos:
  - [ ] `CORRECCIONES_INVENTARIO_REPRODUCCION.md`
  - [ ] `DIAGRAMA_ARQUITECTURA.md`
  - [ ] `EJEMPLOS_CODIGO.md`

- [ ] Code Review:
  - [ ] ✅ 2+ aprobaciones
  - [ ] ✅ Todos los comentarios resueltos

- [ ] Merge a rama principal:
  - [ ] ✅ Build pipeline pasa
  - [ ] ✅ Tests pasan

- [ ] Deploy a staging/producción:
  - [ ] ✅ Ejecutar migrations
  - [ ] ✅ Verificar funcionalidad en ambiente destino

---

## 🚨 ROLLBACK PLAN

Si algo falla:

1. **En BD**:
   ```powershell
   Update-Database -Migration Previous
   ```

2. **En código**:
   ```bash
   git revert <commit-hash>
   ```

3. **Reportar**: Documentar qué falló y contactar al equipo

---

## ✨ CHECKLIST FINAL

- [ ] ✅ Código compilable sin errores
- [ ] ✅ Todos los tests pasan
- [ ] ✅ Pruebas manuales exitosas
- [ ] ✅ Logging configurado
- [ ] ✅ BD actualizada con migración
- [ ] ✅ Servicios registrados en DI
- [ ] ✅ Controladores refactorizados
- [ ] ✅ Documentación completada
- [ ] ✅ PR creado y aprobado
- [ ] ✅ Listo para deploy

---

**Documento**: Checklist de Implementación
**Versión**: 1.0
**Fecha**: 2024
**Estimado Total**: 25-30 horas de trabajo
