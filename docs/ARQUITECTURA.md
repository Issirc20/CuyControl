# Arquitectura

El proyecto CuyControl sigue una arquitectura en capas:

- Domain: Entidades, enums e interfaces del dominio.
- Application: DTOs, servicios de aplicación, validadores y mapeos.
- Infrastructure: Persistencia (EF Core), repositorios, identidad y servicios externos.
- Web: Interfaz (Razor/Controllers/Views), ViewModels y recursos estáticos.

Patrones:
- Repository para acceso a datos.
- Services para lógica de aplicación.
- Identity para autenticación y autorización.
