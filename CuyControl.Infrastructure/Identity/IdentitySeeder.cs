using CuyControl.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CuyControl.Infrastructure.Data;

/// <summary>
/// Clase para inicializar datos de identidad en la base de datos.
/// Lee las credenciales desde la configuración (User Secrets / appsettings.Development) para evitar hardcoded passwords.
/// </summary>
public class IdentitySeeder
{
    /// <summary>
    /// Siembra datos iniciales de identidad.
    /// </summary>
    /// <param name="userManager">Gestor de usuarios.</param>
    /// <param name="roleManager">Gestor de roles.</param>
    /// <param name="configuration">Configuración de la aplicación (usada para leer credenciales de seed).</param>
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<int>> roleManager, IConfiguration configuration, bool isDevelopment)
    {
        // Crear roles si no existen
        var roles = new[] { "Administrador", "Operador", "Veterinario" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
            }
        }

        // Función auxiliar para crear usuario desde configuración
        async Task CreateUserIfConfigured(string sectionKey, string roleName)
        {
            var userName = configuration[$"Seed:{sectionKey}:UserName"];
            var email = configuration[$"Seed:{sectionKey}:Email"];
            var password = configuration[$"Seed:{sectionKey}:Password"];
            var fullName = configuration[$"Seed:{sectionKey}:FullName"];
            var tipoUsuarioValue = configuration[$"Seed:{sectionKey}:TipoUsuario"];
            int tipoUsuario = 0;
            if (!string.IsNullOrWhiteSpace(tipoUsuarioValue) && int.TryParse(tipoUsuarioValue, out var t))
                tipoUsuario = t;

            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                // No crear usuario si no hay credenciales en configuración
                return;
            }

            var existing = await userManager.FindByNameAsync(userName);
            if (existing != null)
                return;

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = string.IsNullOrWhiteSpace(email) ? userName + "@example.com" : email,
                EmailConfirmed = true,
                FullName = string.IsNullOrWhiteSpace(fullName) ? userName : fullName,
                TipoUsuario = tipoUsuario,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, roleName);
            }
        }

        // Crear usuarios solo si están configurados en User Secrets / appsettings.Development
        await CreateUserIfConfigured("Admin", "Administrador");
        await CreateUserIfConfigured("Operador", "Operador");
        await CreateUserIfConfigured("Veterinario", "Veterinario");

        // Si no existe ningún Administrador en entorno de desarrollo, crear uno por defecto
        var admins = await userManager.GetUsersInRoleAsync("Administrador");
        if ((admins == null || admins.Count == 0) && isDevelopment)
        {
            var defaultUserName = configuration[$"Seed:DefaultAdmin:UserName"] ?? "admin";
            var defaultEmail = configuration[$"Seed:DefaultAdmin:Email"] ?? "admin@local";
            var defaultPassword = configuration[$"Seed:DefaultAdmin:Password"] ?? "Admin123!";

            var existing = await userManager.FindByNameAsync(defaultUserName);
            if (existing == null)
            {
                var user = new ApplicationUser
                {
                    UserName = defaultUserName,
                    Email = defaultEmail,
                    EmailConfirmed = true,
                    FullName = "Administrador por defecto",
                    TipoUsuario = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, defaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrador");
                }
            }
        }
    }
}
