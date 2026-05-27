# Esquema de Base de Datos

## Tablas Principales

### ApplicationUsers
- Id (PK)
- UserName (nvarchar)
- Email (nvarchar)
- FullName (nvarchar)
- TipoUsuario (int)
- IsActive (bit)
- CreatedAt (datetime)

### Cuyes
- Id (PK)
- Codigo (nvarchar)
- FechaNacimiento (datetime)
- SexoId (int, FK)
- EstadoId (int, FK)
- JaulaId (int, FK)
- PesoActual (decimal)
- Raza (nvarchar)

### Jaulas
- Id (PK)
- Codigo (nvarchar)
- GalponId (int, FK)
- Capacidad (int)

### Galpones
- Id (PK)
- Nombre (nvarchar)
- Ubicacion (nvarchar)

### Alimentacion
- Id (PK)
- JaulaId (int, FK)
- UsuarioId (int, FK)
- TipoAlimento (nvarchar)
- CantidadAlimento (decimal)
- FechaAlimentacion (datetime)

### InventariosAlimento
- Id (PK)
- TipoAlimento (nvarchar)
- CantidadActual (decimal)
- CantidadMinima (decimal)
- CostoUnitario (decimal)
- FechaUltimaReposicion (datetime)

### Enfermedades
- Id (PK)
- CuyId (int, FK)
- Nombre (nvarchar)
- Descripcion (nvarchar)

### Tratamientos
- Id (PK)
- EnfermedadId (int, FK)
- CuyId (int, FK)
- Medicamento (nvarchar)
- Dosis (nvarchar)
- FechaInicio (datetime)
- FechaFin (datetime)

### Reproducciones
- Id (PK)
- CuyMachoId (int, FK)
- CuyHembraId (int, FK)
- Fecha (datetime)
- Exitosa (bit)

### Partos
- Id (PK)
- ReproduccionId (int, FK)
- CuyHembraId (int, FK)
- FechaParto (datetime)
- CantidadCuyes (int)

### Mortalidades
- Id (PK)
- CuyId (int, FK)
- Fecha (datetime)
- Causa (nvarchar)

### Ventas
- Id (PK)
- CuyId (int, FK)
- FechaVenta (datetime)
- PrecioUnitario (decimal)
- Cantidad (int)
- PrecioTotal (decimal)
- NombreComprador (nvarchar)

## Relaciones Principales

- Galpon 1:N Jaula
- Jaula 1:N Cuy
- Cuy 1:N Alimentacion
- Cuy 1:N Enfermedad
- Enfermedad 1:N Tratamiento
- Cuy 1:N Reproduccion
- Reproduccion 1:N Parto

