using CuyControl.Application.Interfaces;
using CuyControl.Infrastructure.Data;
using CuyControl.Infrastructure.Identity;
using CuyControl.Application.Services;
using CuyControl.Application.Interfaces.Repositories;
using CuyControl.Application.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// Servicios de aplicación
builder.Services.AddScoped<ICuyService, CuyControl.Application.Services.CuyService>();
builder.Services.AddScoped<IVentaService, CuyControl.Application.Services.VentaService>();
builder.Services.AddScoped<IUsuarioService, CuyControl.Application.Services.UsuarioService>();
builder.Services.AddScoped<IReportService, CuyControl.Application.Services.ReportService>();

// Validadores
builder.Services.AddScoped<CuyValidator>();
builder.Services.AddScoped<VentaValidator>();
// Servicios adicionales
builder.Services.AddScoped<IAlimentacionService, CuyControl.Application.Services.AlimentacionService>();

// Repositorios (implementación concreta en Infrastructure)
builder.Services.AddScoped<ICuyRepository, CuyControl.Infrastructure.Data.CuyRepository>();
builder.Services.AddScoped<IVentaRepository, CuyControl.Infrastructure.Data.VentaRepository>();
builder.Services.AddScoped<IUsuarioRepository, CuyControl.Infrastructure.Data.UsuarioRepository>();
builder.Services.AddScoped<IAlimentacionRepository, CuyControl.Infrastructure.Repositories.AlimentacionRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    // Seed de identidad (roles y usuarios por defecto) - entorno de desarrollo
    var isDevelopment = app.Environment.IsDevelopment();
    await IdentitySeeder.SeedAsync(userManager, roleManager, builder.Configuration, isDevelopment);
}

app.Run();
