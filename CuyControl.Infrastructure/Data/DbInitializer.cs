namespace CuyControl.Infrastructure.Data;

/// <summary>
/// Clase para inicializar la base de datos con datos de prueba.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Inicializa la base de datos ejecutando las migraciones pending.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // Aplicar migraciones pendientes
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Siembra datos iniciales en la base de datos.
    /// </summary>
    /// <param name="context">Contexto de base de datos.</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Verificar si ya hay datos
        if (context.Usuarios.Any())
            return;

        // TODO: Implementar siembra de datos iniciales
        await Task.Delay(0);
    }
}
