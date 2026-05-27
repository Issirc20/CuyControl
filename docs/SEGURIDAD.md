# Guía de Seguridad

## Autenticación

- Sistema de autenticación basado en ASP.NET Identity con JWT.
- Contraseñas hasheadas con PBKDF2.
- Sesiones seguras con cookies HttpOnly.

## Autorización

- Implementación de roles: Administrador, Operador, Veterinario.
- Decorador [Authorize(Roles = "...")] en controladores.
- Validación en lado servidor de permisos.

## Controles de Entrada

- Validación de modelos en DTOs.
- FluentValidation para reglas complejas.
- Sanitización de entrada en vistas.

## Protección contra Ataques Comunes

### CSRF (Cross-Site Request Forgery)
- Token AntiForgery en todos los formularios POST/PUT/DELETE.
- Validación [ValidateAntiForgeryToken].

### XSS (Cross-Site Scripting)
- Encoded output en Razor (@Html.Encode).
- Content Security Policy headers (futura mejora).

### SQL Injection
- Uso de EF Core con LINQ (parameterizado automáticamente).
- No se acepta SQL raw sin validación.

### Acceso a Datos Sensibles
- Los reportes filtran datos según rol de usuario.
- Auditoria de modificaciones en registros.

## Encriptación

- Conexión HTTPS en producción.
- Contraseñas almacenadas hasheadas.
- Tokens JWT firmados.

## Auditoría

- Registro de login/logout.
- Timestamp en modificaciones (FechaCreacion, FechaModificacion).
- UsuarioCreacionId, UsuarioModificacionId para rastrear cambios.

## Recomendaciones

1. Usar HTTPS en producción.
2. Actualizar dependencias regularmente (dotnet list package --outdated).
3. Ejecutar análisis de vulnerabilidades (dotnet package vulnerability scanner).
4. No exponer stack traces en producción.
5. Implementar rate limiting en endpoints críticos.
6. Configurar CORS adecuadamente.

## Gestión de Secretos

- No hardcodear credenciales.
- Usar User Secrets en desarrollo.
- Usar variables de entorno en producción (Azure Key Vault, etc.).

