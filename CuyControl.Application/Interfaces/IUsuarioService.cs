using CuyControl.Application.DTOs;

namespace CuyControl.Application.Interfaces;

/// <summary>
/// Interface de servicio para operaciones con usuarios.
/// </summary>
public interface IUsuarioService
{
    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <returns>Lista de DTOs de usuarios.</returns>
    Task<IEnumerable<UsuarioDto>> ObtenerTodosUsuariosAsync();

    /// <summary>
    /// Obtiene un usuario por su identificador.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <returns>DTO del usuario o null si no existe.</returns>
    Task<UsuarioDto?> ObtenerUsuarioPorIdAsync(int id);

    /// <summary>
    /// Obtiene un usuario por nombre de usuario.
    /// </summary>
    /// <param name="nombreUsuario">Nombre de usuario.</param>
    /// <returns>DTO del usuario o null si no existe.</returns>
    Task<UsuarioDto?> ObtenerUsuarioPorNombreAsync(string nombreUsuario);

    /// <summary>
    /// Obtiene usuarios activos.
    /// </summary>
    /// <returns>Lista de DTOs de usuarios activos.</returns>
    Task<IEnumerable<UsuarioDto>> ObtenerUsuariosActivosAsync();

    /// <summary>
    /// Crea un nuevo usuario.
    /// </summary>
    /// <param name="usuarioDto">DTO del usuario a crear.</param>
    /// <returns>DTO del usuario creado.</returns>
    Task<UsuarioDto> CrearUsuarioAsync(UsuarioDto usuarioDto);

    /// <summary>
    /// Actualiza un usuario existente.
    /// </summary>
    /// <param name="id">Identificador del usuario.</param>
    /// <param name="usuarioDto">DTO con datos actualizados.</param>
    /// <returns>DTO del usuario actualizado.</returns>
    Task<UsuarioDto> ActualizarUsuarioAsync(int id, UsuarioDto usuarioDto);

    /// <summary>
    /// Elimina un usuario.
    /// </summary>
    /// <param name="id">Identificador del usuario a eliminar.</param>
    /// <returns>True si se eliminó exitosamente.</returns>
    Task<bool> EliminarUsuarioAsync(int id);

    /// <summary>
    /// Verifica las credenciales de un usuario.
    /// </summary>
    /// <param name="nombreUsuario">Nombre de usuario.</param>
    /// <param name="contrasena">Contraseña sin encriptar.</param>
    /// <returns>DTO del usuario si las credenciales son válidas, null en caso contrario.</returns>
    Task<UsuarioDto?> VerificarCredencialesAsync(string nombreUsuario, string contrasena);

    /// <summary>
    /// Cambia la contraseña de un usuario.
    /// </summary>
    /// <param name="usuarioId">Identificador del usuario.</param>
    /// <param name="contraseñaActual">Contraseña actual.</param>
    /// <param name="nuevaContrasena">Nueva contraseña.</param>
    /// <returns>True si se cambió exitosamente.</returns>
    Task<bool> CambiarContrasenaAsync(int usuarioId, string contraseñaActual, string nuevaContrasena);
}
