# DIAGRAMA DE ARQUITECTURA - INVENTARIO Y REPRODUCCIÓN

## 🏗️ ARQUITECTURA ACTUAL (INCORRECTA)

```
PRESENTACIÓN
	↓
InventarioAlimentoController ──→ ❌ ApplicationDbContext (VIOLACIÓN!)
MovimientoAlimentoController ──→ ❌ ApplicationDbContext (VIOLACIÓN!)
ReproduccionController ────────→ ❌ ApplicationDbContext (VIOLACIÓN!)
PartoController ───────────────→ ❌ ApplicationDbContext (VIOLACIÓN!)
```

**Problemas**:
- Controllers accediendo directamente a la BD
- No hay validaciones
- Código duplicado
- Difícil de testear
- Sin separación de responsabilidades

---

## 🏗️ ARQUITECTURA CORRECTA (PROPUESTA)

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTACIÓN                            │
│  (Razor Pages / Views)                                       │
└────────────┬─────────────────┬──────────────────┬───────────┘
			 │                 │                  │
┌────────────▼──┐  ┌───────────▼──┐  ┌──────────▼──────┐
│  CONTROLADORES │  │ (Otros)      │  │ (Otros)         │
├────────────────┤  └──────────────┘  └─────────────────┘
│ Inventario*    │
│ Movimiento*    │
│ Reproducción*  │
│ Parto*         │
│ Mortalidad*    │
└────────────┬───┘
			 │ (inyecta)
┌────────────▼──────────────────────────────────────────────┐
│             CAPA DE APLICACIÓN (Services)                  │
├────────────────────────────────────────────────────────────┤
│                                                             │
│  ┌──────────────────────┐  ┌──────────────────────┐       │
│  │ IInventarioService   │  │ IMovimientoService   │       │
│  │                      │  │                      │       │
│  │ • Crear()            │  │ • Registrar()        │       │
│  │ • Actualizar()       │  │ • Obtener()          │       │
│  │ • Validar tipos()    │  │ • Stock Actual()     │       │
│  │ • Validar cantidad() │  │ • Con Lock BD        │       │
│  │ • Listar()           │  │ • Con Transacciones  │       │
│  └──────────────────────┘  └──────────────────────┘       │
│                                                             │
│  ┌──────────────────────┐  ┌──────────────────────┐       │
│  │ IReproduccionService │  │ IPartoService        │       │
│  │                      │  │                      │       │
│  │ • Crear()            │  │ • Crear()            │       │
│  │ • Validar sexos()    │  │ • Actualizar Estados │       │
│  │ • Validar no venta() │  │ • Auto LACTANTE      │       │
│  │ • Historial()        │  │ • Contar nacidos()   │       │
│  │ • Auto GESTANTE      │  │ • Validar gestante   │       │
│  └──────────────────────┘  └──────────────────────┘       │
│                                                             │
│  ┌──────────────────────────────────────────────┐          │
│  │ IMortalidadService                           │          │
│  │                                              │          │
│  │ • Crear()                                    │          │
│  │ • Auto registrar cuando EstadoId = 7        │          │
│  │ • Listar()                                   │          │
│  └──────────────────────────────────────────────┘          │
│                                                             │
└────────────┬─────────────────────────────────────┬─────────┘
			 │ (usa)                              │ (usa)
┌────────────▼─────────────────────────────────────▼─────────┐
│        CAPA DE INFRAESTRUCTURA (Repositories)               │
├────────────────────────────────────────────────────────────┤
│                                                             │
│ • IInventarioAlimentoRepository                            │
│ • IMovimientoAlimentoRepository                            │
│ • IReproduccionRepository                                  │
│ • IPartoRepository                                         │
│ • IMortalidadRepository                                    │
│ • ICuyRepository                                           │
│                                                             │
└────────────┬────────────────────────────────────────────────┘
			 │ (EF Core)
┌────────────▼────────────────────────────────────────────────┐
│            BASE DE DATOS (SQL Server)                       │
├────────────────────────────────────────────────────────────┤
│                                                             │
│ • InventariosAlimento                                      │
│ • MovimientosAlimento                                      │
│ • Reproducciones (CON CuyHembraId)                         │
│ • Partos                                                   │
│ • Mortalidades                                             │
│ • Cuyes                                                    │
│                                                             │
└────────────────────────────────────────────────────────────┘

Legend: * = Requiere refactorización
```

---

## 📊 DIAGRAMA DE RELACIONES (Base de Datos)

### ANTES (Incorrecto)
```
Reproduccion
  ├── ReproduccionId (PK)
  ├── CuyMachoId (FK) ──→ Cuy
  ├── FechaCruzamiento
  ├── Exitosa
  └── Partos (1:N) ──→ Parto
						├── PartoId (PK)
						├── ReproduccionId (FK)
						├── FechaParto
						└── NumeroDeCreasVivas

❌ FALTA: CuyHembraId
❌ NO HAY: Relación bidireccional con Cuy (hembra)
❌ PROBLEMA: Validación de género imposible
```

### DESPUÉS (Correcto)
```
Cuy
  ├── CuyId (PK)
  ├── Codigo
  ├── FechaNacimiento
  ├── SexoId (1=Macho, 2=Hembra)
  ├── EstadoId
  ├── JaulaId (FK) ──→ Jaula
  ├── ReproduccionesComoMacho (0:N) ──→ Reproduccion [inversa]
  └── ReproduccionesComoHembra (0:N) ──→ Reproduccion [inversa] ✅ NUEVA

Reproduccion
  ├── ReproduccionId (PK)
  ├── CuyMachoId (FK) ──→ Cuy (es Macho)  ✅
  ├── CuyHembraId (FK) ──→ Cuy (es Hembra) ✅ NUEVO
  ├── FechaCruzamiento
  ├── Exitosa
  ├── FechaCreacion
  ├── UsuarioCreacionId
  └── Partos (1:N) ──→ Parto

Parto
  ├── PartoId (PK)
  ├── ReproduccionId (FK) ──→ Reproduccion ✅
  ├── FechaParto
  ├── NumeroDeCreasVivas
  ├── NumeroDeCreasNatimuertas
  ├── FechaCreacion
  └── UsuarioCreacionId

Mortalidad
  ├── MortalidadId (PK)
  ├── CuyId (FK) ──→ Cuy ✅
  ├── FechaDefuncion
  ├── Causa
  ├── FechaCreacion
  └── UsuarioCreacionId

InventarioAlimento
  ├── InventarioAlimentoId (PK)
  ├── TipoAlimento (UNIQUE)
  ├── CantidadActual
  ├── FechaCreacion
  └── FechaModificacion

MovimientoAlimento
  ├── MovimientoAlimentoId (PK)
  ├── InventarioAlimentoId (FK) ──→ InventarioAlimento
  ├── TipoMovimiento (1=Entrada, 2=Salida)
  ├── Cantidad
  ├── FechaMovimiento
  ├── FechaCreacion
  └── UsuarioCreacionId
```

---

## 🔄 FLUJOS DE DATOS

### Flujo 1: Crear Inventario de Alimento

```
Formulario → Controller → IInventarioAlimentoService
							↓
						Validar datos:
						• TipoAlimento no vacío
						• CantidadActual >= 0
						• No existe tipo duplicado
							↓
						Mapear a Entidad
							↓
						IInventarioAlimentoRepository.AddAsync()
							↓
						SaveChangesAsync()
							↓
						Log: "Inventario creado: ID X"
							↓
						Mapear a DTO
							↓
						Controller → View ("Éxito")
```

### Flujo 2: Registrar Movimiento de Alimento (Crítico)

```
Formulario → Controller → IMovimientoAlimentoService
							↓
						ValidarMovimientoAsync():
						• Cantidad > 0
						• Inventario existe
						• Tipo válido (Entrada/Salida)
							↓
					┌───YES────────────────┐
					↓                      ↓
			  [ES SALIDA]           [ES ENTRADA]
					│                      │
			Validar: Stock >= Cantidad    │
			(SI NO → Excepción)           │
					│                      │
			┌───────┴─────────────────────┘
			↓
		ACTUALIZAR STOCK:
		• Con LOCK de escritura
		• En TRANSACCIÓN ATÓMICA
		• Stock = Stock ± Cantidad
			↓
		Registrar MovimientoAlimento
			↓
		SaveChangesAsync()
			↓
		Log detallado: "Salida: Tipo, Qty, Stock restante"
			↓
		Mapear a DTO
			↓
		Controller → View ("Éxito")

		❌ EXCEPCIÓN → Rollback automático + Log error
```

### Flujo 3: Crear Reproducción

```
Formulario → Controller → IReproduccionService
							↓
					ValidarCrearReproduccion():
					✓ Ambos cuyes existen
					✓ Sexo opuesto
					✓ Macho = SexoId 1
					✓ Hembra = SexoId 2
					✓ No mismo ID
					✓ No vendidos/muertos
					✓ Hembra sin reproducción activa
							↓
					Crear Reproduccion (Exitosa=false)
							↓
					ACTUALIZAR: Cuy(Hembra).EstadoId = 4 (GESTANTE)
							↓
					IReproduccionRepository.AddAsync()
							↓
					ICuyRepository.UpdateAsync()
							↓
					SaveChangesAsync()
							↓
					Log: "Reproducción creada: Macho X x Hembra Y"
							↓
					Controller → View ("Éxito")
```

### Flujo 4: Registrar Parto (Actualización automática de estados)

```
Formulario → Controller → IPartoService
							↓
					ValidarCrearParto():
					✓ Reproducción existe
					✓ Hembra en estado GESTANTE
					✓ Números de crías >= 0
							↓
					Crear Parto
							↓
					ACTUALIZAR automáticamente:
					• Hembra.EstadoId = 5 (LACTANTE) ✅
					• Reproduccion.Exitosa = true ✅
							↓
					IPartoRepository.AddAsync()
							↓
					ICuyRepository.UpdateAsync(Hembra)
							↓
					IReproduccionRepository.UpdateAsync()
							↓
					SaveChangesAsync()
							↓
					Log: "Parto: Repro X, Crías vivas Y, Muertas Z"
							↓
					Controller → View ("Éxito")
```

---

## 🛡️ PROTECCIONES CONTRA PROBLEMAS COMUNES

### Concurrencia en Movimiento de Alimento

```
ANTES (INCORRECTO):
Thread 1: Lee Stock=100
Thread 2: Lee Stock=100
Thread 1: Escribe Stock=100-30=70
Thread 2: Escribe Stock=100-50=50  ❌ INCORRECTO!

DESPUÉS (CORRECTO):
Thread 1: Adquiere LOCK en InventarioAlimento[1]
		 Lee Stock=100
		 Valida 100 >= 30 ✓
		 Escribe Stock=70
		 Libera LOCK

Thread 2: Espera a LOCK
		 Adquiere LOCK
		 Lee Stock=70
		 Valida 70 >= 50 ✓
		 Escribe Stock=20
		 Libera LOCK

Resultado: Stock=20 ✅ CORRECTO!
```

### Integridad de Reproducción

```
ANTES (INCORRECTO):
Usuario A crea reproducción Macho 1 x ? (sin CuyHembraId)
		↓
Sistema permite porque no hay validación
		↓
Usuario B ve reproducción sin saber cuál es la hembra ❌

DESPUÉS (CORRECTO):
Usuario A intenta crear reproducción Macho 1 x Hembra 2
		↓
Sistema VALIDA:
  • Macho existe y es macho (SexoId=1) ✓
  • Hembra existe y es hembra (SexoId=2) ✓
  • No son el mismo ✓
  • No vendidos/muertos ✓
  • Hembra sin reproducción activa ✓
		↓
Reproducción se crea con ambos IDs ✓
Usuario B ve: "Macho 1 x Hembra 2" ✓
Hembra pasa a GESTANTE ✓
```

### Validación Lógica: Reproducción Exitosa

```
ANTES (INCORRECTO):
Usuario crea reproducción
		↓
Se registran 3 partos
		↓
Usuario cambia: Reproducción.Exitosa = false ✓ PERMITIDO ❌
		↓
Dashboard muestra: "3 reproducciones fallidas" ❌ FALSO!

DESPUÉS (CORRECTO):
Usuario crea reproducción
		↓
Se registran 3 partos (cada uno actualiza estado automáticamente)
		↓
Reproducción.Exitosa = true automáticamente
		↓
Usuario intenta cambiar a false
		↓
Sistema: "ERROR: Hay 3 partos registrados, no puede ser no exitosa"
		↓
Datos consistentes ✓
```

---

## 📈 CAMBIOS EN BASE DE DATOS

### Migración EF Core Requerida

```csharp
public partial class AddCuyHembraToReproduccion : Migration
{
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<int>(
			name: "CuyHembraId",
			table: "Reproducciones",
			type: "int",
			nullable: false,
			defaultValue: 0);

		migrationBuilder.CreateIndex(
			name: "IX_Reproducciones_CuyHembraId",
			table: "Reproducciones",
			column: "CuyHembraId");

		migrationBuilder.AddForeignKey(
			name: "FK_Reproducciones_Cuyes_CuyHembraId",
			table: "Reproducciones",
			column: "CuyHembraId",
			principalTable: "Cuyes",
			principalColumn: "CuyId",
			onDelete: ReferentialAction.Restrict);
	}

	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropForeignKey(
			name: "FK_Reproducciones_Cuyes_CuyHembraId",
			table: "Reproducciones");

		migrationBuilder.DropIndex(
			name: "IX_Reproducciones_CuyHembraId",
			table: "Reproducciones");

		migrationBuilder.DropColumn(
			name: "CuyHembraId",
			table: "Reproducciones");
	}
}
```

### SQL Equivalente (Direct SQL)

```sql
-- Agregar columna
ALTER TABLE Reproducciones 
ADD CuyHembraId INT NOT NULL DEFAULT 0;

-- Crear índice
CREATE INDEX IX_Reproducciones_CuyHembraId 
ON Reproducciones(CuyHembraId);

-- Agregar FK
ALTER TABLE Reproducciones
ADD CONSTRAINT FK_Reproducciones_Cuyes_CuyHembraId
FOREIGN KEY (CuyHembraId) REFERENCES Cuyes(CuyId)
ON DELETE NO ACTION;
```

---

**Diagrama generado**: 2024
**Versión**: 1.0
