# Manual Técnico

## Requisitos previos
- .NET 10 SDK
- SQL Server 2019+
- Visual Studio 2022+ (recomendado)

## Instalación

1. Clonar repositorio: 
   ```
   git clone https://github.com/Issirc20/CuyControl
   ```

2. Restaurar dependencias:
   ```
   dotnet restore
   ```

3. Configurar base de datos en appsettings.Development.json

4. Aplicar migraciones:
   ```
   dotnet ef database update
   ```

5. Ejecutar:
   ```
   dotnet run
   ```

## Estructura del Proyecto

- CuyControl.Domain: Entidades y interfaces del dominio.
- CuyControl.Application: Servicios, DTOs y validadores.
- CuyControl.Infrastructure: Repositorios y persistencia.
- CuyControl: Proyecto web (Controllers, Views, ViewModels).
- CuyControl.Tests: Pruebas unitarias e integración.

## Configuración de User Secrets

```
dotnet user-secrets init
dotnet user-secrets set "Seed:Admin:UserName" "admin"
dotnet user-secrets set "Seed:Admin:Password" "Admin123!"
```

## Migraciones de Base de Datos

Crear nueva migración:
```
dotnet ef migrations add NombreMigracion
```

Aplicar:
```
dotnet ef database update
```

