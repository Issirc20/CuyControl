# CuyControl — Documentación oficial (Versión v1)

Fecha de publicación: 2026-05-25

Versión: v1

Resumen: documento de referencia para desarrolladores que describe la arquitectura, la instalación, el diseño del dominio, las decisiones técnicas, cómo ejecutar el proyecto y la hoja de ruta para futuras versiones.

---

## 1. Resumen ejecutivo

CuyControl es un sistema para la gestión integral de crianza y producción de cuyes. Esta versión v1 implementa la estructura base con Arquitectura Limpia (Clean Architecture) y funcionalidades esenciales: administración de cuyes y jaulas, sistema de identidad y roles, gestión básica de ventas, repositorios EF Core y vistas Razor para la interfáz web.

El objetivo de esta versión es proporcionar una base sólida, fácil de mantener y extensible para añadir módulos de sanidad, reproducción, alimentación, reportes y un dashboard analítico.

---

## 2. Alcance de v1

- Estructura por capas: Domain, Application, Infrastructure, Web.
- Entidades y modelos del dominio principales (Cuy, Jaula, Galpón, Usuario, Venta y relacionados).
- Repositorios con patrones genéricos y específicos para las entidades principales.
- Servicios de aplicación para lógica de negocio (CuyService, VentaService) con validación y mapeo.
- Autenticación y autorización mediante ASP.NET Core Identity con roles predefinidos (Administrador, Operador, Veterinario).
- Vistas Razor y controladores para CRUD de Cuy y Jaula.
- Configuración de DI, DbContext y seed de roles en arranque.

---

## 3. Estructura del repositorio

Raíz del repositorio (seleccionar según solución local):

- /CuyControl.Domain — Entidades, enums, interfaces de dominio.
- /CuyControl.Application — DTOs, servicios, interfaces, validadores, mapeos.
- /CuyControl.Infrastructure — Implementación de EF Core, repositorios, Identity y seeders.
- /CuyControl (Web) — Proyecto web ASP.NET Core con Razor Views/Controllers y Program.cs.
- /tests (no incluido por defecto) — recomendado crear proyecto(s) de pruebas con xUnit.


---

## 4. Tecnologías y dependencias

- Plataforma: .NET 10
- ASP.NET Core (Razor Views / MVC)
- Entity Framework Core (SQL Server provider)
- ASP.NET Core Identity (con claves int para IdentityRole y ApplicationUser)
- Librerías sugeridas (no instaladas por defecto en v1): ClosedXML o EPPlus (exportar Excel), Serilog (logging estructurado), xUnit (tests)

---

## 5. Requisitos previos

- .NET 10 SDK instalado
- SQL Server (local o remoto) con una cadena de conexión válida
- dotnet-ef (opcional, para migraciones)
- Visual Studio 2022/2026 o VS Code

---

## 6. Configuración local y ejecución

1. Clonar el repositorio y abrir la solución en Visual Studio o desde CLI.

2. Configurar connection string en appsettings.json (en el proyecto web):

  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CuyControlDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }

Ajusta Server, User Id y Password según tu entorno.

3. Aplicar migraciones y crear la base de datos (desde la raíz de la solución):

  - Instalar herramientas si hace falta: dotnet tool install --global dotnet-ef
  - Añadir migración inicial (si no existe):
    dotnet ef migrations add InitialCreate --project CuyControl.Infrastructure --startup-project CuyControl
  - Aplicar migración:
    dotnet ef database update --project CuyControl.Infrastructure --startup-project CuyControl

4. Ejecutar la aplicación:

  - Desde Visual Studio: establecer proyecto Web como startup y ejecutar.
  - Desde CLI: dotnet run --project CuyControl

5. En arranque la aplicación crea los roles (Administrador, Operador, Veterinario). Seed de usuarios demo puede estar configurado en IdentitySeeder (revisar y modificar contraseñas en entornos reales).

---

## 7. Diseño del dominio (resumen de entidades principales)

- Cuy
  - Propiedades clave: Id, Codigo, FechaNacimiento, SexoId, EstadoId, JaulaId, PesoActual, Raza, Observaciones, FechaCreacion, Auditing.
  - Relaciones: ControlesPeso, Enfermedades, Reproducciones, Ventas.

- Jaula
  - Propiedades clave: Id, Codigo, Capacidad, GalponId, Disponible, CantidadActual.
  - Relaciones: referencia al Galpón y colección de Cuyes.

- Galpón
  - Propiedades clave: Id, Nombre, Ubicacion, Capacidad.

- Venta
  - Propiedades clave: Id, FechaVenta, CuyId, Cantidad, PrecioUnitario, PrecioTotal, MetodoPago, NombreComprador.

- Usuario (Domain Usuario y ApplicationUser para Identity)
  - Usuario del sistema con roles (Administrador, Operador, Veterinario) y auditoría.

Otros modelos (planeados/definidos): Alimentacion, ControlPeso, Enfermedad, Tratamiento, Reproduccion, Parto, Mortalidad, InventarioAlimento.

---

## 8. Capas y responsabilidades

- Domain: Entidades y reglas puras del dominio.
- Application: Casos de uso, DTOs, servicios (orquestación), validadores y contratos de repositorios.
- Infrastructure: Persistencia (EF Core), implementaciones de repositorios, Identity y adaptadores externos.
- Web: Controladores, vistas, ViewModels, autenticación/autorization, Program.cs y middleware.

---

## 9. Repositorios y patrones

- Se usa un BaseRepository<T> genérico con operaciones CRUD comunes.
- Repositorios específicos (CuyRepository, VentaRepository, UsuarioRepository) exponen consultas especializadas (por código, por fecha, conteos, etc.).
- Los contratos de repositorio están definidos en Application.Interfaces.Repositories para evitar dependencias de Application hacia Infrastructure.

---

## 10. Servicios de aplicación

- CuyService
  - Valida entradas con CuyValidator.
  - Usa ICuyRepository para persistencia.
  - Mapea entre DTOs y entidades con MappingProfile (mapeo manual).

- VentaService
  - Valida ventas con VentaValidator.
  - Opera con IVentaRepository para consultas y totales.

- UsuarioService, ReportService
  - Interfaces y stubs creados; completar lógica usando repositorios.

---

## 11. Identity y roles

- Identity configurado con ApplicationUser y IdentityRole<int>.
- Seed de roles (Administrador, Operador, Veterinario) ejecutado al arranque.
- Recomendación: no usar contraseñas en claro en producción; utilizar entorno y secretos para credenciales iniciales.

---

## 12. Mapeos y validaciones

- MappingProfile contiene métodos manuales de mapeo DTO ↔ Entity (evita dependencia a AutoMapper en v1).
- Validadores (CuyValidator, VentaValidator, etc.) realizan validación de negocio y retornan listas de errores.

---

## 13. UI

- Aplicación web basada en Razor Views (MVC): vistas para CRUD de Cuy y Jaula.
- Layout con navegación y controles de autenticación (login/logout).
- Se recomienda añadir:
  - Toastr u otra librería para mensajes de usuario.
  - Validación cliente con unobtrusive validation.
  - Páginas de administración para gestión de roles y usuarios.

---

## 14. Endpoints relevantes (resumen)

- /Cuy
  - GET /Cuy (Index)
  - GET /Cuy/Create
  - POST /Cuy/Create
  - GET /Cuy/Details/{id}
  - GET /Cuy/Edit/{id}
  - POST /Cuy/Edit/{id}
  - POST /Cuy/Delete/{id}

- /Jaula
  - GET /Jaula (Index)
  - GET /Jaula/Create
  - POST /Jaula/Create
  - GET /Jaula/Edit/{id}
  - POST /Jaula/Edit/{id}
  - POST /Jaula/Delete/{id}

- Account (Login/Register/Logout)

- API/Reportes (por implementar)

---

## 15. Testing

- No hay tests unitarios/integración incluidos en v1.
- Recomendación: crear proyecto CuyControl.Tests y añadir pruebas para:
  - Servicios (CuyService, VentaService) usando Moq para simular repositorios.
  - Repositorios con EF Core InMemory o Sqlite en memoria para pruebas de integración.

---

## 16. CI/CD y despliegue

Sugerencia de pipeline básico (GitHub Actions / Azure DevOps):

- pasos:
  - build: dotnet build
  - test: dotnet test
  - publish: dotnet publish -c Release
  - despliegue: Azure App Service / IIS / Docker

Docker:
- Crear Dockerfile en el proyecto web y publicar imagen a ACR o DockerHub.

---

## 17. Seguridad y operación

- Revisar y endurecer Identity (políticas de contraseña, bloqueo de cuenta, 2FA si procede).
- Usar HTTPS obligatorio y HSTS (ya configurado en Program.cs para producción).
- Configurar logging centralizado (Serilog) y monitorización (Application Insights).
- Gestionar secretos y connection strings con User Secrets o Azure Key Vault.

---

## 18. Roadmap y próximas versiones (v1.x → v2)

Prioridad alta (v1.1):
- Completar CRUDs restantes (Alimentación, Sanidad, Reproducción, Partos, Mortalidad).
- Implementar exportes (Excel/PDF) para ventas e inventario.
- Añadir tests unitarios.

Prioridad media (v1.2):
- Dashboard interactivo (Chart.js) con métricas de población, ventas, mortalidad y peso promedio.
- Reportes programados y filtrables.

Prioridad baja (v2):
- Integración con dispositivos IoT (sensores), análisis ML básico para predicción de crecimiento.
- Multi-tenant / roles avanzados y control de acceso por finca.

---

## 19. Contribuir

- Fork y pull requests.
- Seguir convenciones: formato de C# (dotnet format), pruebas para nuevas funcionalidades.
- Abrir issues para bugs y features.

---

## 20. Licencia

- Definir licencia del proyecto (por defecto MIT si no indicado).

---

## 21. CHANGELOG — v1

- v1 (2026-05-25): Estructura inicial, entidades principales, repositorios, servicios Cuy y Venta, Identity con roles, CRUD Cuy/Jaula, vistas Razor, DI y seed de roles.

---

Si deseas, puedo:
- Añadir ejemplos concretos de comandos dotnet ef para tu solución actual.
- Generar plantillas de tests (xUnit) para CuyService y VentaService.
- Implementar dashboard mínimo con Chart.js y endpoints API.

Indica la siguiente tarea y la implemento.